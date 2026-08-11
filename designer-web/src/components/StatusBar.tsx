import { useEffect, useMemo, useState } from "react";
import { useProjectStore } from "@/store/projectStore";
import { summarizePreservedGaps } from "@/lib/preservedImportGaps";

export function StatusBar() {
  const statusMessage = useProjectStore((s) => s.statusMessage);
  const dirty = useProjectStore((s) => s.dirty);
  const project = useProjectStore((s) => s.project);
  const credentials = useProjectStore((s) => s.credentials);
  const [runtime, setRuntime] = useState<"dev" | "java" | "…">("…");
  const [flashKey, setFlashKey] = useState(0);
  const gapSummary = useMemo(() => summarizePreservedGaps(project), [project]);

  useEffect(() => {
    fetch("/api/health")
      .then((r) => r.json())
      .then((d) => setRuntime(d.runtime === "java" ? "java" : "dev"))
      .catch(() => setRuntime("dev"));
  }, []);

  useEffect(() => {
    setFlashKey((k) => k + 1);
  }, [statusMessage]);

  const gapTip =
    gapSummary.totalMarkers > 0
      ? [
          `${gapSummary.columnDisplayConditions} column displayCondition(s)`,
          `${gapSummary.formsAffected} form(s), ${gapSummary.documentsAffected} document(s)`,
          "Amber cues = column visibility still uneditable in Configure Function",
        ].join(" · ")
      : undefined;

  return (
    <footer className="status-bar">
      <span key={flashKey} className="status-bar-message" title={statusMessage}>
        {statusMessage}
      </span>
      <span className="status-bar-meta">
        {project.name} · format {project.format}
        {dirty ? <span className="dirty"> · modified</span> : null}
        {gapSummary.totalMarkers > 0 ? (
          <span className="status-preserved-gaps" title={gapTip}>
            {` · ${gapSummary.totalMarkers} preserved gap${gapSummary.totalMarkers === 1 ? "" : "s"}`}
          </span>
        ) : null}
        {" · "}
        <span className={runtime === "java" ? "runtime-java" : "runtime-dev"}>
          push → {runtime === "java" ? "Java :8080" : "dev runtime :5173"}
        </span>
        {credentials ? ` · ${credentials.user}` : null}
      </span>
    </footer>
  );
}
