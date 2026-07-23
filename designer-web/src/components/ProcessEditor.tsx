import { useEffect, useLayoutEffect, useMemo, useRef, useState, type ReactNode } from "react";
import { CommentStatementBuilder } from "@/components/CommentStatementBuilder";
import { DeleteStatementBuilder } from "@/components/DeleteStatementBuilder";
import { AppendStatementBuilder } from "@/components/AppendStatementBuilder";
import { ForEachStatementBuilder } from "@/components/ForEachStatementBuilder";
import { GetStatementBuilder } from "@/components/GetStatementBuilder";
import { IfStatementBuilder } from "@/components/IfStatementBuilder";
import { ProcessConnectionDialog } from "@/components/ProcessConnectionDialog";
import { SendStatementBuilder } from "@/components/SendStatementBuilder";
import { SetStatementBuilder } from "@/components/SetStatementBuilder";
import { ShowStatementBuilder } from "@/components/ShowStatementBuilder";
import { SkipScriptView } from "@/components/SkipScriptView";
import { insertCommandAtPoint } from "@/lib/processInsert";
import {
  hasProcessStatementDrag,
  hasProcessStatementReorderDrag,
  readProcessStatementDrag,
  readProcessStatementReorderDrag,
  setProcessStatementReorderDrag,
} from "@/lib/designerDrag";
import { formLinksForProcess, collectKnownVariables } from "@/lib/projectModel";
import {
  PROCESS_PANEL_LABELS,
  PROCESS_STATEMENT_PALETTE,
} from "@/processStatements";
import {
  buildProcessScriptLines,
  canMoveProcessCommandAtPath,
  deleteProcessCommandAtPath,
  getProcessCommandAtPath,
  moveProcessCommandAtPath,
  replaceProcessCommandAtPath,
} from "@/lib/processScript";
import {
  EMPTY_APPEND_BUILDER,
  EMPTY_COMMENT_BUILDER,
  EMPTY_DELETE_BUILDER,
  EMPTY_FOREACH_BUILDER,
  EMPTY_GET_BUILDER,
  EMPTY_IF_BUILDER,
  EMPTY_SEND_BUILDER,
  EMPTY_SET_BUILDER,
  EMPTY_SHOW_BUILDER,
  appendBuilderFromCommand,
  appendBuilderHasDraft,
  buildAppendCommand,
  buildCommentCommand,
  buildDeleteCommand,
  buildForEachCommand,
  buildGetCommand,
  buildSendCommand,
  buildShowCommand,
  collectProcessRecordLists,
  collectProcessRecordNames,
  commentBuilderFromCommand,
  commentBuilderHasDraft,
  deleteBuilderFromCommand,
  deleteBuilderHasDraft,
  foreachBuilderFromCommand,
  foreachBuilderHasDraft,
  getBuilderFromCommand,
  getBuilderHasDraft,
  ifBuilderFromCommand,
  ifBuilderHasDraft,
  nextRecordListName,
  rowsAreValid,
  sendBuilderFromCommand,
  sendBuilderHasDraft,
  setBuilderFromCommand,
  setBuilderHasDraft,
  showBuilderFromCommand,
  showBuilderHasDraft,
  type AppendBuilderState,
  type CommentBuilderState,
  type DeleteBuilderState,
  type ForEachBuilderState,
  type GetBuilderState,
  type IfBuilderState,
  type SendBuilderState,
  type SetBuilderState,
  type ShowBuilderState,
} from "@/lib/statementBuilders";
import { buildConditionFromRows } from "@/lib/skipSummary";
import { setActiveFieldTarget } from "@/lib/fieldInsertion";
import { useProjectStore } from "@/store/projectStore";
import { TawalaProcessCommand } from "@/types/tawala";

interface Props {
  processName: string;
}

function isShowCommandType(cmd: TawalaProcessCommand): boolean {
  return cmd.cmd === "show" || cmd.cmd === "showDocument" || cmd.cmd === "edit";
}

