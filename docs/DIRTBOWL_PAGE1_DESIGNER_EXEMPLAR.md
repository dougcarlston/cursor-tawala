# DirtBowl Registration Page 1 → Designer exemplar triage

**Date:** Aug 20, 2026 (owner walkthrough)  
**Purpose of DirtBowl here:** Stress-test / ceiling sample of formatting sophistication authors want from **stock Designer** — **not** a second theme pack or a recipe other projects should copy. Prefer general tools (and optional Insert presets) over Registration-only special cases (`registrationLayout`, DirtBowl-gated CSS).

**Related:** `docs/DESIGNER_BACKLOG_ARCHITECTURE.md` (earlier DirtBowl stress → shell architecture); `docker/tomcat/css/project/dirtbowl2/`; `designer-web/server/registrationLayout.mjs`.

**Themes:** Page 1 layout CSS lives on **Dirtbowl - Variable Width** (`dirtbowl2`). Classic **Dirtbowl** (`dirtbowl`) is a different fixed-width skin and is **not** dual-maintained for this layout.

---

## Triage key

| Bucket | Meaning |
|--------|---------|
| **Version 1 if possible** | Aim to ship in the current product cut when capacity allows |
| **Next version** | Worth doing; not required to call Designer “basically finished” for V1 |
| **Not immediate** | Out of scope for now (hard for authors, wrong product shape, or parked epic) |

---

## Version 1 if possible

| Item | Notes |
|------|--------|
| **Theme content width fit** | Fields must not spill into the page margin. **Product CSS:** `form-layout-core` + `--tawala-form-content-max` (themes set the variable; dirtbowl2 supplies sizes only). |
| **Shared field-column right edge** | Name / School / Email / **Parent phones** share one right margin. **Product CSS** in `form-layout-core` (not dirtbowl2 geometry). |
| **Inline hint formatting** | Parentheticals in the FIB question (`(mm/dd/yyyy)`, `(again)`) with B/I/size/color — **mostly already covered**. Do not spend V1 unless broken. |
| **Form Text tables as layout (basic)** | **Done** — width pt/px parse; `#form table.user` borders; dirtbowl2 no longer strips all borders. **T4:** Design `<table>` content wins over hardcoded `registrationTextToXml` (stock tools). |
| **Per-blank captions above fields** | Small **First** / **Last** (etc.) tight above each input, left-aligned to that blank. **Not** FIB Styles → Above (whole question above row). **Not** only italic text in the question line. High-frequency DirtBowl pattern; structured mode preferred over free WYSIWYG drag. **Designer:** `blank.caption` on property strip; Design idle + Deploy `fibToXml` + Preview stack; product CSS in `form-layout-core`. |
| **Insert presets: Date + Address** | **Done** — Insert → Date / Address emit ordinary FIBs (`fibInsertPresets.ts`); Street caption on Address; DOB uses `mm/dd/yyyy` + `/`. |

**V1 spine (if cutting hard):** width fit → shared column (incl. phones) → per-blank captions → Date/Address presets → basic Form Text table Push fidelity.

---

## Next version

| Item | Notes |
|------|--------|
| **Responsive theme CSS** | `@media` / fluid max-width so a tablet-ish layout is less wrong on phone/desktop. Detection is easy; full multi-layout authoring is not (see Not immediate). |
| **Returns + spacing as primary composition** | Prefer author-controlled soft-rows over auto-detected “Parent blocks.” Polish gap rhythm after captions land. |
| **Tables organizing FIB / field grids** | Author uses tables to place labels/blanks and nudge spacing — same family as Form Text table layout; no deduced data-block widget. |
| **Centered Form Text vs theme column** | Center today is relative to content/text width; authors may expect center on the full theme column. |
| **Get Record List Fields branch** | Named list branch in Fields after Get (multi-list mental model). `Record:` imputation / ForEach branches already cover the common case. |
| **Column-level `displayCondition` editor** | Per-column show/hide on MQL/itemization. Runtime already honors preserved rules; Design edit deferred. Item-level **Display conditionally…** is done. |
| **Form Cut / Copy / Paste** | Shell polish; parked earlier in Aug 20 catchup. |
| **DirtBowl Page 2+ as further exemplar** | Same purpose lens after Page 1 → Designer list is stable. Not “finish the DirtBowl product skin.” |
| **RANDOM function** (numeric / sample-from-list) | Owner Aug 21: e.g. teacher draws *n* questions from a large bank so each exam run shows a different set (anti-cheat). Insert/Process or Get+Where path TBD — not V1. |

---

## Not immediate

| Item | Notes |
|------|--------|
| **Join command** | Hard for non-programmers; nested ForEach is the awkward substitute — accept that. |
| **Auto-detected data blocks** | No “Parent block” widget deduced from fields. Composition (Returns / tables) instead. |
| **DirtBowl-only forever layout engine** | Exemplar → generalize; retire special-case paths over time where stock tools suffice. |
| **True Date / Address composite field types** | Prefer Insert presets that emit stock FIBs. |
| **Full WYSIWYG caption drag-placement** | Unless structured per-blank captions prove insufficient. |
| **Device output sizing** (computer / tablet / phone + autoswitch) | Parked Jul 24 — see `DESIGNER_OPEN_TODOS.md` § After the other two project branches. |
| **Deep T1–T3 convert archaeology** | Live `:8080` output is enough for the exemplar; Design may show structured-node fallback. Optional later. |
| **Classic `dirtbowl` theme parity for Registration layout** | Variable Width (`dirtbowl2`) is the skin for this layout. |

---

## Composition principle (Parent Q4 and similar)

Do **not** invent automatic “blocks of data.” Authors compose with:

1. **Returns / soft-rows** (granular; usually enough), and/or  
2. **Tables** (stronger column lock and spacing) once table fidelity is good enough.

Shared **column geometry** still applies across composed rows (including phone triples filling the field area).

---

## Page 1 feature map (walkthrough notes)

| Area | Exemplar takeaway |
|------|-------------------|
| Page chrome / width | Theme fit + optional later responsive CSS |
| T1–T3 banner / director | Multi-line Form Text, center, color, field tokens; convert UX optional |
| T4 info table | Form Text tables as layout |
| Q1 Name First/Last | Per-blank captions; shared column |
| Q1 DOB | Date preset; inline `(mm/dd/yyyy)` |
| Sex MCQ | Align with FIB label column (stock MCQ + theme) |
| Q3 School | Shared field area to common right edge |
| Q4 Parent unit | Composition, not a block type |
| Q4 Address line | Address preset |
| Q4 Email / (again) / optional | Inline hints + multi-blank |
| Q4 Phones | Shared right edge; wider boxes |
| Submit | Theme chrome (`Submit →` on dirtbowl2) |

---

## Owner decisions locked this session

- DirtBowl is an **exemplar**, not a style guide for Library authors.  
- **Join** out; optional **named Record List** in Get is fine; Fields list branch is next-version polish.  
- **Column-level** conditions in functions: not serious for V1 (runtime OK).  
- Date/Address: **presets emitting FIBs** are the only soft exception to “no special cases.”  
- Device **detect** width via CSS is fine for themes; **three authored layouts** stay parked.
