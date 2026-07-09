import { useEffect, useLayoutEffect, useMemo, useRef, useState, type ReactNode } from "react";
import { IfStatementBuilder } from "@/components/IfStatementBuilder";
import { ProcessConnectionDialog } from "@/components/ProcessConnectionDialog";
import { SetStatementBuilder } from "@/components/SetStatementBuilder";
import { ShowStatementBuilder } from "@/components/ShowStatementBuilder";
import { SkipScriptView } from "@/components/SkipScriptView";
import { insertCommandAtPoint } from "@/lib/processInsert";
import { formLinksForProcess, collectKnownVariables } from "@/lib/projectModel";
import {
  buildProcessScriptLines,
  canMoveProcessCommandAtPath,
  deleteProcessCommandAtPath,
  getProcessCommandAtPath,
  moveProcessCommandAtPath,
  replaceProcessCommandAtPath,
} from "@/lib/processScript";
import {
  EMPTY_IF_BUILDER,
  EMPTY_SET_BUILDER,
  EMPTY_SHOW_BUILDER,
  buildShowCommand,
  ifBuilderFromCommand,
  rowsAreValid,
  setBuilderFromCommand,
  showBuilderFromCommand,
  type IfBuilderState,
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

  let message: ReactNode;
  if (links.length === 0) {
    message = (
      <>
        Not connected as Pre-Process or Post-Process to any Form. Click {hereLink} to change.
      </>
    );
  } else if (links.length === 1) {
    const { role, formName } = links[0];
    message = (
      <>
        Connected as {role}-Process to Form &apos;{formName}&apos;. Click {hereLink} to change.
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
  const updateProcessCommands = useProjectStore((s) => s.updateProcessCommands);
  const proc = project.processes?.find((p) => p.name === processName);
  const [ifBuilder, setIfBuilder] = useState<IfBuilderState>(EMPTY_IF_BUILDER);
  const [setBuilder, setSetBuilder] = useState<SetBuilderState>(EMPTY_SET_BUILDER);
  const [showBuilder, setShowBuilder] = useState<ShowBuilderState>(EMPTY_SHOW_BUILDER);
  const [connectionDialogOpen, setConnectionDialogOpen] = useState(false);
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
  const hasPropertyPanel =
    isActiveProcess &&
    (processStatementPanel === "if" ||
      processStatementPanel === "set" ||
      processStatementPanel === "show");
  const showPaletteHint =
    isActiveProcess && commands.length === 0 && processStatementPanel === "none";

  useEffect(() => {
    if (!isActiveProcess) return;
    if (
      processStatementPanel === "if" ||
      processStatementPanel === "set" ||
      processStatementPanel === "show"
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
    }
  }, [selectedProcessCommandPath, commands, isActiveProcess, processStatementPanel]);

  useEffect(() => {
    if (!isActiveProcess) return;
    if (processStatementPanel === "if" && !selectedProcessCommandPath) {
      setIfBuilder(EMPTY_IF_BUILDER);
    }
    if (processStatementPanel === "set" && !selectedProcessCommandPath) {
      setSetBuilder(EMPTY_SET_BUILDER);
    }
    if (processStatementPanel === "show" && !selectedProcessCommandPath) {
      setShowBuilder(EMPTY_SHOW_BUILDER);
    }
  }, [processStatementPanel, selectedProcessCommandPath, isActiveProcess]);

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
            </div>
            <div className="process-statement-divider" aria-hidden />
          </>
        )}
        <div className="process-script-scroll" ref={scriptRef}>
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
              onMoveCommand={moveCommandAtPath}
              onDeleteCommand={deleteCommandAtPath}
              canMoveCommand={canMoveCommand}
            />
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
