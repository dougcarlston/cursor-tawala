import { describe, expect, it } from "vitest";
import { registrationTextToXml } from "./registrationTextToXml.mjs";

const dirtBowlForm = {
  name: "Registration",
  items: [
    {
      type: "fib",
      label: "Q1",
      blanks: [
        { name: "a", alternateLabel: "FirstName" },
        { name: "b", alternateLabel: "LastName" },
      ],
    },
  ],
};

describe("registrationTextToXml", () => {
  it("returns hardcoded T4 when Design has no table", () => {
    const xml = registrationTextToXml(
      { label: "T4", content: "" },
      "Registration",
      dirtBowlForm,
    );
    expect(xml).toBeTruthy();
    expect(xml).toContain("<table");
    expect(xml).toContain('field name="League"');
  });

  it("defers to Design HTML when T4 content includes a table", () => {
    const xml = registrationTextToXml(
      {
        label: "T4",
        content:
          '<table class="user user-border-1"><tbody><tr><td style="width: 50pt;">Registration:</td></tr></tbody></table>',
      },
      "Registration",
      dirtBowlForm,
    );
    expect(xml).toBeNull();
  });

  it("still hardcodes T2/T3 paragraph blocks without tables", () => {
    expect(
      registrationTextToXml(
        { label: "T2", content: "<p>Mill Valley Dirt Bowl</p>" },
        "Registration",
        dirtBowlForm,
      ),
    ).toBeTruthy();
    expect(
      registrationTextToXml({ label: "T3", content: "" }, "Registration", dirtBowlForm),
    ).toBeTruthy();
  });

  it("skips non-DirtBowl Registration forms", () => {
    expect(
      registrationTextToXml(
        { label: "T4", content: "" },
        "Registration",
        { name: "Registration", items: [{ type: "fib", label: "Q1", blanks: [] }] },
      ),
    ).toBeNull();
  });
});
