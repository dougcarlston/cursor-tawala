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
    expect(typeof project.documents[0].content).toBe("string");
    expect(String(project.documents[0].content)).toContain("Thanks!");
    expect(String(project.documents[0].content)).toContain("Arial");

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

  it("imports Document htmlData when xmlData is absent", () => {
    const xml = `<?xml version="1.0" encoding="utf-8" ?>
<project name="HtmlOnlyDoc" themePath="default" format="1.9">
  <forms><form name="Form 1" startPoint="true"><items/></form></forms>
  <processes/>
  <documents>
    <document name="Score">
      <htmlData><![CDATA[<p>Hello &lt;&lt;FirstName&gt;&gt; — score &lt;&lt;Score&gt;&gt;%</p>]]></htmlData>
      <rawHtmlData><![CDATA[<html><body><p>ignored shell</p></body></html>]]></rawHtmlData>
    </document>
  </documents>
</project>`;
    const { project } = convertTawalaXmlToProject(xml, { sourceLabel: "htmlOnly.tawala" });
    const docs = project.documents as Array<{ name: string; content: string }>;
    expect(docs).toHaveLength(1);
    expect(docs[0].name).toBe("Score");
    expect(docs[0].content).toContain("Hello");
    expect(docs[0].content).toContain('data-field-name="FirstName"');
    expect(docs[0].content).toContain('data-field-name="Score"');
    expect(docs[0].content).toContain("&lt;&lt;FirstName&gt;&gt;");
    expect(docs[0].content).not.toContain("ignored shell");
  });

  it("converts Form Text field refs to HTML field-token (not structured array)", () => {
    const xml = `<?xml version="1.0" encoding="utf-8" ?>
<project name="FieldText" themePath="default" format="1.9">
  <forms>
    <form name="Form 1" startPoint="true">
      <items>
        <text label="T1" style="normal">
          <paragraph align="left" indent="0"><font><field name="Form:Name"/></font></paragraph>
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
    expect(text.type).toBe("text");
    expect(typeof text.content).toBe("string");
    expect(Array.isArray(text.content)).toBe(false);
    const html = String(text.content);
    expect(html).toContain('class="field-token');
    expect(html).toContain('data-field-name="Form:Name"');
    expect(html).toContain("&lt;&lt;Form:Name&gt;&gt;");
  });

  it("converts invitation displayText from string value without empty-text warning", () => {
    const xml = `<?xml version="1.0" encoding="utf-8" ?>
<project name="Inv" themePath="default" format="1.9">
  <forms>
    <form name="Form 1" startPoint="true">
      <items>
        <text label="T1" style="normal">
          <paragraph align="left" indent="0">
            <font><invitation form="NextForm" project="Inv"><displayText><string value="Continue"/></displayText></invitation></font>
          </paragraph>
        </text>
      </items>
    </form>
  </forms>
  <processes></processes>
  <documents></documents>
</project>`;
    const { project, warnings } = convertTawalaXmlToProject(xml);
    const html = String(
      (project as { forms: Array<{ items: Array<{ content?: string }> }> }).forms[0].items[0].content,
    );
    expect(html).toContain('class="invitation-token"');
    expect(html).toContain("Continue");
    expect(html).toContain("&quot;displayText&quot;:&quot;Continue&quot;");
    expect(warnings.some((w) => w.includes('text=""'))).toBe(false);
  });

  it("converts hyperlink link element to hyperlink-token HTML", () => {
    const xml = `<?xml version="1.0" encoding="utf-8" ?>
<project name="Link" themePath="default" format="1.9">
  <forms>
    <form name="Form 1" startPoint="true">
      <items>
        <text label="T1" style="normal">
          <paragraph align="left" indent="0">
            <font><link><description><string value="Help"/></description><url><string value="http://x"/></url></link></font>
          </paragraph>
        </text>
      </items>
    </form>
  </forms>
  <processes></processes>
  <documents></documents>
</project>`;
    const { project, warnings } = convertTawalaXmlToProject(xml);
    const html = String(
      (project as { forms: Array<{ items: Array<{ content?: string }> }> }).forms[0].items[0].content,
    );
    expect(html).toContain('class="hyperlink-token"');
    expect(html).toContain("Help");
    expect(html).toContain("&quot;url&quot;:&quot;http://x&quot;");
    expect(html).toContain("&quot;displayText&quot;:&quot;Help&quot;");
    expect(warnings.some((w) => w.includes("Hyperlink in rich content — empty"))).toBe(false);
  });

  it("converts Document paragraph field to HTML field-token", () => {
    const xml = `<?xml version="1.0" encoding="utf-8" ?>
<project name="DocField" themePath="default" format="1.9">
  <forms><form name="Form 1" startPoint="true"><items/></form></forms>
  <processes/>
  <documents>
    <document name="Letter">
      <xmlData>
        <paragraph align="left" indent="0"><font><field name="Record:Form 1:Name"/></font></paragraph>
      </xmlData>
    </document>
  </documents>
</project>`;
    const { project } = convertTawalaXmlToProject(xml);
    const html = String((project.documents as Array<{ content: string }>)[0].content);
    expect(html).toContain('class="field-token');
    expect(html).toContain('data-field-name="Record:Form 1:Name"');
    expect(html).toContain("&lt;&lt;Record:Form 1:Name&gt;&gt;");
  });

  it("converts Document table invitations with displayText (CYO Dashboard grid)", () => {
    // CYO Dance Agreement Document Dashboard: five Form-link invitations in a table.
    const xml = `<?xml version="1.0" encoding="utf-8" ?>
<project name="CYO" themePath="plain" format="1.16">
  <forms>
    <form name="ViewAll" startPoint="true"><items/></form>
    <form name="Custom Report" startPoint="false"><items/></form>
  </forms>
  <processes/>
  <documents>
    <document name="Dashboard">
      <xmlData>
        <table indent="24">
          <row>
            <cell width="3828"><division align="center"><font color="0066CC"><u>
              <invitation form="ViewAll" project=""><displayText><string value="View Full Report"/></displayText></invitation>
            </u></font></division></cell>
            <cell width="3588"><division align="center"><font color="0066CC"><u>
              <invitation form="Custom Report" project=""><displayText><string value="View Custom Report"/></displayText></invitation>
            </u></font></division></cell>
          </row>
        </table>
      </xmlData>
    </document>
  </documents>
</project>`;
    const { project, warnings } = convertTawalaXmlToProject(xml);
    const html = String((project.documents as Array<{ content: string }>)[0].content);
    expect(html).toContain('class="invitation-token"');
    expect(html).toContain("&quot;form&quot;:&quot;ViewAll&quot;");
    expect(html).toContain("&quot;displayText&quot;:&quot;View Full Report&quot;");
    expect(html).toContain("&quot;form&quot;:&quot;Custom Report&quot;");
    expect(html).toContain("&quot;displayText&quot;:&quot;View Custom Report&quot;");
    expect(warnings.some((w) => w.includes('text=""'))).toBe(false);
  });

  it("imports Document table cells with SUM as function chips and escapes field brackets", () => {
    const xml = `<?xml version="1.0" encoding="utf-8" ?>
<project name="SumDoc" themePath="default" format="1.9">
  <forms><form name="Form 1" startPoint="true"><items/></form></forms>
  <processes/>
  <documents>
    <document name="Details">
      <xmlData>
        <paragraph align="left" indent="0"><font><b>Title</b></font></paragraph>
        <table indent="2685">
          <row>
            <cell width="1450"><division align="right"><font>Adults:</font></division></cell>
            <cell width="2700"><division align="left"><font><sum version="1"><field>Record:Potluck Organizer:numAdults</field></sum></font></division></cell>
          </row>
        </table>
        <paragraph align="left" indent="0"><font><field name="Customize_eventHeader"/></font></paragraph>
      </xmlData>
    </document>
  </documents>
</project>`;
    const { project } = convertTawalaXmlToProject(xml, { sourceLabel: "sumDoc.tawala" });
    const html = String((project.documents as Array<{ content: string }>)[0].content);
    expect(html).toContain("<table");
    expect(html).toContain("margin-left:134.25pt");
    expect(html).toContain('data-function-id="sum"');
    expect(html).toContain("Potluck Organizer:numAdults");
    expect(html).toContain('data-field-name="Customize_eventHeader"');
    expect(html).toMatch(/field-token[^>]*>&lt;&lt;Customize_eventHeader&gt;&gt;/);
    expect(html).not.toMatch(/field-token[^>]*><</);
  });

  it("keeps sequential FIB blanks as separate underscore runs (Online Exam C1)", () => {
    // One blank per paragraph, like Online Exam Builder Form Question Q5.
    const xml = `<?xml version="1.0" encoding="utf-8" ?>
<project name="MultiBlankFib" themePath="default" format="1.8">
  <forms>
    <form name="Question" startPoint="true">
      <items>
        <text label="T1" alternateLabel="Beginning">
          <paragraph align="left" indent="0"><font>Please enter exam question here:</font></paragraph>
        </text>
        <fib label="Q5" alternateLabel="MCQ choices">
          <paragraph align="left" indent="0"><font><b><field name="Question:Question"/></b></font></paragraph>
          <paragraph align="left" indent="0"><font>Response Choices:</font></paragraph>
          <paragraph align="left" indent="0"><blank label="a" length="28" required="true" alternateLabel="Choice1"/></paragraph>
          <paragraph align="left" indent="0"><blank label="b" length="28" required="true" alternateLabel="Choice2"/></paragraph>
          <paragraph align="left" indent="0"><blank label="c" length="28" alternateLabel="Choice3"/></paragraph>
          <paragraph align="left" indent="0"><blank label="d" length="28" alternateLabel="Choice4"/></paragraph>
          <paragraph align="left" indent="0"><blank label="e" length="28" alternateLabel="Choice5"/></paragraph>
          <paragraph align="left" indent="0"><blank label="f" length="28" alternateLabel="Choice6"/></paragraph>
        </fib>
      </items>
    </form>
  </forms>
  <processes/>
  <documents/>
</project>`;
    const { project } = convertTawalaXmlToProject(xml, { sourceLabel: "c1.tawala" });
    const form = (project.forms as Array<{ items: Array<Record<string, unknown>> }>)[0];
    const t1 = form.items.find((i) => i.label === "T1") as { name?: string };
    expect(t1.name).toBe("Beginning");
    const fib = form.items.find((i) => i.label === "Q5") as {
      name?: string;
      prompt?: string;
      blanks?: Array<{ name: string; alternateLabel?: string }>;
    };
    expect(fib.name).toBe("MCQ choices");
    expect(fib.blanks).toHaveLength(6);
    expect(fib.blanks?.map((b) => b.name)).toEqual([
      "Choice1",
      "Choice2",
      "Choice3",
      "Choice4",
      "Choice5",
      "Choice6",
    ]);
    const runs = (fib.prompt ?? "").match(/_+/g) ?? [];
    expect(runs).toHaveLength(6);
    expect(runs.every((r) => r.length === 28)).toBe(true);
    // Contiguous elision would be one 168-char run with no newlines between.
    expect(fib.prompt).toContain("\n");
    expect(fib.prompt).not.toMatch(/_{40,}/);
  });

  it("keeps flat (non-paragraph) text, FIB blanks, and MC wording — Living Will / Wildcat style", () => {
    // Older Publishable apps put prompt text and <blank> directly under the item
    // (no <paragraph>). Without a flat-body path the canvas looked empty.
    const xml = `<?xml version="1.0" encoding="utf-8" ?>
<project name="FlatFormLegacy" themePath="default" format="1.3">
  <forms>
    <form name="LivingWill" startPoint="true" process="Process 1">
      <items>
        <text label="T1">California Living Will

This program will create a Living Will under the laws of the State of California.
</text>
        <fib label="Q1">Personal Information:  <blank label="a" alternateLabel="FirstName" length="15" required="false"/>  (first) <blank label="b" alternateLabel="LastName" length="15" required="false"/> (last)</fib>
        <text label="T2">First Name: <field name="FirstName"/>
Last Name: <field name="LastName"/></text>
        <mc label="Q3" onlyone="true" required="false"><question>Is this information correct?</question><choice label="a">Yes</choice><choice label="b">No</choice></mc>
      </items>
    </form>
  </forms>
  <processes><process name="Process 1"/></processes>
  <documents/>
</project>`;
    const { project } = convertTawalaXmlToProject(xml, { sourceLabel: "flat.tawala" });
    const form = (project.forms as Array<{ items: Array<Record<string, unknown>> }>)[0];
    const t1 = form.items.find((i) => i.label === "T1") as { content?: string };
    expect(String(t1.content ?? "")).toMatch(/California Living Will/);
    expect(String(t1.content ?? "")).toMatch(/illustrative|Living Will|California/i);

    const fib = form.items.find((i) => i.label === "Q1") as {
      prompt?: string;
      blanks?: Array<{ name: string; alternateLabel?: string }>;
    };
    expect(fib.blanks).toHaveLength(2);
    expect(fib.blanks?.map((b) => b.name)).toEqual(["FirstName", "LastName"]);
    expect(fib.prompt).toMatch(/Personal Information:/);
    expect(fib.prompt).toMatch(/_+/);

    const t2 = form.items.find((i) => i.label === "T2") as { content?: string };
    expect(String(t2.content ?? "")).toMatch(/FirstName/);
    expect(String(t2.content ?? "")).toMatch(/LastName/);

    const mc = form.items.find((i) => i.label === "Q3") as {
      question?: string;
      choices?: Array<{ label: string; text: string }>;
    };
    expect(mc.question).toMatch(/Is this information correct/);
    expect(mc.choices).toEqual([
      { label: "a", text: "Yes" },
      { label: "b", text: "No" },
    ]);
  });

  it("preserves static text Send body + inviteTo (no document attr)", () => {
    const xml = `<?xml version="1.0" encoding="utf-8" ?>
<project name="SendTextBody" themePath="default" format="1.3">
  <forms><form name="Petition" startPoint="true"><items/></form></forms>
  <processes>
    <process name="Process 4">
      <send>
        <to addressField="VoterEmail"/>
        <subject>Hello</subject>
        <body inviteTo="Petition">Dear Friend,

Please help.</body>
      </send>
    </process>
  </processes>
  <documents/>
</project>`;
    const { project } = convertTawalaXmlToProject(xml, { sourceLabel: "send.tawala" });
    const cmd = (project.processes as Array<{ commands: Array<Record<string, unknown>> }>)[0]
      .commands[0] as {
      cmd: string;
      body?: { text?: string; inviteTo?: string; document?: string };
    };
    expect(cmd.cmd).toBe("send");
    expect(cmd.body?.document).toBeUndefined();
    expect(cmd.body?.inviteTo).toBe("Petition");
    expect(cmd.body?.text).toMatch(/Dear Friend/);
    expect(cmd.body?.text).toMatch(/Please help/);
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
