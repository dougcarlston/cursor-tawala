/**
 * Library vs My Tawala catalogs + local :8080 start URLs.
 *
 * Source piles (JSON backups):
 *   website-mock/projects/library/  ← ~/Projects/Tawala Projects/WebLibrary
 *   website-mock/projects/mytawala/ ← ~/Projects/Tawala Projects/MyTawala
 *
 * Main Menu public templates (Simple Survey, Sign-up, Potluck, Get Together, …)
 * stay in the Library catalog with live :8080 URLs; JSON lives under
 * designer-web/public/samples/templates/. Designer-only New Project items
 * (Empty/Blank, Form with Process, Form with Process & Document, and
 * Sign-up Sheet w Email — retired from Library Aug 1, 2026) are NOT listed.
 *
 * Library listing groups mirror Designer File → New Project categories
 * (Activities / Meetings and Gatherings / Polls and Surveys), plus WebLibrary
 * extras (Sports / Business / Entertainment / Advanced). Basic is Designer-only — never listed.
 *
 * liveReady: true — owner-vetted product with a working :8080 test-drive (quiet “Live” cue in Library list).
 * Update live URLs after Deploy (Designer File→Deploy or POST /api/deploy) / deploy-tawala-template.mjs.
 *
 * stub: true (Library only) — placeholder / converted-but-unverified entry kept as a visible
 * reminder, NOT a real working demo. Owner Aug 1, 2026: the correct route to clean these up is
 * Designer → Deploy → Publish (checking each equivalent working copy first) — until that route
 * is used, stubs stay in the catalog with a " (stub)" suffix on the display name so the Library
 * listing makes obvious what still needs replacing. Never set on liveReady or main-menu (deployed)
 * entries. There is intentionally NO public Library Delete control — see README.md
 * "Retiring Library stubs" for the agent-run retirement path (list-library-stubs.mjs).
 */

/**
 * Collapsible Library groups — New Project order first, then WebLibrary extras.
 * slug → ?cat= filter; label → project.category display string.
 */
