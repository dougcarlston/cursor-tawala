interface Props {
  html: string;
  onChange: (html: string) => void;
  placeholder?: string;
}

/** Simplified rich-text surface (replaces legacy IE WYSIWYG for text items). */
export function RichTextEditor({ html, onChange, placeholder }: Props) {
  const exec = (cmd: string, value?: string) => {
    document.execCommand(cmd, false, value);
    const el = document.activeElement as HTMLElement | null;
    if (el?.innerHTML !== undefined) onChange(el.innerHTML);
  };

  return (
    <div className="rich-editor">
      <div className="rich-toolbar">
        <button type="button" title="Bold" onMouseDown={(e) => e.preventDefault()} onClick={() => exec("bold")}>
          B
        </button>
        <button type="button" title="Italic" onMouseDown={(e) => e.preventDefault()} onClick={() => exec("italic")}>
          I
        </button>
        <button type="button" title="Underline" onMouseDown={(e) => e.preventDefault()} onClick={() => exec("underline")}>
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
        className="rich-surface"
        contentEditable
        suppressContentEditableWarning
        data-placeholder={placeholder}
        dangerouslySetInnerHTML={{ __html: html || "" }}
        onInput={(e) => onChange((e.target as HTMLDivElement).innerHTML)}
      />
    </div>
  );
}
