# Designer — Insert menu, dialogs, and functions

Captured from legacy **Tawala Project Designer #251 (DEV)** screenshots and owner notes (June 2026).

The **Insert** menu on the main menu bar is **context-sensitive**: its contents and enable states depend on whether a **Form**, **Process**, or **Document** is active.

Related: `DESIGNER_MENU_SPEC.md`, `DESIGNER_DOCUMENT_EDITOR.md`, `DESIGNER_UI_REFERENCE.md`.

---

## Insert menu by context

### Form (MDI child active)

| # | Item | Default state | Enabled when |
|---|------|---------------|--------------|
| 1 | Heading | Active | Always (Design tab) |
| 2 | Text | Active | Always |
| 3 | Fill in the Blank | Active | Always |
| 4 | Multiple Choice | Active | Always |
| 5 | Hidden Field | Active | Always |
| 6 | Page Break | Active | Always |
| 7 | Skip Instructions | Active | Always |
| | *(separator)* | | |
| 8 | Image… | Greyed | Cursor in a **rich-text** area (owner: “text window”; source: `CanInsertImage`) |
| 9 | Invitation… | Greyed | Cursor in **Text** item body (`TextItemView`) |
| 10 | Hyperlink… | Greyed | Cursor in **Text** item body |
| 11 | Function… | Greyed | Cursor in **Text** item body |

**Field** does **not** appear on the Form Insert menu at all.

**Format toolbar `fx`** on forms follows the same rule as **Insert → Function…** — active when cursor is in a **Text** item text box (owner observation; source: `enableOrDisableInsertFunctionButton`).

Top **seven** items match the **Items** palette. (**File Uploader** omitted from browser palette — owner Jul 17.)

### Process (MDI child active)

Insert lists **statement types** only (If, Show, Send, Append, Get, ForEach, Delete, Set, Comment) — see `DESIGNER_MENU_SPEC.md` § Insert B and `DESIGNER_PROCESS_STATEMENTS_*.md`.

No Image, Invitation, Hyperlink, Function, or Field.

### Document (MDI child active)

| Item | Enable rules |
|------|--------------|
| **Field** | When a **field leaf** is selected in the **Fields** palette (Document-only menu item). Browser Jul 17: enabled from palette highlight; inserts at last document caret (same as double-click). |
| **Image…** | When document editor active |
| **Invitation…** | Available in document context |
| **Hyperlink…** | Available in document context |
| **Function…** | When project has **≥1 form**; same as **fx** on document format toolbar |

**Document Field — intentional UX (owner Jul 17, not a bug):**

- Open Document and **do not** place a caret → **double-click** a Fields leaf does nothing (no error). **Drag-and-drop** still places the field where dropped.
- Clicking a Fields leaf focuses the palette (clears an empty Document caret). A subsequent double-click may insert at the default top-left / last landing spot — expected **only for the active MDI Document**.
- Bringing another Document to front (Project Explorer or title bar) **clears** a stale caret/target from a background Document — Fields / Insert → Field must not write into the prior window.
- Title-bar / Explorer activate does **not** invent a caret; click in the document body first.
- **Insert → Field** requires a Fields leaf **and** a live caret in the **active** Document. If the caret is missing (or only a background Document still had focus), status bar shows *Place the cursor in the document text first* (Status Bar must be visible under View).
- Clear Fields leaf highlight: click a form/Variables folder header, click empty Fields chrome, or press Escape — then Insert → Field greys out.

No form item types (Heading, Text, FIB, etc.).

---

## Insert → Image…

Submenu (Form and Document):

| Item | Behavior |
|------|----------|
| **From your PC…** | Insert GIF/JPG/PNG from disk into Form Text or Document. Stores bytes in project `images[]`; Design shows `<img data-tawala-image-id>`; Deploy emits `<image id width height/>` plus `<images><imagedef><imagedata>`. |
| **From the Web…** | Opens **Configure Function** for **DISPLAY IMAGE** (URL or field holding a URL). Renamed Jul 17 — legacy “or Tawala Upload” dropped; File Uploader form item is out of the browser palette. |

### Smoke — From your PC (Jul 16)

1. New project → Form Text → Insert → Image → **From your PC…** → pick a small PNG.
2. Design canvas shows the image; status bar notes `image1`. Large photos are scaled down on insert (max ~480px / editor width).
3. Click the image → drag the **lower-right** handle to resize (aspect locked). Deploy keeps the new width/height. Idle Form Text must still show the image (image-only bodies are not treated as empty — Jul 16).
4. Save JSON → reopen → image still visible at that size; `project.images` has matching `imagedef`.
5. Deploy to `:8080` → form shows the same graphic (not a broken image).
6. Confirm **From the Web…** still opens DISPLAY IMAGE Configure (separate path).

Screenshot: `assets/Insert_-_Image_-_From_the_Web-*.png`

---

## Insert → Invitation…

**Dialog title:** Insert Invitation

Sends a user a **Start link** to this project or another project.

| Control | Purpose |
|---------|---------|
| **Form:** | Dropdown — target form (same row as Project) |
| **in** | Literal connector between Form and Project |
| **Project:** | Dropdown — **(Current Project)** or other project |
| **Display Text:** | Link text shown to the invitee (optional; falls back to form name) |
| **Make this a private invitation** | Checkbox |
| Explanatory text (centered) | Exact legacy copy: *Enter text or a field to be placed in the special variable "_InviteeID" when someone responds to this invitation. That text, or the value of the field, will be available in the "_InviteeID" variable when your invitee responds by clicking the Invitation link.* |
| Text field below | Private value / field for `_InviteeID`; **dimmed** when private unchecked |

