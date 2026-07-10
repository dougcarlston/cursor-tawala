import { useRef, useState, useSyncExternalStore } from "react";
import { useProjectStore } from "@/store/projectStore";
import type { WindowKind } from "@/store/projectStore";
import {
  getActivePaletteEditor,
  getFormattingFocusState,
  isPaletteControlEnabled,
  subscribeFormattingFocus,
  type PaletteControlId,
} from "@/lib/formattingPaletteContext";
import { InsertTableDialog } from "./InsertTableDialog";
import { openFunctionPickerFromEditor } from "@/lib/functionPicker";
import {
  DEFAULT_PALETTE_FONT_FACE,
  DEFAULT_PALETTE_FONT_SIZE_PT,
  defaultFontFaceLabel,
  defaultFontSizeLabel,
} from "@/lib/paletteDefaults";
import {
  FONT_SIZE_PT,
  paletteAlign,
  paletteBold,
  paletteDeleteColumn,
  paletteDeleteRow,
  paletteDeleteTable,
  paletteFontColor,
  paletteFontFace,
  paletteFontSize,
  paletteIndent,
  paletteInsertColumnAfter,
  paletteInsertColumnBefore,
  paletteInsertRowAfter,
  paletteInsertRowBefore,
  paletteInsertTable,
  paletteItalic,
  paletteOutdent,
  paletteReset,
  paletteUnderline,
  getPaletteActiveStateSnapshot,
  subscribePaletteActiveState,
  type PaletteActiveState,
} from "@/lib/paletteCommands";

/** Web-safe font list — `DESIGNER_DOCUMENT_EDITOR.md` § Font Face dropdown. */
const FONT_FACES = [
  DEFAULT_PALETTE_FONT_FACE,
  "Arial Black",
  "Comic Sans MS",
  "Courier New",
  "Georgia",
  "Impact",
  "Tahoma",
  "Times New Roman",
  "Trebuchet MS",
  "Verdana",
] as const;

/** Point sizes — legacy default 12 pt Arial, listed in numeric order. */
const FONT_SIZES = FONT_SIZE_PT.map(String);

const ALIGN_OPTIONS: { value: PaletteActiveState["align"]; label: string; glyph: string }[] = [
  { value: "left", label: "Align Left", glyph: "⯇≡" },
  { value: "center", label: "Align Center", glyph: "≡" },
  { value: "right", label: "Align Right", glyph: "≡⯈" },
  { value: "justify", label: "Justify", glyph: "▤" },
];

interface Props {
  /** Active MDI window kind — palette visible only for form / document. */
  activeKind: WindowKind | null;
}

function PaletteButton({
  id,
  title,
  label,
  enabled,
  active,
  className,
  onClick,
}: {
  id: PaletteControlId;
  title: string;
  label: React.ReactNode;
  enabled: boolean;
  active?: boolean;
  className?: string;
  onClick?: () => void;
}) {
  return (
    <button
      type="button"
      className={`${className ?? ""}${active ? " active" : ""}`}
      title={title}
      disabled={!enabled}
      aria-pressed={active}
      data-palette-control={id}
      // Keep the editor's selection/focus alive so execCommand hits the right range.
      onMouseDown={(e) => e.preventDefault()}
      onClick={onClick}
    >
      {label}
    </button>
  );
}

function PaletteSep() {
  return <span className="formatting-palette-sep" aria-hidden />;
}

/**
 * Row 2 Formatting Palette — shared by Form Text items and Documents.
 * Character/paragraph controls (font face/size, color, reset, B/I/U, indent, alignment) drive
 * the active rich-text editor via `paletteCommands`. Table tools (#11–13) and **fx** (#14) are
 * gated correctly but wired in their own later steps.
 */
