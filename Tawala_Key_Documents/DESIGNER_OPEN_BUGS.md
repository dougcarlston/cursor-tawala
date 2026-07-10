# Designer open bugs

Collected from Cursor chats. Each section is that chat’s open/deferred items only. Later chats may have fixed some items — verify before treating as current.

---

## Document palette & typewriter

Branch: `cursor/forms-canvas-wysiwyg` · Session: July 9–10, 2026

- **Reset Formatting does nothing / is broken** — palette (Document and Form rich text). **Deferred:** icon is always greyed out; TODO left in code to re-enable when fixed.
- **Document HTML → XML export incomplete** — Document deploy path. Only a few function types emit real XML; others become comments. Not deferred by choice — unfinished from the handoff; still a gap if you deploy documents with those functions.
- **Field-token drag polish** — Document (optional leftover from Task 2). Not confirmed broken in testing; noted as possible unfinished polish, not a verified bug.
