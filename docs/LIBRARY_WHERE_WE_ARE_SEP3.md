# Where we are / where we’re going — Sep 8, 2026 (Library / Samples resume)

**Branch:** `cursor/forms-canvas-wysiwyg`  
**Staging (NOT in git):** `~/Projects/ Current Tawala Projects/Website Staging/`  
**Backup (Sep 8, before this pass):** `~/Projects/ Current Tawala Projects/Website Staging backups/Website-Staging-2026-09-08.zip`

Companions: this file; tray READMEs under `2-Ready-for-MyTawala/` and `3-Ready-for-Public-Library/Samples/`. UniqueId sidebar: `.cursor` canvases `library-project-ids.canvas.tsx` (open beside chat).

Sep 4 evening close (pre-travel) is preserved below the Sep 8 delta.

---

## Sep 8 — what changed

1. **Backup Staging** — zip written outside git (path above).
2. **Horses and Penguins Test** — copied from Live Library into Samples tray.
3. **Get Together Sample** + **Potluck Sample** — looping demos were built Sep 8; **parked Sep 9** (`_set-aside/`). Owner: those two are not Samples this pass. Full apps restored to multi-start.
4. **Automated List Builder** — private Push identity stamped; public occupant left in place. Option A AfterSave on Setup; Questionnaire + Administration restored as starts. **Owner Push from Designer** (File→Open tray JSON → Push) then TD-smoke Setup.

Did **not** this pass: public Library Push; retire/hatch occupant `ceihmyxlssxn6yn`; Designer C14/C15/F1.

---

## Exact tray state (Sep 8)

### Samples — `3-Ready-for-Public-Library/Samples/`

| File | Status |
|------|--------|
| `Survey Sample.json` | **Owner approved Sep 9** |
| `Multiple Question Survey Sample.json` | **Owner approved Sep 9.** Drop Report as a **start** (keep Report form; loop still reaches it). TD door = Survey. |
| `Sign-up Sheet Sample.json` | **Owner approved Sep 9** |
| `Horses and Penguins Test.json` | **Owner approved Sep 9** (already Live Library) |
| `Get Together Sample.json` | **Parked Sep 9** → `_set-aside/Get Together Sample__parked-2026-09-09.json`. Not a Library Sample this pass. |
| `Potluck Sample.json` | **Parked Sep 9** → `_set-aside/Potluck Sample__parked-2026-09-09.json`. Not a Library Sample this pass. |

### Smoked real apps — `2-Ready-for-MyTawala/Smoked-Ready-to-Push/`

Public names (drop Sophisticated). Library **Test Drive OK** for these four:

| File | TD door | Notes |
|------|---------|--------|
| `Single Question Poll or Survey.json` | **AdminStart + Start Questionnaire** | **Restored Sep 9** from live My Tawala uniqueId `b7ttti35vdo58pu` (Designer snapshot `8c1098d4957a094ca5db6cb9`, 2026-09-02). Dual-use: Library TD and real poll. Administration is **not** a start. Do **not** Push a different JSON onto this uniqueId. |
| `Sign-up Sheet.json` | **Customize** (also Questionnaire + Administration) | Keeper uniqueId `gnqp5qfd06ktec3`. Leftover My Tawala row deleted Sep 9. Dual-use like Poll: Customize is the TD hub. |
| `Shared To-Do Test Drive.json` | **Setup** only | Option A menu **c**. **Smoke passed Sep 9.** No uniqueId on file. Real app: `Sophisticated Shared To-Do.json` (`qggyhqoy8m4td23`). |
| `Automated List Builder.json` | **Setup** | Typo pass + private Push Sep 8. Display name **Automated List Builder**. Live copy is uniqueId `vrjayw9lwun85i1` (Tomcat still parked as Smoke so we do not overwrite public occupant `ceihmyxlssxn6yn`). Email warning = Library listing honesty, not in-form. |

### Still in prep — `2-Ready-for-MyTawala/`

| File | Status |
|------|--------|
| `Sophisticated Get Together.json` | **Multi-start restored Sep 9:** Customize + Questionnaire + Administration. AfterSave hub removed. **No Library TD / no Sample JSON.** My Tawala only for the full app. File→New Simple Get Together ≠ this file. |
| `Sophisticated Shared To-Do.json` | **Owner smoked + Pushed + Save As Sep 9.** Setup + Signup + Administration. uniqueId `qggyhqoy8m4td23`. TD copy smoke passed. |

Also: older junk at `3-Ready` root (CYO, Dirtbowl Communicator, MVSC, etc.) — **not** this startup-app pass.

---

## Product rules locked this week

