# Make teacherexams.org match the development site

Updated 12 November 2026, late morning. The public name stays teacherexams.org. Designer (5173), the dev API (3001), the website mock (5500), Mailpit (8025), and Postgres (5432) stay off that hostname.

## What is live

The Mini is hardwired. Cloudflare Tunnel `tawala-oeb` serves Tomcat only at `https://teacherexams.org`. Bare `https://teacherexams.org` is still an empty 404. Use `/home` or `/app/library.html`.

The ten current public-library projects are on the Mini database, same project ids and form tokens, owned by `tawalamundo`. Submissions were not copied. Potluck Setup opens at `https://teacherexams.org/p/52ozm3kqd58zlss/0oufcth.Setup`. The Java My Tawala list (`/projectmanager/view`) shows all ten.

Tony’s Beta stylesheets and template images are in the Mini webapp. Home and the old Library table are styled. `lib_category.version` was added on the Mini only (`ALTER TABLE lib_category ADD COLUMN IF NOT EXISTS version integer NOT NULL DEFAULT 0`). Do not drop or rebuild either database.

The 2026 Library page is served at `https://teacherexams.org/app/library.html`. It lists the seven prototype projects (categories, descriptions, Test Drive, Copy link, Copy to My Tawala). It does not default to “logged in as dev,” and it hides the yellow mock banner. Test Drive goes through `/projectmanager/testdrive` and lands on `/t/` (session World, not the shared `/p/` world). The page reads the real Tomcat session from `/home` and shows “Welcome back, tawalamundo.”

Home and the shared Java menu (`homePage.jsp`, `pageheading.jsp`) point LIBRARY at `/app/library.html` and MY TAWALA at `/app/mytawala.html`. The new My Tawala page reads the real `/projectmanager/view` table (name, dates, response count). A project name opens the existing Java details page. Row icons on that new list do not delete or purge. Those JSP and HTML files are in the pack the owner still has to extract; until that extract, the live menu still opens the old table.

## Still to do

The latest pack is `http://192.168.86.246:8766/tawala-public-library.tgz`. On the Mini:

```bash
curl -fsSL -o /tmp/tawala-public-library.tgz http://192.168.86.246:8766/tawala-public-library.tgz
docker cp /tmp/tawala-public-library.tgz tawala-tomcat:/tmp/tawala-public-library.tgz
docker exec tawala-tomcat tar -xzf /tmp/tawala-public-library.tgz -C /usr/local/tomcat/webapps/ROOT
```

Then reload `https://teacherexams.org/home` and open `https://teacherexams.org/app/mytawala.html?v=3`. If the Home menu is unchanged, `docker restart tawala-tomcat` and sign in again. Files extracted into the running webapp are lost on an image rebuild unless they are baked into the image.

After that extract, these are still open:

- **Copy to My Tawala** on the public Library writes browser storage only. It does not create a tawalamundo project.
- Project Details is still the Java page. The prototype `mytawala-project.html` is not on the Mini.
- Three Mini library rows are not on the new Library page: Sign-up Sheet, Sign-up Sheet Sample, and Single Question Poll. Home Featured Solutions is empty (featured was off on the copy). Prototype featured set: Simple Survey, Potluck, Get Together.
- `/library/search` still exists if someone types it. Names there can still repeat because `short_desc` is the name. The column is `varchar(60)` on the Mini. The Java mapping in git is length 200. The Mini WAR was not rebuilt, and the widen-SQL was skipped.
- Registration (`/user/registration`) does not sign the person in or land on My Tawala. The public pages do not yet say that using a project on other people takes a free account and a private copy.
- Same-host thank-you redirect and unique-id-preserving upload are in git and in the laptop image Dockerfile. They are not in the Mini image. Do not rebuild the Mini image just to pick them up until that deploy is intentional.
- Laptop `lib_category.version` was not added. The laptop `lib_project` catalog is still empty.
- Automated List Builder stays off the public Library until invitation links use the public hostname.
- October exam links are still live if someone has the URL.
- Certificate renews before 22 December 2026. Take a Postgres backup before strangers use the site. FileVault still needs the Mini password once after a cold boot. Confirm Docker starts at login, the Mac does not sleep the machine, and the tunnel returns after the ethernet is unplugged.
- tawala.com stays deferred. Do not start payments, ratings, email metering, Clerk, Stripe, or a teacher marketplace.
