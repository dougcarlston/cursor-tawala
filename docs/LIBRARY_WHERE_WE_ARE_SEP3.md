# Where we are / where we’re going — Sep 8, 2026 (Library / Samples resume)

**Branch:** `cursor/forms-canvas-wysiwyg`  
**Staging (NOT in git):** `~/Projects/ Current Tawala Projects/Website Staging/`  
**Backup (Sep 8, before this pass):** `~/Projects/ Current Tawala Projects/Website Staging backups/Website-Staging-2026-09-08.zip`

Companions: this file; tray READMEs under `2-Ready-for-MyTawala/` and `3-Ready-for-Public-Library/Samples/`. UniqueId sidebar: `.cursor` canvases `library-project-ids.canvas.tsx` (open beside chat).

**Owner travel:** Fri Sep 11 afternoon + Sat Sep 12. Resume **Monday Sep 14** (California) from the list below.

---

## Monday Sep 14 — ToDo (owner left Fri Sep 11)

Do **not** start Designer C14/C15/F1. Do **not** implement “projects of projects” until the planning session below is done. Isolation restoration stays the product spine.

### A. First — OEB Setup first-pass — **done Mon Sep 14**

Setup start should open **SetupVariables** (title / date / Q3) when there are **no Question rows**. Hub “Your exam is now set up!” + Update Setup is **original v.8** once any question exists — not a single-door rewrite.

**Bug:** Library JSON Pre-Setup (Aug 28 `e02b507`) set `FirstViewOfSetup` to two quote characters (`"\"\""`), so `isBlank` always failed and Setup skipped first-pass. Session leftover `FirstViewOfSetup=false` or a Test Drive snapshot of live `/p/` questions does the same.

**Done in JSON (not live until Push):** real empty reset at the top of Pre-Setup, then the original “any Question → hub” test. Files:

- `website-mock/projects/library/Online Exam Builder.json`
- `website-mock/projects/mytawala/Online Exam Builder.json`
- Keeper: `~/Projects/ Current Tawala Projects/Current Deployed MyTawala/Online Exam Builder.json`  
  (Open this. Live My Tawala `un7qtlf0tcc0rqq` / Tomcat name **Online Exam Builder e3c56ca6**. Public Library `u3hkqgwtrepjlur`.)

**Smoked My Tawala Use** on `un7q…`: Purge questions → Setup goes to Customize/SetupVariables; Restore 3 questions → Setup hub + pre-AdminDashboard MCQ. **Publish replace in place** onto Library `u3hkqgwtrepjlur`. Owner confirmed Library TD uses that uniqueId with a **`/t/`** door.

**Duplicate My Tawala listing (Sep 14):** Push / Show in My Tawala keyed listings by display-name slug, so a second OEB card appeared with the same uniqueId `un7q…`. **Fixed** — same uniqueId updates the existing listing in place. Owner deleted the older listing (overlay only). Hard-refresh My Tawala (`transfer.js?v=20260914-uidmerge1`).

Also confirm examinee **Exam** still shows Exam Start (title + Q3) — that Push/Publish already smoked Sep 11 (`Pre-Exam`).

### B. Finish Test Drive isolation restoration

**Wired Sep 11 (in this commit):** Library Test Drive / Copy link → `http://localhost:8080/projectmanager/testdrive?id={uniqueId}&form={Form}` → 302 `/t/{uniqueId}/{token}.{Form}` + `JSESSIONID`. Private in-memory World. Two browsers = two copies. My Tawala **Use** stays `/p/`. Start picker stays open after pick (hop starts, same session). Honesty copy: private try-out, not “same uniqueId for every visitor.”

**Still open (do in order):** — **closed Sep 14 evening** (B1–B5 smoked earlier; isolation-pass leftovers below).

