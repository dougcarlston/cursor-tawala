# Prepare teacherexams.org for general use

Draft for the owner, 11 November 2026. Public name stays **teacherexams.org** until tawala.com can be bought back without a $100 blind lookup. Do not point anything at www.tawala.com while it is parked.

The front door, for now, is the **public Library**. A fuller landing page, with more explanation of what Tawala is, can come later. Visitors can browse and Test Drive with no account. Using a project on other people requires a free account and a private copy in My Tawala. Designer and My Tawala appear after they are registered. From a project they control, they invite other people into that project. Those people use the project link. They do not need an account unless they also want to build.

The live Library URL, `https://teacherexams.org/library/search`, currently shows an error page, and the home page’s featured list is empty. The filled Library is the laptop mock, `website-mock/library.html`. Dropping strangers on the Library means fixing that live page and loading the public projects first.

Not in this pass: payments, ratings, email metering, a teacher marketplace, Clerk or Stripe, and the exam-product pitch.

---

## 1. Host stays up

The Mini is hardwired to the main Google Wifi puck. Cloudflare Tunnel is already a boot service (`com.cloudflare.cloudflared`, state running). Tomcat answered on the house network and on `https://teacherexams.org/home` after the last sign-in.

Still to confirm:

- Docker Desktop is set to **start when you sign in**. One restart, sign in once, do not open Terminal, then check `/home` from another computer.
- Energy: start up after a power failure, and do not sleep when the display is off.
- A power cut still stops at the Mini password (disk lock). After that one password, the site should return with no Terminal.
- The tunnel reconnects by itself after a short network drop. Pull the ethernet for a minute, plug it back, and reload `/home` without touching Terminal.
- Certificate on teacherexams.org is good through **22 December 2026**. Renew it before then.
- Postgres on the Mini is **not** reachable from the house network. Checked 11 Nov 2026: port 5432 closed. 8080 open. Designer (5173), the dev API (3001), the website mock (5500), and Mailpit (8025) closed. Recheck after any compose change.

## 2. Only the right doors are public

The public name must reach Tomcat only.

- `https://teacherexams.org/home` is the home page.
- `https://teacherexams.org` with nothing after it is a blank 404. Links and mail should use `/home`.
- Designer, the website mock, Mailpit, and Postgres must stay off that hostname.
- The five October exam links are still live. Decide whether they stay up as ordinary projects or get retired before strangers arrive. Do not delete public Library listings just to tidy the exam try-out.

## 3. Login

Today’s sign-up is `https://teacherexams.org/user/registration`. It asks for a user name and a password, mails a confirmation link, and the link opens an “email verified” page. It does not open My Tawala.

Target, already decided: a personal account on the existing Java `User`, opened by a **magic link** in mail. No second login system. No Clerk. The website mock on the laptop can keep “Log in as dev” until this is real.

Steps:

- Sign-up from the home page asks for an email address (name if we still need it). The mail is the login.
- The link in that mail signs them in and lands on **their** My Tawala, empty the first time.
- The same link, opened again, does not create a second account.
- A later visit on a different computer, with the same email, reaches the same My Tawala.
- Logout on one browser does not log out a different person.
- Lost access is “mail me a new link,” not a separate password-reset puzzle.

Checks:

- Two browsers, two emails, two My Tawalas. Neither sees the other’s projects.
- Same person, two browsers. Both see the same projects after each has used a link.
- Confirmation mail uses `https://teacherexams.org/...`, never `localhost`.

## 4. My Tawala

This is the private library for one account.

- A new account sees an empty My Tawala.
- A project they build in Designer, and a project they copy from the Library, shows up here under their account.
- Copy gives a **new** id. It does not share the Library original, and it does not wipe the Library original.
- They can try a private copy before anyone else has the link.
- Deleting or changing their copy does not change the public Library project, and does not change another person’s copy.

## 5. Library and Test Drive

The public Library is the front door. No account required to browse or Test Drive.

