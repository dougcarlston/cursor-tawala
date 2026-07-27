# CSS Themes Audit — 2026-07-27

Owner ask: find CSS/theme material in archives, confirm the "~28 themes historically" suspicion, and compare Designer's theme list vs local CSS vs archives.

## Headline finding: the 28 is confirmed, and it's a known, already-tracked gap

`TawalaWebapp-build1700/web/WEB-INF/theme-config.xml` (legacy Java `CommonTheme` bean list) defines **exactly 28 visible themes** (plus one hidden 29th, `setup`, used only for the Customization Wizard, `visible="false"`).

`designer-web/src/lib/projectThemes.ts` already mirrors this **exactly** — 28 entries, alphabetical by label, matching the C# `SortedDictionary` order. Designer's theme list is **complete and correct**; nothing to "fix" there.

The real hole is CSS coverage: only **7 of 28** themes have CSS shipped under `docker/tomcat/css/project/`. This exact 7-vs-28 gap is already documented as **owner-flagged TODO #15** in `Tawala_Key_Documents/DESIGNER_OPEN_TODOS.md`.

**New in this audit:** all 21 missing themes' full CSS packs (project.css + images) were located intact in the from-Tony archive dump, in `web.zip` → `web/css/project/{theme}/`. Nothing needs to be reconstructed from scratch — it just needs to be copied in.

## Where things live

| Location | Contents | Theme-like children |
|---|---|---|
| `Tawala/AANew Files about CSS/css/project/` | Doug's own earlier CSS snapshot + `.svn` metadata | 24 dirs (see below) |
| `Tawala/AANew Files about CSS/Tawala-format-test/` | HTML smoke pages: `default-test.html`, `clock-test.html`, `lime-test.html`, `orange-test.html`, `money-test.html`, `coffeecup-test.html` | test harness, not a theme source |
| `Tawala/AANew Files about CSS/Project Themes Test/` | HTML smoke pages: `test-default`, `test-tincarbell`, `test-talkingtomiceelf`, `multi-palette`, `test-rangeofchocolate`, `test-greentea`, `test-fullmoon` | test harness, not a theme source |
| `00-Tawala-ARCHIVES.../from-Tony-website-library-css-dump/NEWZIPS/web.zip` (`web/css/project/`) | **Full legacy Tomcat webapp CSS bundle** — the authoritative theme CSS source | **30 dirs** (28 official + hidden `setup` + stub `white`) |
| `00-Tawala-ARCHIVES.../repo-backup-approx-Jul11-2026/docker/tomcat/css/project/` | Snapshot of a slightly-older `docker/tomcat/css/project` | 6 dirs (subset of live) |
| `Tawala/docker/tomcat/css/project/` (live, in use) | What Deploy/Preview actually serves today | **7 dirs**: `baseball`, `default`, `dirtbowl2`, `greentea`, `mvsc`, `redrays`, `style2` |
| `TawalaWebapp-build1700/web/WEB-INF/theme-config.xml` | Java `CommonTheme` bean definitions — source of truth for the "28" | 28 visible + 1 hidden (`setup`) |
| `TawalaWebapp-build1700/src/.../theme/UserDefinedTheme.java` | Per-user custom themes (DB-backed, parented to a `CommonTheme`) — unrelated to the 28-pack question | n/a |
| `designer-web/src/lib/projectThemes.ts` | Designer's theme menu source | 28 (matches `theme-config.xml` exactly) |

No top-level `AANew Files about CSS` exists directly under `~/Projects/` — only under `~/Projects/Tawala/AANew Files about CSS/` (already in the live repo, not archived). No zips were found needing extraction inside that folder itself; the one dump-level zip worth opening (`web.zip`) lives in the archives folder and was inspected via `unzip -l` / `unzip -p` without extracting (kept originals untouched, no `_extracted` folder was needed).

## Theme comparison table

Legend: ✅ yes · ❌ no · — n/a

