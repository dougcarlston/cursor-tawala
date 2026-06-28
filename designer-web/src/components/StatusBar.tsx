import { useEffect, useState } from "react";
import { useProjectStore } from "@/store/projectStore";

export function StatusBar() {
  const statusMessage = useProjectStore((s) => s.statusMessage);
  const dirty = useProjectStore((s) => s.dirty);
  const project = useProjectStore((s) => s.project);
  const credentials = useProjectStore((s) => s.credentials);
  const [runtime, setRuntime] = useState<"dev" | "java" | "…">("…");

  useEffect(() => {
    fetch("/api/health")
      .then((r) => r.json())
      .then((d) => setRuntime(d.runtime === "java" ? "java" : "dev"))
      .catch(() => setRuntime("dev"));
  }, []);

  return (
    <footer className="status-bar">
      <span>{statusMessage}</span>
      <span>
        {project.name} · format {project.format}
        {dirty ? <span className="dirty"> · modified</span> : null}
        {" · "}
        <span className={runtime === "java" ? "runtime-java" : "runtime-dev"}>
          deploy → {runtime === "java" ? "Java :8080" : "dev runtime :5173"}
        </span>
        {credentials ? ` · ${credentials.user}` : null}
      </span>
    </footer>
  );
}
