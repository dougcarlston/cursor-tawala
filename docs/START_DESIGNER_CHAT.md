# Start the Designer Cursor chat — owner guide

**Purpose:** Switch from the Library / website track to the **Browser Designer** track with a clean chat.

**One command:**

```bash
cd ~/Projects/Tawala
./scripts/start-designer-chat.sh --start --copy
```

That checks (and starts) Designer UI `:5173`, Designer API `:3001`, and Tomcat `:8080` for Push, copies the chat opener to your clipboard, and prints review URLs.

For a full day of Designer work, also leave a Terminal open with:

```bash
cd ~/Projects/Tawala/designer-web
npm run keep
```

(`npm run keep` restarts Vite and the API if one of them dies. The start script can background Vite, but a Terminal you leave open is more reliable.)

---

## Part 1 — What the Designer chat should read first

| Order | File | Purpose |
|-------|------|---------|
| 1 | `docs/START_DESIGNER_CHAT.md` | This guide |
| 2 | `Tawala_Key_Documents/DESIGNER_OPEN_BUGS.md` | First task: two links on one line merge underlines |
| 3 | `Tawala_Key_Documents/DESIGNER_DOCUMENT_EDITOR.md` + `DESIGNER_INSERT_MENU_AND_FUNCTIONS.md` | Document canvas + Link… smoke |
| 4 | `.cursor/rules/tawala-work-scopes.mdc` | Design canvas ≫ Deploy/Push ≫ Preview — do not mix Library |
| 5 | `.cursor/rules/tawala-designer-parked-post-website.mdc` | Parked polish; Link / Push rename / Theme-on-Push already done |

**Do not** open `website-mock/` Library cosmetics or “going live” work in this chat.

---

## Part 2 — Services (Designer thread needs these)

| URL | Role | Start |
|-----|------|--------|
| http://localhost:5173 | Designer UI | `cd designer-web && npm run keep` |
| http://localhost:3001 | Dev API — Push, Preview, deploy | `designer-web/scripts/ensure-dev-api.sh` |
| http://localhost:8080 | Tomcat — live form after Push | `./scripts/start-designer-chat.sh --start` |

Vite on `:5173` often stays up while `:3001` dies. If Push or Preview says “failed to fetch”, run `ensure-dev-api.sh`.

---

## Part 3 — Cursor steps

1. **Save** open files.
2. **New Chat** (`Cmd+L` → New Chat / `+`).
3. **Rename** the chat → `Designer thread`.
4. Run `./scripts/start-designer-chat.sh --copy` (or `--start --copy`).
5. **Paste** the START SCRIPT as your first message.
6. Optional `@` attach: `docs/START_DESIGNER_CHAT.md`, `Tawala_Key_Documents/DESIGNER_OPEN_BUGS.md`.

---

## Part 4 — First task (Aug 26)

**Document — two links on one line merge underlines** (Not blocking for Live Library).

- Open **Shared To-Do** → **Document - User Menu**.
- Two Insert → **Link…** (Form in project) on the same line, with `or` between them: `Sign up for a task` **or** `Mark a task complete`.
- **Bug:** one underline runs under both links and the word `or`. Seen in Design and after Push.
- **Wanted:** each link underlined on its own; `or` is plain text. Also check whether the two links share one click target.

Shots: `Tawala_Key_Documents/assets/Bug_-_Document-two-links-merged-underline-Design-Aug26.png` and `…-Push-Aug26.png`.

Then, if there is time: parked **Document smoothness P0s** (Face/Size multi-chip, invent-with-tables, chip drag, table reflow). Do not “fix” the Font Color picker unless asked.

---

## Part 5 — When you come back to Library (not this chat)

Owner notes from Aug 26, parked until after Designer:

- Public **Library listing** look-and-feel is not as good as **My Tawala** and **Project Details** — cosmetics before any public switch.
- Next product talk: **go live but private**, then fix gates before strangers (Test Drive shared live id, accounts at save-data, payments, email metering, end page, ratings).
- Library start: `./scripts/start-library-chat.sh --start --copy` — `docs/START_LIBRARY_CHAT.md`.

---

*Last updated: Aug 26, 2026 — handoff from Library session (Publish keeps My Tawala; Designer chat tomorrow).*
