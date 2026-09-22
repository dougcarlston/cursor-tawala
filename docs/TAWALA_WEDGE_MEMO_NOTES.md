# Notes on the wedge memo (waiting-room unpack)

Claude’s memo stays in `TAWALA_WEDGE_MEMO.md`. These notes are ours: what is solid, what is wrong, and what that does to the rest of the argument. Read in order. Stop anywhere.

**Action plan (OEB packet first):** `TAWALA_WEDGE_ACTION_PLAN.md`. **Talk after they have used it:** `TAWALA_OEB_TEACHER_VALIDATION.md`.

---

## 1. The one-sentence job (mostly keep)

Claude’s sentence:

> Tawala lets you turn a form into a small database with automatic documents and email, by copying something that already works instead of building it.

That is a fair stranger-facing job. It is a **filter**, not a slogan. The useful test is still: does this need a copy-and-share app with data-over-time and email — or is it secretly a website, a general no-code database, a certified records system, or a store?

Four of the five exclusions are in the right neighborhood:

| Claude said exclude | Keep? | Why |
|---|---|---|
| Website / CMS / visual identity | **Yes** | A Test Drive link is not a homepage. Themes exist; they are not the product. |
| Open-ended custom logic (“whatever a BA dreams up”) | **Mostly** | Designer exists for the few who go deep. Public growth is still customizables, not “become Airtable.” |
| Compliance-grade institutional records (SIS gradebooks, audited HR files, medical) | **Yes as a line** | Tawala keeps records; it does not certify them. That is the SIS / HR trap later in the memo. |
| One-shot anonymous market research | **Softer** | Polls and surveys *are* in the Library. The point is: Tawala is strongest when the same group comes back. A one-question poll is still a real app; it is just not the wedge. |
| Commerce — no checkout, no payments, no inventory | **Wrong** | See below. |

---

## 2. Commerce — Claude invented a product we do not have

Claude treated “no payments” as a **definition** of Tawala. It is not. It is the **current unwired state**.

**What was actually true:** SportsDashboards collected youth-league **registration fees** as part of the same flow as the form, the roster, and the emails. Wait lists could be payment-gated. Designer had a PayPal single-item button: amount (fixed or from a field), success/cancel return forms, payment status written back onto the record. The function was for selling a single item, requesting payment for services, or taking a donation — always **inside a Project**, never as a storefront.

**What is true now:** that button is parked. PayPal-only is not the target. The later design is a **generic third-party checkout** (provider TBD: escrow, fees, failure paths). The payment market changed; we have not chosen a provider yet. That is sequencing, not a decision that Tawala “doesn’t do money.”

**What we still do not do (this exclusion can stay):**

- Inventory / SKUs / carts / merchandising
- Being Shopify
- Collecting money as the *product* rather than as a step in organizing a group

**Corrected filter line:**

> Tawala is not a store. It *can* collect money as a step in a group process (registration fee, donation, single charge). It does not do inventory, carts, or merchandising. Payments are unwired today because the provider question is still open, not because the product excludes them.

---

## 3. What that does to the rest of Claude’s memo

Once commerce is back in the product, several later claims shift. None of them automatically kill the teacher wedge. They do change **why people *can* pay** (fees in a flow) and **how complete sports is**. They do **not** make early revenue the scoreboard — see §18.

**Monetization is not email volume.**  
Claude ranked jobs by whether reminder email will eventually bite billing. Drop that column — see §8. Email is cheap group mail, not a scarce resource. Historically the real money path was a **cut of charged users / registration fees**. Sports and paid gatherings use it. Teachers mostly don’t, unless a field trip later takes a fee. “Teachers won’t pay” has to be argued on *their* budgets, not on SMTP.

**Youth sports is a complete job, not “ops plus a fee amount mentioned on a form.”**  
Claude called sports the best-proven job and the worst-fit *browse-the-Library* growth loop. That second part can still be true (league admins refer each other; a roster is not public content). The first part is *stronger* than Claude wrote, because the live product included money, not just reminders. Fallback-to-sports is not “the email-billing stress test.” It is the full season machine: register → pay → roster → assign teams → email.

**Inventory stays out; “single charge in a flow” stays in.**  
A class tracker that starts calculating district fees, or a fitness app that sells protein powder, would still fail the filter. A Get Together with an optional $5 potluck kitty, a permission slip with a bus fee, or DirtBowl registration with a season fee, would pass.