1. **Stop copying live `/p/` into a new TD World** — **smoked Sep 14.** `TestDriveSupport.copyExistingSubmissionsToTestWorld` is a no-op. Library OEB TD: Administration empty; `…/testdrive?id=u3hk…&form=Setup` → SetupVariables. Same session still hops. **Docker bake:** `docker compose build tawala` recompiles isolation Java over `ROOT.war` (see `docker/tomcat/README.md`).
2. **Idle TTL / leave-wipe** — **smoked Sep 14 (OEB picker).** Closing **all** `:8080` `/t/` tabs discards the drive after **60s**; picker on `:5500` may stay. Same-session hops (Customize ↔ Exam ↔ Admin) **keep** state. Discard drops the in-memory World **and** Test Drive session variables (`Customize_Title` etc.) — Setup FIBs going blank is not enough. Idle **30 min** with 4 min keepalive while the tab is visible. Beacon is on `:8080` `/t/`, not the listing. Honesty: “Closing the browser tab or window ends the Test Drive. Starting a Test Drive of a different project does too.” Owner Sep 14: 20s felt too quick while hopping; grace is **one minute**.
3. **Smoke dual-use via Library TD** — **smoked Sep 14** including Poll. Sign-up Customize-first; Shared To-Do **Start picker** (admin door; this browser may still have Setup single); OEB already; Poll **AdminStart single door** (owner: passes). Default door remains **single start**; catalog seeds picker for **Get Together** and **Potluck**.
4. **Then Publish dual-use as they already are** — no JSON reshape for a single `/p/` door:
   - Sign-up Sheet operate Library uniqueId **`ffawe1uxeudjwpr`** (Tomcat `Sign-up Sheet 49a8b2d5`). My Tawala Use stays `gnqp5qfd06ktec3`. Do not overwrite Sign-up Sheet Sample `kxlgkzkqmaebhz3`.
   - **ALB stays My Tawala only** until invitation Confirm links work on a public host. Do not Publish; do not Free `ceihmyxlssxn6yn`. Live copy `vrjayw9lwun85i1`.
   - **Poll** — Library occupant `ca7ws92mpss0sb5` Redeployed + TD smoked Sep 14.
5. Sync catalog JSON on disk after each successful in-place Publish so Copy to MyTawala matches `:8080`. **Sep 14:** dumped live Library uniqueIds into `Current Deployed Public Library/` (root). Aug 4 `Live Library Projects/` template copies **removed**. Also copied OEB / Shared To-Do / Poll / Sign-up Sheet / Horses into `website-mock/projects/library/`. **Sep 15:** Get Together / Potluck operate overwrite dumped into CDPL + `projects/library/` (`Get Together.json`, `Potluck.json`).

**Isolation-pass close (Sep 14 evening):**

- **Publish replace Redeploys.** Overlay-only listings (Poll) often had uniqueId but no `deployIdentityName`, so replace minted a sibling and Tomcat `ca7ws…` stayed stale. Publish now looks up the live Tomcat name (`POST :3001/api/lookup-unique-id`) and Redeploys onto the listing uniqueId. Missing identity **fails closed** — it does not mint.
- **Poison Poll file.** `website-mock/projects/mytawala/Single Question Poll or Survey.json` is the leak-fixed AdminStart dump **without** live uniqueId stamps. Do not Publish that archive onto the listing.
- **Poll Quit copy.** Thanks document is “You're done. You can close this window.” (not “Thanks for Test Driving…”). Same in CDPL + Library catalog JSON; keeper My Tawala uses the same idea.
- **Copy to MyTawala** smokes the dumped Poll/Sign-up catalog JSON (empty private clone; not `ca7ws…` / `ffawe…`).
- **Get Together / Potluck Live Library Sep 15.** Operate overwrite of Simple uniqueIds `b6do4s50iq64vl8` / `52ozm3kqd58zlss`. Catalog names drop Sophisticated. TD = **Start picker** (catalog `testDriveMode`). File → New simples stay Designer. Copy to MyTawala uses `projects/library/` JSON (overlay sample `jsonFile` is ignored).

### C. Reversions / alterations tied to isolation (do not undo the sandbox)

| Do | Do not |
|----|--------|
| Keep apps multi-start in JSON; TD isolation is the `/t/` World | Rewrite Sign-up / ALB / GT so Customize is the only start |
| Jump-tracks in **published invite URLs** = definition rule (Poll Admin vs User wall) | Jump-track tricks so Test Drive is one portal |
| Copy to MyTawala = empty private clone of the **listing definition** | Same uniqueId as Library TD |
| Publish = empty Library uniqueId (reuse listing id on **replace**) | Put author’s My Tawala data on public TD |
| Start picker = rare opt-in; list stays open while hopping | Close picker on pick (felt like the drive died) |
| | Revive Aug 26 same-uniqueId “keep Question/SetupVariables, purge Exam” |

**Ancillary (same spine):**

