# Designer — Page Header (project banner)

Captured from legacy **Tawala Project Designer #251 (DEV)** screenshot and owner notes (June 2026). Updated Jul 24, 2026 for browser Designer wiring.

Screenshot: `assets/Format_-_Page_Header-*.png`.

**Not the same as the Heading form item** (`DESIGNER_FORM_ITEMS_HEADING.md`).

| | **Heading** (form item) | **Page Header** (project setting) |
|---|-------------------------|-----------------------------------|
| How you add it | **Items** palette → **Heading** → type in `[Replace this with heading of your own.]` | **Project → Page Header…** (legacy: Format → Page Header…) |
| Scope | One section on the **current form** | **Whole project** — banner on deployed / Preview form pages |
| Main/Sub | Heading Type dropdown | N/A |
| Image | No | Optional image via **Browse…** / **Remove** |
| Formatting | Theme Main/Sub only (palette greyed) | Plain text line (unformatted) |

---

## Project → Page Header…

**Dialog title:** `Page Header`

| Section | Controls |
|---------|----------|
| **Text** | Single-line text box (empty when first opened) |
| **Image** | Banner-aspect preview (~520×160) with **highlight frame** = what Deploy shows. **Browse…** GIF/JPG/PNG; **Remove**; drag to **pan**; corner handle **zoom**; side handle **stretch** sideways (no aspect lock). **Show top / center / bottom** + **Reset view**. OK bakes a Deploy crop **and keeps the full original** (`__HEADER_SOURCE__` + viewport) so you can Push, reopen, and keep adjusting. Live banner uses **`object-fit: fill`**. |
| Footer | **OK**, **Cancel** |

Dialog chrome (browser): **opaque** solid fill (~645px wide); form text must not show through.

On **OK**, saves to `project.pageHeader` + optional `project.images` entry id `__HEADER__` (baked crop PNG at source density). Deploy XML: `<pageHeader><text>…</text><image id width height/></pageHeader>` plus `<imagedef>`.

### Runtime use

- Java / Deploy: form pages prepend `<h1 class="pageHeading">` (image + text div) — `DataCollectingProjectController` / `FormPreviewController`.
- Browser Preview / Node runtime: same markup at the top of the form page.
- **Send** → **Include Page Header** enabled when the project has header text or image (`DESIGNER_PROCESS_STATEMENTS_SEND.md`).

### Browser (`designer-web`) — Jul 24–Aug 20, 2026

**Done:** dialog, JSON schema, `.tawala` import, Deploy export, Preview banner, Send checkbox gate, Design-canvas stand-in; opaque/larger dialog; **Aug 20:** pan/zoom/stretch; bake at source crop resolution; pan listeners stabilized; **Show top/center/bottom**; Deploy `object-fit: fill`; **OK keeps editable source** (reopen restores pan/zoom — does not freeze on bake).

**Design canvas:** When Page Header has text and/or image, Form Design shows a top row with chip **`<<Project Header>>`** (optional thumb + title text). Click opens the dialog. This is **not** a Form Heading item — authors may still add Main/Sub headings below the banner.

**Not in this pass (owner note):** dropping a Page Header block into a **Form Text** item or **Document** as reusable content — park for later; project-level banner is the legacy contract.

### Smoke

1. **Project → Page Header…** → dialog is **opaque**; type a title → Browse a PNG → drag/pan (or **Show top**) + stretch → OK.
2. **Push / Redeploy** → check live banner → reopen Page Header → pan still works on the **same** photo (not a frozen bake).
3. Form **Design** → top row shows `<<Project Header>>` (click reopens dialog); Form Heading items can sit below.
4. Form **Preview** → short banner (~160px) with **text over the image**; crop matches dialog (no extra top cut-off).
5. **Redeploy** → same on 8080 (`h1.pageHeading`; hard-refresh CSS if needed — `object-fit: fill`).
6. Process **Send** → **Include Page Header** enabled.
7. Unit: `cd designer-web && npm test -- --run src/lib/pageHeader.test.ts src/lib/pageHeaderImageFit.test.ts server/jsonToXml.test.mjs`

---

## Source

- `TawalaDesigner/Code/TAWALA/DesignerUI/Dialogs/PageHeaderDialog.cs`
- `TawalaDesigner/Code/TAWALA/Projects/PageHeader.cs`
- `TawalaWebapp-build1700/src/com/tawala/project/PageHeader.java`
- Browser fit math: `designer-web/src/lib/pageHeaderImageFit.ts`

---

*Last updated: August 20, 2026.*
