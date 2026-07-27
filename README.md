# cursor-tawala

A Cursor rendition of the Tawala app.

**This is the ACTIVE, live git repo.** Designer runs from here. Everything in this
folder is active/working content — nothing here is historic/archived.

## Where things are

- Browser Designer app: `designer-web/` (run with `cd designer-web && npm run dev`,
  served at http://localhost:5173, dev API at http://localhost:3001)
- Legacy C# Designer source (authoritative UI/XML behavior): `TawalaDesigner/`
- Legacy Java webapp: `TawalaWebapp-build1700/` (Tomcat runtime at http://localhost:8080)
- Owner specs / Designer documentation: `Tawala_Key_Documents/`
- Docker/Tomcat setup: `docker/`
- Deploy & dev helper scripts: `scripts/`

## Library / JSON workspace (outside this repo)

Reconverted project JSON and library sorting work lives in a sibling folder, **not**
inside this git repo:

`/Users/DougC1/Projects/Tawala Projects/Being Reconverted/`

(see the README there — includes `SportsDashboardsTemplateVersion3.json` and other
in-progress reconverts; `00-PRIOR-do-not-use/` inside that folder is itself an older
archive, left as-is).

## Archived / historic content (do not use for active work)

Old backups, duplicate dumps, and empty stubs that used to live inside or next to this
repo have been moved out to keep this tree easy to navigate:

`/Users/DougC1/Projects/00-Tawala-ARCHIVES-do-not-use-for-Designer/`

See the README there for what each subfolder is and where it came from. Nothing was
deleted — only moved.
