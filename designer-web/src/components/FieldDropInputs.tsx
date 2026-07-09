import {
  useState,
  type DragEvent,
  type InputHTMLAttributes,
  type TextareaHTMLAttributes,
} from "react";
import {
  fieldDragAcceptedByTarget,
  fieldAcceptedByTarget,
  fieldInsertText,
  hasFieldDrag,
  insertTokenAtCaret,
  readFieldDragName,
  readFieldDragNameForTarget,
  setActiveFieldTarget,
  type FieldTargetContext,
} from "@/lib/fieldInsertion";

/**
 * Drag/drop handlers that REJECT Fields-panel `<<field>>` drops (owner rule, July 2026).
 * Wired onto **name / identifier** inputs that must never accept a field token:
 * Explorer Form/Process/Document rename, FIB capture-box (on-form) labels, and FIB stored
 * field names. Without these, a native `<input>` would silently insert the drag's `text/plain`
 * `<<name>>` payload. `dragover` shows a "no-drop" cursor; `drop` cancels the default insert.
 */
export function fieldDropRejectHandlers<T extends HTMLElement>() {
  return {
    onDragOver: (e: DragEvent<T>) => {
      if (!hasFieldDrag(e.dataTransfer)) return;
      e.preventDefault();
      e.dataTransfer.dropEffect = "none";
    },
    onDrop: (e: DragEvent<T>) => {
      if (readFieldDragName(e.dataTransfer)) e.preventDefault();
    },
  };
}

/**
 * Plain `<input>` for a name / identifier value that must REJECT field drops (see
 * `fieldDropRejectHandlers`). Mirrors a native input's `onChange`; unlike `FieldTextInput`
 * it never inserts `<<field>>` tokens.
 */
export function NameTextInput({
  className,
  ...rest
}: InputHTMLAttributes<HTMLInputElement>) {
  return (
    <input {...rest} {...fieldDropRejectHandlers<HTMLInputElement>()} className={className} />
  );
}

/**
 * Shared drag-drop + double-click behavior for a native text control that accepts
 * `<<field>>` tokens from the Fields panel (backlog §1 Phase 2). On drop it inserts the
 * token at the caret (replacing any selection) and drives the caller's `onValueChange`;
 * on focus it registers itself as the double-click insertion target.
 */
interface FieldDropOptions extends FieldTargetContext {}

function targetContext(options: FieldDropOptions): FieldTargetContext {
  return {
    bare: options.bare,
    formFieldsOnly: options.formFieldsOnly,
    knownVariables: options.knownVariables,
  };
}

function useFieldDropTarget(
  onValueChange: (next: string) => void,
  options: FieldDropOptions = {},
) {
  const [dragOver, setDragOver] = useState(false);
  const context = targetContext(options);

  const registerActiveTarget = (el: HTMLInputElement | HTMLTextAreaElement) => {
    setActiveFieldTarget((qualifiedName) => {
      if (!fieldAcceptedByTarget(qualifiedName, context)) return;
      const text = fieldInsertText(qualifiedName, context);
      if (options.bare) {
        onValueChange(text);
        requestAnimationFrame(() => {
          try {
            el.focus();
            el.setSelectionRange(text.length, text.length);
          } catch {
            /* element may have unmounted */
          }
        });
        return;
      }
      insertTokenAtCaret(el, text, onValueChange);
    }, context);
  };

  const acceptFieldDrag = (e: React.DragEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    if (!hasFieldDrag(e.dataTransfer)) return false;
    if (!fieldDragAcceptedByTarget(e.dataTransfer, context)) {
      e.preventDefault();
      e.dataTransfer.dropEffect = "none";
      return false;
    }
    e.preventDefault();
    e.dataTransfer.dropEffect = "copy";
    registerActiveTarget(e.currentTarget);
    if (!dragOver) setDragOver(true);
    return true;
  };

  const handlers = {
    onDragEnter: (e: React.DragEvent<HTMLInputElement | HTMLTextAreaElement>) => {
      acceptFieldDrag(e);
    },
    onDragOver: (e: React.DragEvent<HTMLInputElement | HTMLTextAreaElement>) => {
      acceptFieldDrag(e);
    },
    onDragLeave: (e: React.DragEvent<HTMLInputElement | HTMLTextAreaElement>) => {
      const related = e.relatedTarget as Node | null;
      if (related && e.currentTarget.contains(related)) return;
      if (dragOver) setDragOver(false);
    },
    onDrop: (e: React.DragEvent<HTMLInputElement | HTMLTextAreaElement>) => {
      setDragOver(false);
      if (!hasFieldDrag(e.dataTransfer)) return;
      const qualifiedName = readFieldDragNameForTarget(e.dataTransfer, context);
      if (!qualifiedName || !fieldAcceptedByTarget(qualifiedName, context)) {
        e.preventDefault();
        return;
      }
      e.preventDefault();
      const el = e.currentTarget;
      const text = fieldInsertText(qualifiedName, context);
      if (options.bare) {
        onValueChange(text);
        requestAnimationFrame(() => {
          try {
            el.focus();
            el.setSelectionRange(text.length, text.length);
          } catch {
            /* element may have unmounted */
          }
        });
        return;
      }
      insertTokenAtCaret(el, text, onValueChange);
    },
    onFocus: (e: React.FocusEvent<HTMLInputElement | HTMLTextAreaElement>) => {
      registerActiveTarget(e.currentTarget);
    },
  };

  return { dragOver, handlers };
}

function mergeClass(base: string | undefined, dragOver: boolean): string | undefined {
  const cls = `${base ?? ""}${dragOver ? " field-drop-active" : ""}`.trim();
  return cls || undefined;
}

interface FieldTextInputProps
  extends Omit<InputHTMLAttributes<HTMLInputElement>, "onChange"> {
  onValueChange: (next: string) => void;
}

/** `<input>` that accepts `<<field>>` drops and double-click inserts from the Fields panel. */
export function FieldTextInput({ onValueChange, className, ...rest }: FieldTextInputProps) {
  const { dragOver, handlers } = useFieldDropTarget(onValueChange);
  return (
    <input
      {...rest}
      {...handlers}
      className={mergeClass(className, dragOver)}
      onChange={(e) => onValueChange(e.target.value)}
    />
  );
}

/** Process/skip If condition field box — inserts bare `Form:Field`, not `<<…>>`. */
export function QualifiedFieldInput({
  onValueChange,
  className,
  formFieldsOnly,
  knownVariables,
  ...rest
}: FieldTextInputProps & {
  formFieldsOnly?: boolean;
  knownVariables?: ReadonlySet<string>;
}) {
  const { dragOver, handlers } = useFieldDropTarget(onValueChange, {
    bare: true,
    formFieldsOnly,
    knownVariables,
  });
  return (
    <input
      {...rest}
      {...handlers}
      className={mergeClass(className, dragOver)}
      onChange={(e) => onValueChange(e.target.value)}
    />
  );
}

interface FieldTextAreaProps
  extends Omit<TextareaHTMLAttributes<HTMLTextAreaElement>, "onChange"> {
  onValueChange: (next: string) => void;
}

/** `<textarea>` that accepts `<<field>>` drops and double-click inserts from the Fields panel. */
export function FieldTextArea({ onValueChange, className, ...rest }: FieldTextAreaProps) {
  const { dragOver, handlers } = useFieldDropTarget(onValueChange);
  return (
    <textarea
      {...rest}
      {...handlers}
      className={mergeClass(className, dragOver)}
      onChange={(e) => onValueChange(e.target.value)}
    />
  );
}
