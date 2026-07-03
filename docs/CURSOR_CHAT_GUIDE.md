# Cursor chat — practical owner guide

Short reference for using Cursor chat on the AI-Tawala project. Pair with [`CHAT_HANDOFF.md`](CHAT_HANDOFF.md) (what to paste per track) and [`ROADMAP.md`](ROADMAP.md) (status).

**Version note:** Cursor’s UI changes between releases (2.6 → 3.0 Agents/Glass → 3.3+). Icon placement and rename menus vary by layout. When unsure, **hover for tooltips** or open **Command Palette** (`Cmd+Shift+P`) and search the action name.

---

## 1. Chat UI icons (Glass / Agents layout, 2026)

Cursor has two common layouts:

| Layout | Open via | Best for |
|--------|----------|----------|
| **Agents Window** (Glass) | `Cmd+Shift+P` → *Open Agents Window* | Parallel agents, cloud/local handoff |
| **Classic Editor** | `Cmd+Shift+P` → *Open Editor Window* | VS Code–style editing, extensions |

You can run both at once.

### Upper area of the chat panel

Exact icons depend on your version and layout. Typical controls:

| Control | What it does | How to find it |
|---------|--------------|----------------|
| **New chat** | Fresh conversation (no prior turns) | `Cmd+N` or `Cmd+R`; in Glass, **File → New Agent** |
| **History / chat list** | Prior conversations | Left **Agents sidebar** in Glass; **clock icon** in some Editor layouts |
| **Agents** | Agent list / sidebar | **View → Agents**; Glass sidebar is usually always visible |
| **Layout** | Editor vs agent-centric panes | Layout dropdown / gear (top-right in newer builds); `Cmd+E` toggles agent layout |

**Tip:** If history clicks stop working, try **Swap Agent Sidebar Location** (double-arrow at top of chat panel) — a known intermittent bug in some builds.

### Below the input (composer bar)

| Control | What it does |
|---------|--------------|
| **Context ring / %** | How full the **model context window** is for this chat (not billing) |
| **Model picker** | Model for **this** chat; `Cmd+/` cycles models |
| **Mode picker** | Agent / Ask / Plan / Debug — `Shift+Tab` or `Cmd+.` |
| **Send** | Submit prompt; `Cmd+Return` forces send while agent is working |
| **Attach** | Drag/drop files, `Cmd+V` paste screenshot, `@` for files/folders/diffs/past chats |
| **Voice** | Microphone icon — dictate; `Cmd+Shift+Space` toggles voice mode |

Hover each icon for its tooltip if labels are missing.

---

## 2. Context window ring (e.g. 56%, 96%)

### What it means

The ring shows how much of the **current model’s context window** is used **in this chat**. It includes:

- System prompt and tool definitions
- **Rules**, **Skills**, **MCP** tool catalogs
- Your messages, agent replies, file reads, tool output
- **Summarized** older turns (after compression)

**Click the ring** (Cursor 3.3+) for a category breakdown — useful when the chat feels heavy even at moderate %.

### When to worry

| Range | Guidance |
|-------|----------|
| **&lt; ~70%** | Usually fine |
| **~80–90%+** | Slower replies, more “forgetting,” auto-summarization may kick in |
| **Near 100%** | Start a new chat or compress |

### What to do

| Action | When |
|--------|------|
| **`/summarize`** (or `/compress`) | Keep the same chat but free space by compressing older turns |
| **`/context`** | Text breakdown of what is consuming context |
| **New chat** (`Cmd+N`, `Cmd+R`) | Task truly changed, or summarize is not enough |

**Important:** Context % is **not** your dollar usage. Billing is separate (see §6).

---

## 3. How to rename a chat

Clear names matter for the three-track workflow (see [`CHAT_HANDOFF.md`](CHAT_HANDOFF.md)).

### In the UI

**Method A — Chat tab**

1. **Right-click** the chat tab title.
2. Choose **Rename Chat**.
3. Enter the new name.

*(Rename from tab context menu was missing in some 3.0 builds; restored around 3.3.)*

**Method B — Agents sidebar**

1. Open the **Agents sidebar** (conversation list).
2. Click **⋯** next to the conversation.
3. Select **Rename**.

**Method C — Keyboard shortcut**

1. Open Keyboard Shortcuts (`Cmd+R` then `Cmd+S`, or **Preferences → Keyboard Shortcuts**).
2. Search **`composer.renameChat`**.
3. Assign a shortcut.

### Agent automation

The **cursor-app-control** MCP exposes **`rename_chat`** — agents can rename the active tab when you ask (*“rename this chat to Designer — Sign-up Sheet”*). This is automation inside Cursor, not a button you click yourself.

**Sync caveat:** Renaming in Agents Window vs Editor panel did not always sync until recent 3.3 fixes. If titles disagree, rename from the sidebar.

---

## 4. Switch between chats

| Method | Steps |
|--------|--------|
| **Agents sidebar** | Click any conversation in the left list |
| **Toggle panel** | `Cmd+L` (or `Cmd+I` in some layouts) |
| **Previous / next chat** | `Cmd+[` / `Cmd+]` |
| **Agents Window** | `Cmd+Shift+P` → *Open Agents Window* |
| **@Past Chats** | Pull **context** from an old chat into the **current** chat without switching tabs |

