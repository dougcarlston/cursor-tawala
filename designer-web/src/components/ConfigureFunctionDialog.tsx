import { useEffect, useMemo, useState, type ReactNode } from "react";
import { useProjectStore } from "@/store/projectStore";
import { FieldTextInput } from "./FieldDropInputs";
import { FunctionConditionsEditor } from "./FunctionConditionsEditor";
import {
  functionConditionsToConfig,
  parseFunctionConditions,
  type FunctionConditionsState,
} from "@/lib/functionConditions";
import {
  configMeetsRequirements,
  defaultFunctionConfig,
  type ColumnConfig,
  type FunctionConfig,
  type FunctionDef,
  type FunctionParamDef,
} from "@/lib/functionCatalog";

interface Props {
  def: FunctionDef;
  initialConfig?: FunctionConfig;
  onCancel: () => void;
  onSave: (config: FunctionConfig) => void;
}

type FieldKey = string;

const MAX_COLUMNS = 12;

function normalizeColumns(config: FunctionConfig): ColumnConfig[] {
  const cols = (config.column as ColumnConfig[] | undefined) ?? [];
  const n = Math.max(1, Number(config.numberOfColumns ?? cols.length) || cols.length || 1);
  const next = cols.slice(0, n).map((c) => ({
    header: c?.header ?? "",
    contents: c?.contents ?? "",
  }));
  while (next.length < n) next.push({ header: "", contents: "" });
  return next;
}

