# Start a new Cursor chat — owner guide

**Purpose:** Step-by-step instructions for starting a **fresh** Cursor chat on the AI-Tawala project. You do **not** need to know Cursor well — follow the numbered steps in Part 7, then paste the **Start Script** from Part 8 as your first message.

**Project folder:** `~/Projects/AI-Tawala`

**Latest commit (verify):** `14de400` — run `git log -1` in the project folder to confirm.

---

## Part 1 — What the new chat should read first (in order)

Ask the agent to read these files **before** making changes:

| Order | File | One-line purpose |
|-------|------|------------------|
| 1 | `docs/START_NEW_CHAT.md` | This guide — or paste the Start Script below (Part 8) instead |
| 2 | `docs/CHAT_HANDOFF.md` | Latest session checkpoint, key files, what was done last |
| 3 | `docs/ROADMAP.md` | Living plan — phases, template status, what is parked vs active |
| 4 | `docs/DESIGNER_BACKLOG_ARCHITECTURE.md` | MDI, Fields, Explorer, Properties — architecture backlog from DirtBowl stress test |
| 5 | `docs/COMPARING_RUNTIMES.md` | Preview (5173) vs Deploy (8080) — **only when** you are reopening runtime parity work |
| 6 | Key `Tawala_Key_Documents/DESIGNER_*.md` specs | Legacy Designer UI truth for whatever you are building **now** |

**Specs most relevant to current work (Formatting Palette + Text canvas):**

| Spec | Purpose |
|------|---------|
| `Tawala_Key_Documents/DESIGNER_FORM_FORMAT_TOOLBAR.md` | Row 2 Formatting Palette — 14 controls, enable/disable rules |
| `Tawala_Key_Documents/DESIGNER_FORM_ITEMS_TEXT_FIB_MCQ.md` | Text item on canvas, palette when Text is focused |
| `Tawala_Key_Documents/DESIGNER_FORM_ITEMS_HEADING.md` | Heading item — palette greyed when Heading focused |
| `Tawala_Key_Documents/DESIGNER_DOCUMENT_EDITOR.md` | Document format toolbar — shared palette behavior |

**Optional extras:** `docs/CURSOR_CHAT_GUIDE.md` (Cursor UI tips), `docs/NEXT_CHAT_PROMPT.md` (older shorter prompt — superseded by this file for fresh chats).

---

## Part 2 — Three parallel tracks (do not mix)

The repo has **three separate work tracks**. The agent must **not** merge them in one session unless you explicitly ask.

| Track | Status | What it is |
|-------|--------|------------|
| **1. Browser Designer** (`designer-web/`) | **ACTIVE NOW** | Reimplement legacy C# Designer in the browser. Source of truth: `Tawala_Key_Documents/DESIGNER_*.md` + `TawalaDesigner/` C# code. Run at http://localhost:5173. |
| **2. 8080 runtime parity** (Tomcat, Docker, `registrationFibToXml.mjs`, project CSS) | **PARKED** unless you reopen | End-user forms on Tomcat must match expected layout (e.g. DirtBowl Registration). DirtBowl **page 1** is substantially complete. Paused: page 2+, Review headers, RegStep2, full theme parity. |
| **3. Website / library mock & test-drive** (`website-mock/`) | **PARKED** (Phase 3) | Rough draft of tawala.com / Library / MyTawala. Resume when mock needs polish or test-drive URLs change. |

**Rule of thumb:** If you are building Designer UI → track 1 only. If you say “fix Registration on 8080” → track 2. If you say “polish the website mock” → track 3.

Full rule file: `.cursor/rules/tawala-work-scopes.mdc`

---

## Part 3 — Overall plan & current phase

### What already happened

- **DirtBowl** was a **stress test** for Preview/Deploy — it surfaced structural Designer gaps invisible in smaller templates (Simple Survey, Sign-up Sheet, Get Together).
- **Registration page 1 parity** phase is **closed for now** (commits around `c7a23f7`, later work through `14de400`).
- **Architecture backlog** documented in `docs/DESIGNER_BACKLOG_ARCHITECTURE.md`: MDI multi-window, Fields panel, Explorer collapse, Properties vs popups, context-sensitive menu bars.

### Recent milestones (on `origin/main`)

| Commit area | What landed |
|-------------|-------------|
| `e88d3ba` | MDI Pass 1 — multiple form/process/document windows |
| `74f9fba` | MDI / Fields owner decisions recorded |
| `14de400` | Items dock + Processes palette swap; Heading canvas WYSIWYG |

### Current focus

**Form items on canvas (WYSIWYG) + shared Formatting Palette**

