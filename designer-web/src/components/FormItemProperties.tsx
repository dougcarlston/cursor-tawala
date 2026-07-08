import { FibItem, FormItem, TawalaBlank } from "@/types/tawala";
import { FieldTextArea, FieldTextInput, NameTextInput } from "./FieldDropInputs";
import {
  hasStructuredTextContent,
  StructuredTextProperties,
} from "./StructuredTextProperties";

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
  // Text with plain-string content is now canvas-inline WYSIWYG (like Heading); only
  // structured/function-table Text still edits in the panel.
  const textIsCanvasInline = item.type === "text" && !hasStructuredTextContent(item.content);
  const fibIsCanvasInline = item.type === "fib";
  const mcIsCanvasInline = item.type === "mc";
  const fieldIsCanvasInline = item.type === "field";
  const breakIsCanvasInline = item.type === "break";
  const skipIsCanvasInline = item.type === "skipInstructions";
  const labelIsCanvasInline =
    item.type === "heading" ||
    textIsCanvasInline ||
    fibIsCanvasInline ||
    mcIsCanvasInline ||
    fieldIsCanvasInline ||
    breakIsCanvasInline ||
    skipIsCanvasInline;

  return (
    <div className="properties-panel properties-panel-compact">
      {/* Heading and plain-text items are fully canvas-inline: label is edited in the badge,
          body directly on the canvas. Neither lives here — the canvas row is the source of
          truth (owner, July 2026; DESIGNER_FORM_ITEMS_HEADING.md / _TEXT_FIB_MCQ.md). */}
      {!labelIsCanvasInline && (
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

      {textIsCanvasInline && (
        <p className="hint">
          Edit the text label in its badge and the body directly on the canvas. Formatting uses
          the Formatting Palette above the canvas.
        </p>
      )}

      {fibIsCanvasInline && (
        <p className="hint">
          Edit the question label in its badge, the prompt and blanks on the canvas, and per-blank
          options in the property strip below the prompt. Bold/Italic/Underline use the Formatting
          Palette; place the cursor in an underscore blank to edit Alternate Label, Height,
          Required, and Validation.
        </p>
      )}

      {item.type === "text" && hasStructuredTextContent(item.content) && (
        <StructuredTextProperties
          content={item.content}
          onChange={(content) => onChange({ content })}
        />
      )}

      {item.type === "fib" && !fibIsCanvasInline && (
        <FibItemProperties item={item} onChange={onChange} />
      )}

      {mcIsCanvasInline && (
        <p className="hint">
          Edit the question label in its badge, the question and choices on the canvas (press Enter
          after a choice to add the next), and options in the property strip below (multi-select,
          required, choice source). Bold/Italic/Underline use the Formatting Palette.
        </p>
      )}

      {fieldIsCanvasInline && (
        <p className="hint">
          Edit the hidden field name on the canvas (<strong>Name:</strong>). The FIELD badge is
          fixed. Values are set at runtime via process <strong>Set</strong> commands; the field is
          not shown to respondents.
        </p>
      )}

      {breakIsCanvasInline && (
        <p className="hint">
          Page breaks have no editable properties. The BREAK badge and hatched bar mark where a new
          page begins at runtime.
        </p>
      )}

      {skipIsCanvasInline && (
        <p className="hint">
          Edit skip logic on the canvas: click the blue <strong>Edit</strong> link to open the
          Skip Instructions dialog. Use <strong>If</strong> and <strong>SkipTo</strong> statements
          to control where respondents go when leaving the preceding item.
        </p>
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
