# Tawala Platform — Process Execution Engine & Runtime Architecture
*Definitive documentation from Java source (build-1700), SVN r1–r9616*
*Focus: Designer tool, Web runtime engine, and Library/publishing system*

---

## Scope

The primary Tawala platform assets for reimplementation:

1. **The Designer** — C# Windows Forms authoring tool producing `.tawala` XML project files
2. **The Web Runtime** — Java webapp that loads, renders, and executes those projects
3. **The Library / Publishing System** — project library, versioning, ratings, categories, distribution

SportsDashboards was a *customer* of the platform — an application built with Designer and hosted by the runtime. It is referenced here only as a usage example, not as a platform component.

---

## Runtime Package Map

| Package | Files | Role |
|---|---|---|
| `com.tawala.project` | 202 | Top-level model: Project, Form, FormItem, Process |
| `com.tawala.project.commands` | 59 | **Process execution engine** (see below) |
| `com.tawala.project.library` | 37 | Publishing: LibraryProject, Categories, Versions |
| `com.tawala.project.data` | 21 | DataSource, import/export, Excel |
| `com.tawala.project.fib` | 5 | FillInBlank layout strategies |
| `com.tawala.project.formatting` | 17 | Rich text, links, images |
| `com.tawala.project.theme` | 6 | Theme / CSS resolution |
| `com.tawala.web` | 276 | HTTP controllers, form rendering, sessions |
| `com.tawala.email` | 10 | Email queuing, sending, state |
| `com.tawala.component` | 60 | Reusable display components |

---

## Core Data Model

### `Project`
The top-level runtime entity (`@Table("project")`). The entire project definition is stored as a `@Lob` XML string (`project_definition`), parsed lazily into memory. Contains:
- `List<Form>` — the screens/pages
- `List<Document>` — printable document templates
- `List<Process>` — named command sequences (pre- and post-process)
- `Map<String, Image>` — image assets
- `List<DataSource>` — shared data source definitions

**Format versioning:** `format` attribute on root XML element. Current = `1.4`, minimum safe = `1.3`. The runtime enforces this on load.

### `Form`
One screen. XML attributes:
- `name` — unique identifier within the project
- `startPoint` (boolean) — whether it appears in the "start here" list
- `dataEntryOnly` (boolean) — admin-entered data, not shown to public respondents
- `themePath` — CSS theme override for this form
- `process` — name of the post-process to run after submission
- `preProcess` — name of the process to run before displaying
- `dataSourceName` — links form submissions to a shared DataSource

Contains `FormSegments` → `FormItem` objects (questions, text blocks, headings).

### `FormItem` (abstract base)
All renderable form elements inherit from this. Key behavior:
- Reads `displayConditions` child element in constructor
- `toHtml(context)` calls `shouldBeDisplayed(context)` first — if false, returns `NoHtml.INSTANCE`
- `shouldBeDisplayed()` evaluates the `BooleanExpression` (returns `true` if none set)

**Concrete types registered in `Form.FACTORY`:**
```
"text"            → TextBlock
"heading" Main    → HeadingItem
"heading" Sub     → SubheadingItem
"fib"             → FillInBlank
"file"            → FillInBlank (file upload variant)
"mc"              → MultipleChoice
```

---

## The Process Execution Engine

### What a Process Is
A named list of commands defined in the `.tawala` XML:
```xml
<process name="Post-Registration">
  <set field="Status" .../>
  <send>...</send>
  <show form="Confirmation"/>
</process>
```
Processes are invoked in two ways:
- **preProcess** — runs before a form is displayed (populates variables, checks access)
- **process** (post-process) — runs after a form is submitted (sets data, sends email, navigates)

### Command Registry (`ProcessCommandList.FACTORY`)
All 16 command types, exactly as registered:

