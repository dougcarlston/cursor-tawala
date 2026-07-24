import { describe, expect, it } from "vitest";
import {
  headingLegacyType,
  headingPlainText,
  headingSegments,
  headingToPreviewHtml,
  headingToXml,
} from "./headingExport.mjs";

const esc = (s) =>
  String(s ?? "")
    .replace(/&/g, "&amp;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;")
    .replace(/"/g, "&quot;");

describe("headingPlainText", () => {
  it("strips size spans and decodes entities", () => {
    expect(headingPlainText('<span class="heading-size-sub">Age &amp; Size</span>')).toBe(
      "Age & Size",
    );
  });

  it("turns br / block ends into newlines (not glued text)", () => {
    expect(
      headingPlainText(
        '<span class="heading-size-main">Welcome to our hotel.</span><br><span class="heading-size-sub">We\'re happy to have you here.</span>',
      ),
    ).toBe("Welcome to our hotel.\nWe're happy to have you here.");
  });
});

describe("headingLegacyType", () => {
  it("uses subheading type and legacy level", () => {
    expect(headingLegacyType({ type: "subheading", content: "Hi" })).toBe("Sub");
    expect(headingLegacyType({ type: "heading", level: "sub", content: "Hi" })).toBe("Sub");
    expect(headingLegacyType({ type: "heading", level: "main", content: "Hi" })).toBe("Main");
  });

  it("infers Sub when the whole box is heading-size-sub", () => {
    expect(
      headingLegacyType({
        type: "heading",
        content: '<span class="heading-size-sub">Details</span>',
      }),
    ).toBe("Sub");
  });

  it("infers Main for bare text, main spans, or empty", () => {
    expect(headingLegacyType({ type: "heading", content: "Title" })).toBe("Main");
    expect(
      headingLegacyType({
        type: "heading",
        content: '<span class="heading-size-main">Title</span>',
      }),
    ).toBe("Main");
    expect(headingLegacyType({ type: "heading", content: "" })).toBe("Main");
  });

  it("falls back to Main when Main and Sub runs are mixed", () => {
    expect(
      headingLegacyType({
        type: "heading",
        content: 'Main <span class="heading-size-sub">and Sub</span>',
      }),
    ).toBe("Main");
    expect(
      headingLegacyType({
        type: "heading",
        content:
          '<span class="heading-size-main">A</span><span class="heading-size-sub">B</span>',
      }),
    ).toBe("Main");
  });

  it("level=main does not override size-span inference for legacyType callers", () => {
    // headingLegacyType still uses level first; segments are what Preview/Deploy use.
    expect(
      headingLegacyType({
        type: "heading",
        level: "main",
        content: '<span class="heading-size-sub">Only sub</span>',
      }),
    ).toBe("Main");
  });
});

describe("headingSegments", () => {
  it("splits Main then Sub lines into two segments", () => {
    expect(
      headingSegments({
        type: "heading",
        label: "H1",
        content:
          '<span class="heading-size-main">Welcome to our hotel.</span><br><span class="heading-size-sub">We\'re happy to have you here.</span>',
      }),
    ).toEqual([
      { type: "Main", text: "Welcome to our hotel.", blankLinesBefore: 0 },
      { type: "Sub", text: "We're happy to have you here.", blankLinesBefore: 0 },
    ]);
  });

  it("splits adjacent Main/Sub spans with no br (Design wrap glue case)", () => {
    expect(
      headingSegments({
        type: "heading",
        label: "H1",
        content:
          '<span class="heading-size-main">Welcome to our hotel</span><span class="heading-size-sub">We hope you\'ll have a great stay!</span>',
      }),
    ).toEqual([
      { type: "Main", text: "Welcome to our hotel", blankLinesBefore: 0 },
      { type: "Sub", text: "We hope you'll have a great stay!", blankLinesBefore: 0 },
    ]);
  });

  it("records a blank line when Design has br,br between Main and Sub", () => {
    expect(
      headingSegments({
        type: "heading",
        label: "H1",
        content:
          'Welcome to our hotel<br><br><span class="heading-size-sub">We hope you\'ll have a great stay!</span><br>',
      }),
    ).toEqual([
      { type: "Main", text: "Welcome to our hotel", blankLinesBefore: 0 },
      { type: "Sub", text: "We hope you'll have a great stay!", blankLinesBefore: 1 },
    ]);
  });

  it("merges consecutive same-size lines with newline", () => {
    expect(
      headingSegments({
        type: "heading",
        content: '<span class="heading-size-main">A</span><br><span class="heading-size-main">B</span>',
      }),
    ).toEqual([{ type: "Main", text: "A\nB", blankLinesBefore: 0 }]);
  });

  it("splits adjacent Main/Sub even when leftover level=main is set", () => {
    expect(
      headingSegments({
        type: "heading",
        label: "H1",
        level: "main",
        content:
          '<span class="heading-size-main">Welcome to our hotel</span><span class="heading-size-sub">We hope you\'ll have a great stay!</span>',
      }),
    ).toEqual([
      { type: "Main", text: "Welcome to our hotel", blankLinesBefore: 0 },
      { type: "Sub", text: "We hope you'll have a great stay!", blankLinesBefore: 0 },
    ]);
  });

  it("respects whole-box level=sub only when there is no size markup", () => {
    expect(
      headingSegments({
        type: "heading",
        level: "sub",
        content: "One<br>Two",
      }),
    ).toEqual([{ type: "Sub", text: "One\nTwo" }]);
  });
});

describe("headingToXml / headingToPreviewHtml", () => {
  it("emits two heading elements for Main then Sub", () => {
    const item = {
      type: "heading",
      label: "H1",
      content:
        '<span class="heading-size-main">Welcome to our hotel.</span><br><span class="heading-size-sub">We\'re happy to have you here.</span>',
    };
    expect(headingToXml(item, esc, esc)).toBe(
      `<heading label="H1" type="Main">Welcome to our hotel.</heading>` +
        `<heading label="H1.2" type="Sub">We're happy to have you here.</heading>`,
    );
    expect(headingToPreviewHtml(item, esc)).toBe(
      `<h1 class="heading">Welcome to our hotel.</h1>` +
        `<h2 class="subheading heading-stack">We're happy to have you here.</h2>`,
    );
  });

  it("keeps br as newline inside one Main heading", () => {
    const item = {
      type: "heading",
      label: "H1",
      content: "Line one<br>Line two",
    };
    expect(headingToXml(item, esc, esc)).toBe(
      `<heading label="H1" type="Main">Line one\nLine two</heading>`,
    );
    expect(headingToPreviewHtml(item, esc)).toBe(
      `<h1 class="heading">Line one<br>Line two</h1>`,
    );
  });

  it("marks Preview Sub with heading-after-blank when Design had a blank line", () => {
    const item = {
      type: "heading",
      label: "H1",
      content:
        'Welcome to our hotel<br><br><span class="heading-size-sub">We hope you\'ll have a great stay!</span>',
    };
    expect(headingToPreviewHtml(item, esc)).toBe(
      `<h1 class="heading">Welcome to our hotel</h1>` +
        `<h2 class="subheading heading-after-blank">We hope you'll have a great stay!</h2>`,
    );
  });
});
