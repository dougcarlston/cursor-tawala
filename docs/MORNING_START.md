# Morning start — three parallel chats

*Read this once tomorrow morning (~15 min). It sets up three Cursor chats for the AI-Tawala project, parks two of them, and starts work in Designer.*

Pair with [`CHAT_HANDOFF.md`](CHAT_HANDOFF.md) (paste openers) and [`CURSOR_CHAT_GUIDE.md`](CURSOR_CHAT_GUIDE.md) (UI details).

---

## Section 1: Before you start (5 min)

1. **Open the project in Cursor**
   - **File → Open Folder…** (or start Cursor and pick the folder)
   - Choose: `~/Projects/AI-Tawala`

2. **Optional — get the latest docs from GitHub**
   ```bash
   cd ~/Projects/AI-Tawala
   git pull
   ```
   Handoff and chat guides were pushed in commit `de98b58`. Pull if you worked on another machine.

3. **Optional — Docker for 8080 testing later**
   - You do **not** need Docker to start the Designer chat.
   - When you deploy or smoke-test on Tomcat, run from the project root:
     ```bash
     docker compose up -d
     ```
   - See `docker/tomcat/README.md` if Tomcat URLs fail.

---

## Section 2: How to find and manage chats in Cursor

Cursor’s layout changes between versions (Editor vs Agents Window). **Hover icons for tooltips.** If you cannot find something, open **Command Palette** (`Cmd+Shift+P`) and search for *Open Agents*, *New Agent*, or *Rename*.

### Open the chat / agent panel

1. Press **`Cmd+L`** to toggle the chat panel (some layouts use **`Cmd+I`**).
2. Or click the **Agents** or **Chat** icon in the sidebar.
3. For a dedicated agent window: **`Cmd+Shift+P`** → type **Open Agents Window**.

### Start a NEW chat

1. In the chat panel, click **New Chat** or the **+** icon (tooltip often says *New Chat* or *New Agent*).
2. Keyboard shortcuts that work in many builds: **`Cmd+N`** or **`Cmd+R`**.
3. In Agents Window layout: **File → New Agent**.

Each new chat starts with a **fresh conversation** — the agent does not remember other tabs.

### Rename a chat

Do this right after creating each chat so the sidebar stays scannable.

**Method A — chat tab**

1. **Right-click** the chat tab title.
2. Choose **Rename Chat**.
3. Type the name from Sections 3–5 below.

**Method B — Agents sidebar**

1. Find the conversation in the left **Agents** list.
2. Click **⋯** next to it.
3. Choose **Rename**.

*(If rename is missing on the tab, use the sidebar method — some Cursor 3.0 builds lacked tab rename; it returned around 3.3.)*

### Switch between chats

1. Click any conversation title in the **Agents sidebar** history list.
2. Or use **`Cmd+[`** / **`Cmd+]`** for previous/next chat in some layouts.
3. **`Cmd+L`** toggles the panel if it is hidden.

### Context ring (%)

- Near the input box you may see a **ring or percentage** — that is how full **this chat’s** model memory is (not your billing).
- **Hover** the chat title or ring for a tooltip; click the ring (Cursor 3.3+) for a breakdown.
- Under ~70% is usually fine. Near 90%+? Use **`/summarize`** or start a fresh chat for a new task.

### One active chat, two parked

**Do not try to run all three chats at once.** Work in **one** chat; leave the other two idle until you need them. Mixing tracks in one chat causes wrong-file edits.

---

## Section 3: Create Chat 1 — Designer (DO THIS ONE FIRST)

This continues prior browser Designer work. Create and **keep this chat ACTIVE**.

1. **New chat** (Section 2).
2. **Rename** to:
   ```
   Designer — Sign-up Sheet & Phase 4
   ```
3. **Paste** this opener as your first message (then press Send):

   ```
   Project: AI-Tawala (~/Projects/AI-Tawala)
   Track: Browser Designer — designer-web/ (Phase 4)
   Goal: Continue Sign-up Sheet template in browser Designer; deploy smoke test on 8080
   Read first: docs/ROADMAP.md Phase 4, designer-web/README.md, Tawala_Key_Documents/DESIGNER_TEMPLATE_MATRIX.md
   Constraints: Do not mix 8080 CSS/docker or website-mock work in this chat; preview/deploy local only (5173/3001/8080, not www.tawala.com)
   ```

