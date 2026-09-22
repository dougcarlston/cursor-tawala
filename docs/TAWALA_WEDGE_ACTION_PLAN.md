# Teacher wedge — action plan (OEB packet first)

Companion to `TAWALA_WEDGE_MEMO.md`, `TAWALA_WEDGE_MEMO_NOTES.md`, and `TAWALA_OEB_TEACHER_VALIDATION.md` (how you talk to them).

**Not** Version 1 vs 2. **Not** Library / My Tawala / Home. Those are big worlds. Teachers stay in **critique mode** on **one app**.

Validation is **oral, one-on-one, after they have used it** — not a form in the packet. See the validation doc for recruit script, interview order, and non-leading wording.

---

## The test (narrow)

**Recruit call:** would they try this, in a **window long enough for a busy teacher**. Three links will follow. You will **interview them after** they have built a quiz/exam and given it to some of their kids. No survey attached.

**Use (silent):** they get a working OEB only. Build something they’d actually give; try it on students. **No interview, no $100 list, no Library.**

**Interview (oral, after use):** walk-through of what they did, then unprompted stoppers / pass-along / next term, **then** (if time) $100 of R&D across contemplated features, then follow their heat. More than three questions when they have time; fewer when they don’t.

**Persistent roster:** a gimme. **Not** on the $100 list. One sentence on the instruction sheet: *we already have a named class list that can feed the next exam; it is not in this try-out, so you may re-enter names or keep the group small.* If they raise names in the interview, that same sentence — don’t sell it.

**Do not send:** Library, My Tawala, Home, Designer, Test Drive, a worksheet. Packet = three links + one-page sheet.

---



## What they actually receive

Per teacher, a **private** live OEB (own uniqueId — they must not share one Tomcat project or they will see each other’s questions and students).

Three links only (labels in plain language, not form names):


| Give them                    | Opens                                | Who      |
| ---------------------------- | ------------------------------------ | -------- |
| **Build / change the exam**  | Setup (empty first-pass → questions) | Teacher  |
| **Student link**             | Exam                                 | Students |
| **See results / administer** | Administration                       | Teacher  |


Plus a **one-page sheet**: what each link is; don’t share the admin link with students; this copy is theirs; roster not in this try-out; you’ll call after they’ve used it with students.

**Not** `localhost`. Teachers at school/home cannot open `:8080` on your machine. Delivery research (stoppers, HTTPS, don’t expose Postgres, don’t send the Library uniqueId): `TAWALA_OEB_PACKET_DELIVERY.md`.

---



## Minimum build to send the packet


| Item                                                           | Build?                                           |
| -------------------------------------------------------------- | ------------------------------------------------ |
| OEB as it already runs                                         | **No**                                           |
| One uniqueId per teacher, empty questions, three start URLs    | **Ops / Push**                                   |
| Reachable host                                                 | **Ops** — the blocker if we are still local-only |
| Dual MCQ kinds in the *demo*                                   | **No, don’t wait.** It’s on the oral $100 list   |
| Shared Datasource roster in the demo                           | **No.** Stated as already ours, omitted          |
| Storefront / aisle recast                                      | **No** for this test                             |
| Invite-confirm, SMS, Parent Update, fuzzy FIB, audio, essay UI | **No** — oral $100 / their story, after use      |


If the packet is delayed for anything other than **host + uniqueIds + one-page sheet**, we are over-building.

---



## $100 list (oral, after unprompted questions)

They split $100 out loud. $0 is allowed. All on one item is allowed. They may spend on **fixing what they used** or on **something you didn’t name**.

**On the list** (not in the try-out) — freeze from owner Word *OEB Market Validation* (Sep 18); full wording in `TAWALA_OEB_TEACHER_VALIDATION.md` §C:

Term record, class average vs student, per-question points, essay/partial credit + comment, spelling slack, images, notify + time window, save/pass exam, audio, multi-correct MCQ, plus a blank.

**In the try-out** (do not price): Question N of M; Go back / change answers; “Are you all finished now?” before scoring.

Already in the product, **not** in the try-out (stated, not priced): student login from roster; class list / roster.

Do **not** email this list with the packet. It leads. Full wording: `TAWALA_OEB_TEACHER_VALIDATION.md`.

---



## What we learn (and what we don’t)

**We learn:** whether they could use it with kids; what would **stop** them (their words); whether they’d use it again or pass it on **as they used it**; where $100 goes once they have a real sitting in mind. Body language in the room/on the call.

**We do not learn:** Library discovery; My Tawala; second sitting without retyping (we told them the roster exists). Later packets, after this critique.

---



## After the interviews (only then)

- Stoppers that are Setup/UX → fix that before new question types.  
- $100 on pick-all / exact alternate blanks → cheap OEB.  
- $100 on essays / images → those builds.  
- $100 on typo-slack → research; no toy matcher.  
- “I’d pass it on if the class list were in it” → next demo includes named Shared Datasource.  
- Storefront / Library / My Tawala: after they already know the exam app.

---



## First concrete sitting (next work block)

1. Freeze **recruit window** and the **one-page sheet** (roster sentence + you’ll call after).
2. Stand up a **reachable** OEB host; mint uniqueIds.
3. Recruit calls; send packets; **wait**.
4. Interview only after a sitting with students.
5. No Home recast, no dual-MCQ, no roster UI until this round has been heard.

**Sandbox (not Library, not My Tawala):** `~/Projects/ Current Tawala Projects/OEBMod2-nav.json` — File → Open in Designer. Push mints a new uniqueId. Cosmetics go here. `OEBMod1.json` is the earlier owner copy (no student nav).