| Item | Status |
|------|--------|
| Items palette docked between Explorer and canvas | **Done** (`14de400`) |
| Processes palette when Process window active | **Done** |
| Heading canvas WYSIWYG (per-selection Main/Sub, badge label edit) | **Done** — owner verified OK |
| **Formatting Palette shell** (row 2 above canvas) | **NEXT** |
| Text canvas row (`TextCanvasRow`) | **Blocked** until Formatting Palette exists |
| Documents hookup to shared palette | After palette shell |
| Properties panel | **Stays** until all items migrate to popups / canvas-inline editing |

### What comes after Formatting Palette

1. Text canvas row per `DESIGNER_FORM_ITEMS_TEXT_FIB_MCQ.md`
2. FIB / MCQ canvas rows (later)
3. Per-item Properties popups — incremental; permanent Properties panel remains for non-Heading items for now

---

## Part 4 — Items palette & Formatting palette (detail)

### Items palette — DONE (`14de400`)

- **Docked** between Project Explorer and the canvas — **not** inside individual Form windows.
- **Form window active** → **Items** palette (Heading, Text, FIB, MCQ, etc.) — always enabled for forms.
- **Process window active** → **Processes / Statements** palette (If, Show, Send, Append, Get, ForEach, Delete, Set, Comment, etc.).
- **Document window active** → **no** middle column (Documents use the format toolbar path, not form insert).
- **Selection sync** when MDI windows close or minimize — docked palettes target the correct active window.

### Formatting Palette — NEXT (not built yet)

Per `DESIGNER_FORM_FORMAT_TOOLBAR.md`:

