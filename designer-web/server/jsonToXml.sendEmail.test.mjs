import { describe, expect, it } from "vitest";
import { projectToXml } from "./jsonToXml.mjs";
import {
  buildQueryEmailStatusXml,
  buildSendTestEmailXml,
  parseEmailStatusXml,
} from "./emailStatus.mjs";

describe("send command XML", () => {
  it("emits addressLiteral for literal To/From and omits bcc", () => {
    const xml = projectToXml({
      name: "Mail",
      forms: [{ name: "Form 1", startPoint: true, items: [] }],
      processes: [
        {
          name: "Process 1",
          commands: [
            {
              cmd: "send",
              to: { literal: "to@example.com" },
              from: { literal: "from@example.com", aliasLiteral: "Sender" },
              cc: { literal: "cc@example.com" },
              bcc: { literal: "secret@example.com" },
              subject: "Hello",
              body: { document: "Document 1", reset: false, showHeader: true },
            },
          ],
        },
      ],
      documents: [{ name: "Document 1", content: "<p>Hi</p>" }],
    });
    expect(xml).toContain('<to addressLiteral="to@example.com"/>');
    expect(xml).toContain('<from addressLiteral="from@example.com" aliasLiteral="Sender"/>');
    expect(xml).toContain('<cc addressLiteral="cc@example.com"/>');
    expect(xml).not.toContain("<bcc");
    expect(xml).toContain("<subject>Hello</subject>");
    expect(xml).toContain('document="Document 1"');
  });

  it("emits addressField for Form:Field recipients", () => {
    const xml = projectToXml({
      name: "Mail",
      forms: [{ name: "Start", startPoint: true, items: [] }],
      processes: [
        {
          name: "Process 1",
          commands: [
            {
              cmd: "send",
              to: { fieldRef: "Start:UserEmail" },
              from: { fieldRef: "AdminEmail", aliasField: "AdminName" },
              subject: "Welcome",
              body: { document: "Doc", reset: true, showHeader: false },
            },
          ],
        },
      ],
      documents: [{ name: "Doc", content: "" }],
    });
    expect(xml).toContain('<to addressField="Start:UserEmail"/>');
    expect(xml).toContain('<from addressField="AdminEmail" aliasField="AdminName"/>');
    expect(xml).toContain('reset="true"');
    expect(xml).toContain('showHeader="false"');
  });

  it("serializes dynamic subject field tokens", () => {
    const xml = projectToXml({
      name: "Mail",
      forms: [{ name: "Form 1", startPoint: true, items: [] }],
      processes: [
        {
          name: "Process 1",
          commands: [
            {
              cmd: "send",
              to: { literal: "a@b.com" },
              subject: "<<Head>> - Confirmation",
              body: { document: "Doc" },
            },
          ],
        },
      ],
      documents: [{ name: "Doc", content: "" }],
    });
    expect(xml).toContain('<subject><field name="Head"/> - Confirmation</subject>');
  });
});

describe("emailStatus parse", () => {
  it("parses success attributes without exposing secrets", () => {
    const xml = `<?xml version="1.0"?>
<response status="success">
  <emailStatus enabled="true" configured="true" host="mailpit" port="1025"
    auth="false" starttls="false" fromAddress="noreply@tawala.local" fromName="Tawala Local"
    workerEnabled="true" readyCount="1" sendingCount="0" sentCount="2" errorCount="0"
    lastError="" lastErrorAt="0" lastSuccessAt="1" lastWorkerRunAt="2" message="ok"/>
</response>`;
    const parsed = parseEmailStatusXml(xml, "java");
    expect(parsed.available).toBe(true);
    expect(parsed.configured).toBe(true);
    expect(parsed.host).toBe("mailpit");
    expect(parsed.port).toBe(1025);
    expect(parsed.sentCount).toBe(2);
    expect(parsed.message).toBe("ok");
    expect(JSON.stringify(parsed)).not.toMatch(/password/i);
  });

  it("parses auth failures", () => {
    const xml = `<response status="failure"><error id="auth.failed" message="unknown userid or password"/></response>`;
    const parsed = parseEmailStatusXml(xml, "java");
    expect(parsed.available).toBe(false);
    expect(parsed.error).toContain("unknown userid");
  });

  it("builds client XML without leaking extra fields", () => {
    const statusXml = buildQueryEmailStatusXml({ user: "dev", password: "dev" });
    expect(statusXml).toContain('type="queryEmailStatus"');
    expect(statusXml).toContain('user="dev"');
    const testXml = buildSendTestEmailXml({ user: "dev", password: "dev" }, "a@b.com");
    expect(testXml).toContain('type="sendTestEmail"');
    expect(testXml).toContain('to="a@b.com"');
  });
});
