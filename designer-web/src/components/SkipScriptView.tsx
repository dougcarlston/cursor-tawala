import { parentPathAndChildIndex } from "@/lib/skipInsertPath";
import {
  blockSpanForHeader,
  findInsertionLineIndex,
  isEmptyBlock,
  isNonemptyBlockClose,
  type ScriptLine,
} from "@/lib/skipScript";
import type { ReactNode } from "react";

function SkipInsertionLine({
  active = false,
  onClick,
  hitOnly = false,
  insertPath,
  insertIndex,
}: {
  active?: boolean;
  onClick?: () => void;
  /** Zero-height hit target for drag insert (no permanent caret bar). */
  hitOnly?: boolean;
  insertPath?: string;
  insertIndex?: number;
}) {
  if (hitOnly) {
    return (
      <div
        className={`process-insert-hit${active ? " active" : ""}`}
        data-process-insert-path={insertPath}
        data-process-insert-index={insertIndex}
        role="presentation"
        onClick={onClick}
      />
    );
  }
  return (
    <button
      type="button"
      className={`skip-insertion-line${active ? " active" : ""}`}
      title={onClick ? "Click to set insertion point here" : undefined}
      onClick={onClick}
      disabled={!onClick}
      aria-hidden={!onClick ? true : undefined}
    >
      <span className="skip-insertion-arrow">▶</span>
      <span className="skip-insertion-rule" />
    </button>
  );
}

interface Props {
  lines: ScriptLine[];
  insertPath: string;
  insertIndex?: number;
  insertAfterIndex?: number;
  onSelectInsertPath?: (path: string) => void;
  onSelectInsertPoint?: (path: string, index: number) => void;
  selectedCommandPath?: string | null;
  onSelectCommandPath?: (path: string) => void;
  showLineControls?: boolean;
  showAllInsertionGaps?: boolean;
  /**
   * Process drag-insert mode: emit zero-height insert hit targets (no permanent bars).
   * Highlight via highlightInsertPath/Index while dragging.
   */
  insertHitTargets?: boolean;
  highlightInsertPath?: string | null;
  highlightInsertIndex?: number | null;
  onMoveCommand?: (path: string, direction: "up" | "down") => void;
  onDeleteCommand?: (path: string) => void;
  canMoveCommand?: (path: string, direction: "up" | "down") => boolean;
  /** When set, the selected command row can start a reorder drag. */
  onReorderDragStart?: (path: string, dataTransfer: DataTransfer) => void;
}

interface RenderCtx {
  lines: ScriptLine[];
  insertPath: string;
  insertIndex: number;
  indexedMode: boolean;
  hitTargets: boolean;
  highlightInsertPath: string | null;
  highlightInsertIndex: number | null;
  resolvedInsertAfterIndex: number;
  selectedCommandPath: string | null;
  ifBlockSpan: { start: number; end: number } | null;
  /** When rendering inside a shaded If/ForEach block, skip the header's parent-level post gap. */
  shadedHeaderPath: string | null;
  showLineControls: boolean;
  selectPoint: (path: string, index: number) => void;
  onSelectCommandPath?: (path: string) => void;
  onMoveCommand?: (path: string, direction: "up" | "down") => void;
  onDeleteCommand?: (path: string) => void;
  canMoveCommand?: (path: string, direction: "up" | "down") => boolean;
  onReorderDragStart?: (path: string, dataTransfer: DataTransfer) => void;
}

