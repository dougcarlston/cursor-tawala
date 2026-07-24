/** New Project templates — mirrors legacy `templates.xml` featured set (labels + order). */

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

/**
 * Display order matches legacy New Project grid (owner Jul 23):
 * Basic → Activities → Meetings and Gatherings → Polls and Surveys.
 * Sign-up Sheet with E-mail omitted (mail backlog).
 */
export const PROJECT_TEMPLATES: TemplateEntry[] = [
  {
    id: "empty",
    label: "Empty",
    category: "Basic",
    description: "Start with an empty project.",
    samplePath: "empty-project.json",
  },
  {
    id: "form-with-process",
    label: "Form with Process",
    category: "Basic",
    description: "A simple project that contains a Form connected to Process.",
    samplePath: "form-with-process.json",
  },
  {
    id: "form-process-document",
    label: "Form, Process and Document",
    category: "Basic",
    description:
      "A simple project that contains a Form connected to Process and includes a Document.",
    samplePath: "form-process-document.json",
  },
  {
    id: "signup-sheet",
    label: "Sign-up Sheet",
    category: "Activities",
    description: "A sign-up sheet you can modify to meet your needs.",
    samplePath: "signup-sheet.json",
    featured: true,
  },
  {
    id: "get-together",
    label: "Get Together",
    category: "Meetings and Gatherings",
    description: "Select the best date for a meeting.",
    samplePath: "get-together.json",
    featured: true,
  },
  {
    id: "potluck",
    label: "Potluck",
    category: "Meetings and Gatherings",
    description: "Arrange a potluck.",
    samplePath: "potluck.json",
    featured: true,
  },
  {
    id: "simple-survey",
    label: "Simple Survey",
    category: "Polls and Surveys",
    description: "A simple survey you can modify to meet your needs.",
    samplePath: "simple-survey.json",
    featured: true,
  },
  {
    id: "multiple-question-survey",
    label: "Multiple Question Survey",
    category: "Polls and Surveys",
    description: "A survey you can modify to meet your needs.",
    samplePath: "multiple-question-survey.json",
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
