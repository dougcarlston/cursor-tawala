# Process variables typing (SET) — handoff reference

## STATUS (Jul 27, 2026)

**RESOLVED in Deploy export** — expression compiler in `designer-web/server/jsonToXml.mjs` (see `jsonToXml.test.mjs`). Set-statement math and string concat export correctly; **no Final Design chat is required** for this bug unless Deploy smoke tests fail again or you want **Process UI polish** later.

Do **not** reopen as "variables always text" in Final Design unless smoke tests fail.

**Deploy smoke (quick check):**

- `SET a=1`, `SET b=2`, `SET c=<<a>>+<<b>>` → Deploy **3**
- `SET dessert="ice"+"cream"` → **icecream**

Also see `DESIGNER_OPEN_BUGS.md` (if present) for the Jul 27 fix note.

**Jul 27 follow-up:** owner re-tested on the real **Horses and Penguins Test** project (`Set Score to <<Score>> + 1`, `Set Wrong to 17 - <<Score>>`) and still saw text-not-math. This was a **Process** Set (confirmed against the project JSON, not Skip Instructions). The compiler already handled these patterns correctly — the real cause was a **stale dev API process**: `designer-web/server/index.mjs` runs as a plain `node` process with no watch/reload, and it had been started *before* the fix was saved to `jsonToXml.mjs`. Editing server files silently does nothing until the port-3001 process is restarted. Killed the stale process and restarted via `ensure-dev-api.sh`; added 3 regression tests using the project's exact expressions (self-referencing `<<Score>> + 1`, reversed-operand `17 - <<Score>>`, chained `<<Score>> * 100 / 17`). **Any time a `designer-web/server/*` fix is made, restart the dev API before asking the owner to redeploy** — `ensure-dev-api.sh` only restarts when the health check is down, so it won't catch a stale-but-responding process on its own.

---

### Historical — paste into Final Design (Jul 2026, pre-fix)

*The opener below was for a new Design chat before Deploy export was fixed. Keep for context only.*

Implement **Process variable typing** for SET statements in browser Designer (`designer-web/`). Variables should be numeric or text based on how they are set and used — not always treated as text.

---

## Bug (historical)

Variables were treated as text in Deploy export. Owner wanted correct **numeric vs text typing** for Process variables. This is **Process/expression work** — not Form canvas `*CanvasRow` work.

## Intended behavior (owner)

- Variables take their value from Process **SET** statements.
- If the SET value is **numeric** → treat as **numeric** variable.
- If the SET value is **text** → treat as **text** variable.
- A number on the right-hand side encased in quotes (`""`) is **text** (e.g. `"123"`).
- If the RHS is a **Form FIB** field, type follows the value in the FIB (and/or Validation FIB option that predefines numeric).
- **Numeric variables:** combine with `+` `-` `*` `/` `^`; parentheses guide order of operations.
- **Text variables:** only operator is `+` meaning **concatenate**. Example: `SET dessert TO "ice" + "cream"` → `dessert = "icecream"`.

## Out of scope unless needed

- Form canvas redesign
- File Uploader
- SportsDashboards full compare

## Smoke tests

1. `SET a TO 1`; `SET b TO 2`; `SET c TO a + b` → **c is 3** (numeric)
2. `SET dessert TO "ice" + "cream"` → **"icecream"**
3. `SET n TO "123"` → **text**, not number
4. `SET` from FIB blank with numeric validation → **numeric math works**
