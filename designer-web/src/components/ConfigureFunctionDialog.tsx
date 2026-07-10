import { useEffect, useMemo, useState } from "react";
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

/** Legacy `ConfigureFunctionDialog` — parameter fields + yellow help pane. */
export function ConfigureFunctionDialog({ def, initialConfig, onCancel, onSave }: Props) {
  const forms = useProjectStore((s) => s.project.forms);
  const [config, setConfig] = useState<FunctionConfig>(() => ({
    ...defaultFunctionConfig(def),
    ...initialConfig,
  }));
  const [focused, setFocused] = useState<FieldKey>(def.parameters[0]?.id ?? "summary");

  const canSave = useMemo(() => configMeetsRequirements(def, config), [def, config]);

  useEffect(() => {
    setConfig({ ...defaultFunctionConfig(def), ...initialConfig });
    setFocused(def.parameters[0]?.id ?? "summary");
  }, [def, initialConfig]);

  const help = focusedHelp(def, focused, config);

  const patch = (id: string, value: string) => {
    setConfig((prev) => ({ ...prev, [id]: value }));
  };

  const patchColumns = (count: number, cols: ColumnConfig[]) => {
    setConfig((prev) => ({ ...prev, numberOfColumns: count, column: cols }));
  };

  const patchConditions = (state: FunctionConditionsState) => {
    setConfig((prev) => ({ ...prev, ...functionConditionsToConfig(state) }) as FunctionConfig);
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
                forms={forms.map((f) => f.name)}
                focused={focused}
                onFocus={setFocused}
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
        <div className="modal-footer fib-validation-footer">
          <button type="button" onClick={() => onSave(config)} disabled={!canSave}>
            OK
          </button>
          <button type="button" onClick={onCancel}>
            Cancel
          </button>
        </div>
      </div>
    </div>
  );
}

function ParamField({
  param,
  config,
  forms,
  onFocus,
  onPatch,
  onPatchColumns,
  onPatchConditions,
}: {
  param: FunctionParamDef;
  config: FunctionConfig;
  forms: string[];
  focused: string;
  onFocus: (id: string) => void;
  onPatch: (id: string, value: string) => void;
  onPatchColumns: (count: number, cols: ColumnConfig[]) => void;
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
    const count = Number(config.numberOfColumns ?? 2);
    const cols = (config.column as ColumnConfig[] | undefined) ?? [];
    return (
      <div className="configure-function-collection">
        <label>
          <span>Number of columns:</span>
          <input
            type="number"
            min={1}
            max={12}
            value={count}
            onFocus={() => onFocus("numberOfColumns")}
            onChange={(e) => {
              const n = Math.max(1, Math.min(12, Number(e.target.value) || 1));
              const next = [...cols];
              while (next.length < n) next.push({ header: "", contents: "" });
              onPatchColumns(n, next.slice(0, n));
            }}
          />
        </label>
        {Array.from({ length: count }, (_, i) => (
          <div key={i} className="configure-function-column">
            <label>
              <span>Column {i + 1} Heading:</span>
              <FieldTextInput
                value={cols[i]?.header ?? ""}
                onFocus={() => onFocus(`column-${i}-header`)}
                onValueChange={(v) => {
                  const next = [...cols];
                  while (next.length <= i) next.push({ header: "", contents: "" });
                  next[i] = { ...next[i], header: v };
                  onPatchColumns(count, next);
                }}
              />
            </label>
            <label>
              <span>Column {i + 1} Field:</span>
              <FieldTextInput
                value={cols[i]?.contents ?? ""}
                onFocus={() => onFocus(`column-${i}-contents`)}
                onValueChange={(v) => {
                  const next = [...cols];
                  while (next.length <= i) next.push({ header: "", contents: "" });
                  next[i] = { ...next[i], contents: v };
                  onPatchColumns(count, next);
                }}
              />
            </label>
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
    return {
      title: `Column ${idx + 1} Field`,
      body: "The Field to display in this column. Drag from the Fields palette or type a qualified name.",
      compound: true,
    };
  }
  if (focused === "numberOfColumns") {
    return { title: "Number of columns", body: "How many columns the table should have." };
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
