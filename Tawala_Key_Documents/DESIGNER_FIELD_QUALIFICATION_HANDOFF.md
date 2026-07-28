# Field qualification & chat demarcation — handoff for Designer bug-fix chat

**Jul 28, 2026** — Owner wants all chats aligned on **where field-reference bugs are fixed** vs **Deploy/runtime/wiring** work.

Use this file when reopening the **Designer branch** (`cursor/forms-canvas-wysiwyg`, repo `/Users/DougC1/Projects/Tawala/designer-web/`).

---

## Paste into a new Designer chat (copy from here down)

```
Browser Designer field-reference handoff (Jul 28, 2026).

Track: browser Designer ONLY (`designer-web/` — Design canvas, Fields palette, Process/Document/Send editors). Branch `cursor/forms-canvas-wysiwyg`. Repo `/Users/DougC1/Projects/Tawala`.

DO NOT in this chat: Tomcat/Java email SMTP, docker theme CSS, .tawala→JSON converter, SportsDashboards full compare, Website/Library rebuild.

Problem: Drag/drop from Fields palette **Form branch** into Document (and possibly Send subject/body) inserts bare tokens like `<<Email>>`. Java runtime treats bare names as **process variables**, not FIB/form data → blank email bodies. Must insert `<<Form 1:Email>>` (qualified by form folder name + blank alternate label).

Rules (legacy/Java truth — see Reference.java `isVariable()` = no Form: prefix):
- Bare name (`<<Email>>`, `<<FullName>>`) → **process variable** only.
- `<<Form:Field>>` → current form submission (Send body, Document after submit).
- `<<Record:Form:Field>>` → stored rows (MQL/itemization, record functions).
- Variables folder leaves stay bare; Form folder leaves must qualify when inserted into Document/Send/expression contexts that read submission data.

Fix targets: `fieldInsertion.ts` (`qualifyPaletteFieldName`, drag MIME `FIELD_DRAG_FORM_MIME`), Document editor drop path, field token display (chip should show or store qualified name). Stock Signup Sheet w Email .tawala uses `Form 1:Email` in document XML — match that.

Already fixed elsewhere (do not re-open as Designer canvas bugs):
- Process SET math/text export → `designer-web/server/jsonToXml.mjs` (Jul 27).
- Resend SMTP From/Reply-To sanitization → Java `Email.java` (Jul 27).
- 28 theme CSS → `docker/tomcat/css/project/` (Jul 27).

Smoke after fix:
1. New Document on Sign-up project; drag Email from Form 1 → token must be `<<Form 1:Email>>` (not `<<Email>>`).
2. Deploy; submit form; Send email body shows typed email.
3. Variable `<<FullName>>` still works when Process does Set FullName before Send.

Read: Tawala_Key_Documents/DESIGNER_FIELD_QUALIFICATION_HANDOFF.md (full demarcation + tables).
```

---

## Why this matters (owner-confirmed Jul 28)

Sign-up Sheet w Email **Send works** after SMTP fixes. Email **body was blank** because the Document used:

- `<<FullName>>` under **Variables** → works if Process **Set** that variable before Send.
- `<<Email>>`, `<<Tel>>`, `<<Address>>` dragged from **Form 1** → **wrong**: runtime looks for variables named Email/Tel/Address (empty), not the FIB blanks.

Legacy stock template (`Signup Sheet Template w Email.tawala`) uses **`Form 1:FirstName`**, **`Form 1:Email`**, etc. in the document — not bare names.

---

## Chat demarcation — which agent fixes what

| Track | Repo / paths | Fix here when… | **Not** this chat |
|-------|----------------|----------------|-------------------|
| **A. Browser Designer** | `designer-web/src/` — Forms/Process/Document canvas, Fields palette, statement builders, menus | Palette insert wrong token; chip shows short name; Send editor allows invalid From alias; Document caret/palette UX | Tomcat, converter, themes |
| **B. Deploy export / Preview runtime** | `designer-web/server/` — `jsonToXml.mjs`, `documentHtmlToXml.mjs`, fib/MQL export | XML wrong at Deploy; SET math exported as literal text; field refs stripped in export | `*CanvasRow` idle/edit UX |
| **C. Java runtime / Tomcat / email** | `TawalaWebapp-build1700/`, `docker/`, `.env.email.local` | SMTP, Resend, queue worker, From/Reply-To at send time, 8080 layout CSS | Designer palette labels |
| **D. .tawala → JSON converter** | `designer-web/src/lib/tawalaXmlToJson.mjs` | Library reconverts; invitation/hyperlink tokens; structured Text | Design canvas |
| **E. Website / Library (future)** | Java Library JSPs, API, Postgres | Later rebuild | Designer + Deploy for now |
| **F. Legacy C# documentation** | `Tawala_Key_Documents/DESIGNER_*.md` | Owner screenshots → specs | Code unless owner asks |

