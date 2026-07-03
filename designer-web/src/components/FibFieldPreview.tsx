import { FibItem } from "@/types/tawala";

function blankDisplayLabel(blank: { name: string; displayLabel?: string }): string {
  return blank.displayLabel?.trim() || blank.name;
}

/** Deploy-like FIB preview — real inputs sized by blank.length (HTML size units). */
export function FibFieldPreview({ item }: { item: FibItem }) {
  const blanks = item.blanks ?? [];
  const intro =
    typeof item.prompt === "string" && item.prompt.trim() && !item.prompt.includes("//")
      ? item.prompt.trim()
      : "";

  if (item.style === "topLabels") {
    return (
      <div className="fib-preview fib-preview-toplabels">
        {intro ? <p className="fib-preview-intro">{intro}</p> : null}
        {blanks.map((blank) => (
          <div key={blank.name} className="fib-preview-toplabel-row">
            <label className="fib-preview-toplabel-text">{blankDisplayLabel(blank)}</label>
            <input
              type="text"
              className="fib-preview-input"
              size={Math.min(Math.max(blank.length ?? 20, 5), 120)}
              readOnly
              tabIndex={-1}
              aria-label={blankDisplayLabel(blank)}
            />
          </div>
        ))}
      </div>
    );
  }

  const prompt = typeof item.prompt === "string" ? item.prompt : "";
  return (
    <div className="fib-preview">
      {prompt ? <p className="fib-preview-intro">{prompt}</p> : null}
      <div className="fib-preview-inline">
        {blanks.map((blank) => (
          <input
            key={blank.name}
            type="text"
            className="fib-preview-input"
            size={Math.min(Math.max(blank.length ?? 20, 5), 120)}
            readOnly
            tabIndex={-1}
            title={blank.name}
          />
        ))}
      </div>
    </div>
  );
}