function ProcessConnectionBanner({
  links,
  onConnectClick,
}: {
  links: ReturnType<typeof formLinksForProcess>;
  onConnectClick: () => void;
}) {
  const hereLink = (
    <button type="button" className="process-connection-link" onClick={onConnectClick}>
      here
    </button>
  );

  const preLinks = links.filter((l) => l.role === "Pre");
  const postLinks = links.filter((l) => l.role === "Post");

  let message: ReactNode;
  if (links.length === 0) {
    // Legacy Post-centric default when nothing is linked (ViewInfoBarNoConnections).
    message = (
      <>
        Not connected as Pre-Process or Post-Process to any Form. Click {hereLink} to change.
      </>
    );
  } else if (preLinks.length === 0 && postLinks.length === 1) {
    message = (
      <>
        Connected as Post-Process to Form &apos;{postLinks[0].formName}&apos;. Click {hereLink}{" "}
        to change.
      </>
    );
  } else if (preLinks.length === 0 && postLinks.length > 1) {
    // Legacy ViewInfoBarManyConnections — same process may post on many forms.
    message = (
      <>
        Connected as Post-Process to {postLinks.length} Forms. Click {hereLink} to change.
      </>
    );
  } else if (postLinks.length === 0 && preLinks.length === 1) {
    message = (
      <>
        Connected as Pre-Process to Form &apos;{preLinks[0].formName}&apos;. Click {hereLink}{" "}
        to change.
      </>
    );
  } else if (postLinks.length === 0 && preLinks.length > 1) {
    message = (
      <>
        Connected as Pre-Process to {preLinks.length} Forms. Click {hereLink} to change.
      </>
    );
  } else {
    const parts = links.map((l) => `${l.role}-Process to '${l.formName}'`).join("; ");
    message = (
      <>
        Connected: {parts}. Click {hereLink} to change.
      </>
    );
  }

  return <div className="process-connection-banner">{message}</div>;
}