| Rule | Meaning |
|------|---------|
| Sample vs Real vs Builder | **Sample** = that is all it is (looping Library demo). **Real** = Customize/Setup/Admin (drop Sophisticated on publish). **Builder** = OEB-class only. Do not call a real/TD-sibling app a Sample. |
| Library-bound | Everything that works as a Test Drive is **on the plan to move to public Library** — Samples, dual-use real apps, Builders, and TD siblings. Operate-only this week (GT / Potluck) still on that plan once a TD door exists. |
| Double smoke | If the Library TD shape is not the same JSON as the My Tawala working copy, smoke **both**: TD door (one in-session hub) **and** the multi-start deploy. Poll / Sign-up = one JSON, both roles. Shared To-Do = two JSONs (already both smoked). |
| Test Drive wipe | Leaving a start **wipes** TD data. Multi-start is for **My Tawala Use**, not hopping doors in one TD. |
| Library TD door | One **in-session** hub that can reach the product (Poll AdminStart; Sign-up Customize; Shared To-Do Test Drive Setup). |
| **GT + Potluck** | Full apps = My Tawala now. **Library still on the plan** (TD sibling or looping demo later — parked Sample JSONs are not the operate apps). Not this week's Samples folder. |
| Sign-up Sheet w Email | New Project only — never Library |

### Option A pattern (Sign-up / Potluck / GT Customize·Setup)

After-save MCQ with alternate name `AfterSave`. Condition must be `Form:AfterSave` (not `MCQ1`) — Sign-up first fail was wrong field ref. Choices: try guest path / stay editing. Thin Admin not used as TD destination.

List Builder Sep 8: Setup hub is **MenuChoice** (a = customizer, b = list/sign-up, c = stay). Not AfterSave.

---

## Automated List Builder — identity (Sep 8)

**Still true:** live Tomcat name `Automated List Builder` = uniqueId `ceihmyxlssxn6yn` (Library/public occupant). Do not Push the tray file under that bare name.

**Canonical file (Sep 8, after typo pass):** `2-Ready-for-MyTawala/Smoked-Ready-to-Push/Automated List Builder.json`  
Display name **Automated List Builder**. Same live copy: `deployUniqueId` `vrjayw9lwun85i1`. Tomcat is still parked as Smoke so Push does not overwrite public occupant `ceihmyxlssxn6yn`.  
Setup: `http://localhost:8080/p/vrjayw9lwun85i1/r2s6sqe.Setup`

**Email warning (Sep 8, option 2):** Library listing Test Drive / Copy-link honesty (`tooltipSingleEmail`) + **Sends real email.** badge. In-form `TWarn` **removed**. **Version 2 (B9):** runtime TD flag so a form item can show only during Test Drive.

**Not done:** retire/hatch `ceihmyxlssxn6yn`. Extra JSON copies (Sophisticated / Smoke filenames) **deleted Sep 8** — only `Smoked-Ready-to-Push/Automated List Builder.json` remains. Recovery path is the old `.tawala`, not a JSON twin.

---

## Designer (git) — parked

- Form Text blank/Style — done
- **C14** FIB Above + `*`; **C15** Pre-populate / Block Back; **F1** 3-browser — later
- **Process script opening-quote wrap** (Sep 4 polish) — noted in `DESIGNER_OPEN_TODOS.md` + shot in assets

---

## Next

1. **GT + Potluck Test Drive smoke passed Sep 9.** JSON still under `_set-aside/*Sample__parked-2026-09-09.json` unless you Save As’d elsewhere. Do not stamp operate uniqueIds.
2. **Library pack:** ALB Tomcat name vacated (`retire-name` on `ceihmyxlssxn6yn`). ALB goes in with the others. Publish a Library clone (email honesty); do not share My Tawala uniqueId `vrjayw9lwun85i1` as the public Test Drive.
3. My Tawala keepers are in `Current Deployed MyTawala` (Poll, Sign-up, Shared To-Do, GT, Potluck, ALB, OEB).
4. Later: Designer C14/C15/F1.

---

## New-chat opener

```text
Library / Samples pass — resume from docs/LIBRARY_WHERE_WE_ARE_SEP3.md (Sep 9).
Branch cursor/forms-canvas-wysiwyg. Staging outside git: Website Staging.
Smoked-Ready-to-Push: Poll, Sign-up Sheet, Shared To-Do, Automated List Builder.
Samples: Survey / Multi-Q / Sign-up / Horses approved. Library-bound TD: Poll, Sign-up, STD, GT, Potluck. OEB Live. ALB: retire-name ceihmyxlssxn6yn.
List Builder: Automated List Builder.json → uniqueId vrjayw9lwun85i1. Occupant ceihmyxlssxn6yn left live.
No Designer C14 this pass.
```

---

## Fri Sep 4 evening close (pre-travel) — archived

**Owner travel:** Saturday → ~1 week. Left mid-smoke Fri Sep 4 ~11:15.

Samples then: Survey / Multi-Q / Sign-up smoked; Horses not copied; GT/Potluck Samples not built.  
List Builder Push blocked on occupant `ceihmyxlssxn6yn` vs tray `6jpbj1hn2yzghh9`.