/** Legacy `ConfigureFunctionDialog` — parameter fields + yellow help pane. */
export function ConfigureFunctionDialog({ def, initialConfig, onCancel, onSave }: Props) {
  const forms = useProjectStore((s) => s.project.forms);
  const [config, setConfig] = useState<FunctionConfig>(() => ({
    ...defaultFunctionConfig(def),
    ...initialConfig,
  }));
  const [focused, setFocused] = useState<FieldKey>(def.parameters[0]?.id ?? "summary");
  const [selectedColumn, setSelectedColumn] = useState(0);

  const hasColumnCollection = def.parameters.some((p) => p.type === "column-collection");
  const columns = useMemo(() => normalizeColumns(config), [config]);
  const canSave = useMemo(() => configMeetsRequirements(def, config), [def, config]);

  useEffect(() => {
    setConfig({ ...defaultFunctionConfig(def), ...initialConfig });
    setFocused(def.parameters[0]?.id ?? "summary");
    setSelectedColumn(0);
  }, [def, initialConfig]);

  useEffect(() => {
    if (selectedColumn >= columns.length) {
      setSelectedColumn(Math.max(0, columns.length - 1));
    }
  }, [columns.length, selectedColumn]);

  const help = focusedHelp(def, focused, config);

  const patch = (id: string, value: string) => {
    setConfig((prev) => ({ ...prev, [id]: value }));
  };

  const patchColumns = (cols: ColumnConfig[], selectIndex?: number) => {
    const next = cols.slice(0, MAX_COLUMNS);
    setConfig((prev) => ({
      ...prev,
      numberOfColumns: next.length,
      column: next,
    }));
    if (selectIndex !== undefined) setSelectedColumn(selectIndex);
  };

  const patchConditions = (state: FunctionConditionsState) => {
    setConfig((prev) => ({ ...prev, ...functionConditionsToConfig(state) }) as FunctionConfig);
  };

  const addColumn = () => {
    if (columns.length >= MAX_COLUMNS) return;
    const insertAt = Math.min(selectedColumn + 1, columns.length);
    const next = [...columns];
    next.splice(insertAt, 0, { header: "", contents: "" });
    patchColumns(next, insertAt);
    setFocused(`column-${insertAt}-contents`);
  };

  const removeColumn = () => {
    if (columns.length <= 1) return;
    const next = columns.filter((_, i) => i !== selectedColumn);
    patchColumns(next, Math.min(selectedColumn, next.length - 1));
  };

  const moveColumn = (dir: -1 | 1) => {
    const j = selectedColumn + dir;
    if (j < 0 || j >= columns.length) return;
    const next = [...columns];
    [next[selectedColumn], next[j]] = [next[j], next[selectedColumn]];
    patchColumns(next, j);
  };

  return (
    <div className="modal-overlay" role="presentation">
      <div
        className="modal-dialog fib-validation-dialog configure-function-dialog"
        role="dialog"
        aria-labelledby="configure-function-title"
        aria-modal="true"
      >
        <div className="modal-header">
          <h2 id="configure-function-title">
            <span className="fib-validation-fx">fx</span> Configure Function
          </h2>
          <button type="button" className="modal-close" onClick={onCancel} aria-label="Close">
            ×
          </button>
        </div>
        <div className="fib-validation-body">
          <div className="fib-validation-fields configure-function-fields">
            {def.parameters.length === 0 && (
              <p className="configure-function-empty">
                This function has no parameters. Click OK to insert it.
              </p>
            )}
            {def.parameters.map((param) => (
              <ParamField
                key={param.id}
                param={param}
                config={config}
                columns={columns}
                selectedColumn={selectedColumn}
                forms={forms.map((f) => f.name)}
                focused={focused}
                onFocus={setFocused}
                onSelectColumn={setSelectedColumn}
                onPatch={patch}
                onPatchColumns={patchColumns}
                onPatchConditions={patchConditions}
              />
            ))}
          </div>
          <aside className="fib-validation-help">
            <h3>{def.name}</h3>
            <p>{def.description}</p>
            <h4>{help.title}</h4>
            <p>{help.body}</p>
            {help.compound && <p className="fib-validation-help-note">A compound expression</p>}
          </aside>
        </div>
        <div className="modal-footer fib-validation-footer configure-function-footer">
          {hasColumnCollection && (
            <div className="configure-function-column-toolbar" role="toolbar" aria-label="Columns">
              <ToolbarIconButton tip="Add Column" onClick={addColumn} disabled={columns.length >= MAX_COLUMNS}>
                <PlusIcon />
              </ToolbarIconButton>
              <ToolbarIconButton
                tip="Remove Column"
                onClick={removeColumn}
                disabled={columns.length <= 1}
              >
                <MinusIcon />
              </ToolbarIconButton>
              <ToolbarIconButton
                tip="Move Column Up"
                onClick={() => moveColumn(-1)}
                disabled={selectedColumn <= 0}
              >
                <MoveUpIcon />
              </ToolbarIconButton>
              <ToolbarIconButton
                tip="Move Column Down"
                onClick={() => moveColumn(1)}
                disabled={selectedColumn >= columns.length - 1}
              >
                <MoveDownIcon />
              </ToolbarIconButton>
            </div>
          )}
          <div className="configure-function-footer-actions">
            <button type="button" onClick={() => onSave(config)} disabled={!canSave}>
              OK
            </button>
            <button type="button" onClick={onCancel}>
              Cancel
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}

function ParamField({
  param,
  config,
  columns,
  selectedColumn,
  forms,
  onFocus,
  onSelectColumn,
  onPatch,
  onPatchColumns,
  onPatchConditions,
}: {
  param: FunctionParamDef;
  config: FunctionConfig;
  columns: ColumnConfig[];
  selectedColumn: number;
  forms: string[];
  focused: string;
  onFocus: (id: string) => void;
  onSelectColumn: (index: number) => void;
  onPatch: (id: string, value: string) => void;
  onPatchColumns: (cols: ColumnConfig[], selectIndex?: number) => void;
  onPatchConditions: (state: FunctionConditionsState) => void;
}) {
  if (param.type === "tawala-conditions") {
    return (
      <FunctionConditionsEditor
        paramName={param.name}
        state={parseFunctionConditions(config)}
        onChange={onPatchConditions}
        onFocus={() => onFocus(param.id)}
      />
    );
  }

  if (param.type === "column-collection") {
    return (
      <div className="configure-function-collection">
        {columns.map((col, i) => (
          <div
            key={i}
            className={
              i === selectedColumn
                ? "configure-function-column selected"
                : "configure-function-column"
            }
            onMouseDown={() => onSelectColumn(i)}
          >
            <div className="configure-function-column-title">Column {i + 1}</div>
            <label>
              <span>Heading:</span>
              <FieldTextInput
                value={col.header ?? ""}
                onFocus={() => {
                  onSelectColumn(i);
                  onFocus(`column-${i}-header`);
                }}
                onValueChange={(v) => {
                  const next = [...columns];
                  next[i] = { ...next[i], header: v };
                  onPatchColumns(next, i);
                }}
              />
            </label>
            <label>
              <span>Contents:</span>
              <FieldTextInput
                value={col.contents ?? ""}
                onFocus={() => {
                  onSelectColumn(i);
                  onFocus(`column-${i}-contents`);
                }}
                onValueChange={(v) => {
                  const next = [...columns];
                  next[i] = { ...next[i], contents: v };
                  onPatchColumns(next, i);
                }}
              />
            </label>
            <button
              type="button"
              className="configure-function-column-always"
              onClick={() => {
                onSelectColumn(i);
                onFocus(`column-${i}-always`);
              }}
            >
              + Column is always displayed
            </button>
          </div>
        ))}
      </div>
    );
  }

  if (param.type === "tawala-form") {
    return (
      <label>
        <span>{param.name}:</span>
        <select
          value={String(config[param.id] ?? "")}
          onFocus={() => onFocus(param.id)}
          onChange={(e) => onPatch(param.id, e.target.value)}
        >
          <option value="">— select —</option>
          {forms.map((name) => (
            <option key={name} value={name}>
              {name}
            </option>
          ))}
        </select>
      </label>
    );
  }

  if (param.type === "enumeration" && param.choices) {
    return (
      <label>
        <span>{param.name}:</span>
        <select
          value={String(config[param.id] ?? param.defaultValue ?? "")}
          onFocus={() => onFocus(param.id)}
          onChange={(e) => onPatch(param.id, e.target.value)}
        >
          {param.choices.map((c) => (
            <option key={c.value} value={c.value}>
              {c.description}
            </option>
          ))}
        </select>
      </label>
    );
  }

  const isExpression =
    param.type === "expression" ||
    param.type === "tawala-blank" ||
    param.type === "tawala-mcq" ||
    param.type === "tawala-contents-field";

  return (
    <label>
      <span>
        {param.name}
        {param.required ? ":" : " (optional):"}
      </span>
      {isExpression ? (
        <FieldTextInput
          value={String(config[param.id] ?? "")}
          onFocus={() => onFocus(param.id)}
          onValueChange={(v) => onPatch(param.id, v)}
        />
      ) : (
        <input
          type="text"
          value={String(config[param.id] ?? "")}
          onFocus={() => onFocus(param.id)}
          onChange={(e) => onPatch(param.id, e.target.value)}
        />
      )}
    </label>
  );
}

function focusedHelp(
  def: FunctionDef,
  focused: string,
  _config: FunctionConfig,
): { title: string; body: string; compound?: boolean } {
  if (focused.startsWith("column-")) {
    const parts = focused.split("-");
    const idx = Number(parts[1]);
    const field = parts[2];
    if (field === "header") {
      return {
        title: `Column ${idx + 1} Heading`,
        body: "The heading to display for this column.",
        compound: true,
      };
    }
    if (field === "always") {
      return {
        title: "Column is always displayed",
        body: "When set, this column is always shown. Click again to add a display condition.",
      };
    }
    return {
      title: `Column ${idx + 1} Contents`,
      body: "The Field to display in this column. Drag from the Fields palette or type a qualified name.",
      compound: true,
    };
  }

  const param = def.parameters.find((p) => p.id === focused);
  if (param) {
    return {
      title: param.type === "tawala-conditions" ? "WHERE conditions" : param.name,
      body: param.description,
      compound:
        param.type === "expression" ||
        param.type === "tawala-blank" ||
        param.type === "tawala-mcq" ||
        param.type === "tawala-contents-field" ||
        param.type === "tawala-conditions",
    };
  }

  return { title: def.name, body: def.description };
}

function ToolbarIconButton({
  tip,
  disabled,
  onClick,
  children,
}: {
  tip: string;
  disabled?: boolean;
  onClick: () => void;
  children: ReactNode;
}) {
  return (
    <button
      type="button"
      className="configure-function-tool-btn"
      title={tip}
      aria-label={tip}
      disabled={disabled}
      onClick={onClick}
    >
      {children}
    </button>
  );
}

function PlusIcon() {
  return (
    <svg width="16" height="16" viewBox="0 0 16 16" aria-hidden focusable="false">
      <path d="M8 3v10M3 8h10" stroke="#2e7d32" strokeWidth="2" fill="none" strokeLinecap="round" />
    </svg>
  );
}

function MinusIcon() {
  return (
    <svg width="16" height="16" viewBox="0 0 16 16" aria-hidden focusable="false">
      <path d="M3 8h10" stroke="#2e7d32" strokeWidth="2" fill="none" strokeLinecap="round" />
    </svg>
  );
}

function MoveUpIcon() {
  return (
    <svg width="16" height="16" viewBox="0 0 16 16" aria-hidden focusable="false">
      <path d="M8 3 L13 10 H3 Z" fill="#666" />
    </svg>
  );
}

function MoveDownIcon() {
  return (
    <svg width="16" height="16" viewBox="0 0 16 16" aria-hidden focusable="false">
      <path d="M8 13 L13 6 H3 Z" fill="#666" />
    </svg>
  );
}