function ScriptCommandLineRow({
  line,
  pad,
  selected,
  lineClassName,
  onSelect,
  showLineControls,
  onMoveCommand,
  onDeleteCommand,
  canMoveCommand,
  onReorderDragStart,
}: {
  line: ScriptLine;
  pad: string;
  selected: boolean;
  lineClassName: string;
  onSelect: () => void;
  showLineControls?: boolean;
  onMoveCommand?: (path: string, direction: "up" | "down") => void;
  onDeleteCommand?: (path: string) => void;
  canMoveCommand?: (path: string, direction: "up" | "down") => boolean;
  onReorderDragStart?: (path: string, dataTransfer: DataTransfer) => void;
}) {
  const path = line.path!;
  const lineButton = (
    <button
      type="button"
      className={`${lineClassName}${selected ? " selected" : ""}`}
      onMouseDown={(e) => {
        // Allow HTML5 reorder drag when selected; otherwise keep focus from jumping.
        if (!(selected && onReorderDragStart)) e.preventDefault();
      }}
      onClick={onSelect}
    >
      <span className="skip-script-pad">{pad}</span>
      <span>{line.text}</span>
    </button>
  );

  if (!showLineControls) {
    return lineButton;
  }

  return (
    <div
      className={`skip-script-line-row${selected ? " selected" : ""}`}
      draggable={selected && !!onReorderDragStart}
      onDragStart={(e) => {
        if (!selected || !onReorderDragStart) {
          e.preventDefault();
          return;
        }
        const target = e.target as HTMLElement;
        if (target.closest("button.skip-script-line-delete, .skip-script-line-toolbar button")) {
          e.preventDefault();
          return;
        }
        onReorderDragStart(path, e.dataTransfer);
      }}
    >
      {lineButton}
      <span className="skip-script-line-toolbar" role="toolbar" aria-label="Statement actions">
        <button
          type="button"
          title="Move up (Alt+↑)"
          disabled={!canMoveCommand?.(path, "up")}
          onClick={() => onMoveCommand?.(path, "up")}
        >
          ↑
        </button>
        <button
          type="button"
          title="Move down (Alt+↓)"
          disabled={!canMoveCommand?.(path, "down")}
          onClick={() => onMoveCommand?.(path, "down")}
        >
          ↓
        </button>
        <button
          type="button"
          className="skip-script-line-delete"
          title="Delete statement"
          onClick={() => onDeleteCommand?.(path)}
        >
          ×
        </button>
      </span>
    </div>
  );
}

function SkipBlockInterior({
  active,
  showArrow,
  onSelect,
}: {
  zone: string;
  active: boolean;
  showArrow: boolean;
  onSelect: () => void;
}) {
  return (
    <button
      type="button"
      className={`skip-block-interior${active ? " active" : ""}`}
      title={`Click to set insertion point inside this block`}
      onClick={onSelect}
    >
      {showArrow ? (
        <>
          <span className="skip-insertion-arrow">▶</span>
          <span className="skip-insertion-rule" />
        </>
      ) : null}
    </button>
  );
}

function emitGap(ctx: RenderCtx, path: string, index: number, key: string): ReactNode {
  if (!ctx.indexedMode && !ctx.hitTargets) return null;
  const storedActive = ctx.insertPath === path && ctx.insertIndex === index;
  const dragActive =
    ctx.highlightInsertPath === path && ctx.highlightInsertIndex === index;
  const active = ctx.hitTargets ? dragActive : storedActive;
  return (
    <SkipInsertionLine
      key={key}
      active={active}
      hitOnly={ctx.hitTargets}
      insertPath={path}
      insertIndex={index}
      onClick={() => ctx.selectPoint(path, index)}
    />
  );
}