**Do not wire payments in order to pick a wedge.**  
Payments remain a pre-public HOLD (generic API, not PayPal revival). The landscape question is: when we *do* wire it, which aisle actually needs it on day one? Sports and paid gatherings do. Exam Builder does not. That is an argument for keeping sports in the serious aisle even if Home features teachers first — not an argument to rebuild checkout this month.

---

## 4. Other presumptions to mark on a first pass (not unpacked yet)

These are places Claude may also have guessed. We can take them one at a time later. No need to settle them today.

1. **“A roster isn’t Publish-able content.”** True of a *filled* roster. Empty season-ops templates (registration + fee + categorizer) were always the shareable artifact. Sports may be referral-driven *and* Library-copyable as a blank instrument.
2. **Teachers = best growth loop because Publish-an-exam is already scoped.** Loop is Version 2 **Builder content**, not outsider Designer apps (§9). **Also:** sticky market + a real critical test bench (family foundation). See notes §13.
3. **Class tracker → SIS gravity.** Merit — see §6. “Everything about the year’s students” is a dangerous sentence.
4. **Roster as new platform infrastructure.** **No.** Named Shared Datasource on the author (`Third-Period-Algebra-Roster`). Mini-DB, not a Project, not between users. See notes §6.
5. **Fitness.** Interesting reposition (PT / groups, not solo diaries). Ideate only — not top of stack. See notes §11.
6. **Viral vs serious.** Separation has Broderbund precedent (Red Orb). Delay-until-teachers-have-a-face is still open. See notes §10.
7. **HR.** Owner: Claude §7 is valid — should have thought of it. See notes §12.

---

## 5. A working filter (draft, after the commerce fix)

Use this until we rewrite the memo:

1. Copy something that already works; do not start from a blank Process. **Do not grow the Library from outsider Designer Projects** — see §9. Builder-filled *content* is the only outsider path worth keeping.
2. Same group comes back (week, season, year) — or the app is a shareable *instrument* (exam, challenge, registration) others will copy.
3. Documents and a **notify-the-group** step are part of the job, not a PDF export tacked on. That notify was email; leagues already said **text** is how kids and many parents actually hear you — see notes §16.
4. Not a website, not Airtable, not a certified SIS/HR/medical system, not a store, **not a bulk-mail / spam shop**.
5. Money may be a step in the flow. Inventory and merchandising may not.
6. Group notify (email and/or text) is part of the job. Postage cost is not a way to rank markets. Same for SMS at these volumes.

---

## 6. Class tracker — **owner: Claude’s §4 sounds most right**

This is the first stretch of the memo we are keeping as written, not as a hedge.

**What we are signing:**

- A second product called classroom management / class tracker is a **trap** (SIS gravity: weighting, report cards, district rules).
- The next teacher need is **not** “everything about the year’s students.” It is: **same kids, next sitting**, then a **Parent Update Sender** that reads scores and mails a plain summary — no formulas.
- **Roster is a capability other Projects attach to**, not a Library app you Test Drive. Exam Builder consumes it. A filled class is never a public Test Drive; empty ops templates may be copyable.
- **Invite-confirm** (parent email opt-in) is the same public-host problem already blocking Automated List Builder. Build once.
- Hold the line: attendance-note maybe; weighted grades and report cards never.

**What we are not signing:** a rewrite of OEB into a year-long classroom OS. Claude’s order is: list of people → OEB attaches → Parent Update **next door**. That is §4 as written.

**How the roster lives — owner (Sep 17):** a **data pipe to a Shared Datasource** the teacher names (e.g. `Third-Period-Algebra-Roster`). That chunk is **not a Project**. It is a **mini-DB** any of *that author’s* Projects (or copies of Projects) can reach. Shared Data was meant to be visible to **any Process owned by the author** — i.e. anything in **their** My Tawala — not a database shared *between* users. Java still matches that: storage is `getSharedStorageForUser`. Handing a list to someone else is a file problem, not a tenancy problem: turn on the **export** already built into **Multiple Question List** (`show-export-control`).

The A/B/C sketch below is the argument we walked. **C, as a named Shared Datasource, is the call.** Claude’s “Roster object” (B) is this mini-DB with a marketing noun. Fat OEB (A) stays the trap.