window.TAWALA_LIBRARY_CATEGORIES = [
  { slug: "activities", label: "Activities", source: "new-project" },
  { slug: "meetings", label: "Meetings and Gatherings", source: "new-project" },
  { slug: "polls", label: "Polls and Surveys", source: "new-project" },
  { slug: "sports", label: "Sports", source: "weblibrary" },
  { slug: "business", label: "Business", source: "weblibrary" },
  { slug: "entertainment", label: "Entertainment", source: "weblibrary" },
  { slug: "advanced", label: "Advanced", source: "weblibrary" },
];
window.TAWALA_LIBRARY = {
  "simple-survey": {
    "name": "Simple Survey Template",
    "category": "Polls and Surveys",
    "featured": true,
    "iconLabel": "SS",
    "rating": 4,
    "comments": 12,
    "updated": "6/27/26",
    "shortDescription": "A one-question survey with an instant results report.",
    "longDescription": "Replace the sample question with your own multiple-choice question. Respondents pick an answer; the Report form shows live tallies.",
    "jsonFile": "designer-web/public/samples/templates/simple-survey.json",
    "sourcePile": "main-menu",
    "liveReady": true,
    "deployed": true,
    "startPoints": [
      {
        "label": "Survey",
        "url": "http://localhost:8080/p/gy1zssbrwm4fgfm/npwtqlg.Survey"
      },
      {
        "label": "Report",
        "url": "http://localhost:8080/p/gy1zssbrwm4fgfm/d6ceolx.Report"
      }
    ],
    "testDriveUrl": "http://localhost:8080/p/gy1zssbrwm4fgfm/npwtqlg.Survey"
  },
  "signup-sheet": {
    "name": "Sign-up Sheet Template",
    "category": "Activities",
    "featured": true,
    "iconLabel": "SU",
    "rating": 5,
    "comments": 8,
    "updated": "6/27/26",
    "shortDescription": "Collect names and contact info; see signups in a table on the same page.",
    "longDescription": "First name, last name, email, phone, and address fields feed an itemization table so your group can see who has signed up without a separate report form.",
    "jsonFile": "designer-web/public/samples/templates/signup-sheet.json",
    "sourcePile": "main-menu",
    "liveReady": true,
    "deployed": true,
    "startPoints": [
      {
        "label": "Form 1",
        "url": "http://localhost:8080/p/cicw55xxhvwrrh7/l2u4sdg.Form+1"
      }
    ],
    "testDriveUrl": "http://localhost:8080/p/cicw55xxhvwrrh7/l2u4sdg.Form+1"
  },
  "potluck": {
    "name": "Potluck Template",
    "category": "Meetings and Gatherings",
    "featured": true,
    "iconLabel": "PL",
    "rating": 4,
    "comments": 15,
    "updated": "7/29/26",
    "shortDescription": "Potluck invitation \u2014 headcount, dish contributions, and a shared report.",
    "longDescription": "Invite guests to a potluck, collect RSVPs and what each person will bring. Uses Potluck Organizer (start), Report, documents, and processes for thanks and delete.",
    "jsonFile": "designer-web/public/samples/templates/potluck.json",
    "sourcePile": "main-menu",
    "liveReady": true,
    "deployed": true,
    "startPoints": [
      {
        "label": "Potluck Organizer",
        "url": "http://localhost:8080/p/52ozm3kqd58zlss/uhqc1kc.Potluck+Organizer"
      },
      {
        "label": "Report",
        "url": "http://localhost:8080/p/52ozm3kqd58zlss/cni7mae.Report"
      }
    ],
    "testDriveUrl": "http://localhost:8080/p/52ozm3kqd58zlss/uhqc1kc.Potluck+Organizer"
  },
  "get-together": {
    "name": "Get Together Template",
    "category": "Meetings and Gatherings",
    "featured": true,
    "iconLabel": "GT",
    "rating": 4,
    "comments": 11,
    "updated": "7/2/26",
    "shortDescription": "Find the best date for an event \u2014 availability plus top preference.",
    "longDescription": "Two MCQs: which dates work (multi-select) and top preference (single). Report includes a question-correlation table to see the best overlap.",
    "jsonFile": "designer-web/public/samples/templates/get-together.json",
    "sourcePile": "main-menu",
    "liveReady": true,
    "deployed": true,
    "startPoints": [
      {
        "label": "Survey",
        "url": "http://localhost:8080/p/b6do4s50iq64vl8/g6zi1ar.Survey"
      },
      {
        "label": "Report",
        "url": "http://localhost:8080/p/b6do4s50iq64vl8/ejeypox.Report"
      }
    ],
    "testDriveUrl": "http://localhost:8080/p/b6do4s50iq64vl8/g6zi1ar.Survey"
  },
  "alextimon": {
    "name": "AlexTimon (stub)",
    "category": "Business",
    "featured": false,
    "iconLabel": "AL",
    "rating": 0,
    "comments": 0,
    "updated": "7/28/26",
    "shortDescription": "Converted project (26 forms). Start points: Password.",
    "longDescription": "Backup copy from ~/Projects/Tawala Projects/WebLibrary. 26 forms, 29 processes, 16 documents. Start points: Password.",
    "jsonFile": "projects/library/AlexTimon.json",
    "sourcePile": "library",
    "stub": true,
    "deployed": false,
    "startPoints": [
      {
        "label": "Password"
      }
    ],
    "testDriveUrl": null
  },
  "automated-list-builder": {
    "name": "Automated List Builder (stub)",
    "category": "Activities",
    "featured": false,
    "iconLabel": "AL",
    "rating": 0,
    "comments": 0,
    "updated": "7/28/26",
    "shortDescription": "Converted project (15 forms). Start points: Questionnaire, Administration, Setup.",
    "longDescription": "Backup copy from ~/Projects/Tawala Projects/WebLibrary. 15 forms, 19 processes, 21 documents. Start points: Questionnaire, Administration, Setup.",
    "jsonFile": "projects/library/Automated List Builder.json",
    "sourcePile": "library",
    "stub": true,
    "deployed": false,
    "startPoints": [
      {
        "label": "Questionnaire"
      },
      {
        "label": "Administration"
      },
      {
        "label": "Setup"
      }
    ],
    "testDriveUrl": null
  },
  "clientprofiler": {
    "name": "ClientProfiler (stub)",
    "category": "Business",
    "featured": false,
    "iconLabel": "CL",
    "rating": 0,
    "comments": 0,
    "updated": "7/28/26",
    "shortDescription": "Converted project (26 forms). Start points: Password.",
    "longDescription": "Backup copy from ~/Projects/Tawala Projects/WebLibrary. 26 forms, 29 processes, 16 documents. Start points: Password.",
    "jsonFile": "projects/library/ClientProfiler.json",
    "sourcePile": "library",
    "stub": true,
    "deployed": false,
    "startPoints": [
      {
        "label": "Password"
      }
    ],
    "testDriveUrl": null
  },
  "cyo-checkdeposit-request1": {
    "name": "CYO CheckDeposit Request1 (stub)",
    "category": "Sports",
    "featured": false,
    "iconLabel": "CC",
    "rating": 0,
    "comments": 0,
    "updated": "7/28/26",
    "shortDescription": "Converted project (11 forms). Start points: Admin, CheckReqForm, DepositForm.",
    "longDescription": "Backup copy from ~/Projects/Tawala Projects/WebLibrary. 11 forms, 13 processes, 6 documents. Start points: Admin, CheckReqForm, DepositForm.",
    "jsonFile": "projects/library/CYO CheckDeposit Request1.json",
    "sourcePile": "library",
    "stub": true,
    "deployed": false,
    "startPoints": [
      {
        "label": "Admin"
      },
      {
        "label": "CheckReqForm"
      },
      {
        "label": "DepositForm"
      }
    ],
    "testDriveUrl": null
  },
  "cyo-exceptions-app": {
    "name": "CYO Exceptions App (stub)",
    "category": "Sports",
    "featured": false,
    "iconLabel": "CE",
    "rating": 0,
    "comments": 0,
    "updated": "7/28/26",
    "shortDescription": "Converted project (15 forms). Start points: ExceptionRequest, Setup, ClubData.",
    "longDescription": "Backup copy from ~/Projects/Tawala Projects/WebLibrary. 15 forms, 19 processes, 23 documents. Start points: ExceptionRequest, Setup, ClubData, FullReport, SummaryReport.",
    "jsonFile": "projects/library/CYO Exceptions App.json",
    "sourcePile": "library",
    "stub": true,
    "deployed": false,
    "startPoints": [
      {
        "label": "ExceptionRequest"
      },
      {
        "label": "Setup"
      },
      {
        "label": "ClubData"
      },
      {
        "label": "FullReport"
      },
      {
        "label": "SummaryReport"
      }
    ],
    "testDriveUrl": null
  },
  "dirtbowl": {
    "name": "DirtBowl (stub)",
    "category": "Sports",
    "featured": false,
    "iconLabel": "DI",
    "rating": 5,
    "comments": 6,
    "updated": "7/28/26",
    "shortDescription": "Converted project (77 forms). Start points: Registration, AdminDash.",
    "longDescription": "Backup copy from ~/Projects/Tawala Projects/WebLibrary. 77 forms, 91 processes, 46 documents. Start points: Registration, AdminDash.",
    "jsonFile": "projects/library/DirtBowl.json",
    "sourcePile": "library",
    "stub": true,
    "deployed": false,
    "startPoints": [
      {
        "label": "Registration"
      },
      {
        "label": "AdminDash"
      }
    ],
    "testDriveUrl": null
  },
  "genericlistmanager": {
    "name": "GenericListManager (stub)",
    "category": "Advanced",
    "featured": false,
    "iconLabel": "GE",
    "rating": 0,
    "comments": 0,
    "updated": "7/28/26",
    "shortDescription": "Converted project (34 forms). Start points: Administration, Utility.",
    "longDescription": "Backup copy from ~/Projects/Tawala Projects/WebLibrary. 34 forms, 44 processes, 20 documents. Start points: Administration, Utility.",
    "jsonFile": "projects/library/GenericListManager.json",
    "sourcePile": "library",
    "stub": true,
    "deployed": false,
    "startPoints": [
      {
        "label": "Administration"
      },
      {
        "label": "Utility"
      }
    ],
    "testDriveUrl": null
  },
  "horses-and-penguins-test": {
    "name": "Horses and Penguins Test",
    "category": "Entertainment",
    "featured": false,
    "iconLabel": "HA",
    "rating": 0,
    "comments": 0,
    "updated": "7/30/26",
    "shortDescription": "Fun quiz — horses vs penguins. Score tracking with Process math.",
    "longDescription": "Owner-vetted Entertainment try-out. One form, one process (score/wrong math), six answer documents. Theme style2. Start point: Form 1.",
    "jsonFile": "projects/library/Horses and Penguins Test.json",
    "sourcePile": "library",
    "liveReady": true,
    "deployed": true,
    "startPoints": [
      {
        "label": "Form 1",
        "url": "http://localhost:8080/p/wg77ytn0bgq1x70/wdq78g1.Form+1"
      }
    ],
    "testDriveUrl": "http://localhost:8080/p/wg77ytn0bgq1x70/wdq78g1.Form+1"
  },
  "league-age-calculator": {
    "name": "League Age calculator (stub)",
    "category": "Sports",
    "featured": false,
    "iconLabel": "LA",
    "rating": 0,
    "comments": 0,
    "updated": "7/28/26",
    "shortDescription": "Converted project (3 forms). Start points: Setup, Widget.",
    "longDescription": "Backup copy from ~/Projects/Tawala Projects/WebLibrary. 3 forms, 4 processes, 4 documents. Start points: Setup, Widget.",
    "jsonFile": "projects/library/League Age calculator.json",
    "sourcePile": "library",
    "stub": true,
    "deployed": false,
    "startPoints": [
      {
        "label": "Setup"
      },
      {
        "label": "Widget"
      }
    ],
    "testDriveUrl": null
  },
  "lunch-order-menu": {
    "name": "Lunch Order Menu (stub)",
    "category": "Activities",
    "featured": false,
    "iconLabel": "LO",
    "rating": 0,
    "comments": 0,
    "updated": "7/28/26",
    "shortDescription": "Converted project (5 forms). Start points: Main Menu.",
    "longDescription": "Backup copy from ~/Projects/Tawala Projects/WebLibrary. 5 forms, 5 processes, 6 documents. Start points: Main Menu.",
    "jsonFile": "projects/library/Lunch Order Menu.json",
    "sourcePile": "library",
    "stub": true,
    "deployed": false,
    "startPoints": [
      {
        "label": "Main Menu"
      }
    ],
    "testDriveUrl": null
  },
  "multiple-question-survey": {
    "name": "Multiple Question Survey Template",
    "category": "Polls and Surveys",
    "featured": false,
    "iconLabel": "MQ",
    "rating": 4,
    "comments": 9,
    "updated": "7/30/26",
    "shortDescription": "Multi-question poll with bar-graph tallies and a response table on Report.",
    "longDescription": "Owner-vetted Polls and Surveys try-out (corrected port). Survey collects name, several multiple-choice questions, and optional results link. Report shows choice-tally tables per MCQ plus an itemization table of all responses.",
    "jsonFile": "projects/library/Multiple Question Survey Template.json",
    "sourcePile": "library",
    "liveReady": true,
    "deployed": true,
    "startPoints": [
      {
        "label": "Survey",
        "url": "http://localhost:8080/p/grniytf6dvmobqe/y7ucha7.Survey"
      },
      {
        "label": "Report",
        "url": "http://localhost:8080/p/grniytf6dvmobqe/w2licdd.Report"
      }
    ],
    "testDriveUrl": "http://localhost:8080/p/grniytf6dvmobqe/y7ucha7.Survey"
  },
  "mvsc-communicator": {
    "name": "MVSC Communicator (stub)",
    "category": "Sports",
    "featured": false,
    "iconLabel": "MC",
    "rating": 0,
    "comments": 0,
    "updated": "7/28/26",
    "shortDescription": "Converted project (18 forms). Start points: Start.",
    "longDescription": "Backup copy from ~/Projects/Tawala Projects/WebLibrary. 18 forms, 23 processes, 4 documents. Start points: Start.",
    "jsonFile": "projects/library/MVSC Communicator.json",
    "sourcePile": "library",
    "stub": true,
    "deployed": false,
    "startPoints": [
      {
        "label": "Start"
      }
    ],
    "testDriveUrl": null
  },
  "mvsc-registration": {
    "name": "MVSC Registration (stub)",
    "category": "Sports",
    "featured": false,
    "iconLabel": "MR",
    "rating": 0,
    "comments": 0,
    "updated": "7/28/26",
    "shortDescription": "Converted project (1 forms). Start points: Form 1.",
    "longDescription": "Backup copy from ~/Projects/Tawala Projects/WebLibrary. 1 forms, 0 processes, 0 documents. Start points: Form 1.",
    "jsonFile": "projects/library/MVSC Registration.json",
    "sourcePile": "library",
    "stub": true,
    "deployed": false,
    "startPoints": [
      {
        "label": "Form 1"
      }
    ],
    "testDriveUrl": null
  },
  "online-exam-builder": {
    "name": "Online Exam Builder",
    "category": "Polls and Surveys",
    "featured": false,
    "iconLabel": "OE",
    "rating": 0,
    "comments": 0,
    "updated": "8/4/26",
    "shortDescription": "Build and administer an online exam — questions, scoring, and examinee results.",
    "longDescription": "Owner-vetted Polls and Surveys Live app (8-3-26 build). Administration/Setup to configure the exam and questions; Exam for examinees; CustomizationPreview for branding. Library Test Drive opens Administration first.",
    "jsonFile": "projects/library/Online Exam Builder.json",
    "sourcePile": "library",
    "liveReady": true,
    "deployed": true,
    "uniqueId": "u3hkqgwtrepjlur",
    "startPoints": [
      {
        "label": "Exam",
        "url": "http://localhost:8080/p/u3hkqgwtrepjlur/sto3lpi.Exam"
      },
      {
        "label": "Administration",
        "url": "http://localhost:8080/p/u3hkqgwtrepjlur/ef6sx16.Administration"
      },
      {
        "label": "CustomizationPreview",
        "url": "http://localhost:8080/p/u3hkqgwtrepjlur/oio6z9y.CustomizationPreview"
      },
      {
        "label": "Setup",
        "url": "http://localhost:8080/p/u3hkqgwtrepjlur/dc2nyex.Setup"
      }
    ],
    "testDriveUrl": "http://localhost:8080/p/u3hkqgwtrepjlur/ef6sx16.Administration"
  },
  // signup-sheet-email — retired from public Library (owner Aug 1, 2026). Still on
  // Designer → File → New Project (`designer-web/public/samples/templates/signup-sheet-w-email.json`).
  // Not seeded into TAWALA_MYTAWALA. Re-Publish when a finished runtime-customizable version exists.
  "sportsdashboards-template": {
    "name": "SportsDashboards Template (stub)",
    "category": "Sports",
    "featured": false,
    "iconLabel": "ST",
    "rating": 4,
    "comments": 6,
    "updated": "7/28/26",
    "shortDescription": "Converted project (79 forms). Start points: Registration, AdminDash.",
    "longDescription": "Backup copy from ~/Projects/Tawala Projects/WebLibrary. 79 forms, 94 processes, 47 documents. Start points: Registration, AdminDash.",
    "jsonFile": "projects/library/SportsDashboards Template.json",
    "sourcePile": "library",
    "stub": true,
    "deployed": false,
    "startPoints": [
      {
        "label": "Registration"
      },
      {
        "label": "AdminDash"
      }
    ],
    "testDriveUrl": null
  },
  "st-patrick-sportsdashboards": {
    "name": "St Patrick SportsDashboards (stub)",
    "category": "Sports",
    "featured": false,
    "iconLabel": "SP",
    "rating": 0,
    "comments": 0,
    "updated": "7/28/26",
    "shortDescription": "Converted project (100 forms). Start points: Registration, AdminDash, UtilityToSetPlayerAges.",
    "longDescription": "Backup copy from ~/Projects/Tawala Projects/WebLibrary. 100 forms, 120 processes, 60 documents. Start points: Registration, AdminDash, UtilityToSetPlayerAges, RegistrantDeduping.",
    "jsonFile": "projects/library/St Patrick SportsDashboards.json",
    "sourcePile": "library",
    "stub": true,
    "deployed": false,
    "startPoints": [
      {
        "label": "Registration"
      },
      {
        "label": "AdminDash"
      },
      {
        "label": "UtilityToSetPlayerAges"
      },
      {
        "label": "RegistrantDeduping"
      }
    ],
    "testDriveUrl": null
  },
  "tawala-invoicing": {
    "name": "Tawala Invoicing (stub)",
    "category": "Advanced",
    "featured": false,
    "iconLabel": "TI",
    "rating": 0,
    "comments": 0,
    "updated": "7/28/26",
    "shortDescription": "Converted project (6 forms). Start points: Start.",
    "longDescription": "Backup copy from ~/Projects/Tawala Projects/WebLibrary. 6 forms, 8 processes, 9 documents. Start points: Start.",
    "jsonFile": "projects/library/Tawala Invoicing.json",
    "sourcePile": "library",
    "stub": true,
    "deployed": false,
    "startPoints": [
      {
        "label": "Start"
      }
    ],
    "testDriveUrl": null
  }
};
window.TAWALA_MYTAWALA = {
  "bbbulkmail": {
    "name": "BBBulkMail",
    "category": "Business",
    "featured": false,
    "iconLabel": "BB",
    "rating": 0,
    "comments": 0,
    "updated": "7/28/26",
    "shortDescription": "Converted project (2 forms). Start points: Start.",
    "longDescription": "Backup copy from ~/Projects/Tawala Projects/MyTawala. 2 forms, 3 processes, 2 documents. Start points: Start.",
    "jsonFile": "projects/mytawala/BBBulkMail.json",
    "sourcePile": "mytawala",
    "deployed": false,
    "startPoints": [
      {
        "label": "Start"
      }
    ],
    "testDriveUrl": null
  },
  "campaigndashboards": {
    "name": "CampaignDashboards",
    "category": "Sports",
    "featured": false,
    "iconLabel": "CA",
    "rating": 0,
    "comments": 0,
    "updated": "7/28/26",
    "shortDescription": "Converted project (35 forms). Start points: AdminDash.",
    "longDescription": "Backup copy from ~/Projects/Tawala Projects/MyTawala. 35 forms, 42 processes, 26 documents. Start points: AdminDash.",
    "jsonFile": "projects/mytawala/CampaignDashboards.json",
    "sourcePile": "mytawala",
    "deployed": false,
    "startPoints": [
      {
        "label": "AdminDash"
      }
    ],
    "testDriveUrl": null
  },
  "cyo-check-request": {
    "name": "CYO Check Request",
    "category": "Sports",
    "featured": false,
    "iconLabel": "CC",
    "rating": 0,
    "comments": 0,
    "updated": "7/28/26",
    "shortDescription": "Converted project (6 forms). Start points: Start, CheckReqForm.",
    "longDescription": "Backup copy from ~/Projects/Tawala Projects/MyTawala. 6 forms, 7 processes, 4 documents. Start points: Start, CheckReqForm.",
    "jsonFile": "projects/mytawala/CYO Check Request.json",
    "sourcePile": "mytawala",
    "deployed": false,
    "startPoints": [
      {
        "label": "Start"
      },
      {
        "label": "CheckReqForm"
      }
    ],
    "testDriveUrl": null
  },
  "cyo-checkdeposit-request": {
    "name": "CYO CheckDeposit Request",
    "category": "Sports",
    "featured": false,
    "iconLabel": "CC",
    "rating": 0,
    "comments": 0,
    "updated": "7/28/26",
    "shortDescription": "Converted project (8 forms). Start points: Admin, CheckReqForm, Setup.",
    "longDescription": "Backup copy from ~/Projects/Tawala Projects/MyTawala. 8 forms, 8 processes, 3 documents. Start points: Admin, CheckReqForm, Setup.",
    "jsonFile": "projects/mytawala/CYO CheckDeposit Request.json",
    "sourcePile": "mytawala",
    "deployed": false,
    "startPoints": [
      {
        "label": "Admin"
      },
      {
        "label": "CheckReqForm"
      },
      {
        "label": "Setup"
      }
    ],
    "testDriveUrl": null
  },
  "designer-candidate-app-01": {
    "name": "Designer Candidate App 01",
    "category": "Dev / test",
    "featured": false,
    "iconLabel": "DC",
    "rating": 0,
    "comments": 0,
    "updated": "7/28/26",
    "shortDescription": "Converted project (3 forms). Start points: Form 1, Form 2, Form 3.",
    "longDescription": "Backup copy from ~/Projects/Tawala Projects/MyTawala. 3 forms, 3 processes, 3 documents. Start points: Form 1, Form 2, Form 3.",
    "jsonFile": "projects/mytawala/Designer Candidate App 01.json",
    "sourcePile": "mytawala",
    "deployed": false,
    "startPoints": [
      {
        "label": "Form 1"
      },
      {
        "label": "Form 2"
      },
      {
        "label": "Form 3"
      }
    ],
    "testDriveUrl": null
  },
  "dirtbowl-communicator": {
    "name": "Dirtbowl Communicator",
    "category": "Sports",
    "featured": false,
    "iconLabel": "DC",
    "rating": 0,
    "comments": 0,
    "updated": "7/28/26",
    "shortDescription": "Converted project (19 forms). Start points: Start.",
    "longDescription": "Backup copy from ~/Projects/Tawala Projects/MyTawala. 19 forms, 23 processes, 6 documents. Start points: Start.",
    "jsonFile": "projects/mytawala/Dirtbowl Communicator.json",
    "sourcePile": "mytawala",
    "deployed": false,
    "startPoints": [
      {
        "label": "Start"
      }
    ],
    "testDriveUrl": null
  },
  "displaylabeltest": {
    "name": "DisplayLabelTest",
    "category": "Dev / test",
    "featured": false,
    "iconLabel": "DI",
    "rating": 0,
    "comments": 0,
    "updated": "7/28/26",
    "shortDescription": "Converted project (1 forms). Start points: Form 1.",
    "longDescription": "Backup copy from ~/Projects/Tawala Projects/MyTawala. 1 forms, 0 processes, 0 documents. Start points: Form 1.",
    "jsonFile": "projects/mytawala/DisplayLabelTest.json",
    "sourcePile": "mytawala",
    "deployed": false,
    "startPoints": [
      {
        "label": "Form 1"
      }
    ],
    "testDriveUrl": null
  },
  "emailer-with-signup": {
    "name": "Emailer With Signup.dgmod",
    "category": "Activities",
    "featured": false,
    "iconLabel": "EW",
    "rating": 0,
    "comments": 0,
    "updated": "7/28/26",
    "shortDescription": "Converted project (15 forms). Start points: EmailerStart, JoinMailingList.",
    "longDescription": "Backup copy from ~/Projects/Tawala Projects/MyTawala. 15 forms, 21 processes, 7 documents. Start points: EmailerStart, JoinMailingList.",
    "jsonFile": "projects/mytawala/Emailer With Signup.dgmod.json",
    "sourcePile": "mytawala",
    "deployed": false,
    "startPoints": [
      {
        "label": "EmailerStart"
      },
      {
        "label": "JoinMailingList"
      }
    ],
    "testDriveUrl": null
  },
  "paypal-tester": {
    "name": "PayPal Tester",
    "category": "Business",
    "featured": false,
    "iconLabel": "PT",
    "rating": 0,
    "comments": 0,
    "updated": "7/28/26",
    "shortDescription": "Converted project (3 forms). Start points: Start.",
    "longDescription": "Backup copy from ~/Projects/Tawala Projects/MyTawala. 3 forms, 3 processes, 2 documents. Start points: Start.",
    "jsonFile": "projects/mytawala/PayPal Tester.json",
    "sourcePile": "mytawala",
    "deployed": false,
    "startPoints": [
      {
        "label": "Start"
      }
    ],
    "testDriveUrl": null
  },
  "potluck-kids-too": {
    "name": "Potluck - Kids Too",
    "category": "Meetings",
    "featured": false,
    "iconLabel": "PK",
    "rating": 0,
    "comments": 0,
    "updated": "7/28/26",
    "shortDescription": "Converted project (4 forms). Start points: Setup, Potluck Organizer, Administration.",
    "longDescription": "Backup copy from ~/Projects/Tawala Projects/MyTawala. 4 forms, 7 processes, 11 documents. Start points: Setup, Potluck Organizer, Administration.",
    "jsonFile": "projects/mytawala/Potluck - Kids Too.json",
    "sourcePile": "mytawala",
    "deployed": false,
    "startPoints": [
      {
        "label": "Setup"
      },
      {
        "label": "Potluck Organizer"
      },
      {
        "label": "Administration"
      }
    ],
    "testDriveUrl": null
  },
  "realdirtwheader": {
    "name": "RealDirtwHeader",
    "category": "Advanced",
    "featured": false,
    "iconLabel": "RE",
    "rating": 0,
    "comments": 0,
    "updated": "7/28/26",
    "shortDescription": "Converted project (60 forms). Start points: Registration, AdminDash.",
    "longDescription": "Backup copy from ~/Projects/Tawala Projects/MyTawala. 60 forms, 69 processes, 35 documents. Start points: Registration, AdminDash.",
    "jsonFile": "projects/mytawala/RealDirtwHeader.json",
    "sourcePile": "mytawala",
    "deployed": false,
    "startPoints": [
      {
        "label": "Registration"
      },
      {
        "label": "AdminDash"
      }
    ],
    "testDriveUrl": null
  },
  "shared-to-do": {
    "name": "Shared To-Do",
    "category": "Activities",
    "featured": false,
    "iconLabel": "ST",
    "rating": 0,
    "comments": 0,
    "updated": "7/28/26",
    "shortDescription": "Converted project (9 forms). Start points: Administration, Setup, Signup.",
    "longDescription": "Backup copy from ~/Projects/Tawala Projects/MyTawala. 9 forms, 13 processes, 10 documents. Start points: Administration, Setup, Signup.",
    "jsonFile": "projects/mytawala/Shared To-Do.json",
    "sourcePile": "mytawala",
    "deployed": false,
    "startPoints": [
      {
        "label": "Administration"
      },
      {
        "label": "Setup"
      },
      {
        "label": "Signup"
      }
    ],
    "testDriveUrl": null
  },
  "single-question-poll-or-survey": {
    "name": "Single Question Poll or Survey",
    "category": "Polls",
    "featured": false,
    "iconLabel": "SQ",
    "rating": 0,
    "comments": 0,
    "updated": "7/28/26",
    "shortDescription": "Converted project (5 forms). Start points: Customize, Start Questionnaire, Administration.",
    "longDescription": "Backup copy from ~/Projects/Tawala Projects/MyTawala. 5 forms, 6 processes, 5 documents. Start points: Customize, Start Questionnaire, Administration.",
    "jsonFile": "projects/mytawala/Single Question Poll or Survey.json",
    "sourcePile": "mytawala",
    "deployed": false,
    "startPoints": [
      {
        "label": "Customize"
      },
      {
        "label": "Start Questionnaire"
      },
      {
        "label": "Administration"
      }
    ],
    "testDriveUrl": null
  }
};