export function ProcessEditor({ processName }: Props) {
  const project = useProjectStore((s) => s.project);
  const selection = useProjectStore((s) => s.selection);
  const processInsertPath = useProjectStore((s) => s.processInsertPath);
  const processInsertIndex = useProjectStore((s) => s.processInsertIndex);
  const selectedProcessCommandPath = useProjectStore((s) => s.selectedProcessCommandPath);
  const processStatementPanel = useProjectStore((s) => s.processStatementPanel);
  const setProcessInsertPoint = useProjectStore((s) => s.setProcessInsertPoint);
  const setSelectedProcessCommandPath = useProjectStore((s) => s.setSelectedProcessCommandPath);
  const moveSelectedProcessCommand = useProjectStore((s) => s.moveSelectedProcessCommand);
  const moveProcessCommandBefore = useProjectStore((s) => s.moveProcessCommandBefore);
  const insertProcessCommand = useProjectStore((s) => s.insertProcessCommand);
  const toggleProcessStatementPanel = useProjectStore((s) => s.toggleProcessStatementPanel);
  const updateProcessCommands = useProjectStore((s) => s.updateProcessCommands);
  const proc = project.processes?.find((p) => p.name === processName);
  const [ifBuilder, setIfBuilder] = useState<IfBuilderState>(EMPTY_IF_BUILDER);
  const [setBuilder, setSetBuilder] = useState<SetBuilderState>(EMPTY_SET_BUILDER);
  const [showBuilder, setShowBuilder] = useState<ShowBuilderState>(EMPTY_SHOW_BUILDER);
  const [sendBuilder, setSendBuilder] = useState<SendBuilderState>(EMPTY_SEND_BUILDER);
  const [appendBuilder, setAppendBuilder] = useState<AppendBuilderState>(EMPTY_APPEND_BUILDER);
  const [getBuilder, setGetBuilder] = useState<GetBuilderState>(EMPTY_GET_BUILDER);
  const [forEachBuilder, setForEachBuilder] = useState<ForEachBuilderState>(EMPTY_FOREACH_BUILDER);
  const [deleteBuilder, setDeleteBuilder] = useState<DeleteBuilderState>(EMPTY_DELETE_BUILDER);
  const [commentBuilder, setCommentBuilder] = useState<CommentBuilderState>(EMPTY_COMMENT_BUILDER);
  const [connectionDialogOpen, setConnectionDialogOpen] = useState(false);
  const [dragInsertPath, setDragInsertPath] = useState<string | null>(null);
  const [dragInsertIndex, setDragInsertIndex] = useState<number | null>(null);
  const [dragCaretTop, setDragCaretTop] = useState<number | null>(null);
  const scriptRef = useRef<HTMLDivElement>(null);

  const commands = proc?.commands ?? [];
  const scriptLines = useMemo(() => buildProcessScriptLines(commands), [commands]);
  const knownVariables = useMemo(
    () => collectKnownVariables(project, commands),
    [project, commands],
  );
  const formNames = useMemo(() => project.forms.map((f) => f.name), [project.forms]);
  const formLinks = useMemo(
    () => formLinksForProcess(project, processName),
    [project, processName],
  );
  const documentNames = useMemo(
    () => (project.documents ?? []).map((d) => d.name),
    [project.documents],
  );
  const processRecordLists = useMemo(() => collectProcessRecordLists(commands), [commands]);
  const processRecordNames = useMemo(() => collectProcessRecordNames(commands), [commands]);

  const insertAtArrow = (cmd: TawalaProcessCommand) => {
    const result = insertCommandAtPoint(
      commands,
      processInsertPath,
      processInsertIndex,
      cmd,
    );
    updateProcessCommands(processName, result.commands);
    setProcessInsertPoint(result.insertPath, result.insertIndex);
  };

  const selectedCommand =
    selectedProcessCommandPath != null
      ? getProcessCommandAtPath(commands, selectedProcessCommandPath)
      : null;

  const isActiveProcess = selection.kind === "process" && selection.name === processName;
  const isModifyIf =
    processStatementPanel === "if" &&
    selectedCommand?.cmd === "if" &&
    selectedProcessCommandPath != null;
  const isModifySet =
    processStatementPanel === "set" &&
    selectedCommand?.cmd === "set" &&
    selectedProcessCommandPath != null;
  const isShowCommand =
    selectedCommand?.cmd === "show" ||
    selectedCommand?.cmd === "showDocument" ||
    selectedCommand?.cmd === "edit";
  const isModifyShow =
    processStatementPanel === "show" && isShowCommand && selectedProcessCommandPath != null;
  const isModifySend =
    processStatementPanel === "send" &&
    selectedCommand?.cmd === "send" &&
    selectedProcessCommandPath != null;
  const isModifyAppend =
    processStatementPanel === "append" &&
    selectedCommand?.cmd === "append" &&
    selectedProcessCommandPath != null;
  const isModifyGet =
    processStatementPanel === "get" &&
    selectedCommand?.cmd === "get" &&
    selectedProcessCommandPath != null;
  const isModifyForEach =
    processStatementPanel === "foreach" &&
    selectedCommand?.cmd === "foreach" &&
    selectedProcessCommandPath != null;
  const isModifyDelete =
    processStatementPanel === "delete" &&
    selectedCommand?.cmd === "delete" &&
    selectedProcessCommandPath != null;
  const isModifyComment =
    processStatementPanel === "comment" &&
    selectedCommand?.cmd === "comment" &&
    selectedProcessCommandPath != null;
  const hasPropertyPanel =
    isActiveProcess &&
    (processStatementPanel === "if" ||
      processStatementPanel === "set" ||
      processStatementPanel === "show" ||
      processStatementPanel === "send" ||
      processStatementPanel === "append" ||
      processStatementPanel === "get" ||
      processStatementPanel === "foreach" ||
      processStatementPanel === "delete" ||
      processStatementPanel === "comment");
  const showPaletteHint =
    isActiveProcess && commands.length === 0 && processStatementPanel === "none";

  useEffect(() => {
    if (!isActiveProcess) return;
    if (
      processStatementPanel === "if" ||
      processStatementPanel === "set" ||
      processStatementPanel === "show" ||
      processStatementPanel === "send" ||
      processStatementPanel === "append" ||
      processStatementPanel === "get" ||
      processStatementPanel === "foreach" ||
      processStatementPanel === "delete" ||
      processStatementPanel === "comment"
    )
      return;
    setActiveFieldTarget(null);
  }, [isActiveProcess, processStatementPanel]);

  useEffect(() => {
    if (!isActiveProcess) return;
    return () => setActiveFieldTarget(null);
  }, [isActiveProcess]);

  // Load builder state synchronously when script selection changes (before Modify click).
  useLayoutEffect(() => {
    if (!isActiveProcess || !selectedProcessCommandPath) return;
    const cmd = getProcessCommandAtPath(commands, selectedProcessCommandPath);
    if (!cmd) return;
    if (processStatementPanel === "if" && cmd.cmd === "if") {
      setIfBuilder(ifBuilderFromCommand(cmd));
    } else if (processStatementPanel === "set" && cmd.cmd === "set") {
      setSetBuilder(setBuilderFromCommand(cmd));
    } else if (processStatementPanel === "show" && isShowCommandType(cmd)) {
      setShowBuilder(showBuilderFromCommand(cmd));
    } else if (processStatementPanel === "send" && cmd.cmd === "send") {
      setSendBuilder(sendBuilderFromCommand(cmd));
    } else if (processStatementPanel === "append" && cmd.cmd === "append") {
      setAppendBuilder(appendBuilderFromCommand(cmd));
    } else if (processStatementPanel === "get" && cmd.cmd === "get") {
      setGetBuilder(getBuilderFromCommand(cmd));
    } else if (processStatementPanel === "foreach" && cmd.cmd === "foreach") {
      setForEachBuilder(foreachBuilderFromCommand(cmd));
    } else if (processStatementPanel === "delete" && cmd.cmd === "delete") {
      setDeleteBuilder(deleteBuilderFromCommand(cmd));
    } else if (processStatementPanel === "comment" && cmd.cmd === "comment") {
      setCommentBuilder(commentBuilderFromCommand(cmd));
    }
  }, [selectedProcessCommandPath, commands, isActiveProcess, processStatementPanel]);

  useEffect(() => {
    if (!isActiveProcess) return;
    if (processStatementPanel === "if" && !selectedProcessCommandPath) {
      setIfBuilder((prev) => (ifBuilderHasDraft(prev) ? prev : EMPTY_IF_BUILDER));
    }
    if (processStatementPanel === "set" && !selectedProcessCommandPath) {
      setSetBuilder((prev) => (setBuilderHasDraft(prev) ? prev : EMPTY_SET_BUILDER));
    }
    if (processStatementPanel === "show" && !selectedProcessCommandPath) {
      setShowBuilder((prev) => (showBuilderHasDraft(prev) ? prev : EMPTY_SHOW_BUILDER));
    }
    if (processStatementPanel === "send" && !selectedProcessCommandPath) {
      setSendBuilder((prev) => (sendBuilderHasDraft(prev) ? prev : EMPTY_SEND_BUILDER));
    }
    if (processStatementPanel === "append" && !selectedProcessCommandPath) {
      setAppendBuilder((prev) => (appendBuilderHasDraft(prev) ? prev : EMPTY_APPEND_BUILDER));
    }
    if (processStatementPanel === "get" && !selectedProcessCommandPath) {
      setGetBuilder((prev) =>
        getBuilderHasDraft(prev)
          ? prev
          : { ...EMPTY_GET_BUILDER, recordList: nextRecordListName(commands) },
      );
    }
    if (processStatementPanel === "foreach" && !selectedProcessCommandPath) {
      setForEachBuilder((prev) => {
        if (foreachBuilderHasDraft(prev)) return prev;
        const lists = collectProcessRecordLists(commands);
        return {
          ...EMPTY_FOREACH_BUILDER,
          recordList: lists.length === 1 ? lists[0] : "",
        };
      });
    }
    if (processStatementPanel === "delete" && !selectedProcessCommandPath) {
      setDeleteBuilder((prev) => (deleteBuilderHasDraft(prev) ? prev : EMPTY_DELETE_BUILDER));
    }
    if (processStatementPanel === "comment" && !selectedProcessCommandPath) {
      setCommentBuilder((prev) => (commentBuilderHasDraft(prev) ? prev : EMPTY_COMMENT_BUILDER));
    }
    // Closing the Show panel drops multi-tab leftovers (Form/URL/etc.) so the next Add
    // can't accidentally commit a Visit to Form 1 the designer never meant.
    if (processStatementPanel === "none") {
      setShowBuilder(EMPTY_SHOW_BUILDER);
    }
  }, [processStatementPanel, selectedProcessCommandPath, isActiveProcess, commands]);

  useEffect(() => {
    if (!isActiveProcess) return;
    const onKeyDown = (e: KeyboardEvent) => {
      if (!e.altKey || selectedProcessCommandPath == null) return;
      if (e.key === "ArrowUp") {
        e.preventDefault();
        moveSelectedProcessCommand("up");
      } else if (e.key === "ArrowDown") {
        e.preventDefault();
        moveSelectedProcessCommand("down");
      }
    };
    window.addEventListener("keydown", onKeyDown);
    return () => window.removeEventListener("keydown", onKeyDown);
  }, [isActiveProcess, moveSelectedProcessCommand, selectedProcessCommandPath]);

  if (!proc) {
    return <div className="placeholder-editor">Process not found</div>;
  }

  const setCommands = (next: TawalaProcessCommand[]) => {
    updateProcessCommands(processName, next);
  };

  const submitIf = () => {
    if (!rowsAreValid(ifBuilder.rows, knownVariables)) return;
    const condition = buildConditionFromRows(ifBuilder.combinator, ifBuilder.rows);
    const modifyPath = selectedProcessCommandPath;
    if (modifyPath) {
      const existing = getProcessCommandAtPath(commands, modifyPath);
      if (existing?.cmd === "if") {
        const updated: TawalaProcessCommand = {
          cmd: "if",
          condition,
          then: (existing.then as TawalaProcessCommand[] | undefined) ?? [],
          ...(ifBuilder.hasElse
            ? { else: (existing.else as TawalaProcessCommand[] | undefined) ?? [] }
            : {}),
        };
        setCommands(replaceProcessCommandAtPath(commands, modifyPath, updated));
        return;
      }
    }
    const ifCmd: TawalaProcessCommand = {
      cmd: "if",
      condition,
      then: [],
      ...(ifBuilder.hasElse ? { else: [] } : {}),
    };
    insertAtArrow(ifCmd);
  };

  const submitSet = () => {
    const cmd: TawalaProcessCommand = {
      cmd: "set",
      field: setBuilder.field.trim(),
      value: setBuilder.value,
    };
    if (setBuilder.arithmeticAsText) {
      cmd.arithmeticAsText = true;
    }
    if (isModifySet && selectedProcessCommandPath) {
      setCommands(replaceProcessCommandAtPath(commands, selectedProcessCommandPath, cmd));
      return;
    }
    insertAtArrow(cmd);
  };

  const submitShow = () => {
    const cmd = buildShowCommand(showBuilder);
    if (isModifyShow && selectedProcessCommandPath) {
      setCommands(replaceProcessCommandAtPath(commands, selectedProcessCommandPath, cmd));
      // Clear multi-tab draft so a Form selection can't linger and Add a second Show.
      setShowBuilder(EMPTY_SHOW_BUILDER);
      return;
    }
    insertAtArrow(cmd);
    setShowBuilder(EMPTY_SHOW_BUILDER);
  };

  const submitSend = () => {
    const cmd = buildSendCommand(sendBuilder, knownVariables, false);
    if (isModifySend && selectedProcessCommandPath) {
      setCommands(replaceProcessCommandAtPath(commands, selectedProcessCommandPath, cmd));
      return;
    }
    insertAtArrow(cmd);
  };

  const submitAppend = () => {
    const cmd = buildAppendCommand(appendBuilder);
    if (isModifyAppend && selectedProcessCommandPath) {
      setCommands(replaceProcessCommandAtPath(commands, selectedProcessCommandPath, cmd));
      return;
    }
    insertAtArrow(cmd);
  };

  const submitGet = () => {
    const cmd = buildGetCommand(getBuilder);
    if (isModifyGet && selectedProcessCommandPath) {
      setCommands(replaceProcessCommandAtPath(commands, selectedProcessCommandPath, cmd));
      return;
    }
    insertAtArrow(cmd);
  };

  const submitForEach = () => {
    const existing =
      isModifyForEach && selectedProcessCommandPath
        ? getProcessCommandAtPath(commands, selectedProcessCommandPath)
        : null;
    const cmd = buildForEachCommand(
      forEachBuilder,
      (existing?.do as TawalaProcessCommand[] | undefined) ?? [],
    );
    if (isModifyForEach && selectedProcessCommandPath) {
      setCommands(replaceProcessCommandAtPath(commands, selectedProcessCommandPath, cmd));
      return;
    }
    insertAtArrow(cmd);
  };

  const submitDelete = () => {
    const cmd = buildDeleteCommand(deleteBuilder);
    if (isModifyDelete && selectedProcessCommandPath) {
      setCommands(replaceProcessCommandAtPath(commands, selectedProcessCommandPath, cmd));
      return;
    }
    insertAtArrow(cmd);
  };

  const submitComment = () => {
    const cmd = buildCommentCommand(commentBuilder);
    if (isModifyComment && selectedProcessCommandPath) {
      setCommands(replaceProcessCommandAtPath(commands, selectedProcessCommandPath, cmd));
      return;
    }
    insertAtArrow(cmd);
  };

  const deleteCommandAtPath = (path: string) => {
    const next = deleteProcessCommandAtPath(commands, path);
    setCommands(next);
    if (selectedProcessCommandPath === path) {
      setSelectedProcessCommandPath(null);
    }
  };

  const moveCommandAtPath = (path: string, direction: "up" | "down") => {
    const moved = moveProcessCommandAtPath(commands, path, direction);
    if (!moved) return;
    setCommands(moved.commands);
    setSelectedProcessCommandPath(moved.newPath);
  };

  const canMoveCommand = (path: string, direction: "up" | "down") =>
    canMoveProcessCommandAtPath(commands, path, direction);

  return (
    <div className="form-editor process-editor">
      <ProcessConnectionBanner
        links={formLinks}
        onConnectClick={() => setConnectionDialogOpen(true)}
      />
      {showPaletteHint && (
        <div className="process-palette-hint-strip">
          To create a new statement, click one of the buttons in the Statements palette.
        </div>
      )}
      <div
        className={`process-statement-window${showPaletteHint ? "" : " process-statement-window-top"}`}
      >
        {hasPropertyPanel && (
          <>
            <div className="process-statement-properties">
              {processStatementPanel === "if" && (
                <IfStatementBuilder
                  embedded
                  knownVariables={knownVariables}
                  state={ifBuilder}
                  onStateChange={setIfBuilder}
                  submitLabel={isModifyIf ? "Modify" : "Add"}
                  onSubmit={submitIf}
                />
              )}
              {processStatementPanel === "set" && (
                <SetStatementBuilder
                  embedded
                  state={setBuilder}
                  onStateChange={setSetBuilder}
                  submitLabel={isModifySet ? "Modify" : "Add"}
                  onSubmit={submitSet}
                  knownVariables={knownVariables}
                />
              )}
              {processStatementPanel === "show" && (
                <ShowStatementBuilder
                  embedded
                  state={showBuilder}
                  onStateChange={setShowBuilder}
                  submitLabel={isModifyShow ? "Modify" : "Add"}
                  onSubmit={submitShow}
                  documentNames={documentNames}
                  formNames={formNames}
                  knownVariables={knownVariables}
                />
              )}
              {processStatementPanel === "send" && (
                <SendStatementBuilder
                  embedded
                  state={sendBuilder}
                  onStateChange={setSendBuilder}
                  submitLabel={isModifySend ? "Modify" : "Add"}
                  onSubmit={submitSend}
                  project={project}
                  documentNames={documentNames}
                  knownVariables={knownVariables}
                />
              )}
              {processStatementPanel === "append" && (
                <AppendStatementBuilder
                  embedded
                  state={appendBuilder}
                  onStateChange={setAppendBuilder}
                  submitLabel={isModifyAppend ? "Modify" : "Add"}
                  onSubmit={submitAppend}
                  documentNames={documentNames}
                />
              )}
              {processStatementPanel === "get" && (
                <GetStatementBuilder
                  embedded
                  state={getBuilder}
                  onStateChange={setGetBuilder}
                  submitLabel={isModifyGet ? "Modify" : "Add"}
                  onSubmit={submitGet}
                  formNames={formNames}
                  knownVariables={knownVariables}
                />
              )}
              {processStatementPanel === "foreach" && (
                <ForEachStatementBuilder
                  embedded
                  state={forEachBuilder}
                  onStateChange={setForEachBuilder}
                  submitLabel={isModifyForEach ? "Modify" : "Add"}
                  onSubmit={submitForEach}
                  recordNames={processRecordNames}
                  recordLists={processRecordLists}
                />
              )}
              {processStatementPanel === "delete" && (
                <DeleteStatementBuilder
                  embedded
                  state={deleteBuilder}
                  onStateChange={setDeleteBuilder}
                  submitLabel={isModifyDelete ? "Modify" : "Add"}
                  onSubmit={submitDelete}
                  formNames={formNames}
                  knownVariables={knownVariables}
                />
              )}
              {processStatementPanel === "comment" && (
                <CommentStatementBuilder
                  embedded
                  state={commentBuilder}
                  onStateChange={setCommentBuilder}
                  submitLabel={isModifyComment ? "Modify" : "Add"}
                  onSubmit={submitComment}
                />
              )}
            </div>
            <div className="process-statement-divider" aria-hidden />
          </>
        )}
        <div
          className={`process-script-scroll${dragInsertPath != null ? " process-script-dragging" : ""}`}
          ref={scriptRef}
          onDragOver={(e) => {
            const isPalette = hasProcessStatementDrag(e.dataTransfer);
            const isReorder = hasProcessStatementReorderDrag(e.dataTransfer);
            if (!isPalette && !isReorder) return;
            e.preventDefault();
            e.stopPropagation();
            e.dataTransfer.dropEffect = isReorder ? "move" : "copy";
            const scroll = scriptRef.current;
            if (!scroll) return;
            autoScrollProcessScript(scroll, e.clientY);
            const hit = nearestProcessInsertHit(scroll, e.clientY);
            if (!hit) return;
            setDragInsertPath(hit.path);
            setDragInsertIndex(hit.index);
            setDragCaretTop(hit.caretTop);
          }}
          onDragLeave={(e) => {
            if (!e.currentTarget.contains(e.relatedTarget as Node)) {
              setDragInsertPath(null);
              setDragInsertIndex(null);
              setDragCaretTop(null);
            }
          }}
          onDrop={(e) => {
            const hit =
              dragInsertPath != null && dragInsertIndex != null
                ? { path: dragInsertPath, index: dragInsertIndex }
                : nearestProcessInsertHit(e.currentTarget, e.clientY);
            setDragInsertPath(null);
            setDragInsertIndex(null);
            setDragCaretTop(null);
            if (!hit) return;

            const fromPath = readProcessStatementReorderDrag(e.dataTransfer);
            if (fromPath) {
              e.preventDefault();
              e.stopPropagation();
              moveProcessCommandBefore(fromPath, hit.path, hit.index);
              return;
            }

            const label = readProcessStatementDrag(e.dataTransfer);
            if (!label) return;
            e.preventDefault();
            e.stopPropagation();
            if (PROCESS_PANEL_LABELS.has(label)) {
              setProcessInsertPoint(hit.path, hit.index);
              toggleProcessStatementPanel(label);
              return;
            }
            const def = PROCESS_STATEMENT_PALETTE.find((d) => d.label === label);
            if (def) {
              insertProcessCommand(def.template, { path: hit.path, index: hit.index });
            }
          }}
          onDragEnd={() => {
            setDragInsertPath(null);
            setDragInsertIndex(null);
            setDragCaretTop(null);
          }}
        >
          {dragInsertPath != null && dragCaretTop != null ? (
            <div
              className="form-canvas-insert-caret process-script-insert-caret"
              style={{ top: dragCaretTop }}
              aria-hidden
            >
              <img
                className="form-canvas-insert-caret-marker"
                src="/designer/Insert.png"
                width={16}
                height={13}
                alt=""
              />
            </div>
          ) : null}
          <div className="process-script-body">
            <button
              type="button"
              className="process-script-insert-rail"
              aria-label="Click vertically in this margin to set the insertion point"
              title="Click in this left margin to place ▶ (aim vertically only)"
              onClick={(e) => {
                const scroll = scriptRef.current;
                if (!scroll) return;
                const hit = nearestProcessInsertHit(scroll, e.clientY);
                if (hit) setProcessInsertPoint(hit.path, hit.index);
              }}
            />
            <div className="process-script-lines">
              <div className="skip-script-area process-script-area">
                <SkipScriptView
                  lines={scriptLines}
                  insertPath={processInsertPath}
                  insertIndex={processInsertIndex}
                  onSelectInsertPoint={setProcessInsertPoint}
                  selectedCommandPath={selectedProcessCommandPath}
                  onSelectCommandPath={setSelectedProcessCommandPath}
                  showLineControls
                  showAllInsertionGaps
                  insertHitTargets
                  highlightInsertPath={dragInsertPath}
                  highlightInsertIndex={dragInsertIndex}
                  onMoveCommand={moveCommandAtPath}
                  onDeleteCommand={deleteCommandAtPath}
                  canMoveCommand={canMoveCommand}
                  onReorderDragStart={(path, dt) => setProcessStatementReorderDrag(dt, path)}
                />
              </div>
            </div>
          </div>
        </div>
      </div>
      {connectionDialogOpen && (
        <ProcessConnectionDialog
          processName={processName}
          onClose={() => setConnectionDialogOpen(false)}
        />
      )}
    </div>
  );
}

