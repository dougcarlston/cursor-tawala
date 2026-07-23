# Contaminated Deploy quarantine — Jul 22, 2026

## What was wrong

Tomcat UserProject **`Simple Survey`** (`uniqueId` `qyzju5cyuagbidj`, `user_project_id` 450, `project_id` 1200) had:

- Project XML still titled / heading **Sign-up Sheet Template** (Redeploy-by-name mix-up)
- **7** leftover `Form 1` submission rows (Safari smoke names) under that same project

So the Deploy page looked like a Sign-up form while the list showed survey-style signups — easy to read as “apps sharing data.”

## What we did

| Action | Detail |
|--------|--------|
| Wiped submissions | Deleted all **7** `Form 1` rows for `project_id` 1200 |
| Deleted Deploy | Removed `user_project` 450, its link ids (`qyzju5cyuagbidj` / `contamjul22simples`), and `project` 1200 |
| Tomcat restart | Cleared in-memory link cache so old URLs return **Project Not Found** |
| Freed name | **`Simple Survey` is available** for a clean New Project → Deploy |

Old URL (dead): `http://localhost:8080/p/qyzju5cyuagbidj/isvhvtn.Form+1` → Project Not Found.

There was no separate Designer disk file for this Deploy — it lived only in Tomcat/Postgres. This note is the quarantine marker.

## Clean smoke next

1. Designer: **File → New Project → Simple Survey** (templates restored under `public/samples/templates/`).
2. Deploy under name **Simple Survey** (gets a fresh uniqueId).
3. Between featured apps, always **New Project** — do not load another template and Redeploy under the previous name.

## Related fix

HttpSession `StoredContextInfo` is now keyed by `userProjectId` (`ExecutionContext.java`) so Safari multi-tab Deploy is less likely to thrash in-progress form state across apps.
