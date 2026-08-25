# Catchup memo — return ~Thursday Aug 20, 2026

**Left:** Monday evening Aug 10  
**Branch:** `cursor/forms-canvas-wysiwyg`  
**Repo:** `~/Projects/Tawala`  
**Start Designer:** `cd designer-web && npm run dev` → `:5173` + API `:3001`; Tomcat `:8080`; website mock `:5500`

This is the plain-English briefing for the first session back. Detail lives in the specs named below; you do not need to re-read the whole chat.

---

## What “today” (Aug 10) already shipped

### Morning — Website / Library (already committed earlier Aug 10)

My Tawala + Library mock is in good shape: Use / Test Drive / Save to MyTawala / Deploy-as-share / Publish vocabulary; guest mode; listing metrics; lean My Tawala + rich Project Details. Brief that handed Designer back: `docs/DESIGNER_RESUME_FROM_WEBSITE_AUG10.md`.

### Afternoon — Browser Designer (this commit)

| Done | In plain English |
|------|------------------|
| **Deploy → Push** | In Designer, the old **Deploy…** wording is now **Push…** / **Push to My Tawala**. Website Project Details still says **Deploy** for sharing links/embeds with other people — that is intentional. |
| **Theme on Push** | When you Push (or Push this version), the project’s theme is supposed to land on the live `:8080` form, not only in the My Tawala dropdown. |
| **Skip → If Fields** | You can expand Form folders in the Fields palette while editing Skip/If, and drag or double-click fields into the If targets. Cancel on the Skip dialog no longer keeps half-edited changes. |
| **Configure Function scroll** | Long field lists (e.g. Multiple Question List) keep the typing area in view as you add rows. |
| **One Link dialog** | Invitation + Hyperlink are one **Link** insert dialog (project form first; URL second). |
| **MAX / MIN** | New Document functions, wired in Designer + Java source. |
| **Remove Duplicates** | New Process statement (keep first/last by a key field). |
| **COUNT** | **Not** a new function. It was the same idea as existing **FORM RECORD COUNT**. We added COUNT briefly, then removed it after you caught the old engineer note was wrong. Use FORM RECORD COUNT. |
| **SUM** | Already existed; left alone. |
| **Get Together totals** | Correlation / totals CSS tweak so columns line up better on default theme. |
| **Form Item conditional display** | Right-click the form-item badge → **Display conditionally…** (Where rows). When set, badges show `{Qn}` braces. Push exports the conditions. Spec + screenshots: `DESIGNER_FORM_ITEMS_CONDITIONAL_DISPLAY.md`. This unblocked ~7 Library projects that had been sequestered for “convert errors.” |
| **Page Header image tool** | **Done Aug 20** — pan/zoom/stretch (no aspect lock); bake at source crop resolution. |

---

## First things when you sit down again

Do these in order; each is short.

1. **Hard-refresh** Designer (`:5173`), API health (`:3001/api/health`), Tomcat (`:8080`), website mock (`:5500`) if you use Library flow.
2. **Rebuild / redeploy the Java WAR** — **Done Aug 20** (RemoveDuplicates ConfigElement fix + Tomcat recreate).
3. **Smoke MAX / MIN** — **Owner Passed Aug 20.** (Leftover blank value on `:8080` only — tracked separately, not a MAX/MIN failure.)
4. **Smoke Remove Duplicates** — **Owner Passed Aug 20.**
5. **Spot-check sequestered → released projects** that used item-level display conditions (Campaign Dashboards was one you looked at Aug 10). Open → Push → confirm braces/editor still feel right.
6. **Done Aug 21:** New Project / distinct uniqueIds — first Push mints non-colliding Tomcat name (`deployIdentity.mjs`). Smoke: File→New → Push → empty blanks + new uniqueId.
7. **Optional:** Library `.tawala` reconvert quality pass (`DESIGNER_OPEN_TODOS` owner queue #16).

---

## Backlog that can wait until after the smokes

- **Lost-stash bugs (owner Aug 11 — closed Aug 20–21):** Write-up in `DESIGNER_OPEN_BUGS.md` § “Parked Jul 30 / reconfirmed Aug 11”. All fixed: FIB Styles Align-right; Qn badge column; Sign-up right-justify bottoms; Online Exam TinyMCE; Form Text blank-line Deploy + **image drag-select (Aug 21)**.
- **Page Header image editor** — reinstall mothballed tool; allow horizontal stretch (`DESIGNER_PAGE_HEADER.md`, TODO #17).
- Remaining template Deploy/Push smokes in `DESIGNER_TEMPLATE_MATRIX.md` — **Done Aug 20** (#4 Form+Process+Document Passed; Phase 2 complete).
- Polish: form Cut/Copy/Paste; Get RecordList Fields branch; column-level `displayCondition` editor (item-level is done) — see **Deferred / next version** in `docs/DIRTBOWL_PAGE1_DESIGNER_EXEMPLAR.md`.
- Three-browser look-and-feel / menu audit — still gated until Designer is “basically finished.”
- **DirtBowl Page 1 → Designer exemplar** — **Triaged Aug 20:** `docs/DIRTBOWL_PAGE1_DESIGNER_EXEMPLAR.md` (V1 vs next vs not immediate). Page 2+ still paused as further exemplar only after that list is stable.
- Registration / DirtBowl Page 2+ on `:8080` — still paused unless you reopen that track.

---

## How to think about the three tracks now

You can fold **docs + browser Designer** into one product lane when you return. Keep **Registration / Tomcat `:8080` layout parity** separate until you reopen it.

**Design canvas ≫ Preview.** Fix Push/runtime in server/export paths; do not rewrite Form row idle/edit UX just to make Preview match.

---

## Useful files

| File | Why |
|------|-----|
| `docs/CATCHUP_MEMO_RETURN_AUG20.md` | This memo |
| `docs/DESIGNER_RESUME_FROM_WEBSITE_AUG10.md` | Morning Website → Designer handoff |
| `Tawala_Key_Documents/DESIGNER_OPEN_BUGS.md` | Lost-stash bugs (FIB Styles, badges, Sign-up align, Online Exam TinyMCE) |
| `Tawala_Key_Documents/DESIGNER_OPEN_TODOS.md` | Live backlog (#17 Page Header, #16 reconvert, etc.) |
| `Tawala_Key_Documents/DESIGNER_FORM_ITEMS_CONDITIONAL_DISPLAY.md` | Conditional display contract |
| `Tawala_Key_Documents/DESIGNER_INSERT_MENU_AND_FUNCTIONS.md` | MAX/MIN / FORM RECORD COUNT / function matrix |
| `Tawala_Key_Documents/DESIGNER_PAGE_HEADER.md` | Banner tool reopen notes |
| `docs/DIRTBOWL_PAGE1_DESIGNER_EXEMPLAR.md` | Page 1 → Designer feature triage (Aug 20) |

Welcome back — start with the WAR rebuild + MAX/MIN/Remove Duplicates smoke, then pick lost-stash bugs, Page Header, or Library reconvert.
