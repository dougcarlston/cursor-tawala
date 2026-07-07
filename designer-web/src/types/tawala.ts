/** JSON project definition schema (format 2.0) — mirrors TAWALA_XML_TO_JSON_MAPPING.md */

export type FormItemType =
  | "heading"
  | "text"
  | "fib"
  | "mc"
  | "field"
  | "break"
  | "skipInstructions";

export interface TawalaBlank {
  name: string;
  length?: number;
  required?: boolean;
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
  level?: "main" | "sub";
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

export function createDefaultItem(type: FormItemType, label: string): FormItem {
  switch (type) {
    case "heading":
      return { type, label, level: "main", content: "New heading" };
    case "text":
      return { type, label, content: "" };
    case "fib":
      return {
        type,
        label,
        prompt: "[Replace with your question. Underscores create blanks.]",
        blanks: [{ name: "a", length: 20 }],
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
