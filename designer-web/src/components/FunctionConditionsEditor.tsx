import { FieldTextInput } from "./FieldDropInputs";
import {
  DEFAULT_FUNCTION_CONDITIONS,
  functionConditionsRowIsComplete,
  type FunctionConditionRow,
  type FunctionConditionsState,
} from "@/lib/functionConditions";
import {
  conditionOpsForKind,
  defaultOpForKind,
  isUnaryConditionOp,
  remapConditionOp,
  type ConditionFieldKind,
} from "@/lib/mcConditionOperators";
import { lookupFormFieldMcItem } from "@/lib/projectModel";
import { useProjectStore } from "@/store/projectStore";
import type { TawalaProject } from "@/types/tawala";
import { useEffect, useRef } from "react";

interface Props {
  /** Parameter label from catalog (e.g. "Count only the records"). */
  paramName: string;
  state: FunctionConditionsState;
  onChange: (next: FunctionConditionsState) => void;
  onFocus?: () => void;
  /** `displayWhen` = Hyperlink dialog (no "Limit output…" chrome). */
  variant?: "recordsWhere" | "displayWhen";
  disabled?: boolean;
}

function fieldKindFromRef(project: TawalaProject, fieldRef: string): ConditionFieldKind {
  const mc = lookupFormFieldMcItem(project, fieldRef);
  if (!mc) return "hybrid";
  return mc.onlyone === false ? "mcMany" : "mcOne";
}

/**
 * Legacy `ConditionListControl` — "Limit output to records where" + field / operator / value rows.
 * Hyperlink uses `variant="displayWhen"` → "Display link only when" + condition row.
 * When the left field is an MCQ, operator ids switch to `mc*` (legacy MCOne / MCMany).
 */
export function FunctionConditionsEditor({
  paramName,
  state,
  onChange,
  onFocus,
  variant = "recordsWhere",
  disabled = false,
}: Props) {
  const project = useProjectStore((s) => s.project);
  const { combinator, rows } = state;
  const multi = rows.length > 1;
  const displayWhen = variant === "displayWhen";
  const onChangeRef = useRef(onChange);
  onChangeRef.current = onChange;
  const stateRef = useRef(state);
  stateRef.current = state;
  const rowFingerprint = rows.map((r) => `${r.field}\0${r.op}`).join("|");

  // Remap stale FIB ops (equals/contains) to mc* when the field is an MCQ.
  useEffect(() => {
    const current = stateRef.current;
    let changed = false;
    const nextRows = current.rows.map((row) => {
      if (!row.field.trim()) return row;
      const kind = fieldKindFromRef(project, row.field);
      const nextOp = remapConditionOp(row.op, kind);
      if (nextOp === row.op) return row;
      changed = true;
      return { ...row, op: nextOp };
    });
    if (changed) onChangeRef.current({ ...current, rows: nextRows });
  }, [project, rowFingerprint]);

  const patchRow = (index: number, patch: Partial<FunctionConditionRow>) => {
    onChange({
      ...state,
      rows: rows.map((r, i) => (i === index ? { ...r, ...patch } : r)),
    });
  };

  const onFieldChange = (index: number, field: string) => {
    const prev = rows[index];
    const kind = fieldKindFromRef(project, field);
    const op = remapConditionOp(prev.op, kind);
    patchRow(index, { field, op });
  };

  const addRowAfter = (index: number) => {
    const next = [...rows];
    next.splice(index + 1, 0, { field: "", op: defaultOpForKind("hybrid"), value: "" });
    onChange({ ...state, rows: next });
  };

  const removeRow = (index: number) => {
    if (rows.length <= 1) {
      onChange({
        ...DEFAULT_FUNCTION_CONDITIONS,
        rows: [{ field: "", op: defaultOpForKind("hybrid"), value: "" }],
      });
      return;
    }
    onChange({ ...state, rows: rows.filter((_, i) => i !== index) });
  };

  return (
    <div
      className={`function-conditions-editor${displayWhen ? " function-conditions-display-when" : ""}`}
      onFocus={onFocus}
      aria-disabled={disabled || undefined}
    >
      {displayWhen ? (
        <div className="function-conditions-header">
          <span className="function-conditions-where">{paramName}</span>
          {multi && (
            <>
              <select
                className="function-conditions-combinator"
                value={combinator}
                disabled={disabled}
                aria-label="Condition combinator"
                onChange={(e) =>
                  onChange({
                    ...state,
                    combinator: e.target.value as FunctionConditionsState["combinator"],
                  })
                }
              >
                <option value="and">ALL</option>
                <option value="or">ANY</option>
              </select>
              <span className="function-conditions-where-tail">of the following conditions are true:</span>
            </>
          )}
        </div>
      ) : (
        <>
          <div className="function-conditions-header">
            <span className="function-conditions-where">Limit output to records where</span>
            {multi && (
              <>
                <select
                  className="function-conditions-combinator"
                  value={combinator}
                  disabled={disabled}
                  aria-label="Condition combinator"
                  onChange={(e) =>
                    onChange({
                      ...state,
                      combinator: e.target.value as FunctionConditionsState["combinator"],
                    })
                  }
                >
                  <option value="and">ALL</option>
                  <option value="or">ANY</option>
                </select>
                <span className="function-conditions-where-tail">of the following are true:</span>
              </>
            )}
          </div>
          <p className="function-conditions-param-name">{paramName}</p>
        </>
      )}
      {rows.map((row, i) => {
        const kind = fieldKindFromRef(project, row.field);
        const ops = conditionOpsForKind(kind);
        const opValue = ops.some((o) => o.id === row.op) ? row.op : defaultOpForKind(kind);
        const valuePlaceholder =
          kind === "mcOne" || kind === "mcMany" ? "Choice letter (e.g. d)" : "Value";
        return (
          <div key={i} className="skip-if-row function-conditions-row">
            <FieldTextInput
              configureDialog
              bare
              className="skip-if-field"
              placeholder="Record:Form:Field"
              value={row.field}
              disabled={disabled}
              onFocus={onFocus}
              onValueChange={(v) => onFieldChange(i, v)}
            />
            <select
              className="skip-if-operator"
              value={opValue}
              aria-label="Operator"
              disabled={disabled}
              onFocus={onFocus}
              onChange={(e) => patchRow(i, { op: e.target.value })}
            >
              {ops.map((op) => (
                <option key={op.id} value={op.id}>
                  {op.label}
                </option>
              ))}
            </select>
            {!isUnaryConditionOp(opValue) ? (
              <FieldTextInput
                configureDialog
                className="skip-if-value"
                placeholder={valuePlaceholder}
                value={row.value}
                disabled={disabled}
                onFocus={onFocus}
                onValueChange={(v) => patchRow(i, { value: v })}
              />
            ) : (
              <span className="skip-if-value-placeholder" aria-hidden />
            )}
            <button
              type="button"
              className="skip-if-row-btn"
              title="Add condition row"
              disabled={disabled || !functionConditionsRowIsComplete({ ...row, op: opValue })}
              onClick={() => addRowAfter(i)}
            >
              +
            </button>
            <button
              type="button"
              className="skip-if-row-btn"
              title="Remove condition row"
              disabled={disabled}
              onClick={() => removeRow(i)}
            >
              −
            </button>
          </div>
        );
      })}
    </div>
  );
}