| Theme (label) | path | In Designer list (28)? | Has local CSS (docker)? | Found in `web.zip` archive? | Found in AANew Files? |
|---|---|:---:|:---:|:---:|:---:|
| Baseball | `baseball` | ✅ | ✅ | ✅ | ✅ |
| Basic Blue | `basicblue` | ✅ | ❌ | ✅ | ✅ |
| Basic Green | `basicgreen` | ✅ | ❌ | ✅ | ✅ |
| Basic Pink | `basicpink` | ✅ | ❌ | ✅ | ✅ |
| Basic Yellow | `basicyellow` | ✅ | ❌ | ✅ | ✅ |
| Big Q | `style2` | ✅ | ✅ | ✅ | ✅ |
| Blue Lined Paper | `blueline` | ✅ | ❌ | ✅ | ✅ |
| Chocolate | `chocolate` | ✅ | ❌ | ✅ | ❌ |
| Dark | `dark` | ✅ | ❌ | ✅ | ✅ |
| Default | `default` | ✅ | ✅ | ✅ | ✅ |
| Dirtbowl | `dirtbowl` | ✅ | ❌ | ✅ | ❌ |
| Dirtbowl - Variable Width | `dirtbowl2` | ✅ | ✅ | — (n/a, added post-archive) | ❌ |
| Full Moon | `fullmoon` | ✅ | ❌ | ✅ | ❌ |
| Green Lined Paper | `greenline` | ✅ | ❌ | ✅ | ✅ |
| Green Tea | `greentea` | ✅ | ✅ | ✅ | ❌ |
| Light Green | `litegreen` | ✅ | ❌ | ✅ | ✅ |
| Lime | `lime` | ✅ | ❌ | ✅ | ✅ |
| MVSC | `mvsc` | ✅ | ✅ | ✅ | ❌ |
| Orange Swirl | `orangeswirl` | ✅ | ❌ | ✅ | ❌ |
| Plain | `plain` | ✅ | ❌ | ✅ | ✅ |
| Purple Haze | `purplehaze` | ✅ | ❌ | ✅ | ❌ |
| Red | `red` | ✅ | ❌ | ✅ | ✅ |
| Red Rays | `redrays` | ✅ | ✅ | ✅ | — (n/a) |
| Salzburg | `salzburg` | ✅ | ❌ | ✅ | ❌ |
| Soup's On | `soup` | ✅ | ❌ | ✅ | ❌ |
| Tennis | `tennis` | ✅ | ❌ | ✅ | ❌ |
| Tin Car Bell | `tincarbell` | ✅ | ❌ | ✅ | ❌ |
| Yellow | `yellow` | ✅ | ❌ | ✅ | ✅ |
| *(hidden)* Setup for Customization Wizard | `setup` | ✅ (hidden) | ❌ | ✅ (empty-ish, 434B) | ✅ |
| *(not in 28-list)* White | `white` | ❌ | ❌ | ✅ (but **0-byte stub**, no real CSS) | ✅ |
| *(not in 28-list)* Clock / Coffeecup / Fabric1 / Lightbulb / Money / Orange / Pinkrose / Whiteflowers | various | ❌ | ❌ | ❌ | ✅ (8 extra, one-off/test names, not part of the official catalog) |

**Bottom line counts:**
- Designer lists: **28** (correct, complete, matches legacy Java exactly)
- Have local CSS today: **7 / 28**
- Have full CSS available in `web.zip` archive: **28 / 28** (100% — every missing theme's `project.css` + `images/` were found intact)
- `AANew Files about CSS` extra names (`clock`, `coffeecup`, `fabric1`, `lightbulb`, `money`, `orange`, `pinkrose`, `whiteflowers`, plus stub `white`): earlier/experimental theme names **not** in the official 28-theme catalog — likely predate or were superseded by the current set; not needed to close the gap.

## Recommendation

1. **High-confidence, low-risk next step:** copy the 21 missing theme folders (`basicblue`, `basicgreen`, `basicpink`, `basicyellow`, `blueline`, `chocolate`, `dark`, `dirtbowl`, `fullmoon`, `greenline`, `lime`, `litegreen`, `orangeswirl`, `plain`, `purplehaze`, `red`, `salzburg`, `soup`, `tennis`, `tincarbell`, `yellow`) from `web.zip` → `web/css/project/{theme}/` into `docker/tomcat/css/project/{theme}/`, one-to-one by folder name — no renaming or mapping needed, names already match `projectThemes.ts` `path` values exactly.
   - This directly closes owner TODO #15 (`DESIGNER_OPEN_TODOS.md`): once copied, `LOCAL_CSS_PATHS` in `designer-web/src/lib/projectThemes.ts` can be expanded from 7 to 28 and the grey/blue distinction in the theme menu goes away for these.
   - This is a **Preview/Deploy CSS-asset copy**, not a Design canvas change — safe per work-scope rules, but still recommend a manual smoke pass (open a couple of newly-added theme pages on 8080) before flipping `LOCAL_CSS_PATHS`.
2. Do **not** bother with the 8 extra `AANew Files about CSS` names (`clock`, `coffeecup`, etc.) or `white` — they aren't part of the legacy 28-theme catalog and `white`'s CSS is an empty stub even in the archive.
3. Left as a report only per instructions — **no CSS was copied into `docker/` in this pass.** Say the word and this can be done as a small, reviewable follow-up (21 folder copies + a one-line `LOCAL_CSS_PATHS` update).