**OK** / **Cancel** (centered)

Screenshot: [`assets/Insert_Invitation.png`](assets/Insert_Invitation.png)

**Deploy:** emits `<font color="000080"><u><invitation form="…" project="…">…</invitation></u></font>` inside Form Text / Document paragraphs. Runtime must show a live `<a href=…>` (not plain text). If Deploy shows plain text only, restart the Designer API on `:3001` so it loads current `documentHtmlToXml.mjs`.

---

## Insert → Hyperlink…

**Dialog title:** Hyperlink

Inserts a URL hyperlink into rich text (Form Text item or Document).

| Control | Purpose |
|---------|---------|
| **Url:** | Destination URL (label left of field) |
| **Display text:** | Optional |
| Italic note | `(optional; if you leave this blank the full URL or filename will be shown)` |
| **Open in new browser window.** | Checkbox (trailing period matches legacy) |
| Separator | Thin horizontal rule |
| **Display link conditionally** | Checkbox — enables condition section |
| **Display link only when** | Field (green box), operator dropdown, value, **+** / **−** |

**OK** / **Cancel** (centered)

Screenshot: [`assets/Insert_Hyperlink.png`](assets/Insert_Hyperlink.png)

**Deploy:** emits `<font color="000080"><u><link>…</link></u></font>`. Runtime must show a live `<a href=…>` (with `target="_blank"` when new-window is set).

---

## Insert → Function… / toolbar **fx**

**Dialog title:** Insert Function

Two-step flow: pick function → **Configure Function** dialog for parameters.

### Smoke — fx / Insert → Function (Jul 16)

1. Open a project that already has Form Text with an image and/or existing function chips (e.g. `MCQ.json`).
2. Click into a Text body (not an MCQ question). **fx** and **Insert → Function…** should stay enabled after clicking Project Explorer or Items (row still selected).
3. Place the caret *after* an existing function chip (not on it) → **fx** / Insert → Function opens the **Insert Function** list (not Configure).
4. Click a function chip → Configure opens for that function.
5. New empty Text → Insert → Function still opens the list.

### Insert Function dialog

| Control | Purpose |
|---------|---------|
| **Select a category:** | Dropdown (see categories below) |
| **Select a function:** | Scrollable list; selection shows description in lower pane |
| **OK** / **Cancel** | OK proceeds to Configure Function |

Screenshots: `assets/Insert_-_Function-*.png`, `Insert_-_Function_-_Database-*.png`, `Insert_-_Function_-_Math-*.png`, `Insert_-_Function_-_Payments-*.png`, `Insert_-_Function_Tables-*.png`, `AllFunctions1-*.png`, `AllFunctions2-*.png`, `AllFunctions3-*.png`

### Categories and functions (owner’s Jan 2011 build)

#### All — complete list (17 functions)

Owner confirmed via scrolled screenshots (`AllFunctions1`–`3`). Display order in dialog:

| # | Function |
|---|----------|
| 1 | CATEGORIZER |
| 2 | DISPLAY IMAGE |
| 3 | DISPLAY MULTIPLE-CHOICE QUESTION RESPONSES |
| 4 | EXPORT TEAM ROSTER |
| 5 | FORM RECORD COUNT |
| 6 | LINK TO PROJECT DETAILS IN MY TAWALA |
| 7 | MULTIPLE QUESTION LIST |
| 8 | PAYPAL SINGLE ITEM PURCHASE BUTTON |
| 9 | PROJECT EMAIL COUNT |
| 10 | QUESTION CORRELATION TABLE |
| 11 | RANKED MULTIQUESTION RESPONSE LIST |
| 12 | RANKED RESPONSE COUNTS |
| 13 | RANKED RESPONSE NAME |
| 14 | RESPONSE BAR GRAPH |
| 15 | RESPONSE TOTALS |
| 16 | SINGLE QUESTION LIST |
| 17 | SUM |

**Configure screenshot capture (owner, July 10, 2026):** **All 17 done.**

#### How **All** relates to specialty categories

**All is not computed at runtime** — it is a separate explicit list in the function repository XML. In practice on the owner’s build:

| Set | Count | Notes |
|-----|-------|-------|
| **Tables** + **Database Functions** + **Math Functions** + **Payments** | 12 | Every specialty function appears in **All** |
| **All-only** (not in any specialty menu) | 5 | DISPLAY IMAGE, DISPLAY MULTIPLE-CHOICE QUESTION RESPONSES, EXPORT TEAM ROSTER, LINK TO PROJECT DETAILS IN MY TAWALA, PROJECT EMAIL COUNT |
| **All** total | **17** | 12 + 5 |

So **All ⊇ union of the four specialty categories**, plus five additional entries. **EXPORT TEAM ROSTER** appears only under **All** on the Jan 2011 build (present in deployed webapp config; not in repo test XML).

#### Database Functions

| Function | Description (from dialog) |
|----------|---------------------------|
| **FORM RECORD COUNT** | Returns the number of responses received by a specified Form. |
| **RANKED RESPONSE COUNTS** | Returns the number of responses that selected the Nth most popular choice… |
| **RANKED RESPONSE NAME** | Displays the text of the most popular response choice… |

#### Math Functions

| Function | Description |
|----------|-------------|
| **SUM** | Calculates the sum of the values in a fill-in-the-blank question or hidden field. |