When an unregistered visitor tries to go past that — use a project on other people, keep a copy, or open Designer — tell them, in plain words: that takes a free account. They create a user name and password, which creates their private library, then they move a project into it. Test Drive stays a private try-out in that browser. The current pages do not yet say this. Copy to My Tawala only says that login is required.

- Test Drive is `/t/` and belongs to **that browser session only**. It starts empty of other people’s answers. Closing it discards it. It must not copy live `/p/` rows.
- Two visitors Test Driving the same Library project at the same time each get their own drive.
- Two tabs in one browser still share one session. A drive for project A must not clobber a drive for project B.
- The live Library id of a project is never the link we hand to a respondent.
- Automated List Builder stays out of the public Library until invitation links use `teacherexams.org` in both directions.

## 6. Designer

Registered users get a Designer link. Visitors do not.

- Designer is not on the public hostname today. Decide how a signed-in person opens it (on the Mini, or still on a machine you run) before the home-page link goes live.
- Push from Designer goes to the Mini’s Tomcat, not the laptop, and never to www.tawala.com.
- A Push lands in **that user’s** My Tawala as a new private project.
- Deploy credentials and the dev API stay off the public hostname.

## 7. Recruiting other people into a project

The owner of a project invites respondents. Those people are not Tawala accounts.

- The link in the invitation is `https://teacherexams.org/p/...` for that owner’s copy.
- Two owners who each copied the same Library project get two links and two sets of answers.
- Several respondents on **one** owner’s link share that one project. That is correct. Their answers belong to that project.
- A respondent cannot open the owner’s My Tawala, Designer, or another owner’s project by editing the link.
- After the last Submit, the thank-you stays on **teacherexams.org**. The laptop has an uncommitted Java change for this. Confirm the Mini is actually running it before real mail goes out. A thank-you that jumps to `localhost` is a failure.

## 8. Simultaneous users

Assume many visitors, two owners, and overlapping Test Drives. One browser on the laptop is not a test.

Run these together, on purpose:

- Two registrations at the same moment.
- Two Test Drives of one Library project.
- Two respondents submitting the same live project at the same moment, including a double-click on Submit.
- One person with the same project open in two tabs, and a second person in another browser.
- An owner in My Tawala while a respondent is submitting that project.

What must be true:

- Test Drives do not see each other’s answers or the live project’s answers.
- A double Submit does not double-score or double-send mail.
- One owner’s save does not change another owner’s project.
- Restarting Tomcat may drop in-memory Test Drives. It must not drop accounts, projects, or submitted answers. Those live in Postgres.

## 9. Mail

Registration links and invitation links both depend on the Mini’s mail worker.

- One real message arrives from a new registration.
- The link in it opens on teacherexams.org.
- A second click of the same link does not send a second account.
- Test Drive of a project that would send mail does **not** send real mail.
- A failed send does not leave a half-created account with no way to retry.

## 10. Data and leftovers

- Submissions and accounts are in Postgres on the Mini. Back up that database before strangers use the site, and know how to restore it. Do not delete the database to “fix” a demo.
- The October exam copies and any answers in them are still on that database. Know which ids they are before the site is public.
- SSH on the Mini is open on the house network. That is fine. It must not be reachable from the internet.
- Old admin and setup links for those exam copies still work if someone has the address. Treat them as private owner links, not as samples on the home page.

## 11. Rehearsal before calling it open

One sitting, from a phone that is not on the house Wi-Fi:

1. Open `https://teacherexams.org/home`.
2. Test Drive one sample. A second phone Test Drives the same sample. The answers do not mix.
3. Register with a real email. The link lands in My Tawala.
4. Copy one Library project into that My Tawala.
5. Submit once as a respondent on that private link. The answer shows for the owner and not in the Library Test Drive.
6. Invite a second email. That person opens the project link and cannot see My Tawala.
7. Restart the Mini, sign in once, and repeat only step 1 with no Terminal.

When those seven pass, general access is fair. Until then, the home page can stay up for you.
