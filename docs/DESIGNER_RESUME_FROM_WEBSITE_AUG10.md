# Designer resume brief — from Website / Library (past ~13 days)

**For chat:** `Designer - Field Qualification & Signup`  
**Date:** Aug 10, 2026  
**Branch:** `cursor/forms-canvas-wysiwyg` (Library commit `7fcbf7f` and earlier)  
**Owner:** pasting this after Website/Library plate; smoke guest path himself before/while you work.

Copy from **Paste opener** downward into that chat.

---

## Paste opener

```
Project: Tawala (~/Projects/Tawala)
Track: Browser Designer — designer-web/ (resume after Website/Library plate)
Branch: cursor/forms-canvas-wysiwyg
Goal: Run parked Designer MUST DOs now that Library/My Tawala mock is in good shape — especially Deploy→Push rename + Theme applies on Push; then prior MUST DO Link conflation / Field Qualification work as owner directs
Read first:
  1. .cursor/rules/tawala-designer-parked-post-website.mdc  (Aug 10 MUST DOs + Jul 31 Link conflation)
  2. docs/CHAT_HANDOFF.md Chat 1 “Immediate phases ahead”
  3. website-mock/README.md § Designer Push rename checklist + Task #5 Theme HOLD
  4. This brief if attached / in docs/DESIGNER_RESUME_FROM_WEBSITE_AUG10.md
Constraints:
  - Preview/deploy local only (5173 / 3001 / 8080) — never www.tawala.com
  - Do NOT redesign website-mock Library/My Tawala in this chat unless a Push/Theme contract requires a tiny hook
  - Website My Tawala Details verb “Deploy” = share/embed for participants — KEEP that name; only Designer shell “Deploy” → “Push…”
  - localhost vs 127.0.0.1 = different localStorage; owner projects live on http://localhost:5500
```

---

## What the Website thread did (why you’re being called)

Roughly **Jul 28 – Aug 10** the owner prioritized **Libraries as the core of the Website** (`website-mock/` on `:5500`), not Designer canvas polish.

### Product vocabulary (do not re-invent)

| Verb | Where | Meaning |
|------|--------|---------|
| **Use** | My Tawala | Owner runs their own live start(s) — no purge |
| **Test Drive** | Library | Try without account; purge-on-start; shareable Copy link |
| **Save to MyTawala** | Library (was Save a copy) | Acquire into private My Tawala; **requires mock login / not guest** |
| **Deploy** (website Details) | My Tawala Project Details | Go live for *others* + Copy link / iframe embed |
| **Publish** | My Tawala → Library | Public catalog |
| **Push** (Designer — rename pending) | Designer | What UI still calls **Deploy…** → Show in My Tawala |

**Soft gate for viral access = save data**, not N uses (HOLD for end-page promo). Pre-live HOLDs still parked: real auth (#21), payments (#22), email metering (#20). **Website P0 (Aug 24):** private uniqueId on Copy to MyTawala — Task #26 in `website-mock/README.md` (retire shared Library uniqueId).

### Website surfaces that now exist

- Lean **My Tawala** listing + rich **Project Details** (Project Data, Versions, Theme dropdown, Deploy share, Publish, Edit in Designer).
- **Library** listing only for browse (public **Library detail pages retired** Aug 10). Multi-start Test Drive = hot-link list of start names (no faint dropdown).
- Mock session: default **logged in as `dev`**; **Logout** → guest mode (no Save; MY TAWALA → canned demo). Unlock: `http://localhost:5500/_unlock-mytawala.html`.
- Latest Library commit: `7fcbf7f` — guest taste, gated Save, listing metrics.

### What Website already did for Theme (PARTIAL)

- My Tawala Details **Theme / Appearance** dropdown lists themes, persists `themePath` on the My Tawala overlay, survives reload, carries on acquire/Push/Publish/copy.
- Changing Theme **does not** restyle live `:8080` forms. That is **your** job on Push/Redeploy.

---

## MUST DO for you now (parked Aug 10 + earlier)

Priority order owner expects when switching back:

### 1. Rename Designer Deploy → Push (UI/copy only)

- User-facing: **Deploy…** / **Deploy this version** → **Push to My Tawala** / *Push Project to your MyTawala library* (and **Push this version**).
- Keep `/api/deploy`, `deployProject`, code ids for now.
- Checklist: `website-mock/README.md` § **Designer Push rename**.
- **Do not** rename website Details **Deploy** (share).

### 2. Theme applies on Push

- On Push / Redeploy / Show in My Tawala, write deployed project CSS/theme from `themePath` so `:8080` matches My Tawala’s Theme choice.
- Overlay-only Theme on website stays; you make runtime honor it.
- Spec park: `.cursor/rules/tawala-designer-parked-post-website.mdc`.

### 3. Still outstanding from Jul 31 (if not done in Field Qualification chat)

- **Unify Insert → Invitation + Hyperlink** into one Link dialog — **Form-in-project primary**, external URL secondary, private InviteeID tertiary.
- Spec: `Tawala_Key_Documents/DESIGNER_INSERT_MENU_AND_FUNCTIONS.md` § unified Link.

### 4. Field Qualification / Sign-up w Email (this chat’s original mission)

- Continue whatever was in flight in Field Qualification (palette → Document, Sign-up w Email, etc.).
- Handoff file if needed: `Tawala_Key_Documents/DESIGNER_FIELD_QUALIFICATION_HANDOFF.md`.

### 5. Parked (not blocking Live Library) — later

- Jul 30 FIB Styles “Align right side” radio; Form Text paragraph blank lines lost on Deploy + image selection break — `DESIGNER_OPEN_BUGS.md`.
- Document P0s, native confirm, Font Color, Skip stubs — see parked rule.
- Conversion batch queue (C1–C4) — document only until owner batches fixes.

---

## Runtimes (unchanged)

| Port | Role |
|------|------|
| `:5173` | Browser Designer |
| `:3001` | Designer API (deploy, purge, records) |
| `:8080` | Tomcat / live forms |
| `:5500` | website-mock (Library / My Tawala) |

Show in My Tawala / Push receipt should open **`http://localhost:5500/...`** (not `127.0.0.1`).

---

## Suggested first reply from Designer agent

1. Confirm read of parked rule + this brief.  
2. Ask owner: start with **Push rename**, **Theme on Push**, or continue **Field Qualification** item X?  
3. Do not reopen Website Library layout unless blocked.

---

## One-liner for owner’s notes

*Website Library/My Tawala mock is far enough; Designer must rename Deploy→Push in the shell and make Theme stick on Push to :8080; keep website Details “Deploy” as share; Link conflation + Field Qualification still on Designer plate.*
