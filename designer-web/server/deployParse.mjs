/** Parse Java /client uploadProject response — filter to one project. */

export function startPointFormNames(project) {
  return new Set(
    (project?.forms ?? [])
      .filter((f) => f.startPoint === true)
      .map((f) => f.name),
  );
}

/** Keep only URLs for forms the designer marked as Starting Point. */
export function filterStartpointsForMarkedForms(project, startpoints) {
  const names = startPointFormNames(project);
  if (names.size === 0) return [];
  return startpoints.filter((sp) => names.has(sp.form));
}

export function parseStartpointsFromBlock(block) {
  const points = [];
  const re = /<startpoint form="([^"]+)" url="([^"]+)"/g;
  let m;
  while ((m = re.exec(block))) {
    points.push({ form: m[1], url: m[2] });
  }
  return points;
}

export function parseStartpointsForProject(xmlText, projectName) {
  if (!xmlText || !projectName) return [];

  const escaped = String(projectName).replace(/[.*+?^${}()|[\]\\]/g, "\\$&");
  const blockRe = new RegExp(
    `<deployment project="${escaped}"[^>]*>([\\s\\S]*?)</deployment>`,
    "i",
  );
  const blockMatch = xmlText.match(blockRe);
  if (blockMatch) return parseStartpointsFromBlock(blockMatch[1]);

  // Case-insensitive name fallback
  const allDeployments = [
    ...xmlText.matchAll(/<deployment project="([^"]*)"[^>]*>([\s\S]*?)<\/deployment>/g),
  ];
  const lower = String(projectName).toLowerCase();
  for (const [, name, body] of allDeployments) {
    if (name.toLowerCase() === lower) return parseStartpointsFromBlock(body);
  }

  return [];
}

export function parseDeployFailure(xmlText) {
  if (!xmlText) return null;
  const m = xmlText.match(/<error id="([^"]+)" message="([^"]*)"/);
  if (m) return `${m[1]}: ${m[2]}`;
  // Java sometimes returns the public Error HTML page instead of <response status="failure">
  // when project XML is rejected during upload (e.g. invalid function table markup).
  if (/<h1[^>]*>\s*Error\s*<\/h1>/i.test(xmlText) || /We are very sorry!/i.test(xmlText)) {
    return (
      "Java rejected the uploaded project XML (server Error page). " +
      "Common causes: invalid function tables (itemization headers must be expression form), " +
      "or unsupported export markup. Check the browser Designer export / server logs."
    );
  }
  if (/<!DOCTYPE\s+html/i.test(xmlText) && !/status="success"/i.test(xmlText)) {
    return "Java /client returned HTML instead of a deploy response — upload likely failed.";
  }
  return null;
}

export function uniqueIdFromStartpoints(startpoints) {
  const url = startpoints[0]?.url ?? "";
  const m = url.match(/\/p\/([^/]+)\//);
  return m ? m[1] : undefined;
}
