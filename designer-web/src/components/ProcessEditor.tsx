import { useState } from "react";
import { useProjectStore } from "@/store/projectStore";
import { TawalaProcessCommand } from "@/types/tawala";

interface Props {
  processName: string;
}

const COMMAND_TEMPLATES: Record<string, TawalaProcessCommand> = {
  comment: { cmd: "comment", text: "-- note" },
  show: { cmd: "show", form: "Report" },
  showDocument: { cmd: "show", document: "Document 1" },
  set: { cmd: "set", field: "FieldName", value: "value" },
  get: { cmd: "get", recordList: "Records", sourceForms: ["Form1"] },
  if: {
    cmd: "if",
    condition: { field: "Form:Field", op: "equals", value: "Yes" },
    then: [{ cmd: "set", field: "Result", value: "yes" }],
  },
  foreach: {
    cmd: "foreach",
    recordName: "Rec",
    recordList: "Records",
    do: [{ cmd: "set", field: "X", value: "<<Rec:Form:Field>>" }],
  },
};

export function ProcessEditor({ processName }: Props) {
  const project = useProjectStore((s) => s.project);
  const updateProcessCommands = useProjectStore((s) => s.updateProcessCommands);
  const proc = project.processes?.find((p) => p.name === processName);
  const [selected, setSelected] = useState(0);
  const [jsonError, setJsonError] = useState<string | null>(null);

  if (!proc) {
    return <div className="placeholder-editor">Process not found</div>;
  }

  const commands = proc.commands ?? [];

  const setCommands = (next: TawalaProcessCommand[]) => {
    updateProcessCommands(processName, next);
    if (selected >= next.length) setSelected(Math.max(0, next.length - 1));
  };

  const addCommand = (type: string) => {
    const template = COMMAND_TEMPLATES[type];
    if (!template) return;
    setCommands([...commands, structuredClone(template)]);
    setSelected(commands.length);
  };

  const updateSelected = (raw: string) => {
    try {
      const parsed = JSON.parse(raw) as TawalaProcessCommand;
      const next = [...commands];
      next[selected] = parsed;
      setCommands(next);
      setJsonError(null);
    } catch (e) {
      setJsonError(e instanceof Error ? e.message : "Invalid JSON");
    }
  };

  return (
    <div className="form-editor process-editor">
      <div className="panel-title" style={{ borderBottom: "1px solid #aca899" }}>
        Process: {proc.name}
      </div>
      <div className="process-layout">
        <div className="process-list">
          <div className="process-toolbar">
            {Object.keys(COMMAND_TEMPLATES).map((t) => (
              // Contained hover tip (`.win-tip`) instead of a native `title`: native tooltips
              // render at OS level and spill outside the MDI window, then linger a couple of
              // seconds (owner Bug 2, July 2026). This one is clipped to the window.
              <span key={t} className="win-tip" data-tip={`Add ${t}`}>
                <button type="button" onClick={() => addCommand(t)}>
                  +{t}
                </button>
              </span>
            ))}
          </div>
          <ul>
            {commands.map((c, i) => (
              <li key={i}>
                <button
                  type="button"
                  className={selected === i ? "selected" : ""}
                  onClick={() => setSelected(i)}
                >
                  {i + 1}. {c.cmd}
                </button>
                <span className="win-tip win-tip-right" data-tip="Remove">
                  <button
                    type="button"
                    className="item-delete"
                    onClick={() => setCommands(commands.filter((_, j) => j !== i))}
                  >
                    ×
                  </button>
                </span>
              </li>
            ))}
          </ul>
          {commands.length === 0 && (
            <p className="hint" style={{ padding: 8 }}>
              Add commands — mirrors legacy ProcessView
            </p>
          )}
        </div>
        <div className="process-detail">
          {commands[selected] ? (
            <>
              <textarea
                className="code-area"
                rows={24}
                defaultValue={JSON.stringify(commands[selected], null, 2)}
                key={selected}
                onBlur={(e) => updateSelected(e.target.value)}
              />
              {jsonError && <p className="error-text">{jsonError}</p>}
            </>
          ) : (
            <p className="hint">Select or add a command</p>
          )}
        </div>
      </div>
    </div>
  );
}
