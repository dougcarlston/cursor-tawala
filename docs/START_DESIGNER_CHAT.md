# Start the Designer Cursor chat — owner guide

**Purpose:** Open a **Browser Designer** chat. After 11 Nov 2026, read the pivot first.

**Read first:** [`docs/HANDOFF_NOV11_2026.md`](HANDOFF_NOV11_2026.md)

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

(`npm run keep` restarts Vite and the API if one of them dies.)

---

## Part 1 — What the Designer chat should read first

| Order | File | Purpose |
|-------|------|---------|
| 1 | `docs/HANDOFF_NOV11_2026.md` | Product pivot + uncommitted files |
| 2 | `docs/START_DESIGNER_CHAT.md` | This guide |
| 3 | `Tawala_Key_Documents/DESIGNER_OPEN_TODOS.md` | B1 Document smoothness; F1 3-browser |
| 4 | `.cursor/rules/tawala-work-scopes.mdc` | Design canvas ≫ Push ≫ Preview |
| 5 | `.cursor/rules/tawala-designer-parked-post-website.mdc` | Parked polish; Link/Push/Theme already done |

Do **not** mix website-mock Library cosmetics or payment/auth vendor work into a Designer change.

---

## Part 2 — Services

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
5. **Paste** the START SCRIPT (or the opener in `HANDOFF_NOV11_2026.md`).
6. Optional `@` attach: `docs/HANDOFF_NOV11_2026.md`.

---

## Part 4 — First Designer work (after Nov 11)

The Aug 26 two-link underline bug is **fixed** (2 Sep 2026). Do not reopen unless it regresses.

Owner picks among:

- Document smoothness P0s (**B1**) — Face/Size multi-chip, invent-with-tables, chip drag, table reflow
- **F1** 3-browser smoke (when Designer feels finished enough)
- Native `confirm()` before public use

---

## Part 5 — Public site (not this Designer change)

My Tawala + a host that stays up; accounts so projects persist. Payments parked. Teacher-exam packets parked. Details: `docs/HANDOFF_NOV11_2026.md`.

---

*Last updated: 11 November 2026.*