#### Payments

| Function | Description |
|----------|-------------|
| **PAYPAL SINGLE ITEM PURCHASE BUTTON** | Creates a button that allows you to sell a single item, request payment for services or receive a donation. Note: To use this function in your projects you must arrange for online payment support with Tawala Systems, Inc. Visit our website for further details. |

#### Tables

| Function | Description |
|----------|-------------|
| **CATEGORIZER** | Provides an interactive (drag and drop) method for grouping stored records into categories. |
| **MULTIPLE QUESTION LIST** | Displays a table showing a list of responses to specified questions in a Form. |
| **QUESTION CORRELATION TABLE** | Table of MCQ choices per respondent; optional second MCQ for preferred choice. |
| **RANKED MULTIQUESTION RESPONSE LIST** | Most common MCQ response + associated field list; correlates two MCQs. |
| **RESPONSE BAR GRAPH** | Bar graph of count and percentage per MCQ response. |
| **RESPONSE TOTALS** | Table of count per MCQ response. |
| **SINGLE QUESTION LIST** | List of all responses to a FIB or hidden field. |

> **Note:** Per-function **Configure** dialogs are documented below as the owner captures them. Parameter source of truth also lives in `TawalaDesigner/.../display-component-repository.xml` and C# function runtime.

Shared chrome (all Configure Function dialogs):

| Region | Content |
|--------|---------|
| Title | **Configure Function** (fx icon) |
| Left | Parameter fields |
| Right | Yellow help pane — function name, description, focused-field help, often red **REQUIRED** |
| Footer | **OK** (greyed until required satisfied) / **CANCEL** |

---

## Configure Function: CATEGORIZER (`categorizer`)

Owner screenshot July 10, 2026. Owner note: unsure how to use in practice — not needed for the four sample apps; SportsDashboards-style interactive grouping.

**Description (help pane):** Provides an interactive (drag and drop) method for grouping stored records into categories.

### Left pane

| Control | Type | Notes |
|---------|------|-------|
| **Category Names** | Blank/hidden field (green when focused) | Help: field whose values are category names in the Target table. Link: *Blank or hidden field*. **REQUIRED**. |
| **Category IDs** | Blank/hidden field | IDs stored into Category Storage Field when a record is assigned. |
| **Category Storage Field** | Blank/hidden field | On the Source list record; set to Category ID on assign. |
| **Show this form on completion** | Form dropdown | Screenshot default **Registration**. Shown after user **Save Changes**. |
| **Column *n*** | Collection | **Heading**, **Contents**, link **+ Column is always displayed**. Source-table columns. |
| **Show only fields from records where** | Conditions | Field / op / value / **+** **−**. |

### Footer

Green **+** / **−**, up/down (columns); **OK** greyed when incomplete; **CANCEL**.

Screenshot: [`assets/Function_-_Categorizer.png`](assets/Function_-_Categorizer.png)

### Browser gaps

Catalog params present. Document HTML→XML: **not emitted** (comment stub) — **Deferred.** Column UI matches Multiple Question List (+/−/↑/↓ toolbar); per-column “always displayed” condition editor still deferred.

---

## Configure Function: DISPLAY IMAGE (`display-image`)

Owner screenshot July 10, 2026. Owner: **simple**. Also reachable from **Insert → Image → From the Web…**.

**Description:** Displays an image from an `http://` / `https://` URL (or a field that holds one). For a file on disk, use **Insert → Image → From your PC…** (project embed) — not this function.

| Field | Required | Help (focused on Image Source) |
|-------|----------|--------------------------------|
| **Image Source** | **REQUIRED** | Field, variable or literal URL (`http://` / `https://`). Link: *A compound expression*. (Repository XML name: **Image URL** / id `source`.) |
| **Display width** | Optional | **Whole pixels** (not inches). Blank = natural width. Prefer width alone so height scales. |
| **Display height** | Optional (hidden in browser Configure Jul 13) | Legacy still supports it; UI hides it so width-only scaling stays simple. |
| **Alternative name** | Optional | Hover text in browsers |

No column toolbar. **OK** greyed until Image Source filled.

**Owner Jul 13 / Update Jul 17:** Design canvas shows a `<<DISPLAY IMAGE(...)>>` token only. **Form Preview** shows a dashed placeholder box sized to width (and height if set; else banner height 80px) with the **image name** centered (Alternative name, else URL filename / field name — not the full URL). **Deploy** (Java) renders the real `<img>`. Local files: **Insert → Image → From your PC…**. File Uploader form item is out of the browser palette (deferred in specs only). Owner: drag-in images are rare (headers); prefer editing graphics outside Designer, then paste a URL.

Screenshot: [`assets/Function_-_Display_Image.png`](assets/Function_-_Display_Image.png)

### Browser gaps

Configure fields match (Display height hidden — width-only so Deploy keeps aspect ratio). Document **and Form Text** HTML→XML: **emits** `<display-image>` (Jul 13). Empty Document canvas husks are omitted from Deploy XML (Jul 13) so they don’t stack as blank lines. Design mode is token-only. Form Preview: sized name-centered placeholder box (Jul 13; Design/Preview only — not Deploy). Deploy: live image. **Owner smoke Jul 18: Passed.**

---

## Configure Function: DISPLAY MULTIPLE-CHOICE QUESTION RESPONSES (`display-mcq-label`)

Owner screenshot July 10, 2026. Owner: **simple**.

