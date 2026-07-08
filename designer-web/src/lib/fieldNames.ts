import { TawalaProject } from "@/types/tawala";

/** Collect stored field names used across the project (FIB blanks, MCQ, hidden fields). */
export function collectProjectFieldNames(
  project: TawalaProject,
  exclude?: { formName: string; itemIndex: number },
): Set<string> {
  const taken = new Set<string>();
  for (const form of project.forms) {
    form.items.forEach((it, itemIndex) => {
      if (exclude && exclude.formName === form.name && exclude.itemIndex === itemIndex) return;
      if (it.type === "fib") {
        for (const b of it.blanks ?? []) {
          const nm = b.alternateLabel?.trim();
          if (nm) taken.add(nm.toLowerCase());
        }
      } else if (it.type === "mc") {
        const nm = (it.name ?? it.alternateLabel)?.trim();
        if (nm) taken.add(nm.toLowerCase());
      } else if (it.type === "field") {
        const nm = (it.fieldName ?? it.name)?.trim();
        if (nm) taken.add(nm.toLowerCase());
      }
    });
  }
  return taken;
}

/** Legacy `HiddenField.createDefaultName()` — Field1, Field2, … across all forms. */
export function nextHiddenFieldName(project: TawalaProject): string {
  let num = 1;
  const re = /^Field(\d+)$/i;
  for (const form of project.forms) {
    for (const it of form.items) {
      if (it.type !== "field") continue;
      const nm = (it.fieldName ?? it.name ?? "").trim();
      const m = re.exec(nm);
      if (m) num = Math.max(num, Number(m[1]) + 1);
    }
  }
  return `Field${num}`;
}

export function isValidHiddenFieldName(name: string, taken: Set<string>): string | null {
  const trimmed = name.trim();
  if (!trimmed) return "Name is required.";
  if (trimmed.includes(":")) return "Name cannot contain a colon.";
  if (taken.has(trimmed.toLowerCase())) return `"${trimmed}" is already used by another field.`;
  return null;
}