The useful invariant: **the teacher should not retype thirty names every time they give another exam.** That is “don’t start over.” It does not require turning Online Exam Builder into the year’s classroom OS.

Claude’s memo is a bit two-faced here. §4 sounds like *stay inside the exam app* (roster as something OEB attaches to; keep data over time). §8 then has a **Parent Update Sender** as a *separate* Library app that *consumes* exam results. That second move is already a pipe. So “don’t start over” is about **the people**, not about stuffing every new job into one Project.

Three ways we considered:

**A. Fat OEB** — rejected as the SIS path.

**B. Platform “Roster” noun** — same mini-DB, new word. Do not invent it.

**C. Named Shared Datasource** — **the call.** OEB stays the instrument. `Third-Period-Algebra-Roster` is the people. Copies of the exam GET that source. Scores can APPEND back or live on the exam Project. Library: don’t Test Drive a stranger’s roster; do copy an empty exam.

DirtBowl as one fat Project was a *deployment* shape. The *pipe* shape is what Communicators already used (email list as named source). Teachers get the pipe shape, with a name they chose.

**Locked:** do not rewrite OEB to host the school year. Do not start a second product called Class Tracker that re-asks for the students. Do not share the mini-DB across Tawala accounts (export a file if a colleague needs names). Same kids, empty sitting, no duplicate-and-prune.

---

## 7. The in-house pipes (SportsDashboards) — what we can still see

Owner recall: pipes were built **after Version 1 closed**, so they were never well documented. Clean. Used by **in-house staff** (nobody else knew). Definitely SportsDashboards. Exact jobs not remembered.

What is still in the tree matches that story.

**The mechanism is Shared Data / DataSource, not a myth.** In Java, a form can carry `dataSourceName`. Submissions then live in that user’s **shared storage**, not only inside one Project. Another Project’s Process can GET those fields (`ExternalForms` / `Record:DataSource:…` in the Fields palette). One project writes; another reads. The analysis notes call this a **master-list** pattern.

The website-mock README parks “Shared Data” as a Fleischhauer-era idea with no spec. That is about **not inventing My Tawala UI for it this phase**. The engine and the SportsDashboards projects are real.

**What we can name without guessing too hard:** the surviving examples are **Communicators** bolted onto a league **email/contact list**.

- `Dirtbowl Communicator` — form `DataBase` uses data source **Dirtbowl Email List** (name, email, team, …). The app’s job is: message the whole list, optional MCQ, review replies, add/edit/delete/audit addresses.
- `MVSC Communicator` — same shape, data source **MVSC Email List**.

DirtBowl **Registration** itself has no `dataSourceName` in the JSON we have. So the fat season project stayed fat; the **pipe was for a second job** (mail the league) sharing **the people**, not for stuffing the mailer into Registration.

That is exactly the split we wanted in §6: keep the instrument (or the season app) from becoming every adjacent job; share the list.

**Still unknown** (waiting-room memory is the right source): whether rosters, payments, or team assignment also crossed projects this way, or only the email list. Whether staff copied a Communicator per league and pointed it at a named list. Whether “SDT3” on the SportsDashboards backlog is this same thing or a later rewrite.

**Bearing on teachers:** we do not need Claude’s new “Roster” noun. The class is a **named Shared Datasource** (`Third-Period-Algebra-Roster`) on that teacher’s My Tawala. OEB and Parent Update GET it. It is not a Project, not a Library listing, and not shared to another Tawala user except as an exported table (MQL export). Public later means documenting this staff-only trick — not inventing a third architecture.

---

## 8. Email cost is not the barrier (and bulk mail is not the product)

Claude’s landscape table has a fourth column: **email-volume-sensitive?** He used it to rank jobs, to worry that teachers won’t absorb billing, and to treat youth sports as the first place email costs “bite.”

Owner: **email is cheap.** The send market is competitive; large volume is not expensive. Teachers and most Tawala groups are not a cost problem. Also: our projects are **communication inside a group** (this class, this league, this potluck) — not blasting strangers. You *could* aim Tawala at bulk lists. That is a different industry, not interesting, and under constant criticism. We should not wander there.

Two separate ideas got glued together in the old HOLD notes:

1. **Metering for us** — we still may want a counter (where did this month’s sends come from) because we pay the provider. That is operator hygiene. It is not a reason to avoid an audience.
2. **Email price as a go-to-market filter** — Claude’s column. Drop it. Do not rank Classroom vs Sports vs Fitness by SMTP.

