-- Seed Divisions records for DirtBowl (Q5/Q6 dynamic pickers on Registration).
-- Safe to re-run: replaces existing Divisions submissions for shared project storage.

WITH target AS (
  SELECT up.project_id
  FROM user_project up
  JOIN users u ON u.user_id = up.user_id
  WHERE u.user_name = 'dev'
  ORDER BY up.user_project_id DESC
  LIMIT 1
),
removed AS (
  DELETE FROM submission s
  USING target t
  WHERE s.project_id = t.project_id
    AND s.form = 'Divisions'
  RETURNING s.submission_id
),
rows AS (
  SELECT * FROM (VALUES
    ('1', 'Rookie League (ages 5–6)'),
    ('2', 'Farm League (ages 7–8)'),
    ('3', 'Minors (ages 9–10)'),
    ('4', 'Majors (ages 11–12)')
  ) AS v(division_id, div_name)
)
INSERT INTO submission (submission_id, contents, form, created_dt, project_id)
SELECT
  nextval('seq_submission_id'),
  '<linked-hash-map>
  <entry><string>DivisionID</string><string-array><string>' || r.division_id || '</string></string-array></entry>
  <entry><string>DivNames</string><string-array><string>' || r.div_name || '</string></string-array></entry>
</linked-hash-map>',
  'Divisions',
  NOW(),
  t.project_id
FROM target t
CROSS JOIN rows r;

SELECT s.submission_id, s.project_id, s.form,
  substring(s.contents from 'DivNames.*?<string>([^<]+)') AS division
FROM submission s
JOIN (
  SELECT up.project_id
  FROM user_project up
  JOIN users u ON u.user_id = up.user_id
  WHERE u.user_name = 'dev'
  ORDER BY up.user_project_id DESC
  LIMIT 1
) latest ON latest.project_id = s.project_id
WHERE s.form = 'Divisions'
ORDER BY s.submission_id;
