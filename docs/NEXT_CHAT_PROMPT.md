# Next Designer chat — copy-paste prompt

**Purpose:** Paste the **LONG** block below into a **fresh Designer chat** after holiday (or whenever context is high). The repo holds durable memory in [`ROADMAP.md`](ROADMAP.md), [`COMPARING_RUNTIMES.md`](COMPARING_RUNTIMES.md), and [`DESIGNER_BACKLOG_ARCHITECTURE.md`](DESIGNER_BACKLOG_ARCHITECTURE.md); this prompt bootstraps the agent with phase, constraints, and next steps.

**Read first:** [`docs/ROADMAP.md`](ROADMAP.md), [`docs/COMPARING_RUNTIMES.md`](COMPARING_RUNTIMES.md), [`docs/DESIGNER_BACKLOG_ARCHITECTURE.md`](DESIGNER_BACKLOG_ARCHITECTURE.md). **Do not commit or push unless the owner explicitly asks.**

---

## LONG — paste into a new Designer chat

```text
Project: ~/Projects/AI-Tawala
Track: Browser Designer — architecture backlog (post parity phase)
Current phase: Designer architecture backlog surfaced by DirtBowl stress test; Registration page 1 parity closed pending Q4 verify
Read first: docs/DESIGNER_BACKLOG_ARCHITECTURE.md, docs/ROADMAP.md Phase 4, docs/COMPARING_RUNTIMES.md
Constraints: do not commit or push unless I explicitly ask

Context:
- Simple Survey, Sign-up Sheet, and Get Together passed Preview + Deploy without template-specific errors.
- DirtBowl Registration page 1 Preview/Deploy parity is substantially complete / closed for now.
- Pending my visual verification: final Q4 email-note field-column alignment in docker/tomcat/css/project/dirtbowl2/project.css (uncommitted).
- DirtBowl served as a stress test, not an authoring benchmark — it surfaced structural Designer gaps invisible in smaller templates.

Designer architecture backlog (five items — see docs/DESIGNER_BACKLOG_ARCHITECTURE.md):
1. Multi-window / MDI architecture
2. Forms ↔ Processes connection transparency
3. Collapsible Explorer menus (Fields; Processes under Forms)
4. Properties panel vs legacy popup dialogs
5. Multiple context-sensitive menu bars

Cross-refs: Tawala_Key_Documents/DESIGNER_MENU_SPEC.md, DESIGNER_STARTUP_AND_FORM_CANVAS.md, DESIGNER_UI_REFERENCE.md, DESIGNER_DOCUMENT_EDITOR.md

Also recorded elsewhere (not duplicated in architecture backlog):
- insertion-point arrow and Move Up / Down — prerequisites for serious Process work (ROADMAP Phase 4)
- FIB/layout parity and hint-text styling — docs/DESIGNER_BACKLOG_FORMS_FIBS.md

Next task:
Please continue from the current phase:
1. Read docs/DESIGNER_BACKLOG_ARCHITECTURE.md and docs/ROADMAP.md Phase 4
2. Summarize the architecture backlog and recommend the highest-value first implementation slice
3. Do not commit project.css or doc updates until I verify the Q4 gap fix on :8080 vs :5173
4. Do not make code changes until you summarize the current state back to me
```

---

## SHORT — for later sessions

After the first restart, the short version is usually enough because repo docs hold most of the memory.

```text
Continue AI-Tawala browser Designer work from main.

Read first:
- docs/DESIGNER_BACKLOG_ARCHITECTURE.md
- docs/ROADMAP.md Phase 4
- docs/COMPARING_RUNTIMES.md

Current state:
- DirtBowl Registration page 1 Preview/Deploy parity substantially complete (pending Q4 email-note CSS verify)
- DirtBowl stress test → five-item Designer architecture backlog
- Simple Survey, Sign-up Sheet, Get Together passed earlier
- next focus: Designer architecture (MDI, explorer, properties UX), not Registration CSS parity

Do not commit or push unless I explicitly ask.
Before changing anything, summarize the current state and propose the next architecture slice.
```
