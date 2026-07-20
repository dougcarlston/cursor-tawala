import { useEffect, useLayoutEffect, useMemo, useRef, useState } from "react";
import { createPortal } from "react-dom";
import type { FormItem, SkipCommand, TawalaForm, TawalaProject } from "@/types/tawala";
import { IfStatementBuilder } from "@/components/IfStatementBuilder";
import { SetStatementBuilder } from "@/components/SetStatementBuilder";
import { FieldTextInput } from "@/components/FieldDropInputs";
import { SkipScriptView } from "@/components/SkipScriptView";
import { useDraggableDialog } from "@/hooks/useDraggableDialog";
import { formatInsertPathLabel } from "@/lib/skipScript";
import { buildScriptLines } from "@/lib/skipScript";
import { insertCommandAtPoint } from "@/lib/processInsert";
import {
  canMoveProcessCommandAtPath,
  deleteProcessCommandAtPath,
  getProcessCommandAtPath,
  moveProcessCommandAtPath,
  replaceProcessCommandAtPath,
} from "@/lib/processScript";
import { setActiveFieldTarget } from "@/lib/fieldInsertion";
import { collectKnownVariables } from "@/lib/projectModel";
import {
  clearSkipDialogSession,
  readSkipDialogSession,
  writeSkipDialogSession,
} from "@/lib/skipDialogSession";
import {
  EMPTY_CONDITION_ROW,
  EMPTY_IF_BUILDER,
  EMPTY_SET_BUILDER,
  ifBuilderFromCommand,
  ifBuilderHasDraft,
  setBuilderFromCommand,
  setBuilderHasDraft,
  setBuilderIsValid,
  type IfBuilderState,
  type SetBuilderState,
} from "@/lib/statementBuilders";
import { buildConditionFromRows } from "@/lib/skipSummary";

function freshIfBuilder(): IfBuilderState {
  return { ...EMPTY_IF_BUILDER, rows: [{ ...EMPTY_CONDITION_ROW }] };
}

function freshSetBuilder(): SetBuilderState {
  return { ...EMPTY_SET_BUILDER };
}

interface Props {
  projectName: string;
  project: TawalaProject;
  form: TawalaForm;
  commands: SkipCommand[];
  /** Stable key for draft survival across accidental remounts (formName::index). */
  sessionKey: string;
  onSave: (commands: SkipCommand[]) => void;
}

const SKIP_STATEMENT_BUTTONS = [
  { key: "if", label: "If" },
  { key: "skipTo", label: "SkipTo" },
  { key: "set", label: "Set" },
  { key: "comment", label: "Comment" },
] as const;

type PanelMode = "none" | "if" | "skipTo" | "set" | "comment";

const END_OF_FORM = "__EndOfForm__";

const SKIP_TOOLBAR = [
  { id: "cut", label: "Cut", icon: <CutIcon /> },
  { id: "copy", label: "Copy", icon: <CopyIcon /> },
  { id: "paste", label: "Paste", icon: <PasteIcon /> },
  { id: "delete", label: "Delete", icon: <DeleteIcon /> },
  { id: "undo", label: "Undo", icon: <UndoIcon /> },
  { id: "redo", label: "Redo", icon: <RedoIcon /> },
] as const;

/** Destinations: question/text/heading labels only (not Hidden Field / Break / Skip). */
function skipDestinations(items: FormItem[]): { value: string; label: string }[] {
  const dests = items
    .filter(
      (i) =>
        i.type !== "skipInstructions" &&
        i.type !== "break" &&
        i.type !== "field",
    )
    .map((i) => {
      const alt = String(
        ("alternateLabel" in i && i.alternateLabel) ||
          ("name" in i && (i as { name?: string }).name) ||
          "",
      ).trim();
      const value = alt || i.label;
      return { value, label: alt && alt !== i.label ? `${i.label} (${alt})` : i.label };
    });
  dests.push({ value: END_OF_FORM, label: "End of Form" });
  return dests;
}

/**
 * Edit Skip Instructions — mini process editor (legacy SkipInstructionsDialog).
 * Layout from owner screenshots (July 2026): Statements palette, If builder, script + insertion arrow.
 */