function renderLineAt(ctx: RenderCtx, i: number): { nodes: ReactNode[]; nextIndex: number } {
  const line = ctx.lines[i];
  const pad = "    ".repeat(line.indent);
  const nodes: ReactNode[] = [];
  const isIfHeaderLine = ctx.ifBlockSpan != null && i === ctx.ifBlockSpan.start;
  const headerClass = isIfHeaderLine ? " skip-if-block-shade-header" : "";

  if (line.lineType === "block-open" && line.insertZone) {
    const zone = line.insertZone;
    const active = ctx.insertPath === zone && ctx.insertIndex === 0;
    const empty = isEmptyBlock(ctx.lines, i);

    nodes.push(
      <div key={`open-${i}`} className={`skip-script-line skip-block-open${headerClass}`}>
        <span className="skip-script-pad">{pad}</span>
        <span className="skip-block-paren" aria-hidden>
          (
        </span>
      </div>,
    );

    if (empty) {
      nodes.push(
        <div key={`interior-${i}`} className="skip-script-line skip-block-interior-row">
          <span className="skip-script-pad">{pad}</span>
          <SkipBlockInterior
            zone={zone}
            active={active}
            showArrow={active}
            onSelect={() => ctx.selectPoint(zone, 0)}
          />
        </div>,
      );
      const closeLine = ctx.lines[i + 1];
      const closePad = "    ".repeat(closeLine.indent);
      nodes.push(
        <div key={`close-${i}`} className="skip-script-line skip-block-close">
          <span className="skip-script-pad">{closePad}</span>
          <span className="skip-block-paren" aria-hidden>
            )
          </span>
        </div>,
      );
      return { nodes, nextIndex: i + 2 };
    }

    const gap = emitGap(ctx, zone, 0, `gap-${zone}-0`);
    if (gap) nodes.push(gap);
    return { nodes, nextIndex: i + 1 };
  }

  if (line.lineType === "block-close" && line.closeZone) {
    const zone = line.closeZone;
    const active = ctx.insertPath === zone && !ctx.indexedMode;
    const showCloseInterior = !isNonemptyBlockClose(ctx.lines, i);
    nodes.push(
      <div key={`close-${i}`} className="skip-script-line skip-block-close">
        <span className="skip-script-pad">{pad}</span>
        {showCloseInterior ? (
          <SkipBlockInterior
            zone={zone}
            active={active}
            showArrow={false}
            onSelect={() => ctx.selectPoint(zone, 0)}
          />
        ) : null}
        <span className="skip-block-paren" aria-hidden>
          )
        </span>
      </div>,
    );
    return { nodes, nextIndex: i + 1 };
  }

  if (line.lineType === "otherwise" && line.insertZone) {
    const zone = line.insertZone;
    const active = ctx.insertPath === zone && ctx.insertIndex === 0;
    nodes.push(
      <div key={`otherwise-${i}`} className="skip-script-line skip-script-otherwise">
        <span className="skip-script-pad">{pad}</span>
        <button
          type="button"
          className={`skip-otherwise-label${active ? " active" : ""}`}
          title="Click to set insertion point in the Otherwise branch"
          onClick={() => ctx.selectPoint(zone, 0)}
        >
          {line.text}
        </button>
      </div>,
    );
    return { nodes, nextIndex: i + 1 };
  }

  if (line.lineType === "comment" && line.path) {
    const selected = line.path === ctx.selectedCommandPath;
    nodes.push(
      <ScriptCommandLineRow
        key={`line-${i}`}
        line={line}
        pad={pad}
        selected={selected}
        lineClassName={`skip-script-line skip-script-comment${headerClass}`}
        showLineControls={ctx.showLineControls}
        onMoveCommand={ctx.onMoveCommand}
        onDeleteCommand={ctx.onDeleteCommand}
        canMoveCommand={ctx.canMoveCommand}
        onReorderDragStart={ctx.onReorderDragStart}
        onSelect={() => ctx.onSelectCommandPath?.(line.path!)}
      />,
    );
    if (line.path !== ctx.shadedHeaderPath) {
      const { parentPath, childIndex } = parentPathAndChildIndex(line.path);
      const gap = emitGap(ctx, parentPath, childIndex + 1, `gap-after-${line.path}`);
      if (gap) nodes.push(gap);
    }
    return { nodes, nextIndex: i + 1 };
  }

  if (line.path) {
    const selected = line.path === ctx.selectedCommandPath;
    const cmdHeaderClass =
      line.lineType === "if-header" || line.lineType === "foreach-header"
        ? " skip-script-header"
        : "";
    nodes.push(
      <ScriptCommandLineRow
        key={`line-${i}`}
        line={line}
        pad={pad}
        selected={selected}
        lineClassName={`skip-script-line skip-script-command${cmdHeaderClass}${headerClass}`}
        showLineControls={ctx.showLineControls}
        onMoveCommand={ctx.onMoveCommand}
        onDeleteCommand={ctx.onDeleteCommand}
        canMoveCommand={ctx.canMoveCommand}
        onReorderDragStart={ctx.onReorderDragStart}
        onSelect={() => ctx.onSelectCommandPath?.(line.path!)}
      />,
    );
    if (line.path !== ctx.shadedHeaderPath) {
      const { parentPath, childIndex } = parentPathAndChildIndex(line.path);
      const gap = emitGap(ctx, parentPath, childIndex + 1, `gap-after-${line.path}`);
      if (gap) nodes.push(gap);
    }
    return { nodes, nextIndex: i + 1 };
  }

  nodes.push(
    <div key={`struct-${i}`} className="skip-script-line skip-script-struct">
      <span className="skip-script-pad">{pad}</span>
      <span>{line.text}</span>
    </div>,
  );
  return { nodes, nextIndex: i + 1 };
}

function renderWithLegacyArrow(ctx: RenderCtx, i: number, nodes: ReactNode[]) {
  if (!ctx.indexedMode && i === ctx.resolvedInsertAfterIndex) {
    const line = ctx.lines[i];
    const emptyInteriorActive =
      line.lineType === "block-open" &&
      line.insertZone === ctx.insertPath &&
      isEmptyBlock(ctx.lines, i);
    if (!emptyInteriorActive) {
      nodes.push(
        <SkipInsertionLine
          key={`ins-${i}`}
          active
          onClick={() => ctx.selectPoint(ctx.insertPath, ctx.insertIndex)}
        />,
      );
    }
  }
}