**What stays true about email:** it is part of the job (invites, confirmations, nudges, Communicator) — and **text is the same job** for a lot of families (notes §16). Parent opt-in is still a **trust and deliverability** problem, not a cents-per-message problem. Spiky viral chain-letters are a *reputation* problem if they share a sending face with a teacher’s class mail — that is Claude §5 / notes §10, not a cost argument.

**What this does to the memo:**

- The “teachers won’t pay because they don’t send enough mail / won’t absorb email billing” case against the wedge **falls down**. If teachers are a weak money story, it is school budgets and seasonal use, not SendGrid.
- Sports is not “the job that hits email billing first.” It is still the job that **takes fees** and runs a season. Rank it on that, and on referral vs Library-browse, not on reminder volume.
- Monetize (when we do) with **the group’s money in the flow** (registration, donation, single charge — §2) and/or a normal account, not a postage meter on My Tawala.
- A working exclusion to add next to “not a store”: **not a bulk-mail / spam shop.** Group mail is in. List-rental and blast campaigns are out.

Falsify later with a real invoice, not with a thought experiment about teacher newsletters.

---

## 9. Designer is not undersold — and outsider Projects do not fill the Library

Claude’s hedge on the wedge: he is “not fully confident” the ~5% Designer figure, and he treats **Publish-an-exam** as the growth loop that makes teachers the pick. That can be read as: maybe more people will author than we think, and the Library compounds because they publish back.

Owner: **we are not underselling Designer.** Even if that were wrong, outsider Designer work is very unlikely to produce Projects **other people actually want** — unless it goes through a **Builder** (Exam Builder first). This is not a guess.

**What happened ~20 years ago:** a beta group made dozens and dozens of Projects. Almost none stayed in the public Library. The reputation system pushed them out. They could not compete with a shop of **three professional programmers**, whose apps held the top spots by **public appeal**. Volume of amateur authoring ≠ Library quality.

So:

- **Designer** is how Tawala-quality apps (and Builders) get born, and how a few people go deep for *their* group. It is not the public contribution engine. Putting Designer on Home as a growth loop would repeat a lesson we already paid for.
- **Public Library instruments** (Sign-up, Get Together, OEB, DirtBowl-class ops) stay **staff-authored**, or they do not stay.
- **The exception is Builder content:** a teacher does not design a new exam *app*; they fill OEB and (Version 2) Publish the *exam*. That is a bounded object (questions + setup, student data stripped). It can live in a user-contributed folder under the Builder. It still may not out-appeal a staff exam on Home — and it does not have to. Copy-for-my-class is enough.
- Claude’s cold-start worry (not enough decent contributed exams) is real **for a marketplace of tests**. It is the wrong worry **for a catalog of tools**. Tools come from the three; tests *may* come from teachers, behind a quality bar we already watched work (reputation pushed weak ones out).

**What this does to the memo:** keep the stranger job as “copy something that already works.” Do not “lean into Designer” to grow. Do not treat Publish-a-new-Project as the teacher loop — that loop failed empirically. Keep Version 2 as Publish-**as-content**, with eyes open that most of it will not earn a top Library slot, and that is fine. Falsifier §10.3 (publish rate) tests the *exception*, not whether beta Designers can stock the aisle.

**Hours:** if the Library’s job is a few staff instruments plus Builder *content*, then **deepening OEB** beats adding another Sign-up cousin. See §17.

---

## 10. Serious vs entertainment — a second label, not a secret (Claude §5)

Claude wants a **hard split**: different storefront (not just a Library category), different tone, no cross-links from the serious Home into chain-letter games. A parent who got an exam mail should not land on a trivia promo. He also wants to **delay** viral until teachers have given Tawala a face.

Owner parallel: **Broderbund / Red Orb.** Education and productivity were the larger, more evergreen businesses (products lasted years, not a season). Games looked like they would hurt that reputation, so games shipped under a second label (Broder spelled backwards — a weak connection was enough). Retailers said it was a good idea. Everyone already knew Broderbund made games. Microsoft made games too, and it did not obviously hurt Office.

What that actually recommends:

