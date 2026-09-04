# Where we are / where we’re going — Sep 4, 2026 evening (pre-travel)

**Owner travel:** Saturday → ~1 week (laptops; Wi‑Fi uncertain).  
**Left mid-smoke Fri Sep 4 ~11:15** — may peek Saturday ~1h.  
**Branch:** `cursor/forms-canvas-wysiwyg`  
**Staging (NOT in git — backup separately):**  
`~/Projects/ Current Tawala Projects/Website Staging/`

Companions: this file; tray READMEs under `2-Ready-for-MyTawala/` and `3-Ready-for-Public-Library/Samples/`.

---

## Exact tray state (Fri Sep 4 close)

### Samples — `3-Ready-for-Public-Library/Samples/`

| File | Status |
|------|--------|
| `Survey Sample.json` | Owner smoked ✓ |
| `Multiple Question Survey Sample.json` | Owner smoked ✓ |
| `Sign-up Sheet Sample.json` | Owner smoked ✓ (New Project copy) |
| Horses and Penguins | Smoke passed; **file not copied into Samples tray yet** |
| Get Together Sample / Potluck Sample | **Needed for Library** (not full-app TD) — not built yet |

### Smoked real apps — `2-Ready-for-MyTawala/Smoked-Ready-to-Push/`

Public names (drop Sophisticated). Library **Test Drive OK** for these three:

| File | TD door | Notes |
|------|---------|--------|
| `Single Question Poll or Survey.json` | **AdminStart** | Go back to Customize fixed (`forceCustomizeEdit`) |
| `Sign-up Sheet.json` | **Customize** | Option A: AfterSave → Try signup sheet / Stay (`Customize:AfterSave`) |
| `Shared To-Do.json` | **Setup** | Option A: menu **c** → View list / try signup (`SignupForTask`) |

### Still in prep — `2-Ready-for-MyTawala/`

| File | Status |
|------|--------|
| `Sophisticated Get Together.json` | Option A AfterSave wired; **no Library TD** — Sample/movie instead. My Tawala only for full app. |
| `Sophisticated Potluck.json` | Option A AfterSave wired; **no Library TD** — Sample/movie instead. |
| `Sophisticated Automated List Builder.json` | **Push blocked** — see below. Twin public-named file was tried; may be gone from tray. |

Also: older junk at `3-Ready` root (CYO, Dirtbowl Communicator, MVSC, etc.) — **not** this startup-app pass.

---

## Product rules locked this week

| Rule | Meaning |
|------|---------|
| Sample vs Real vs Builder | Sample = Library demo; Real = Customize/Setup/Admin (drop Sophisticated on publish); Builder = OEB-class only |
| Test Drive wipe | Leaving a start **wipes** TD data. Multi-start is for **My Tawala Use**, not hopping doors in one TD. |
| Library TD door | One **in-session** hub that can reach the product (Poll AdminStart; Sign-up/Shared To-Do Option A). |
| **GT + Potluck** | **Not** Library Test Drive. Correlation/multi-attendee needs many replies. Library = **Sample or movie**. Full apps = My Tawala. |
| Sign-up Sheet w Email | New Project only — never Library |

### Option A pattern (Sign-up / Potluck / GT Customize·Setup)

After-save MCQ with alternate name **`AfterSave`**. Condition must be `Form:AfterSave` (not `MCQ1`) — Sign-up first fail was wrong field ref. Choices: try guest path / stay editing. Thin Admin not used as TD destination.

---

## Automated List Builder — Push blocked (Fri afternoon)

**Symptom:** Cannot Push under any display rename of “Automated List Builder”.

**Cause:** Tomcat already has live name **`Automated List Builder`** uniqueId **`ceihmyxlssxn6yn`** (Library/public occupant). Staging JSON had `deployUniqueId` **`6jpbj1hn2yzghh9`** ≠ occupant → **409 `name-occupied`**. Renaming display name alone does not clear the path if deploy identity still resolves to that occupied name.

**Workaround proven:** minted private identity Push succeeds, e.g.  
`Automated List Builder Smoke 026523ea` → uniqueId **`vrjayw9lwun85i1`**  
Setup: `http://localhost:8080/p/vrjayw9lwun85i1/r2s6sqe.Setup`

**Resume:** Clear/replace `deployIdentityName` + `deployUniqueId` on the JSON so next Push mints a private name; or retire/hatch the public occupant. Then TD-smoke Setup door (may need Option A like Shared To-Do).

---

## Designer (git) — parked

- Form Text blank/Style — done  
- **C14** FIB Above + `*`; **C15** Pre-populate / Block Back; **F1** 3-browser — later  
- **Process script opening-quote wrap** (Sep 4 polish) — noted in `DESIGNER_OPEN_TODOS.md` + shot in assets  

---

## Next (when owner returns)

1. **Backup Staging** (zip/copy) — not in git.  
2. List Builder: mint private Push identity → TD smoke or park.  
3. **GT / Potluck Samples** (or movie) for Library.  
4. Copy Horses into Samples tray.  
5. Cross-check Samples + Smoked TD trio → Push public names to Library/My Tawala as planned.  
6. Later: Designer C14/C15/F1.

---

## New-chat opener

```text
Library / Samples pass — resume from docs/LIBRARY_WHERE_WE_ARE_SEP3.md (Sep 4 evening).
Branch cursor/forms-canvas-wysiwyg. Staging outside git: Website Staging.
Smoked-Ready-to-Push: Poll, Sign-up Sheet, Shared To-Do (TD OK).
GT+Potluck = Sample/movie for Library (no TD). List Builder Push blocked: name Automated List Builder occupied by uniqueId ceihmyxlssxn6yn — mint private deployIdentity or retire occupant. Smoke URL that worked: /p/vrjayw9lwun85i1/…Setup
No Designer C14 this pass.
```
