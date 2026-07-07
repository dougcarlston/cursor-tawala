import { FibItem, FormItem, TawalaBlank, TawalaChoice } from "@/types/tawala";
import { RichTextEditor } from "./RichTextEditor";
import { FieldTextArea, FieldTextInput, NameTextInput } from "./FieldDropInputs";
import {
  hasStructuredTextContent,
  StructuredTextProperties,
} from "./StructuredTextProperties";

function nextChoiceName(choices: TawalaChoice[]): string {
  for (let i = 0; i < 26; i++) {
    const name = String.fromCharCode(97 + i);
    if (!choices.some((c) => c.name === name)) return name;
  }
  return `c${choices.length + 1}`;
}

function nextBlankName(blanks: TawalaBlank[]): string {
  for (let i = 0; i < 26; i++) {
    const name = String.fromCharCode(97 + i);
    if (!blanks.some((b) => b.name === name)) return name;
  }
  return `f${blanks.length + 1}`;
}

interface Props {
  item: FormItem;
  onChange: (patch: Partial<FormItem>) => void;
}

export function FormItemProperties({ item, onChange }: Props) {
  return (
    <div className="properties-panel properties-panel-compact">
      {/* Heading is fully canvas-inline: its label is edited in the badge, its text and
          per-run Main/Sub sizing in the inline editor. None of it lives here — the canvas
          row is the source of truth (owner, July 2026; DESIGNER_FORM_ITEMS_HEADING.md). */}
      {item.type !== "heading" && (
        <label>
          Item label
          <FieldTextInput value={item.label} onValueChange={(v) => onChange({ label: v })} />
        </label>
      )}

      {item.type === "heading" && (
        <p className="hint">
          Edit the heading label in its badge, and the text / Main-Sub sizing directly on the
          canvas. Select text, then choose Main or Sub to size just that part.
        </p>
      )}

      {item.type === "text" &&
        (hasStructuredTextContent(item.content) ? (
          <StructuredTextProperties
            content={item.content}
            onChange={(content) => onChange({ content })}
          />
        ) : (
          <label>
            Content
            <RichTextEditor
              html={typeof item.content === "string" ? item.content : ""}
              onChange={(html) => onChange({ content: html })}
              placeholder="Enter text…"
            />
          </label>
        ))}

      {item.type === "fib" && (
        <FibItemProperties item={item} onChange={onChange} />
      )}

      {item.type === "mc" && (
        <>
          <label>
            Question
            <FieldTextArea
              value={item.question ?? ""}
              rows={2}
              onValueChange={(v) => onChange({ question: v })}
            />
          </label>
          <label className="property-checkbox">
            <input
              type="checkbox"
              checked={item.onlyone !== false}
              onChange={(e) => onChange({ onlyone: e.target.checked })}
            />
            Only one choice
          </label>
          {(item.choices ?? []).map((c, i) => (
            <div key={c.name} className="mc-choice-row">
              <label>
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
              <button
                type="button"
                className="mc-choice-remove"
                title="Remove choice"
                disabled={(item.choices ?? []).length <= 1}
                onClick={() => {
                  const choices = (item.choices ?? []).filter((_, j) => j !== i);
                  onChange({ choices });
                }}
              >
                −
              </button>
            </div>
          ))}
          <button
            type="button"
            className="mc-choice-add"
            onClick={() => {
              const choices = [...(item.choices ?? [])];
              const name = nextChoiceName(choices);
              choices.push({ name, text: `Choice ${name}` });
              onChange({ choices });
            }}
          >
            + Add choice
          </button>
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

function FibItemProperties({
  item,
  onChange,
}: {
  item: FibItem;
  onChange: (patch: Partial<FibItem>) => void;
}) {
  const blanks = item.blanks ?? [];

  return (
    <>
      <label>
        Question
        <FieldTextArea
          value={item.prompt ?? ""}
          rows={2}
          onValueChange={(v) => onChange({ prompt: v })}
          placeholder="e.g. Enter your full name here:"
        />
      </label>
      <p className="hint">
        Shown above this item&apos;s response fields. Per-field labels below are separate from
        stored field names used in processes and tables.
      </p>
      <div className="fib-fields-toolbar">
        <span>Response fields ({blanks.length})</span>
        <button
          type="button"
          className="fib-field-add"
          onClick={() => {
            const name = nextBlankName(blanks);
            onChange({
              blanks: [
                ...blanks,
                {
                  name,
                  alternateLabel: name,
                  displayLabel: "",
                  length: 20,
                },
              ],
            });
          }}
        >
          + Add field
        </button>
      </div>
      {blanks.map((b, i) => (
        <details key={`${b.name}-${i}`} className="fib-field-details" open={i === 0}>
          <summary>{b.displayLabel?.trim() || `Field ${i + 1}`}</summary>
          <div className="fib-field-grid">
            <label>
              Label on form
              {/* FIB capture-box label: field drops NOT allowed (owner rule, July 2026).
                  Fine-grained FIB drop zones are deferred to the WYSIWYG canvas redesign. */}
              <NameTextInput
                value={b.displayLabel ?? ""}
                placeholder="Shown to respondent"
                onChange={(e) => {
                  const next = [...blanks];
                  next[i] = { ...b, displayLabel: e.target.value };
                  onChange({ blanks: next });
                }}
              />
            </label>
            <label>
              Stored name
              {/* Field identifier used in processes/tables: field drops NOT allowed. */}
              <NameTextInput
                value={b.alternateLabel ?? b.name}
                onChange={(e) => {
                  const next = [...blanks];
                  next[i] = { ...b, alternateLabel: e.target.value };
                  onChange({ blanks: next });
                }}
              />
            </label>
            <label>
              Width
              <input
                type="number"
                min={5}
                max={120}
                value={b.length ?? 20}
                onChange={(e) => {
                  const next = [...blanks];
                  next[i] = { ...b, length: Number(e.target.value) || 20 };
                  onChange({ blanks: next });
                }}
              />
            </label>
            <button
              type="button"
              className="fib-field-remove"
              disabled={blanks.length <= 1}
              title="Remove this response field"
              onClick={() => onChange({ blanks: blanks.filter((_, j) => j !== i) })}
            >
              Remove field
            </button>
          </div>
        </details>
      ))}
    </>
  );
}