/**
 * Structured skip script with clickable insertion zones inside `( )` blocks.
 */
export function SkipScriptView({
  lines,
  insertPath,
  insertIndex = 0,
  insertAfterIndex: legacyInsertAfterIndex,
  onSelectInsertPath,
  onSelectInsertPoint,
  selectedCommandPath = null,
  onSelectCommandPath,
  showLineControls = false,
  showAllInsertionGaps = false,
  insertHitTargets = false,
  highlightInsertPath = null,
  highlightInsertIndex = null,
  onMoveCommand,
  onDeleteCommand,
  canMoveCommand,
  onReorderDragStart,
}: Props) {
  const indexedMode = showAllInsertionGaps && onSelectInsertPoint != null;
  const hitTargets = insertHitTargets && onSelectInsertPoint != null;
  const resolvedInsertAfterIndex =
    legacyInsertAfterIndex ??
    findInsertionLineIndex(lines, insertPath, insertIndex);

  const ifBlockSpan =
    selectedCommandPath != null ? blockSpanForHeader(lines, selectedCommandPath) : null;

  const selectPoint = (path: string, index: number) => {
    if (onSelectInsertPoint) {
      onSelectInsertPoint(path, index);
    } else if (onSelectInsertPath) {
      onSelectInsertPath(path);
    }
  };

  const ctx: RenderCtx = {
    lines,
    insertPath,
    insertIndex,
    indexedMode,
    hitTargets,
    highlightInsertPath,
    highlightInsertIndex,
    resolvedInsertAfterIndex,
    selectedCommandPath,
    ifBlockSpan,
    shadedHeaderPath: null,
    showLineControls,
    selectPoint,
    onSelectCommandPath,
    onMoveCommand,
    onDeleteCommand,
    canMoveCommand,
    onReorderDragStart,
  };

  if (lines.length === 0) {
    return (
      <div className="skip-script-area">
        {hitTargets ? (
          <SkipInsertionLine
            hitOnly
            active={
              highlightInsertPath === "root" && (highlightInsertIndex ?? 0) === 0
            }
            insertPath="root"
            insertIndex={0}
            onClick={() => selectPoint("root", 0)}
          />
        ) : (
          <SkipInsertionLine
            active={insertPath === "root" && insertIndex === 0}
            onClick={() => selectPoint("root", 0)}
          />
        )}
      </div>
    );
  }

  const elements: ReactNode[] = [];

  if (indexedMode || hitTargets) {
    const rootGap = emitGap(ctx, "root", 0, "gap-root-0");
    if (rootGap) elements.push(rootGap);
  }

  let i = 0;
  while (i < lines.length) {
    if (ifBlockSpan && i === ifBlockSpan.start) {
      const headerLine = ctx.lines[ifBlockSpan.start];
      const headerPath = headerLine.path;
      const shadeCtx: RenderCtx = { ...ctx, shadedHeaderPath: headerPath };
      const shadeChildren: ReactNode[] = [];
      let j = ifBlockSpan.start;
      while (j <= ifBlockSpan.end) {
        const { nodes, nextIndex } = renderLineAt(shadeCtx, j);
        shadeChildren.push(...nodes);
        renderWithLegacyArrow(ctx, j, shadeChildren);
        j = nextIndex;
      }
      elements.push(
        <div key={`shade-${ifBlockSpan.start}`} className="skip-if-block-shade">
          {shadeChildren}
        </div>,
      );
      if (headerPath) {
        const { parentPath, childIndex } = parentPathAndChildIndex(headerPath);
        const postBlockGap = emitGap(
          ctx,
          parentPath,
          childIndex + 1,
          `gap-after-block-${headerPath}`,
        );
        if (postBlockGap) elements.push(postBlockGap);
      }
      i = ifBlockSpan.end + 1;
      continue;
    }

    const { nodes, nextIndex } = renderLineAt(ctx, i);
    elements.push(...nodes);
    renderWithLegacyArrow(ctx, i, elements);
    i = nextIndex;
  }

  if (!indexedMode && resolvedInsertAfterIndex < 0 && insertPath === "root") {
    elements.unshift(
      <SkipInsertionLine
        key="ins-root-start"
        active
        onClick={() => selectPoint("root", 0)}
      />,
    );
  }

  return <div className="skip-script-area">{elements}</div>;
}