**Description:** Displays the text of the response choice in a multiple-choice question.

| Field | Type | Notes |
|-------|------|-------|
| **Multiple Choice Field** | MCQ field | Empty until drop/pick. **REQUIRED**. |
| **Display** | Dropdown | **only labels of selected choices** (`label_only`); **all choices, using the question layout** (`all_choices`). Help: *Select how the responses will be displayed* / *Choose from the drop-down list* / **REQUIRED**. |

Screenshot: [`assets/Function_-_Display_MCQ_Responses.png`](assets/Function_-_Display_MCQ_Responses.png)

### Browser gaps

Catalog + Configure present. Document **and Form Text** HTML→XML: **emits** `<display-mcq-label>` (Jul 13). Runtime shows labels only after that MCQ has answers in the submission (typically a later page or Document after Submit — blank / all “Not selected” on the unanswered form). **Owner smoke Jul 18: Passed.**

**Deploy spacing (Jul 18):** Design canvas gaps between placed DISPLAY MCQ chips are absolute `top` positions; Java lays Document paragraphs in flow. Export (a) **sorts placed lines by `top`/`left`** so drag-reordered chips keep Design order on Deploy, (b) keeps intentional Double-Return blanks (`data-doc-blank`) as spacer paragraphs, and (c) injects blank paragraphs from large `top` gaps. **Restart the Designer API** (`npm run keep`) after pulling so Deploy uses the new exporter, then Redeploy.

**`label_only` vs `all_choices`:** Java `label_only` prints selected choice **text** (e.g. `Dog`); `all_choices` uses the question’s checkbox layout. If Deploy shows checkboxes under a `label_only` chip, the lines were likely out of DOM order before the sort fix — Redeploy after API restart.

**`all_choices` Deploy:** Java emits `/images/checkbox_on.gif` and `/images/checkbox_off.gif`. Those assets were missing from local Tomcat (404 → broken-image icons + overlapping alt text). Patched into `docker/tomcat/images/` + Dockerfile (Jul 16); hot-copied into running `tawala-tomcat`. Rebuild image for permanence. **Smoke:** open `http://localhost:8080/images/checkbox_on.gif` — must be 200 before Redeploy.

**From-your-PC picture on Form Text:** Design/Preview can show `src=data:…` even when `project.images[]` is empty; Deploy needs `<imagedef>`. Jul 16: Deploy harvests base64 from HTML `data-tawala-image-id` embeds into `<images>` if missing from `images[]`. Redeploy after API restart.

**Deploy smoke (same form as the MCQ):** Configure **Display** = **all choices, using the question layout** → Redeploy → you should see checkbox glyphs + choice text under the Text item (no question prompt). Default **only labels of selected choices** is intentionally **blank** on that fill-in form until the MCQ has been submitted (put labels mode on a Document/review page after Submit).

**Deploy blank while Design shows the token (Jul 16):** If the function token sits inside a styled `<span>` (palette face/size/color), a naive `</span>` match closed the outer span too early and Deploy XML kept only the token text — no `<display-mcq-label>`. Fixed: nested tag matching + unwrap nested `<font>` so Java does not drop the component. Also: browsers often store `color: rgb(0, 0, 0)` — Java `Font` requires 6-digit hex and **rejects the whole upload** on `rgb(...)`; convert to hex on export.

Tables-category MCQ functions (RESPONSE TOTALS, RESPONSE BAR GRAPH, QUESTION CORRELATION TABLE, etc.): HTML→XML **emits** real elements (Jul 13); conditions infer `<form name>` from the Question field when no separate form param.

---

## Configure Function: EXPORT TEAM ROSTER (`export-team-roster`)

Owner screenshot July 10, 2026. Owner: **designed only for SportsDashboards** (All-only on Jan 2011 build; not in repo `display-component-repository.xml` test catalog).

**Description (help pane):** Exports Team Roster

| Field | Required | Help |
|-------|----------|------|
| **Link text** | **REQUIRED** | Text of the link that will create the roster. Link: *A compound expression*. |
| **Team ID** | (shown) | Team identifier for the roster export. |

Screenshot: [`assets/Function_-_Export_Team_Roster.png`](assets/Function_-_Export_Team_Roster.png)

### Browser gaps

Catalog entry exists but **`parameters: []`** — Configure shows “no parameters.” Need **Link text** + **Team ID** to match legacy. Document HTML→XML: **not emitted** (comment stub) — **Deferred.** Low priority outside SportsDashboards.

---

## Configure Function: FORM RECORD COUNT (`record-count`)

Owner screenshot July 10, 2026. Database Functions category. Document HTML→XML already emits real `<record-count>`.

**Description:** Returns the number of responses received by a specified Form.

| Field | Type | Notes |
|-------|------|-------|
| **Form** | Form dropdown | Screenshot: **Registration**. **REQUIRED**. |
| **Count only the records where** | Conditions | Field / op / value / **+** **−**. Screenshot example: `Registration:Grade` **equals** `6`. Help: only matching records included in the count. **REQUIRED** (conditions block). |

**OK** enabled when Form + conditions satisfy requirements (screenshot shows OK active with sample condition filled).

Screenshot: [`assets/Function_-_Form_Record_Count.png`](assets/Function_-_Form_Record_Count.png)

### Browser gaps

Configure matches (Form + conditions). XML export: **yes** (Where fields as `Record:Form:Field`; multi-row nested `<and>`/`<or>`; **Jul 18:** `FIB1:a`-style blanks qualify as `Record:Form 1:FIB1:a`). Preview: filtered session count — Where ops + FIB Item:blank lookup fixed Jul 18.

