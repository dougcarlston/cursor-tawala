# Static assets patched into ROOT.war at Docker build time

## `images/silk/`

FamFamFam Silk icons used by Java display components (e.g. `QuestionCorrelationTable`):

| File | Used for |
|------|----------|
| `tick.gif` | Selected date / choice |
| `star.png` | Preferred choice; best-option marker on totals row |
| `bin.gif`, `stop.png` | My Tawala project manager controls |

Source: [markjames/famfamfam-silk-icons](https://github.com/markjames/famfamfam-silk-icons) (CC BY 2.5).

## `images/submit-progress.gif`

Animated spinner shown in the **“Processing, please wait…”** modal on form submit (`web/scripts/project/default.js` references `/images/submit-progress.gif`). Not bundled in build-1700 `ROOT.war`; patched from YUI `loading.gif` so the wait panel is not a broken image.

## `images/checkbox_on.gif` / `images/checkbox_off.gif`

12×12 view-only checkbox glyphs used by Java `Checkbox.toHtml(..., selectedValues)` when **DISPLAY MCQ RESPONSES** (`display-mcq-label`) is in **`all_choices`** mode. Missing from build-1700 `ROOT.war`; without them the runtime shows broken-image icons and overlapping alt text (“Not selected”). Also copied into `TawalaWebapp-build1700/web/images/` for WAR rebuilds.

## `css/project/`

Project theme styles loaded per `themePath` in `.tawala` XML.

| Path | Templates using it |
|------|-------------------|
| `default.css` + `default/` (implicit) | Get Together, Form with process |
| `greentea/` | Potluck |
| `style2/` | Simple Survey |
| `baseball/` | Sign-up Sheet (stub CSS until full theme transfer) |
| `mvsc/` | SignupSheets sample (`themePath=mvsc`) — local stub so Send/confirmation render does not 404 |
| `dirtbowl2/` | DirtBowl registration |

`default.css` copied from owner legacy **Project Themes Test** bundle. Theme-specific `project.css` + `images/` under `greentea/` and `style2/` from the same source.

Rebuild after changes:

```bash
docker compose build tawala && docker compose up -d tawala
```
