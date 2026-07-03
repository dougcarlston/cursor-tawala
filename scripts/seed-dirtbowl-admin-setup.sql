-- Seed AdminSetup for the latest deployed DirtBowl project (fee, address, league vars).
-- Safe to re-run: replaces existing AdminSetup submission for that project.

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
    AND s.form = 'AdminSetup'
  RETURNING s.submission_id
)
INSERT INTO submission (submission_id, contents, form, created_dt, project_id)
SELECT
  nextval('seq_submission_id'),
  '<linked-hash-map>
  <entry><string>AdminsName</string><string-array><string>Doug Carlston</string></string-array></entry>
  <entry><string>AdminsEmail</string><string-array><string>doug@carlston.net</string></string-array></entry>
  <entry><string>AdminPhone</string><string-array><string>415-555-0100</string></string-array></entry>
  <entry><string>AdminAddress</string><string-array><string>120 Locust Ave.</string></string-array></entry>
  <entry><string>AdminCity</string><string-array><string>Mill Valley</string></string-array></entry>
  <entry><string>AdminST</string><string-array><string>CA</string></string-array></entry>
  <entry><string>AdminZIP</string><string-array><string>94941</string></string-array></entry>
  <entry><string>LeagueName</string><string-array><string>Mill Valley Dirt Bowl</string></string-array></entry>
  <entry><string>Season</string><string-array><string>2026</string></string-array></entry>
  <entry><string>ScheduleURL</string><string-array><string>https://www.dirtbowl.com</string></string-array></entry>
  <entry><string>LeagueWebSite</string><string-array><string>https://www.dirtbowl.com</string></string-array></entry>
  <entry><string>IndividualSignupFee</string><string-array><string>75</string></string-array></entry>
  <entry><string>ChargeDescription</string><string-array><string>Dirt Bowl registration</string></string-array></entry>
  <entry><string>PlayerIDGenerator</string><string-array><string>1000</string></string-array></entry>
  <entry><string>UsingDivisions?</string><string-array><string>Yes</string></string-array></entry>
</linked-hash-map>',
  'AdminSetup',
  NOW(),
  t.project_id
FROM target t;

SELECT s.submission_id, s.project_id, s.form
FROM submission s
JOIN (
  SELECT up.project_id
  FROM user_project up
  JOIN users u ON u.user_id = up.user_id
  WHERE u.user_name = 'dev'
  ORDER BY up.user_project_id DESC
  LIMIT 1
) latest ON latest.project_id = s.project_id
WHERE s.form = 'AdminSetup';
