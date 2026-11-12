# Make teacherexams.org match the development site

Updated 12 November 2026, afternoon. The public name stays teacherexams.org. Designer (5173), the dev API (3001), the website mock (5500), Mailpit (8025), and Postgres (5432) stay off that hostname.

## What is live

The Mini is hardwired. Cloudflare Tunnel `tawala-oeb` serves Tomcat only at `https://teacherexams.org`. Bare `https://teacherexams.org` is still an empty 404. Use `/home` or `/app/library.html`.

The ten current public-library projects are on the Mini database, same project ids and form tokens, owned by `tawalamundo`. Submissions were not copied. Potluck Setup opens at `https://teacherexams.org/p/52ozm3kqd58zlss/0oufcth.Setup`. `docker restart tawala-tomcat` drops in-memory sessions only. It does not delete Postgres.

Home and Library use the 2026 pages. The Home menu and the shared menu point LIBRARY at `/app/library.html` and MY TAWALA at `/app/mytawala.html`. The featured row no longer covers the project titles, and the default project icon is back. Featured projects on Home are Potluck, Get Together, and Simple Survey.

My Tawala lists the ten real account projects (name, dates, response count). A project name opens `/app/mytawala-project.html`, which uses the development layout: Project options on the left, the word buttons across the top, and Project Data with a green **Use**. The old Java details address jumps to that page. The Java page stays available for the new page to read.

The caret in front of the project name follows the blue line above Project Data. The first click shows the start links. The second click shows every form. Pick Record is not a start link for Potluck. The same rule applies across the known projects: a Run icon on the account is not enough to call a form a start link. **Use** opens the real form. Rename, Make a Copy, Backup, Restore, Deploy, Publish, Delete, Purge, theme, and Edit in Designer are visible and do not change the account.

The next Push writes `startPoint="false"` for forms that are not start links. Projects already on the Mini keep the old files until they are pushed again. This details page does not wait for that.

Files extracted into the running webapp are lost if the Mini image is rebuilt. Do not rebuild the image just to pick up the thank-you redirect or the unique-id upload.

## To do

1. **Project Details actions.** Copy link is the next one that should work on the real form. Rename, Delete, Purge, Deploy, Publish, theme, and Edit in Designer stay disconnected until each one is wired on purpose. Purge and Delete that actually change the account still live on the Java page behind the redirect.
2. **Copy to My Tawala** on the public Library still writes browser storage. It does not create a tawalamundo project.
3. **Library rows still missing** from the new Library page: Sign-up Sheet, Sign-up Sheet Sample, and Single Question Poll.
4. **Registration** (`/user/registration`) does not sign the person in or land on My Tawala.
5. **Mini image**, when a rebuild is intentional: same-host thank-you redirect and unique-id-preserving upload are in git and in the laptop image, not in the Mini image. Re-push projects so non-start forms keep `startPoint="false"` on the account itself.
6. **Automated List Builder** stays off the public Library until invitation links use the public hostname.
7. **Ops before strangers use the site.** Certificate renews before 22 December 2026. Take a Postgres backup. FileVault still needs the Mini password once after a cold boot. Confirm Docker starts at login, the Mac does not sleep the machine, and the tunnel returns after the ethernet is unplugged.
8. **Leave parked.** tawala.com, payments, ratings, email metering, Clerk, Stripe, and a teacher marketplace. October exam links stay live if someone has the URL. `/library/search` still exists if someone types it.

GitHub push is still local-only until the `dougcarlston` account can push `cursor-tawala`. Do not push as `Tawalamundo`.
