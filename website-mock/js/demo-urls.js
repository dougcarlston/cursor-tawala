/**
 * Library vs My Tawala catalogs + local :8080 start URLs.
 *
 * Source piles (JSON backups):
 *   website-mock/projects/library/  ← ~/Projects/Tawala Projects/WebLibrary
 *   website-mock/projects/mytawala/ ← ~/Projects/Tawala Projects/MyTawala
 *
 * Main Menu public templates (Simple Survey, Sign-up, Potluck, Get Together, …)
 * stay in the Library catalog with live :8080 URLs; JSON lives under
 * designer-web/public/samples/templates/. Designer-only New Project basics
 * (Empty/Blank, Form with Process, Form with Process & Document) are NOT listed.
 *
 * Library listing groups mirror Designer File → New Project categories
 * (Activities / Meetings and Gatherings / Polls and Surveys), plus WebLibrary
 * extras (Sports / Business / Advanced). Basic is Designer-only — never listed.
 *
 * Update live URLs after: node scripts/deploy-tawala-template.mjs "<Template Name>"
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
    "name": "AlexTimon",
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
    "deployed": false,
    "startPoints": [
      {
        "label": "Password"
      }
    ],
    "testDriveUrl": null
  },
  "automated-list-builder": {
    "name": "Automated List Builder",
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
    "name": "ClientProfiler",
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
    "deployed": false,
    "startPoints": [
      {
        "label": "Password"
      }
    ],
    "testDriveUrl": null
  },
  "cyo-checkdeposit-request1": {
    "name": "CYO CheckDeposit Request1",
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
    "name": "CYO Exceptions App",
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
    "name": "DirtBowl",
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
    "name": "GenericListManager",
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
    "category": "Polls and Surveys",
    "featured": false,
    "iconLabel": "HA",
    "rating": 0,
    "comments": 0,
    "updated": "7/28/26",
    "shortDescription": "Converted project (1 forms). Start points: Form 1.",
    "longDescription": "Backup copy from ~/Projects/Tawala Projects/WebLibrary. 1 forms, 1 processes, 6 documents. Start points: Form 1.",
    "jsonFile": "projects/library/Horses and Penguins Test.json",
    "sourcePile": "library",
    "deployed": false,
    "startPoints": [
      {
        "label": "Form 1"
      }
    ],
    "testDriveUrl": null
  },
  "league-age-calculator": {
    "name": "League Age calculator",
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
    "name": "Lunch Order Menu",
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
    "updated": "7/2/26",
    "shortDescription": "Multi-question poll with bar-graph tallies and a response table on Report.",
    "longDescription": "Survey collects name, several multiple-choice questions, and optional results link. Report shows choice-tally tables per MCQ plus an itemization table of all responses. Backup in website-mock/projects/library/.",
    "jsonFile": "projects/library/Multiple Question Survey Template.json",
    "sourcePile": "library",
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
    "name": "MVSC Communicator",
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
    "deployed": false,
    "startPoints": [
      {
        "label": "Start"
      }
    ],
    "testDriveUrl": null
  },
  "mvsc-registration": {
    "name": "MVSC Registration",
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
    "updated": "7/28/26",
    "shortDescription": "Converted project (11 forms). Start points: Exam, Administration, CustomizationPreview.",
    "longDescription": "Backup copy from ~/Projects/Tawala Projects/WebLibrary. 11 forms, 17 processes, 13 documents. Start points: Exam, Administration, CustomizationPreview, Setup.",
    "jsonFile": "projects/library/Online Exam Builder.json",
    "sourcePile": "library",
    "deployed": false,
    "startPoints": [
      {
        "label": "Exam"
      },
      {
        "label": "Administration"
      },
      {
        "label": "CustomizationPreview"
      },
      {
        "label": "Setup"
      }
    ],
    "testDriveUrl": null
  },
  "signup-sheet-email": {
    "name": "Sign-up Sheet Template w Email",
    "category": "Activities",
    "featured": false,
    "iconLabel": "SE",
    "rating": 4,
    "comments": 4,
    "updated": "7/2/26",
    "shortDescription": "Sign-up sheet like the basic template, plus a Send process for new signups.",
    "longDescription": "Same FIB fields and itemization table as the Sign-up Sheet template. Process 1 sends the NewSignup document by email (placeholder address in template XML).",
    "jsonFile": "designer-web/public/samples/templates/signup-sheet-w-email.json",
    "sourcePile": "main-menu",
    "deployed": true,
    "startPoints": [
      {
        "label": "Form 1",
        "url": "http://localhost:8080/p/onszvng2ec776jt/uwh7ift.Form+1"
      }
    ],
    "testDriveUrl": "http://localhost:8080/p/onszvng2ec776jt/uwh7ift.Form+1"
  },
  "sportsdashboards-template": {
    "name": "SportsDashboards Template",
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
    "name": "St Patrick SportsDashboards",
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
    "name": "Tawala Invoicing",
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
  libraryCategories() {
    return window.TAWALA_LIBRARY_CATEGORIES || [];
  },
  categoryBySlug(slug) {
    return this.libraryCategories().find((c) => c.slug === slug) || null;
  },
  categoryByLabel(label) {
    return this.libraryCategories().find((c) => c.label === label) || null;
  },
  libraryEntries() {
    return Object.keys(window.TAWALA_LIBRARY).map((id) => ({
      id,
      ...window.TAWALA_LIBRARY[id],
    }));
  },
  myTawalaEntries() {
    return Object.keys(window.TAWALA_MYTAWALA).map((id) => ({
      id,
      ...window.TAWALA_MYTAWALA[id],
    }));
  },
  /** Prefer Library, then My Tawala (detail pages that accept either id). */
  get(id) {
    return window.TAWALA_LIBRARY[id] || window.TAWALA_MYTAWALA[id] || null;
  },
  getLibrary(id) {
    return window.TAWALA_LIBRARY[id] || null;
  },
  getMyTawala(id) {
    return window.TAWALA_MYTAWALA[id] || null;
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
  startPointHtml(sp) {
    const label = String(sp.label || "Start");
    if (sp.url) {
      return (
        '<a href="' +
        sp.url +
        '" target="_blank" rel="noopener">' +
        label.replace(/&/g, "&amp;").replace(/</g, "&lt;") +
        "</a>"
      );
    }
    return (
      '<span class="start-point-pending" title="Not deployed on local :8080 yet">' +
      label.replace(/&/g, "&amp;").replace(/</g, "&lt;") +
      ' <em>(not deployed yet)</em></span>'
    );
  },
};
