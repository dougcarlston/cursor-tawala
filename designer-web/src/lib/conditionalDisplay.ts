/**
 * Form Item conditional display — open dialog request + condition ↔ editor mapping.
 * Spec: DESIGNER_FORM_ITEMS_CONDITIONAL_DISPLAY.md
 */

import {
  DEFAULT_FUNCTION_CONDITIONS,
  EMPTY_FUNCTION_CONDITION_ROW,
  functionConditionsAreValid,
  functionConditionsRowIsComplete,
  type FunctionConditionsState,
} from "./functionConditions";
import { isUnaryConditionOp } from "./mcConditionOperators";
import { hasDisplayCondition } from "./preservedImportGaps";
import { parseConditionToRows } from "./statementBuilders";

type ConditionTree = {
  op?: string;
  field?: string;
  value?: unknown;
  conditions?: ConditionTree[];
  and?: ConditionTree[];
  or?: ConditionTree[];
};

export type ConditionalDisplayRequest = {
  formName: string;
  itemIndex: number;
};

let request: ConditionalDisplayRequest | null = null;
const listeners = new Set<() => void>();

function emit() {
  for (const l of listeners) l();
}

export function subscribeConditionalDisplay(listener: () => void): () => void {
  listeners.add(listener);
  return () => listeners.delete(listener);
}

export function getConditionalDisplayRequest(): ConditionalDisplayRequest | null {
  return request;
}

export function openConditionalDisplayDialog(formName: string, itemIndex: number): void {
  request = { formName, itemIndex };
  emit();
}

export function clearConditionalDisplayRequest(): void {
  request = null;
  emit();
}

export function displayConditionToEditorState(dc: unknown): {
  enabled: boolean;
  conditions: FunctionConditionsState;
} {
  if (!hasDisplayCondition(dc)) {
    return {
      enabled: false,
      conditions: {
        ...DEFAULT_FUNCTION_CONDITIONS,
        rows: [{ ...EMPTY_FUNCTION_CONDITION_ROW }],
      },
    };
  }
  const { combinator, rows } = parseConditionToRows(dc as ConditionTree | undefined);
  return {
    enabled: true,
    conditions: {
      combinator,
      rows: rows.length > 0 ? rows : [{ ...EMPTY_FUNCTION_CONDITION_ROW }],
    },
  };
}

/** When checkbox is on, require at least one complete Where row. */
export function conditionalDisplayOkEnabled(
  enabled: boolean,
  conditions: FunctionConditionsState,
): boolean {
  if (!enabled) return true;
  if (!functionConditionsAreValid(conditions.rows)) return false;
  return conditions.rows.some((r) => functionConditionsRowIsComplete(r));
}

/**
 * Checkbox off → clear. Checkbox on → single clause or `{ op, conditions }` tree
 * (same shape as Skip/If / import).
 */
export function editorStateToDisplayCondition(
  enabled: boolean,
  conditions: FunctionConditionsState,
): unknown | undefined {
  if (!enabled) return undefined;
  const clauses = conditions.rows
    .filter((r) => r.field.trim())
    .map((r) => {
      const base: ConditionTree = { field: r.field.trim(), op: r.op };
      if (!isUnaryConditionOp(r.op)) {
        base.value = r.value;
      }
      return base;
    });
  if (clauses.length === 0) return undefined;
  if (clauses.length === 1) return clauses[0];
  return { op: conditions.combinator, conditions: clauses };
}