export function SkipInstructionsDialog({
  projectName,
  project,
  form,
  commands: initialCommands,
  sessionKey,
  onSave,
}: Props) {
  const restored = readSkipDialogSession(sessionKey);
  const [commands, setCommands] = useState<SkipCommand[]>(
    () =>
      restored?.commands ??
      structuredClone(Array.isArray(initialCommands) ? initialCommands : []),
  );
  const [insertPath, setInsertPath] = useState(restored?.insertPath ?? "root");
  const [insertIndex, setInsertIndex] = useState(restored?.insertIndex ?? 0);
  const [selectedCommandPath, setSelectedCommandPath] = useState<string | null>(
    restored?.selectedCommandPath ?? null,
  );
  const [panel, setPanel] = useState<PanelMode>(restored?.panel ?? "none");
  const [ifBuilder, setIfBuilder] = useState<IfBuilderState>(
    () => restored?.ifBuilder ?? freshIfBuilder(),
  );
  const [setBuilder, setSetBuilder] = useState<SetBuilderState>(
    () => restored?.setBuilder ?? freshSetBuilder(),
  );
  const [skipToDest, setSkipToDest] = useState(restored?.skipToDest ?? END_OF_FORM);
  const [commentText, setCommentText] = useState(restored?.commentText ?? "");

  const scriptRef = useRef<HTMLDivElement>(null);
  const dialogRef = useRef<HTMLDivElement>(null);
  const destinations = skipDestinations(form.items);
  const { offset, titleBarProps } = useDraggableDialog(
    { x: -160, y: -30 },
    { dialogRef },
  );

  // Persist mid-edit draft so a remount (e.g. form canvas re-key) does not wipe If/Set.
  useEffect(() => {
    writeSkipDialogSession(sessionKey, {
      commands,
      insertPath,
      insertIndex,
      selectedCommandPath,
      panel,
      ifBuilder,
      setBuilder,
      skipToDest,
      commentText,
    });
  }, [
    sessionKey,
    commands,
    insertPath,
    insertIndex,
    selectedCommandPath,
    panel,
    ifBuilder,
    setBuilder,
    skipToDest,
    commentText,
  ]);

  const finish = (nextCommands: SkipCommand[]) => {
    clearSkipDialogSession(sessionKey);
    onSave(nextCommands);
  };

  const scriptLines = useMemo(() => buildScriptLines(commands), [commands]);
  const knownVariables = useMemo(
    () => collectKnownVariables(project, commands),
    [project, commands],
  );
  const isEmpty = commands.length === 0;
  const canAddComment = commentText.trim().length > 0;
  const insertPathLabel = formatInsertPathLabel(insertPath);

  const selectedCommand =
    selectedCommandPath != null
      ? getProcessCommandAtPath(commands, selectedCommandPath)
      : null;
  const isModifyIf =
    panel === "if" && selectedCommand?.cmd === "if" && selectedCommandPath != null;
  const isModifySkipTo =
    panel === "skipTo" &&
    selectedCommand?.cmd === "skip" &&
    selectedCommandPath != null;
  const isModifySet =
    panel === "set" && selectedCommand?.cmd === "set" && selectedCommandPath != null;
  const isModifyComment =
    panel === "comment" &&
    selectedCommand?.cmd === "comment" &&
    selectedCommandPath != null;

  const setInsertPoint = (path: string, index: number) => {
    setSelectedCommandPath(null);
    setInsertPath(path);
    setInsertIndex(index);
  };

  const insertAtArrow = (cmd: SkipCommand) => {
    const result = insertCommandAtPoint(commands, insertPath, insertIndex, cmd);
    setCommands(result.commands);
    setInsertPath(result.insertPath);
    setInsertIndex(result.insertIndex);
    setSelectedCommandPath(null);
    requestAnimationFrame(() => {
      scriptRef.current?.scrollTo(0, scriptRef.current.scrollHeight);
    });
  };

  const closeBuilderPanel = () => {
    setPanel("none");
    setActiveFieldTarget(null);
  };

  const submitIf = () => {
    const condition = buildConditionFromRows(ifBuilder.combinator, ifBuilder.rows);
    if (isModifyIf && selectedCommandPath) {
      const existing = getProcessCommandAtPath(commands, selectedCommandPath);
      if (existing?.cmd === "if") {
        const updated: SkipCommand = {
          cmd: "if",
          condition,
          then: (existing.then as SkipCommand[] | undefined) ?? [],
          ...(ifBuilder.hasElse
            ? { else: (existing.else as SkipCommand[] | undefined) ?? [] }
            : {}),
        };
        setCommands(replaceProcessCommandAtPath(commands, selectedCommandPath, updated));
        return;
      }
    }
    const ifCmd: SkipCommand = {
      cmd: "if",
      condition,
      then: [],
      ...(ifBuilder.hasElse ? { else: [] } : {}),
    };
    insertAtArrow(ifCmd);
  };

  const submitSkipTo = () => {
    const cmd: SkipCommand = { cmd: "skip", to: skipToDest };
    if (isModifySkipTo && selectedCommandPath) {
      setCommands(replaceProcessCommandAtPath(commands, selectedCommandPath, cmd));
      return;
    }
    insertAtArrow(cmd);
  };

  const submitSet = () => {
    if (!setBuilderIsValid(setBuilder)) return;
    const cmd: SkipCommand = {
      cmd: "set",
      field: setBuilder.field.trim(),
      value: setBuilder.value,
    };
    if (setBuilder.arithmeticAsText) {
      cmd.arithmeticAsText = true;
    }
    if (isModifySet && selectedCommandPath) {
      setCommands(replaceProcessCommandAtPath(commands, selectedCommandPath, cmd));
      return;
    }
    insertAtArrow(cmd);
  };

  const submitComment = () => {
    if (!canAddComment) return;
    const cmd: SkipCommand = { cmd: "comment", text: commentText.trim() };
    if (isModifyComment && selectedCommandPath) {
      setCommands(replaceProcessCommandAtPath(commands, selectedCommandPath, cmd));
      return;
    }
    insertAtArrow(cmd);
  };

  const deleteCommandAtPath = (path: string) => {
    const next = deleteProcessCommandAtPath(commands, path);
    setCommands(next);
    if (selectedCommandPath === path) {
      setSelectedCommandPath(null);
    }
  };

  const moveCommandAtPath = (path: string, direction: "up" | "down") => {
    const moved = moveProcessCommandAtPath(commands, path, direction);
    if (!moved) return;
    setCommands(moved.commands);
    setSelectedCommandPath(moved.newPath);
  };

  const canMoveCommand = (path: string, direction: "up" | "down") =>
    canMoveProcessCommandAtPath(commands, path, direction);

  const selectCommandPath = (path: string) => {
    setSelectedCommandPath(path);
    const cmd = getProcessCommandAtPath(commands, path);
    if (!cmd) return;
    if (cmd.cmd === "if") {
      setPanel("if");
      setIfBuilder(ifBuilderFromCommand(cmd));
    } else if (cmd.cmd === "skip") {
      setPanel("skipTo");
      setSkipToDest(String(cmd.to ?? END_OF_FORM));
    } else if (cmd.cmd === "set") {
      setPanel("set");
      setSetBuilder(setBuilderFromCommand(cmd));
    } else if (cmd.cmd === "comment") {
      setPanel("comment");
      setCommentText(String(cmd.text ?? ""));
    }
  };

  // When selection changes while a panel is open, keep builder fields in sync.
  useLayoutEffect(() => {
    if (!selectedCommandPath) return;
    const cmd = getProcessCommandAtPath(commands, selectedCommandPath);
    if (!cmd) return;
    if (panel === "if" && cmd.cmd === "if") {
      setIfBuilder(ifBuilderFromCommand(cmd));
    }
    if (panel === "skipTo" && cmd.cmd === "skip") {
      setSkipToDest(String(cmd.to ?? END_OF_FORM));
    }
    if (panel === "set" && cmd.cmd === "set") {
      setSetBuilder(setBuilderFromCommand(cmd));
    }
    if (panel === "comment" && cmd.cmd === "comment") {
      setCommentText(String(cmd.text ?? ""));
    }
  }, [selectedCommandPath, commands, panel]);

  const openPanel = (mode: PanelMode) => {
    if (panel === mode) {
      closeBuilderPanel();
      return;
    }
    setPanel(mode);
    // Match Process IF/Set: keep in-progress drafts when re-opening the same panel
    // (clicking away must not wipe a complex If before Add ↓).
    if (mode === "if") {
      setIfBuilder((prev) => (ifBuilderHasDraft(prev) ? prev : freshIfBuilder()));
    }
    if (mode === "set") {
      setSetBuilder((prev) => (setBuilderHasDraft(prev) ? prev : freshSetBuilder()));
    }
    if (mode === "skipTo") {
      // Keep current dest when still valid; otherwise End of Form (not first FIB —
      // that made Skip after FIB1 look like a no-op when Add used FIB1 by accident).
      setSkipToDest((prev) =>
        destinations.some((d) => d.value === prev) ? prev : END_OF_FORM,
      );
    }
  };

  const toolbarDeleteEnabled = selectedCommandPath != null;

  // Portal to document.body so the dialog is not clipped by MDI overflow / PE stacking
  // (Process IF is embedded in ProcessEditor, so it never hit this).
  return createPortal(
    <div className="modal-overlay skip-modal-overlay" role="presentation">
      <div
        ref={dialogRef}
        className="modal-dialog skip-instructions-dialog"
        role="dialog"
        aria-labelledby="skip-instructions-title"
        aria-modal="true"
        style={{ transform: `translate(${offset.x}px, ${offset.y}px)` }}
      >
        <div
          className="modal-header skip-dialog-titlebar"
          title="Drag to reposition"
          {...titleBarProps}
        >
          <h2 id="skip-instructions-title">Edit Skip Instructions — {projectName}</h2>
        </div>

        <div className="skip-dialog-toolbar explorer-toolbar" role="toolbar" aria-label="Edit commands">
          {SKIP_TOOLBAR.map(({ id, label, icon }) => {
            const isDelete = id === "delete";
            const enabled = isDelete && toolbarDeleteEnabled;
            return (
              <span key={id} className="win-tip" data-tip={label}>
                <button
                  type="button"
                  disabled={!enabled}
                  title={enabled ? label : `${label} (not yet)`}
                  aria-label={label}
                  onClick={() => {
                    if (isDelete && selectedCommandPath) {
                      deleteCommandAtPath(selectedCommandPath);
                    }
                  }}
                >
                  {icon}
                </button>
              </span>
            );
          })}
        </div>

        <div className="skip-dialog-body">
          <aside className="skip-dialog-palette">
            <div className="skip-dialog-palette-title">Statements</div>
            {SKIP_STATEMENT_BUTTONS.map(({ key, label }) => (
              <button
                key={key}
                type="button"
                className={panel === key ? "active" : ""}
                onClick={() => openPanel(key as PanelMode)}
              >
                {label}
              </button>
            ))}
          </aside>

          <div className="skip-dialog-main">
            {panel === "if" && (
              <IfStatementBuilder
                knownVariables={knownVariables}
                state={ifBuilder}
                onStateChange={setIfBuilder}
                submitLabel={isModifyIf ? "Modify" : "Add ↓"}
                onSubmit={submitIf}
              />
            )}

            {panel === "skipTo" && (
              <div className="skip-statement-panel skip-skipto-builder">
                <div className="skip-statement-panel-tab">Skip To</div>
                <div className="skip-statement-panel-body skip-skipto-body">
                  <label>
                    Skip to:
                    <select
                      value={skipToDest}
                      onChange={(e) => setSkipToDest(e.target.value)}
                    >
                      {destinations.map((d) => (
                        <option key={d.value} value={d.value}>
                          {d.label}
                        </option>
                      ))}
                    </select>
                  </label>
                  <button
                    type="button"
                    className="skip-add-btn"
                    onClick={submitSkipTo}
                  >
                    {isModifySkipTo ? "Modify" : "Add ↓"}
                  </button>
                </div>
              </div>
            )}

            {panel === "set" && (
              <SetStatementBuilder
                state={setBuilder}
                onStateChange={setSetBuilder}
                submitLabel={isModifySet ? "Modify" : "Add ↓"}
                onSubmit={submitSet}
                knownVariables={knownVariables}
              />
            )}

            {panel === "comment" && (
              <div className="skip-statement-panel skip-comment-builder">
                <div className="skip-statement-panel-tab">Comment</div>
                <div className="skip-statement-panel-body skip-comment-body">
                  <FieldTextInput
                    className="skip-comment-input"
                    placeholder="Comment text"
                    value={commentText}
                    onValueChange={setCommentText}
                  />
                  <div className="skip-comment-add-row">
                    <button
                      type="button"
                      className="skip-add-btn"
                      disabled={!canAddComment}
                      onClick={submitComment}
                    >
                      {isModifyComment ? "Modify" : "Add ↓"}
                    </button>
                  </div>
                </div>
              </div>
            )}

            <div className="skip-script-panel" ref={scriptRef}>
              {isEmpty && panel === "none" && (
                <p className="skip-empty-hint">
                  To create a new statement, click one of the buttons in the Statements palette.
                </p>
              )}
              {!isEmpty && (
                <p className="skip-insert-path-hint">
                  {selectedCommandPath ? (
                    <>
                      Editing selected statement — change the builder and click{" "}
                      <strong>Modify</strong>, or click an insert gap to add a new line.
                    </>
                  ) : (
                    <>
                      Insertion point: <strong>{insertPathLabel}</strong> — click inside{" "}
                      <code>( )</code>, <em>Otherwise</em>, or a line to move it.
                    </>
                  )}
                </p>
              )}
              <SkipScriptView
                lines={scriptLines}
                insertPath={insertPath}
                insertIndex={insertIndex}
                onSelectInsertPoint={setInsertPoint}
                selectedCommandPath={selectedCommandPath}
                onSelectCommandPath={selectCommandPath}
                showLineControls
                onMoveCommand={moveCommandAtPath}
                onDeleteCommand={deleteCommandAtPath}
                canMoveCommand={canMoveCommand}
              />
            </div>
          </div>
        </div>

        <div className="modal-footer skip-dialog-footer">
          <button type="button" onClick={() => finish(commands)}>
            Close
          </button>
        </div>
      </div>
    </div>,
    document.body,
  );
}

