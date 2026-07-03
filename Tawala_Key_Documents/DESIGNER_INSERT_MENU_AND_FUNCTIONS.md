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

> **Note:** Per-function **Configure** dialogs and runtime behavior are **not** fully documented here. Owner: document individual functions when implementing/enabling in `designer-web`. Parameter details exist in `TawalaDesigner/.../display-component-repository.xml` and C# function runtime.

---

## Configure Function dialog (example: DISPLAY IMAGE)

Opened after selecting a function (or directly from **Image → From the Web or Tawala Upload…**).

**Title:** Configure Function

| Region | Content |
|--------|---------|
| Left | Parameter fields |
| Right | Yellow help pane — function name, description, field-level help |

**DISPLAY IMAGE** fields (owner screenshot):

| Field | Required | Help summary |
|-------|----------|--------------|
| **Image Source** | **REQUIRED** | Field, variable, or literal URL. File from **File Uploader**, or `http://` / `https://` URL. Link: *A compound expression* |
| **Display width** | Optional | Pixels; blank = original width |
| **Display height** | Optional | Pixels; blank = original height |
| **Alternative name** | Optional | Hover text in browsers |

**OK** greyed until required fields satisfied. **CANCEL** always active.

Help text (right pane):

> Displays an image that can be accessed via a URL. The image can be a file uploaded via a Tawala "File Uploader" Form Item, or any other valid URL for an image on the Internet.

Screenshot: `assets/Insert_-_Image_-_From_the_Web-*.png`

---

## Runtime token syntax

Inserted functions appear as inline tokens in rich text, e.g. `<<FORM RECORD COUNT(Form 1)>>` — see `DESIGNER_UI_REFERENCE.md`.

---

## Browser Designer (`designer-web`) gaps

| Area | Legacy | Browser today |
|------|--------|----------------|
| Context Insert menus | Form / Process / Document | Partial |
| Invitation / Hyperlink dialogs | Yes | No |
| Function picker + Configure | Full repository | No |
| Image from PC / upload URL | Yes | No |
| Insert Field (document) | Yes | No |

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

*Last updated: June 2026.*
