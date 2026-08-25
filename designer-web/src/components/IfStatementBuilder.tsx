import { useEffect, useRef } from "react";
import { QualifiedFieldInput, FieldTextInput } from "@/components/FieldDropInputs";
import type { ConditionCombinator, ConditionRow, IfBuilderState } from "@/lib/statementBuilders";
import { rowsAreValid } from "@/lib/statementBuilders";
import {
  conditionOpsForKind,
  defaultOpForKind,
  isUnaryConditionOp,
  remapConditionOp,
  type ConditionFieldKind,
} from "@/lib/mcConditionOperators";
import { lookupFormFieldMcItem } from "@/lib/projectModel";
import type { TawalaProject } from "@/types/tawala";

export interface IfStatementBuilderProps {
  project: TawalaProject;
  knownVariables: ReadonlySet<string>;
  state: IfBuilderState;
  onStateChange: (next: IfBuilderState) => void;
  submitLabel: string;
  onSubmit: () => void;
  /** Leave Modify/edit without applying — clears selection so insert gaps work again. */
  onCancel?: () => void;
  /** Process MDI window — flatter chrome, no extra panel border (double-line divider below). */
  embedded?: boolean;
}

function fieldKindFromRef(project: TawalaProject, fieldRef: string): ConditionFieldKind {
  const mc = lookupFormFieldMcItem(project, fieldRef);
  if (!mc) return "hybrid";
  return mc.onlyone === false ? "mcMany" : "mcOne";
}

/**
 * Shared If statement property panel — used by Skip Instructions and Process editor.
 * Legacy copy and layout from `SkipInstructionsDialog` / `DESIGNER_PROCESS_STATEMENTS_IF.md`.
 * When the left field is an MCQ, operators switch to `mc*` (same as Function Where).
 */
export function IfStatementBuilder({
  project,
  knownVariables,
  state,
  onStateChange,
  submitLabel,
  onSubmit,
  onCancel,
  embedded = false,
}: IfStatementBuilderProps) {
  const { combinator, rows, hasElse } = state;
  const canSubmit = rowsAreValid(rows, knownVariables);
  const onChangeRef = useRef(onStateChange);
  onChangeRef.current = onStateChange;
  const stateRef = useRef(state);
  stateRef.current = state;
  const rowFingerprint = rows.map((r) => `${r.field}\0${r.op}`).join("|");

  // Remap stale FIB ops (equals/contains) to mc* when the field is an MCQ — converted
  // skips keep mcEquals, but a Hybrid dropdown pick of "equals" must not stick.
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

  const setCombinator = (value: ConditionCombinator) => {
    onStateChange({ ...state, combinator: value });
  };

  const updateRow = (index: number, patch: Partial<ConditionRow>) => {
    onStateChange({
      ...state,
      rows: rows.map((r, i) => (i === index ? { ...r, ...patch } : r)),
    });
  };

  const onFieldChange = (index: number, field: string) => {
    const prev = rows[index];
    const kind = fieldKindFromRef(project, field);
    const op = remapConditionOp(prev.op, kind);
    updateRow(index, { field, op });
  };

  const addRowAfter = (index: number) => {
    const next = [...rows];
    next.splice(index + 1, 0, { field: "", op: defaultOpForKind("hybrid"), value: "" });
    onStateChange({ ...state, rows: next });
  };

  const removeRow = (index: number) => {
    if (rows.length <= 1) return;
    onStateChange({ ...state, rows: rows.filter((_, j) => j !== index) });
  };

  return (
    <div
      className={`skip-statement-panel skip-if-builder${embedded ? " process-embedded" : ""}`}
    >
      <div className="skip-statement-panel-tab">If</div>
      <div className="skip-statement-panel-body">
        <p className="skip-if-intro">
          If{" "}
          {rows.length > 1 ? (
            <>
              <select
                value={combinator}
                onChange={(e) => setCombinator(e.target.value as ConditionCombinator)}
                aria-label="Condition combinator"
                className="skip-combinator"
              >
                <option value="and">ALL</option>
                <option value="or">ANY</option>
              </select>{" "}
              of the following conditions are true, execute the first set of commands:
            </>
          ) : (
            <>the following condition is true, execute the first set of commands:</>
          )}
        </p>
        {rows.map((row, i) => {
          const kind = fieldKindFromRef(project, row.field);
          const ops = conditionOpsForKind(kind);
          const opIds = new Set(ops.map((o) => o.id));
          return (
            <div key={i} className="skip-if-row">
              <QualifiedFieldInput
                className="skip-if-field"
                placeholder="Form:Field"
                knownVariables={knownVariables}
                value={row.field}
                onValueChange={(v) => onFieldChange(i, v)}
              />
              <select
                value={row.op}
                onChange={(e) => updateRow(i, { op: e.target.value })}
                aria-label="Operator"
                className="skip-if-operator"
              >
                {!opIds.has(row.op) ? (
                  <option value={row.op}>{row.op}</option>
                ) : null}
                {ops.map((op) => (
                  <option key={op.id} value={op.id}>
                    {op.label}
                  </option>
                ))}
              </select>
              {!isUnaryConditionOp(row.op) ? (
                <FieldTextInput
                  className="skip-if-value"
                  placeholder={kind === "hybrid" ? "Value" : "Choice letter"}
                  value={row.value}
                  onValueChange={(v) => updateRow(i, { value: v })}
                />
              ) : (
                <span className="skip-if-value-placeholder" aria-hidden />
              )}
              <button
                type="button"
                className="skip-if-row-btn"
                title="Add condition row"
                onClick={() => addRowAfter(i)}
              >
                +
              </button>
              <button
                type="button"
                className="skip-if-row-btn"
                title="Remove condition row"
                disabled={rows.length <= 1}
                onClick={() => removeRow(i)}
              >
                −
              </button>
            </div>
          );
        })}
        <label className="skip-if-else">
          <input
            type="checkbox"
            checked={hasElse}
            onChange={(e) => onStateChange({ ...state, hasElse: e.target.checked })}
          />
          Otherwise execute second set of commands
        </label>
        <div className="skip-if-add-row">
          <button type="button" className="skip-add-btn" disabled={!canSubmit} onClick={onSubmit}>
            {submitLabel}
          </button>
          {onCancel ? (
            <button type="button" className="skip-cancel-btn" onClick={onCancel}>
              Cancel
            </button>
          ) : null}
        </div>
      </div>
    </div>
  );
}
