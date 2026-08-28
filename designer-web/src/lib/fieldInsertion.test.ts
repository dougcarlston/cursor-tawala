/**
 * Fields palette qualification + FieldTextInput caret insert (Process / Configure).
 * Document canvas uses chips via fieldTokens; these helpers still gate drag/drop naming.
 */
import { describe, expect, it } from "vitest";
import {
  FIELD_DRAG_FORM_MIME,
  FIELD_DRAG_MIME,
  fieldAcceptedByTarget,
  fieldInsertText,
  fieldToken,
  insertTokenAtCaret,
  isFormFieldReference,
  isRecordQualifiedField,
  paletteLeafInsertName,
  qualifyPaletteFieldName,
  readFieldDragNameForTarget,
  setFieldDragData,
} from "./fieldInsertion";

/** Minimal DataTransfer stand-in for node vitest (no browser DataTransfer). */
function fakeDataTransfer(): DataTransfer {
  const store = new Map<string, string>();
  return {
    types: [] as string[],
    effectAllowed: "uninitialized" as DataTransfer["effectAllowed"],
    dropEffect: "none" as DataTransfer["dropEffect"],
    files: [] as unknown as FileList,
    items: [] as unknown as DataTransferItemList,
    setData(type: string, value: string) {
      store.set(type, value);
      (this as unknown as { types: string[] }).types = Array.from(store.keys());
    },
    getData(type: string) {
      return store.get(type) ?? "";
    },
    clearData() {
      store.clear();
      (this as unknown as { types: string[] }).types = [];
    },
    setDragImage() {},
  } as DataTransfer;
}

describe("qualifyPaletteFieldName", () => {
  it("qualifies bare form-branch leaves as Form:Field", () => {
    expect(qualifyPaletteFieldName("Email", "Form 1")).toBe("Form 1:Email");
    expect(qualifyPaletteFieldName("FIB1:a", "Form 1")).toBe("Form 1:FIB1:a");
  });

  it("leaves Variables (no form) and same-form already-qualified names alone", () => {
    expect(qualifyPaletteFieldName("FullName")).toBe("FullName");
    expect(qualifyPaletteFieldName("FullName", null)).toBe("FullName");
    expect(qualifyPaletteFieldName("Form 1:Email", "Form 1")).toBe("Form 1:Email");
    expect(qualifyPaletteFieldName("Form 1:FIB1:a", "Form 1")).toBe("Form 1:FIB1:a");
  });
});

describe("fieldInsertText / paletteLeafInsertName", () => {
  it("wraps tokens for Document/Set targets and keeps bare for If", () => {
    expect(fieldInsertText("Form 1:Email", {})).toBe("<<Form 1:Email>>");
    expect(fieldInsertText("Form 1:Email", { bare: true })).toBe("Form 1:Email");
    expect(fieldToken("Var")).toBe("<<Var>>");
  });

  it("resolves double-click insert names the same as drag qualification", () => {
    expect(paletteLeafInsertName("Email", "Signup", {})).toBe("Signup:Email");
    expect(paletteLeafInsertName("FIB1:a", "Signup", {})).toBe("Signup:FIB1:a");
    expect(paletteLeafInsertName("Email", "Signup", {}, "Override:X")).toBe("Override:X");
  });
});

describe("setFieldDragData / readFieldDragNameForTarget", () => {
  it("puts qualified text/plain on form-branch drags (native fallback)", () => {
    const dt = fakeDataTransfer();
    setFieldDragData(dt, "Email", "Form 1");
    expect(dt.getData(FIELD_DRAG_MIME)).toBe("Email");
    expect(dt.getData(FIELD_DRAG_FORM_MIME)).toBe("Form 1");
    expect(dt.getData("text/plain")).toBe("<<Form 1:Email>>");
    expect(readFieldDragNameForTarget(dt, {})).toBe("Form 1:Email");
  });

  it("puts Form:FIB1:a on FIB Item:blank form-branch drags", () => {
    const dt = fakeDataTransfer();
    setFieldDragData(dt, "FIB1:a", "Form 2");
    expect(dt.getData("text/plain")).toBe("<<Form 2:FIB1:a>>");
    expect(readFieldDragNameForTarget(dt, {})).toBe("Form 2:FIB1:a");
  });

  it("keeps Variables bare in text/plain", () => {
    const dt = fakeDataTransfer();
    setFieldDragData(dt, "FullName");
    expect(dt.getData("text/plain")).toBe("<<FullName>>");
    expect(readFieldDragNameForTarget(dt, {})).toBe("FullName");
  });
});

describe("fieldAcceptedByTarget", () => {
  it("accepts form fields, variables, and record-qualified names for If targets", () => {
    expect(fieldAcceptedByTarget("Form 1:Email", { formFieldsOnly: true })).toBe(true);
    expect(fieldAcceptedByTarget("FullName", { formFieldsOnly: true })).toBe(false);
    expect(
      fieldAcceptedByTarget("FullName", { knownVariables: new Set(["FullName"]) }),
    ).toBe(true);
    expect(
      fieldAcceptedByTarget("UnknownVar", { knownVariables: new Set(["FullName"]) }),
    ).toBe(true);
    expect(
      fieldAcceptedByTarget("Rec:Form:Field", { knownVariables: new Set(["FullName"]) }),
    ).toBe(true);
  });
});

describe("isFormFieldReference / isRecordQualifiedField", () => {
  it("detects colon-qualified and record-qualified names", () => {
    expect(isFormFieldReference("Form 1:Email")).toBe(true);
    expect(isFormFieldReference("Email")).toBe(false);
    expect(isRecordQualifiedField("Rec:Form:Field")).toBe(true);
    expect(isRecordQualifiedField("Form:Field")).toBe(false);
  });
});

describe("insertTokenAtCaret (FieldTextInput)", () => {
  it("splices a token at the caret and replaces a selection", () => {
    const prevRaf = globalThis.requestAnimationFrame;
    globalThis.requestAnimationFrame = ((cb: FrameRequestCallback) => {
      cb(0);
      return 0;
    }) as typeof requestAnimationFrame;
    try {
      const el = {
        value: "hello world",
        selectionStart: 6,
        selectionEnd: 6,
        focus() {},
        setSelectionRange() {},
      } as unknown as HTMLInputElement;

      let next = "";
      insertTokenAtCaret(el, "<<A>>", (v) => {
        next = v;
      });
      expect(next).toBe("hello <<A>>world");

      el.value = "hello world";
      el.selectionStart = 0;
      el.selectionEnd = 5;
      insertTokenAtCaret(el, "<<B>>", (v) => {
        next = v;
      });
      expect(next).toBe("<<B>> world");
    } finally {
      if (prevRaf) globalThis.requestAnimationFrame = prevRaf;
      else Reflect.deleteProperty(globalThis, "requestAnimationFrame");
    }
  });
});
