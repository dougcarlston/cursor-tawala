import { useEffect, useRef, useState } from "react";
import {
  cancelSaveAsDialog,
  confirmSaveAs,
  suggestedProjectFileName,
} from "@/lib/shellCommands";

interface Props {
  open: boolean;
}

/**
 * In-app Save As name prompt (legacy File → Save As…).
 * Chromium then gets the native folder picker with `suggestedName`; Safari downloads
 * to Downloads under the chosen name (no OS folder picker).
 */
export function SaveAsDialog({ open }: Props) {
  const [name, setName] = useState(() => suggestedProjectFileName());
  const inputRef = useRef<HTMLInputElement>(null);

  useEffect(() => {
    if (!open) return;
    setName(suggestedProjectFileName());
    // Defer so the input is mounted before focus/select.
    const id = window.requestAnimationFrame(() => {
      const el = inputRef.current;
      if (!el) return;
      el.focus();
      el.select();
    });
    const onKey = (e: KeyboardEvent) => {
      if (e.key === "Escape") {
        e.preventDefault();
        e.stopPropagation();
        cancelSaveAsDialog();
      }
    };
    window.addEventListener("keydown", onKey, true);
    return () => {
      window.cancelAnimationFrame(id);
      window.removeEventListener("keydown", onKey, true);
    };
  }, [open]);

  if (!open) return null;

  const submit = (e: React.FormEvent) => {
    e.preventDefault();
    void confirmSaveAs(name);
  };

  return (
    <div className="modal-backdrop" role="presentation">
      <div className="modal save-as-dialog" role="dialog" aria-modal="true" aria-labelledby="save-as-title">
        <h2 id="save-as-title">Save As</h2>
        <p className="hint">
          Choose a file name. On Chrome, you can then pick a folder. Safari saves to Downloads (no
          folder picker).
        </p>
        <form onSubmit={submit}>
          <label>
            File name
            <input
              ref={inputRef}
              value={name}
              onChange={(e) => setName(e.target.value)}
              spellCheck={false}
              autoComplete="off"
              aria-describedby="save-as-ext-hint"
            />
          </label>
          <p id="save-as-ext-hint" className="hint save-as-ext-hint">
            A <code>.json</code> extension is added if missing.
          </p>
          <div className="modal-actions">
            <button type="button" onClick={() => cancelSaveAsDialog()}>
              Cancel
            </button>
            <button type="submit">Save As</button>
          </div>
        </form>
      </div>
    </div>
  );
}
