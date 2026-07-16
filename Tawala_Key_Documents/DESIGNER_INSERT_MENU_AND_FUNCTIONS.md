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

Top **seven** items match the **Items** palette (except **File Uploader** is on the palette but not in Insert).

### Process (MDI child active)

Insert lists **statement types** only (If, Show, Send, Append, Get, ForEach, Delete, Set, Comment) — see `DESIGNER_MENU_SPEC.md` § Insert B and `DESIGNER_PROCESS_STATEMENTS_*.md`.

No Image, Invitation, Hyperlink, Function, or Field.

### Document (MDI child active)

| Item | Enable rules |
|------|--------------|
| **Field** | When a **field leaf** is selected in the **Fields** palette (Document-only menu item) |
| **Image…** | When document editor active |
| **Invitation…** | Available in document context |
| **Hyperlink…** | Available in document context |
| **Function…** | When project has **≥1 form**; same as **fx** on document format toolbar |

No form item types (Heading, Text, FIB, etc.).

---

## Insert → Image…

Submenu (Form and Document):

| Item | Behavior |
|------|----------|
| **From your PC…** | Insert image file from local disk |
| **From the Web or Tawala Upload…** | Opens **Configure Function** for **DISPLAY IMAGE** (see below). Owner: likely pairs with **File Uploader** form item URL in the image-source field — not yet tested on reference PC |

Screenshot: `assets/Insert_-_Image_-_From_the_Web-*.png`

---

## Insert → Invitation…

**Dialog title:** Insert Invitation

Sends a user a **Start link** to this project or another project.

| Control | Purpose |
|---------|---------|
| **Form:** | Dropdown — target form for the invitation link (e.g. Form 2) |
| **in Project:** | Dropdown — **(Current Project)** or other project |
| **Display Text:** | Link text shown to the invitee |
| **Make this a private invitation** | Checkbox |
| Explanatory text | Describes storing text or a field value in **`_InviteeID`** when the invitee responds via the invitation link |
| Text field below | For private-invitation value; **dimmed/inactive** when checkbox unchecked |

**OK** / **Cancel**

Screenshot: `assets/Insert_-_Invitation-*.png`

---

## Insert → Hyperlink…

**Dialog title:** Hyperlink

Inserts a URL hyperlink into rich text (Form Text item or Document).

| Control | Purpose |
|---------|---------|
| **Url** | Destination URL |
| **Display text** | Optional; italic note: *if you leave this blank the full URL or filename will be shown* |
| **Open in new browser window** | Checkbox |
| **Display link conditionally** | Checkbox — enables condition section |
| **Display link only when** | If-style row: field (green box), operator dropdown, value, **+** / **−** |

**OK** / **Cancel**

Screenshot: `assets/Insert_Hyperlink-*.png`

---

## Insert → Function… / toolbar **fx**

**Dialog title:** Insert Function

Two-step flow: pick function → **Configure Function** dialog for parameters.

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

Owner screenshot July 10, 2026. Owner: **simple**. Also reachable from **Image → From the Web or Tawala Upload…** in legacy.

**Description:** Displays an image that can be accessed via a URL. The image can be a file uploaded via a Tawala "File Uploader" Form Item, or any other valid URL for an image on the Internet.

| Field | Required | Help (focused on Image Source) |
|-------|----------|--------------------------------|
| **Image Source** | **REQUIRED** | Field, variable or literal URL. File Uploader or `http://` / `https://`. Link: *A compound expression*. (Repository XML name: **Image URL** / id `source`.) |
| **Display width** | Optional | **Whole pixels** (not inches). Blank = natural width. Prefer width alone so height scales. |
| **Display height** | Optional (hidden in browser Configure Jul 13) | Legacy still supports it; UI hides it so width-only scaling stays simple. |
| **Alternative name** | Optional | Hover text in browsers |

No column toolbar. **OK** greyed until Image Source filled.

**Owner Jul 13:** Design canvas shows a `<<DISPLAY IMAGE(...)>>` token only. **Form Preview** shows a dashed placeholder box sized to width (and height if set; else banner height 80px) with the **image name** centered (Alternative name, else URL filename / field name — not the full URL). **Deploy** (Java) renders the real `<img>`. File Uploader (form item) is the path for user-uploaded images; greyed in browser Designer and missing on the Jan 2011 reference build. Owner: drag-in images are rare (headers); prefer editing graphics outside Designer, then paste a URL.

Screenshot: [`assets/Function_-_Display_Image.png`](assets/Function_-_Display_Image.png)

### Browser gaps

