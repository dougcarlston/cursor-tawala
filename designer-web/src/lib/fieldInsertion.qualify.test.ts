import { describe, expect, it } from "vitest";
import {
  FIELD_DRAG_FORM_MIME,
  FIELD_DRAG_MIME,
  fieldInsertText,
  fieldToken,
  formFieldOwnershipFromLeaves,
  paletteLeafInsertName,
  qualifyPaletteFieldName,
  readFieldDragNameForTarget,
  reconnectBareFieldName,
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
    expect(qualifyPaletteFieldName("Field1", "Form 1")).toBe("Form 1:Field1");
    expect(qualifyPaletteFieldName("MCQ1", "Form 1")).toBe("Form 1:MCQ1");
  });

  it("qualifies FIB Item:blank leaves that already contain a colon", () => {
    // Regression: includes(":") used to skip these → ambiguous <<FIB1:a>> on multi-form projects.
    expect(qualifyPaletteFieldName("FIB1:a", "Form 1")).toBe("Form 1:FIB1:a");
    expect(qualifyPaletteFieldName("FIB1:b", "Form 2")).toBe("Form 2:FIB1:b");
    expect(qualifyPaletteFieldName("FIB1:c", "Form 1")).toBe("Form 1:FIB1:c");
    expect(qualifyPaletteFieldName("FIB2:a", "Form 1")).toBe("Form 1:FIB2:a");
  });

  it("leaves Variables (no form) and already-qualified names unchanged", () => {
    expect(qualifyPaletteFieldName("FullName", null)).toBe("FullName");
    expect(qualifyPaletteFieldName("FullName", undefined)).toBe("FullName");
    expect(qualifyPaletteFieldName("_InviteeID", null)).toBe("_InviteeID");
    expect(qualifyPaletteFieldName("Form 1:Email", "Form 1")).toBe("Form 1:Email");
    expect(qualifyPaletteFieldName("Form 1:FIB1:a", "Form 1")).toBe("Form 1:FIB1:a");
    expect(qualifyPaletteFieldName("Record:Form 1:Email", "Form 1")).toBe(
      "Record:Form 1:Email",
    );
  });

  it("qualifies field leaf when form and field share the same title (Online Exam)", () => {
    // Regression: treating name === formName as already-qualified inserted bare
    // <<Question>> instead of <<Question:Question>> into MCQ/FIB.
    expect(qualifyPaletteFieldName("Question", "Question")).toBe("Question:Question");
    expect(qualifyPaletteFieldName("Exam", "Exam")).toBe("Exam:Exam");
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

  it("Form-branch FIB drag inserts <<Form 1:FIB1:a>> (not bare <<FIB1:a>>)", () => {
    const dt = mockDataTransfer();
    setFieldDragData(dt, "FIB1:a", "Form 1");
    expect(dt.getData(FIELD_DRAG_MIME)).toBe("FIB1:a");
    expect(dt.getData(FIELD_DRAG_FORM_MIME)).toBe("Form 1");
    expect(dt.getData("text/plain")).toBe("<<Form 1:FIB1:a>>");
    expect(readFieldDragNameForTarget(dt, {})).toBe("Form 1:FIB1:a");
    expect(fieldInsertText("Form 1:FIB1:a", {})).toBe("<<Form 1:FIB1:a>>");
  });

  it("same FIB name from Form 2 qualifies to Form 2 (disambiguates multi-form)", () => {
    const dt = mockDataTransfer();
    setFieldDragData(dt, "FIB1:a", "Form 2");
    expect(readFieldDragNameForTarget(dt, {})).toBe("Form 2:FIB1:a");
    expect(dt.getData("text/plain")).toBe("<<Form 2:FIB1:a>>");
  });

  it("Variables-folder drag stays bare <<FullName>> / <<_InviteeID>>", () => {
    const dt = mockDataTransfer();
    setFieldDragData(dt, "FullName");
    expect(dt.getData(FIELD_DRAG_FORM_MIME)).toBe("");
    expect(dt.getData("text/plain")).toBe("<<FullName>>");
    expect(readFieldDragNameForTarget(dt, {})).toBe("FullName");
    expect(fieldToken(readFieldDragNameForTarget(dt, {})!)).toBe("<<FullName>>");

    const invitee = mockDataTransfer();
    setFieldDragData(invitee, "_InviteeID");
    expect(readFieldDragNameForTarget(invitee, {})).toBe("_InviteeID");
    expect(invitee.getData("text/plain")).toBe("<<_InviteeID>>");
  });

  it("paletteLeafInsertName matches drag qualification for double-click", () => {
    expect(paletteLeafInsertName("Email", "Form 1", {})).toBe("Form 1:Email");
    expect(paletteLeafInsertName("FIB1:a", "Form 1", {})).toBe("Form 1:FIB1:a");
    expect(paletteLeafInsertName("FIB1:b", "Form 2", {})).toBe("Form 2:FIB1:b");
    expect(paletteLeafInsertName("FullName", undefined, {})).toBe("FullName");
    expect(paletteLeafInsertName("_InviteeID", undefined, {})).toBe("_InviteeID");
  });

  it("Process If bare target still gets Form:Field without <<>>", () => {
    expect(fieldInsertText("Form 1:Email", { bare: true })).toBe("Form 1:Email");
    expect(fieldInsertText("Form 1:FIB1:a", { bare: true })).toBe("Form 1:FIB1:a");
  });
});

describe("reconnectBareFieldName", () => {
  const ownership = formFieldOwnershipFromLeaves([
    {
      name: "Form 1",
      fields: ["Field1", "MCQ1", "FIB1:a", "FIB1:b", "FIB1:c", "FIB2:a"],
    },
    {
      name: "Form 2",
      fields: ["FIB1:a", "FIB1:b", "FIB1:c"],
    },
  ]);

  it("qualifies unique bare leaves to their sole owning form", () => {
    expect(reconnectBareFieldName("Field1", ownership)).toBe("Form 1:Field1");
    expect(reconnectBareFieldName("MCQ1", ownership)).toBe("Form 1:MCQ1");
    expect(reconnectBareFieldName("FIB2:a", ownership)).toBe("Form 1:FIB2:a");
  });

  it("leaves ambiguous FIB blanks bare when two forms share the leaf", () => {
    expect(reconnectBareFieldName("FIB1:a", ownership)).toBe("FIB1:a");
    expect(reconnectBareFieldName("FIB1:b", ownership)).toBe("FIB1:b");
    expect(reconnectBareFieldName("FIB1:c", ownership)).toBe("FIB1:c");
  });

  it("leaves Variables and already-qualified names unchanged", () => {
    expect(reconnectBareFieldName("_InviteeID", ownership)).toBe("_InviteeID");
    expect(reconnectBareFieldName("FullName", ownership)).toBe("FullName");
    expect(reconnectBareFieldName("Form 1:Field1", ownership)).toBe("Form 1:Field1");
    expect(reconnectBareFieldName("Form 2:FIB1:a", ownership)).toBe("Form 2:FIB1:a");
    expect(reconnectBareFieldName("Record:Form 1:MCQ1", ownership)).toBe(
      "Record:Form 1:MCQ1",
    );
  });

  it("qualifies FIB when only one form owns it", () => {
    const single = formFieldOwnershipFromLeaves([
      { name: "Form 1", fields: ["FIB1:a", "FIB1:b"] },
      { name: "Form 2", fields: ["CoachEmail"] },
    ]);
    expect(reconnectBareFieldName("FIB1:a", single)).toBe("Form 1:FIB1:a");
    expect(reconnectBareFieldName("CoachEmail", single)).toBe("Form 2:CoachEmail");
  });
});