**Where field drop (Jul 18):** Configure Where field box uses **replace** (bare `Form:Field`), not insert-at-caret — dropping a new field no longer appends onto the previous one (which broke matching and confused Document Deploy). Focus selects the whole field for easy overwrite.

---

## Configure Function: LINK TO PROJECT DETAILS IN MY TAWALA (`link-to-project-details`)

Owner screenshot July 10, 2026. All-only; navigates to My Tawala project details (needs www.tawala.com / hosted account in real use).

**Description:** Creates a link to navigate to the project details of the current project.

| Field | Type | Notes |
|-------|------|-------|
| **Link text** | Expression | **REQUIRED**. |
| **Open in** | Dropdown | **the current window** (`current-window`); **a new window** (`new-window`). Help: *Determines how the page will be open.* / *Choose from the drop-down list* / **REQUIRED**. |

Screenshot: [`assets/Function_-_Link_to_Project_Details.png`](assets/Function_-_Link_to_Project_Details.png)

### Browser gaps

Configure fields match. Document HTML→XML: **not emitted** (comment stub) — **Deferred.** Runtime depends on hosted My Tawala — limited value for local 8080.

---

## Configure Function: MULTIPLE QUESTION LIST (`itemization-table`)

Owner screenshot July 10, 2026 (legacy Designer). Function id `itemization-table` — used by **Sign-up Sheet** (signup list) and **Potluck** (who’s bringing what).

**Title:** Configure Function  
**Right pane header:** MULTIPLE QUESTION LIST  
**Description:** Displays a table showing a list of responses to specified questions in a Form.

### Left pane (parameters)

| Control | Type | Default (fresh) | Notes |
|---------|------|-----------------|-------|
| **Show link to print the table** | Dropdown | **no** | Choices: no / yes. Focus shows field help + **REQUIRED** (red) in the yellow pane. |
| **Show link to export the table to Excel** | Dropdown | **no** | Choices: no / yes. |
| **Excel Template** | Dropdown | **Default** | Visible in owner’s Jan 2011-era UI; **not** listed on `itemization-table` in `display-component-repository.xml` v2 — treat as later UI chrome unless confirmed in C#. |
| **Column *n*** (bordered group) | Collection | Starts at Column 1 | **Heading** (text), **Contents** (field), link **+ Column is always displayed** (per-column display condition). |
| **Show only fields from records where** | Conditions | One blank row | Field box, operator dropdown, value box, **+** / **−** |

### Footer toolbar

| Control | Role |
|---------|------|
| Green **+** / **−** | Add / remove columns |
| Up / Down arrows | Reorder columns |
| **OK** (green check) | Greyed until required fields satisfied |
| **CANCEL** (red X) | Always active |

### Help pane quirks

- Field help for **Show link to print the table** repeats the label twice (owner: accidental duplication), then blue *Choose from the drop-down list*, then red **REQUIRED**.
- Same two-column layout as other Configure Function dialogs (params left, yellow help right).

Screenshot: [`assets/Function_-_Multiple_Question_List.png`](assets/Function_-_Multiple_Question_List.png)

### Browser Designer (`designer-web`) — same function

Owner (July 10): browser Configure is **functionally the same** for the core params, but legacy is easier to scan visually.

| Area | Legacy | Browser today |
|------|--------|----------------|
| Print / Excel export dropdowns | Yes | Yes (`show-print-control`, `show-export-control`) |
| Excel Template | Shown (**Default**) | **Missing** |
| Columns | Bordered “Column 1” with Heading / Contents / “always displayed” link; +/−/↑/↓ toolbar | Bordered Column *n* groups; footer **+** / **−** / ▲ / ▼ with hover tips (Jul 15). “Always displayed” link present (display-condition editor still deferred) |
| Conditions | “Show only fields from records where” | Present (`FunctionConditionsEditor`) |
| OK greyed until valid | Yes | Yes |
| Yellow help + REQUIRED | Yes (with duplicate label bug) | Help pane present; REQUIRED styling may differ |
| Document HTML → XML | Real `<itemization-table>` | **Yes** — SignupSheet owner-passed Jul 16 |

---

## Configure Function: PAYPAL SINGLE ITEM PURCHASE BUTTON (`paypal-single-item-button`)

Owner screenshots July 10, 2026 (button-type list + style dropdown). Requires Tawala online payment arrangement — out of scope for local sample-app smoke unless payment is wired.

**Description:** Creates a button that allows you to sell a single item, request payment for services or receive a donation. Note: To use this function in your projects you must arrange for online payment support with Tawala Systems, Inc. Visit our website for further details.

| Field | Type | Notes |
|-------|------|-------|
| **Type of button** | Dropdown | **Buy**; **Buy (displays credit card logos)**; **Pay**; **Pay (displays credit card logos)**; **Donate**; **Donate (displays credit card logos)**. Help: *Pick the transaction type…* / **REQUIRED**. |
| **Item/service description.** | Expression | Trailing period in legacy label. **REQUIRED**. |
| **Transaction amount.** | Expression | Trailing period in legacy label. **REQUIRED**. |
| **Return to this form on success.** | Form dropdown | Screenshot often **Registration**. |
| **Return to this form on failure to pay.** | Form dropdown | |
| **Store the transaction status in this field.** | Blank/hidden | |
| **Store the transaction amount in this field.** | Blank/hidden | |
| **Select how to style PayPal screens** | Dropdown | **PayPal Style** (`paypal`); **Tawala Theme Style** (`tawala`). Help: use PayPal Style if already configured in PayPal admin. **REQUIRED**. |

