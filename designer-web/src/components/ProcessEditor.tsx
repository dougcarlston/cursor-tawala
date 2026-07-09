import { useEffect, useMemo, useRef, useState, type ReactNode } from "react";
import { IfStatementBuilder } from "@/components/IfStatementBuilder";
import { ProcessConnectionDialog } from "@/components/ProcessConnectionDialog";
import { SetStatementBuilder } from "@/components/SetStatementBuilder";
import { SkipScriptView } from "@/components/SkipScriptView";
import { getCommandsAtInsertPath } from "@/lib/skipInsertPath";
import { formLinksForProcess, collectKnownVariables } from "@/lib/projectModel";
import {
  buildProcessScriptLines,
  canMoveProcessCommandAtPath,
  deleteProcessCommandAtPath,
  getProcessCommandAtPath,
  insertionPathAfterBlockInsert,
  moveProcessCommandAtPath,
  replaceProcessCommandAtPath,
} from "@/lib/processScript";
import { findInsertionLineIndex } from "@/lib/skipScript";
import {
  EMPTY_IF_BUILDER,
  EMPTY_SET_BUILDER,
  ifBuilderFromCommand,
  setBuilderFromCommand,
  type IfBuilderState,
  type SetBuilderState,
} from "@/lib/statementBuilders";
import { buildConditionFromRows } from "@/lib/skipSummary";
import { setActiveFieldTarget } from "@/lib/fieldInsertion";
import { useProjectStore } from "@/store/projectStore";
import { TawalaProcessCommand } from "@/types/tawala";

interface Props {
  processName: string;
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
  const selectedProcessCommandPath = useProjectStore((s) => s.selectedProcessCommandPath);
  const processStatementPanel = useProjectStore((s) => s.processStatementPanel);
  const setProcessInsertPath = useProjectStore((s) => s.setProcessInsertPath);
  const setSelectedProcessCommandPath = useProjectStore((s) => s.setSelectedProcessCommandPath);
  const moveSelectedProcessCommand = useProjectStore((s) => s.moveSelectedProcessCommand);
  const updateProcessCommands = useProjectStore((s) => s.updateProcessCommands);
  const proc = project.processes?.find((p) => p.name === processName);
  const [ifBuilder, setIfBuilder] = useState<IfBuilderState>(EMPTY_IF_BUILDER);
  const [setBuilder, setSetBuilder] = useState<SetBuilderState>(EMPTY_SET_BUILDER);
  const [connectionDialogOpen, setConnectionDialogOpen] = useState(false);
  const scriptRef = useRef<HTMLDivElement>(null);

  const commands = proc?.commands ?? [];
  const scriptLines = useMemo(() => buildProcessScriptLines(commands), [commands]);
  const knownVariables = useMemo(
    () => collectKnownVariables(project, commands),
    [project, commands],
  );
  const insertAfterIndex = useMemo(
    () => findInsertionLineIndex(scriptLines, processInsertPath),
    [scriptLines, processInsertPath],
  );
  const formLinks = useMemo(
    () => formLinksForProcess(project, processName),
    [project, processName],
  );

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
  const hasPropertyPanel =
    isActiveProcess && (processStatementPanel === "if" || processStatementPanel === "set");
  const showPaletteHint =
    isActiveProcess && commands.length === 0 && processStatementPanel === "none";

  useEffect(() => {
    if (!isActiveProcess) return;
    if (processStatementPanel === "if" || processStatementPanel === "set") return;
    setActiveFieldTarget(null);
  }, [isActiveProcess, processStatementPanel]);

  useEffect(() => {
    if (!isActiveProcess) return;
    return () => setActiveFieldTarget(null);
  }, [isActiveProcess]);

  // Palette owns which panel is open; script selection only loads Modify state when types match.
  useEffect(() => {
    if (!isActiveProcess || !selectedProcessCommandPath) return;
    const cmd = getProcessCommandAtPath(commands, selectedProcessCommandPath);
    if (!cmd) return;
    if (processStatementPanel === "if" && cmd.cmd === "if") {
      setIfBuilder(ifBuilderFromCommand(cmd));
    } else if (processStatementPanel === "set" && cmd.cmd === "set") {
      setSetBuilder(setBuilderFromCommand(cmd));
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
    const condition = buildConditionFromRows(ifBuilder.combinator, ifBuilder.rows);
    if (isModifyIf && selectedProcessCommandPath) {
      const existing = getProcessCommandAtPath(commands, selectedProcessCommandPath);
      const updated: TawalaProcessCommand = {
        cmd: "if",
        condition,
        then: (existing?.then as TawalaProcessCommand[] | undefined) ?? [],
        ...(ifBuilder.hasElse
          ? { else: (existing?.else as TawalaProcessCommand[] | undefined) ?? [] }
          : {}),
      };
      setCommands(replaceProcessCommandAtPath(commands, selectedProcessCommandPath, updated));
      return;
    }
    const ifCmd: TawalaProcessCommand = {
      cmd: "if",
      condition,
      then: [],
      ...(ifBuilder.hasElse ? { else: [] } : {}),
    };
    const nextCommands = structuredClone(commands);
    const target = getCommandsAtInsertPath(nextCommands, processInsertPath);
    const insertedIndex = target.length;
    target.push(ifCmd);
    setCommands(nextCommands);
    setProcessInsertPath(
      insertionPathAfterBlockInsert(processInsertPath, insertedIndex, ifCmd),
    );
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
    const nextCommands = structuredClone(commands);
    const target = getCommandsAtInsertPath(nextCommands, processInsertPath);
    target.push(cmd);
    setCommands(nextCommands);
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
            </div>
            <div className="process-statement-divider" aria-hidden />
          </>
        )}
        <div className="process-script-scroll" ref={scriptRef}>
          <div className="skip-script-area process-script-area">
            <SkipScriptView
              lines={scriptLines}
              insertPath={processInsertPath}
              insertAfterIndex={commands.length === 0 ? -1 : insertAfterIndex}
              onSelectInsertPath={setProcessInsertPath}
              selectedCommandPath={selectedProcessCommandPath}
              onSelectCommandPath={setSelectedProcessCommandPath}
              showLineControls
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
