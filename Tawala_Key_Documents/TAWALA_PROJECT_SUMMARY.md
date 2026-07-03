# Tawala — Project Summary
*Compiled from archive documents, June 2026*

---

## 1. Origins and Core Concept

The original product vision (from **TawalaOrigins.doc**) was to let any person — without programming knowledge — construct a questionnaire comprising forms and multiple-choice questions, then follow a user-created *process* to generate printed or emailed letters, memos, or other documents. Input could come from local sources, web forms, databases, or computed processes. Output could go to screen, printer, website, email, or remote web services.

The founding idea was to **democratize web and email application development for non-programmers in small organizations** — particularly those with limited IT resources and the need to communicate with and manage membership beyond simple list management.

Revenue model options considered: standalone retail sale, web-based subscription, advertising, or community/open-source model with premium packages sold by category (e.g., "processes for the law office").

---

## 2. Product Architecture — Key Components

### The Designer (Desktop App, .NET / Windows)
The core authoring tool. A desktop application built in .NET by John Fleischhauer (project manager and front-end) and Sergei Lilichenko (database/back-end). Designers used it to:
- Build **Projects** (web/email apps) composed of **Forms**, **Documents**, and **Functions**
- Define **Forms**: data-collection pages with fill-in fields, multiple-choice questions (with branching/conditional logic), file uploaders, hidden fields, and special items like the **Categorizer** (drag-and-drop record assignment)
- Define **Documents**: output pages with templated text, hyperlinks, styles, and embedded data fields
- Define **Functions**: process logic (GET, FOR EACH, IF, SET VARIABLE, SHOW STORED RECORD, etc.) that automated workflows
- **Deploy** projects to the Tawala server with one click
- Manage **Shared Data Tables (SDTs)**: data sources shared across projects

Key Designer UI elements mentioned in documents:
- **Project Explorer**: tree-view of all components (Forms, Documents, Functions, Hidden Fields)
- **Fields Palette**: available data fields for drag-and-drop into expressions
- **Process/Flow view**: visible sequencing of workflow steps
- **Preview Tab**: live preview of how a Form would render in the browser
- **Styles**: theming system for fonts, colors, applied per item type

Known technical issues during development:
- TX controls (a third-party UI component) were persistently buggy and were being replaced
- IE browser control integration in Designer was complex
- Large projects (e.g., SDT) took 6+ minutes to load/deploy at one point
- Numerous tracked bugs in Designer Stories spreadsheet (bug IDs in 500–932 range)

### The Web Server / Tawala Platform
Hosted at **build.tawala.com** (and **tawala.com**). Provided:
- **MyTawala**: the web-based project management dashboard for Designers — manage, deploy, invite users, view responses, backup/restore/purge data, publish to community library
- **Project Details pages**: per-project management (deploy status, invitation links, embed-in-webpage options, purge/delete)
- **End-user-facing pages**: the deployed Form and Document pages that end-users (clients, members, players) actually saw and filled in
- **Community Library**: repository of shared/published projects for others to browse and use
- **Customizables**: pre-built projects that non-technical users could configure without using Designer (simplified 3–5 step customization UI)

### Database (PostgreSQL)
The back-end database was **PostgreSQL**, managed primarily by Sergei Lilichenko. Key data concepts:
- Each Form submission stored as records in the database
- **Shared Data Tables (SDTs)**: cross-project shared databases — a major architectural feature
- Projects could query SDT records using GET / FOR EACH statements in Functions
- Records could be filtered, sorted, and iterated over in process logic
- Designer could import data from Excel into SDTs

