/**
 * Project variable names for Deploy-time expression disambiguation (server).
 * Mirrors designer-web/src/lib/projectModel.ts `collectProjectVariables`.
 */

const ASSIGNMENT_COMMANDS = new Set([
  "set",
  "addTo",
  "subtractFrom",
  "multiplyBy",
  "divideBy",
]);

const VARIABLE_REFERENCE = /<<([^<>]+)>>/g;

function isPlainVariableName(value) {
  return (
    typeof value === "string" &&
    value.trim().length > 0 &&
    !value.includes(":") &&
    !value.includes("<<") &&
    !/\s/.test(value)
  );
}

function collectVariableReferences(value, out) {
  VARIABLE_REFERENCE.lastIndex = 0;
  let match;
  while ((match = VARIABLE_REFERENCE.exec(value)) !== null) {
    const token = match[1].trim();
    if (isPlainVariableName(token)) out.add(token);
  }
}

function collectVariablesFromNode(node, out) {
  if (Array.isArray(node)) {
    for (const child of node) collectVariablesFromNode(child, out);
    return;
  }
  if (!node || typeof node !== "object") return;
  if (typeof node.cmd === "string" && ASSIGNMENT_COMMANDS.has(node.cmd)) {
    const target = node.field ?? node.variable;
    if (isPlainVariableName(target)) out.add(target.trim());
  }
  for (const value of Object.values(node)) {
    if (typeof value === "string") collectVariableReferences(value, out);
    else if (value && typeof value === "object") collectVariablesFromNode(value, out);
  }
}

/** Set of plain project variable names (includes `_InviteeID`). */
export function collectProjectVariableNames(project) {
  const vars = new Set(["_InviteeID"]);
  if (!project || typeof project !== "object") return vars;
  for (const process of project.processes ?? []) {
    collectVariablesFromNode(process.commands, vars);
  }
  for (const form of project.forms ?? []) {
    for (const item of form.items ?? []) {
      if (item?.type === "skipInstructions") {
        collectVariablesFromNode(item.commands, vars);
      }
    }
  }
  return vars;
}