| XML Tag | Class | Purpose |
|---|---|---|
| `show document="..."` | `ShowDocument` | Display a document template |
| `show form="..."` | `ShowForm` | Navigate to a named form |
| `show` | `Show` | Redirect to a computed URL |
| `edit` | `EditRecord` | Modify a record in-place |
| `if` | `If` | Conditional branch (trueSet / falseSet) |
| `send` | `Send` | Compose and queue an email |
| `set` | `SetCommand` | Assign a value to a field or variable |
| `append` | `Append` | Append a string to a field |
| `addTo` | `MathCommand.AddTo` | Numeric addition |
| `subtractFrom` | `MathCommand.SubtractFrom` | Numeric subtraction |
| `multiplyBy` | `MathCommand.MultiplyBy` | Numeric multiplication |
| `divideBy` | `MathCommand.DivideBy` | Numeric division |
| `get` | `Get` | Query records from form submissions |
| `delete` | `Delete` | Delete a record |
| `foreach` | `ForEach` | Iterate over a record list |
| `forEachMc` | `ForEachMc` | Iterate over MC selections |
| `comment` | *(ignored)* | Designer comments, not executed |

The skip mechanism is separate, registered in `FormSegments`, not `ProcessCommandList`:
- `skip to="..."` — unconditional skip to a form item by label/alternateLabel
- `skipif` → `SkipIf` — conditional skip; wraps `If` with `SkipBlock` branches

### `ExecutionContext` — The Runtime State Object
Constructed fresh for each form request. Carries:
- `UserProject` — the running project instance
- `FormSubmission submission` — the current form's posted data
- `FormSubmission variables` — named variables that persist across forms in the session
- `Map<String, CompositeFormSubmission> recordMap` — named records (from `get` / `foreach`)
- `Map<String, List<CompositeFormSubmission>> recordListMap` — named record lists
- `Map<String, VirtualDocument> documents` — built documents
- `Domain domain` — the hosting domain
- `HttpSession` (via `Request`) — full session access
- Flags: `previewMode`, `generatingEmailContent`, `evaluatingWhereClause`

**Key `ExecutionContext` methods:**
- `getValue(name)` → resolves a `Reference` and returns a `Value`
- `setValue(fieldId, value)` → writes to submission or variables
- `getRecordList(listId)` / `setRecordList(...)` — record list management
- `mapRecord(recordId, record)` / `removeRecordMapping(...)` — record cursor management

### `Reference` — The Field Addressing System
Field references are colon-delimited strings, parsed with up to 3 tokens:

| Format | Example | Meaning |
|---|---|---|
| `fieldName` | `Status` | Variable (no form qualifier) |
| `FormName:fieldName` | `Registration:Email` | Field on a specific form's submission |
| `RecordName:FormName:fieldName` | `Player:Registration:Email` | Field on a named record |

Resolution logic (in order):
1. If 1 token → treated as a variable (stored in `variables` FormSubmission)
2. If 3 tokens and first matches a record name → record reference
3. If 2-3 tokens and first matches a form name → form-qualified field
4. Otherwise → treated as a variable

This addressing scheme is used uniformly in `set`, `get` conditions, `displayConditions`, `send`, and all other command operands.

### `Value` — The Universal Value Type
All field data is ultimately a `String`. `Value` wraps it with:
- `toString()` — the raw string
- `asNumber()` → `SmartNumber` for math operations
- `isEmpty()` — true if null or blank
- `equals()` — case-insensitive trim comparison; numeric comparison when both are numeric; NULL never equals anything (SQL semantics)

### `BooleanExpression` — The Condition System
Used in `displayConditions`, `get` where-clauses, `if` conditions, `skipif` conditions.

**All registered operators:**

| XML Tag | Semantics |
|---|---|
| `and` | Logical AND of child expressions |
| `or` | Logical OR of child expressions |
| `stringEquals` / `equals` | String/numeric equality |
| `doesNotEqual` | Inequality |
| `beginsWith` | String prefix |
| `endsWith` | String suffix |
| `contains` | Substring match |
| `doesNotContain` | Substring absence |
| `isBlank` | Field is empty |
| `isNotBlank` | Field is non-empty |
| `mcIsBlank` | MC has no selection |
| `mcIsNotBlank` | MC has a selection |
| `isLessThan` | Numeric < |
| `isGreaterThan` | Numeric > |
| `isLessThanOrEqualTo` | Numeric ≤ |
| `isGreaterThanOrEqualTo` | Numeric ≥ |
| `mcContains` | MC selection list contains value |
| `mcDoesNotContain` | MC selection list lacks value |
| `mcEquals` | MC selection list exactly equals |
| `mcDoesNotEqual` | MC selection list doesn't equal |

