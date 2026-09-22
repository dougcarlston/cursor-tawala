# Tawala: Landscape and Wedge

## 1. The one-sentence job

**For a stranger who will never open Designer: Tawala lets you turn a form into a small database with automatic documents and email, by copying something that already works instead of building it.**

What that excludes:

- It excludes anyone whose job is "build me a website." Tawala has no page design, no CMS, no visual identity work. A Test Drive link is not a homepage.
- It excludes anyone who needs open-ended custom logic. If the job requires "whatever the business analyst can dream up," that's a general no-code database, and Tawala should lose that customer on purpose.
- It excludes compliance-grade institutional record-keeping — gradebooks that feed a state reporting system, HR files that have to survive an audit, medical records. Tawala keeps records; it does not certify them.
- It excludes one-shot anonymous research (a market-research survey with no ongoing relationship to the respondent). Tawala is best where the same group comes back — next week, next season, next year.
- It excludes commerce. No checkout, no payments, no inventory. Money can be *mentioned* (a fee amount on a form) but Tawala doesn't collect it.

This sentence is a filter, not a slogan. Every "should we build this" question should run through it: does this need a copy-and-share app with data-over-time and email, or does it secretly need one of the five things above? If the second, say no.

---

## 2. Landscape: jobs, not personas

I'm mapping jobs against three questions you asked for, plus a fourth I added because it decides monetization: **does email volume matter enough that billing eventually bites this job specifically.**

| Job (who, what, how often) | Test Drive enough, or year-long data? | Copy / Publish likely? | Home / serious aisle / entertainment | Email-volume-sensitive? |
|---|---|---|---|---|
| **Teacher runs and reuses an exam/quiz** | Test Drive proves it in one sitting; but *reuse* (same exam, new term) needs a saved copy — that's already "My Tawala," not year-long records | High copy rate; Publish is the whole point of v2 | Serious aisle, and a strong Home candidate | Low — exams go to a class list, not a mailing campaign |
| **Teacher tracks a class across a year** (attendance-adjacent, ongoing scores, notes) | Needs real year-long records + rollover | Copy yes; Publish is awkward (nobody wants a stranger's actual gradebook) | Serious aisle only, not Home yet — unproven | Medium — periodic parent digests |
| **Youth sports league runs a season** (registration, teams, reminders) | Year-long-ish (season-long) records; proven in your own history | Copy by league admins, not really "Publish" — a roster isn't content | Serious aisle; sports admins are not a browsing/Home audience, they arrive by referral | High — reminder email at scale is core to the job, and this is the job most likely to hit real billing volume first |
| **Club/HOA/small nonprofit runs an event or membership list** | Test Drive covers events (Potluck, Get Together already do this); membership tracking needs year-over-year data | Copy is natural, Publish is plausible ("here's how our HOA runs elections") | Home-friendly — legible to everyone, low trust barrier | Low |
| **Church or civic group coordinates volunteers/attendance** | Mix — event sign-up is one-shot, volunteer rotation is ongoing | Copy likely, Publish plausible (volunteer schedule templates) | Serious aisle, could be Home-friendly if framed generically as "groups," not religion-specific | Low |
| **Fitness/accountability group runs a challenge** | One-shot Test Drive is enough to try; the *value* is longitudinal, so retention depends on records-over-time | High copy rate expected, Publish very plausible (challenge templates), but **no proof app exists yet — this whole row is a hypothesis** | Home-friendly if the tone stays light; could also live in serious aisle if framed as personal record-keeping | Low-medium (weekly nudges) |
| **Individual logs personal progress alone** (workout log, habit log, study log) | Needs longitudinal data, but it's private — no group, no shareability, weak fit for your "share with a group" growth loop | Copy yes, Publish rarely (nobody wants a stranger's diary) | Neither, honestly — this is a weak fit for a platform whose loop depends on *sharing* | Low |
| **Trivia/quiz/chain-letter game** | Test Drive is the whole experience; no year-long data | Extremely high share rate, low "keep a serious copy" rate | Must not sit on Home or the serious Library — separate channel (see §5) | Low, but spiky — viral spread means spiky email load, worse for billing predictability than steady load |
| **Small business/nonprofit onboarding checklist ("HR-flavored")** | Test Drive plus a modest ongoing per-hire record | Copy plausible, Publish unlikely (checklists are too company-specific) | Serious aisle, not Home, not marketed as HR | Low |

