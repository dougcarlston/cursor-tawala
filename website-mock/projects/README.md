# Project JSON (Library vs My Tawala) — mock is “out there”

These folders are the **website-mock catalogs**. They represent what the local mock site shows.
They are **not** your free-form design workspace.

Owner staging (on computer) lives **outside** git:

`~/Projects/Tawala Projects/Website Staging/`

| Staging tray | Pushes toward mock |
|--------------|--------------------|
| `2-Ready-for-MyTawala/` | → `website-mock/projects/mytawala/` |
| `3-Ready-for-Public-Library/` | → `website-mock/projects/library/` **only after** My Tawala |

Product rule: **My Tawala first**; Public Library is a later promote (not a direct first landing).

| Catalog | Mock path | Meaning |
|---------|-----------|---------|
| **My Tawala** | `mytawala/` | Personal / account projects |
| **Public Library** | `library/` | Public try-outs + category |

## How the mock uses these

- Catalog + test-drive URLs: `website-mock/js/demo-urls.js`
  - `TAWALA_LIBRARY` → Library / home
  - `TAWALA_MYTAWALA` → My Tawala listing / details
- Designer New Project templates stay in `designer-web/public/samples/templates/` (not these piles).

## Refresh from staging (examples)

```bash
# My Tawala gate
rsync -a --exclude='.DS_Store' --exclude='README.md' --exclude='.gitkeep' \
  "$HOME/Projects/Tawala Projects/Website Staging/2-Ready-for-MyTawala/" \
  website-mock/projects/mytawala/

# Public Library promote (after My Tawala)
rsync -a --exclude='.DS_Store' --exclude='README.md' --exclude='.gitkeep' \
  "$HOME/Projects/Tawala Projects/Website Staging/3-Ready-for-Public-Library/" \
  website-mock/projects/library/
```

Then update `js/demo-urls.js` if names were added/removed, and redeploy for `:8080` test-drives when needed.

Older local sources `WebLibrary/` / deleted-path `MyTawala/` may still appear in old MANIFEST dates; prefer **Website Staging** going forward.
