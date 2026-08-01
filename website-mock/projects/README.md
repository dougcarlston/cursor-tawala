# Project JSON backups (Library vs My Tawala)

Copied **2026-07-29** from the owner’s working piles (outside the git repo):

| Pile | Source | Repo path |
|------|--------|-----------|
| **Library / public try-outs** | `~/Projects/Tawala Projects/WebLibrary/` | `website-mock/projects/library/` |
| **Personal My Tawala** | `~/Projects/Tawala Projects/MyTawala/` | `website-mock/projects/mytawala/` |

Each folder has a `MANIFEST.json` (file list + sizes). Spelling: **SportsDashboards** (not SportsBoard).

## How the mock uses these

- Catalog + test-drive URLs: `website-mock/js/demo-urls.js`
  - `TAWALA_LIBRARY` → Library / home pages
  - `TAWALA_MYTAWALA` → My Tawala listing / project details
- Designer-only New Project items (**Empty/Blank**, **Form with Process**, **Form with Process & Document**, **Sign-up Sheet w Email**) are **not** in either pile and must **not** appear in the Library catalog. They stay File → New Project only (`designer-web/public/samples/templates/`). Sign-up Sheet w Email was retired from the public Library (owner Aug 1, 2026); re-Publish when a finished runtime-customizable version exists.
- Main Menu **public** templates (Simple Survey, Sign-up Sheet, Potluck, Get Together) remain in the Library catalog with live `:8080` URLs; their JSON lives under `designer-web/public/samples/templates/`, not in `projects/library/`.

## Refresh copies

```bash
rsync -a --exclude='.DS_Store' \
  "$HOME/Projects/Tawala Projects/WebLibrary/" \
  website-mock/projects/library/
rsync -a --exclude='.DS_Store' \
  "$HOME/Projects/Tawala Projects/MyTawala/" \
  website-mock/projects/mytawala/
```

Then update `js/demo-urls.js` if names were added/removed, and redeploy templates when you want new `:8080` test-drives.