- **Two storefronts, not two denials.** Red Orb did not pretend the company had no games. It kept the *shelf* from mixing Print Shop with a one-season title. For Tawala: a second Home / subdomain / label for entertainment is the Broderbund move. Hiding that the same people built both is not.
- **The harm is juxtaposition, not knowledge.** Teachers and parents can know “Tawala also has games.” What they should not get is an exam confirmation that *promotes* a chain letter, or a Classroom aisle with Horses and Penguins in the same window. Claude’s “no cross-links from serious into games” matches the retailer logic. Claude’s “delay until the serious aisle has a face” is stricter than Broderbund, which **shipped both**, under two labels.
- **Evergreen vs seasonal** is the real Broderbund reason, and it maps. Sign-up, exams, league ops last. Viral trivia is a season. Mixing them on one Home makes the evergreen thing look like this week’s gag.
- **Microsoft is a weak comfort.** Different products, different channels, enormous trust already in the bank. Tawala is one site, one Test Drive, one From: line. We do not have Office’s insulation. A second label is cheaper insurance than hoping we are Microsoft.

**Working call (not a SKU plan):** keep the split. Do not put entertainment on the main Home *even temporarily*. A weak second name is enough. Do not spend the delay waiting for teachers to “brand” Tawala before any game exists — Broderbund did not wait for that. Do not spend it on secrecy either. If we try a walled viral hedge, it lives on the other label from day one.

(Claude’s memo §10 is beta falsifiers. This notes §10 is the Red Orb reading of his §5.)

---

## 11. Fitness — interesting, not next (Claude §6)

Claude: do not build a solo California diary. Build a **group** line (challenge → maybe a private log that rides on it → PE/team testing that reuses roster). He treats this as a fast-follow, not the lead wedge.

Owner: the **reposition** is interesting — chasing **PT trainers** (a person with a client list), not individuals, may be the right cut. It is still a **big build**, and we have **nothing in the space**. Fine to keep ideating. **Not going to the top of the stack.**

That sits cleanly with the rest of these notes:

- Group / trainer, not a private log, matches the share-with-a-group job and the “not a diary” filter.
- A trainer’s client list is the same shape as a coach’s roster or a teacher’s class — which is why Claude put PE/team third, *after* roster exists. That is an argument to **reuse** §4 later, not to start a fitness studio now.
- With no proof app, Home slot 4 should not pretend we have a challenge sample. Poll-or-rotate is honest; a fake fitness tile is not.
- Staff would have to author the first instrument (§9). There will be no Library of outsider workout apps.

**Working call:** keep fitness on the ideation shelf (PT / small group as the face, not the lonely log). Do not let it jump the teacher §4 sequence, invite-confirm, or packing the Library we already have. Reopen when we *want* a second deep vertical, not when we need a Home decoration.

---

## 12. HR — don’t sell it; the club on-ramp is the thought we skipped (Claude §7)

**Owner: equally valid; should have thought of this.**

Claude’s rule, kept as written:

- Do not chase HR until a design partner asks **unprompted**, with **no security review** attached.
- Until then the on-ramp that does not say “HR”: a **new volunteer / team-member onboarding checklist** for clubs, nonprofits, small teams. Checklist + welcome mail + maybe a manager ping. Lives in **Clubs & Groups**, never claims compliance.
- The stop sign: SSO, audit logs, retention policy. That person is buying an HR suite. We are not building one.

Why this was easy to miss and still right: surveys, sign-ups, and “business” in the leftover Library buckets *look* like an HR aisle. They are the same mechanism as a club bringing on a volunteer. Naming it HR invites procurement. Naming it a team checklist keeps it a Tawala Project.

Not a build order item. A **filter** we already needed (next to “not a store,” “not an SIS,” “not bulk mail”). If someone later asks for onboarding, we already know which door is ours.

---

## 13. Why lead with teachers — sticky market, and a bench we already have

Claude picked teachers mainly for **Exam Builder + Publish-as-content**. That is still on the table (as Builder *content*, §9). Owner has two other reasons Claude could not have known.

**Education was the stickiest software market** in the Broderbund years: long-term loyalty, products that last years (the same evergreen vs seasonal split as Red Orb). That is a reason to *lead* there even if sports was Tawala’s deepest *deployed* job.

**There is already a critical test market.** The family foundation has honored **100+ California high school teachers** over **20 years**, and stays in constant touch with **more than half** of them — meetings and ongoing conversation, not a mailing list. They know the owner well enough to be **blunt if it helps**. This is not “a quick scan, I’m busy, don’t bother me.”