- **Row 2 toolbar** above the canvas — **14 controls** (Font Face through table tools + **fx**).
- **Greyed** when a **Heading** item has focus (entire toolbar disabled).
- **Live** when a **Text** item has focus.
- Table tools (#12–13) enabled **only** when the cursor is inside a table.
- **Shared** by Form Text items **and** Documents (same core controls as Document toolbar).
- **Text item WYSIWYG on canvas is blocked** until this palette shell exists — do not start Text canvas row first.

**Owner reference screenshots** (in Cursor project assets):

| Screenshot | Path |
|------------|------|
| Text selected on Form canvas | `.cursor/projects/Users-DougC1-Projects-AI-Tawala/assets/Text_Item_in_Forms-e712e814-aeeb-4eae-8601-29da8826b108.png` |
| Formatting Palette detail (Document) | `.cursor/projects/Users-DougC1-Projects-AI-Tawala/assets/Text_Tools_Palette-3a4f7923-cab1-46fb-b320-3555eaacc3be.png` |
| Earlier captures | `assets/Text_Tools_Palette-*.png`, `assets/Formatting_-_*.png` |

---

## Part 5 — Git state

- **Latest commit:** `14de400` on `origin/main` — verify with:
  ```bash
  cd ~/Projects/AI-Tawala && git log -1 --oneline
  ```
- **Do not commit or push** unless you explicitly ask the agent to. Agents are instructed to wait for your request.
- **Accidental `package-lock.json` at repo root** — safe to ignore or delete; it is not part of the Designer app (`designer-web/` has its own `package-lock.json`).

---

## Part 6 — How to run locally

**Browser Designer (normal work):**

```bash
cd ~/Projects/AI-Tawala/designer-web && npm run dev
```

| URL | Role |
|-----|------|
| http://localhost:5173 | Designer UI (Vite) |
| http://localhost:3001 | Designer dev API (deploy, form runtime) |

**8080 Tomcat** — only if you reopen the **runtime parity** track:

```bash
# Docker or local Tomcat — see docker/ and designer-web/README.md
# Example: TAWALA_JAVA_URL=http://localhost:8080 npm run dev
```

---

## Part 7 — EXACT Cursor steps for owner (macOS)

### Before you start

1. **Save** any open files in your editor (`Cmd+S`).
2. **Optional:** Finish or close the current chat — you do not have to, but a **new** chat keeps context smaller and cheaper.

### Start a new chat

3. **Open Cursor** with the `AI-Tawala` folder as the workspace (`File → Open Folder…` → `~/Projects/AI-Tawala`).

4. **Open the chat panel:**
   - Press **`Cmd+L`** (common shortcut), **or**
   - Click the **chat / agent icon** in the left sidebar, **or**
   - **Command Palette:** `Cmd+Shift+P` → type **“New Chat”** or **“Focus Chat”**.

5. **Start a fresh conversation** (no prior turns in this thread):
   - Click **“New Chat”** (plus icon or label at top of chat panel), **or**
   - **`Cmd+N`** / **`Cmd+R`** in some layouts, **or**
   - Glass / Agents layout: **File → New Agent**.

6. **Optional — attach context** (helps the agent find files faster):
   - Type **`@`** in the message box and pick files, **or**
   - Use **“Add context”** / paperclip if your Cursor version shows it.
   - Suggested attachments for the current phase:
     - `docs/START_NEW_CHAT.md`
     - `docs/CHAT_HANDOFF.md`
     - `Tawala_Key_Documents/DESIGNER_FORM_FORMAT_TOOLBAR.md`
   - You do **not** need to attach everything — the Start Script tells the agent what to read.

7. **Paste the Start Script** (Part 8 below) as your **first message** in the new chat. Send it.

8. **Tell the agent what you want** (already in the script, but you can add):
   - Read the docs **first**.
   - Summarize the plan back to you.
   - **Ask before** large or risky changes.
   - **No commit or push** unless you ask.

9. **Rename the chat** (recommended): click the chat title at the top → e.g. `Designer — Formatting Palette`.

### If context gets high again

When the context ring fills or answers get slow:

1. Ask the agent to **update** `docs/CHAT_HANDOFF.md` with a session checkpoint.
2. **Optional:** ask for a **checkpoint commit** (only if you want one).
3. Start another **new chat** using this file again.

More Cursor UI detail: `docs/CURSOR_CHAT_GUIDE.md`

### What NOT to do

- Do **not** paste huge logs or entire spec folders into chat — point the agent at file paths.
- Do **not** mix tracks (e.g. “fix 8080 CSS” + “build Formatting Palette”) in one session unless you intend to.
- Do **not** assume the agent will commit — say **“please commit”** explicitly when you want that.
- Do **not** use www.tawala.com for Preview/Deploy — use localhost (5173 / 3001 / 8080).

---

## Part 8 — Copy-paste START SCRIPT

Copy **everything** inside the box below (from `Project:` through the last line) and paste it as the **first message** in your new chat.

```text
Project: ~/Projects/AI-Tawala
Current commit: 14de400 (verify with git log -1)

Read first (in order):
1. docs/START_NEW_CHAT.md
2. docs/CHAT_HANDOFF.md
3. docs/ROADMAP.md
4. docs/DESIGNER_BACKLOG_ARCHITECTURE.md
5. Tawala_Key_Documents/DESIGNER_FORM_FORMAT_TOOLBAR.md
6. Tawala_Key_Documents/DESIGNER_FORM_ITEMS_TEXT_FIB_MCQ.md (Text section)
7. Tawala_Key_Documents/DESIGNER_FORM_ITEMS_HEADING.md (palette greyed rules)

Active track: Browser Designer (designer-web/) — ONLY this track unless I say otherwise.

Parked tracks (do not work on unless I reopen):
- 8080 runtime parity (Tomcat/Docker, registrationFibToXml.mjs, project CSS) — DirtBowl Registration page 1 substantially complete
- Website / library mock (website-mock/) — Phase 3

Current phase: Form items WYSIWYG on canvas + shared Formatting Palette.
Done: MDI Pass 1 (e88d3ba), Items/Processes dock swap (14de400), Heading canvas WYSIWYG verified OK.
NEXT: Formatting Palette shell (row 2 above canvas, 14 controls per DESIGNER_FORM_FORMAT_TOOLBAR.md) — greyed for Heading, live for Text; table tools only in table; shared with Documents later.
Text canvas row is BLOCKED until Formatting Palette exists.
Properties panel stays until items migrate to popups/canvas-inline.

Constraints:
- Do NOT commit or push unless I explicitly ask.
- Three-bucket discipline: do not merge Designer code, 8080 parity, and website mock in one change.
- Read docs and summarize plan before large changes.
- Ask me before architectural surprises.

Task: Continue from CHAT_HANDOFF — begin Formatting Palette shell per DESIGNER_FORM_FORMAT_TOOLBAR.md (or I will give a different task in a follow-up message).
```

---

## Part 9 — Suggested first command to the new agent

After the Start Script, you can send this as a **second message** (or combine with the script):

```text
Read START_NEW_CHAT.md and CHAT_HANDOFF.md, confirm current phase, then begin Formatting Palette shell per DESIGNER_FORM_FORMAT_TOOLBAR.md.
```

The agent should:

1. Read the listed docs.
2. Confirm: active track = browser Designer; next = Formatting Palette shell.
3. Propose a small first slice (UI shell + enable/disable rules) before coding.
4. Wait for your OK before committing anything.

---

## Quick reference

| Question | Answer |
|----------|--------|
| Where is the app? | `designer-web/` |
| How do I run it? | `cd designer-web && npm run dev` → http://localhost:5173 |
| What are we building now? | Formatting Palette shell, then Text canvas row |
| What is parked? | 8080 parity (except if you reopen), website mock |
| Can the agent commit? | Only when you explicitly ask |
| Session memory lives in | `docs/CHAT_HANDOFF.md`, `docs/ROADMAP.md` |

*Last updated: July 2026 — after commit 14de400 (Items dock, Heading WYSIWYG).*