Two things fall out of this table that aren't in your hunch list:

- **Youth sports is your best-proven job and your worst-fit growth loop.** It has the deepest real usage you've ever had, but a roster isn't shareable content, and league admins don't discover you by browsing a Library — they get referred by another league admin. That's a sales motion, not a try→copy→share→publish motion. Keep it as a fallback and as a stress-test for the email-billing model, but don't build Home around it right now.
- **"Individual logs personal progress alone" is a trap disguised as personal improvement.** It's real appetite, but it doesn't use your actual mechanism — a private diary has no group to share with. If you build for California-fitness, build the *group* version first (see §6), not the solo tracker, even though solo trackers are easier to imagine.

---

## 3. Wedge recommendation

**Wedge: Teachers × "give students an assessment I don't have to hand-grade, and can reuse next term" × Exam Builder (with v2 Publish-as-content as the proof of the loop, not just the proof of the app).**

Ranking your hunches:

1. **Teachers — the pick.** You already have a working proof app with real usage, and it's the only job on the table where Publish-as-content isn't speculative — it's a scoped, already-planned feature. It's also the only job where "copy something that already works" and "share it back so others can copy it" are the *same* action a normal user would take unprompted (a teacher who wrote a good midterm already wants to hand it to a colleague). That's a growth loop you don't have to invent.
2. **California fitness — real, but zero proof apps exist.** The appetite is not in question; your mechanism-fit is. Worth a fast-follow bet in parallel (§6), not the lead wedge, because right now it's all hypothesis and no deployed app.
3. **Viral entertainment — a multiplier, not a wedge.** It can pour top-of-funnel traffic into Tawala, but it doesn't teach you anything about whether Forms+records+email solves a real recurring job, and it actively damages trust in the serious aisle if not walled off first. Sequence it after the serious aisle has a face (§5).
4. **HR — correctly your lowest priority.** You already know why: procurement, security review, and incumbents. I'd go further than "hard market" — it's a market where your core growth loop (try without an account, copy a public library app, publish it back) is actively disqualifying. No HR buyer wants their onboarding checklist living next to a public trivia game in a "vetted Library." Don't chase it; see §7 for the narrow on-ramp that avoids saying "HR" at all.

**What Home stops featuring if you take this wedge:** stop treating the current leftover-bucket variety (poll, potluco, to-do, trivia) as equally weighted Home real estate. Home should feature things that either *are* the wedge or *stress-test* the wedge's boundaries — not a representative sample of everything in the catalog. Concretely: Horses and Penguins (trivia) and the generic poll come off Home even if they stay in the Library, because their presence next to Exam Builder undercuts the "this is a serious tool" signal you need for teachers to trust it with student data.

**Strongest case against this pick:**

- Teachers are seasonal (usage clusters at the start and end of terms, dead in summer) and famously reluctant to pay for anything, including via their institution. If your monetization eventually runs through email volume, this may be the job with the *lowest* willingness to absorb billing, even though it's your best growth loop.
- The content-marketplace loop (Publish an exam, another teacher copies it) has a cold-start problem: it needs a critical mass of decent contributed exams before it's better than "just write your own," and you don't know yet how many contributions that takes.
- Anything involving students and parent email brushes against real privacy expectations (not necessarily FERPA-regulated if you're not a school vendor of record, but perceived-privacy matters even when the law doesn't strictly apply). Get this wrong once, publicly, and it costs you the whole aisle, not just one Project.
- I'm not fully confident the "5% use Designer" number and the youth-sports-to-teacher transfer of trust actually holds — you know both audiences from lived experience, teachers less so than sports. Treat this pick as a hypothesis to falsify in beta (§10), not a settled bet.