**Agent Tabs (Editor):** Multiple chats side-by-side or in a grid (Cursor 3.0+).

---

## 5. Name an Agent vs chat title

These are easy to confuse:

| Concept | What it is |
|---------|------------|
| **Chat / conversation title** | Label on the tab or in history — rename with steps in §3 |
| **Model picker selection** | Which AI model runs **this** chat (`Cmd+/`) |
| **Agent mode** | How the assistant behaves (Agent vs Ask vs Plan vs Debug) — not a separate “named agent” unless you use cloud Agent tabs |

For this project, **rename the chat** to the track name (`Designer — …`, `8080 — …`, `Website — …`). The model can stay on your usual default.

---

## 6. Skills, hooks, rules, MCP

| Concept | What it is | Where it lives (this project) |
|---------|------------|-------------------------------|
| **Rules** | Short persistent guidelines injected into every prompt | **Project:** `.cursor/rules/*.mdc` (e.g. `tawala-work-scopes.mdc`, `tawala-project-map.mdc`) |
| **Skills** | Longer on-demand workflows (`SKILL.md`) | **Project:** `.cursor/skills/`; **Personal:** `~/.cursor/skills/`; **Built-in (do not edit):** `~/.cursor/skills-cursor/` |
| **Hooks** | Scripts at agent lifecycle events (before shell, after edit, etc.) | **Project:** `.cursor/hooks.json` + `.cursor/hooks/`; **User:** `~/.cursor/hooks.json`; see skill `~/.cursor/skills-cursor/create-hook/` |
| **MCP** | External tools (browser, app control, APIs) | **Project:** `.cursor/mcp.json`; **Global:** `~/.cursor/mcp.json`; **Customize** sidebar in Cursor |

**How they affect context:** Rules, skills, and MCP catalogs consume part of the context ring baseline **in every chat** in this repo. They do not replace your paste opener — still say which track you want.

**Customize hub:** Cursor sidebar → **Customize** — install plugins, rules, skills, MCP, hooks.

**Rules vs skills:** Rules = constraints (*“use designer-web for Phase 4”*). Skills = procedures (*“create a commit following these steps”*).

---

## 7. Billing and limits

| Need | Where |
|------|--------|
| **Usage dashboard** | [cursor.com/dashboard](https://cursor.com/dashboard) → **Usage** |
| **Billing / plan** | [cursor.com/dashboard/billing](https://cursor.com/dashboard/billing) or **Cursor Settings** (`Cmd+Shift+J`) → Account |
| **Pricing tiers** | [cursor.com/pricing](https://cursor.com/pricing) |

Plans (Hobby, Pro, Pro+, Ultra, Teams) include a monthly **API usage budget** at provider rates. **Auto** and **Composer** may have separate allowances. Exact numbers change — always check **Usage** in settings, not this doc.

**Context ring % is NOT billing.** The ring is per-chat model memory; billing is token/API usage on the dashboard.

---

## 8. Monitoring and gating

| Signal | Meaning |
|--------|---------|
| **Usage dashboard** | Remaining included usage, on-demand spend, per-model breakdown |
| **In-editor notification** | Often appears when monthly included usage is exhausted |
| **At limit** | You may need to enable **on-demand** billing, raise spend cap, wait for reset, or upgrade — see [overages help](https://cursor.com/help/account-and-billing/overages.md) |

**Team accounts:** Admins can set spend alerts on the team dashboard.

**What does not gate:** Hitting 90% context in one chat does not block billing-tier access — it only affects that conversation’s memory (see §2).

---

## 9. What carries between chats vs what does not

### Carries (same repo / account)

| Persists | Notes |
|----------|--------|
| **Project rules** | `.cursor/rules/` — every new chat in AI-Tawala |
| **User rules** | Cursor **Customize** — all your projects |
| **Skills, MCP, hooks** | On disk; agent loads when relevant |
| **Codebase index** | Same workspace |
| **Git state, files, terminals** | Real filesystem |
| **`docs/ROADMAP.md`** | Your shared status dashboard — update when parking tracks |
| **Default model** | Cursor Settings → Models |

### Does not carry (new chat = fresh conversation)

| Resets | Notes |
|--------|--------|
| **Message history** | Unless you `@Past Chats` or paste a handoff opener |
| **Tool results** from prior chat | Re-read files if needed |
| **In-chat memory** | Agent does not automatically remember another tab |
| **Context ring %** | Starts lower; rules/MCP still use baseline |
| **Summaries** | Only exist in the chat where `/summarize` ran |

**Handoff pattern for AI-Tawala:** Renamed chat + opener from [`CHAT_HANDOFF.md`](CHAT_HANDOFF.md) + `@Past Chats` when you need prior context.

---

## Quick reference

```
New chat:        Cmd+N / Cmd+R
Toggle panel:    Cmd+L
Rename chat:     Right-click tab → Rename  OR  sidebar ⋯ → Rename
Context full:    /summarize  or  new chat
Usage $:         cursor.com/dashboard
Pricing:         cursor.com/pricing
Project rules:   .cursor/rules/
Handoff openers: docs/CHAT_HANDOFF.md
Status:          docs/ROADMAP.md
```
