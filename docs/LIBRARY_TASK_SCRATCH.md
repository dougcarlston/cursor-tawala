# Library task scratch — tomorrow (Aug 25)

Working copy. Canonical detail: `website-mock/README.md` § Task List.  
Review: `http://localhost:5500` only. Origin already has uniqueId + Designer session commits.

**Done Aug 24 (drop from daily view):** #26 clone-on-acquire · #27 occupancy slice 1 · Test Drive honesty copy · EXPORT/IMPORT smoke (field **names**).

**Done Aug 26:** #27 slice 2 — Library Retire frees the `:8080` **name** (`/api/retire-name` + Tomcat `retireDeployment`; uniqueId kept). Review `library-admin.html?v=20260826-retire27&admin=1`. Do not hatch Online Exam’s two copies. **Details Version** no longer repeats Source: hide auto `Copied from Library (…)` (`?v=20260826-srcver1`).

---

## Tomorrow — in this order

Stop after 1–2 build items if jetlag wins. Decisions first; do not start Auth0/Clerk wiring or live ratings.

### A. Owner decisions (talk, then maybe code)

1. **Bigger picture** — READ Aug 25 (`Tawala_Key_Documents/TAWALA_PLAN.md`). Marketplace of vetted tests/tools, not a sports-club product. Four Library sections (Me / Us / Org / Produced by users). Registration = keep records + free Designer. Reputation = user rating **and** professional review grade; comments guide users and sourcing. Do not build tutorials, audio I/O, AI cross-project, or subscriptions from this.
2. **#15 Backup package** — **DONE Aug 25:** live definition + current responses + properties. Restore Redeploys bundled definition onto `:8080` (same uniqueId) then data. Push this version = definition switch only. **Restore must not change Theme unless the backup’s My Tawala Theme property says so.** Review `?v=20260825-themebak1`.
3. **EXPORT / IMPORT + Delete / Purge** — **SIGNED OFF Aug 25.** Export (whole project OR one form) = response **data only**, not a snapshot of the Project definition. Backup/Restore = paired snapshot of now (live definition + current responses + My Tawala properties); Restore is the time machine. Import mismatch = field **NAMES** only, not MCQ wording/choices; empty-form skip (e.g. Report with no rows) is expected, not a reject. Delete = My Tawala row only (Library + Tomcat XML stay). Purge = clear responses on this uniqueId. Use does not purge. Sports stats-only Purge stays later. Mock Export file is `.export.json`, not Excel — documented gap, not a fail.

### B. Small leftover from uniqueId (same track, short)

4. **Online Exam uniqueId audit** — AUDITED Aug 25. Catalog Test Drive `u3hkqgwtrepjlur` is Tomcat **Online Exam Builder-8-3-26**. Occupancy on bare **Online Exam Builder** hits sibling `455sem0swhcswu5`. **Owner Aug 25: leave catalog URLs as-is today.** Cleanup is parked (do not drop) — see below.
5. **#27 slice 2** — **DONE Aug 26.** Retire of a live uniqueId vacates the Tomcat/Node **name** (`{name} (retired {uniqueId})`). Occupancy can reuse the name. uniqueId unchanged. Exam two-copy cleanup still parked.

### Yesterday leftover smoke (owner, 4 bullets — Restore / Deploy share)

Do not spend a session on these; they were signed off in product copy Aug 25:

1. Restore a Backup — **Theme stays** unless the backup’s My Tawala Theme property says otherwise (`?v=20260825-themebak1`).
2. Details **DEPLOY** — share popup opens **at the top**.
3. Share label default is **Click here.** (not the form name).
4. Copied URL still has `/p/{uniqueId}/`.

### C. Product research (not implement)

6. **User registration / accounts (#21 + #24)** — **CONFIRMED Aug 25** (owner). Emulate **magic-link at save-data** (door) + **GitHub personal account, org later** (shape). School roster **out**. Guest Test Drive stays free. Gate = Copy to MyTawala / keep records / Designer / Publish. Vendor (Auth0 vs Clerk) is the security pass, not this choice. Still HOLD — do not wire until the auth pass.
7. **Project reputation (#17 reopen)** — **CONFIRMED Aug 25** (owner). Amazon **shape** (stars, verified use, helpful) plus a **separate** professional review grade — never average the two. Who may rate: verified saved records only (Records ≥ 1 or account-bound save). Not Test Drive, not empty Copy, not the author on their own row. Mock: no gameable stars. Still HOLD — do not ship ratings.

---

## This week (after tomorrow)

8. **#2 Details ops rail** — **DONE Aug 25** (owner: looks good). Delete on Details (own group, then back to listing). Purge = data. De-activate = Library visibility. Listing Delete remains.
9. **#10 Deploy / share** — **DONE Aug 25 remainder:** uniqueId-in-URL + owner start labels. **share11:** dialog opens at top; Share label default **Click here.** (not form name); uniqueId lecture removed. **share13 Aug 26:** Project Data tree keeps Designer form names (Customize); do not persist Click here. over them. Review `?v=20260826-share13`.
10. **#13 Times used / Last used** — **DONE Aug 25.** Mock `tawala.mock.usageStats` after a My Tawala **Use** that actually opens (listing single-start or Details banner); blocked Use does not count. Unused = —. Library Test Drive does not bump the My Tawala copy. **Details visibility Aug 25:** banner heads were CSS-hidden below 48rem; Project Data row now shows Times used / Last used (`details13`). **Details packing Aug 25 (`stub18-pdata2`):** title uses leftover width on one line; Records / Times used / Last used hug the right-edge buttons.
11. **#18 Stub cleanup** — **DONE Aug 25.** Catalog 0 stubs; Publish is the Library path; retired listing Push-from-Designer chrome. Make a Copy still needs Designer Push. **Aug 25 notes strip:** symbiotic notes stripped from listing Transfers + Details footers so chrome can be judged for space.
12. **#14 Test Drive** — per-drive uniqueId + real leave/wipe. Waits for **Library Live**, not tomorrow’s mock.

## Parked (unless bigger picture unparks them)

- **Online Exam Builder name/id cleanup (owner Aug 25 — not today, do not drop).** Two live Tomcat copies: Library Test Drive `u3hkqgwtrepjlur` (name **Online Exam Builder-8-3-26**) vs occupancy on the exact name **Online Exam Builder** (`455sem0swhcswu5`, different form tokens). Library listing title is the bare name; the URL is the dated copy. Later goal: one display name, one Tomcat name, one Test Drive uniqueId. Method: retire or hatch-Redeploy onto the chosen public id — **never File → New**. Occupancy refuse that mentions `455…` is protecting the extra copy, not the catalog. Do not retarget catalog to `455…` unless that row is deliberately chosen as public.
- **#16 Download latest** — prefer Pull into Designer.
- **#20 Email metering** — billing.
- **#22 Payments**.
- **#23 End page** — Designer/runtime, not mock chrome.
- **#25 Library categories** — featured-audience IA.
- Designer Document P0s / Push rename — Designer chat (`43c253d`).

## Other leftovers (not tomorrow’s product)

- Uncommitted: `tawala.war.dev`, `_diag-*`, project-tray reorg, `START_LIBRARY_CHAT.md` + `start-library-chat.sh`.
- Designer chat: review `43c253d`; do not reopen `218e624` unless a uniqueId regression.