---

## 4. Teachers: is a class tracker the second app, or a trap?

**Mostly a trap, if built as a second front-end Project. Not a trap if treated as a platform capability that the *existing* proof app consumes.**

The tell is in your own framing: you called it "the year's students, and everything that must be tracked about them." That "everything" is exactly the gravity well that turns a narrow tool into an SIS. Full gradebooks fail or succeed on grade weighting, standards alignment, report-card formatting rules that vary by district, and multi-teacher data sharing — none of which is a Forms+Process+email job, all of which is what "class tracker" quietly grows into if you let teacher feature requests drive the roadmap.

**What I'd actually ship, in order:**

1. **Persistent roster, as a platform capability, not a Project.** A roster is a list of students (with parent email) that survives across multiple runs of *other* Projects and across a school-year rollover. This is infrastructure Exam Builder already implicitly wants (run the same exam against the same class twice) — build it as something Exam Builder attaches to, before you build anything called "tracker."
2. **The smallest thing that's still "the year's students": a Parent Update Sender**, not a gradebook. It reads existing per-exam results tied to a roster and periodically emails parents a plain summary ("Here's what your child scored on this term's assessments"). No formulas, no weighting, no standards. It's a *consumer* of exam data plus a scheduled email — both things Tawala already does well — with no new grading logic to own.
3. If that lands, extend narrowly (attendance note, one field at a time) — but hold the line against ever building weighted grade calculations or district-specific report-card layouts. That's the line between "Tawala Project" and "we accidentally built an SIS."

**What the platform must already support before any of this:** rosters that persist and roll over between school years without losing history; parent email that's opt-in and confirmable (this is the same "invitation-confirm on a public host" capability blocking Automated List Builder — it's not teacher-specific, it's foundational, and it should be built once and used everywhere;) and a privacy posture that's explainable in one sentence to a nervous parent, before you need a lawyer to explain it.

**Library placement:** teacher *content* (exams) belongs in the serious Library under something like "Classroom," authored via Publish-as-content, browsable and copyable with no account. Teacher *operations* (roster, parent updates) should not be publicly browsable in the same way — nobody should be able to "Test Drive" a stranger's actual class roster. Keep operations Projects available to copy as an empty template, but don't surface real instances the way you'd surface a public exam.

---

## 5. Viral vs. serious: separate them, and delay viral

**Hard separation, and yes — delay.**

Concretely: a different subdomain or clearly distinct top-level section (not just a different Library category label), a visually and tonally distinct Home, and no cross-links from the serious storefront into the game channel. A parent who got an exam-confirmation email from Tawala should never land on a page that also promotes a chain-letter game — that one juxtaposition can undo the trust the serious aisle needs to earn.

Delay it specifically until the serious aisle (teachers, in this plan) has enough real usage that the brand has a face other than "whatever's on Home this week." Viral traffic is valuable, but right now it would be the *only* signal Home sends about what Tawala is, and that signal is the wrong one for a copy who needs to trust you with their students' data. Sequence: prove the teacher loop first, then open the entertainment channel as a separate front door, not a room in the same house.

One caveat, flagged as uncertain: it's possible viral entertainment is actually a *better* near-term test of the underlying mechanism (Test Drive → share → maybe keep a copy) with much lower stakes than testing it on teachers. If you're nervous about the teacher bet taking too long to read a signal, running a small, walled-off viral experiment in parallel — never featured on the main Home — is a reasonable hedge. Just don't let it bleed into the serious storefront.

---

## 6. Fitness as a deep vertical

"Go as deep as possible" as Tawala Projects, not a wearable company, means: use what Tawala already does — forms, records-over-time, and email nudges to a group — rather than sensors, integrations, or tracking hardware. Concretely, three Projects that build a line, not a point:

1. **Group Fitness Challenge (first proof app).** A small group (a friend group, an office, a gym class) logs a shared metric weekly; Tawala aggregates and emails a standings update. This is the one that actually uses your mechanism — shareable, group-based, uses for-each/aggregate process logic you already have, and produces a natural nudge email. This is the fitness equivalent of Exam Builder: pick this first, not a solo log.
2. **Personal Training/Habit Log (second).** Individual, longitudinal, private — weaker fit for the share loop, but strong for retention once someone's already in via #1 (a challenge participant who wants to keep logging after the challenge ends). Build this second so it rides on people already acquired through #1, not as a stand-alone acquisition play.
3. **PE / Team Fitness Testing (third — and this is the interesting one).** This is where fitness overlaps teachers and youth sports directly: a coach or PE teacher runs a periodic fitness test (mile time, push-ups, etc.) against a persistent roster and tracks improvement across a season or year. It reuses the roster capability from §4, reuses the coach/parent-email pattern from your sports history, and gives you a second wedge candidate that shares infrastructure with the teacher bet instead of competing with it for engineering time.

Where it must stay purely consumer: #2, the solo log — there's no institution, no roster, no group, and building group features into it would be over-engineering a private diary. Where it should deliberately overlap: #3, because that overlap is what makes fitness cheap to build *if* the teacher wedge succeeds first, and expensive to build in isolation.

I'm not confident which of #1's specific challenge formats (weight loss, step count, running streak) has the best organic share rate in California specifically — that's worth a cheap test (three landing variants, see which Test Drive link gets shared unprompted) before committing engineering time to more than the first version.

---

## 7. HR: narrow on-ramp, not a suite

**Do not chase this until a design partner asks you for it unprompted, with no security review attached.** Until then, the on-ramp that doesn't say "HR": a **New Volunteer/Team-Member Onboarding Checklist**, marketed to clubs, nonprofits, and small teams — not to HR departments. It's the same Forms+Process+email mechanism (a checklist, a welcome email, maybe a manager notification), but it lives in "Clubs & Groups," not "Business," and it never claims to be compliant with anything. The moment a prospective user asks about SSO, audit logs, or data retention policy, that's the signal to stop — that's an HR buyer, not a club organizer, and they're asking for the suite you've explicitly decided not to build.

---

## 8. Product development order (teacher wedge)

In build order, with dependency and priority marked:

1. **Public invite-confirm links** (platform/Designer work). *Marketing-critical.* Blocks parent email opt-in for everything downstream, and separately unblocks Automated List Builder going public — do this first regardless of which wedge wins.
2. **Persistent roster with school-year rollover** (platform/Designer work). *Marketing-critical.* Everything in §4 depends on this; it is not a Library app, it's infrastructure Exam Builder and future Projects attach to.
3. **Exam Builder v2: Publish-as-content** (Builder feature + Library curation). *Marketing-critical.* This is the actual growth-loop test — without it you have a good app, not a compounding Library.
4. **Parent Update Sender** (Library app, consumes 1+2+3). *Marketing-critical.* The smallest "class tracker" that's still real, and the first thing that makes the roster capability visible and valuable to a teacher rather than invisible plumbing.
5. **Teacher "copy my class" onboarding flow** (product/UX, not a Project per se). *Marketing-critical.* Reduces the friction of going from "copied one exam" to "set up my whole class" — this is where soft-gate account creation should actually happen.
6. **Permission slip / field-trip form linked to roster** (Library app, pure — no new platform work once 1+2 exist). *Nice.* Reinforces roster utility with a second, unrelated-to-grades use case.
7. **A second Builder-publishes-content type** (e.g., a Worksheet Builder) (Library app + reuse of 3's curation pattern). *Nice.* Tests whether the Publish-as-content pattern generalizes beyond exams, which matters for your long-term thesis but isn't needed to validate the wedge itself.
8. **Reading/homework log, minimal** (Library app). *Nice.* Only build if #4 shows teachers actually want more than one recurring data point tracked — don't build it speculatively.

Items 1 and 2 are the ones that force real platform/Designer engineering rather than Library-app authoring; everything from item 4 onward should be buildable as Library apps on top of that platform work without touching Designer again.

---

## 9. Home and Library IA

**Four Home slots before you know if the wedge is working** — chosen to test the wedge while keeping Home legible to a stranger who's never heard of Tawala:

1. **Exam Builder** — the wedge bet, front and center.
2. **Sign-up Sheet** — the single most universally legible app you have; its job is less "convert this specific visitor" and more "teach any visitor what a Tawala app even is" before they commit to a niche.
3. **Get Together / Potluck** — tests the general clubs/gatherings job, which is your best fallback if teachers underperform, and it's Home-friendly by nature (low trust barrier, no privacy stakes).
4. **One deliberately different domain, rotated** — a fitness challenge sample once one exists, or a poll if not — clearly framed as "see what else runs on this," used to read appetite in a domain you haven't committed to. Do not put trivia/chain-letter content here even temporarily; keep that channel separate from day one per §5.

**What replaces them once you have signal:** if teachers pull — swap slot 4 for Parent Update Sender or the Permission Slip app, and consider dropping Get Together to a secondary slot; Home starts looking like a classroom storefront with one general-utility anchor (Sign-up Sheet) for legibility. If fitness pulls instead — swap in the Group Fitness Challenge and let it take a primary slot. If nothing pulls — that's itself a falsification result (§10), and Home should default back toward the proven-but-hard-to-grow-organically sports/ops story while you reconsider.

**Library aisle naming**, replacing the leftover buckets (Activities, Meetings, Polls, Sports, Business, Entertainment, Advanced): split at the top level into a serious side and an entertainment side that don't share navigation. Serious-side aisles by job, not by feature: **Classroom** (exams, worksheets, parent updates), **Clubs & Groups** (gatherings, sign-ups, volunteer coordination), **Sports & Teams** (registration, rosters, season ops), and a smaller **Personal Records** aisle once fitness has a real proof app. Entertainment lives entirely outside this structure — its own section or subdomain, not an aisle alongside Classroom.

---

## 10. What would falsify this in private beta

Five observables, tracked from a closed beta (real users, not open internet):

1. **Test-drive-to-copy conversion on Exam Builder.** If teachers try it but rarely create an account/keep a copy, the tool isn't solving a felt weekly pain — it's a curiosity.
2. **Reuse across a second run.** Do teachers who copy an exam actually run it again — a retake, a parallel section, next term — or use it once and disappear? One-and-done usage means you've built a nice form, not a platform habit.
3. **Publish rate among exam authors.** This is the real test of the growth loop, not just the app. If almost nobody publishes, the content-marketplace thesis for teachers is wrong, even if Exam Builder itself is well-liked.
4. **Parent email engagement** (opens/clicks on confirmations and, once built, Parent Update Sender). This tells you whether the email leg of the mechanism actually works for this audience, and whether it's worth billing for later.
5. **The shape of unsolicited feature requests.** If teachers ask for more question types and more exam formats, you're on the narrow path you want. If they ask for grade weighting, standards alignment, or report-card formatting, they're pulling you toward the SIS trap regardless of what you intended to build — that's a signal to hold the line, not to follow the demand.

**If these fail, the fallback wedge is youth sports/clubs & leagues operations** — not fitness, and not viral. It's the one job in this memo with real, already-proven deployment depth; the reason it isn't the lead pick is growth-loop fit (referral-driven, not browse-and-copy), not product-market fit. If teachers don't validate, the honest move is to accept a slower, more sales-assisted motion around season operations rather than forcing a self-serve story that isn't working, while keeping a small, separately-funneled fitness experiment running in parallel as the faster-feedback bet.