function nearestProcessInsertHit(
  scrollEl: HTMLElement,
  clientY: number,
): { path: string; index: number; caretTop: number } | null {
  const hits = Array.from(
    scrollEl.querySelectorAll<HTMLElement>("[data-process-insert-path]"),
  );
  if (!hits.length) return null;
  let best = hits[0];
  let bestDist = Infinity;
  for (const el of hits) {
    const rect = el.getBoundingClientRect();
    const mid = rect.top + rect.height / 2;
    const dist = Math.abs(clientY - mid);
    if (dist < bestDist) {
      bestDist = dist;
      best = el;
    }
  }
  const path = best.dataset.processInsertPath;
  const index = Number(best.dataset.processInsertIndex);
  if (!path || !Number.isFinite(index)) return null;
  // Position caret relative to the scroll container's content box.
  const scrollRect = scrollEl.getBoundingClientRect();
  const caretTop = best.getBoundingClientRect().top - scrollRect.top + scrollEl.scrollTop;
  return { path, index, caretTop };
}

function autoScrollProcessScript(scrollEl: HTMLElement, clientY: number): void {
  const rect = scrollEl.getBoundingClientRect();
  const edge = 36;
  const step = 18;
  if (clientY < rect.top + edge) {
    scrollEl.scrollTop = Math.max(0, scrollEl.scrollTop - step);
  } else if (clientY > rect.bottom - edge) {
    scrollEl.scrollTop = Math.min(
      scrollEl.scrollHeight - scrollEl.clientHeight,
      scrollEl.scrollTop + step,
    );
  }
}
