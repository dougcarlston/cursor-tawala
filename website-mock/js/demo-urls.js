/**
 * Local 8080 start URLs — update after redeploying templates.
 * Regenerate: node scripts/deploy-tawala-template.mjs "<Template Name>"
 */
window.TAWALA_DEMO_URLS = {
  "simple-survey": {
    name: "Simple Survey Template",
    category: "Polls",
    shortDescription: "A one-question survey with an instant results report.",
    longDescription:
      "Replace the sample question with your own multiple-choice question. " +
      "Respondents pick an answer; the Report form shows live tallies.",
    startPoints: [
      { label: "Survey", url: "http://localhost:8080/p/gy1zssbrwm4fgfm/npwtqlg.Survey" },
      { label: "Report", url: "http://localhost:8080/p/gy1zssbrwm4fgfm/d6ceolx.Report" },
    ],
    testDriveUrl: "http://localhost:8080/p/gy1zssbrwm4fgfm/npwtqlg.Survey",
  },
  "signup-sheet": {
    name: "Sign-up Sheet Template",
    category: "Activities",
    shortDescription: "Collect names and contact info; see signups in a table on the same page.",
    longDescription:
      "First name, last name, email, phone, and address fields feed an itemization table " +
      "so your group can see who has signed up without a separate report form.",
    startPoints: [
      {
        label: "Form 1",
        url: "http://localhost:8080/p/cicw55xxhvwrrh7/l2u4sdg.Form+1",
      },
    ],
    testDriveUrl: "http://localhost:8080/p/cicw55xxhvwrrh7/l2u4sdg.Form+1",
  },
  "form-with-process": {
    name: "Form with process",
    category: "Basic (Designer demo)",
    shortDescription: "Designer demo — empty form linked to an empty process (not a sample app).",
    longDescription:
      "Shows how to attach Process 1 to Form 1 in the Designer. " +
      "No fields or statements; used to teach form–process linking, not as a end-user template.",
    startPoints: [
      {
        label: "Form 1",
        url: "http://localhost:8080/p/5u8ql7u0nvla3co/3w6olnt.Form+1",
      },
    ],
    testDriveUrl: "http://localhost:8080/p/5u8ql7u0nvla3co/3w6olnt.Form+1",
  },
  potluck: {
    name: "Potluck Template",
    category: "Meetings",
    shortDescription: "Potluck invitation — headcount, dish contributions, and a shared report.",
    longDescription:
      "Invite guests to a potluck, collect RSVPs and what each person will bring. " +
      "Uses Potluck Organizer (start), Report, documents, and processes for thanks and delete.",
    startPoints: [
      {
        label: "Potluck Organizer",
        url: "http://localhost:8080/p/t03vtb1poh34kkn/2bpec4j.Potluck+Organizer",
      },
      { label: "Report", url: "http://localhost:8080/p/t03vtb1poh34kkn/3i70frf.Report" },
    ],
    testDriveUrl: "http://localhost:8080/p/t03vtb1poh34kkn/2bpec4j.Potluck+Organizer",
  },
  "get-together": {
    name: "Get Together Template",
    category: "Meetings",
    shortDescription: "Find the best date for an event — availability plus top preference.",
    longDescription:
      "Two MCQs: which dates work (multi-select) and top preference (single). " +
      "Report includes a question-correlation table to see the best overlap.",
    startPoints: [
      { label: "Survey", url: "http://localhost:8080/p/b6do4s50iq64vl8/g6zi1ar.Survey" },
      { label: "Report", url: "http://localhost:8080/p/b6do4s50iq64vl8/ejeypox.Report" },
    ],
    testDriveUrl: "http://localhost:8080/p/b6do4s50iq64vl8/g6zi1ar.Survey",
  },
  "form-process-document": {
    name: "Form with process connecting a document",
    category: "Basic (Designer demo)",
    shortDescription: "Designer demo — empty form whose process shows a document after submit.",
    longDescription:
      "Form 1 is linked to Process 1, which runs Show Document Document 1 after submit. " +
      "Teaches form–process–document wiring; not a featured sample app.",
    startPoints: [
      {
        label: "Form 1",
        url: "http://localhost:8080/p/x6oovxu7g8tjelp/ullhihn.Form+1",
      },
    ],
    testDriveUrl: "http://localhost:8080/p/x6oovxu7g8tjelp/ullhihn.Form+1",
  },
  "signup-sheet-email": {
    name: "Sign-up Sheet Template w Email",
    category: "Activities",
    shortDescription: "Sign-up sheet like the basic template, plus a Send process for new signups.",
    longDescription:
      "Same FIB fields and itemization table as the Sign-up Sheet template. " +
      "Process 1 sends the NewSignup document by email (placeholder address in template XML).",
    startPoints: [
      {
        label: "Form 1",
        url: "http://localhost:8080/p/onszvng2ec776jt/uwh7ift.Form+1",
      },
    ],
    testDriveUrl: "http://localhost:8080/p/onszvng2ec776jt/uwh7ift.Form+1",
  },
  "multiple-question-survey": {
    name: "Multiple Question Survey Template",
    category: "Polls",
    shortDescription: "Multi-question poll with bar-graph tallies and a response table on Report.",
    longDescription:
      "Survey collects name, several multiple-choice questions, and optional results link. " +
      "Report shows choice-tally tables per MCQ plus an itemization table of all responses.",
    startPoints: [
      {
        label: "Survey",
        url: "http://localhost:8080/p/grniytf6dvmobqe/y7ucha7.Survey",
      },
      {
        label: "Report",
        url: "http://localhost:8080/p/grniytf6dvmobqe/w2licdd.Report",
      },
    ],
    testDriveUrl: "http://localhost:8080/p/grniytf6dvmobqe/y7ucha7.Survey",
  },
};