/** @deprecated Use TAWALA_LIBRARY — kept so older snippets keep working. */
window.TAWALA_DEMO_URLS = window.TAWALA_LIBRARY;

/** Helpers shared by library / home / My Tawala / detail pages. */
window.TawalaDemo = {
  /** Listing title — never show file extensions (.json / .tawala). On-disk format may still be JSON. */
  displayName(name) {
    return String(name || "")
      .replace(/\.tawala\.xml$/i, "")
      .replace(/\.tawala$/i, "")
      .replace(/\.json$/i, "");
  },
  /** Base repo categories + admin Add/Rename/Delete overlay (see transfer.js § category defs). */
  libraryCategories() {
    if (
      typeof window !== "undefined" &&
      window.TawalaTransfer &&
      typeof window.TawalaTransfer.effectiveLibraryCategories === "function"
    ) {
      return window.TawalaTransfer.effectiveLibraryCategories();
    }
    return window.TAWALA_LIBRARY_CATEGORIES || [];
  },
  categoryBySlug(slug) {
    return this.libraryCategories().find((c) => c.slug === slug) || null;
  },
  categoryByLabel(label) {
    return this.libraryCategories().find((c) => c.label === label) || null;
  },
  libraryEntries() {
    const base = Object.keys(window.TAWALA_LIBRARY).map((id) => ({
      id,
      ...window.TAWALA_LIBRARY[id],
    }));
    // Publish (My Tawala → Library) writes a localStorage overlay (transfer.js) and can
    // retire a matching stub out of the listing. See README § Publish.
    if (
      typeof window !== "undefined" &&
      window.TawalaTransfer &&
      typeof window.TawalaTransfer.withLibraryOverlay === "function"
    ) {
      return window.TawalaTransfer.withLibraryOverlay(base);
    }
    return base;
  },
  myTawalaEntries() {
    const base = Object.keys(window.TAWALA_MYTAWALA).map((id) => ({
      id,
      ...window.TAWALA_MYTAWALA[id],
    }));
    // Deploy → Show in My Tawala writes a localStorage overlay (transfer.js).
    // Deleted ids (account-private) are filtered inside withMyTawalaOverlay.
    if (
      typeof window !== "undefined" &&
      window.TawalaTransfer &&
      typeof window.TawalaTransfer.withMyTawalaOverlay === "function"
    ) {
      return window.TawalaTransfer.withMyTawalaOverlay(base);
    }
    return base;
  },
  /** Prefer Library, then My Tawala (detail pages that accept either id). */
  get(id) {
    return this.getLibrary(id) || this.getMyTawala(id) || null;
  },
  getLibrary(id) {
    if (!id) return null;
    const hasTransfer = typeof window !== "undefined" && window.TawalaTransfer;
    // A retired stub is gone from the public Library — only a fresh Publish overlay at the
    // same id (rare) should still resolve here; otherwise callers fall back to My Tawala.
    if (hasTransfer && typeof window.TawalaTransfer.isLibraryRetired === "function" && window.TawalaTransfer.isLibraryRetired(id)) {
      const overlayOnly =
        typeof window.TawalaTransfer.getLibraryOverlayEntry === "function"
          ? window.TawalaTransfer.getLibraryOverlayEntry(id)
          : null;
      return overlayOnly;
    }
    const base = window.TAWALA_LIBRARY[id] || null;
    if (hasTransfer && typeof window.TawalaTransfer.getLibraryOverlayEntry === "function") {
      const overlay = window.TawalaTransfer.getLibraryOverlayEntry(id);
      if (overlay) return base ? { ...base, ...overlay, id } : { ...overlay, id };
    }
    return base;
  },
  getMyTawala(id) {
    if (!id) return null;
    if (
      typeof window !== "undefined" &&
      window.TawalaTransfer &&
      typeof window.TawalaTransfer.isMyTawalaDeleted === "function" &&
      window.TawalaTransfer.isMyTawalaDeleted(id)
    ) {
      return null;
    }
    const base = window.TAWALA_MYTAWALA[id] || null;
    if (
      typeof window !== "undefined" &&
      window.TawalaTransfer &&
      typeof window.TawalaTransfer.getOverlayEntry === "function"
    ) {
      const overlay = window.TawalaTransfer.getOverlayEntry(id);
      if (overlay) {
        return base ? { ...base, ...overlay, id } : { ...overlay, id };
      }
    }
    return base;
  },
  /** @deprecated Prefer libraryEntries() */
  entries() {
    return this.libraryEntries();
  },
  featured() {
    return this.libraryEntries().filter((p) => p.featured);
  },
  /** Non-featured library projects (home “Featured Solutions” list below the icons). */
  moreSolutions() {
    return this.libraryEntries().filter((p) => !p.featured);
  },
  isDeployed(p) {
    return !!(p && p.deployed && p.testDriveUrl);
  },
  /** Owner-vetted Library product with a live :8080 test-drive (distinct from placeholders). */
  isLiveReady(p) {
    return !!(p && p.liveReady && this.isDeployed(p));
  },
  /** Quiet “Live” cue for vetted rows — not a banner; placeholders omit this. */
  liveReadyHtml(p) {
    try {
      if (!this.isLiveReady(p)) return "";
      return (
        '<span class="library-live-ready" title="Vetted · live on localhost:8080">Live</span>'
      );
    } catch {
      return "";
    }
  },
  /** Same contract as designer-web `isValidUniqueId` (1–20 alphanumeric). */
  isValidUniqueId(uniqueId) {
    return typeof uniqueId === "string" && /^[A-Za-z0-9]{1,20}$/.test(uniqueId);
  },
  /** Extract uniqueId from `/p/{uniqueId}/…` (Library / My Tawala test-drive URLs). */
  uniqueIdFromUrl(url) {
    if (!url) return null;
    const m = String(url).match(/\/p\/([A-Za-z0-9]{1,20})(?:\/|$)/);
    return m ? m[1] : null;
  },
  /**
   * Prefer explicit deploy uniqueId (overlay / receipt), then testDriveUrl,
   * then first start point with a URL.
   */
  uniqueIdForProject(p) {
    if (!p) return null;
    if (this.isValidUniqueId(p.uniqueId)) return p.uniqueId;
    const fromTest = this.uniqueIdFromUrl(p.testDriveUrl);
    if (fromTest) return fromTest;
    const sps = p.startPoints || [];
    for (let i = 0; i < sps.length; i++) {
      const id = this.uniqueIdFromUrl(sps[i] && sps[i].url);
      if (id) return id;
    }
    return null;
  },
  /**
   * Resolve :8080 uniqueId for My Tawala Purge.
   * Prefer My Tawala overlay/seed, then latest deploy-inbox receipt.
   * Does not read public Library catalog (avoids purging a twin’s data by slug collision).
   */
  resolvePurgeUniqueId(projectId) {
    if (!projectId) return null;
    const mine =
      typeof this.getMyTawala === "function" ? this.getMyTawala(projectId) : null;
    const fromMine = this.uniqueIdForProject(mine);
    if (fromMine) return fromMine;

    if (
      typeof window !== "undefined" &&
      window.TawalaTransfer &&
      typeof window.TawalaTransfer.getDeployInbox === "function"
    ) {
      const receipts = window.TawalaTransfer.getDeployInbox().filter(
        (e) => e && e.id === projectId
      );
      for (let i = 0; i < receipts.length; i++) {
        const r = receipts[i];
        if (this.isValidUniqueId(r.uniqueId)) return r.uniqueId;
        const sps = Array.isArray(r.startpoints) ? r.startpoints : [];
        for (let j = 0; j < sps.length; j++) {
          const id = this.uniqueIdFromUrl(sps[j] && sps[j].url);
          if (id) return id;
        }
      }
    }

    return null;
  },
  /**
   * Dev API base for purge (designer-web :3001). Override with window.TAWALA_DEV_API.
   */
  purgeApiBase() {
    return (typeof window !== "undefined" && window.TAWALA_DEV_API) || "http://localhost:3001";
  },
  /**
   * Purge Postgres submissions (+ Node session if present) for a uniqueId.
   * Requires designer-web API + Docker Postgres for :8080 projects.
   */
  async purgeResponses(uniqueId) {
    if (!uniqueId) {
      return { status: "failure", error: "uniqueId required" };
    }
    const url = this.purgeApiBase().replace(/\/$/, "") + "/api/purge-responses";
    try {
      const res = await fetch(url, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          uniqueId,
          credentials: { user: "dev", password: "dev" },
        }),
      });
      const data = await res.json().catch(() => ({}));
      if (!res.ok) {
        return {
          status: "failure",
          uniqueId,
          error: data.error || data.javaDb?.error || `HTTP ${res.status}`,
          ...data,
        };
      }
      return data;
    } catch (e) {
      return {
        status: "failure",
        uniqueId,
        error:
          String(e.message || e) +
          " — is designer-web API on :3001? (cd designer-web && npm run dev)",
      };
    }
  },
  /**
   * Purge :8080 submissions for a My Tawala project after Publish (owner Aug 1, 2026 — Publish
   * to Library must never leave one account's prior test/demo responses visible to whoever
   * uses the newly-published Library project next). Resolves the same uniqueId as My Tawala
   * PURGE (`resolvePurgeUniqueId`) and reuses `purgeResponses`. Returns `{ status: "skipped" }`
   * when the source project has no linked :8080 deploy yet — Publish still succeeds; callers
   * must surface that responses were NOT cleared rather than staying silent about it.
   */
  async purgeAfterPublish(projectId) {
    const uniqueId = this.resolvePurgeUniqueId(projectId);
    if (!uniqueId) {
      return { status: "skipped", reason: "no-uniqueid", uniqueId: null };
    }
    const result = await this.purgeResponses(uniqueId);
    return { ...result, uniqueId };
  },
  /**
   * Purge project response data then open the :8080 form (clean slate each Test drive).
   * Opens a blank tab synchronously (keeps the user gesture for popup blockers),
   * then navigates after purge. Failed / timed-out purge never blocks opening the form.
   * Post-tab-close purge is not available in this static mock.
   */
  async openTestDrive(url, opts) {
    const options = opts || {};
    const purgeFirst = options.purge !== false;
    const purgeMs = typeof options.purgeTimeoutMs === "number" ? options.purgeTimeoutMs : 8000;
    const target = url || null;
    if (!target) return { opened: false, purge: null };

    // Capture gesture before any await — otherwise browsers block the popup.
    const tab = window.open("about:blank", "_blank");

    let purge = null;
    try {
      if (purgeFirst) {
        const id = this.uniqueIdFromUrl(target);
        if (id) {
          const timedOut = new Promise((resolve) => {
            setTimeout(
              () => resolve({ status: "failure", uniqueId: id, error: `purge timed out after ${purgeMs}ms` }),
              purgeMs
            );
          });
          purge = await Promise.race([this.purgeResponses(id), timedOut]);
          if (purge.status !== "success") {
            console.warn("[TawalaDemo] purge before test drive failed:", purge.error || purge);
          }
        }
      }
    } catch (e) {
      purge = { status: "failure", error: String((e && e.message) || e) };
      console.warn("[TawalaDemo] purge before test drive threw:", purge.error);
    }

    // Always navigate — blank tab must not stick on purge failure.
    if (tab && !tab.closed) {
      try {
        tab.opener = null;
      } catch {
        /* ignore */
      }
      try {
        tab.location.href = target;
        return { opened: true, purge };
      } catch (navErr) {
        console.warn("[TawalaDemo] tab navigate failed, falling back:", navErr);
      }
    }
    // Popup blocked or navigate failed — last resort (may also be blocked after await).
    window.open(target, "_blank", "noopener");
    return { opened: true, purge, popupBlocked: !tab };
  },
  /**
   * Export submission data for a deployed project (EXPORT / data half of BACKUP).
   * Postgres (Java) or dev session store — see designer-web/server/projectResponses.mjs.
   * @returns {Promise<{status:"success"|"failure", uniqueId:string, source?:"postgres"|"dev-session",
   *   forms?:Array, fieldsByForm?:Record<string,string[]>, count?:number, error?:string}>}
   */
  async exportResponses(uniqueId) {
    if (!uniqueId) return { status: "failure", error: "uniqueId required" };
    const url = this.purgeApiBase().replace(/\/$/, "") + "/api/export-responses";
    try {
      const res = await fetch(url, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ uniqueId, credentials: { user: "dev", password: "dev" } }),
      });
      const data = await res.json().catch(() => ({}));
      if (!res.ok) {
        return { status: "failure", uniqueId, error: data.error || `HTTP ${res.status}`, ...data };
      }
      return data;
    } catch (e) {
      return {
        status: "failure",
        uniqueId,
        error: String(e.message || e) + " — is designer-web API on :3001? (cd designer-web && npm run dev)",
      };
    }
  },
  /**
   * Replace submission data for a deployed project (IMPORT / data half of RESTORE).
   * `forms` must match the shape returned by exportResponses() for the given `source`.
   * @param {string} uniqueId
   * @param {Array} forms
   * @param {{source?: "postgres"|"dev-session", mode?: "replace"|"merge"}} opts
   */
  async importResponses(uniqueId, forms, opts) {
    const options = opts || {};
    if (!uniqueId) return { status: "failure", error: "uniqueId required" };
    if (!Array.isArray(forms)) return { status: "failure", error: "forms array required" };
    const url = this.purgeApiBase().replace(/\/$/, "") + "/api/import-responses";
    try {
      const res = await fetch(url, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          uniqueId,
          forms,
          source: options.source || null,
          mode: options.mode || "replace",
          credentials: { user: "dev", password: "dev" },
        }),
      });
      const data = await res.json().catch(() => ({}));
      if (!res.ok) {
        return { status: "failure", uniqueId, error: data.error || `HTTP ${res.status}`, ...data };
      }
      return data;
    } catch (e) {
      return {
        status: "failure",
        uniqueId,
        error: String(e.message || e) + " — is designer-web API on :3001? (cd designer-web && npm run dev)",
      };
    }
  },
  /**
   * Prefer end-user start forms over Admin/Setup/Preview when ranking a single
   * operate URL (Online Exam Builder: Exam vs Administration). Used when Use
   * opens a single-start runtime link, and for start-point list ordering.
   * Multi-start Use navigates to Project Details instead (see projectUseTarget).
   * @param {Array<{label?:string,form?:string,url?:string|null}>} startPoints
   * @returns {{label?:string,form?:string,url?:string|null}|null}
   */
  pickPrimaryStartPoint(startPoints) {
    const list = (startPoints || []).filter((s) => s && s.url);
    if (!list.length) return null;
    const labelOf = (s) => String(s.label || s.form || "").trim();
    // Exact / high-value public entry forms first.
    const prefer = [
      /^exam$/i,
      /^registration$/i,
      /^survey$/i,
      /^form\s*\+?\s*1$/i,
      /^form\s*1$/i,
      /^sign[-\s]?up/i,
      /^potluck/i,
      /^exception\s*request/i,
    ];
    for (let p = 0; p < prefer.length; p++) {
      const hit = list.find((s) => prefer[p].test(labelOf(s)));
      if (hit) return hit;
    }
    // Skip admin / setup / preview / reports when a plain operate form exists.
    const deprioritize =
      /admin|setup|customiz|preview|utility|report|dash|config|scoring|answer/i;
    const nonAdmin = list.find((s) => !deprioritize.test(labelOf(s)));
    return nonAdmin || list[0];
  },
  /**
   * Library Test Drive entry point — for apps with Exam + Administration/Setup
   * (Online Exam Builder), open Admin/Setup first so the drive starts where you
   * configure questions. My Tawala Use for multi-start goes to Project Details
   * (not Exam); single-start Use still uses pickPrimaryStartPoint.
   */
  pickLibraryTestDriveStartPoint(startPoints) {
    const list = (startPoints || []).filter((s) => s && s.url);
    if (!list.length) return null;
    const labelOf = (s) => String(s.label || s.form || "").trim();
    const hasExam = list.some((s) => /^exam$/i.test(labelOf(s)));
    if (hasExam) {
      const setupPrefer = [/^administration$/i, /^setup$/i, /^admin$/i];
      for (let p = 0; p < setupPrefer.length; p++) {
        const hit = list.find((s) => setupPrefer[p].test(labelOf(s)));
        if (hit) return hit;
      }
    }
    return this.pickPrimaryStartPoint(startPoints);
  },
  /** Preferred :8080 URL when ranking start points (Exam over Admin when both exist). */
  primaryStartUrl(startPoints, fallbackUrl) {
    const primary = this.pickPrimaryStartPoint(startPoints);
    if (primary && primary.url) return primary.url;
    return fallbackUrl || null;
  },
  /**
   * Library listing / detail Test Drive URL. Prefer Setup/Admin for Exam apps;
   * fall back to stored testDriveUrl.
   */
  libraryTestDriveUrl(project) {
    if (!project) return null;
    const preferred = this.pickLibraryTestDriveStartPoint(project.startPoints);
    if (preferred && preferred.url) return preferred.url;
    return project.testDriveUrl || null;
  },
  /** Delegated clicks for elements with data-testdrive-url (or .js-testdrive href). */
  bindTestDriveClicks(root) {
    const scope = root || document;
    if (scope.__tawalaTestDriveBound) return;
    scope.__tawalaTestDriveBound = true;
    scope.addEventListener("click", (ev) => {
      const el = ev.target.closest("[data-testdrive-url], a.js-testdrive");
      if (!el) return;
      const href =
        el.getAttribute("data-testdrive-url") ||
        (el.classList.contains("js-testdrive") ? el.getAttribute("href") : null);
      if (!href || href === "#") return;
      ev.preventDefault();
      // data-testdrive-purge="false" → open without wiping DB (My Tawala operate / exam after Admin setup).
      const purgeAttr = el.getAttribute("data-testdrive-purge");
      const purge = purgeAttr !== "false" && purgeAttr !== "0";
      void this.openTestDrive(href, { purge });
    });
  },
  /** Inline stars after the project name (no separate Rating column). Unrated → omit. */
  starsHtml(rating) {
    const n = Number(rating) || 0;
    if (n <= 0) return "";
    let html = '<span class="rating-stars" aria-label="' + n + ' of 5 stars">';
    for (let i = 1; i <= 5; i++) {
      html += '<span class="star' + (i <= n ? " on" : "") + '">★</span>';
    }
    return html + "</span>";
  },
  /**
   * Start-point link HTML.
   * @param {{label?:string,url?:string|null}} sp
   * @param {{purge?:boolean}} opts — Default purge true (listing / primary Test Drive).
   *   Project Details (Library and My Tawala) pass purge:false so Admin/Setup writes
   *   (questions / config) survive when opening Exam / Registration next.
   */
  startPointHtml(sp, opts) {
    const options = opts || {};
    const purge = options.purge !== false;
    const label = String(sp.label || sp.form || "Start");
    const escLabel = label.replace(/&/g, "&amp;").replace(/</g, "&lt;");
    if (sp.url) {
      const safeUrl = String(sp.url).replace(/"/g, "&quot;");
      if (purge) {
        return (
          '<a class="js-testdrive" href="' +
          sp.url +
          '" data-testdrive-url="' +
          safeUrl +
          '" target="_blank" rel="noopener" title="Purges prior responses for this project, then opens :8080">' +
          escLabel +
          "</a>"
        );
      }
      // Project Details / operate: full token URL as-is, no purge (use PURGE / primary Test Drive to clear).
      return (
        '<a href="' +
        sp.url +
        '" target="_blank" rel="noopener" title="Open this start form on :8080 (keeps project data; use PURGE or primary Test Drive to clear)">' +
        escLabel +
        "</a>"
      );
    }
    return (
      '<span class="start-point-pending" title="No local :8080 URL yet (dev)">' +
      escLabel +
      "</span>"
    );
  },
};

/** Bind once when the catalog script loads (Library / home / My Tawala pages). */
if (typeof document !== "undefined") {
  if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", () => window.TawalaDemo.bindTestDriveClicks(document));
  } else {
    window.TawalaDemo.bindTestDriveClicks(document);
  }
}
