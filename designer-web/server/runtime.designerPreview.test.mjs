/**
 * Designer Form Preview: no Submit navigation; rich HTML must not get League `""` rewrite.
 */
import { describe, expect, it } from "vitest";
import { createSession } from "./sessionStore.mjs";
import { renderFormPage } from "./runtime.mjs";

function potluckLikeProject() {
  return {
    name: "potluck",
    themePath: "default",
    forms: [
      {
        name: "Potluck Organizer",
        themePath: "default",
        items: [
          {
            type: "text",
            label: "T2",
            content: "Fill in the form below.",
          },
          {
            type: "fib",
            label: "Q1",
            prompt: "Name______",
            blanks: [{ name: "attendeeName", length: 20 }],
          },
          {
            type: "skipInstructions",
            label: "skip1",
            commands: [{ cmd: "skip", to: "__EndOfForm__" }],
          },
          {
            type: "text",
            label: "T6",
            content:
              'You entered the following information:<div data-doc-blank="1" style="min-height: 13.7pt;">' +
              '<table class="user user-border-2" style="position: relative; width: 216pt; left: 57.3pt; top: 12.3pt;">' +
              '<tr><td style="width: 108pt;" class="">Name&nbsp;</td>' +
              '<td style="width: 108pt;" class="">&nbsp;&lt;&lt;attendeeName&gt;&gt;</td></tr>' +
              "</table></div>",
          },
          {
            type: "mc",
            label: "Q5",
            question: "Is this information correct?",
            choices: [
              { text: "Yes", value: "a" },
              { text: "No", value: "b" },
            ],
          },
        ],
      },
    ],
  };
}

describe("designer Preview (review mode)", () => {
  it("disables Submit and does not post", () => {
    const project = potluckLikeProject();
    const html = renderFormPage(
      project,
      "Potluck Organizer",
      "http://localhost:5173",
      "preview-designer",
      createSession(project),
      { designerPreview: true },
    );
    expect(html).toMatch(/type="submit"[^>]*\bdisabled\b/);
    expect(html).toContain("onsubmit=\"return false;\"");
    expect(html).toContain("Preview only");
  });

  it("shows all segments so confirmation page is visible without Submit", () => {
    const project = potluckLikeProject();
    const html = renderFormPage(
      project,
      "Potluck Organizer",
      "http://localhost:5173",
      "preview-designer",
      createSession(project),
      { designerPreview: true },
    );
    expect(html).toContain("Fill in the form below.");
    expect(html).toContain("You entered the following information");
    expect(html).toContain("Is this information correct?");
    expect(html).toContain("Page 2 (after Submit)");
  });

  it("does not rewrite empty HTML attributes class=\"\" into Dirt Bowl", () => {
    const project = potluckLikeProject();
    const html = renderFormPage(
      project,
      "Potluck Organizer",
      "http://localhost:5173",
      "preview-designer",
      createSession(project),
      { designerPreview: true },
    );
    expect(html).not.toMatch(/class=Dirt Bowl/);
    expect(html).not.toContain("class=Dirt Bowl");
    expect(html).toContain("You entered the following information");
    expect(html).toContain("<table");
    // Cell markup stays intact (empty class ok; must not inject league name into attrs).
    expect(html).toMatch(/<td[^>]*>Name/);
  });

  it("does not invent Dirt Bowl for plain-text \"\" on non-Registration forms", () => {
    const project = {
      name: "LeaguePlain",
      forms: [
        {
          name: "Form 1",
          items: [{ type: "text", label: "T1", content: 'Welcome to "" registration.' }],
        },
      ],
    };
    const html = renderFormPage(
      project,
      "Form 1",
      "http://localhost:5173",
      "uid-league",
      createSession(project),
    );
    expect(html).toContain("Welcome to &quot;&quot; registration.");
    expect(html).not.toContain("Dirt Bowl");
  });

  it("substitutes plain-text \"\" when League has a value (even off Registration)", () => {
    const project = {
      name: "LeaguePlain",
      forms: [
        {
          name: "Form 1",
          items: [{ type: "text", label: "T1", content: 'Welcome to "" registration.' }],
        },
      ],
    };
    const session = createSession(project);
    session.fields.League = "Mill Valley League";
    const html = renderFormPage(
      project,
      "Form 1",
      "http://localhost:5173",
      "uid-league-set",
      session,
    );
    expect(html).toContain("Welcome to Mill Valley League registration.");
    expect(html).not.toContain("&quot;&quot;");
  });

  it("substitutes plain-text \"\" on Registration for non-layout labels when League is set", () => {
    const project = {
      name: "DirtBowl",
      forms: [
        {
          name: "Registration",
          // TX is not a registrationLayout special label — falls through to enhancePlainText.
          items: [{ type: "text", label: "TX", content: 'Welcome to "" registration.' }],
        },
      ],
    };
    const session = createSession(project);
    session.fields.League = "Mill Valley League";
    const html = renderFormPage(
      project,
      "Registration",
      "http://localhost:5173",
      "uid-reg-league",
      session,
    );
    expect(html).toContain("Welcome to Mill Valley League registration.");
    expect(html).not.toContain("&quot;&quot;");
  });

  it("live / Deploy path still has an active Submit", () => {
    const project = potluckLikeProject();
    const html = renderFormPage(
      project,
      "Potluck Organizer",
      "http://localhost:5173",
      "deploy-uid",
      createSession(project),
    );
    expect(html).toMatch(/<input type="submit"[^>]*value="Submit"/);
    expect(html).not.toMatch(/type="submit"[^>]*\bdisabled\b/);
    expect(html).not.toContain("onsubmit=\"return false;\"");
  });

  it("emits Instructional / Error classes and Preview CSS for Text styles", () => {
    const project = {
      name: "Styles",
      forms: [
        {
          name: "Form 1",
          items: [
            { type: "text", label: "T1", style: "instructional", content: "Help text" },
            { type: "text", label: "T2", style: "error", content: "Bad value" },
          ],
        },
      ],
    };
    const html = renderFormPage(
      project,
      "Form 1",
      "http://localhost:5173",
      "uid-styles",
      createSession(project),
      { designerPreview: true },
    );
    expect(html).toContain('class="text instructional"');
    expect(html).toContain('class="text error text-item-error"');
    expect(html).toContain("div.text.instructional");
    expect(html).toContain("color: #000080");
    expect(html).toContain("color: #c00000");
  });
});
