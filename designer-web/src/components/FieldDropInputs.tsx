import {
  useState,
  type DragEvent,
  type InputHTMLAttributes,
  type TextareaHTMLAttributes,
} from "react";
import {
  fieldToken,
  hasFieldDrag,
  insertTokenAtCaret,
  isFormFieldReference,
  isValidIfConditionField,
  readFieldDragName,
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

function useFieldDropTarget(
  onValueChange: (next: string) => void,
  options: FieldDropOptions = {},
) {
  const [dragOver, setDragOver] = useState(false);
  const insertValue = (name: string) => (options.bare ? name : fieldToken(name));
  const acceptsField = (name: string) => {
    if (options.formFieldsOnly && !isFormFieldReference(name)) return false;
    if (
      options.knownVariables &&
      !isValidIfConditionField(name, options.knownVariables)
    ) {
      return false;
    }
    return true;
  };

  const handlers = {
    onDragOver: (e: React.DragEvent<HTMLInputElement | HTMLTextAreaElement>) => {
      if (!hasFieldDrag(e.dataTransfer)) return;
      const name = readFieldDragName(e.dataTransfer);
      if (name && !acceptsField(name)) {
        e.preventDefault();
        e.dataTransfer.dropEffect = "none";
        return;
      }
      e.preventDefault();
      e.dataTransfer.dropEffect = "copy";
      if (!dragOver) setDragOver(true);
    },
    onDragLeave: () => {
      if (dragOver) setDragOver(false);
    },
    onDrop: (e: React.DragEvent<HTMLInputElement | HTMLTextAreaElement>) => {
      setDragOver(false);
      const name = readFieldDragName(e.dataTransfer);
      if (!name || !acceptsField(name)) return;
      e.preventDefault();
      insertTokenAtCaret(e.currentTarget, insertValue(name), onValueChange);
    },
    onFocus: (e: React.FocusEvent<HTMLInputElement | HTMLTextAreaElement>) => {
      const el = e.currentTarget;
      const context: FieldTargetContext = {
        bare: options.bare,
        formFieldsOnly: options.formFieldsOnly,
        knownVariables: options.knownVariables,
      };
      setActiveFieldTarget((name) => {
        if (!acceptsField(name)) return;
        insertTokenAtCaret(el, insertValue(name), onValueChange);
      }, context);
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
