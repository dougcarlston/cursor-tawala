# Designer session — 2026-07-14

## Done today
- Owner smoke of morning checklist #1–8 and full Document retest on **5173 only** (8080 deferred). Chrome + Safari.
- Committed earlier: `29b3a23` Document/table canvas + formatting; `312a68c` align / resize reflow / continue-after-break.
- **#4 table-drag snapback** and **#5 window-resize restore** fixed via `data-doc-home-top` packing — **owner smoked PASS**.
- Safari Save As investigated twice:
  - In-DOM Blob `<a download>` — still failed on Safari.
  - Form POST `/api/download-project` with `Content-Disposition: attachment` — still unsatisfactory to owner: saves an **Untitled** project into **Downloads** (no real Save As folder/name picker). Treat as **still open**.
- Safari quirks logged (not fixed / low priority): field/chip selection highlight bleeds left; no native Save As picker (WebKit/FSAA gap); Download prompts / `.json`→`.html` nudges.
- Dev server note: agent-shell SIGTERM killed Vite/API; use launchd-detached start so 5173 survives.

## Pass (retest)
Selection, Save/dirty (Chrome), Face/Size/B/I, Color, Tables (#8–9, #11), Align (#12), Continue-after-break (#14), bonuses (#15–16), table snapback (#4 fix), resize restore (#5 fix).

## Still open — tomorrow
1. **Safari / Save As (important):** Current behavior dumps Untitled `*.json` to Downloads. Need a better UX (sensible default filename from project name; clear Save vs Save As; document Safari limits; ideally name prompt before download). Owner stopped for the day with this failing their bar.
2. **Safari chip highlight → left margin** (cosmetic; low priority).
3. **New Project → “Could not parse JSON.”** (deferred; not in today's fix batch).
4. **Enhancement:** Color picker Recent row (4–6) for applying one color to separated words.
5. After Save As is acceptable: commit any leftover Save As work if not in this commit; optional full re-smoke; 8080 deploy still deferred unless owner asks.
6. Push only when owner asks (`cursor/forms-canvas-wysiwyg` may be ahead of origin).

## Do not
- Do not treat Safari Save As as done.
- Prefer Chrome for Save As picker until Safari path is acceptable.
