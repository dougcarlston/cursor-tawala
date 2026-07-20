/**
 * Legacy Function/If Where operators for MCQ fields.
 * Display labels match Hybrid (equals, contains, …); XML tags are mc*.
 * Single-select (onlyone) → MCOne (4 ops); multi → MCMany (6 ops).
 * @see TawalaDesigner ComparisonOperator.cs MCOneOperator / MCManyOperator
 */

export interface ConditionOperatorOption {
  /** Persisted / XML element name (e.g. mcEquals, equals). */
  id: string;
  label: string;
}

/** FIB / Hybrid operators (unchanged Skip / If list). */
export const HYBRID_CONDITION_OPS: ConditionOperatorOption[] = [
  { id: "equals", label: "equals" },
  { id: "doesNotEqual", label: "does not equal" },
  { id: "contains", label: "contains" },
  { id: "doesNotContain", label: "does not contain" },
  { id: "beginsWith", label: "begins with" },
  { id: "endsWith", label: "ends with" },
  { id: "isLessThan", label: "is less than" },
  { id: "isLessThanOrEqualTo", label: "is less than or equal to" },
  { id: "isGreaterThan", label: "is greater than" },
  { id: "isGreaterThanOrEqualTo", label: "is greater than or equal to" },
  { id: "isBlank", label: "is blank" },
  { id: "isNotBlank", label: "is not blank" },
];

/** Single-select MCQ (onlyone !== false). */
export const MC_ONE_CONDITION_OPS: ConditionOperatorOption[] = [
  { id: "mcEquals", label: "equals" },
  { id: "mcDoesNotEqual", label: "does not equal" },
  { id: "mcIsBlank", label: "is blank" },
  { id: "mcIsNotBlank", label: "is not blank" },
];

/** Multi-select MCQ (onlyone === false). */
export const MC_MANY_CONDITION_OPS: ConditionOperatorOption[] = [
  { id: "mcEquals", label: "equals" },
  { id: "mcDoesNotEqual", label: "does not equal" },
  { id: "mcContains", label: "contains" },
  { id: "mcDoesNotContain", label: "does not contain" },
  { id: "mcIsBlank", label: "is blank" },
  { id: "mcIsNotBlank", label: "is not blank" },
];

export const UNARY_MC_CONDITION_OPS = new Set(["mcIsBlank", "mcIsNotBlank"]);

export type ConditionFieldKind = "hybrid" | "mcOne" | "mcMany";

export function conditionOpsForKind(kind: ConditionFieldKind): ConditionOperatorOption[] {
  if (kind === "mcOne") return MC_ONE_CONDITION_OPS;
  if (kind === "mcMany") return MC_MANY_CONDITION_OPS;
  return HYBRID_CONDITION_OPS;
}

export function defaultOpForKind(kind: ConditionFieldKind): string {
  if (kind === "mcOne") return "mcEquals";
  if (kind === "mcMany") return "mcContains";
  return "equals";
}

/** Friendly label for token display (covers Hybrid + mc*). */
export function conditionOpLabel(op: string): string {
  const all = [...HYBRID_CONDITION_OPS, ...MC_ONE_CONDITION_OPS, ...MC_MANY_CONDITION_OPS];
  const hit = all.find((o) => o.id === op);
  return hit?.label ?? op;
}

/**
 * When the left field changes kind, keep a parallel op if possible
 * (e.g. equals → mcEquals); otherwise use the kind default.
 */
export function remapConditionOp(prevOp: string, nextKind: ConditionFieldKind): string {
  const nextOps = conditionOpsForKind(nextKind);
  if (nextOps.some((o) => o.id === prevOp)) return prevOp;

  const label = conditionOpLabel(prevOp);
  const byLabel = nextOps.find((o) => o.label === label);
  if (byLabel) return byLabel.id;

  // FIB equals ↔ mcEquals; FIB contains ↔ mcContains
  const fibToMc: Record<string, string> = {
    equals: "mcEquals",
    doesNotEqual: "mcDoesNotEqual",
    contains: "mcContains",
    doesNotContain: "mcDoesNotContain",
    isBlank: "mcIsBlank",
    isNotBlank: "mcIsNotBlank",
    mcEquals: "equals",
    mcDoesNotEqual: "doesNotEqual",
    mcContains: "contains",
    mcDoesNotContain: "doesNotContain",
    mcIsBlank: "isBlank",
    mcIsNotBlank: "isNotBlank",
  };
  const mapped = fibToMc[prevOp];
  if (mapped && nextOps.some((o) => o.id === mapped)) return mapped;

  return defaultOpForKind(nextKind);
}

export function isUnaryConditionOp(op: string): boolean {
  return op === "isBlank" || op === "isNotBlank" || UNARY_MC_CONDITION_OPS.has(op);
}
