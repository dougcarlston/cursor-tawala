# Tawala Designer — Startup & Form Canvas

*From live **Tawala Project Designer #251 (DEV)** walkthrough, June 2026.*  
*See also `DESIGNER_MENU_SPEC.md` for menus, palettes, and explorer. Process and document depth: `DESIGNER_PROCESS_STATEMENTS_*.md`, `DESIGNER_DOCUMENT_EDITOR.md`.*

---

## Terminology

| Term | Meaning |
|------|---------|
| **Form canvas** | Center MDI window **Form - [name]** on the **Design** tab — where form items are placed and edited. |
| **Process editor** | Center window **Process - [name]** — `DESIGNER_PROCESS_STATEMENTS_*.md` |
| **Document editor** | Center window **Document - [name]** — `DESIGNER_DOCUMENT_EDITOR.md` |

Each form window has **Design** (default) and **Preview** tabs.

---

## 1) Cold startup

- Title: `Tawala Project Designer - Untitled`
- **Project Explorer:** empty `Forms`, `Processes`, `Documents` folders
- **Center:** large empty grey area (no MDI child open yet)
- **Fields:** `Variables` folder only (selected)
- **Toolbar:** New, Open, Save, Run/Deploy active; Cut/Copy/Paste/Print/Undo/Redo greyed out

---

## 2–3) New Project dialog

**Triggers:** File → New Project…, toolbar New icon, **Ctrl+N**

### Project type (left tree)

Under **All:**

- Basic
- Activities
- Meetings and Gatherings
- Polls and Surveys

### Templates (right pane, by category)

**Basic**

| Template | Description |
|----------|-------------|
| Empty | Start with an empty project. |
| Form with Process | |
| Form, Process and Document | |

**Activities**

| Template |
|----------|
| Sign-up Sheet |
| Sign-up Sheet with E-mail |

**Meetings and Gatherings**

| Template |
|----------|
| Get Together |
| Potluck |

**Polls and Surveys**

| Template |
|----------|
| Multiple Questi… *(truncated in UI)* |
| Simple Survey |

**Buttons:** OK, Cancel  
**Description area:** updates for selected template (e.g. “Start with an empty project.” for Empty).

---

## 4) First form after empty project

After **Empty** template + OK:

- Explorer: **Form 1** appears under Forms (selected)
- Center: **Form - Form 1** window opens automatically
- **Design** tab active; canvas shows blue hint:

  > Drag items from the palette on the left to create your form.

- **Items** palette visible (form selected)
- Format toolbar mostly greyed (nothing to format yet)

**Note:** New Form can also be created later via explorer toolbar or File → Add New → Form.

---

## 5) Drag Heading onto canvas

First item dropped from **Items → Heading**:

| UI element | Behavior |
|------------|----------|
| Orange label box | **H1** (item design label) |
| Text area | Placeholder: `[Replace this with heading of your own.]` (selectable/editable) |
| **Heading Type:** dropdown | **Main** (default) or **Sub** |

---

## 6) Rename project / form and edit heading

- Explorer: form renamed **Form 1** → **First Project** (slow double-click or F2 rename)
- Heading text edited to **Single Question Project**
- **Fields** tree gains **First Project** folder under Variables area

*(Window title follows form name: **Form - First Project**.)*

---

## 7) Heading Type: Main vs Sub

**Heading Type** dropdown on canvas:

| Value | Meaning |
|-------|---------|
| **Main** | Primary heading style |
| **Sub** | Subheading style |

---

## 8) Preview tab (broken offline — owner confirmed)

**Preview** tab on the form window is meant to show live web rendering of the form.

**Owner behavior (June 2026):** Preview **never shows the form** — only the error dialog:

> *An error occured while generating the preview.*  
> *The remote name could not be resolved: 'www.tawala.com'*

(**Retry** may appear on the dialog.) No other Preview behavior observed on the Jan 2011 build without www.tawala.com.

Legacy Preview POSTs to Tawala’s hosted preview API. **Workaround:** deploy to local 8080 (`scripts/deploy-tawala-template.mjs`) and open the start URL in a browser. Browser Designer should use **local runtime** (5173/8080) for Preview instead.

---

## Startup paths (summary)

```
Cold start (Untitled)
  → New Project (Ctrl+N / menu / toolbar)
      → Pick template (Empty, Get Together, Simple Survey, …)
          → Empty: Form 1 auto-opens
              → Drag items from Items palette
              → Design tab editing
              → Preview tab (needs local runtime fix)
```

---

## Browser Designer implications

| Legacy behavior | Target for `designer-web` |
|-----------------|---------------------------|
| New Project templates | Optional later; start with Empty + DirtBowl sample |
| Auto-open Form 1 | Consider on new project |
| Canvas placeholder hint | Match copy when form has zero items |
| H1 + Main/Sub + inline edit | Partially in `FormEditor`; needs canvas UX |
| Preview → tawala.com | Route to local runtime preview |
| Rename form in explorer | Not implemented |

---

## Next documentation (planned)

- **Item properties** per palette type (Heading, Text, FIB, MC, …)
- **Statement properties** (If, Set, Get, …)
- Drag-from-Items vs Insert menu vs double-click

---

*Last updated: June 2026.*