That retires Claude’s hedge that “you know sports better than teachers.” DirtBowl is the proven Tawala *product* history. Teachers are the proven *relationship* history. For a private beta, the second matters more: we do not have to go hunting for a closed group who will tell us the truth.

**How to use it (and how not to):** this is a **design-partner bench** for OEB, roster/parent-update, and whether §4’s line holds (they will ask for a gradebook; we need people who will say that out loud). It is not a lead list, not a foundation endorsement of Tawala, and not a substitute for later strangers. Claude’s falsifiers (copy, second sitting, publish-as-content, parent mail) can actually be run with people who will not spare us.

**Working call:** teacher-first stays the lead *for this reason too*, not only because OEB exists. Sports remains the proven ops fallback. Fitness stays on the shelf (§11).

---

## 14. Home four + Library aisles for a real audience (Claude §9)

**Owner: two useful ideas.** We have done versions of both before; they get easier once the test aim is one group (teachers), not “everyone.”

**1. Four Home projects as the stranger intro.**  
Same device as the old featured strip — but it used to be untargeted. With a teacher bench (§13), the window can *introduce Tawala* without pretending the catalog is the product. Claude’s draft four (Exam Builder, Sign-up Sheet, Get Together/Potluck, a rotated “something else”) is a starting list, not a locked SKU. Sign-up still earns a slot as “this is what an app even is.” Trivia does not, even as a fourth. Fitness does not until there is an app (§11). Exact four can wait until we touch Home; the rule is: **four, aimed, no entertainment.**

**2. Recast Library divisions by audience/job, not leftover buckets.**  
Owner language, close to Claude’s: **For the classroom**, **Managing clubs and other groups**, and so on (Sports/teams when we want that aisle; not a fake Personal Records row). Drop Activities / Meetings / Polls / Business / Advanced as the public face.

**Entertainment: hide or move off this Library for now.**  
Stronger than a second label on day one, weaker than building Red Orb. The serious Library should not still have an Entertainment aisle “for completeness.” Games can return under the other shelf (§10) when we mean to sell them. Horses and Penguins does not need a public hook while teachers are the test.

Not a build this sitting. When Home/Library copy comes back, this is the IA: four aimed tiles; classroom + clubs (+ sports when honest); entertainment not on this storefront.

---

## 15. Metrics — the habit is the point (Claude memo §10)

**Owner: may not be the right measures; it is certainly an important point.**

Claude’s discipline is: **write down what would change our mind** before the foundation bench uses anything. Do not wait for a vibe. His five:

1. Test Drive → Copy on Exam Builder (curiosity vs tool)
2. Second sitting (reuse, not one-and-done)
3. Publish-as-content rate among exam authors
4. Parent email engagement
5. Shape of feature asks (more question types vs gradebook / SIS)

Fallback if those fail: sports/clubs ops, not fitness, not viral.

**What I would keep without arguing the numbers:** 1, 2, and 5. Copy + a second run is “they used it with a class.” Feature-ask shape is how we know §4 is holding — the bench will ask for a gradebook; that is a signal to hold the line, not to build it. Those three can be watched with people we already know (§13).

**What I would not make load-bearing yet:**

