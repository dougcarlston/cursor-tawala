import { CSSProperties, Fragment, ReactNode } from "react";
import { RichContentBlock, RichTextNode } from "@/types/tawala";
import {
  defaultFunctionConfig,
  getFunctionDef,
  type ColumnConfig,
  type FunctionConfig,
} from "@/lib/functionCatalog";
import { requestFunctionPicker } from "@/lib/functionPicker";

interface ItemizationColumn {
  header: string;
  field: string;
}

interface ItemizationNode extends RichTextNode {
  type: "itemizationTable";
  form?: string;
  version?: number;
  columns?: ItemizationColumn[];
  showPrint?: boolean;
  showExport?: boolean;
  conditions?: unknown;
  combinator?: string;
  ["show-print-control"]?: string | boolean;
  ["show-export-control"]?: string | boolean;
}

const ITEMIZATION_TOKEN_LABEL = "MULTIPLE QUESTION LIST";
const CORRELATION_TOKEN_LABEL = "QUESTION CORRELATION TABLE";
const CHOICE_TALLY_TOKEN_LABEL = "RESPONSE BAR GRAPH";
const ITEMIZATION_FUNCTION_ID = "itemization-table";
const CORRELATION_FUNCTION_ID = "question-correlation-table";
const CHOICE_TALLY_FUNCTION_ID = "choice-tally-table";

interface QuestionCorrelationNode extends RichTextNode {
  type: "questionCorrelationTable";
  form?: string;
  questionField?: string;
  displayField?: string;
  preferredField?: string;
}

interface ChoiceTallyNode extends RichTextNode {
  type: "choiceTallyTable";
  form?: string;
  field?: string;
}

function findEditableFunctionTable(
  content: RichContentBlock[],
): ItemizationNode | QuestionCorrelationNode | ChoiceTallyNode | null {
  for (const block of content) {
    for (const node of block.nodes ?? []) {
      if (node.type === "itemizationTable") return node as ItemizationNode;
      if (node.type === "questionCorrelationTable") return node as QuestionCorrelationNode;
      if (node.type === "choiceTallyTable") return node as ChoiceTallyNode;
    }
  }
  return null;
}

function alignmentStyle(align?: string): CSSProperties | undefined {
  return align ? { textAlign: align as CSSProperties["textAlign"] } : undefined;
}

function isFunctionNode(node: RichTextNode) {
  return (
    node.type === "itemizationTable" ||
    node.type === "choiceTallyTable" ||
    node.type === "questionCorrelationTable"
  );
}

function flagToEnum(v: unknown, fallback = "false"): "true" | "false" {
  if (v === true || v === "true" || v === "yes" || v === 1 || v === "1") return "true";
  if (v === false || v === "false" || v === "no" || v === 0 || v === "0") return "false";
  return fallback === "true" ? "true" : "false";
}

function itemizationToConfig(node: ItemizationNode): FunctionConfig {
  const def = getFunctionDef(ITEMIZATION_FUNCTION_ID);
  const cols = node.columns ?? [];
  const base = def ? defaultFunctionConfig(def) : {};
  return {
    ...base,
    "show-print-control": flagToEnum(node.showPrint ?? node["show-print-control"]),
    "show-export-control": flagToEnum(node.showExport ?? node["show-export-control"]),
    numberOfColumns: Math.max(1, cols.length),
    column: cols.map((c) => ({ header: c.header ?? "", contents: c.field ?? "" })),
    "form-name": node.form ?? "",
    conditionsRows: (Array.isArray(node.conditions)
      ? node.conditions
      : [{ field: "", op: "equals", value: "" }]) as FunctionConfig["conditionsRows"],
    conditionsCombinator: node.combinator === "or" ? "or" : "and",
  };
}

function correlationToConfig(node: QuestionCorrelationNode): FunctionConfig {
  const def = getFunctionDef(CORRELATION_FUNCTION_ID);
  const base = def ? defaultFunctionConfig(def) : {};
  return {
    ...base,
    "question-field-name": node.questionField ?? "",
    "display-field-name": node.displayField ?? "",
    "preferred-choice-field-name": node.preferredField ?? "",
    conditionsRows: [{ field: "", op: "equals", value: "" }],
    conditionsCombinator: "and",
  };
}

function choiceTallyToConfig(node: ChoiceTallyNode): FunctionConfig {
  const def = getFunctionDef(CHOICE_TALLY_FUNCTION_ID);
  const base = def ? defaultFunctionConfig(def) : {};
  const field = String(node.field ?? "").trim();
  return {
    ...base,
    field: field.includes("<<") ? field : field ? `<<${field}>>` : "",
    conditionsRows: [{ field: "", op: "equals", value: "" }],
    conditionsCombinator: "and",
  };
}

