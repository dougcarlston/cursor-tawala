import { FormItem } from "@/types/tawala";
import { RichTextEditor } from "./RichTextEditor";

interface Props {
  item: FormItem;
  onChange: (patch: Partial<FormItem>) => void;
}

export function FormItemProperties({ item, onChange }: Props) {
  return (
    <div className="properties-panel">
      <label>
        Label
        <input value={item.label} onChange={(e) => onChange({ label: e.target.value })} />
      </label>

      {(item.type === "heading" || item.type === "text") && (
        <>
          {item.type === "heading" && (
            <label>
              Level
              <select
                value={item.level ?? "main"}
                onChange={(e) =>
                  onChange({ level: e.target.value as "main" | "sub" })
                }
              >
                <option value="main">Main heading</option>
                <option value="sub">Sub heading</option>
              </select>
            </label>
          )}
          {item.type === "text" ? (
            <label>
              Content
              <RichTextEditor
                html={typeof item.content === "string" ? item.content : ""}
                onChange={(html) => onChange({ content: html })}
                placeholder="Enter text…"
              />
            </label>
          ) : (
            <label>
              Content
              <input
                value={item.content ?? ""}
                onChange={(e) => onChange({ content: e.target.value })}
              />
            </label>
          )}
        </>
      )}

      {item.type === "fib" && (
        <>
          <label>
            Prompt
            <textarea
              value={item.prompt ?? ""}
              rows={3}
              onChange={(e) => onChange({ prompt: e.target.value })}
            />
          </label>
          <p className="hint">Blanks: {(item.blanks ?? []).map((b) => b.name).join(", ")}</p>
        </>
      )}

      {item.type === "mc" && (
        <>
          <label>
            Question
            <textarea
              value={item.question ?? ""}
              rows={2}
              onChange={(e) => onChange({ question: e.target.value })}
            />
          </label>
          <label>
            <input
              type="checkbox"
              checked={item.onlyone !== false}
              onChange={(e) => onChange({ onlyone: e.target.checked })}
            />{" "}
            Only one choice
          </label>
          {(item.choices ?? []).map((c, i) => (
            <label key={c.name}>
              Choice {c.name}
              <input
                value={c.text}
                onChange={(e) => {
                  const choices = [...(item.choices ?? [])];
                  choices[i] = { ...c, text: e.target.value };
                  onChange({ choices });
                }}
              />
            </label>
          ))}
        </>
      )}

      {item.type === "field" && (
        <label>
          Field name
          <input
            value={item.fieldName ?? ""}
            onChange={(e) => onChange({ fieldName: e.target.value })}
          />
        </label>
      )}

      {item.type === "skipInstructions" && (
        <label>
          Commands (JSON)
          <textarea
            rows={12}
            className="code-area"
            value={JSON.stringify(item.commands ?? [], null, 2)}
            onChange={(e) => {
              try {
                onChange({ commands: JSON.parse(e.target.value) });
              } catch {
                /* ignore while typing */
              }
            }}
          />
        </label>
      )}
    </div>
  );
}
