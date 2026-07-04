# Next Designer chat — copy-paste prompt

**Purpose:** Paste the **LONG** block below into a **fresh Designer chat** after holiday (or whenever context is high). The repo holds durable memory in [`ROADMAP.md`](ROADMAP.md) and [`COMPARING_RUNTIMES.md`](COMPARING_RUNTIMES.md); this prompt bootstraps the agent with phase, constraints, and next steps.

**Read first:** [`docs/ROADMAP.md`](ROADMAP.md) and [`docs/COMPARING_RUNTIMES.md`](COMPARING_RUNTIMES.md). **Do not commit or push unless the owner explicitly asks.**

---

## LONG — paste into a new Designer chat

```text
Project: ~/Projects/AI-Tawala
Track: Browser Designer + Preview/Deploy parity
Current phase: Preview vs Deploy theme/CSS parity, using DirtBowl Registration as the reference sample
Read first: docs/ROADMAP.md and docs/COMPARING_RUNTIMES.md
Constraints: do not commit or push unless I explicitly ask

Context:
- Simple Survey passed earlier.
- Sign-up Sheet is effectively done for the current browser Designer phase; remaining issues are general Designer/deploy UX, not template-specific.
- Get Together passed Preview and Deploy with no template-specific errors found.
- DirtBowl is NOT currently a meaningful browser-Designer authoring benchmark because the browser Designer still lacks the legacy multi-window / MDI architecture and clear Pre/Post process visibility. Treat DirtBowl as a Preview/Deploy stress test, not an authoring-parity test.

What we established in the previous chat:
- The key shared memory has been saved into the repo docs, especially:
  - docs/ROADMAP.md
  - docs/COMPARING_RUNTIMES.md
  - docs/CHAT_HANDOFF.md
  - relevant Tawala_Key_Documents/DESIGNER_*.md files
- We completed a checkpoint commit and push:
  - commit: a4cc76e
  - branch: main
- Preview vs Deploy for DirtBowl Registration became the next focused phase.
- We found that Preview generally looks better than Deploy, but the gap is now mostly theme/CSS parity rather than total runtime failure.
- We also found a likely data/seed mismatch between Preview and Deploy, but that is separate from style/theme parity.

DirtBowl Registration parity status:
- Fixed/improved on Deploy:
  - Sex of Registrant MCQ alignment
  - Parent Phone Numbers overlap/crowding
  - Submit button styling
  - overall deploy parity improved substantially
- Remaining refinement:
  - grouped lower parent-contact block vertical spacing

Important working rule:
- While reviewing templates, focus on:
  - template-specific data/content errors
  - Preview mismatches
  - Deploy/runtime failures
- Treat broader authoring complaints as general Designer backlog, not template-specific blockers.

Designer backlog already identified:
- browser Designer needs legacy-style multi-window / MDI architecture for serious large-project authoring
- Pre/Post processes should be visible under forms in Project Explorer
- insertion-point arrow and Move Up / Down are later prerequisites for serious Process work
- FIB/layout parity and hint-text styling are backlog items already recorded in the repo

Next task:
Please continue from the current phase:
1. Read docs/ROADMAP.md and docs/COMPARING_RUNTIMES.md
2. Confirm the current parity target and any remaining open issue for DirtBowl Registration
3. Recommend the next narrow step in Preview vs Deploy theme/CSS parity
4. Do not make changes until you summarize the current state back to me
```

---

## SHORT — for later sessions

After the first restart, the short version is usually enough because repo docs hold most of the memory.

```text
Continue AI-Tawala browser Designer work from commit a4cc76e on main.

Read first:
- docs/ROADMAP.md
- docs/COMPARING_RUNTIMES.md

Current state:
- Sign-up Sheet passed for this phase
- Get Together passed Preview + Deploy
- DirtBowl is being used as a Preview/Deploy parity sample, not an authoring benchmark
- next phase is Preview vs Deploy theme/CSS parity using DirtBowl Registration
- deploy parity improved substantially; remaining known refinement is lower parent-contact grouped spacing

Do not commit or push unless I explicitly ask.
Before changing anything, summarize the current state and propose the next narrow parity step.
```
