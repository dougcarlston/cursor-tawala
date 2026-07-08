/** JSON project definition schema (format 2.0) — mirrors TAWALA_XML_TO_JSON_MAPPING.md */

export type FormItemType =
  | "heading"
  | "text"
  | "fib"
  | "mc"
  | "field"
  | "break"
  | "skipInstructions";

/**
 * A blank's validation function (legacy `Blank.ValidationFunction`). `type` is the metadata id
 * (see `FIB_VALIDATORS` in `lib/fibBlanks.ts`); `errorMessage` is required for all validators
 * except Proper Name; `lowerLimit` / `upperLimit` apply to Integer only.
 */
export interface BlankValidation {
  type: string;
  errorMessage?: string;
  lowerLimit?: string;
  upperLimit?: string;
}

export interface TawalaBlank {
  name: string;
  length?: number;
  required?: boolean;
  /** Multi-line blank height in lines (legacy Blank.Height; default 1). */
  height?: number;
  /** Validation function for this blank (legacy ValidationFunction). */
  validation?: BlankValidation;
  /** Internal field name for Processes / Fields palette (legacy alternateLabel). */
  alternateLabel?: string;
  /** Label shown above the input on the live form (topLabels and similar). */
  displayLabel?: string;
}

export interface TawalaChoice {
  name: string;
  text: string;
}

export interface FormItemBase {
  type: FormItemType;
  label: string;
  alternateLabel?: string;
  style?: string;
}

export interface HeadingItem extends FormItemBase {
  type: "heading";
  /**
   * Legacy whole-item size (Main/Sub). Kept only for backward-compat / migration:
   * older headings stored one size for the whole box here. New headings encode size
   * per run inside `content` markup (see below) and leave `level` unset. When both are
   * absent, size defaults to Main. See `DESIGNER_FORM_ITEMS_HEADING.md`.
   */
  level?: "main" | "sub";
  /**
   * Heading text. May be plain text (legacy) or minimal inline markup with per-run
   * size: `<span class="heading-size-sub">…</span>` / `<span class="heading-size-main">…</span>`.
   * Bare (unwrapped) text renders at Main size. Only these two size classes are used —
   * no bold/italic/color (that is the Text item). Rendered per-selection in
   * `HeadingCanvasRow`.
   */
  content?: string;
}

export interface TextItem extends FormItemBase {
  type: "text";
  content?: string | RichContentBlock[];
}

export interface FibItem extends FormItemBase {
  type: "fib";
  prompt?: string;
  blanks?: TawalaBlank[];
}

export interface McItem extends FormItemBase {
  type: "mc";
  onlyone?: boolean;
  required?: boolean;
  choices?: TawalaChoice[];
  question?: string;
  /** Stored field name for <<…>> references (exported as alternateLabel). */
  name?: string;
}

export interface FieldItem extends FormItemBase {
  type: "field";
  fieldName?: string;
  /** Stored field name in JSON projects (DirtBowl uses `name`). */
  name?: string;
}

export interface BreakItem extends FormItemBase {
  type: "break";
}

export interface SkipCommand {
  cmd: string;
  [key: string]: unknown;
}

export interface SkipInstructionsItem extends FormItemBase {
  type: "skipInstructions";
  commands?: SkipCommand[];
}

export type FormItem =
  | HeadingItem
  | TextItem
  | FibItem
  | McItem
  | FieldItem
  | BreakItem
  | SkipInstructionsItem;

export interface TawalaForm {
  name: string;
  startPoint?: boolean;
  /** Legacy "Block Back Button" flag (C# IForm.BlockBackButton). */
  blockBackButton?: boolean;
  process?: string;
  preProcess?: string;
  themePath?: string;
  dataEntryOnly?: boolean;
  dataSourceName?: string;
  items: FormItem[];
}

export interface TawalaDocument {
  name: string;
  content?: string | RichContentBlock[];
}

export interface RichTextNode {
  type: string;
  text?: string;
  name?: string;
  field?: string;
  face?: string;
  size?: number;
  color?: string;
  nodes?: RichTextNode[];
}

export interface RichContentBlock {
  type: string;
  align?: string;
  text?: string;
  nodes?: RichTextNode[];
  rows?: { cells: { width?: number; content?: RichContentBlock[] }[] }[];
}

export interface TawalaProcessCommand {
  cmd: string;
  [key: string]: unknown;
}

export interface TawalaProcess {
  name: string;
  commands: TawalaProcessCommand[];
}

export interface TawalaProject {
  name: string;
  format: string;
  themePath?: string;
  forms: TawalaForm[];
  processes?: TawalaProcess[];
  documents?: TawalaDocument[];
}

export type EditorTab = "design" | "preview";

export type ProjectNodeKind = "forms" | "processes" | "documents" | "form" | "process" | "document";

export interface Selection {
  kind: ProjectNodeKind;
  name?: string;
}

export const FORM_ITEM_PALETTE: { label: string; type: FormItemType; tag: string }[] = [
  { label: "Heading", type: "heading", tag: "HeadingItem" },
  { label: "Text", type: "text", tag: "TextItem" },
  { label: "Fill in the blank", type: "fib", tag: "FibItem" },
  { label: "Multiple Choice", type: "mc", tag: "McqItem" },
  { label: "Hidden Field", type: "field", tag: "FieldItem" },
  { label: "Page Break", type: "break", tag: "BreakItem" },
  { label: "Skip Instructions", type: "skipInstructions", tag: "SkipItem" },
];

/**
 * Legacy default heading text (`Resources.HeadingItemDefaultRTF`). Inserted selected
 * so the designer can type over it immediately (see `HeadingCanvasRow` select-all).
 */
export const HEADING_PLACEHOLDER = "[Replace this with heading of your own.]";

/**
 * Legacy default text-item body (`Resources.TextItemDefaultRTF`). Inserted selected so the
 * designer types over it immediately (see `TextCanvasRow` select-all), mirroring Heading.
 */
export const TEXT_PLACEHOLDER = "[Replace this with text of your own.]";

/**
 * Legacy default FIB prompt (`Resources.FibItemDefaultRTF`). Inserted selected so the designer
 * can type over it immediately (see `FibCanvasRow` select-all).
 */
export const FIB_PLACEHOLDER = "[Replace this with your question. Underscores create blanks.]";

export function createDefaultItem(type: FormItemType, label: string): FormItem {
  switch (type) {
    case "heading":
      return { type, label, level: "main", content: HEADING_PLACEHOLDER };
    case "text":
      return { type, label, content: TEXT_PLACEHOLDER };
    case "fib":
      return {
        type,
        label,
        prompt: FIB_PLACEHOLDER,
        blanks: [{ name: "a", length: 10, height: 1, alternateLabel: `${label}:a` }],
      };
    case "mc":
      return {
        type,
        label,
        onlyone: true,
        question: "[Replace with your question.]",
        choices: [
          { name: "a", text: "Choice 1" },
          { name: "b", text: "Choice 2" },
        ],
      };
    case "field":
      return { type, label, fieldName: "FieldName" };
    case "break":
      return { type, label };
    case "skipInstructions":
      return { type, label, commands: [] };
    default:
      return { type: "text", label, content: "" };
  }
}

export function emptyProject(): TawalaProject {
  return {
    name: "Untitled",
    format: "2.0",
    themePath: "default",
    forms: [
      {
        name: "Form 1",
        startPoint: true,
        items: [],
      },
    ],
    processes: [],
    documents: [],
  };
}