function patchChoiceTallyFromConfig(
  content: RichContentBlock[],
  config: FunctionConfig,
): RichContentBlock[] {
  return content.map((block) => ({
    ...block,
    nodes: (block.nodes ?? []).map((node) => {
      if (node.type !== "choiceTallyTable") return node;
      let field = String(config.field ?? "").trim();
      if (field.startsWith("<<") && field.endsWith(">>")) field = field.slice(2, -2).trim();
      const formPart = field.includes(":") ? field.split(":")[0] : undefined;
      return {
        ...(node as ChoiceTallyNode),
        field,
        form: formPart || (node as ChoiceTallyNode).form,
      };
    }),
  }));
}

function patchCorrelationFromConfig(
  content: RichContentBlock[],
  config: FunctionConfig,
): RichContentBlock[] {
  return content.map((block) => ({
    ...block,
    nodes: (block.nodes ?? []).map((node) => {
      if (node.type !== "questionCorrelationTable") return node;
      const prev = node as QuestionCorrelationNode;
      const preferred = String(config["preferred-choice-field-name"] ?? "").trim();
      return {
        ...prev,
        questionField: String(config["question-field-name"] ?? "").trim(),
        displayField: String(config["display-field-name"] ?? "").trim(),
        preferredField: preferred || undefined,
      };
    }),
  }));
}

function patchItemizationFromConfig(
  content: RichContentBlock[],
  config: FunctionConfig,
): RichContentBlock[] {
  const cols = (config.column as ColumnConfig[] | undefined) ?? [];
  const n = Number(config.numberOfColumns ?? cols.length) || cols.length;
  const form = String(config["form-name"] ?? "").trim();
  const nextColumns: ItemizationColumn[] = cols.slice(0, n).map((c) => ({
    header: c.header ?? "",
    field: c.contents ?? "",
  }));

  return content.map((block) => ({
    ...block,
    nodes: (block.nodes ?? []).map((node) => {
      if (node.type !== "itemizationTable") return node;
      const prev = node as ItemizationNode;
      return {
        ...prev,
        form: form || prev.form,
        columns: nextColumns,
        showPrint: flagToEnum(config["show-print-control"]) === "true",
        showExport: flagToEnum(config["show-export-control"]) === "true",
        conditions: config.conditionsRows ?? prev.conditions,
        combinator: config.conditionsCombinator === "or" ? "or" : "and",
      };
    }),
  }));
}

function renderFunctionBlock(
  node: RichTextNode,
  key: string,
  style: CSSProperties | undefined,
  onActivate: () => void,
) {
  if (node.type === "itemizationTable") {
    return (
      <button
        key={key}
        type="button"
        className="function-table-badge function-table-standalone function-table-editable"
        style={style}
        title="Click to edit MULTIPLE QUESTION LIST"
        onClick={(e) => {
          e.stopPropagation();
          onActivate();
        }}
      >
        <span className="function-table-label function-table-token">{ITEMIZATION_TOKEN_LABEL}</span>
      </button>
    );
  }

  if (node.type === "questionCorrelationTable") {
    return (
      <button
        key={key}
        type="button"
        className="function-table-badge function-table-standalone function-table-editable"
        style={style}
        title="Click to edit QUESTION CORRELATION TABLE"
        onClick={(e) => {
          e.stopPropagation();
          onActivate();
        }}
      >
        <span className="function-table-label function-table-token">{CORRELATION_TOKEN_LABEL}</span>
      </button>
    );
  }

  if (node.type === "choiceTallyTable") {
    return (
      <button
        key={key}
        type="button"
        className="function-table-badge function-table-standalone function-table-editable"
        style={style}
        title="Click to edit RESPONSE BAR GRAPH"
        onClick={(e) => {
          e.stopPropagation();
          onActivate();
        }}
      >
        <span className="function-table-label function-table-token">{CHOICE_TALLY_TOKEN_LABEL}</span>
      </button>
    );
  }

  return (
    <div key={key} className="function-table-badge" style={style}>
      Function table
    </div>
  );
}

