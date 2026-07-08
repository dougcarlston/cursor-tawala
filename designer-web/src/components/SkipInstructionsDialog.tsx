import { useMemo, useRef, useState } from "react";
import type { FormItem, SkipCommand, TawalaForm, TawalaProject } from "@/types/tawala";
import { FieldTextInput, QualifiedFieldInput } from "@/components/FieldDropInputs";
import { SkipScriptView } from "@/components/SkipScriptView";
import { useDraggableDialog } from "@/hooks/useDraggableDialog";
import { buildScriptLines, findInsertionLineIndex, formatInsertPathLabel } from "@/lib/skipScript";
import { getCommandsAtInsertPath } from "@/lib/skipInsertPath";
import { isValidIfConditionField, setActiveFieldTarget } from "@/lib/fieldInsertion";
import { collectKnownVariables } from "@/lib/projectModel";
import {
  SKIP_OPERATORS,
  SKIP_OPERATOR_LABELS,
  UNARY_SKIP_OPERATORS,
  buildConditionFromRows,
} from "@/lib/skipSummary";

interface Props {
  projectName: string;
  project: TawalaProject;
  form: TawalaForm;
  commands: SkipCommand[];
  onSave: (commands: SkipCommand[]) => void;
}

type Combinator = "and" | "or";

interface ConditionRow {
  field: string;
  op: string;
  value: string;
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

function rowsAreValid(rows: ConditionRow[], knownVariables: ReadonlySet<string>): boolean {
  return rows.every((r) => {
    if (!isValidIfConditionField(r.field, knownVariables)) return false;
    if (UNARY_SKIP_OPERATORS.has(r.op)) return true;
    return r.value.trim().length > 0;
  });
}

function expressionHasArithmetic(value: string): boolean {
  return /[+\-*/]/.test(value);
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

  const [combinator, setCombinator] = useState<Combinator>("and");
  const [conditionRows, setConditionRows] = useState<ConditionRow[]>([
    { field: "", op: "isBlank", value: "" },
  ]);
  const [hasElse, setHasElse] = useState(false);
  const [skipToDest, setSkipToDest] = useState("__EndOfForm__");
  const [setField, setSetField] = useState("");
  const [setValue, setSetValue] = useState("");
  const [setArithmeticAsText, setSetArithmeticAsText] = useState(false);
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
  const canAddIf = rowsAreValid(conditionRows, knownVariables);
  const canAddSet = setField.trim().length > 0 && setValue.trim().length > 0;
  const canAddComment = commentText.trim().length > 0;
  const setArithmeticEnabled = expressionHasArithmetic(setValue);

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

  /** Collapse upper builder when the same Statements palette button is clicked again. */
  const closeBuilderPanel = () => {
    setPanel("none");
    setActiveFieldTarget(null);
  };

  const addIfBlock = () => {
    if (!canAddIf) return;
    const condition = buildConditionFromRows(combinator, conditionRows);
    const ifCmd: SkipCommand = {
      cmd: "if",
      condition,
      then: [],
      ...(hasElse ? { else: [] } : {}),
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

  const addSkipTo = () => {
    appendToTarget({ cmd: "skip", to: skipToDest });
  };

  const addSet = () => {
    if (!canAddSet) return;
    const cmd: SkipCommand = {
      cmd: "set",
      field: setField.trim(),
      value: setValue,
    };
    if (setArithmeticAsText) {
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
    if (mode === "skipTo" && destinations.length) {
      setSkipToDest(destinations[0].value);
    }
  };

  const updateRow = (index: number, patch: Partial<ConditionRow>) => {
    setConditionRows((rows) => rows.map((r, i) => (i === index ? { ...r, ...patch } : r)));
  };

  const addRowAfter = (index: number) => {
    setConditionRows((rows) => {
      const next = [...rows];
      next.splice(index + 1, 0, { field: "", op: "isBlank", value: "" });
      return next;
    });
  };

  const removeRow = (index: number) => {
    setConditionRows((rows) => (rows.length <= 1 ? rows : rows.filter((_, j) => j !== index)));
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
              <div className="skip-statement-panel skip-if-builder">
                <div className="skip-statement-panel-tab">If</div>
                <div className="skip-statement-panel-body">
                  <p className="skip-if-intro">
                    If{" "}
                    {conditionRows.length > 1 ? (
                      <>
                        <select
                          value={combinator}
                          onChange={(e) => setCombinator(e.target.value as Combinator)}
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
                  {conditionRows.map((row, i) => (
                    <div key={i} className="skip-if-row">
                      <QualifiedFieldInput
                        className="skip-if-field"
                        placeholder="Form:Field"
                        knownVariables={knownVariables}
                        value={row.field}
                        onValueChange={(v) => updateRow(i, { field: v })}
                      />
                      <select
                        value={row.op}
                        onChange={(e) => updateRow(i, { op: e.target.value })}
                        aria-label="Operator"
                        className="skip-if-operator"
                      >
                        {SKIP_OPERATORS.map((op) => (
                          <option key={op} value={op}>
                            {SKIP_OPERATOR_LABELS[op]}
                          </option>
                        ))}
                      </select>
                      {!UNARY_SKIP_OPERATORS.has(row.op) ? (
                        <FieldTextInput
                          className="skip-if-value"
                          placeholder="Value"
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
                        disabled={conditionRows.length <= 1}
                        onClick={() => removeRow(i)}
                      >
                        −
                      </button>
                    </div>
                  ))}
                  <label className="skip-if-else">
                    <input
                      type="checkbox"
                      checked={hasElse}
                      onChange={(e) => setHasElse(e.target.checked)}
                    />
                    Otherwise execute second set of commands
                  </label>
                  <div className="skip-if-add-row">
                    <button
                      type="button"
                      className="skip-add-btn"
                      disabled={!canAddIf}
                      onClick={addIfBlock}
                    >
                      Add ↓
                    </button>
                  </div>
                </div>
              </div>
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
                  <button type="button" className="skip-add-btn" onClick={addSkipTo}>
                    Add ↓
                  </button>
                </div>
              </div>
            )}

            {panel === "set" && (
              <div className="skip-statement-panel skip-set-builder">
                <div className="skip-statement-panel-tab">Set</div>
                <div className="skip-statement-panel-body skip-set-body">
                  <div className="skip-set-row">
                    <QualifiedFieldInput
                      className="skip-set-field"
                      placeholder="Form:Field or Variable Name"
                      value={setField}
                      onValueChange={setSetField}
                    />
                    <span className="skip-set-to">to</span>
                    <FieldTextInput
                      className="skip-set-expression"
                      placeholder="Value or expression"
                      value={setValue}
                      onValueChange={(v) => {
                        setSetValue(v);
                        if (!expressionHasArithmetic(v)) setSetArithmeticAsText(false);
                      }}
                    />
                  </div>
                  <label
                    className={`skip-set-arithmetic${setArithmeticEnabled ? "" : " disabled"}`}
                  >
                    <input
                      type="checkbox"
                      checked={setArithmeticAsText}
                      disabled={!setArithmeticEnabled}
                      onChange={(e) => setSetArithmeticAsText(e.target.checked)}
                    />
                    Treat arithmetic expression as text (do not interpret +, -, * or / as math)
                  </label>
                  <div className="skip-set-add-row">
                    <button
                      type="button"
                      className="skip-add-btn"
                      disabled={!canAddSet}
                      onClick={addSet}
                    >
                      Add ↓
                    </button>
                  </div>
                </div>
              </div>
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