- Honesty / tests: `js/testDriveHonesty.test.mjs`, `js/testDriveSandbox.test.mjs` — keep green when touching TD URLs.
- Cache-bust Library scripts after TD/JSON changes.
- Two hard walls still: unregistered visitors never see each other; clients never jump into Administration (invitees on a private My Tawala uniqueId are the exception).
- GT/Potluck looping Sample JSONs stay parked in `_set-aside/`. Live Library is the operate apps. Keepers: `Current Deployed MyTawala` + CDPL dumps.
- Duplicate My Tawala rows with the **same** uniqueId: Delete **one listing** only (overlay). Does not retire `:8080`.
- Designer C14/C15/F1 still parked.

### D. Planning session — “Projects of projects”

**When:** Isolation **B1** and OEB first-pass **A** are already live. Needed because a Library try-out of a filled exam must be a sandbox with the author’s **questions**, not the author’s live uniqueId and not other visitors’ scores.

**Locked Sep 15 (owner):** product calls **confirmed**. This path is **Version 2** for public rollout. Version 1 may **plan and provision** the two-tier model (Builder vs user-contributed content) so we do not paint ourselves into a corner — no public “Publish as an exam,” no user-contributed folder, no Details copy until Version 2.

#### Version split

| | Version 1 (now / live Library) | Version 2 (public) |
|--|-------------------------------|---------------------|
| Users | Copy OEB to My Tawala, Use it, Publish still means “Library product” (blocked as duplicate of OEB). | Publish **as an exam** into User-contributed → that Builder. |
| Library | Blank Builders as ordinary liveReady listings (OEB today). | Badge and/or Builders section; sub-folder of exams under each Builder. |
| Provision | Data shape + admin-only Builder flag + do not hard-code “OEB is the only exception.” Keep/strip map stored on the Builder, unused by Publish. | Wire Publish / Copy / TD to that map; snapshot Builder version onto the exam listing. |
| Legal / promote | Out of band if ever needed. | Administrator contacts author; membership incorporate consent. |

Do **not** ship Version 2 UX in Version 1 “quietly.” Empty reserved fields are fine; a visible exam-Publish button is not.

**Sep 15 (Version 1 refuse):** Publishing My Tawala OEB as a **new** Library project (replace = None) must **alert** the duplicate-product string — not fail silently. Designer Push names like `Online Exam Builder e3c56ca6` count as the same product as catalog OEB. Overwrite of the **same product** is only the listing’s **author** (or Library admin for a Tawala seed with no member author). A coincidental same Library **name** with different code is blocked with a separate name-taken alert and **cannot** replace a Tawala seed (even as admin).

#### Product

Library holds **Builders** (blank instruments) **and** **user-contributed content** built *inside* a Builder, not in Designer. There may be **many** Builders; OEB is the first. A teacher Publishes an exam: keep authoring/subject content, strip student/respondent data. Other teachers Copy to My Tawala and run it with their classes.

**Builder is a Tawala Administrator designation.** Only that Administrator (the person who runs the public Library — not a teacher using a Builder’s Administration form, not the listed author of a My Tawala copy) can mark a Library product as a Builder. Users cannot self-declare a project as a Builder. A new Tawala-made project may be designated when it is added. Designation may show as a **badge**, or Builders may live in a **special section of the Library** (or both).

A Builder’s **My Tawala Project Details** (where **PUBLISH** already lives) gets a section that explains: a User of this Builder can **Publish an exam written in it** — not an update of the Builder itself.

**Promoting contributor work** (to a Builder, to the public Library, or into the user-contributed Library) is not self-serve. The Tawala Administrator **contacts the author** and negotiates an agreement. That agreement **must** include legal consent for **all Tawala users** to incorporate the contributed project in their own work, subject only to the terms of their Tawala registration membership. Until that agreement exists, a user-published exam stays the author’s listing under the Builder’s user-contributed folder.

#### Locked product calls (owner Sep 15)

| Call | Decision |
|------|----------|
| Who Publishes content | **Any person with a My Tawala copy of the Builder** may Publish **as an exam** (content product). That is never an update of OEB (`u3hk…`). Duplicate-product refuse still blocks a second blank OEB. |
| Where it lists | **User-contributed content**, in a **sub-folder under the Builder** (under OEB for exams). Not a sibling of the blank tool in Advanced / Polls. |
| Updates | Default = **new contribution** (new Library uniqueId, new listing in that sub-folder). **Overwrite** only when the User is the **Author of that exam** and **explicitly** chooses replace. Overwrite targets the exam listing, never the Builder. |
| Builder upgrades | Published exams stay on the **Builder definition as of that publication**. New OEB capabilities do **not** flow to old exams. They pick up new capabilities only if the exam Author **updates and explicitly overwrites** from a copy that already has the newer Builder. |
| Many Builders | Any number. Each has its own user-contributed sub-folder and its own keep/strip map. Badge and/or a Library Builders section are allowed chrome. |
| Promote to Builder / public Library | Tawala Administrator contacts the author and negotiates an agreement. Required term: all Tawala members may incorporate the project in their own work, subject only to registration membership. No in-app “make this a Builder” for contributors. |

