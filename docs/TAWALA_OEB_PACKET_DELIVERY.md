# OEB packet — delivery for 5–7 teachers

What has to be true before recruit calls. Companion to `TAWALA_WEDGE_ACTION_PLAN.md`. **Not** Library, My Tawala, or www.tawala.com.

The exam app already works on local Tomcat. **Minting copies is ordinary Push.** The work is a **reachable, staying-up, HTTPS front** on **only** the form runtime — plus not handing them the **Library** uniqueId by mistake.

---

## What is already done (not stoppers)

- OEB runs on Java `:8080` (Setup → questions → Exam → Administration).
- Empty first-pass Setup exists (no leftover Library questions on a **fresh** Push).
- Push mints a new `uniqueId` and start URLs with form tokens (`/p/{id}/{token}.Exam`, etc.).
- Isolation `/t/` Test Drive is **the wrong door** for this packet (wipe). We already know not to use it.
- Scale: 5–7 teachers plus a class of phones is nothing for this stack.

---

## Real stoppers (in order)

### 1. Host their browsers can reach — for the whole window

`localhost:8080` is invisible at school. A URL that dies in two days is worse than waiting.

Need:

- **HTTPS.** School filters and current Chrome dislike raw `http://` IPs.
- **Stable hostname** for weeks (not a random ngrok subdomain that rotates).
- **Process stays up** while the window is open (lid-close / sleep / Docker quit kills the try-out).
- **Only Tomcat 8080** on that hostname. Do **not** publish Designer (`5173`), API (`3001`), website-mock (`5500`), Mailpit (`8025`), or **Postgres (`5432`)**. Compose currently maps `5432:5432` on the Mac with a known password. That must **never** face the internet.

Practical shapes (pick one; don’t build all):

| Shape | Fits 5–7 teachers if… | Main risk |
|-------|------------------------|-----------|
| **Tunnel in front of local Docker** (e.g. Cloudflare Tunnel → `localhost:8080` only) | The Mac (or a mini) stays awake the whole window | Sleep, power, “I rebuilt compose and the tunnel broke” |
| **Small always-on box** (mini / cheap VPS) running the same `docker compose` Tomcat+Postgres | You can Push to **that** `TAWALA_JAVA_URL`, not laptop `:8080` | Ops: backups, SMTP from a new IP, not mixing Library data onto a public IP casually |
| Open home router port 8080 | — | **Don’t.** No TLS, school blocks, Postgres accidentally exposed |

**Smoke before any recruit call:** from a **phone on cellular** (not house Wi‑Fi), open the **student** Exam link, submit a dummy answer, see it in Administration. If that fails, nobody at school will succeed.

### 2. Wrong uniqueId (Library vs private copy)

Live Library OEB is `u3hkqgwtrepjlur`. **Never send that.** Teachers would share one public exam World with whoever Test-Drives the Library.

Each teacher: **new Push**, empty questions, **their** three URLs. Write uniqueId on the one-pager so we can purge/retire later.

### 3. Start URLs still say `localhost`

Push receipts and catalog rows are `http://localhost:8080/p/…`. The packet must use the **public host** and **https**. Form tokens stay (`sto3lpi.Exam` style) — only the host changes.

Teacher sheet: **Setup**, **Exam** (students), **Administration**. Not CustomizationPreview, not Test Drive, not `:5500`.

### 4. Laptop sleep / Docker down mid-exam

If the host is your Mac and it sleeps during 3rd period, the sitting dies and the critique becomes “the site was down.” For a multi-week window, treat **stay-awake** as part of the host, or use an always-on box.

### 5. Student data on our machine (trust, not code)

Answers and names live in **our** Postgres. This is a private R&D try-out, not a district vendor. One sentence on the recruit call and the sheet: *student answers sit on a test server we run; use a small group; don’t use this as the official graded midterm.* Teachers decide what they are willing to put there. Don’t surprise them.

### 6. Email (only if they tick notify)

OEB can **notify by email**. Local Send uses Resend / `doug@carlston.net` (see `docs/EMAIL_DELIVERY_OPS.md`). On a public host:

- SMTP must still work (same env file on that Tomcat).
- Links inside mail must not say `localhost` (if any).
- School filters may junk the From.
- Optional for v1 packet: tell teachers notify-by-email is **best-effort** on this try-out, or leave it off if we haven’t smoked Send through the public host.

Not a reason to block the packet if Exam + Administration work without mail.

### 7. Shared school computers / sessions

Java already keys some session state by uniqueId inside one `JSESSIONID`. Still: a lab machine that never logs out can get weird. Sheet: *use the student link; don’t use the teacher links on a student machine.*

---

## Not stoppers (don’t wait on these)

- Dual MCQ kinds, roster UI, Home recast, invite-confirm, SMS, auth/Clerk, www.tawala.com.
- Node `:3001` runtime — teachers need **Java** OEB fidelity (email, multi-form). Don’t send `:5173/p/…`.
- Performance at 5–7 concurrent classes.

---

## Delivery checklist (able to call)

- [ ] Public **https** hostname → **only** Tomcat 8080  
- [ ] Postgres **not** reachable from the internet  
- [ ] Machine/VM **stays up** for the agreed window  
- [ ] Cellular-phone smoke: Exam submit → visible in Administration  
- [ ] N = 5–7 **fresh** uniqueIds, empty Setup, three rewritten URLs each  
- [ ] One-pager: three links, roster sentence, you’ll call after, student-data sentence  
- [ ] Library uniqueId `u3hk…` **not** in any packet  
- [ ] Optional: one Send/notify smoke to yourself through the public host  

When that list is green, recruit calls are fair. Until then, lining up guinea pigs risks a window that starts with “the link doesn’t open.”
