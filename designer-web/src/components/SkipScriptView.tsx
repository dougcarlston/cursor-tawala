import { parentInsertPath } from "@/lib/skipInsertPath";
import type { ScriptLine } from "@/lib/skipScript";
import type { ReactNode } from "react";

function SkipInsertionLine({
  active = false,
  onClick,
}: {
  active?: boolean;
  onClick?: () => void;
}) {
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
  insertAfterIndex: number;
  onSelectInsertPath: (path: string) => void;
}

function isEmptyBlock(lines: ScriptLine[], openIndex: number): boolean {
  const open = lines[openIndex];
  const next = lines[openIndex + 1];
  return (
    open.lineType === "block-open" &&
    next?.lineType === "block-close" &&
    next.indent === open.indent &&
    next.closeZone === open.insertZone
  );
}

function SkipBlockInterior({
  zone,
  active,
  showArrow,
  onSelectInsertPath,
}: {
  zone: string;
  active: boolean;
  showArrow: boolean;
  onSelectInsertPath: (path: string) => void;
}) {
  return (
    <button
      type="button"
      className={`skip-block-interior${active ? " active" : ""}`}
      title="Click to set insertion point inside this block"
      onClick={() => onSelectInsertPath(zone)}
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

/**
 * Structured skip script with clickable insertion zones inside `( )` blocks.
 */
export function SkipScriptView({
  lines,
  insertPath,
  insertAfterIndex,
  onSelectInsertPath,
}: Props) {
  if (lines.length === 0) {
    return (
      <div className="skip-script-area">
        <SkipInsertionLine
          active={insertPath === "root"}
          onClick={() => onSelectInsertPath("root")}
        />
      </div>
    );
  }

  const elements: ReactNode[] = [];

  for (let i = 0; i < lines.length; i++) {
    const line = lines[i];
    const pad = "    ".repeat(line.indent);

    if (line.lineType === "block-open" && line.insertZone) {
      const zone = line.insertZone;
      const active = insertPath === zone;
      const empty = isEmptyBlock(lines, i);

      elements.push(
        <div key={`open-${i}`} className="skip-script-line skip-block-open">
          <span className="skip-script-pad">{pad}</span>
          <span className="skip-block-paren" aria-hidden>
            (
          </span>
        </div>,
      );

      if (empty) {
        elements.push(
          <div key={`interior-${i}`} className="skip-script-line skip-block-interior-row">
            <span className="skip-script-pad">{pad}</span>
            <SkipBlockInterior
              zone={zone}
              active={active}
              showArrow={active}
              onSelectInsertPath={onSelectInsertPath}
            />
          </div>,
        );
        const closeLine = lines[i + 1];
        const closePad = "    ".repeat(closeLine.indent);
        elements.push(
          <div key={`close-${i}`} className="skip-script-line skip-block-close">
            <span className="skip-script-pad">{closePad}</span>
            <span className="skip-block-paren" aria-hidden>
              )
            </span>
          </div>,
        );
        i++;
      }
    } else if (line.lineType === "block-close" && line.closeZone) {
      const zone = line.closeZone;
      const active = insertPath === zone;
      elements.push(
        <div key={`close-${i}`} className="skip-script-line skip-block-close">
          <span className="skip-script-pad">{pad}</span>
          <SkipBlockInterior
            zone={zone}
            active={active}
            showArrow={false}
            onSelectInsertPath={onSelectInsertPath}
          />
          <span className="skip-block-paren" aria-hidden>
            )
          </span>
        </div>,
      );
    } else if (line.lineType === "otherwise" && line.insertZone) {
      const zone = line.insertZone;
      const active = insertPath === zone;
      elements.push(
        <div key={`otherwise-${i}`} className="skip-script-line skip-script-otherwise">
          <span className="skip-script-pad">{pad}</span>
          <button
            type="button"
            className={`skip-otherwise-label${active ? " active" : ""}`}
            title="Click to set insertion point in the Otherwise branch"
            onClick={() => onSelectInsertPath(zone)}
          >
            {line.text}
          </button>
        </div>,
      );
    } else if (line.lineType === "comment") {
      elements.push(
        <button
          key={`line-${i}`}
          type="button"
          className="skip-script-line skip-script-comment"
          onClick={() => onSelectInsertPath(insertPathForCommandLine(line))}
        >
          <span className="skip-script-pad">{pad}</span>
          <span>{line.text}</span>
        </button>,
      );
    } else if (line.path) {
      elements.push(
        <button
          key={`line-${i}`}
          type="button"
          className="skip-script-line skip-script-command"
          onClick={() => onSelectInsertPath(insertPathForCommandLine(line))}
        >
          <span className="skip-script-pad">{pad}</span>
          <span>{line.text}</span>
        </button>,
      );
    } else {
      elements.push(
        <div key={`struct-${i}`} className="skip-script-line skip-script-struct">
          <span className="skip-script-pad">{pad}</span>
          <span>{line.text}</span>
        </div>,
      );
    }

    if (i === insertAfterIndex) {
      const emptyInteriorActive =
        line.lineType === "block-open" &&
        line.insertZone === insertPath &&
        isEmptyBlock(lines, i);
      if (!emptyInteriorActive) {
        elements.push(
          <SkipInsertionLine
            key={`ins-${i}`}
            active
            onClick={() => onSelectInsertPath(insertPath)}
          />,
        );
      }
    }
  }

  if (insertAfterIndex < 0 && insertPath === "root") {
    elements.unshift(
      <SkipInsertionLine
        key="ins-root-start"
        active
        onClick={() => onSelectInsertPath("root")}
      />,
    );
  }

  return <div className="skip-script-area">{elements}</div>;
}

/** After clicking a command line, insert inside that command's parent block. */
function insertPathForCommandLine(line: ScriptLine): string {
  if (!line.path) return "root";
  if (line.lineType === "if-header") return `${line.path}/then`;
  return parentInsertPath(line.path);
}
