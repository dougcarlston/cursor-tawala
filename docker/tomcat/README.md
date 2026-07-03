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

## `css/project/`

Project theme styles loaded per `themePath` in `.tawala` XML.

| Path | Templates using it |
|------|-------------------|
| `default.css` + `default/` (implicit) | Get Together, Form with process |
| `greentea/` | Potluck |
| `style2/` | Simple Survey |
| `baseball/` | Sign-up Sheet (stub CSS until full theme transfer) |
| `dirtbowl2/` | DirtBowl registration |

`default.css` copied from owner legacy **Project Themes Test** bundle. Theme-specific `project.css` + `images/` under `greentea/` and `style2/` from the same source.

Rebuild after changes:

```bash
docker compose build tawala && docker compose up -d tawala
```