### Themes / Visual Styling
A rich theming system was developed. From **Tawala-format-test/** and **Project Themes Test/** directories, there were named CSS themes including:
- basicblue, basicgreen, basicpink, basicyellow
- blueline, greenline (lined-paper textures)
- dark, default, fabric1, lime, litegreen, money
- clock, coffeecup, lightbulb, orange, pinkrose
- fullmoon, greentea, rangeofchocolate, talkingtomiceelf, tincarbell, whiteflowers, plain, red, yellow, white, style2, setup

Each theme had its own CSS and associated images. The YUI (Yahoo UI Library) was used for JavaScript.

---

## 3. Product Evolution Over Time

### Phase 1 — General-Purpose Process Automation Tool
The earliest concept (TawalaOrigins) was a broadly applicable process automation and document generation tool for non-programmers — something like a no-code workflow engine. The community/library model was central: users would share and build on each other's processes.

### Phase 2 — Designer + Customizables Model (pre-2008)
After showing to outside investors, the team concluded that most people "glazed the moment they saw a Process." Key pivot: build **customizable pre-built projects** that anyone could use without touching Designer. The theory: 5% of users would go deep with Designer; the rest would use customizables. Revenue would come from advertising (volume) or Designer subscriptions.

Lessons learned from this phase (Strategic Recap):
- Customizables were 100× harder to build than templates
- Many users were still put off by the UI complexity
- Designer was still buggy (TX controls)
- Clearest value proposition was: **integration of database functionality with email process control and notification**
- Programmers found Designer genuinely useful; junior programmers found it accessible
- People needed help understanding what to *do* with it — templates and examples helped enormously

### Phase 3 — Sports Dashboards / Dirt Bowl (2007–2008)
The major real-world deployment. A pivot toward **youth sports league management**, branded as **SportsDashboards** (sportsdashboards.com). The first major client project was **Dirt Bowl** — a kids' baseball/softball league managed by Kerry Huffman in Marin County.

Dirt Bowl demonstrated the full Tawala workflow in practice:
- **Recruitment**: administrator sends emails to prior-year players; tracks responses; sends nudge emails to non-responders; manages unsubscribes
- **Registration Form**: web form collecting player data (name, birthdate, parent info, medical info, team preferences, friends requests, coach preferences)
- **Payment tracking**: manual payment receipt marking; PayPal/Google Checkout integration planned
- **Medical waivers**: collected via form; pre-populated PDF option considered
- **Team assignment (Categorizer)**: drag-and-drop UI — administrator drags player records from a roster table into team tables (replacing their prior Excel cut-and-paste workflow)
- **Player Dashboard**: personalized web page for each player/family showing team, schedule, coach info
- **Coach Dashboard**: similar page for coaches with their roster
- **Administrator Dashboard**: central control with links to all management functions
- **Email notifications**: automated emails to players and coaches at various stages
- **Recruitment/waitlist management**: opt-out, unsubscribe, manual removal

The **MVSC** (Marin Volleyball or similar) and **CYO** (Catholic Youth Organization) leagues were also mentioned as customers or prospects.

### Phase 4 — SDT3 / Broader Platform (2009–2012+)
By this stage the product was iterating on a third major version of the SportsDashboards platform (SDT3). Agile-style iteration backlog and icebox stories were being tracked. Key features in development or planned:
- New Forms UI (replacing TX controls with IE browser control)
- Shared Data Tables as a first-class feature
- Conditional display of form items
- Dynamic Multiple Choice Questions (MCQs)
- File upload display in Documents
- Session data preserved in invitation links
- Better error handling in Designer
- Sort capability on GET/FOR EACH function results
- Admin can prepare next season (data rollover)
- Wait lists with payment-gating
- Second administrator email address
- Head coach / assistant coach distinction
- Coach-submitted group registrations
- Jersey/kit size configuration
- Age determination rules

### Phase 5 — GetActive / Broader Market Ambitions (~2010–2012)
Screenshots in **GetActiveAds/** suggest the product was also being marketed under broader categories: "Sports Registration Tools," "Online Sports Reg," "Youth League Mgmt." AdWords campaigns and landing pages were being tested.

The **BoosterBoards** project (OmniOutliner file) suggests an extension for school booster clubs or similar organizations.

---

## 4. Business Model and Customers

- **Primary target**: small organizations (sports leagues, schools, churches, clubs) with limited IT
- **Key customer**: Youth sports leagues — particularly in the San Francisco Bay Area initially
- **Pricing**: Mentioned $5 per charged user fee OR 10% of charge (whichever less) for professional licensing. Free base tier implied for individual/community users.
- **Professional Services**: Up to 4 hours customization per projected $1,000 in anticipated client revenue
- **Revenue model considered**: advertising (needed millions of page views/month) OR subscription OR professional services/licensing
- **Notable business development**: Kerry Huffman as a key early client/evangelist; Gorog (apparel printing for sports teams) as a potential affiliate partner; Bay Area Equity Fund as an investor

---

## 5. Key People Referenced

- **Doug Carlston** — Owner, CEO, product visionary, primary author of design specs and strategy docs
- **John Fleischhauer (JF)** — Overall project manager and front-end developer; led Designer app development and UI; primary bug tracker and release manager
- **Sergei Lilichenko** — Back-end developer; focused on PostgreSQL database design and server-side data layer
- **Kerry Huffman (KH)** — Primary external client; ran Dirt Bowl baseball league; provided detailed requirements feedback
- **Tony** — Developer (mentioned in bug discussions, worked on table/email rendering issues)
- **Steve P** — Mentioned in meetings with Sergei about sort functionality
- **Al** — Mentioned in connection with Help system content
- **Matt Siegel / Donna Bonifield** — Earlier spec work (pre-Fleischhauer era, work described as unsatisfactory)

---

## 6. Technical Stack (from documents)

- **Platform**: .NET / Windows (Designer desktop app)
- **Front-end developer**: John Fleischhauer
- **Database**: PostgreSQL — designed and managed by Sergei Lilichenko
- **Server**: Web-hosted (build.tawala.com); likely ASP.NET backend
- **Frontend**: HTML, CSS (YUI framework), JavaScript; Flash (.swf) for some demos
- **Data layer**: Server-side PostgreSQL; records stored per Form submission; Shared Data Tables (SDTs) as cross-project shared databases
- **Email**: Integral to the platform — invitation emails, notification emails, nudge/reminder campaigns
- **Version control**: SVN (Subversion) — evidence in CSS directory structure (.svn folders)
- **Demos**: Built in Flash (Macromedia/Adobe)

---

## 7. Demo Projects / Customizables (from New Approach doc)

Named apps that were pre-built or planned as customizables:
- **Automated Email List Builder** — build and maintain a group contact list
- **Poll / Survey / Election** — multi-question questionnaire with optional results display
- **Date Finder** — find best date/time for a group event
- **Potluck Organizer** — who's coming and what they're bringing
- **Volunteer Sign-Up** — registration page for activities
- **To-Do / Task Manager** — group task assignment with status tracking
- **Online Exam / Quiz** — test with automatic grading and result recording
- **Sports Dashboard** (most fully developed)
- **CastaCurse** — a social/viral spell-casting novelty app (demonstrated viral mechanics)
- **BoosterBoards** — school/organization booster management
- **Political organizing tools** (Obama.oo3 outline present)

---

## 8. What Is NOT in This Archive

- **Source code** (any .cs, .sln, .csproj, .aspx, .sql files) — to come from Fleischhauer (front-end/.NET) and Lilichenko (PostgreSQL schema and back-end)
- **PostgreSQL database schema** — table definitions, stored procedures, migration scripts
- **Server configuration / deployment scripts**
- **API documentation** — especially Designer ↔ server communication protocol
- **Any git/SVN repository snapshots** (only SVN metadata fragments present in CSS folders)

---

*Summary compiled from: TawalaOrigins.doc, Strategic Recap.doc, Project Details.doc, DirtBall Project.doc, DirtBowlCompletion.doc, Dirtball Stories.doc, Changes to Dirtbowl Project.doc, Development Areas for Overall Strategy.doc, New Approach to Tawala Ho.doc, Categorizer-2.doc, Shared Data.doc, New features for Das#59B479.doc, WebPageReview.doc, What's New at Tawala.doc, John Strategy.doc, Gorog Notes.rtf, SportsServicesIssues7-25-08.doc, Designer Stories and#9B2FC9.xls, Dirt Bowl Tasks-1.xls, Youth Sports Needs Features Problems.xls, SDT3CurrentIterationBacklog.xlsx, SDT3 Icebox stories.xlsx, Tawala Services.doc, Youth sports management.doc, CastaCurse.doc, and CSS/HTML theme directories.*
