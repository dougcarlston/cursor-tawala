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
  const m = xmlText.match(/<error id="([^"]+)" message="([^"]*)"/);
  return m ? `${m[1]}: ${m[2]}` : null;
}

export function uniqueIdFromStartpoints(startpoints) {
  const url = startpoints[0]?.url ?? "";
  const m = url.match(/\/p\/([^/]+)\//);
  return m ? m[1] : undefined;
}
