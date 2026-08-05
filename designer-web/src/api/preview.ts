import { TawalaProject } from "@/types/tawala";

export async function syncPreviewProject(
  project: TawalaProject,
  formName?: string,
  options?: { resetSession?: boolean },
): Promise<string> {
  const res = await fetch("/api/preview", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({
      project,
      formName,
      resetSession: options?.resetSession === true,
    }),
  });
  const data = await res.json();
  if (!res.ok) {
    throw new Error(data.error ?? res.statusText);
  }
  return (data.url as string) ?? "";
}

/** Clear Designer Form Preview answers/records for this project (File→New starts empty). */
export async function resetPreviewSession(project: TawalaProject): Promise<void> {
  await syncPreviewProject(project, undefined, { resetSession: true });
}
