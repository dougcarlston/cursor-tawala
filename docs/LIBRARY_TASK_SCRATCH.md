# Library task scratch — tomorrow (Aug 25)

Working copy. Canonical detail: `website-mock/README.md` § Task List.  
Review: `http://localhost:5500` only. Origin already has uniqueId + Designer session commits.

**Done Aug 24 (drop from daily view):** #26 clone-on-acquire · #27 occupancy slice 1 · Test Drive honesty copy · EXPORT/IMPORT smoke (field **names**).

---

## Tomorrow — in this order

Stop after 1–2 build items if jetlag wins. Decisions first; do not start Auth0/Clerk wiring or live ratings.

### A. Owner decisions (talk, then maybe code)

1. **Bigger picture** — why registration + reputation now (you said you’ll explain). That sets whether we emulate a marketplace (Amazon-like) or a club roster (sports commissioners).
2. **#15 Backup package** — confirm default: deployed definition + current responses? How Restore differs from **Push this version** in the UI. No Backup UI expansion until this is yes/no.
3. **EXPORT / IMPORT + Delete / Purge** — owner sign-off on the Aug 24 smokes, or one more pass. Mismatch = field **names**, not MCQ wording.

### B. Small leftover from uniqueId (same track, short)

4. **Online Exam uniqueId audit** — catalog Test Drive is `u3hkqgwtrepjlur`; occupancy saw Tomcat name **Online Exam Builder** as `455sem0swhcswu5`. Decide: update catalog URLs, or hatch-Redeploy stock onto the name you want public.
5. **#27 slice 2 (optional)** — Retire must **free** the `:8080` name. Only if A is done and you still have steam. C# Designer → Tomcat stays ungated.

### C. Product research (not implement)

6. **User registration / accounts (#21 + #24)** — emulate, don’t invent. Shortlist 2–3 patterns (e.g. email magic-link, GitHub-style, school-roster). Soft gate stays **save data**, not N Test Drives. Real passwords/recovery = security pass; mock session `dev` is not that.
7. **Project reputation (#17 reopen)** — Amazon-like *shape* (stars, reviews, “verified” signal, helpful votes). Do **not** ship gameable stars on the mock. Decide: who can rate (Copy to MyTawala? finished Test Drive? published author?), and what “verified use” means.

---

## This week (after tomorrow)

8. **#2 Details ops rail** — Purge ≠ Delete ≠ De-activate grouping.
9. **#10 Deploy / share** — picker/embed polish; uniqueId-in-URL still HOLD stretch.
10. **#13 Times used / Last used** — mock counters on My Tawala **Use** only.
11. **#18 Stub cleanup** — Designer Push → Publish replacement → retire stub (needs a live definition, not chrome).
12. **#14 Test Drive** — per-drive uniqueId + real leave/wipe. Waits for **Library Live**, not tomorrow’s mock.

## Parked (unless bigger picture unparks them)

- **#16 Download latest** — prefer Pull into Designer.
- **#20 Email metering** — billing.
- **#22 Payments**.
- **#23 End page** — Designer/runtime, not mock chrome.
- **#25 Library categories** — featured-audience IA.
- Designer Document P0s / Push rename — Designer chat (`43c253d`).

## Other leftovers (not tomorrow’s product)

- Uncommitted: `tawala.war.dev`, `_diag-*`, project-tray reorg, `START_LIBRARY_CHAT.md` + `start-library-chat.sh`.
- Designer chat: review `43c253d`; do not reopen `218e624` unless a uniqueId regression.
