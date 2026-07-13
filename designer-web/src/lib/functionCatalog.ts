/**
 * Display-function catalog — mirrors `display-component-repository.xml` and
 * `DESIGNER_INSERT_MENU_AND_FUNCTIONS.md` § Insert → Function….
 */

import { functionConditionsAreValid } from "./functionConditions";
import type { FunctionConditionRow } from "./functionConditions";

export type FunctionParamType =
  | "tawala-form"
  | "tawala-blank"
  | "tawala-mcq"
  | "tawala-contents-field"
  | "expression"
  | "enumeration"
  | "tawala-conditions"
  | "number-of-columns"
  | "column-collection";

export interface FunctionParamChoice {
  value: string;
  description: string;
}

export interface FunctionParamDef {
  id: string;
  type: FunctionParamType;
  name: string;
  description: string;
  required: boolean;
  /** When true, omitted from Configure UI (still in catalog for export / round-trip). */
  hidden?: boolean;
  choices?: FunctionParamChoice[];
  defaultValue?: string;
}

export interface FunctionDef {
  id: string;
  name: string;
  description: string;
  parameters: FunctionParamDef[];
}

export interface FunctionCategory {
  id: string;
  label: string;
  functionIds: string[];
}

/** Serialized parameter values from Configure Function. */
export type FunctionConfig = Record<
  string,
  string | number | ColumnConfig[] | FunctionConditionRow[] | undefined
>;

export interface ColumnConfig {
  header?: string;
  contents: string;
}

const CONDITIONS_PARAM: FunctionParamDef = {
  id: "conditions",
  type: "tawala-conditions",
  name: "Include only the records",
  description:
    "Only the records that match these conditions will be included. Leave blank to include all records from the selected form.",
  required: false,
};

const RANK_PARAM: FunctionParamDef = {
  id: "rank",
  type: "enumeration",
  name: "Rank",
  description: "Ranking of the popular choice (first, second, or third most popular).",
  required: true,
  defaultValue: "1",
  choices: [
    { value: "1", description: "first" },
    { value: "2", description: "second" },
    { value: "3", description: "third" },
  ],
};

const COLUMN_COLLECTION: FunctionParamDef = {
  id: "column",
  type: "column-collection",
  name: "Column Information",
  description: "Information about columns in the table.",
  required: true,
};