Configure fields match (Display height hidden — width-only so Deploy keeps aspect ratio). Document **and Form Text** HTML→XML: **emits** `<display-image>` (Jul 13). Empty Document canvas husks are omitted from Deploy XML (Jul 13) so they don’t stack as blank lines. Design mode is token-only. Form Preview: sized name-centered placeholder box (Jul 13; Design/Preview only — not Deploy). Deploy: live image.

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

Catalog + Configure present. Document **and Form Text** HTML→XML: **emits** `<display-mcq-label>` (Jul 13). Runtime shows labels only after that MCQ has answers in the submission (typically a later page or Document after Submit — blank on the unanswered form).

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

Configure matches (Form + conditions). XML export: **yes**. Node Preview (Jul 16): shows filtered session count (`richHtmlPreview`).

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

---

## Configure Function: RESPONSE TOTALS (`response-totals-table`)

Owner screenshot July 10, 2026. Table Layout dropdown open.

**Description:** Displays a table showing the count of each response to a multiple-choice question.

| Field | Required | Notes |
|-------|----------|-------|
| **Table Layout** | Yes | **Tall** (`vertical`) — Choice/Count columns; **Wide** (`horizontal`) — Choice/Count rows. |
| **Question** | Yes | MCQ whose counts to show. |
| **Include only the records where** | Conditions | |

Screenshot: [`assets/Function_-_Response_Totals.png`](assets/Function_-_Response_Totals.png)

### Browser gaps

Catalog matches. Document HTML→XML: **emits** `<response-totals-table>`. **Smoke-needed.**

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
| 2 | DISPLAY IMAGE | `display-image` | **Yes** | **Code ready** — Preview placeholder + Deploy XML; owner Deploy smoke (URL image) |
| 3 | DISPLAY MCQ RESPONSES | `display-mcq-label` | **Yes** | **Code ready** — Preview stub; Deploy live after submit; owner smoke |
| 4 | EXPORT TEAM ROSTER | `export-team-roster` | **Deferred stub** | Empty params |
| 5 | FORM RECORD COUNT | `record-count` | **Yes** | **Preview wired Jul 16** (session record count); Deploy XML yes; owner smoke |
| 6 | LINK TO PROJECT DETAILS | `link-to-project-details` | **Deferred stub** | Hosted My Tawala |
| 7 | MULTIPLE QUESTION LIST | `itemization-table` | **Yes** | **Done** — SignupSheet Jul 16 |
| 8 | PAYPAL BUTTON | `paypal-single-item-button` | **Deferred stub** | Payment integration |
| 9 | PROJECT EMAIL COUNT | `project-email-count` | **Yes** | Smoke-needed (needs Send history) |
| 10 | QUESTION CORRELATION TABLE | `question-correlation-table` | **Yes** | Get Together **Passed w/ caveats** |
| 11 | RANKED MULTIQUESTION LIST | `popular-choice-correlation-table` | **Yes** | Smoke-needed |
| 12 | RANKED RESPONSE COUNTS | `popular-choice-count` | **Yes** | Smoke-needed |
| 13 | RANKED RESPONSE NAME | `popular-choice-display` | **Yes** | Smoke-needed |
| 14 | RESPONSE BAR GRAPH | `choice-tally-table` | **Yes** | Simple Survey **Passed**; Multi Survey **Passed** (template) |
| 15 | RESPONSE TOTALS | `response-totals-table` | **Yes** | Smoke-needed |
| 16 | SINGLE QUESTION LIST | `simple-list` | **Yes** | Smoke-needed |
| 17 | SUM | `sum` | **Yes** | Potluck **Passed w/ caveats** |

**Insert siblings (not in the 17):** Invitation…, Hyperlink…, Image from PC — **not implemented** (stubs). Image → From the Web → DISPLAY IMAGE Configure works.

---

## Runtime token syntax

Inserted functions appear as inline tokens in rich text, e.g. `<<FORM RECORD COUNT(Form 1)>>` — see `DESIGNER_UI_REFERENCE.md`.

---

## Browser Designer (`designer-web`) gaps

| Area | Legacy | Browser today |
|------|--------|----------------|
| Context Insert menus | Form / Process / Document | Form / Process / Document context OK (Jul 12); Invitation / Hyperlink / Image-from-PC stubs |
| Invitation / Hyperlink dialogs | Yes | **No** (stubs) |
| Function picker + Configure | Full repository | Picker + Configure for all 17; see status matrix above |
| Image from PC / upload URL | Yes | **No** (File Uploader deferred) |
| Insert Field (document) | Yes | Fields palette / tokens partial |
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
