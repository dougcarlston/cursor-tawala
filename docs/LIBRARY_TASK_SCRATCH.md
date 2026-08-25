# Library task scratch — tomorrow (Aug 25)

Working copy. Canonical detail: `website-mock/README.md` § Task List.  
Review: `http://localhost:5500` only. Origin already has uniqueId + Designer session commits.

**Done Aug 24 (drop from daily view):** #26 clone-on-acquire · #27 occupancy slice 1 · Test Drive honesty copy · EXPORT/IMPORT smoke (field **names**).

---

## Tomorrow — in this order

Stop after 1–2 build items if jetlag wins. Decisions first; do not start Auth0/Clerk wiring or live ratings.

### A. Owner decisions (talk, then maybe code)

1. **Bigger picture** — READ Aug 25 (`Tawala_Key_Documents/TAWALA_PLAN.md`). Marketplace of vetted tests/tools, not a sports-club product. Four Library sections (Me / Us / Org / Produced by users). Registration = keep records + free Designer. Reputation = user rating **and** professional review grade; comments guide users and sourcing. Do not build tutorials, audio I/O, AI cross-project, or subscriptions from this.
2. **#15 Backup package** — DECIDED Aug 25: live definition + current responses + properties. Restore = time machine (definition then data, same uniqueId). Push this version = definition switch only. Mock Restore still does not Redeploy the bundled definition — later build, not today’s chrome.
3. **EXPORT / IMPORT + Delete / Purge** — **SIGNED OFF Aug 25.** Export (whole project OR one form) = response **data only**, not a snapshot of the Project definition. Backup/Restore = paired snapshot of now (live definition + current responses + My Tawala properties); Restore is the time machine. Import mismatch = field **NAMES** only, not MCQ wording/choices; empty-form skip (e.g. Report with no rows) is expected, not a reject. Delete = My Tawala row only (Library + Tomcat XML stay). Purge = clear responses on this uniqueId. Use does not purge. Sports stats-only Purge stays later. Mock Export file is `.export.json`, not Excel — documented gap, not a fail.

### B. Small leftover from uniqueId (same track, short)

4. **Online Exam uniqueId audit** — AUDITED Aug 25. Catalog Test Drive `u3hkqgwtrepjlur` is Tomcat **Online Exam Builder-8-3-26**. Occupancy on bare **Online Exam Builder** hits sibling `455sem0swhcswu5`. **Owner Aug 25: leave catalog URLs as-is today.** Cleanup is parked (do not drop) — see below.
5. **#27 slice 2 (optional)** — Retire must **free** the `:8080` name. Only if A is done and you still have steam. C# Designer → Tomcat stays ungated.

### C. Product research (not implement)

6. **User registration / accounts (#21 + #24)** — **CONFIRMED Aug 25** (owner). Emulate **magic-link at save-data** (door) + **GitHub personal account, org later** (shape). School roster **out**. Guest Test Drive stays free. Gate = Copy to MyTawala / keep records / Designer / Publish. Vendor (Auth0 vs Clerk) is the security pass, not this choice. Still HOLD — do not wire until the auth pass.
7. **Project reputation (#17 reopen)** — **CONFIRMED Aug 25** (owner). Amazon **shape** (stars, verified use, helpful) plus a **separate** professional review grade — never average the two. Who may rate: verified saved records only (Records ≥ 1 or account-bound save). Not Test Drive, not empty Copy, not the author on their own row. Mock: no gameable stars. Still HOLD — do not ship ratings.

---

## This week (after tomorrow)

8. **#2 Details ops rail** — **DONE Aug 25** (owner: looks good). Delete on Details (own group, then back to listing). Purge = data. De-activate = Library visibility. Listing Delete remains.
9. **#10 Deploy / share** — PARTIAL Aug 25: Invite→link, Include→embed, preselect highlighted start; owner said share-panel language is clearer. uniqueId-in-URL still HOLD.
10. **#13 Times used / Last used** — mock counters on My Tawala **Use** only.
11. **#18 Stub cleanup** — Designer Push → Publish replacement → retire stub (needs a live definition, not chrome).
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