export const FUNCTION_CATALOG: FunctionDef[] = [
  {
    id: "categorizer",
    name: "CATEGORIZER",
    description:
      "Provides an interactive (drag and drop) method for grouping stored records into categories.",
    parameters: [
      {
        id: "category-names",
        type: "tawala-blank",
        name: "Category Names",
        description: "Field whose values will be shown as category names in the Target table.",
        required: true,
      },
      {
        id: "category-ids",
        type: "tawala-blank",
        name: "Category IDs",
        description: "Field whose values will be used as category IDs.",
        required: true,
      },
      {
        id: "category-storage-field",
        type: "tawala-blank",
        name: "Category Storage Field",
        description: "Field set to the Category ID when a record is assigned a category.",
        required: true,
      },
      {
        id: "navigate-to",
        type: "tawala-form",
        name: "Show this form on completion",
        description: 'Form shown when the user selects "Save Changes".',
        required: true,
      },
      COLUMN_COLLECTION,
      {
        id: "conditions",
        type: "tawala-conditions",
        name: "Show only fields from records",
        description: "Only fields in records matching these conditions appear in the Source list.",
        required: true,
      },
    ],
  },
  {
    id: "display-image",
    name: "DISPLAY IMAGE",
    description:
      "Shows an image from an http:// or https:// URL. To use an uploaded file, put a File Uploader on a form and drop that field here (File Uploader is greyed in this Designer until implemented — same gap as the Jan 2011 legacy build).",
    parameters: [
      {
        id: "source",
        type: "expression",
        name: "Image Source",
        description:
          "Paste a full image URL (http:// or https://), or drop a field that holds a URL. Desktop file paths and drag-dropping image files are not supported here.",
        required: true,
      },
      {
        id: "width",
        type: "expression",
        name: "Display width (pixels)",
        description:
          "Optional width in whole pixels (e.g. 320 or 640 — not inches). Leave blank for the image’s natural size. Prefer setting width only so height scales and aspect ratio is preserved.",
        required: false,
      },
      {
        id: "height",
        type: "expression",
        name: "Display height (pixels)",
        description:
          "Optional. Leave blank so height scales from width (recommended). Hidden in Configure — set width only to avoid distorting the image.",
        required: false,
        hidden: true,
      },
      {
        id: "alt_title",
        type: "expression",
        name: "Alternative name",
        description: "Hover / accessibility text in browsers.",
        required: false,
      },
    ],
  },
  {
    id: "display-mcq-label",
    name: "DISPLAY MULTIPLE-CHOICE QUESTION RESPONSES",
    description: "Displays the text of the response choice in a multiple-choice question.",
    parameters: [
      {
        id: "field-name",
        type: "tawala-mcq",
        name: "Multiple Choice Field",
        description: "The multiple-choice question whose responses to display.",
        required: true,
      },
      {
        id: "display",
        type: "enumeration",
        name: "Display",
        description: "How responses will be displayed.",
        required: true,
        defaultValue: "label_only",
        choices: [
          { value: "label_only", description: "only labels of selected choices" },
          { value: "all_choices", description: "all choices, using the question layout" },
        ],
      },
    ],
  },
  {
    id: "export-team-roster",
    name: "EXPORT TEAM ROSTER",
    description: "Exports a team roster table (legacy All-category function).",
    parameters: [],
  },
  {
    id: "record-count",
    name: "FORM RECORD COUNT",
    description: "Returns the number of responses received by a specified Form.",
    parameters: [
      {
        id: "form-name",
        type: "tawala-form",
        name: "Form",
        description: "The form whose records you wish to count.",
        required: true,
      },
      {
        id: "conditions",
        type: "tawala-conditions",
        name: "Count only the records",
        description: "Only records matching these conditions are counted.",
        required: true,
      },
    ],
  },
  {
    id: "link-to-project-details",
    name: "LINK TO PROJECT DETAILS IN MY TAWALA",
    description: "Creates a link to navigate to the project details of the current project.",
    parameters: [
      {
        id: "description",
        type: "expression",
        name: "Link text",
        description: "Text of the link.",
        required: true,
      },
      {
        id: "open-preference",
        type: "enumeration",
        name: "Open in",
        description: "Determines how the page will open.",
        required: true,
        defaultValue: "current-window",
        choices: [
          { value: "current-window", description: "the current window" },
          { value: "new-window", description: "a new window" },
        ],
      },
    ],
  },
  {
    id: "itemization-table",
    name: "MULTIPLE QUESTION LIST",
    description: "Displays a table showing a list of responses to specified questions in a Form.",
    parameters: [
      {
        id: "show-print-control",
        type: "enumeration",
        name: "Show link to print the table",
        required: true,
        description: "Show link to print the table",
        defaultValue: "false",
        choices: [
          { value: "false", description: "no" },
          { value: "true", description: "yes" },
        ],
      },
      {
        id: "show-export-control",
        type: "enumeration",
        name: "Show link to export the table to Excel",
        required: true,
        description: "Show link to export the table to Excel",
        defaultValue: "false",
        choices: [
          { value: "false", description: "no" },
          { value: "true", description: "yes" },
        ],
      },
      COLUMN_COLLECTION,
      {
        id: "conditions",
        type: "tawala-conditions",
        name: "Show only fields from records",
        description: "Only fields in records matching these conditions are shown.",
        required: true,
      },
    ],
  },
  {
    id: "paypal-single-item-button",
    name: "PAYPAL SINGLE ITEM PURCHASE BUTTON",
    description:
      "Creates a button to sell a single item, request payment, or receive a donation. Requires Tawala online payment support.",
    parameters: [
      {
        id: "button-type",
        type: "enumeration",
        name: "Type of button",
        required: true,
        description: "Transaction type.",
        defaultValue: "buy",
        choices: [
          { value: "buy", description: "Buy" },
          { value: "buy-logo", description: "Buy (displays credit card logos)" },
          { value: "pay", description: "Pay" },
          { value: "pay-logo", description: "Pay (displays credit card logos)" },
          { value: "donate", description: "Donate" },
          { value: "donate-logo", description: "Donate (displays credit card logos)" },
        ],
      },
      {
        id: "item",
        type: "expression",
        name: "Item/service description",
        required: true,
        description: "Description shown during the payment process.",
      },
      {
        id: "amount",
        type: "expression",
        name: "Transaction amount",
        required: true,
        description: "Field or variable containing the transaction amount.",
      },
      {
        id: "successful-payment-return-form-name",
        type: "tawala-form",
        name: "Return to this form on success",
        required: true,
        description: "Form shown after successful payment.",
      },
      {
        id: "cancelled-payment-return-form-name",
        type: "tawala-form",
        name: "Return to this form on failure to pay",
        required: true,
        description: "Form shown if payment is cancelled.",
      },
      {
        id: "status-field",
        type: "tawala-blank",
        name: "Store the transaction status in this field",
        required: true,
        description: "Updated with transaction status on success.",
      },
      {
        id: "amount-field",
        type: "tawala-blank",
        name: "Store the transaction amount in this field",
        required: true,
        description: "Updated with the amount paid on success.",
      },
      {
        id: "style-option",
        type: "enumeration",
        name: "Select how to style PayPal screens",
        required: true,
        description: "PayPal screen styling.",
        defaultValue: "paypal",
        choices: [
          { value: "paypal", description: "PayPal Style" },
          { value: "tawala", description: "Tawala Theme Style" },
        ],
      },
    ],
  },
  {
    id: "project-email-count",
    name: "PROJECT EMAIL COUNT",
    description: "Count all emails sent for this project.",
    parameters: [],
  },
  {
    id: "question-correlation-table",
    name: "QUESTION CORRELATION TABLE",
    description:
      "Table of MCQ choices per respondent; optional second MCQ for preferred choice.",
    parameters: [
      {
        id: "question-field-name",
        type: "tawala-mcq",
        name: "Question with all choices",
        required: true,
        description: "MCQ whose responses appear in the table.",
      },
      {
        id: "display-field-name",
        type: "tawala-blank",
        name: "Field to display in left column",
        required: true,
        description: 'Record identifier column (often the respondent name).',
      },
      {
        id: "preferred-choice-field-name",
        type: "tawala-mcq",
        name: "Question with preferred choice",
        required: false,
        description: "Optional second MCQ for preferred choice.",
      },
      CONDITIONS_PARAM,
    ],
  },
  {
    id: "popular-choice-correlation-table",
    name: "RANKED MULTIQUESTION RESPONSE LIST",
    description:
      "Most common MCQ response plus associated field list; correlates two MCQs.",
    parameters: [
      RANK_PARAM,
      {
        id: "choice-available-field-name",
        type: "tawala-mcq",
        name: "Main Question",
        required: true,
        description: "MCQ for popular choice information.",
      },
      {
        id: "choice-preferred-field-name",
        type: "tawala-mcq",
        name: "Second Question",
        required: true,
        description: "MCQ to correlate with the main question.",
      },
      {
        id: "popular-choice-display-field-name",
        type: "tawala-blank",
        name: "Column One Contents",
        required: true,
        description: "Field shown in the first column.",
      },
      {
        id: "conditions",
        type: "tawala-conditions",
        name: "Display only choices from records",
        required: true,
        description: "Only matching records are shown.",
      },
    ],
  },
  {
    id: "popular-choice-count",
    name: "RANKED RESPONSE COUNTS",
    description:
      "Returns the number of responses that selected the Nth most popular choice.",
    parameters: [
      RANK_PARAM,
      {
        id: "popular-choice-field-name",
        type: "tawala-mcq",
        name: "Question",
        required: true,
        description: "MCQ for popular choice information.",
      },
      CONDITIONS_PARAM,
    ],
  },
  {
    id: "popular-choice-display",
    name: "RANKED RESPONSE NAME",
    description:
      "Displays the text of the most popular response choice to a specified MCQ.",
    parameters: [
      RANK_PARAM,
      {
        id: "popular-choice-field-name",
        type: "tawala-mcq",
        name: "Question",
        required: true,
        description: "MCQ for popular choice information.",
      },
      {
        id: "conditions",
        type: "tawala-conditions",
        name: "Display only text from records",
        required: true,
        description: "Only matching records are shown.",
      },
    ],
  },
  {
    id: "choice-tally-table",
    name: "RESPONSE BAR GRAPH",
    description:
      "Bar graph showing the number and percentage of each response to an MCQ.",
    parameters: [
      {
        id: "field",
        type: "tawala-mcq",
        name: "Question",
        required: true,
        description: "MCQ whose response counts to show.",
      },
      CONDITIONS_PARAM,
    ],
  },
  {
    id: "response-totals-table",
    name: "RESPONSE TOTALS",
    description: "Table showing the count of each response to a multiple-choice question.",
    parameters: [
      {
        id: "layout-type",
        type: "enumeration",
        name: "Table Layout",
        required: true,
        description: 'Tall = "Choice" and "Count" columns; Wide = rows.',
        defaultValue: "vertical",
        choices: [
          { value: "vertical", description: "Tall" },
          { value: "horizontal", description: "Wide" },
        ],
      },
      {
        id: "field",
        type: "tawala-mcq",
        name: "Question",
        required: true,
        description: "MCQ whose count values to show.",
      },
      CONDITIONS_PARAM,
    ],
  },
  {
    id: "simple-list",
    name: "SINGLE QUESTION LIST",
    description:
      "List showing all responses to a fill-in-the-blank question or hidden field.",
    parameters: [
      {
        id: "simple-list-field",
        type: "tawala-blank",
        name: "Name of question or hidden field",
        required: true,
        description: "Question or hidden field whose values to display.",
      },
      {
        id: "conditions",
        type: "tawala-conditions",
        name: "Show only responses from records",
        required: true,
        description: "Only matching records are shown.",
      },
    ],
  },
  {
    id: "sum",
    name: "SUM",
    description:
      "Calculates the sum of values in a fill-in-the-blank question or hidden field.",
    parameters: [
      {
        id: "field",
        type: "tawala-blank",
        name: "Name of question or hidden field",
        required: true,
        description: "Blank or hidden field whose values to sum.",
      },
      CONDITIONS_PARAM,
    ],
  },
];

