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
| **Image** | Large preview (~520×220); **Browse…** — pick GIF/JPG/PNG from PC; **Remove** — clear image (no in-dialog crop/resize — Deploy uses full image + theme `object-fit: cover`) |
| Footer | **OK**, **Cancel** |

Dialog chrome (browser): **opaque** solid fill (~645px wide); form text must not show through.

On **OK**, saves to `project.pageHeader` + optional `project.images` entry id `__HEADER__` (full-resolution imagedef). Deploy XML: `<pageHeader><text>…</text><image id width height/></pageHeader>` plus `<imagedef>`.

### Runtime use

- Java / Deploy: form pages prepend `<h1 class="pageHeading">` (image + text div) — `DataCollectingProjectController` / `FormPreviewController`.
- Browser Preview / Node runtime: same markup at the top of the form page.
- **Send** → **Include Page Header** enabled when the project has header text or image (`DESIGNER_PROCESS_STATEMENTS_SEND.md`).

### Browser (`designer-web`) — Jul 24–28, 2026

**Done:** dialog, JSON schema, `.tawala` import, Deploy export, Preview banner, Send checkbox gate, Design-canvas stand-in; opaque/larger dialog.

**Parked:** in-dialog pan/crop/resize (baking to banner size made Deploy banners soft). Deploy keeps the full photo and theme cover crop.

**Design canvas:** When Page Header has text and/or image, Form Design shows a top row with chip **`<<Project Header>>`** (optional thumb + title text). Click opens the dialog. This is **not** a Form Heading item — authors may still add Main/Sub headings below the banner.

**Not in this pass (owner note):** dropping a Page Header block into a **Form Text** item or **Document** as reusable content — park for later; project-level banner is the legacy contract.

### Smoke

1. **Project → Page Header…** → dialog is **opaque**; type a title → Browse a PNG → OK.
2. Form **Design** → top row shows `<<Project Header>>` (click reopens dialog); Form Heading items can sit below.
3. Form **Preview** → short banner (~160px) with **text over the image** (sharp full-res source + `object-fit: cover`).
4. **Redeploy** → same on 8080 (`h1.pageHeading`; hard-refresh CSS if needed).
5. Process **Send** → **Include Page Header** enabled.
6. Unit: `cd designer-web && npm test -- --run src/lib/pageHeader.test.ts server/jsonToXml.test.mjs`

---

## Source

- `TawalaDesigner/Code/TAWALA/DesignerUI/Dialogs/PageHeaderDialog.cs`
- `TawalaDesigner/Code/TAWALA/Projects/PageHeader.cs`
- `TawalaWebapp-build1700/src/com/tawala/project/PageHeader.java`

---

*Last updated: July 28, 2026.*
