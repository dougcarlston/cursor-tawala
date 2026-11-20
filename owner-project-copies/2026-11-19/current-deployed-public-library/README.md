# Current Deployed Public Library — Live-only tray

JSON here is a **dump of the live Tomcat Library uniqueIds** (`:8080` / Postgres `project_definition`), converted with the same XML→JSON path as Designer File → Open.

**Rebuilt 2026-09-14** from live uniqueIds (not Designer Save As); **Get Together / Potluck overwritten Sep 15** (operate Publish onto the old Simple uniqueIds). Each file stamps `deployUniqueId`, `deployIdentityName`, `_dumpedAt`.

## Live set (Sep 14)

| File | Library uniqueId | Tomcat name | Notes |
|------|------------------|-------------|-------|
| `Online Exam Builder.json` | `u3hkqgwtrepjlur` | Online Exam Builder | Pushed/Published Sep 14 |
| `Shared To-Do.json` | `bx44wpdnspbsi3k` | Shared To-Do bd155bbc | Live Sep 9 |
| `Single Question Poll or Survey.json` | `ca7ws92mpss0sb5` | …63bc7d5b | Leak-fix Redeployed Sep 14 |
| `Horses and Penguins Test.json` | `wg77ytn0bgq1x70` | Horses and Penguins Test | Leave as published |
| `Survey Sample.json` | `gy1zssbrwm4fgfm` | Simple Survey Template | Catalog overwrite of Simple Survey |
| `Multiple Question Survey Sample.json` | `grniytf6dvmobqe` | Multiple Question Survey Template | Catalog overwrite |
| `Sign-up Sheet Sample.json` | `kxlgkzkqmaebhz3` | Sign-up Sheet Sample | Looping sample |
| `Sign-up Sheet.json` | `ffawe1uxeudjwpr` | Sign-up Sheet 49a8b2d5 | Operate dual-use (Customize + Questionnaire + Administration) |
| `Get Together.json` | `b6do4s50iq64vl8` | Get Together Template | **Operate overwrite Sep 15** (Customize + Questionnaire + Administration). File→New simple stays Designer-only. |
| `Potluck.json` | `52ozm3kqd58zlss` | Potluck | **Operate overwrite Sep 15** (Setup + Organizer + Administration). File→New simple stays Designer-only. |

**Not dumped:** Automated List Builder (My Tawala only until Confirm links work). My Tawala operate copies (`un7q…`, `b7ttt…`, `gnqp5…`) stay in `Current Deployed MyTawala/`.

Re-dump from repo root:

```bash
node scripts/dump-live-library-json.mjs --out "$HOME/Projects/ Current Tawala Projects/Current Deployed Public Library" \
  u3hkqgwtrepjlur:"Online Exam Builder.json" \
  bx44wpdnspbsi3k:"Shared To-Do.json" \
  ca7ws92mpss0sb5:"Single Question Poll or Survey.json" \
  wg77ytn0bgq1x70:"Horses and Penguins Test.json" \
  gy1zssbrwm4fgfm:"Survey Sample.json" \
  grniytf6dvmobqe:"Multiple Question Survey Sample.json" \
  kxlgkzkqmaebhz3:"Sign-up Sheet Sample.json" \
  ffawe1uxeudjwpr:"Sign-up Sheet.json" \
  52ozm3kqd58zlss:"Potluck.json" \
  b6do4s50iq64vl8:"Get Together.json"
```

XML→JSON is lossy-with-warnings (Send, some itemization tables). For a lossless round-trip, Designer File → Open the live uniqueId is still better — this tray is the cheap keeper.

The old Aug 4 `Live Library Projects/` template copies were removed Sep 14 — those apps still exist as the JSON in this folder’s root (and on live Tomcat).
