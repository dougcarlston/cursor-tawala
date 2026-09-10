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

Did **not** this pass: public Library Push; Designer C14/C15/F1.

**Sep 9 — Library admin Delete (replaces Retire):** listings are removed with no My Tawala copy; the name is free for a later Publish. Occupant uniqueIds (ALB `ceihmyxlssxn6yn`) can be vacated from library-admin **Free a live :8080 name** — no agent hatch. Tomcat still cannot destroy uniqueIds (rename only).

**Sep 9 — Test Drive doors:** Library admin **Test Drive doors** sets per listing **Single door** (default) or **Start picker**. Single door also picks which start Test Drive / Copy link open. Copy to MyTawala still clones every start. Stored in `tawala.mock.libraryTestDriveDoors`. Missing overlay uses the old heuristics (OEB→Administration, email/Shared To-Do→Setup).

---

## Exact tray state (Sep 8)

### Samples — `3-Ready-for-Public-Library/Samples/`

| File | Status |
|------|--------|
| `Survey Sample.json` | **Live Library Sep 9** (overwrite Simple Survey) |
| `Multiple Question Survey Sample.json` | **Live Library Sep 9** (overwrite Simple Multiple Question Survey). Drop Report as a **start** (keep Report form; loop still reaches it). TD door = Survey. |
| `Sign-up Sheet Sample.json` | **Live Library Sep 9** |
| `Horses and Penguins Test.json` | **Live Library — leave as-is** (owner Sep 9; do not re-Publish or rewrite the blurb) |
| `Get Together Sample.json` | **Parked Sep 9** → `_set-aside/Get Together Sample__parked-2026-09-09.json`. Not a Library Sample this pass. |
| `Potluck Sample.json` | **Parked Sep 9** → `_set-aside/Potluck Sample__parked-2026-09-09.json`. Not a Library Sample this pass. |

### Smoked real apps — `2-Ready-for-MyTawala/Smoked-Ready-to-Push/`

Public names (drop Sophisticated). Library **Test Drive OK** for these four:

| File | TD door | Notes |
|------|---------|--------|
| `Single Question Poll or Survey.json` | **AdminStart + Start Questionnaire** | Live My Tawala uniqueId `b7ttti35vdo58pu`. **Library TD OK** (AdminStart). **Not Library until owner Pushes the Sep 9 leak fix:** Survey-link respondents get thank-you only (no Administration invitation). Organizer still uses AdminStart / Administration. Email notify still links Administration for the organizer. |
| `Sign-up Sheet.json` | **Customize** (also Questionnaire + Administration) | Keeper uniqueId `gnqp5qfd06ktec3`. Leftover My Tawala row deleted Sep 9. Dual-use like Poll: Customize is the TD hub. |
| `Shared To-Do Test Drive.json` | **Setup** only | Smoke artifact — **not** the public listing. Live Library is the three-start app (`shared-to-do`, uniqueId `bx44wpdnspbsi3k`). Website Test Drive opens Setup only. |
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
| Double smoke | If the Library TD shape is not the same JSON as the My Tawala working copy, smoke **both**: TD door (one in-session hub) **and** the multi-start deploy. Poll / Sign-up = one JSON, both roles. **Shared To-Do Library = one JSON** (three starts); website Test Drive opens Setup only. The Setup-only `Shared To-Do Test Drive.json` is a smoke artifact, not the listing. |
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

## Next — tomorrow (Sep 10)

**First group is in Live Library (owner Sep 9):** Survey Sample, Multiple Question Survey Sample, Sign-up Sheet Sample, Horses and Penguins Test. Horses stays as published — no description edit, no re-Publish. **Shared To-Do** is also Live (`bx44wpdnspbsi3k`).

1. **Publish Sign-up Sheet** (operate JSON, uniqueId `gnqp5qfd06ktec3`) as a new Library listing — not the Sample already Live.
2. **ALB:** Library admin **Free a live :8080 name** for occupant `ceihmyxlssxn6yn`, then Publish a Library clone from the `vrjayw9lwun85i1` definition (email honesty). Do not share that My Tawala uniqueId as public Test Drive.
3. **Poll** — wait for owner Push of the leak-fix JSON onto keeper `b7ttti35vdo58pu` before any Library Publish. Poll v.8 still Defer-rewrite.
4. **GT + Potluck** — parked Sample JSONs stay in `_set-aside/`. Not this Samples group. Do not stamp operate uniqueIds.
5. Optional: set any Live listing’s Test Drive door in Library admin (default is already single door).
6. Later: Designer C14/C15/F1. Keepers live in `Current Deployed MyTawala`.

---

## New-chat opener

```text
Library / Samples pass — resume from docs/LIBRARY_WHERE_WE_ARE_SEP3.md (Sep 9).
Branch cursor/forms-canvas-wysiwyg. Staging outside git: Website Staging.
Smoked-Ready-to-Push: Poll, Sign-up Sheet, Shared To-Do, Automated List Builder.
Samples first group Live Library (Sep 9): Survey Sample, MQSS, Sign-up Sample, Horses (leave as-is). Shared To-Do Live Library Sep 9 (`bx44wpdnspbsi3k`, Setup-only TD door). Next: Sign-up Sheet (operate), ALB (retire-name ceihmyxlssxn6yn). Single Question Poll = My Tawala keeper, Library later. Poll v.8 still Defer-rewrite. OEB Live. GT/Potluck not this Samples group.
List Builder: Automated List Builder.json → uniqueId vrjayw9lwun85i1. Occupant ceihmyxlssxn6yn left live.
No Designer C14 this pass.
```

---

## Fri Sep 4 evening close (pre-travel) — archived

**Owner travel:** Saturday → ~1 week. Left mid-smoke Fri Sep 4 ~11:15.

Samples then: Survey / Multi-Q / Sign-up smoked; Horses not copied; GT/Potluck Samples not built.  
List Builder Push blocked on occupant `ceihmyxlssxn6yn` vs tray `6jpbj1hn2yzghh9`.
