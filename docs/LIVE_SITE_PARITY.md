# Make teacherexams.org match the development site

Updated 19 November 2026, evening. The public name stays teacherexams.org. Designer (5173), the dev API (3001), the website mock (5500), Mailpit (8025), and Postgres (5432) stay off that hostname.

Keeper files and which copy is later: `docs/OWNER_PROJECT_COPIES_2026-11-19.md`. What was tested today: `docs/SESSION_2026-11-19.md`.

## What is live

The Mini is hardwired. Cloudflare Tunnel `tawala-oeb` serves Tomcat only at `https://teacherexams.org`. Bare `https://teacherexams.org` is still an empty 404. Use `/home` or `/app/library.html`.

The ten current public-library projects are on the Mini database, same project ids and form tokens, owned by `tawalamundo`. Submissions were not copied. Potluck Setup opens at `https://teacherexams.org/p/52ozm3kqd58zlss/0oufcth.Setup`. `docker restart tawala-tomcat` drops in-memory sessions only. It does not delete Postgres.

Home and Library use the 2026 pages. The Home menu and the shared menu point LIBRARY at `/app/library.html` and MY TAWALA at `/app/mytawala.html`. The featured row no longer covers the project titles, and the default project icon is back. Featured projects on Home are Potluck, Get Together, and Simple Survey.

The public Library lists the catalog names (Simple Survey, Potluck, Get Together, Horses and Penguins Test, Simple Multiple Question Survey, Online Exam Builder, Shared To-Do) plus **User Project Submissions**. That section is where a non-admin publish lands. The long name **Single Question Poll or Survey 63bc7d5b** is already published there (library project id 4). A second publish of that name is refused. Survey Sample, Multiple Question Survey Sample, and Sign-up Sheet Sample are not titles on the page. Those files are kept. The catalog was not cleaned up.

My Tawala lists the account projects (name, dates, response count). A project name opens `/app/mytawala-project.html`. The old Java details address jumps to that page. The caret shows start links on the first click and every form on the second. A Run icon is not enough to call a form a start. For Single Question Poll or Survey the start links are **Start Questionnaire** and **AdminStart**. **Use** opens the real form.

Confirmed on the public site: Copy link (public https URL), the Deploy share dialog, Backup, Export, Import, and form-level Purge. Times Used shows the response count. Last Used is the date of the latest submission.

Rename, Make a Copy, the theme dropdown, Edit in Designer, Push this version, and activate/deactivate stay disconnected. Designer must not open localhost:5173 from the public site.

Library Admin Tools is `/app/library-admin.html`, linked from the Library sidebar for an administrator. Rename, description, category, and delete of a listing belong there, not on the public Library rows. Only the author of a User Project Submission, and an administrator, may change that project’s name or description. Description and Rename on the public Library page show only on a project you published.

Administrator for an account with no administrator yet is `/app/administrator.jsp`, visible to tawalamundo while no administrator exists. Signed-out visitors are sent home. After the flag is on, only an administrator can open it.

Files extracted into the running webapp are lost if the Mini image is rebuilt. Do not rebuild the image just to pick up a page or class change. A Java class change needs `docker restart tawala-tomcat`. A page or script change does not.

## To do

1. **Confirm the last Library update.** Library Admin Tools must list projects under the signed-in line (`20261119-admin8`). Then edit one description, rename a User Project Submission by its name, and, with a second account, confirm that account cannot change someone else’s name or description. Delete of the long-named poll reported success and left the row; confirm a delete only if that row should go.
2. **Project Details still disconnected.** Rename, Make a Copy, theme, Edit in Designer, Push this version, and activate/deactivate. Restore has not been rechecked.
3. **Copy to My Tawala** on the public Library still writes browser storage. It does not create a tawalamundo project.
4. **Library titles not on the page.** Sign-up Sheet, Sign-up Sheet Sample, Survey Sample, and Multiple Question Survey Sample. Do not rename the live catalog until launch prep. Use `docs/OWNER_PROJECT_COPIES_2026-11-19.md` to pick the keeper file.
5. **Registration** (`/user/registration`) does not sign the person in or land on My Tawala.
6. **Mini image**, when a rebuild is intentional: same-host thank-you redirect and unique-id-preserving upload are in git and in the laptop image, not in the Mini image. Re-push projects so non-start forms keep `startPoint="false"` on the account itself.
7. **Automated List Builder** stays off the public Library until invitation links use the public hostname.
8. **Ops before strangers use the site.** Certificate renews before 22 December 2026. Take a Postgres backup. FileVault still needs the Mini password once after a cold boot. Confirm Docker starts at login, the Mac does not sleep the machine, and the tunnel returns after the ethernet is unplugged.
9. **Leave parked.** tawala.com, payments, ratings, email metering, Clerk, Stripe, and a teacher marketplace. October exam links stay live if someone has the URL. `/library/search` still exists if someone types it.

Do not push as `Tawalamundo`.
