import { describe, expect, it } from "vitest";
import {
  FIELD_DRAG_FORM_MIME,
  FIELD_DRAG_MIME,
  fieldInsertText,
  fieldToken,
  paletteLeafInsertName,
  qualifyPaletteFieldName,
  readFieldDragNameForTarget,
  setFieldDragData,
} from "./fieldInsertion";

/** Minimal DataTransfer stand-in for unit tests (jsdom DataTransfer is incomplete). */
function mockDataTransfer(): DataTransfer {
  const store = new Map<string, string>();
  return {
    getData: (type: string) => store.get(type) ?? "",
    setData: (type: string, value: string) => {
      store.set(type, value);
    },
    get types() {
      return Array.from(store.keys());
    },
    effectAllowed: "uninitialized" as DataTransfer["effectAllowed"],
    dropEffect: "none" as DataTransfer["dropEffect"],
    files: [] as unknown as FileList,
    items: [] as unknown as DataTransferItemList,
    clearData: () => store.clear(),
    setDragImage: () => {},
  } as DataTransfer;
}

describe("qualifyPaletteFieldName", () => {
  it("qualifies bare Form-branch leaves as Form:Field", () => {
    expect(qualifyPaletteFieldName("Email", "Form 1")).toBe("Form 1:Email");
    expect(qualifyPaletteFieldName("Tel", "Form 1")).toBe("Form 1:Tel");
  });

  it("leaves Variables (no form) and already-qualified names unchanged", () => {
    expect(qualifyPaletteFieldName("FullName", null)).toBe("FullName");
    expect(qualifyPaletteFieldName("FullName", undefined)).toBe("FullName");
    expect(qualifyPaletteFieldName("Form 1:Email", "Form 1")).toBe("Form 1:Email");
    expect(qualifyPaletteFieldName("Record:Form 1:Email", "Form 1")).toBe(
      "Record:Form 1:Email",
    );
  });
});

describe("setFieldDragData / readFieldDragNameForTarget", () => {
  it("Form-branch drag inserts <<Form 1:Email>> (not bare <<Email>>)", () => {
    const dt = mockDataTransfer();
    setFieldDragData(dt, "Email", "Form 1");
    expect(dt.getData(FIELD_DRAG_MIME)).toBe("Email");
    expect(dt.getData(FIELD_DRAG_FORM_MIME)).toBe("Form 1");
    expect(dt.getData("text/plain")).toBe("<<Form 1:Email>>");
    expect(readFieldDragNameForTarget(dt, {})).toBe("Form 1:Email");
    expect(fieldInsertText("Form 1:Email", {})).toBe("<<Form 1:Email>>");
  });

  it("Variables-folder drag stays bare <<FullName>>", () => {
    const dt = mockDataTransfer();
    setFieldDragData(dt, "FullName");
    expect(dt.getData(FIELD_DRAG_FORM_MIME)).toBe("");
    expect(dt.getData("text/plain")).toBe("<<FullName>>");
    expect(readFieldDragNameForTarget(dt, {})).toBe("FullName");
    expect(fieldToken(readFieldDragNameForTarget(dt, {})!)).toBe("<<FullName>>");
  });

  it("paletteLeafInsertName matches drag qualification for double-click", () => {
    expect(paletteLeafInsertName("Email", "Form 1", {})).toBe("Form 1:Email");
    expect(paletteLeafInsertName("FullName", undefined, {})).toBe("FullName");
  });

  it("Process If bare target still gets Form:Field without <<>>", () => {
    expect(fieldInsertText("Form 1:Email", { bare: true })).toBe("Form 1:Email");
  });
});
