/**
 * Focused XML → format 2.0 JSON conversion smoke for File → Open / CLI.
 */
import { describe, expect, it } from "vitest";
import {
  convertTawalaXmlToProject,
  countProcessCommands,
  isTawalaProjectFileName,
} from "@/lib/tawalaXmlToJson.mjs";

const MINI_PROJECT = `<?xml version="1.0" encoding="utf-8" ?>
<project name="MiniImport" themePath="default" format="1.9" designerBuild="203">
  <forms>
    <form name="Form 1" startPoint="true" process="Post">
      <items>
        <heading type="Main" label="H1">
          <paragraph align="left" indent="0"><font face="Arial" size="280">Welcome</font></paragraph>
        </heading>
        <fib label="Q1" style="labelBeforeBlank">
          <paragraph align="left" indent="0">Name <blank label="a" length="20" required="true"/></paragraph>
        </fib>
      </items>
    </form>
  </forms>
  <processes>
    <process name="Post">
      <show form="Form 1"/>
      <set field="Form 1:Done"><string value="yes"/></set>
    </process>
  </processes>
  <documents>
    <document name="Thanks">
      <xmlData>
        <paragraph align="left" indent="0"><font face="Arial" size="240">Thanks!</font></paragraph>
      </xmlData>
    </document>
  </documents>
  <images>
    <imagedef id="logo1">
      <imagedata imageFormat="PNG">iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mP8z8BQDwAEhQGAhKmMIQAAAABJRU5ErkJggg==</imagedata>
    </imagedef>
  </images>
</project>
`;

describe("convertTawalaXmlToProject", () => {
  it("maps project shell, form items, process commands, document, and imagedef", () => {
    const { project: raw, warnings } = convertTawalaXmlToProject(MINI_PROJECT, {
      sourceLabel: "fixture.tawala",
    });
    const project = raw as {
      name: string;
      format: string;
      themePath: string;
      _convertedFrom: string;
      _originalFormat: string;
      forms: Array<{
        name: string;
        startPoint: boolean;
        process: string;
        items: Array<{ type: string; content?: string; blanks?: unknown[] }>;
      }>;
      processes: Array<{ commands: unknown[] }>;
      documents: Array<{ name: string; content: unknown }>;
      images: Array<{ id: string; imageFormat: string; data: string }>;
    };

    expect(project.name).toBe("MiniImport");
    expect(project.format).toBe("2.0");
    expect(project.themePath).toBe("default");
    expect(project._convertedFrom).toBe("fixture.tawala");
    expect(project._originalFormat).toBe("1.9");

    expect(project.forms).toHaveLength(1);
    expect(project.forms[0].name).toBe("Form 1");
    expect(project.forms[0].startPoint).toBe(true);
    expect(project.forms[0].process).toBe("Post");
    expect(project.forms[0].items.map((i) => i.type)).toEqual(["heading", "fib"]);
    expect(project.forms[0].items[0].content).toContain("Welcome");
    const fib = project.forms[0].items[1] as {
      type: string;
      prompt?: string;
      blanks?: unknown[];
    };
    expect(fib.blanks).toHaveLength(1);
    expect(fib.prompt).toMatch(/^Name _{20}$/);
    expect(fib.prompt).toContain("____________________");

    expect(project.processes).toHaveLength(1);
    expect(project.processes[0].commands).toEqual([
      { cmd: "show", form: "Form 1" },
      { cmd: "set", field: "Form 1:Done", value: "yes", concat: false },
    ]);
    expect(countProcessCommands(project.processes[0].commands)).toBe(2);

    expect(project.documents).toHaveLength(1);
    expect(project.documents[0].name).toBe("Thanks");
    expect(Array.isArray(project.documents[0].content)).toBe(true);

    expect(project.images).toHaveLength(1);
    expect(project.images[0]).toMatchObject({
      id: "logo1",
      imageFormat: "PNG",
    });
    expect(project.images[0].data.length).toBeGreaterThan(20);

    // No pageHeader/styles in fixture — warnings may still be empty or unrelated.
    expect(Array.isArray(warnings)).toBe(true);
  });

  it("rejects XML without a project root", () => {
    expect(() => convertTawalaXmlToProject("<root/>")).toThrow(/No <project>/);
  });

  it("embeds Form Text <image> as data-URL img using project imagedefs", () => {
    const xml = `<?xml version="1.0" encoding="utf-8" ?>
<project name="Img" themePath="default" format="1.9">
  <forms>
    <form name="Form 1" startPoint="true">
      <items>
        <text label="T1" style="instructional">
          <paragraph align="left" indent="0">
            <font>Click </font>
            <font><image id="image1" width="22" height="23"/></font>
            <font> to deploy.</font>
          </paragraph>
        </text>
      </items>
    </form>
  </forms>
  <processes></processes>
  <documents></documents>
  <images>
    <imagedef id="image1">
      <imagedata imageFormat="PNG">iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mP8z8BQDwAEhQGAhKmMIQAAAABJRU5ErkJggg==</imagedata>
    </imagedef>
  </images>
</project>`;
    const { project } = convertTawalaXmlToProject(xml);
    const text = (project as { forms: Array<{ items: Array<{ type: string; content?: string }> }> })
      .forms[0].items[0];
    expect(text.type).toBe("text");
    expect(typeof text.content).toBe("string");
    expect(text.content).toContain('data-tawala-image-id="image1"');
    expect(text.content).toContain("data:image/png;base64,");
    expect(text.content).toContain('class="tawala-embedded-image"');
  });

  it("converts Form Text <table> with <division> cells to editable HTML", () => {
    const xml = `<?xml version="1.0" encoding="utf-8" ?>
<project name="Tbl" themePath="default" format="1.9">
  <forms>
    <form name="Form 1" startPoint="true">
      <items>
        <text label="T5" style="normal">
          <paragraph align="left" indent="0"><font>You entered:</font></paragraph>
          <table indent="0">
            <row>
              <cell width="2115"><division indent="0" align="left"><font>Name</font></division></cell>
              <cell width="8370"><division indent="0" align="left"><font><field name="Form 1:attendeeName"/></font></division></cell>
            </row>
            <row>
              <cell width="2115"><division indent="0" align="left"><font>Dish</font></division></cell>
              <cell width="8370"><division indent="0" align="left"><font><field name="Form 1:contribution"/></font></division></cell>
            </row>
          </table>
        </text>
      </items>
    </form>
  </forms>
  <processes></processes>
  <documents></documents>
</project>`;
    const { project } = convertTawalaXmlToProject(xml);
    const text = (project as { forms: Array<{ items: Array<{ type: string; content?: unknown }> }> })
      .forms[0].items[0];
    expect(typeof text.content).toBe("string");
    const html = String(text.content);
    expect(html).toContain("<table");
    expect(html).toContain("Name");
    expect(html).toContain("Dish");
    expect(html).toContain('data-field-name="Form 1:attendeeName"');
    expect(html).toContain('data-field-name="Form 1:contribution"');
    expect(Array.isArray(text.content)).toBe(false);
  });
});

describe("isTawalaProjectFileName", () => {
  it("accepts .tawala and .tawala.xml", () => {
    expect(isTawalaProjectFileName("DirtBowl.tawala")).toBe(true);
    expect(isTawalaProjectFileName("SignupSheets.tawala.xml")).toBe(true);
    expect(isTawalaProjectFileName("my.tawala.XML")).toBe(true);
  });

  it("rejects plain JSON and unrelated XML", () => {
    expect(isTawalaProjectFileName("project.json")).toBe(false);
    expect(isTawalaProjectFileName("config.xml")).toBe(false);
  });
});
