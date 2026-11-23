# Make teacherexams.org match the development site

Updated 23 November 2026, morning. The public name stays teacherexams.org. The laptop Designer (5173), the dev API (3001), the website mock (5500), Mailpit (8025), and Postgres (5432) stay off that hostname. Designer for other people is the signed-in page on the Mini.

Keeper files and which copy is later: `docs/OWNER_PROJECT_COPIES_2026-11-19.md`. What was tested: `docs/SESSION_2026-11-19.md` and `docs/SESSION_2026-11-20.md`.

## What is live

The Mini is hardwired. Cloudflare Tunnel `tawala-oeb` serves Tomcat only at `https://teacherexams.org`. Bare `https://teacherexams.org` is still an empty 404. Use `/home` or `/app/library.html`.

The ten current public-library projects are on the Mini database, same project ids and form tokens, owned by `tawalamundo`. Submissions were not copied. Potluck Setup opens at `https://teacherexams.org/p/52ozm3kqd58zlss/0oufcth.Setup`. `docker restart tawala-tomcat` drops in-memory sessions only. It does not delete Postgres.

Home and Library use the 2026 pages. The Home menu and the shared menu point LIBRARY at `/app/library.html` and MY TAWALA at `/app/mytawala.html`. The featured row no longer covers the project titles, and the default project icon is back. Featured projects on Home are Potluck, Get Together, and Simple Survey.

The public Library lists the catalog names (Simple Survey, Potluck, Get Together, Horses and Penguins Test, Simple Multiple Question Survey, Online Exam Builder, Shared To-Do) plus **User Project Submissions**. That section is where a non-admin publish lands. The long name **Single Question Poll or Survey 63bc7d5b** is already published there (library project id 4). A second publish of that name is refused. Survey Sample, Multiple Question Survey Sample, and Sign-up Sheet Sample are not titles on the page. Those files are kept. The catalog was not cleaned up.

My Tawala lists the account projects (name, dates, response count). A project name opens `/app/mytawala-project.html`. The old Java details address jumps to that page. The caret shows start links on the first click and every form on the second. A Run icon is not enough to call a form a start. For Single Question Poll or Survey the start links are **Start Questionnaire** and **AdminStart**. **Use** opens the real form.

Confirmed on the public site: Copy link (public https URL), the Deploy share dialog, Backup, Export, Import, and form-level Purge. Times Used shows the response count. Last Used is the date of the latest submission. Rename, Make a Copy, Copy to My Tawala, and the theme dropdown are on the signed-in account.

Designer for a signed-in account is `https://teacherexams.org/app/designer/`. Push from there updates that account. The first Push of the library Horses file became **Horses and Penguins Test 2**. Later Pushes update Test 2 and leave the public Library copy alone. A version note typed on the Push dialog is stored on the new version and shows in Project Details → Versions.

On Project Details, **Activate** opens the live form and **De-activate** shows the offline page. The project and its responses stay. **Push this version** makes one checked older version the live form. The address stays the same, responses stay, and no new version row is added. **Delete this version** removes that one older version. **Delete Selected Items** removes every checked older version. The live version has no checkbox and cannot be deleted. A deleted number is not reused.

**Edit project in Designer** on Project Details opens the live account project in `/app/designer/`. A later Push updates that same project. **Download this version** saves a Designer `.json` file. No box checked downloads the live version. One checked older version downloads that version. Opening the file in Designer and Pushing it updates the same account project. Confirmed 23 November on **Horses and Penguins Test 2**. Restore has not been rechecked.

Library Admin Tools is `/app/library-admin.html`, linked from the Library sidebar for an administrator. Rename, description, category, and delete of a listing belong there, not on the public Library rows. Only the author of a User Project Submission, and an administrator, may change that project’s name or description. Description and Rename on the public Library page show only on a project you published.

Administrator for an account with no administrator yet is `/app/administrator.jsp`, visible to tawalamundo while no administrator exists. Signed-out visitors are sent home. After the flag is on, only an administrator can open it.

Files extracted into the running webapp are lost if the Mini image is rebuilt. Do not rebuild the image just to pick up a page or class change. A Java class change needs `docker restart tawala-tomcat`. A page or script change does not.

## To do

1. **Automated List Builder** stays off the public Library until invitation links use the public hostname.
2. **Library Admin Tools, not rechecked on 20 November.** The list should show under the signed-in line. Then edit one description, rename a User Project Submission by its name, and, with a second account, confirm that account cannot change someone else’s name or description.
3. **Restore** has not been rechecked.
4. **Library titles not on the page.** Sign-up Sheet, Sign-up Sheet Sample, Survey Sample, and Multiple Question Survey Sample. Do not rename the live catalog until launch prep. Use `docs/OWNER_PROJECT_COPIES_2026-11-19.md` to pick the keeper file.
5. **Registration** (`/user/registration`) does not sign the person in or land on My Tawala.
6. **Mini image**, when a rebuild is intentional: same-host thank-you redirect and unique-id-preserving upload are in git and in the laptop image, not in the Mini image. Re-push projects so non-start forms keep `startPoint="false"` on the account itself.
7. **Ops before strangers use the site.** Certificate renews before 22 December 2026. Take a Postgres backup. FileVault still needs the Mini password once after a cold boot. Confirm Docker starts at login, the Mac does not sleep the machine, and the tunnel returns after the ethernet is unplugged.
8. **Leave parked.** tawala.com, payments, ratings, email metering, Clerk, Stripe, and a teacher marketplace. October exam links stay live if someone has the URL. `/library/search` still exists if someone types it.

Do not push as `Tawalamundo`.