**Why start-point names are not enough:** OEB subject matter lives on `Question` and `SetupVariables` (and Customize_* derived from setup). Student data lives on `Exam` / `Answer` (and scores, incomplete tests, email confirmations). Administration is a **door**, not a data class. Aug 26 “keep Question/SetupVariables, purge Exam” on the **same** uniqueId scrambled OEB — do not revive it.

#### OEB keep / strip (Version 2 — table agreed as the map)

| Form | Role | Publish as exam | Copy that exam to MyTawala | Library TD of that exam (`/t/`) |
|------|------|-----------------|----------------------------|----------------------------------|
| `SetupVariables` | Authoring | **Keep** | **Clone** | **Seed** into the sandbox |
| `Question` | Authoring | **Keep** | **Clone** | **Seed** |
| `Exam` | Respondent | **Strip** | Empty | Empty |
| `Answer` | Respondent | **Strip** | Empty | Empty |
| `Scoring` | Derived report | **Strip** | Empty | Empty |
| `ShowExamineeDetail` | Student report | **Strip** | Empty | Empty |
| `Setup` / `Administration` / `Exam` starts | Doors | Keep as starts | Keep as starts | No write-back to `u3hk…` or the author’s My Tawala id |
| `Edit Question` / `Delete Question` / `CustomizationPreview` | Transient | Empty | Empty | Empty |

Reset session leftovers on kept rows (e.g. `LastSessionId`). Keep setup policy fields (title, date, Q3, ShowScore, email-on-complete). Other Builders get their **own** maps later — do not hard-code OEB form names into Publish.

**Isolation (Version 2):** B1 stays — do not copy **student** rows from live `/p/` into a new TD World. Content-product TD **does** copy **authoring** rows from the **exam’s Library uniqueId** into `/t/`. Questions present, scores empty. Version 1 keeps today’s empty TD World for the blank OEB tool.

#### Identity

- Exam = **new Library uniqueId** unless the Author explicitly overwrites their existing exam listing.
- Never Publish an exam onto the teacher’s My Tawala uniqueId.
- Never Redeploy the Builder uniqueId (`u3hk…`) as a filled exam.
- Occupancy is by **title**: “American History Final Exam” must not collide with “Online Exam Builder.”
- Derived-from-Builder flag (or equivalent) so duplicate-refuse does not treat every OEB-based exam as “the same as Online Exam Builder.”
- **Pinned Builder version:** an exam listing stores a snapshot of the Builder (definition + keep-form rows) at Publish/overwrite time. It is **linked to that former OEB**, not a live pointer at current Library OEB. Upgrading the blank tool (`u3hk…`) does not rewrite already-published exams. Copy to MyTawala of an old exam clones that pinned snapshot. To gain later Builder features, the Author must be working in a newer Builder copy, then **explicitly overwrite** the exam listing.

#### Sequenced build

**Version 1 provision (landed Sep 15 — no public UX):**

1. **Done.** Admin-only **Builder** flag + keep/strip map on catalog OEB (`builder: true`). Not shown on public Library.
2. **Done.** Reserved fields stripped from every My Tawala read/write (`stripBuilderProvisionFromPrivateRow`) so a teacher’s private OEB Use is unchanged.
3. **Done.** Duplicate-refuse still blocks Publish of a filled OEB copy as “another OEB.” Builder flag does not bypass that. Version 2 Publish-as-exam stays a separate path.
4. No Publish-as-exam dialog, no Details explanation, no User-contributed folder on the listing.

**Version 2 (public):**

1. My Tawala Project Details copy + Publish dialog: **Publish as an exam** (default new contribution) vs **Overwrite my exam…** (Author only).
2. Publish path: new uniqueId + snapshot of that Builder version + keep-form submissions + strip the rest; list under User-contributed → Builder.
3. Copy to MyTawala of an exam clones keep-forms.
4. Library TD of an exam seeds keep-forms into `/t/`.
5. Badge / Builders section / sub-folder chrome.
6. Later Builders: admin designation + their own keep/strip map.
7. Promote-to-Builder / public Library: Administrator agreement (not an in-app button).