**Default rule:** If the owner sees wrong **authoring** (what got inserted in the project JSON/HTML), fix **A**. If Deploy XML or runtime behavior is wrong but the project JSON looks right, fix **B** or **C**.

**Design canvas ≫ Preview** — see `.cursor/rules/tawala-work-scopes.mdc`. Do not change Form/Document canvas rows to make Preview match.

---

## Field reference rules (runtime — do not “simplify” in Designer)

Java: `Reference.isVariable()` is true when there is **no `Form:` prefix** — checked **before** “use current submission.”

| Reference in project | Meaning | Example |
|----------------------|---------|---------|
| `<<Name>>` | Process **variable** `Name` | `<<FullName>>` after `Set FullName to …` |
| `<<Form:Field>>` | Blank on form **Form** (alternate label **Field**) | `<<Form 1:Email>>` |
| `<<Record:Form:Field>>` | Stored submission row (MQL, functions) | Itemization column |

**FIB internal labels** (`a`, `b` on FIB1) are not what Documents use — **alternate label** is (e.g. `Email`, `firstName`). Two forms can both have `FIB1:a`; disambiguation is **`Form 1:…` vs `Form 2:…`**, not bare `a`.

**Numeric vs text variables** — SET typing and Deploy export: see `DESIGNER_VARIABLES_TYPING_HANDOFF.md` (Deploy side **fixed Jul 27**).

---

## Designer bugs to fix (this chat’s backlog)

### P1 — Fields palette → Document (owner hit this Jul 28)

- **Symptom:** Drag Email/Tel/Address from **Form 1** into Document → `<<Email>>` not `<<Form 1:Email>>`.
- **Expected:** `qualifyPaletteFieldName` + `FIELD_DRAG_FORM_MIME` should qualify on drop/double-click (`fieldInsertion.ts`, Document/RichTextEditor drop handlers).
- **Verify:** Sign-up **NewSignup** document; redeploy; Send body shows contact fields.

### P1 — Show qualified names on field chips (optional polish)

- Chips may display short names while storing wrong bare refs — tooltip or visible `Form 1:Email` reduces confusion.

### P2 — Audit other insert targets

- Send **Subject** / expression boxes: same qualification as Document?
- Process **If** field: may use bare `Form:Field` without `<<>>` (see `FieldTargetContext.bare`) — different rule, do not break.
- Configure Function **Where**: may need `Record:Form:Field` for record selectors (`mcDynamicConfig.ts`).

### Done in Designer recently (Jul 27) — do not duplicate

- Send **From (Name)** cannot combine two fields — `validateSendFromName` in `sendEmailValidation.ts`; combine with **Set** first. See `DESIGNER_PROCESS_STATEMENTS_SEND.md`.

---

## Already fixed outside Designer (Jul 27 — other chats)

| Issue | Where fixed | Designer action |
|-------|-------------|-----------------|
| SET math treated as text | `server/jsonToXml.mjs` | None — restart :3001 after server edits |
| Resend 550 From/Reply-To | `Email.java` `buildSafeReplyTo` | None |
| Missing row spacing on Deploy | `docker/tomcat/css/form-layout-core.css` baked in image | None |
| 28 themes | `docker/tomcat/css/project/` | TODO #15 largely done |
| File Uploader | Deferred Jul 27 — palette hidden | Do not implement this build |

---

## Smoke checklist (after palette fix)

1. **Insert:** Form 1 → drag **Email** into Document → stored token `<<Form 1:Email>>`.
2. **Variable unchanged:** drag **FullName** from Variables → still `<<FullName>>`.
3. **Deploy + submit:** Sign-up Sheet w Email → inbox body shows name + email + phone + address.
4. **MQL table** on same form still uses `<<Form 1:Email>>` or `Record:Form 1:Email>>` per context (see signup-sheet.json sample).

---

## Key code & spec pointers

| Area | Path |
|------|------|
| Qualify on insert | `designer-web/src/lib/fieldInsertion.ts` |
| Fields palette | `designer-web/src/components/FieldsPalette.tsx` |
| Document export qualify | `designer-web/server/documentHtmlToXml.mjs` (`qualifyFieldRef`) |
| Send spec | `Tawala_Key_Documents/DESIGNER_PROCESS_STATEMENTS_SEND.md` |
| Stock signup w email | `Tawala-projects/Templates/Signup Sheet Template w Email.tawala` |
| Work scopes | `.cursor/rules/tawala-work-scopes.mdc` |
| SET variables handoff | `Tawala_Key_Documents/DESIGNER_VARIABLES_TYPING_HANDOFF.md` |

---

## Branch & workspace

- **Active repo:** `/Users/DougC1/Projects/Tawala` (not empty `~/Projects/AI-Tawala` stub).
- **Designer:** `cd designer-web && npm run dev` → http://localhost:5173 , API :3001.
- **Deploy test:** http://localhost:8080 , creds `dev`/`dev`.
