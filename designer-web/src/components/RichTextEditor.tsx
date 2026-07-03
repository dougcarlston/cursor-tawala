import { useEffect, useRef } from "react";

interface Props {
  html: string;
  onChange: (html: string) => void;
  placeholder?: string;
}

/** Simplified rich-text surface (replaces legacy IE WYSIWYG for text items). */
export function RichTextEditor({ html, onChange, placeholder }: Props) {
  const surfaceRef = useRef<HTMLDivElement>(null);
  const lastHtml = useRef(html);

  useEffect(() => {
    const el = surfaceRef.current;
    if (!el) return;
    if (document.activeElement === el) return;
    const next = html || "";
    if (next !== lastHtml.current) {
      el.innerHTML = next;
      lastHtml.current = next;
    }
  }, [html]);

  useEffect(() => {
    const el = surfaceRef.current;
    if (el && !el.innerHTML && html) {
      el.innerHTML = html;
      lastHtml.current = html;
    }
  }, []);

  const exec = (cmd: string, value?: string) => {
    const el = surfaceRef.current;
    if (!el) return;
    el.focus();
    document.execCommand(cmd, false, value);
    const next = el.innerHTML;
    lastHtml.current = next;
    onChange(next);
  };

  return (
    <div className="rich-editor">
      <div className="rich-toolbar">
        <button
          type="button"
          title="Bold"
          onMouseDown={(e) => e.preventDefault()}
          onClick={() => exec("bold")}
        >
          B
        </button>
        <button
          type="button"
          title="Italic"
          onMouseDown={(e) => e.preventDefault()}
          onClick={() => exec("italic")}
        >
          I
        </button>
        <button
          type="button"
          title="Underline"
          onMouseDown={(e) => e.preventDefault()}
          onClick={() => exec("underline")}
        >
          U
        </button>
        <select
          defaultValue="3"
          onChange={(e) => exec("fontSize", e.target.value)}
          title="Font size"
        >
          <option value="2">Small</option>
          <option value="3">Normal</option>
          <option value="4">Large</option>
          <option value="5">X-Large</option>
        </select>
      </div>
      <div
        ref={surfaceRef}
        className="rich-surface"
        contentEditable
        suppressContentEditableWarning
        data-placeholder={placeholder}
        onInput={(e) => {
          const next = (e.target as HTMLDivElement).innerHTML;
          lastHtml.current = next;
          onChange(next);
        }}
      />
    </div>
  );
}
