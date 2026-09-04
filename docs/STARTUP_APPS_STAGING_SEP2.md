# Startup / sophisticated apps — smoke set (Sep 2, 2026)

Tray: `Website Staging/3-Ready-for-Public-Library/`

Selection base: `Prior Versions 8-20-2026/Sophisticated Versions of Startup Apps/` (+ twins).  
**Size gate:** prefer **40–150 KB**. Flag under 40 or over 150 (may still be the right product).

Alternates (distinct copies) live in `_alternates-startup-apps/`.

**Online Exam Builder** is **not** in this set — see `Website Staging/Builders-hold/` (classroom / builder track).

## Primary smoke set

| File | KB | Flag | Notes / alternate |
|------|---:|------|-------------------|
| ~~`Sophisticated Get Together.json`~~ | — | — | **Moved → `2-Ready-for-MyTawala/`** (approved Sep 2; Push My Tawala; Test Drive later) |
| ~~`Get Together.json`~~ | — | — | Twin → `_set-aside/` (same codebase as Sophisticated) |
| ~~`Sophisticated Sign-up Sheet.json`~~ | — | — | **Moved → `2-Ready-for-MyTawala/`** (approved Sep 2; Push My Tawala; Library/Test Drive later) |
| ~~`Sign-up Sheet.json`~~ | — | — | Twin set-aside; full-feature stubs deleted Sep 2 |
| ~~`Sophisticated Poll or Survey.json`~~ | — | — | **Deferred Sep 3** → `Website Staging/Defer-rewrite/` (rewrite later; not this pass) |
| ~~`Poll or Survey.json`~~ | — | — | Twin deferred with Sophisticated |
| ~~`Single Question Poll or Survey.json`~~ | — | — | **Moved → `2-Ready-for-MyTawala/`** as `Sophisticated Single Question…` (from My Tawala Sep 2) |
| ~~`Sophisticated Potluck.json`~~ | — | — | **Moved → `2-Ready-for-MyTawala/`** (approved Sep 2; kids/adults in one app) |
| ~~`Kids Too.json`~~ | — | — | **Not separate** — tossed to `1-Review/Trash/`; content folded into Sophisticated Potluck |
| ~~`Shared To-Do.json`~~ | — | — | **Moved → `2-Ready-for-MyTawala/`** as Sophisticated Shared To-Do (approved Sep 2) |
| ~~`Emailer With Signup.dgmod.json`~~ | — | — | **Deferred Sep 3** → `Defer-rewrite/` (empty process-shell forms; unfinished) |

## Suspect / out of this group (already in tray)

| File | KB | Note |
|------|---:|------|
| `CYO Exceptions App.json` | 273 | OVER 150 — not a startup app |
| `Dirtbowl Communicator.json` | 199 | OVER 150 — not a startup app |
| `Designer Candidate App 01 copy.json` | 15 | UNDER 40 — stub |
| ~~`Automated List Builder.json`~~ | — | — | **Moved → `2-Ready-for-MyTawala/`** as Sophisticated Automated List Builder (approved Sep 3; Test Drive prep later) |
| `MVSC Registration.json` | 71 | different product family |

## Library replacement policy (owner Sep 3)

**Library naming:** never **Simple**. Rows are either a **Sample** (looping demo) or the **real app**. Drop “Sophisticated” on publish (tray filename only).

**Builder** is reserved for apps that *create a more complex instrument* (exam, full poll constructor) — **Online Exam Builder**, and the deferred Poll or Survey v.8 rewrite. **Not** for one-off event/list tools that happen to have Customize (Potluck, Get Together, Sign-up).

Live Library check: only **Simple Survey** (rename to a Sample) and **Horses and Penguins Test** currently “just work.”

| Live Library now | Policy |
|------------------|--------|
| **Simple Survey Template** (~6.4 KB) | **Keep**; Library title → **Survey Sample** (or similar). Ready-to-use loop. Also New Project. |
| **Horses and Penguins Test** | **Keep** (works). |
| **Multiple Question Survey Template** (~12.4 KB) | **Keep** as **Multiple Question Survey Sample**; add results link then back to survey so **one start** and responses accumulate. |
| **Potluck Template** (~20.5 KB) | Thin ≈ real Potluck minus kids count and Customize. **Keep as Potluck Sample** only if we **loop** the organizer so attendees/contributions accumulate. Else drop. Real app = Customize + kids (`2-Ready` Potluck). |
| **Get Together Template** (~10.7 KB) | **Keep as Get Together Sample** only if Form1 **Post Show Form1** (multiple names, one start) **and** replace the bad theme. Real app = Customize version in `2-Ready`. |
| **Sign-up Sheet** | Publish New Project looping **Sign-up Sheet Sample**. Real app = Customize / Questionnaire / Administration (`2-Ready`). Old broken catalog seed stays gone. |
| **Online Exam Builder** | **Builder** / classroom (`Builders-hold/`). |
| **Poll or Survey v.8** | Deferred rewrite — **Builder** class (construct a poll/petition), not a Sample. |

**Sign-up Sheet w Email:** **New Project only** — cannot be used without Designer. Never Library.

