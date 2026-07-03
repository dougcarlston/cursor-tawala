/** New Project templates — mirrors legacy `templates.xml` featured set. */

export interface TemplateEntry {
  id: string;
  label: string;
  category: string;
  description: string;
  /** Path under /samples/templates/ */
  samplePath: string;
  /** Home-page featured apps (Simple Survey, Sign-up, Potluck, Get Together) */
  featured?: boolean;
}

export const TEMPLATE_CATEGORIES = [
  "Basic",
  "Activities",
  "Meetings and Gatherings",
  "Polls and Surveys",
] as const;

export const PROJECT_TEMPLATES: TemplateEntry[] = [
  {
    id: "empty",
    label: "Empty",
    category: "Basic",
    description: "Start with an empty project.",
    samplePath: "empty-project.json",
  },
  {
    id: "form-process-document",
    label: "Form, Process and Document",
    category: "Basic",
    description: "Form connected to a process that shows a document (Designer layout demo).",
    samplePath: "form-process-document.json",
  },
  {
    id: "form-with-process",
    label: "Form with Process",
    category: "Basic",
    description: "Empty form linked to an empty process.",
    samplePath: "form-with-process.json",
  },
  {
    id: "signup-sheet",
    label: "Sign-up Sheet",
    category: "Activities",
    description: "Collect contact info; signups appear in a table on the same form.",
    samplePath: "signup-sheet.json",
    featured: true,
  },
  {
    id: "get-together",
    label: "Get Together",
    category: "Meetings and Gatherings",
    description: "Pick the best date — availability plus top preference.",
    samplePath: "get-together.json",
    featured: true,
  },
  {
    id: "simple-survey",
    label: "Simple Survey",
    category: "Polls and Surveys",
    description: "One multiple-choice question with a results report.",
    samplePath: "simple-survey.json",
    featured: true,
  },
  {
    id: "potluck",
    label: "Potluck",
    category: "Meetings and Gatherings",
    description: "Potluck invitation — RSVP, headcount, and dish contribution (simplified starter).",
    samplePath: "potluck.json",
    featured: true,
  },
];

export function templatesByCategory(): Map<string, TemplateEntry[]> {
  const map = new Map<string, TemplateEntry[]>();
  for (const cat of TEMPLATE_CATEGORIES) {
    map.set(
      cat,
      PROJECT_TEMPLATES.filter((t) => t.category === cat),
    );
  }
  return map;
}