**No role-based conditions exist.** Admin visibility is handled at the form level (the `Administration` form set is protected by `OnlyAdministratorAccessInterceptor`), not via `displayConditions`.

### `Operator` — The Value Expression System
Used in `set`, `send` subject/body, and math commands to compute values:

| XML form | Class | Meaning |
|---|---|---|
| `<string value="literal"/>` | `LiteralOperator` | Hardcoded string |
| `<string field="FieldRef"/>` | `ReferenceOperator` | Read a field value |
| `<operand value="..."/>` | `LiteralOperator` | Same (alternate tag) |
| `<operand field="..."/>` | `ReferenceOperator` | Same (alternate tag) |
| `<field .../>` | `ReferenceOperator` | Same (third alternate) |
| `<add>` | `AddOperator` | Numeric addition |
| `<sub>` | `SubtractOperator` | Numeric subtraction |
| `<mul>` | `MultiplyOperator` | Numeric multiplication |
| `<div>` | `DivideOperator` | Numeric division |

A `StringConcatenationExpression` is a list of `Operator`s whose values are concatenated. This is used for the `set` expression and the `send` subject line.

### `Get` Command — The Query Layer
Loads records from past form submissions into the `ExecutionContext`:
```xml
<get recordList="Players">
  <forms>
    <form name="Registration"/>
  </forms>
  <conditions>
    <equals field="Registration:TeamId" value="..."/>
  </conditions>
</get>
```
- Queries across one or more forms (cross-form joins via cartesian product)
- Conditions are a `BooleanExpression` evaluated per-record
- Supports both current-project data and shared (cross-project) `DataSource` references
- No indexing — full scan in memory. Works well for hundreds of records per project.

### `ForEach` Command — The Iteration Layer
```xml
<foreach recordList="Players" record="Player">
  <send>...</send>
</foreach>
```
- Iterates over a record list loaded by `get`
- Maps each record to the name given by `record` attribute
- Inner commands can reference fields as `Player:Registration:Email`
- Supports `stopImmediately()` for early exit

### `Send` Command — Email Composition
Builds and queues an email from process context:
```xml
<send>
  <from addressField="Registration:ParentEmail"/>
  <to addressLiteral="admin@example.com"/>
  <cc addressField="Registration:Email2"/>
  <subject>Registration confirmed for <field name="Registration:FirstName"/></subject>
  <body document="ConfirmationLetter"/>
</send>
```
- `from` / `to` / `cc` accept both `addressLiteral` (hardcoded) and `addressField` (dynamic from field reference)
- Subject is a `StringConcatenationExpression` (can include field values)
- Body is either a named `Document` template or inline static text
- Queues to the `email` table with `State.READY`; the batch processor sends it

---

## The Data Source System

`DataSource` defines a named schema for shared cross-project data storage. Fields are either `StringField` or `MultiChoiceField`. When a form's `dataSourceName` matches a DataSource, its submissions are stored in shared storage rather than project-private storage. This enables:
- One project writing registrations that another project can read
- A "master list" pattern where one admin project populates data consumed by many end-user projects

`RecordSelector` handles the `externalSharedData` flag to transparently route `get` queries to the shared store.

---

## The Library / Publishing System

### Data Model (`LibraryProject`)
A published project entry (`@Table("lib_project")`):

| Field | Type | Notes |
|---|---|---|
| `authorId` | String | The publishing user |
| `name` | String (unique) | Public name in the library |
| `shortDescription` | String (60 chars) | One-line summary |
| `longDescription` | text | Full description |
| `category` | `Category` | Hierarchical category |
| `iconURL` | String | Thumbnail image |
| `videoURL` | String | Demo video link |
| `snapshotTile` | String | Screenshot tile |
| `featured` / `featuredOrder` | boolean / int | Landing page placement |
| `downloadCount` | int | Times downloaded |
| `testDriveCount` | int | Times test-driven |
| `cloneCount` | int | Times cloned |
| `commentCount` | int | Cached comment count |
| `rating` | int | Aggregated rating (1–5) |
| `versions` | List | Versioned project definitions |
| `deleted` | boolean | Soft delete |

