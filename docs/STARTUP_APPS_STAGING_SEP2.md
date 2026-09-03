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
| `Sophisticated Poll or Survey.json` | 170.9 | OVER 150 | Large multi-feature survey |
| `Poll or Survey.json` | 171.2 | OVER 150 | Twin; alt Aug 20 170KB |
| ~~`Single Question Poll or Survey.json`~~ | — | — | **Moved → `2-Ready-for-MyTawala/`** as `Sophisticated Single Question…` (from My Tawala Sep 2) |
| ~~`Sophisticated Potluck.json`~~ | — | — | **Moved → `2-Ready-for-MyTawala/`** (approved Sep 2; kids/adults in one app) |
| ~~`Kids Too.json`~~ | — | — | **Not separate** — tossed to `1-Review/Trash/`; content folded into Sophisticated Potluck |
| ~~`Shared To-Do.json`~~ | — | — | **Moved → `2-Ready-for-MyTawala/`** as Sophisticated Shared To-Do (approved Sep 2) |
| `Emailer With Signup.dgmod.json` | 126.3 | ok | Alt Jul 28 113KB |

## Suspect / out of this group (already in tray)

| File | KB | Note |
|------|---:|------|
| `CYO Exceptions App.json` | 273 | OVER 150 — not a startup app |
| `Dirtbowl Communicator.json` | 199 | OVER 150 — not a startup app |
| `Designer Candidate App 01 copy.json` | 15 | UNDER 40 — stub |
| `Automated List Builder.json` | 99 | different product family |
| `MVSC Registration.json` | 71 | different product family |

## How to smoke

1. File→Open from this tray in Designer.  
2. Push → My Tawala under a **public name** (drop “Sophisticated”).  
3. Use / fix / re-Push.  
4. Publish to Library when clean.

## Approved / out of this tray

- **Sophisticated Shared To-Do** → `../2-Ready-for-MyTawala/` (approved Sep 2; single-start later with the set).
- **Sophisticated Potluck** → `../2-Ready-for-MyTawala/` (kids/adults included; former Kids Too tossed).
- **Sophisticated Single Question Poll or Survey** → `../2-Ready-for-MyTawala/` (from My Tawala Sep 2).
- **Sophisticated Get Together** → `../2-Ready-for-MyTawala/` (My Tawala Push; starts dead until Push; not Library yet).
- **Sophisticated Sign-up Sheet** → `../2-Ready-for-MyTawala/` (My Tawala Push; not Library yet).

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
- **Deferred (batch later):** single Start Point on approved sophisticated apps for Library Test Drive

### Tomorrow — start here

1. **Sophisticated Poll or Survey** (`3-Ready` pair; ~171 KB — over size flag; compare twins, keep one)
2. **Emailer With Signup.dgmod.json** (startup / mass-email adjacent)
3. **Add one list-builder / mass-email** to the smoke set (high demand). Candidates:
   - `3-Ready/Automated List Builder.json` (~99 KB)
   - `1-Review/Under Construction/Alumni List Builder.json`
   - Prior Versions `List Builder and Managers/`
   Pick the cleanest convert; smoke → `2-Ready` if good

Then continue Library/site work or the single-start batch when the sophisticated pile is settled.

**Trays:** smoke in `3-Ready-for-Public-Library/`; approved → `2-Ready-for-MyTawala/`; README there lists the set.