export const FUNCTION_CATEGORIES: FunctionCategory[] = [
  {
    id: "all",
    label: "All",
    functionIds: FUNCTION_CATALOG.map((f) => f.id),
  },
  {
    id: "database",
    label: "Database Functions",
    functionIds: ["record-count", "popular-choice-count", "popular-choice-display"],
  },
  {
    id: "math",
    label: "Math Functions",
    functionIds: ["sum"],
  },
  {
    id: "payments",
    label: "Payments",
    functionIds: ["paypal-single-item-button"],
  },
  {
    id: "tables",
    label: "Tables",
    functionIds: [
      "categorizer",
      "itemization-table",
      "choice-tally-table",
      "response-totals-table",
      "popular-choice-correlation-table",
      "simple-list",
      "question-correlation-table",
    ],
  },
];

export function getFunctionDef(id: string): FunctionDef | undefined {
  return FUNCTION_CATALOG.find((f) => f.id === id);
}

export function defaultFunctionConfig(def: FunctionDef): FunctionConfig {
  const config: FunctionConfig = {};
  for (const param of def.parameters) {
    if (param.type === "column-collection") {
      // Legacy Configure starts with a single Column 1 group; + adds more.
      config.numberOfColumns = 1;
      config.column = [{ header: "", contents: "" }];
    } else if (param.type === "tawala-conditions") {
      config.conditionsRows = [{ field: "", op: "equals", value: "" }];
      config.conditionsCombinator = "and";
    } else if (param.defaultValue !== undefined) {
      config[param.id] = param.defaultValue;
    } else {
      config[param.id] = "";
    }
  }
  return config;
}

