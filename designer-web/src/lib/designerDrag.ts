/**
 * HTML5 drag payloads for Project Explorer entities and Items/Statements palettes.
 */

import type { FormItemType } from "@/types/tawala";
import type { WindowKind } from "@/store/projectStore";

export const EXPLORER_ENTITY_MIME = "application/x-tawala-explorer-entity";
export const FORM_ITEM_MIME = "application/x-tawala-form-item";
export const PROCESS_STATEMENT_MIME = "application/x-tawala-process-statement";

export interface ExplorerEntityDrag {
  kind: WindowKind;
  name: string;
}

export function setExplorerEntityDrag(
  dataTransfer: DataTransfer,
  kind: WindowKind,
  name: string,
): void {
  const payload: ExplorerEntityDrag = { kind, name };
  dataTransfer.setData(EXPLORER_ENTITY_MIME, JSON.stringify(payload));
  dataTransfer.setData("text/plain", `${kind}:${name}`);
  dataTransfer.effectAllowed = "copy";
}

export function hasExplorerEntityDrag(dataTransfer: DataTransfer | null): boolean {
  if (!dataTransfer) return false;
  return Array.from(dataTransfer.types).includes(EXPLORER_ENTITY_MIME);
}

export function readExplorerEntityDrag(
  dataTransfer: DataTransfer | null,
): ExplorerEntityDrag | null {
  if (!dataTransfer) return null;
  const raw = dataTransfer.getData(EXPLORER_ENTITY_MIME);
  if (!raw) return null;
  try {
    const parsed = JSON.parse(raw) as ExplorerEntityDrag;
    if (
      (parsed.kind === "form" || parsed.kind === "process" || parsed.kind === "document") &&
      typeof parsed.name === "string" &&
      parsed.name
    ) {
      return parsed;
    }
  } catch {
    /* ignore */
  }
  return null;
}

export function setFormItemDrag(dataTransfer: DataTransfer, type: FormItemType): void {
  dataTransfer.setData(FORM_ITEM_MIME, type);
  dataTransfer.setData("text/plain", type);
  dataTransfer.effectAllowed = "copy";
}

export function hasFormItemDrag(dataTransfer: DataTransfer | null): boolean {
  if (!dataTransfer) return false;
  return Array.from(dataTransfer.types).includes(FORM_ITEM_MIME);
}

export function readFormItemDrag(dataTransfer: DataTransfer | null): FormItemType | null {
  if (!dataTransfer) return null;
  const type = dataTransfer.getData(FORM_ITEM_MIME);
  return type ? (type as FormItemType) : null;
}

export function setProcessStatementDrag(dataTransfer: DataTransfer, label: string): void {
  dataTransfer.setData(PROCESS_STATEMENT_MIME, label);
  dataTransfer.setData("text/plain", label);
  dataTransfer.effectAllowed = "copy";
}

export function hasProcessStatementDrag(dataTransfer: DataTransfer | null): boolean {
  if (!dataTransfer) return false;
  return Array.from(dataTransfer.types).includes(PROCESS_STATEMENT_MIME);
}

export function readProcessStatementDrag(dataTransfer: DataTransfer | null): string | null {
  if (!dataTransfer) return null;
  const label = dataTransfer.getData(PROCESS_STATEMENT_MIME);
  return label || null;
}