Screenshots: [`assets/Function_-_Paypal_Single_Item.png`](assets/Function_-_Paypal_Single_Item.png), [`assets/Function_-_Paypal_Single_Item_Style.png`](assets/Function_-_Paypal_Single_Item_Style.png)

### Browser gaps

Catalog params match. Document HTML→XML: **not emitted** (comment stub) — **Deferred.** Runtime needs payment integration.

---

## Configure Function: PROJECT EMAIL COUNT (`project-email-count`)

Owner screenshot July 10, 2026. All-only; no parameters.

**Description:** Count all emails sent for this project.

Left pane empty. Help: **No Parameters** / *This function has no parameters.* **OK** / **CANCEL** both active.

Screenshot: [`assets/Function_-_Project_Email_Count.png`](assets/Function_-_Project_Email_Count.png)

### Browser gaps

Catalog `parameters: []` matches. Document HTML→XML already emits `<project-email-count/>`. Runtime meaningful only with mail/Send history.

---

## Configure Function: QUESTION CORRELATION TABLE (`question-correlation-table`)

Owner screenshot July 10, 2026 (DirtBowl/Registration-style sample values).

**Description:** Displays a table showing the choices made by all respondents to a multiple choice question. The left-most column contains the names of the respondents (or any fill-in-the-blank or text field you choose to display). The other columns indicate which choices were selected. Optionally, a second multiple choice question can be designated to indicate the favored choice for each respondent.

| Field | Required | Notes |
|-------|----------|-------|
| **Question with all choices** | Yes | Screenshot: `<<Record:Registration:SexMCQ>>`. |
| **Field to display in left column** | Yes | Screenshot: `<<Record:Registration:Parish>>`. |
| **Question with preferred choice** | Optional | Help explains multi-select + preferred (e.g. available dates vs preferred date). Legacy help link typo: **Multipe choice**. |
| **Display only the records where** | Conditions | Field / op / value / **+** **−**. |

Screenshot: [`assets/Function_-_Question_Correlation_Table.png`](assets/Function_-_Question_Correlation_Table.png)

### Browser gaps

Catalog + Configure present. Form Text with embedded `questionCorrelationTable` (e.g. Get Together **Report**) supports canvas rich-edit around the token and click-to-Configure (same path as Multiple Question List). Document **and Form Text** HTML→XML: **emits** `<question-correlation-table>` (Jul 13+). Template Deploy smoke: Get Together **Passed w/ caveats**.

---

## Configure Function: RANKED MULTIQUESTION RESPONSE LIST (`popular-choice-correlation-table`)

Owner screenshot July 10, 2026. Rank dropdown open: **first** / **second** / **third**.

**Description:** Computes the most common response to a multiple-choice question and displays a list of the contents of an associated field for users who have chosen that response. It also compares the response choice of one multiple-choice question with a second multiple-choice question.

| Field | Required | Notes |
|-------|----------|-------|
| **Rank** | Yes | **first** / **second** / **third**. Help: ranking of the popular choice. |
| **Main Question** | Yes | MCQ for popular-choice analysis. |
| **Second Question** | Yes | MCQ to correlate with main. |
| **Column One Contents** | Yes | Associated field listed for users who chose that response. |
| **Display only choices from records where** | Conditions | Field / op / value / **+** **−**. |

Screenshot: [`assets/Function_-_Ranked_Multiquestion_Response_List.png`](assets/Function_-_Ranked_Multiquestion_Response_List.png)

### Browser gaps

Catalog matches. Document HTML→XML: **emits** `<popular-choice-correlation-table>`. **Smoke-needed** (no sample template owner-passed yet).

---

## Configure Function: RANKED RESPONSE COUNTS (`popular-choice-count`)

Owner screenshot July 10, 2026. Database Functions category.

**Description:** Returns the number of responses that selected the most popular choice for a given multiple choice question. (Rank still selects first/second/third most popular.)

| Field | Required | Notes |
|-------|----------|-------|
| **Rank** | Yes | **first** / **second** / **third**. |
| **Question** | Yes | MCQ to analyze. |
| **Include only the records where** | Conditions | Optional in repository XML (`required="false"`); UI still shows the block. |

Screenshot: [`assets/Function_-_Ranked_Response_Counts.png`](assets/Function_-_Ranked_Response_Counts.png)

### Browser gaps

Catalog matches. Document HTML→XML: **emits** `<popular-choice-count>`. **Smoke-needed.**

---

## Configure Function: RANKED RESPONSE NAME (`popular-choice-display`)

Owner screenshot July 10, 2026. Database Functions category.

**Description:** Displays the text of the most popular response choice to a specified multiple-choice question.

| Field | Required | Notes |
|-------|----------|-------|
| **Rank** | Yes | **first** / **second** / **third**. |
| **Question** | Yes | MCQ whose popular choice text to show. |
| **Display only text from records where** | Conditions | Field / op / value / **+** **−**. |

Screenshot: [`assets/Function_-_Ranked_Response_Name.png`](assets/Function_-_Ranked_Response_Name.png)

### Browser gaps

Catalog matches. Document HTML→XML: **emits** `<popular-choice-display>`. **Smoke-needed.**

---

