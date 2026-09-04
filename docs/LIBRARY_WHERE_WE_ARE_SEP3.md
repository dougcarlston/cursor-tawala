# Where we are / where we’re going — Sep 3, 2026 (pre-travel)

**Owner travel:** Saturday → ~1 week away (laptops along; Wi‑Fi uncertain).  
**Tomorrow (Fri):** thorough backup + confirm next steps before leaving.  
**Branch:** `cursor/forms-canvas-wysiwyg` (this handoff assumes that tip is committed).  
**Staging trays (not in git):** `~/Projects/ Current Tawala Projects/Website Staging/`

Use this file after Context compaction or a new chat. Companion: `docs/STARTUP_APPS_STAGING_SEP2.md`, Samples README in the staging tray.

---

## Product policy (locked Sep 3)

| Term | Meaning |
|------|---------|
| **Sample** | Looping Library demo (try it; responses accumulate in the shared Test Drive). Never call Library apps **Simple**. |
| **Real app** | Customize / Setup / Admin products (Sign-up, Potluck, Get Together, Shared To-Do, List Builder, Single Question Poll). Drop **Sophisticated** on publish. |
| **Builder** | Only apps that *construct* a bigger instrument — **Online Exam Builder**, deferred Poll/Survey v.8. **Not** Potluck / Get Together / Sign-up just because they have Customize. |
| **Sign-up Sheet w Email** | New Project / Designer only — never Library. |
| **Wildcat Week** | My Tawala only — not Library. |

**Library Test Drive vs starts (agreed Sep 3 — partially applied):**

- Keep **multiple start points** on real apps (guest + admin URLs after Copy).
- **Library Test Drive** always opens the setup door (Customize / Setup / Administration / AdminStart).
- **My Tawala Use / Project Details** — leave as-is (multi-start → Details to pick).
- **Publishers later:** alias list now; Form Properties “Library Test Drive start” mark later.

**Note:** Five of six `2-Ready` apps still have only one `startPoint` from an earlier pass. **Sophisticated Sign-up Sheet** already has Customize + Questionnaire + Administration again (owner editing). **Next code step:** restore multi-start on the other five + expand `pickLibraryTestDriveStartPoint` aliases (not Exam-only).

---

## Where we are

### Samples tray (`3-Ready-for-Public-Library/Samples/`)

| Status | File / name |
|--------|-------------|
| **Ready** | `Survey Sample.json` (renamed off Simple; MCQ placeholder junk stripped; Submit wording; Report loop) |
| **Ready** | `Multiple Question Survey Sample.json` (Yes→Report / No→Survey; Report→Survey; Group Results function chips restored after copy-paste hollowed them) |
| **In progress** | Sign-up Sheet Sample — start from New Project → Activities → **Sign-up Sheet** (looping). Not the `2-Ready` Customize app. |
| **Still to do** | Horses and Penguins (keep if OK); Potluck Sample / Get Together Sample only if looped (+ GT theme); else drop |

### Real apps (`2-Ready-for-MyTawala/`)

| File | Notes |
|------|--------|
| Sophisticated Sign-up Sheet | Owner editing; multi-start restored |
| Sophisticated Get Together | Customize-only start until multi restored |
| Sophisticated Potluck | Setup-only; kids/adults in one app |
| Sophisticated Shared To-Do | Setup-only; internal name Shared To-Do |
| Sophisticated Automated List Builder | Setup-only; remote Confirm/Opt-out needs public host |
| Sophisticated Single Question Poll or Survey | AdminStart-only |

**Do not Push / Publish** the piles until Samples + multi-start / Test Drive preference are settled and cross-checked.

### Designer (in git)

| Item | Status |
|------|--------|
| Document two-link underlines (Design + Push) | Done earlier (`12d8c94`) |
| Form Text Instructional blank gaps + typing into spacer “font change” | **Fixed** — `preserveFormTextFlowParagraphs`; Design CSS margins; tests |
| **C14** FIB Above auto-split + required `*` on label | Parked — next Designer batch |
| **C15** Pre-populate With Last Entry + Block Back Button | Logged — never smoked; next Designer batch |
| **F1** 3-browser smoke | Re-surfaced — after live, before public |

### Deferred / elsewhere

- `Defer-rewrite/`: Poll or Survey v.8 pair; Emailer With Signup  
- `Builders-hold/`: Online Exam Builder (already on Library; Test Drive → Administration)  
- Website: home step-through / movie for Sample vs Copy to MyTawala (Samples should stay short)

---

## Where we’re going (ordered)

### Friday (before travel) — backup + clarity

1. Backup: git commit/push this branch; copy/zip **Website Staging** trays (Samples + `2-Ready` especially); note local Designer/Tomcat if needed.  
2. Skim this file + Samples README — agree Friday stop line.  
3. Optional if energy: finish Sign-up Sheet Sample into `Samples/`; restore multi-start on remaining `2-Ready` apps (no Push yet).

### After return / with Wi‑Fi (Library pass)

1. Finish remaining Samples (or drop Potluck/GT Sample if not loopable).  
2. Restore multi-start on `2-Ready` + wire Library Test Drive preference aliases.  
3. Cross-check Doug ↔ agent piles.  
4. Push under **public** names → Publish / replace Live Library (retire thin “Simple” rows; no Simple in titles).  
5. Optional: Sample instructional + home tour for Copy to MyTawala.

### Later Designer batch (not Library)

- **C14**, **C15** smoke, then gated **F1** when appropriate.

### Explicitly not this week

- Poll v.8 rewrite; Emailer; C14 while Samples unfinished; unifying Design canvas with Preview for Deploy.

---

## Open gotchas

- **Copy-paste of function chips** between projects can strip `data-function-*` and leave pretty dead text (`color(srgb…)` chrome). Re-Insert Function or copy whole Text item. Hit MQS Sample Report T2.  
- **Stale `deployIdentityName` / `deployUniqueId`** blocks rename-Push — clear stamps or mint a private name.  
- Staging tray paths live **outside** the Tawala git repo — backup Staging separately from `git push`.  
- Designer API `:3001` — restart after server edits (`ensure-dev-api.sh` won’t reload an already-up process).

---

## Suggested new-chat opener (after compaction / return)

```text
Library / Samples pass — resume from docs/LIBRARY_WHERE_WE_ARE_SEP3.md
Branch cursor/forms-canvas-wysiwyg. Staging: Website Staging Samples + 2-Ready.
Do not Push until multi-start restored and Samples cross-checked. No Designer C14 this pass.
```