### Publishing Flow
```
Designer produces .tawala XML
   ↓
User uploads to UserProject (their live instance)
   ↓
User submits to library → ProjectLibraryService.onProjectSubmission()
   ↓
LibraryProject created, LibraryProjectVersion 1 assigned
   ↓
Events fired → LibraryChangeEvent, ProjectVersionAdded
```

### `ProjectLibraryService` — Service Operations
| Method | Action |
|---|---|
| `onProjectSubmission` | First publish to library |
| `onProjectUpdate` | Metadata update (name, desc, category) |
| `onAddingProjectVersion` | Upload new `.tawala` version |
| `onProjectDownloadByUser` | Record download, track user+version |
| `onRatingProject` | Submit or update rating |
| `onProjectTestDrive` | Increment test-drive counter |
| `onProjectDelete` | Soft delete |
| `getProjectHistory` | Full event timeline for one project |
| `getChangesSince(date)` | All library changes since a date |
| `getChangesByUserSince` | User-scoped change history |
| `getFeaturedProjects` | Landing page list |
| `getProjectEligibleForInclusionOnLandingPages` | Vetted, non-deleted projects |

### Categories
Hierarchical. `Category` has a parent/child tree. `CategoryEditor` handles create/rename/move/delete. All changes fire `CategoryChangeEvent`s.

---

## Confirmed Status of Open Items

### Item 1: Admin-Only Fields — COMPLETE ✓
`displayConditions` is fully implemented in `FormItem` (the base class). The inheritance chain `FillInBlank → Question → FormItem` means all form items support it. The `FACTORY.ignore("displayConditions")` in leaf classes prevents double-parsing, not suppression. Admin visibility is enforced at the form level via access interceptors — not per-field conditions.

### Item 2: Email / DMARC — CONFIRMED BUG, NEVER FIXED
`Email.java` in build-1700 sets `From:` to `sportsdashboards@tawala.com` while putting the sender's actual address only in `Reply-To:`. Yahoo's 2014 `p=reject` DMARC policy killed all such emails. Sergei's fix attempt is still present as a commented-out line. **Reimplementation fix:** verified sending domain + transactional provider (SendGrid, Postmark, SES); sender's address in `Reply-To:` only.

### Item 3: Variable Phone Labels — INCOMPLETE
No `PhoneLabel` / `Phone1Label` style field exists anywhere in the runtime. Labels are hardcoded strings in the `.tawala` XML. The 2008 commit noted "not yet displayed elsewhere" and it never was. **Reimplementation fix:** define phone label fields as Admin Setup variables, reference them via the standard `<field name="..."/>` mechanism in question XML and report templates.

### Item 4: Hardcoded Little League Date — CONFIRMED (SportsDashboards-specific)
Not relevant to platform reimplementation. Illustrates why date-driven rules should use setup variables, not code constants.

---

## Reimplementation Readiness Summary

| Component | Status | Priority |
|---|---|---|
| Form item types (fib, mc, text, heading, image) | Fully documented | High |
| displayConditions / BooleanExpression | Fully documented | High |
| Process command set (all 16 types) | Fully documented | High |
| Reference addressing system | Fully documented | High |
| Value / SmartNumber semantics | Fully documented | High |
| Operator / StringConcatenation expressions | Fully documented | High |
| Get / ForEach / RecordSelector (query layer) | Fully documented | High |
| Skip / SkipIf mechanism | Fully documented | High |
| Send (email composition) | Documented, bug noted | High |
| DataSource / shared cross-project data | Fully documented | Medium |
| Project format versioning (1.3 → 1.4) | Confirmed | Medium |
| Library / publishing system | Fully documented | Medium |
| Pre/post process invocation model | Fully documented | High |
| DMARC-safe email sending | Fix specified | High |
| Variable phone labels | Incomplete in original | Low |
| Theme / CSS system | Partially documented | Medium |
| Excel import/export | Present, not detailed | Low |
| Project backup system | Present, not detailed | Low |