## Configure Function: RESPONSE BAR GRAPH (`choice-tally-table`)

Owner screenshot July 10, 2026. Used by **Simple Survey** (results). Form Text JSON path and Document HTML→XML both emit `<choice-tally-table>`.

**Description:** Displays a bar graph showing both the number and percentage of each response to a multiple-choice question.

| Field | Required | Notes |
|-------|----------|-------|
| **Question** | Yes | MCQ for counts/percentages. Help link typo: **Multipe choice**. |
| **Include only the records where** | Conditions | Field / op / value / **+** **−**. |

Screenshot: [`assets/Function_-_Response_Bar_Graph.png`](assets/Function_-_Response_Bar_Graph.png)

### Browser gaps

Catalog matches. Document **and Form Text** HTML→XML: **emits** `<choice-tally-table>`. Template Deploy smoke: Simple Survey **Passed**; Multiple Question Survey **Passed** (owner Jul 2026).

**Design / Preview (Jul 16):** Structured Form Text `choiceTallyTable` (Simple Survey Report) was invisible on the Design canvas — now shows a **RESPONSE BAR GRAPH** token (click to Configure). Node Preview renders Choice / Count / Percentage table (+ bar) from session records.

---

## Configure Function: RESPONSE TOTALS (`response-totals-table`)

Owner Configure capture + Deploy smoke Jul 16.

**Deploy title:** each totals table is preceded by the **MCQ question text** (e.g. “Where is Greenland?”) as a bold paragraph — not a rename of the **Choice** column header. Export injects titles in `jsonToXml.mjs` (`injectResponseTotalsQuestionTitles`), including when several chips share one paragraph. Tall Preview also uses the question as the first-column header. Column headers on Deploy stay **Choice** | **Count** (`component.properties`) unless/until Java renders the question in the Tall header.

**Deploy spacing:** export also emits a blank spacer paragraph after each table; Tomcat `table.component { margin: 12px 0 }` in `default.css` reinforces equal one-line rhythm (CSS is baked into ROOT.war — rebuild or `docker cp` after CSS edits).

### Smoke — Response Totals (Jul 16)

1. Form with 3 MCQs + Text with three RESPONSE TOTALS (one per MCQ).
2. Configure → double-click Fields leaf into **Question** — fills Configure, not the Text body.
3. OK → chip font size unchanged vs before Configure.
4. Redeploy: each table has its MCQ question text **above** it; column headers still **Choice** | **Count**.
5. Redeploy / live CSS: equal gaps between stacked tables (not flush).

Owner screenshot July 10, 2026. Table Layout dropdown open.

**Description:** Displays a table showing the count of each response to a multiple-choice question.

| Field | Required | Notes |
|-------|----------|-------|
| **Table Layout** | Yes | **Tall** (`vertical`) — Choice/Count columns; **Wide** (`horizontal`) — Choice/Count rows. Deploy adds the MCQ question text above the table. |
| **Question** | Yes | MCQ whose counts to show. |
| **Include only the records where** | Conditions | |

Screenshot: [`assets/Function_-_Response_Totals.png`](assets/Function_-_Response_Totals.png)

### Browser gaps

Catalog matches. Document + Form Text HTML→XML: **emits** `<response-totals-table>` with question title paragraphs + spacers. Preview renders the same title above Choice/Count.

**Parked (owner Jul 17 — final Designer run-through):** Where clause **`<<field>>` is not blank** showed inappropriate list behavior. Capture exact wrong result on retest; see `DESIGNER_OPEN_BUGS.md` § Functions.

---

## Configure Function: SINGLE QUESTION LIST (`simple-list`)

Owner screenshot July 10, 2026.

**Description:** Displays a list showing all the responses to a fill-in-the-blank question or recorded values for a hidden field.

| Field | Required | Notes |
|-------|----------|-------|
| **Name of question or hidden field** | Yes | Help: *Blank or hidden field*. |
| **Show only responses from records where** | Conditions | |

Screenshot: [`assets/Function_-_Single_Question_List.png`](assets/Function_-_Single_Question_List.png)

### Browser gaps

Catalog matches. Document HTML→XML: **emits** `<simple-list>`. **Smoke-needed.**

---

## Configure Function: SUM (`sum`)

Owner screenshot July 10, 2026. Math Functions; used by **Potluck** (adult/kid headcount). Document HTML→XML already emits real `<sum>`.

**Description:** Calculates the sum of the values in a fill-in-the-blank question or hidden field.

| Field | Required | Notes |
|-------|----------|-------|
| **Name of question or hidden field** | Yes | Blank or hidden field to sum. |
| **Include only the records where** | Conditions | |

Screenshot: [`assets/Function_-_Sum.png`](assets/Function_-_Sum.png)

### Browser gaps

Catalog matches. Document HTML→XML: **yes** (`<sum>`). Template Deploy smoke: Potluck **Passed w/ caveats**.

---

## Function status matrix (Jul 16, 2026)

Source of truth for Document HTML→XML: `designer-web/server/documentHtmlToXml.mjs` (`default` → XML comment). Form Text structured nodes also export via `jsonToXml.mjs` where noted.