- **Publish rate (#3)** tests the *exception* (Builder content in the Library, §9), not whether teachers will *run* a staff exam. A low publish rate with high copy-and-reuse would still be a working wedge. Treat #3 as a later marketplace question.
- **Parent mail (#4)** needs invite-confirm and a Parent Update app that do not exist. Opens/clicks are about **whether the job happened**, not about postage (§8). Too early to be a go/no-go.

**Working call:** keep a short “we were wrong if…” list before any teacher try-out. Start with copy, second sitting, and what they ask for next. Revise the list when we have seen one real cycle. Do not instrument a dashboard this sitting.

---

## 16. Text, not only email — sports leagues already said so (filter 5.3)

Owner, rereading: leagues **pushed back on email dependence**. Most of the kids and many of the parents lived on **text**. That is probably still the world. Same as §8: at *group* volumes, SMS is also cheap. This is not an argument for bulk text marketing (still out, next to bulk email).

Designer **Send** today is email-only (`DESIGNER_PROCESS_STATEMENTS_SEND.md`). Owner: a **fairly minor** addition — a second channel on Send (phone field in, text out), neglected on the To Do list, not a new product. Logged as **D5** in `DESIGNER_OPEN_TODOS.md`. Not this sitting, not a wedge-blocker.

**What it does to the story:** Claude’s “documents and email” job is really **records + notify the group**. Parent Update and league reminders should assume **email and/or text**. Invite-confirm still applies (they asked for it; it lands). Teachers on the foundation bench may still be more email-native than youth sports; do not let that hide the sports (and many parents) fact.

**Working call:** keep 5.3 as “notify,” not “email-only.” Do not build SMS to pick a wedge. Do not skip it forever because the first Send UI had one tab.

---

## 17. Deepen the Builder, don’t stock the aisle (implication of §9)

**Owner:** if Section 9 is right (few staff instruments, classroom aisle, no amateur Project flood), programming hours are better spent **adding capability to Builder apps** than minting more Library Projects. For teachers, that means making **OEB** able to ask the questions teachers actually write.

A suite (some or all — not a locked sprint):

1. **FIB with slack** — two different problems glued together:
   - **Several acceptable answers** (exact strings the teacher lists) is ordinary scoring.
   - **Minor spelling / typing errors vs genuine ignorance** is the hard one. Owner has seen it done well; does not know the algorithms. **Guess: this may be the hardest of the five.** Do not ship a sloppy matcher that marks “wrong idea, close letters” as correct, or a typo as a zero. Park the fuzzy part until we know a method we trust; do not pretend it is a small OEB tweak.
2. **Essay FIB** — long answer; teacher grades **en masse** (all responses to *this* question, one after another) on a **0/10–10/10** range. That is how essays are graded; it is not an auto-scored blank.
3. **MCQ, more than one right** — e.g. Harlem Renaissance, pick all that apply. **Can be done now, no Designer change.** The teacher-author cannot toggle `onlyone` themselves; the Builder can offer **two designed MCQ kinds** (one with `onlyone: true`, one `false`). They pick the kind when they add a question.
4. **Image in the stem or the choices** — art exams; math that is really a figure or notation dump, not typed ASCII.
5. **Audio in the stem, maybe in choices** — “which is the chickadee?” (a)(b)(c)(d).

This is the **good** teacher feature-ask Claude’s falsifier #5 was watching for (more exam formats), as opposed to the SIS ask (§4). It also fits the foundation bench: they will tell us which of the five they would actually use.

**Split (honest):** **#3 is OEB-only, now.** Several-acceptable-answers is OEB scoring. Essay bulk-grade is OEB admin UI. Fuzzy slack (#1b) is a research/algorithm problem, not a checkbox. 4–5 are media (platform + OEB editor); audio is large. None of this is a new Library row.

**Working call:** when we spend hours on the teacher wedge after invite-confirm / Shared Datasource roster, **prefer this suite over another catalog Project**. **#3 first** if we want a cheap win. Do not start fuzzy-FIB because it sounds like “just scoring.” Prioritize the rest with the bench. Do not start them this sitting.

---

## 18. Run rate and legitimacy first; profit later (or not)

**Owner:** not indifferent to revenue. **Early, revenue is not the critical concern — run rate is.** Nothing built so far looks expensive to operate (group email/SMS, current apps). The job of this chapter is to do **enough to validate utility** with an audience **large enough to confer legitimacy**. Profit, if any, is down the road.

**Non-profit is a live option** if the utility is high and the audience is enthusiastic but poor — a fair description of teachers. That is not a slogan and not a decision. It is permission to **lead with teachers without a paywall thesis**.

What this does to Claude:

- Drop ranking jobs by who will absorb **billing** (already §8). Sports-with-fees is still a *complete product*, not the *early P&L*.
- “Teachers won’t pay” is not a case *against* the wedge. It may be a case *for* non-profit, or for a later cheap account, once legitimacy exists.
- Payments stay a capability for leagues and paid gatherings when we wire them — not a reason to delay teacher validation.
- Metrics (§15) stay copy / second sitting / what they ask for — not ARR, not conversion-to-paid.

**Working call:** keep the lights cheap (that is the run-rate test). Spend hours on utility the foundation bench will feel. Do not design a pricing page to pick the wedge. Revisit profit vs non-profit after there is an audience that makes the question real.