export function configMeetsRequirements(def: FunctionDef, config: FunctionConfig): boolean {
  for (const param of def.parameters) {
    if (!param.required) continue;
    if (param.type === "column-collection") {
      const cols = config.column as ColumnConfig[] | undefined;
      const n = Number(config.numberOfColumns ?? 0);
      if (!cols || cols.length < n) return false;
      for (let i = 0; i < n; i++) {
        if (!cols[i]?.contents?.trim()) return false;
      }
      continue;
    }
    if (param.type === "tawala-conditions") {
      const rows = config.conditionsRows as FunctionConditionRow[] | undefined;
      if (!rows || !functionConditionsAreValid(rows)) return false;
      continue;
    }
    const val = config[param.id];
    if (val === undefined || String(val).trim() === "") return false;
  }
  // Optional pixel fields (DISPLAY IMAGE width/height): blank or <<field>> or positive integer.
  for (const id of ["width", "height"] as const) {
    if (!(id in config) && !def.parameters.some((p) => p.id === id)) continue;
    if (!def.parameters.some((p) => p.id === id)) continue;
    const raw = String(config[id] ?? "").trim();
    if (!raw) continue;
    if (/^<<[^<>]+>>$/.test(raw)) continue;
    if (!/^\d+$/.test(raw) || Number(raw) < 1 || Number(raw) > 8000) return false;
  }
  return true;
}