## Split + sequence (Sep 3 — no Push until both piles are done)

Work in parallel, then **check each other’s work**, then Push My Tawala / Library under public names (no Simple, no Sophisticated).

**Travel:** owner away ~1 week from Saturday; Fri = backup + next steps. Full status: `docs/LIBRARY_WHERE_WE_ARE_SEP3.md`.

### A. Doug — Samples

Tray: `3-Ready-for-Public-Library/Samples/` (README checklist there).

**Ready:** Survey Sample; Multiple Question Survey Sample.  
**In progress:** Sign-up Sheet Sample (New Project loop).  
Still: Horses and Penguins; Potluck/GT Sample only if looped.

### B. Agent — real apps (`2-Ready-for-MyTawala/`)

**Policy (Sep 3 evening):** keep **multiple starts** on real apps; Library Test Drive prefers Customize / Setup / Administration / AdminStart. My Tawala Use / Details unchanged.

**Current tray state:** most apps still have a single `startPoint` from an earlier pass; **Sign-up Sheet** again has Customize + Questionnaire + Administration. **Next:** restore multi-start on the other five + expand Test Drive preference aliases.

| File | Intended Test Drive door | Guest / other starts (restore) |
|------|--------------------------|--------------------------------|
| Sign-up Sheet | **Customize** | Questionnaire, Administration |
| Get Together | **Customize** | Questionnaire, Administration |
| Potluck | **Setup** | Potluck Organizer, Administration |
| Shared To-Do | **Setup** | Administration, Signup |
| Automated List Builder | **Setup** | Questionnaire, Administration |
| Single Question Poll | **AdminStart** | (poll is `Questionnaire`; Start Questionnaire is a skip stub) |

**Not this pass:** C14 FIB; Poll v.8 / Emailer rewrite; OEB (already shipped); Wildcat (My Tawala only).

### Then

1. Cross-check Samples folder vs 2-Ready (multi-start + Test Drive preference).  
2. Push real apps + Samples.  
3. Publish / replace Live Library rows.

## How to smoke

1. File→Open from this tray in Designer.  
2. Push → My Tawala under a **public name** (drop “Sophisticated”).  
3. Use / fix / re-Push.  
4. Publish to Library when clean.

## Approved / out of this tray

All six sit in `2-Ready`. **Do not Push** until Samples are ready and multi-start + Test Drive preference are settled (`docs/LIBRARY_WHERE_WE_ARE_SEP3.md`).

- **Sophisticated Automated List Builder** → `../2-Ready-for-MyTawala/` (Setup door; restore Questionnaire/Administration starts).
- **Sophisticated Shared To-Do** → `../2-Ready-for-MyTawala/` (Setup door; internal name Shared To-Do).
- **Sophisticated Potluck** → `../2-Ready-for-MyTawala/` (Setup door; kids/adults included).
- **Sophisticated Single Question Poll or Survey** → `../2-Ready-for-MyTawala/` (AdminStart door).
- **Sophisticated Get Together** → `../2-Ready-for-MyTawala/` (Customize door).
- **Sophisticated Sign-up Sheet** → `../2-Ready-for-MyTawala/` (multi-start restored by owner Sep 3 evening).
## Not for Public Library

- **Wildcat Week** — My Tawala only (private / school-week style); removed from this Library smoke tray Sep 2. Live copy stays on My Tawala; working JSON if needed: `1-Review/Under Construction/Wildcat Week.json`.

---

## Session note — Sep 2 evening (for tomorrow)

### Done today

- Smoke / approve sophisticated startups → park in `Website Staging/2-Ready-for-MyTawala/`:
  - Sign-up Sheet, Get Together, Single Question Poll, Potluck (Kids Too folded in — not separate), Shared To-Do
- Tray hygiene: twins/stubs set aside or tossed; OEB stays in `Builders-hold/`; Wildcat = My Tawala only
- Potluck: fixed broken Append (`ConfirmationAttend` → `ConfirmationEmail`); restored adults+kids blanks/tables
- Shared To-Do: reopened Document two-links underline bug — **Design CSS** + **Push** fix (`documentHtmlToXml` no longer underlines plain `or` between invitations). Re-Push after API restart.
- Occupancy: tray JSON with stale `deployIdentityName`/`deployUniqueId` blocks rename-Push — clear stamps or mint private name
- **Sep 3:** single Start Point on the six `2-Ready` apps; Samples tray created for Doug. Push after cross-check.

Poll v.8 + Emailer already in `Defer-rewrite/`. Optional later: Alumni List Builder.

**Trays:** smoke in `3-Ready-for-Public-Library/`; approved → `2-Ready-for-MyTawala/`; README there lists the set.

## Deferred rewrite (Sep 3)

- **Emailer With Signup** → `Defer-rewrite/` (Sep 3 reconvert OK; product is process-shell / unfinished). Next smoke: **Automated List Builder**.
- **Poll or Survey** (both copies) → `Website Staging/Defer-rewrite/`. Old v.8: poor form labels, pre-function data display, Reorder vs chart order mismatch. Not a cheap update — park until a dedicated rewrite.