export function FormattingPalette({ activeKind }: Props) {
  const focus = useSyncExternalStore(
    subscribeFormattingFocus,
    getFormattingFocusState,
    getFormattingFocusState,
  );
  const editorTab = useProjectStore((s) => s.editorTab);
  const formCount = useProjectStore((s) => s.project.forms.length);
  const [alignMenuOpen, setAlignMenuOpen] = useState(false);
  const [tableMenuOpen, setTableMenuOpen] = useState(false);
  const [insertTableOpen, setInsertTableOpen] = useState(false);
  const colorInputRef = useRef<HTMLInputElement>(null);

  const paletteVisible = activeKind === "form" || activeKind === "document";
  const designActive = activeKind === "document" || editorTab === "design";
  const enabled = (id: PaletteControlId) =>
    paletteVisible ? isPaletteControlEnabled(id, focus, designActive, formCount) : false;

  // Live B/I/U, alignment, font face/size at caret (updates on selectionchange).
  const canFormat = enabled("bold");
  const state = useSyncExternalStore(
    subscribePaletteActiveState,
    () => getPaletteActiveStateSnapshot(canFormat),
    () => null,
  );

  if (!paletteVisible) return null;

  // Indent the palette so its controls line up with the left edge of the MDI canvas.
  // Left columns (fixed widths + 1px right borders): Explorer (.designer-left 220px) and,
  // only when a Form is active, the Items column (.designer-items 150px). Documents have no
  // Items column, so the canvas starts right after Explorer.
  const EXPLORER_WIDTH = 220 + 1;
  const ITEMS_WIDTH = 150 + 1;
  const canvasLeftOffset = activeKind === "form" ? EXPLORER_WIDTH + ITEMS_WIDTH : EXPLORER_WIDTH;

  // Selects and the color picker take focus from the editor, so save its selection first.
  const saveEditorSelection = () => getActivePaletteEditor()?.saveSelection();

  const openColorPicker = () => {
    saveEditorSelection();
    colorInputRef.current?.click();
  };

  const openInsertTableDialog = () => {
    saveEditorSelection();
    setInsertTableOpen(true);
  };

  return (
    <>
    <div
      className="formatting-palette"
      role="toolbar"
      aria-label="Formatting palette"
      title="Formatting palette (row 2)"
      style={{ paddingLeft: canvasLeftOffset }}
    >
      <select
        className="formatting-palette-select"
        title="Font Face"
        disabled={!enabled("fontFace")}
        value={state?.fontFace ?? DEFAULT_PALETTE_FONT_FACE}
        onMouseDown={saveEditorSelection}
        onChange={(e) => paletteFontFace(e.target.value)}
      >
        {FONT_FACES.map((face) => (
          <option key={face} value={face}>
            {defaultFontFaceLabel(face)}
          </option>
        ))}
      </select>

      <select
        className="formatting-palette-select formatting-palette-select-narrow"
        title="Font Point Size"
        disabled={!enabled("fontSize")}
        value={state?.fontSize ?? String(DEFAULT_PALETTE_FONT_SIZE_PT)}
        onMouseDown={saveEditorSelection}
        onChange={(e) => paletteFontSize(e.target.value)}
      >
        {FONT_SIZES.map((size) => (
          <option key={size} value={size}>
            {defaultFontSizeLabel(size)}
          </option>
        ))}
      </select>

      <span className="formatting-palette-split">
        <PaletteButton
          id="fontColor"
          title="Font Color"
          label={<span className="formatting-palette-color-a">A</span>}
          enabled={enabled("fontColor")}
          onClick={openColorPicker}
        />
        <button
          type="button"
          className="formatting-palette-split-arrow"
          title="Font Color"
          disabled={!enabled("fontColor")}
          aria-label="Font color menu"
          onMouseDown={(e) => e.preventDefault()}
          onClick={openColorPicker}
        >
          ▾
        </button>
        <input
          ref={colorInputRef}
          type="color"
          className="formatting-palette-color-input"
          tabIndex={-1}
          aria-hidden
          onChange={(e) => paletteFontColor(e.target.value)}
        />
      </span>

      <PaletteButton
        id="reset"
        title="Reset Formatting"
        label="⌫"
        enabled={enabled("reset")}
        className="formatting-palette-icon"
        onClick={paletteReset}
      />

      <PaletteSep />

      <PaletteButton
        id="bold"
        title="Bold"
        label={<strong>B</strong>}
        enabled={enabled("bold")}
        active={state?.bold}
        onClick={paletteBold}
      />
      <PaletteButton
        id="italic"
        title="Italic"
        label={<em>I</em>}
        enabled={enabled("italic")}
        active={state?.italic}
        onClick={paletteItalic}
      />
      <PaletteButton
        id="underline"
        title="Underline"
        label={<span className="formatting-palette-underline">U</span>}
        enabled={enabled("underline")}
        active={state?.underline}
        onClick={paletteUnderline}
      />

      <PaletteSep />

      <PaletteButton
        id="outdent"
        title="Outdent"
        label="⇤"
        enabled={enabled("outdent")}
        onClick={paletteOutdent}
      />
      <PaletteButton
        id="indent"
        title="Indent"
        label="⇥"
        enabled={enabled("indent")}
        onClick={paletteIndent}
      />

      <span className="formatting-palette-split">
        <PaletteButton
          id="alignment"
          title="Paragraph Alignment"
          label="≡"
          enabled={enabled("alignment")}
          active={state ? state.align !== "left" : false}
          onClick={() => paletteAlign(state?.align === "left" ? "center" : "left")}
        />
        <button
          type="button"
          className="formatting-palette-split-arrow"
          title="Paragraph Alignment"
          disabled={!enabled("alignment")}
          aria-label="Alignment menu"
          onMouseDown={(e) => e.preventDefault()}
          onClick={() => {
            setTableMenuOpen(false);
            setAlignMenuOpen((o) => !o);
          }}
        >
          ▾
        </button>
        {alignMenuOpen && enabled("alignment") && (
          <div className="formatting-palette-menu" role="menu">
            {ALIGN_OPTIONS.map((opt) => (
              <button
                key={opt.value}
                type="button"
                role="menuitemradio"
                aria-checked={state?.align === opt.value}
                className={state?.align === opt.value ? "active" : undefined}
                onMouseDown={(e) => e.preventDefault()}
                onClick={() => {
                  paletteAlign(opt.value);
                  setAlignMenuOpen(false);
                }}
              >
                <span className="formatting-palette-menu-glyph">{opt.glyph}</span> {opt.label}
              </button>
            ))}
          </div>
        )}
      </span>

      <PaletteSep />

      <PaletteButton
        id="insertTable"
        title="Insert Table"
        label="▦"
        enabled={enabled("insertTable")}
        className="formatting-palette-icon"
        onClick={openInsertTableDialog}
      />
      <PaletteButton
        id="deleteTable"
        title="Delete Table"
        label="▦✕"
        enabled={enabled("deleteTable")}
        className="formatting-palette-icon"
        onClick={paletteDeleteTable}
      />

      <span className="formatting-palette-split">
        <PaletteButton
          id="tableRowCol"
          title="Insert or Delete Row or Column"
          label="▦+"
          enabled={enabled("tableRowCol")}
          className="formatting-palette-icon"
        />
        <button
          type="button"
          className="formatting-palette-split-arrow"
          title="Insert or Delete Row or Column"
          disabled={!enabled("tableRowCol")}
          aria-label="Table row or column menu"
          onMouseDown={(e) => e.preventDefault()}
          onClick={() => {
            setAlignMenuOpen(false);
            setTableMenuOpen((o) => !o);
          }}
        >
          ▾
        </button>
        {tableMenuOpen && enabled("tableRowCol") && (
          <div className="formatting-palette-menu" role="menu">
            {(
              [
                ["Insert Column Before", paletteInsertColumnBefore],
                ["Insert Column After", paletteInsertColumnAfter],
                ["Insert Row Before", paletteInsertRowBefore],
                ["Insert Row After", paletteInsertRowAfter],
              ] as const
            ).map(([label, action]) => (
              <button
                key={label}
                type="button"
                role="menuitem"
                onMouseDown={(e) => e.preventDefault()}
                onClick={() => {
                  action();
                  setTableMenuOpen(false);
                }}
              >
                {label}
              </button>
            ))}
            <span className="formatting-palette-menu-sep" aria-hidden />
            {(
              [
                ["Delete Column", paletteDeleteColumn],
                ["Delete Row", paletteDeleteRow],
              ] as const
            ).map(([label, action]) => (
              <button
                key={label}
                type="button"
                role="menuitem"
                onMouseDown={(e) => e.preventDefault()}
                onClick={() => {
                  action();
                  setTableMenuOpen(false);
                }}
              >
                {label}
              </button>
            ))}
          </div>
        )}
      </span>

      <PaletteSep />

      <PaletteButton
        id="fx"
        title="Insert or Edit a Function"
        label="fx"
        enabled={enabled("fx")}
        onClick={() => {
          saveEditorSelection();
          openFunctionPickerFromEditor();
        }}
      />
    </div>
    {insertTableOpen && (
      <InsertTableDialog
        onCancel={() => setInsertTableOpen(false)}
        onInsert={(options) => {
          setInsertTableOpen(false);
          paletteInsertTable(options);
        }}
      />
    )}
    </>
  );
}
