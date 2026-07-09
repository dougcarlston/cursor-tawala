import { useMemo, useRef, useState } from "react";
import type { FormItem, SkipCommand, TawalaForm, TawalaProject } from "@/types/tawala";
import { IfStatementBuilder } from "@/components/IfStatementBuilder";
import { SetStatementBuilder } from "@/components/SetStatementBuilder";
import { FieldTextInput } from "@/components/FieldDropInputs";
import { SkipScriptView } from "@/components/SkipScriptView";
import { useDraggableDialog } from "@/hooks/useDraggableDialog";
import { buildScriptLines, findInsertionLineIndex, formatInsertPathLabel } from "@/lib/skipScript";
import { getCommandsAtInsertPath } from "@/lib/skipInsertPath";
import { setActiveFieldTarget } from "@/lib/fieldInsertion";
import { collectKnownVariables } from "@/lib/projectModel";
import {
  EMPTY_IF_BUILDER,
  EMPTY_SET_BUILDER,
  setBuilderIsValid,
  type IfBuilderState,
  type SetBuilderState,
} from "@/lib/statementBuilders";
import { buildConditionFromRows } from "@/lib/skipSummary";

interface Props {
  projectName: string;
  project: TawalaProject;
  form: TawalaForm;
  commands: SkipCommand[];
  onSave: (commands: SkipCommand[]) => void;
}

const SKIP_STATEMENT_BUTTONS = [
  { key: "if", label: "If" },
  { key: "skipTo", label: "SkipTo" },
  { key: "set", label: "Set" },
  { key: "comment", label: "Comment" },
] as const;

type PanelMode = "none" | "if" | "skipTo" | "set" | "comment";

const SKIP_TOOLBAR = [
  { id: "cut", label: "Cut", icon: <CutIcon /> },
  { id: "copy", label: "Copy", icon: <CopyIcon /> },
  { id: "paste", label: "Paste", icon: <PasteIcon /> },
  { id: "delete", label: "Delete", icon: <DeleteIcon /> },
  { id: "undo", label: "Undo", icon: <UndoIcon /> },
  { id: "redo", label: "Redo", icon: <RedoIcon /> },
] as const;

function skipDestinations(items: FormItem[]): { value: string; label: string }[] {
  const dests = items
    .filter((i) => i.type !== "skipInstructions" && i.type !== "break")
    .map((i) => ({ value: i.label, label: i.label }));
  dests.push({ value: "__EndOfForm__", label: "End of Form" });
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
  onSave,
}: Props) {
  const [commands, setCommands] = useState<SkipCommand[]>(() =>
    structuredClone(Array.isArray(initialCommands) ? initialCommands : []),
  );
  const [insertPath, setInsertPath] = useState("root");
  const [panel, setPanel] = useState<PanelMode>("none");
  const [ifBuilder, setIfBuilder] = useState<IfBuilderState>(EMPTY_IF_BUILDER);
  const [setBuilder, setSetBuilder] = useState<SetBuilderState>(EMPTY_SET_BUILDER);
  const [skipToDest, setSkipToDest] = useState("__EndOfForm__");
  const [commentText, setCommentText] = useState("");

  const scriptRef = useRef<HTMLDivElement>(null);
  const destinations = skipDestinations(form.items);
  const { offset, titleBarProps } = useDraggableDialog({ x: -160, y: -30 });

  const scriptLines = useMemo(() => buildScriptLines(commands), [commands]);
  const knownVariables = useMemo(
    () => collectKnownVariables(project, commands),
    [project, commands],
  );
  const insertAfterIndex = useMemo(
    () => findInsertionLineIndex(scriptLines, insertPath),
    [scriptLines, insertPath],
  );
  const isEmpty = commands.length === 0;
  const canAddComment = commentText.trim().length > 0;
  const insertPathLabel = formatInsertPathLabel(insertPath);

  const appendToTarget = (cmd: SkipCommand) => {
    const nextCommands = structuredClone(commands);
    const target = getCommandsAtInsertPath(nextCommands, insertPath);
    target.push(structuredClone(cmd));
    setCommands(nextCommands);
    requestAnimationFrame(() => {
      scriptRef.current?.scrollTo(0, scriptRef.current.scrollHeight);
    });
  };

  const closeBuilderPanel = () => {
    setPanel("none");
    setActiveFieldTarget(null);
  };

  const addIfBlock = () => {
    const condition = buildConditionFromRows(ifBuilder.combinator, ifBuilder.rows);
    const ifCmd: SkipCommand = {
      cmd: "if",
      condition,
      then: [],
      ...(ifBuilder.hasElse ? { else: [] } : {}),
    };
    const nextCommands = structuredClone(commands);
    const target = getCommandsAtInsertPath(nextCommands, insertPath);
    const ifIndex = target.length;
    target.push(ifCmd);
    const thenPath =
      insertPath === "root" ? `root/${ifIndex}/then` : `${insertPath}/${ifIndex}/then`;
    setCommands(nextCommands);
    setInsertPath(thenPath);
  };

  const addSet = () => {
    if (!setBuilderIsValid(setBuilder)) return;
    const cmd: SkipCommand = {
      cmd: "set",
      field: setBuilder.field.trim(),
      value: setBuilder.value,
    };
    if (setBuilder.arithmeticAsText) {
      cmd.arithmeticAsText = true;
    }
    appendToTarget(cmd);
  };

  const addComment = () => {
    if (!canAddComment) return;
    appendToTarget({ cmd: "comment", text: commentText.trim() });
  };

  const openPanel = (mode: PanelMode) => {
    if (panel === mode) {
      closeBuilderPanel();
      return;
    }
    setPanel(mode);
    if (mode === "if") setIfBuilder(EMPTY_IF_BUILDER);
    if (mode === "set") setSetBuilder(EMPTY_SET_BUILDER);
    if (mode === "skipTo" && destinations.length) {
      setSkipToDest(destinations[0].value);
    }
  };

  return (
    <div className="modal-overlay skip-modal-overlay" role="presentation">
      <div
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
          {SKIP_TOOLBAR.map(({ id, label, icon }) => (
            <span key={id} className="win-tip" data-tip={label}>
              <button type="button" disabled title={`${label} (not yet)`} aria-label={label}>
                {icon}
              </button>
            </span>
          ))}
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
                submitLabel="Add ↓"
                onSubmit={addIfBlock}
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
                    onClick={() => appendToTarget({ cmd: "skip", to: skipToDest })}
                  >
                    Add ↓
                  </button>
                </div>
              </div>
            )}

            {panel === "set" && (
              <SetStatementBuilder
                state={setBuilder}
                onStateChange={setSetBuilder}
                submitLabel="Add ↓"
                onSubmit={addSet}
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
                      onClick={addComment}
                    >
                      Add ↓
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
                  Insertion point: <strong>{insertPathLabel}</strong> — click inside{" "}
                  <code>( )</code>, <em>Otherwise</em>, or a line to move it.
                </p>
              )}
              <SkipScriptView
                lines={scriptLines}
                insertPath={insertPath}
                insertAfterIndex={insertAfterIndex}
                onSelectInsertPath={setInsertPath}
              />
            </div>
          </div>
        </div>

        <div className="modal-footer skip-dialog-footer">
          <button type="button" onClick={() => onSave(commands)}>
            Close
          </button>
        </div>
      </div>
    </div>
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