function CutIcon() {
  return (
    <svg width="14" height="14" viewBox="0 0 16 16" aria-hidden>
      <path fill="currentColor" d="M4 2l2 2-2 2 1 1 2-2 2 2-1-1-2 2-2-2 1-1-2-2 2-2-1-1z" />
      <circle cx="12" cy="4" r="2" fill="none" stroke="currentColor" strokeWidth="1.2" />
      <circle cx="12" cy="12" r="2" fill="none" stroke="currentColor" strokeWidth="1.2" />
    </svg>
  );
}

function CopyIcon() {
  return (
    <svg width="14" height="14" viewBox="0 0 16 16" aria-hidden>
      <rect x="5" y="5" width="8" height="9" fill="none" stroke="currentColor" strokeWidth="1.2" />
      <rect x="3" y="2" width="8" height="9" fill="var(--panel,#fff)" stroke="currentColor" strokeWidth="1.2" />
    </svg>
  );
}

function PasteIcon() {
  return (
    <svg width="14" height="14" viewBox="0 0 16 16" aria-hidden>
      <rect x="4" y="2" width="8" height="3" fill="none" stroke="currentColor" strokeWidth="1.2" />
      <rect x="3" y="4" width="10" height="10" fill="none" stroke="currentColor" strokeWidth="1.2" />
    </svg>
  );
}

function DeleteIcon() {
  return (
    <svg width="14" height="14" viewBox="0 0 16 16" aria-hidden>
      <path fill="currentColor" d="M5 3h6l1 2h3v1H2V5h3l1-2zm1 4h1v6H6V7zm3 0h1v6H9V7zM4 14h8l1-8H3l1 8z" />
    </svg>
  );
}

function UndoIcon() {
  return (
    <svg width="14" height="14" viewBox="0 0 16 16" aria-hidden>
      <path
        fill="none"
        stroke="currentColor"
        strokeWidth="1.4"
        d="M3 8a5 5 0 0 1 8-3.5M3 8l2-2M3 8l2 2"
      />
    </svg>
  );
}

function RedoIcon() {
  return (
    <svg width="14" height="14" viewBox="0 0 16 16" aria-hidden>
      <path
        fill="none"
        stroke="currentColor"
        strokeWidth="1.4"
        d="M13 8a5 5 0 0 0-8-3.5M13 8l-2-2M13 8l-2 2"
      />
    </svg>
  );
}
