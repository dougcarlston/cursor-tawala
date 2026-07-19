/**
 * When a Document is renamed in the Explorer, rewrite Process statements that
 * reference that document (Show / Send / Append), including nested If / ForEach.
 */

import type { TawalaProcessCommand, TawalaProject } from "@/types/tawala";

function rewriteExactName(value: unknown, oldName: string, newName: string): unknown {
  return typeof value === "string" && value === oldName ? newName : value;
}

function rewriteSendBody(
  body: unknown,
  oldName: string,
  newName: string,
): unknown {
  if (!body || typeof body !== "object" || Array.isArray(body)) return body;
  const next = { ...(body as Record<string, unknown>) };
  next.document = rewriteExactName(next.document, oldName, newName);
  return next;
}

/** Rewrite one command (and nested then/else/commands) for a document rename. */
export function rewriteDocumentNameInCommand(
  cmd: TawalaProcessCommand,
  oldName: string,
  newName: string,
): TawalaProcessCommand {
  if (!oldName || !newName || oldName === newName) return cmd;

  const next: TawalaProcessCommand = { ...cmd };

  switch (cmd.cmd) {
    case "show":
    case "showDocument":
      next.document = rewriteExactName(cmd.document, oldName, newName);
      break;
    case "send":
      next.document = rewriteExactName(cmd.document, oldName, newName);
      if (cmd.body !== undefined) {
        next.body = rewriteSendBody(cmd.body, oldName, newName);
      }
      break;
    case "append":
      next.document = rewriteExactName(cmd.document, oldName, newName);
      next.appendage = rewriteExactName(cmd.appendage, oldName, newName);
      break;
    default:
      break;
  }

  if (Array.isArray(cmd.then)) {
    next.then = rewriteDocumentNameInCommands(
      cmd.then as TawalaProcessCommand[],
      oldName,
      newName,
    );
  }
  if (Array.isArray(cmd.else)) {
    next.else = rewriteDocumentNameInCommands(
      cmd.else as TawalaProcessCommand[],
      oldName,
      newName,
    );
  }
  if (Array.isArray(cmd.commands)) {
    next.commands = rewriteDocumentNameInCommands(
      cmd.commands as TawalaProcessCommand[],
      oldName,
      newName,
    );
  }

  return next;
}

export function rewriteDocumentNameInCommands(
  commands: TawalaProcessCommand[],
  oldName: string,
  newName: string,
): TawalaProcessCommand[] {
  return commands.map((cmd) => rewriteDocumentNameInCommand(cmd, oldName, newName));
}

/**
 * Apply a Document rename across all Process commands in the project.
 */
export function cascadeDocumentRenameInProject(
  project: TawalaProject,
  oldName: string,
  newName: string,
): TawalaProject {
  if (!oldName || !newName || oldName === newName) return project;

  const processes = (project.processes ?? []).map((proc) => ({
    ...proc,
    commands: rewriteDocumentNameInCommands(proc.commands ?? [], oldName, newName),
  }));

  return { ...project, processes };
}