**Not this product:** nested projects-of-projects-of-projects; billing; Designer work to “make” a Builder; automated contract/e-sign.

**Still open (chrome, Version 2):** how the sub-folder lists; badge vs a Builders section. Public Library Project Details stay retired — Version 2 explanation lives on **My Tawala Details** of a Builder copy.

---

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

**Sep 11 — Start picker stays open:** choosing a start opens the try-out in another tab and leaves the list up so you can hop. Close / Escape still dismisses it. Copy-link still closes after copy.

**Sep 11 — OEB examinee Exam Start:** Exam form preProcess is **Pre-Exam** (copy SetupVariables → Customize_* , then `showDocument Exam Start`). Name page shows title, date, and Q3 pre-instructions. Administration / CustomizationPreview still use SetupCustomizationVariables. Owner: Push from My Tawala, then update public Library.

---

## Exact tray state (Sep 8)

### Samples — `3-Ready-for-Public-Library/Samples/`

| File | Status |
|------|--------|
| `Survey Sample.json` | **Live Library Sep 9** (overwrite Simple Survey) |
| `Multiple Question Survey Sample.json` | **Live Library Sep 9** (overwrite Simple Multiple Question Survey). Drop Report as a **start** (keep Report form; loop still reaches it). TD door = Survey. |
| `Sign-up Sheet Sample.json` | **Live Library Sep 9** |
| `Horses and Penguins Test.json` | **Live Library — leave as-is** (owner Sep 9; do not re-Publish or rewrite the blurb) |
| `Get Together Sample.json` | **Parked Sep 9** → `_set-aside/`. Not a Library Sample. Operate **Get Together** is Live Library `b6do4s50iq64vl8` (Sep 15 overwrite of Simple). |
| `Potluck Sample.json` | **Parked Sep 9** → `_set-aside/`. Not a Library Sample. Operate **Potluck** is Live Library `52ozm3kqd58zlss` (Sep 15 overwrite of Simple). |

### Smoked real apps — `2-Ready-for-MyTawala/Smoked-Ready-to-Push/`

Public names (drop Sophisticated). Library **Test Drive OK** except ALB:

| File | TD door | Notes |
|------|---------|--------|
| `Single Question Poll or Survey.json` | **AdminStart** (single door) | **Library TD smoked Sep 14.** My Tawala `b7ttti35vdo58pu`. Public Library `ca7ws92mpss0sb5` (Redeployed leak-fix onto that uniqueId). Do not overwrite Survey Sample. Do not enable Start picker. |
| `Sign-up Sheet.json` | **Customize** (also Questionnaire + Administration) | Keeper uniqueId `gnqp5qfd06ktec3`. Leftover My Tawala row deleted Sep 9. Dual-use like Poll: Customize is the TD hub. |
| `Shared To-Do Test Drive.json` | **Setup** only | Smoke artifact — **not** the public listing. Live Library is the three-start app (`shared-to-do`, uniqueId `bx44wpdnspbsi3k`). Website Test Drive uses **Start picker**. |
| `Automated List Builder.json` | **Setup** | Typo pass + private Push Sep 8. Display name **Automated List Builder**. Live copy is uniqueId `vrjayw9lwun85i1` (Tomcat still parked as Smoke so we do not overwrite public occupant `ceihmyxlssxn6yn`). Email warning = Library listing honesty, not in-form. **ALB stays My Tawala.** |
| `Sophisticated Get Together.json` | **Start picker** (Customize / Questionnaire / Administration) | **Live Library Sep 15** as **Get Together** `b6do4s50iq64vl8`. My Tawala `ry35281dufqqrqq`. File→New simple stays Designer. |
| Sophisticated Potluck (keeper) | **Start picker** (Setup / Organizer / Administration) | **Live Library Sep 15** as **Potluck** `52ozm3kqd58zlss`. My Tawala `7egcx52fn4w0fqo`. File→New simple stays Designer. |

### Still in prep — `2-Ready-for-MyTawala/`

| File | Status |
|------|--------|
| `Sophisticated Shared To-Do.json` | **Owner smoked + Pushed + Save As Sep 9.** Setup + Signup + Administration. uniqueId `qggyhqoy8m4td23`. TD copy smoke passed. Live Library listing is the three-start app (`bx44wpdnspbsi3k`). |