| # | Function | id | XML emit | Owner smoke / notes |
|---|----------|-----|----------|---------------------|
| 1 | CATEGORIZER | `categorizer` | **Deferred stub** | No sample need |
| 2 | DISPLAY IMAGE | `display-image` | **Yes** | **Passed** — owner Jul 18 (Configure URL → Design token → Preview placeholder → Deploy live image) |
| 3 | DISPLAY MCQ RESPONSES | `display-mcq-label` | **Yes** | **Passed** — owner Jul 18 (Configure + Deploy; spacing between stacked chips fixed same day) |
| 4 | EXPORT TEAM ROSTER | `export-team-roster` | **Deferred stub** | Empty params |
| 5 | FORM RECORD COUNT | `record-count` | **Yes** | **Preview + Where ops fixed Jul 18** (`isNotBlank` etc.); Deploy XML `Record:` fields; owner smoke |
| 6 | LINK TO PROJECT DETAILS | `link-to-project-details` | **Deferred stub** | Hosted My Tawala |
| 7 | MULTIPLE QUESTION LIST | `itemization-table` | **Yes** | **Done** — SignupSheet Jul 16 |
| 8 | PAYPAL BUTTON | `paypal-single-item-button` | **Deferred stub** | Payment integration |
| 9 | PROJECT EMAIL COUNT | `project-email-count` | **Yes** | Smoke-needed (needs Send history) |
| 10 | QUESTION CORRELATION TABLE | `question-correlation-table` | **Yes** | Get Together **Passed w/ caveats** |
| 11 | RANKED MULTIQUESTION LIST | `popular-choice-correlation-table` | **Yes** | Smoke-needed |
| 12 | RANKED RESPONSE COUNTS | `popular-choice-count` | **Yes** | Smoke-needed |
| 13 | RANKED RESPONSE NAME | `popular-choice-display` | **Yes** | Smoke-needed |
| 14 | RESPONSE BAR GRAPH | `choice-tally-table` | **Yes** | **Design+Preview Jul 16** — Report shows token + tally table; Simple/Multi Survey Deploy passed |
| 15 | RESPONSE TOTALS | `response-totals-table` | **Yes** | Smoke-needed |
| 16 | SINGLE QUESTION LIST | `simple-list` | **Yes** | Smoke-needed |
| 17 | SUM | `sum` | **Yes** | Potluck **Passed w/ caveats** |

**Insert siblings (not in the 17):** Invitation…, Hyperlink… — **wired Jul 16** (dialogs + Design tokens + Deploy XML). **Image → From your PC…** — Approach A (Jul 16): project `images[]` + Deploy `<imagedef>`. **Image → From the Web** → DISPLAY IMAGE Configure works.

### Smoke — Invitation / Hyperlink (Jul 16)

**Owner OK Jul 16** — both live and valid on Deploy (after config double-encode fix).

1. Form Text → Insert → **Invitation…** → Form + Project on one row with **in** between; Display Text; OK → blue underline chip in Design.
2. Redeploy → runtime shows a **live** `<a>` (blue underline), not plain text. Click navigates to the invited form.
3. Insert → **Hyperlink…** → Url + optional Display text + **Open in new browser window.** → OK → chip → Redeploy opens URL (`target=_blank` when checked).
4. Private invitation: check private, drop a field into the InviteeID box → Redeploy includes `<authenticationTokenValue>`.
5. If step 2/3 shows plain text only: restart API (`:3001`) so export includes `<invitation>` / `<link>`, then Redeploy.
6. If links are blue but **invalid** (JS alert / empty href): chips were double-encoded in Design — fixed Jul 16 (`setAttribute` uses raw JSON; Deploy decodes `&amp;quot;`). Hard-refresh Designer, re-insert chips **or** Redeploy existing content after API restart.

---

## Runtime token syntax

Inserted functions appear as inline tokens in rich text, e.g. `<<FORM RECORD COUNT(Form 1)>>` — see `DESIGNER_UI_REFERENCE.md`.

---

## Browser Designer (`designer-web`) gaps

| Area | Legacy | Browser today |
|------|--------|----------------|
| Context Insert menus | Form / Process / Document | Form / Process / Document context OK (Jul 12); Invitation / Hyperlink live (Jul 16) |
| Invitation / Hyperlink dialogs | Yes | **Yes (Jul 16)** — Insert dialogs + Design tokens + Deploy `<invitation>` / `<link>` |
| Function picker + Configure | Full repository | Picker + Configure for all 17; see status matrix above |
| Image from PC / Web URL | Yes | **From your PC** Approach A (Jul 16); **From the Web…** = DISPLAY IMAGE (Jul 17 rename); File Uploader form item omitted from palette |
| Insert Field (document) | Yes | **Jul 17:** Insert → Field from Fields selection; **Jul 17 fix:** only active MDI Document (stale prior cleared on window activate); status nudge when no caret |
| Document HTML → XML for functions | Full set | **13 emit**; **4 deferred stubs** (categorizer, roster, link, paypal) — Jul 16 |

---

## Source cross-reference

| Topic | C# / XML |
|-------|----------|
| Form Insert enable | `TawalaDesigner/Code/TAWALA/Forms/MDIFormView.cs` |
| Document Insert enable | `TawalaDesigner/Code/TAWALA/Documents/MDIDocumentView.cs` |
| Insert Function UI | `TawalaDesigner/Code/TAWALA/FunctionControls/Editor/InsertFunctionDialog.cs` |
| Function catalog | `TawalaDesigner/Code/TawalaTest/TestSupport/Properties/display-component-repository.xml` |
| Configure Function | `TawalaDesigner/Code/TAWALA/Functions/Controls/ConfigureFunctionDialog*` |

---

*Last updated: July 16, 2026 — function XML emit matrix synced to `documentHtmlToXml.mjs`.*
