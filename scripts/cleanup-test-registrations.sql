-- Remove test Registration / RegStep2 rows for the latest deployed project.
-- Keeps AdminSetup and other forms. Review counts before relying on in production.

WITH target AS (
  SELECT up.project_id
  FROM user_project up
  ORDER BY up.user_project_id DESC
  LIMIT 1
)
SELECT s.form, COUNT(*) AS rows_before
FROM submission s
JOIN target t ON t.project_id = s.project_id
WHERE s.form IN ('Registration', 'RegStep2')
GROUP BY s.form
ORDER BY s.form;

WITH target AS (
  SELECT up.project_id
  FROM user_project up
  ORDER BY up.user_project_id DESC
  LIMIT 1
)
DELETE FROM submission s
USING target t
WHERE s.project_id = t.project_id
  AND s.form IN ('Registration', 'RegStep2');

WITH target AS (
  SELECT up.project_id
  FROM user_project up
  ORDER BY up.user_project_id DESC
  LIMIT 1
)
SELECT s.form, COUNT(*) AS rows_after
FROM submission s
JOIN target t ON t.project_id = s.project_id
WHERE s.form IN ('Registration', 'RegStep2', 'AdminSetup')
GROUP BY s.form
ORDER BY s.form;