Also: older junk at `3-Ready` root (CYO, Dirtbowl Communicator, MVSC, etc.) — **not** this startup-app pass.

---

## Product rules locked this week

| Rule | Meaning |
|------|---------|
| Sample vs Real vs Builder | **Sample** = that is all it is (looping Library demo). **Real** = Customize/Setup/Admin (drop Sophisticated on publish). **Builder** = Tawala Administrator designation (OEB first; more later). Users Publish **content** from a Builder, never a second copy of the Builder. Do not call a real/TD-sibling app a Sample. |
| Library-bound | Everything that works as a Test Drive is **on the plan to move to public Library** — Samples, dual-use real apps, Builders. |
| **GT + Potluck** | **Live Library Sep 15** as operate apps (overwrite Simple uniqueIds). TD = **Start picker**. File → New simples stay Designer samples. Parked looping Sample JSONs stay in `_set-aside/`. |
| Double smoke | If the Library TD shape is not the same JSON as the My Tawala working copy, smoke **both**: TD door **and** the multi-start deploy. Poll / Sign-up = one JSON, both roles. **Shared To-Do / Get Together / Potluck** = one JSON (three starts); Library TD uses **Start picker**. The Setup-only `Shared To-Do Test Drive.json` is a smoke artifact, not the listing. |
| Test Drive wipe | Leaving a start **wipes** TD data. Multi-start is for **My Tawala Use**, not hopping doors in one TD. |
| Library TD door | Single hub when that is enough (Poll AdminStart; Sign-up Customize). **Start picker** for Shared To-Do, Get Together, Potluck. |
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

**Owner Sep 14:** keep ALB **off Library** until Confirm links work both directions (public hostname in invitation URLs; snowball from My Tawala `/p/`, not `/t/`). Stay on `vrjayw9lwun85i1`. Do not overwrite public occupant `ceihmyxlssxn6yn`.

**Not done:** retire/hatch `ceihmyxlssxn6yn`. Extra JSON copies (Sophisticated / Smoke filenames) **deleted Sep 8** — only `Smoked-Ready-to-Push/Automated List Builder.json` remains. Recovery path is the old `.tawala`, not a JSON twin.

---

## Poll — revisit (Sep 14)

**What used to work:** two roles, two doors. Organizer: **AdminStart** → Administration dashboard. Respondents: **Start Questionnaire** (survey link) → thank-you. Email notify to the organizer still links Administration.

**What broke it:** a single-portal Test Drive attempt. Respondent **Questionnaire Done** grew “see the results so far → Administration,” and Administration was left as a start. Anyone who took the poll could walk into the dashboard. That is the Admin/User leak.

**Keeper already undoes that reshape** (`Current Deployed MyTawala/Single Question Poll or Survey.json`, uniqueId `b7ttti35vdo58pu`):

- Questionnaire Done = “Thank you. Your response has been recorded.” No Administration invitation.
- Administration `startPoint: false`.
- `Admin?` flag: organizer “Test your Survey” returns to the dashboard; a real respondent does not.

**Owner smoke Sep 14 (still leaked):** Use → **Start Questionnaire** on live `b7ttt` still opened the Administration menu at the end of the poll. Live XML already had thank-you-only. The remaining leak is **`Admin?` sticky on `/p/`**.

**JSON fix in keeper, Pushed to My Tawala `b7ttt`.** Website Publish replace onto `ca7ws92mpss0sb5` **did not Redeploy Tomcat** (listing uniqueId stayed Sep 9 XML). **Redeployed Sep 14 22:16 UTC** onto `ca7ws…` from keeper (same leak-fix). Purge Library responses after Redeploy so TD stays empty.

**Still leftover from the TD experiment (do not keep as product):**

- Same-session hop AdminStart → Start Questionnaire can still mix roles. **Do not enable Start picker** on this listing. Library TD = AdminStart only; **Test your Survey** is the Admin preview of the poll.

**Library Test Drive (locked Sep 14):** **single door, AdminStart only.** Owner smoked: passes. Poll-takers are not the TD audience (wall is My Tawala Use → Start Questionnaire). Organizer **Test your Survey** is how an Admin sees the poll, then returns to Administration. Leave-wipe grace **60s**. Do **not** turn on Start picker. Do **not** reshape JSON so one portal is both Admin and respondent.