4. **Add one line** below the opener:
   ```
   This continues work from the long session that pushed 897ac8a and de98b58.
   ```

5. **First task** to give the agent (same message or follow-up):
   ```
   File → New Project → Sign-up Sheet — retest Design, Preview, and Deploy per ROADMAP Phase 4.
   ```

6. **Mark mentally as ACTIVE** — this is the only chat you work in today unless Section 6 says otherwise.

**Dev server** (when the agent asks or you test yourself):
```bash
cd ~/Projects/AI-Tawala/designer-web && npm run dev
```
→ http://localhost:5173

---

## Section 4: Create Chat 2 — 8080 (create then PARK)

1. **New chat**.
2. **Rename** to:
   ```
   8080 — templates, Docker, Tomcat CSS
   ```
3. **Paste** this opener and Send:

   ```
   Project: AI-Tawala (~/Projects/AI-Tawala)
   Track: Template deploy → Tomcat 8080 (Phase 2)
   Goal: Deploy/smoke-test templates; fix runtime CSS or deploy scripts when URLs break
   Read first: docs/ROADMAP.md Phase 2, Tawala_Key_Documents/DESIGNER_TEMPLATE_MATRIX.md, docker/tomcat/README.md
   Constraints: Do not refactor designer-web UI or website-mock in this chat unless deploy URL wiring requires it
   ```

4. **Add one line**:
   ```
   Parked until Designer deploy needs CSS/docker help or template regression.
   ```

5. **Do not send more messages** until Designer deploy breaks layout or you need a template smoke test.

6. **Optional** — note in [`ROADMAP.md`](ROADMAP.md) Phase 2:
   ```
   Parked July 2026 — resume when Designer deploy needs CSS/docker or regression check.
   ```

---

## Section 5: Create Chat 3 — Website (create then PARK)

1. **New chat**.
2. **Rename** to:
   ```
   Website — library mock & test-drive links
   ```
3. **Paste** this opener and Send:

   ```
   Project: AI-Tawala (~/Projects/AI-Tawala)
   Track: Website mock — website-mock/ (Phase 3)
   Goal: Polish library/home pages; keep test-drive links pointed at live 8080 template URLs
   Read first: docs/ROADMAP.md Phase 3, website-mock/README.md, website-mock/js/demo-urls.js
   Constraints: Do not change designer-web or Tomcat deploy logic here; link to Phase 2 URLs only
   ```

4. **Park immediately** — no further messages.

5. **Optional** — note in [`ROADMAP.md`](ROADMAP.md) Phase 3:
   ```
   Parked July 2026 — resume after Sign-up Sheet and Get Together pass in Designer.
   ```

---

## Section 6: Your focus today

| Chat | Status | When to use |
|------|--------|-------------|
| **Designer** (Chat 1) | **ACTIVE** | All work today |
| **8080** (Chat 2) | Parked | Brief switch if deploy layout/CSS breaks after Designer deploy |
| **Website** (Chat 3) | Parked | Wait until Sign-up Sheet + Get Together pass in Designer |

**Rules of thumb**

1. **Only work in Chat 1 (Designer)** for normal progress.
2. **Stuck on 8080 layout after deploy?** Switch to Chat 2, describe the broken URL, then return to Chat 1.
3. **Website waits** — no library polish until featured templates deploy cleanly from browser Designer.

---

## Section 7: Quick reference links

| Doc | Path |
|-----|------|
| Chat openers & track split | [`docs/CHAT_HANDOFF.md`](CHAT_HANDOFF.md) |
| Rename, switch, context %, billing | [`docs/CURSOR_CHAT_GUIDE.md`](CURSOR_CHAT_GUIDE.md) |
| Status dashboard & demo URLs | [`docs/ROADMAP.md`](ROADMAP.md) |

---

## Section 8: End of day

1. **Update [`ROADMAP.md`](ROADMAP.md)** — one line under Phase 4 (or a checkbox) with what passed or what is next.
2. **Park Designer chat** — if mid-task, send a short closing note:
   ```
   Next: [one sentence — e.g. finish Sign-up Sheet deploy smoke test on 8080]
   ```
3. Leave **8080** and **Website** chats idle; do not paste unrelated work into Designer.

---

*You are done with setup when three renamed chats exist, two are parked, and Designer Chat 1 is answering your Sign-up Sheet task.*
