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
| **Page Header image tool** | **Not rebuilt yet** — only put back on the TODO list (#17). You asked to reinstall the mothballed pan/crop/resize tool and allow sideways stretch (drop non-distort). |

---

## First things when you sit down again

Do these in order; each is short.

1. **Hard-refresh** Designer (`:5173`), API health (`:3001/api/health`), Tomcat (`:8080`), website mock (`:5500`) if you use Library flow.
2. **Rebuild / redeploy the Java WAR** — **Done Aug 20** (RemoveDuplicates ConfigElement fix + Tomcat recreate).
3. **Smoke MAX / MIN** — **Owner Passed Aug 20.** (Leftover blank value on `:8080` only — tracked separately, not a MAX/MIN failure.)
4. **Smoke Remove Duplicates** — **Owner Passed Aug 20.**
5. **Spot-check sequestered → released projects** that used item-level display conditions (Campaign Dashboards was one you looked at Aug 10). Open → Push → confirm braces/editor still feel right.
6. **OPEN:** New Project / distinct uniqueIds must never inherit old submissions — `DESIGNER_OPEN_BUGS.md` § Aug 20.
7. **Optional:** Library `.tawala` reconvert quality pass (`DESIGNER_OPEN_TODOS` owner queue #16).

---

## Backlog that can wait until after the smokes

- **Lost-stash bugs (owner Aug 11 — documented, not fixed):** Full write-up + screenshots in `DESIGNER_OPEN_BUGS.md` § “Parked Jul 30 / reconfirmed Aug 11”. Summary:
  1. FIB Styles **Align right side** control almost invisible (already known Jul 30; reconfirmed).
  2. Form canvas **Qn badges uneven widths** — want a straight right edge; SKIP may stick out.
  3. Sign-up Template **Right justified** — labels bounce mid vs bottom of blanks.
  4. Online Exam Builder **SetupVariables** — multi-line FIB TinyMCE: font size random + bleed between boxes (**Fixed Aug 20** — `custom_content.css` + pt sizes; smoke on `:8080`).
  5. (Still parked from Jul 30) Form Text blank-line spacing on Push + image selection break.
- **Page Header image editor** — reinstall mothballed tool; allow horizontal stretch (`DESIGNER_PAGE_HEADER.md`, TODO #17).
- Remaining template Deploy/Push smokes in `DESIGNER_TEMPLATE_MATRIX.md`.
- Polish: form Cut/Copy/Paste; Get RecordList Fields branch; column-level `displayCondition` editor (item-level is done).
- Three-browser look-and-feel / menu audit — still gated until Designer is “basically finished.”
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

Welcome back — start with the WAR rebuild + MAX/MIN/Remove Duplicates smoke, then pick lost-stash bugs, Page Header, or Library reconvert.