**Sep 14 evening:** Quit **Thanks** is no longer “Thanks for Test Driving…”. `website-mock/projects/mytawala/Single Question Poll or Survey.json` is the leak-fixed dump without live uniqueId stamps (replaced the Customize + Administration-invitation copy).

---

## Designer (git) — parked

- Form Text blank/Style — done
- **C14** FIB Above + `*`; **C15** Pre-populate / Block Back; **F1** 3-browser — later
- **Process script opening-quote wrap** (Sep 4 polish) — noted in `DESIGNER_OPEN_TODOS.md` + shot in assets

---

## Next — Sep 11

**Locked (owner Sep 11):** Stop reshaping Library JSON so Test Drive can be a single `/p/{libraryUniqueId}` door. That warps apps (Sign-up TD skipped Customize), risks visitor-vs-visitor data mixing, and risks client/admin jump-tracks. Legacy TD was a **per-drive temporary instantiation** (session world on `/t/…`, not the live `/p/` uniqueId). Restore that isolation **before** more dual-use Publishes. Admin **Start picker** may stay as a rare opt-in; it is not how the catalog stays honest.

Smoke of Customize-first apps via My Tawala **Use** does **not** count as Library TD smoke.

**First group is in Live Library (owner Sep 9):** Survey Sample, Multiple Question Survey Sample, Sign-up Sheet Sample, Horses and Penguins Test. Horses stays as published. **Shared To-Do** is also Live (`bx44wpdnspbsi3k`).

1. **First: Test Drive sandbox** — **wired Sep 11.** Library Test Drive / Copy link → `/projectmanager/testdrive?id=&form=` (session World → `/t/…`). Default door still a single start (Customize/Setup/Admin heuristics; picker opt-in). Do not rewrite Sign-up / ALB / GT JSON for a single `/p/` door. Then Publish dual-use apps **as they already are**.
2. **Then** Publish dual-use apps **as they already are** (Sign-up Sheet operate `gnqp5qfd06ktec3`). **ALB stays My Tawala** until public Confirm links work.
3. **Poll** — **Library TD smoked Sep 14** (AdminStart single door on `ca7ws…`). Poll v.8 still Defer-rewrite.
4. **GT + Potluck** — **Live Library Sep 15** (operate overwrite of Simple uniqueIds; picker TD). Parked looping Sample JSONs stay in `_set-aside/`. File → New simples stay Designer.
5. Later: Designer C14/C15/F1. Keepers in `Current Deployed MyTawala`.

### Parked — User Contributed “projects of projects” (owner Sep 11)

A teacher could Publish **American History Final Exam** from OEB: same Designer definition as Online Exam Builder, **keep administrator/authoring data** (questions, setup), **strip student/respondent data**. Library would hold both **tools** (blank OEB) and **content products** (filled OEB instances). Teachers build in OEB, not Designer.

**In session Sep 15** — see § **D**. Product confirmed; public rollout is **Version 2**. Version 1 provision allowed (data only). Do not revive the Aug 26 same-uniqueId “keep Question / SetupVariables, purge Exam” path.

---

## Resume after 11 Nov 2026

School-exam GTM and OEB “Publish as an exam” Version 2 are **parked**. Isolation rules in this file still stand. Full pivot: [`HANDOFF_NOV11_2026.md`](HANDOFF_NOV11_2026.md).

---

## New-chat opener

```text
Library / Samples pass — resume from docs/LIBRARY_WHERE_WE_ARE_SEP3.md.
Branch cursor/forms-canvas-wysiwyg. Staging outside git: Website Staging.
**A + B closed Sep 14.** **Get Together + Potluck Live Library Sep 15** (operate overwrite of Simple uniqueIds; picker TD). **ALB stays My Tawala.** Real login = pre-live HOLD (#21), not this pass.
Section D confirmed: two-tier is **Version 2** public. Version 1 Builder hooks landed Sep 15 (catalog only; private OEB Use unchanged).
No Designer C14 this pass.
```

---

## Fri Sep 4 evening close (pre-travel) — archived

**Owner travel:** Saturday → ~1 week. Left mid-smoke Fri Sep 4 ~11:15.

Samples then: Survey / Multi-Q / Sign-up smoked; Horses not copied; GT/Potluck Samples not built.  
List Builder Push blocked on occupant `ceihmyxlssxn6yn` vs tray `6jpbj1hn2yzghh9`.
