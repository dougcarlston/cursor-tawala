# Designer — Page Header (project banner)

Captured from legacy **Tawala Project Designer #251 (DEV)** screenshot and owner notes (June 2026).

Screenshot: `assets/Format_-_Page_Header-*.png`.

**Not the same as the Heading form item** (`DESIGNER_FORM_ITEMS_HEADING.md`).

| | **Heading** (form item) | **Page Header** (project setting) |
|---|-------------------------|-----------------------------------|
| How you add it | **Items** palette → **Heading** → type in `[Replace this with heading of your own.]` | **Format → Page Header…** |
| Scope | One section on the **current form** | **Whole project** — banner on deployed pages |
| Main/Sub | Heading Type dropdown | N/A |
| Image | No | Optional image via **Browse…** |

---

## Format → Page Header…

**Dialog title:** `Page Header`

| Section | Controls |
|---------|----------|
| **Text** | Single-line text box (empty when first opened) |
| **Image** | Large preview rectangle (empty until image chosen); **Browse…** (folder icon) — pick image from PC; **Remove** (red X) — clear image |
| Footer | **OK**, **Cancel** |

On **OK**, saves to `Project.PageHeader` (XML `<pageHeader>`): header text + optional embedded image.

### Runtime use

- Appears at top of **deployed** form/document pages when included.
- **Show** / **Send** statements may offer **Include Page Header** when project has header content (`DESIGNER_PROCESS_STATEMENTS_SHOW.md`, `DESIGNER_PROCESS_STATEMENTS_SEND.md`).

---

## Source

- `TawalaDesigner/Code/TAWALA/DesignerUI/Dialogs/PageHeaderDialog.cs`
- `TawalaDesigner/Code/TAWALA/Projects/PageHeader.cs`

---

*Last updated: June 2026.*
