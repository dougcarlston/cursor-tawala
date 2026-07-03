import { TawalaProject } from "@/types/tawala";

export async function syncPreviewProject(
  project: TawalaProject,
  formName: string,
): Promise<string> {
  const res = await fetch("/api/preview", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ project, formName }),
  });
  const data = await res.json();
  if (!res.ok) {
    throw new Error(data.error ?? res.statusText);
  }
  return data.url as string;
}