function renderInlineNodes(nodes: RichTextNode[] = [], onActivate: () => void): ReactNode[] {
  return nodes.map((node, index) => {
    const key = `${node.type}-${index}`;
    switch (node.type) {
      case "text":
        return <Fragment key={key}>{node.text ?? ""}</Fragment>;
      case "bold":
        return <strong key={key}>{renderInlineNodes(node.nodes ?? [], onActivate)}</strong>;
      case "italic":
        return <em key={key}>{renderInlineNodes(node.nodes ?? [], onActivate)}</em>;
      case "underline":
        return <u key={key}>{renderInlineNodes(node.nodes ?? [], onActivate)}</u>;
      case "font": {
        const style: CSSProperties = {};
        if (node.face) style.fontFamily = node.face;
        if (node.size) style.fontSize = `${node.size / 20}pt`;
        if (node.color) style.color = `#${String(node.color).replace(/^#/, "")}`;
        return (
          <span key={key} style={style}>
            {renderInlineNodes(node.nodes ?? [], onActivate)}
          </span>
        );
      }
      case "itemizationTable":
        return (
          <button
            key={key}
            type="button"
            className="function-table-inline function-table-token function-table-editable"
            title="Click to edit MULTIPLE QUESTION LIST"
            onClick={(e) => {
              e.stopPropagation();
              onActivate();
            }}
          >
            {ITEMIZATION_TOKEN_LABEL}
          </button>
        );
      case "questionCorrelationTable":
        return (
          <button
            key={key}
            type="button"
            className="function-table-inline function-table-token function-table-editable"
            title="Click to edit QUESTION CORRELATION TABLE"
            onClick={(e) => {
              e.stopPropagation();
              onActivate();
            }}
          >
            {CORRELATION_TOKEN_LABEL}
          </button>
        );
      case "choiceTallyTable":
        return (
          <button
            key={key}
            type="button"
            className="function-table-inline function-table-token function-table-editable"
            title="Click to edit RESPONSE BAR GRAPH"
            onClick={(e) => {
              e.stopPropagation();
              onActivate();
            }}
          >
            {CHOICE_TALLY_TOKEN_LABEL}
          </button>
        );
      default:
        return <Fragment key={key}>{renderInlineNodes(node.nodes ?? [], onActivate)}</Fragment>;
    }
  });
}

interface Props {
  content: RichContentBlock[];
  onChange?: (content: RichContentBlock[]) => void;
}

/** Design-canvas badge for embedded function tables (legacy Designer naming). */
export function FunctionTableBadge({ content, onChange }: Props) {
  const table = findEditableFunctionTable(content);

  const openFunctionEditor = () => {
    if (!table || !onChange) return;
    if (table.type === "itemizationTable") {
      const def = getFunctionDef(ITEMIZATION_FUNCTION_ID);
      if (!def) return;
      requestFunctionPicker({
        mode: "edit",
        existing: {
          element: document.createElement("span"),
          functionId: ITEMIZATION_FUNCTION_ID,
          config: itemizationToConfig(table),
          instanceId: 0,
        },
        commitConfig: (_def, config) => {
          onChange(patchItemizationFromConfig(content, config));
        },
      });
      return;
    }

    if (table.type === "choiceTallyTable") {
      const def = getFunctionDef(CHOICE_TALLY_FUNCTION_ID);
      if (!def) return;
      requestFunctionPicker({
        mode: "edit",
        existing: {
          element: document.createElement("span"),
          functionId: CHOICE_TALLY_FUNCTION_ID,
          config: choiceTallyToConfig(table),
          instanceId: 0,
        },
        commitConfig: (_def, config) => {
          onChange(patchChoiceTallyFromConfig(content, config));
        },
      });
      return;
    }

    const def = getFunctionDef(CORRELATION_FUNCTION_ID);
    if (!def) return;
    requestFunctionPicker({
      mode: "edit",
      existing: {
        element: document.createElement("span"),
        functionId: CORRELATION_FUNCTION_ID,
        config: correlationToConfig(table),
        instanceId: 0,
      },
      commitConfig: (_def, config) => {
        onChange(patchCorrelationFromConfig(content, config));
      },
    });
  };

  if (
    !table ||
    (table.type !== "itemizationTable" &&
      table.type !== "questionCorrelationTable" &&
      table.type !== "choiceTallyTable")
  ) {
    return <div className="function-table-badge">Function table</div>;
  }

  return (
    <div className="function-table-preview">
      {content.map((block, blockIndex) => {
        if (block.type === "paragraph") {
          const nodes = block.nodes ?? [];
          const style = alignmentStyle(block.align);
          if (nodes.some(isFunctionNode)) {
            let segmentIndex = 0;
            const pieces: ReactNode[] = [];
            let inlineNodes: RichTextNode[] = [];

            const flushInlineNodes = () => {
              if (!inlineNodes.length) return;
              pieces.push(
                <p
                  key={`${blockIndex}-text-${segmentIndex}`}
                  className="text-block structured-text-preview"
                  style={style}
                >
                  {renderInlineNodes(inlineNodes, openFunctionEditor)}
                </p>,
              );
              inlineNodes = [];
              segmentIndex += 1;
            };

            for (const node of nodes) {
              if (isFunctionNode(node)) {
                flushInlineNodes();
                pieces.push(
                  renderFunctionBlock(
                    node,
                    `${blockIndex}-fn-${segmentIndex}`,
                    style,
                    openFunctionEditor,
                  ),
                );
                segmentIndex += 1;
              } else {
                inlineNodes.push(node);
              }
            }

            flushInlineNodes();
            return <Fragment key={blockIndex}>{pieces}</Fragment>;
          }

          return (
            <p key={blockIndex} className="text-block structured-text-preview" style={style}>
              {renderInlineNodes(nodes, openFunctionEditor)}
            </p>
          );
        }

        if (block.type === "text") {
          return (
            <p key={blockIndex} className="text-block structured-text-preview">
              {block.text ?? ""}
            </p>
          );
        }

        return null;
      })}
    </div>
  );
}
