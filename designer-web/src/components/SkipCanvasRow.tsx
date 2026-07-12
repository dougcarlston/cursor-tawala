import { useState } from "react";
import type { SkipInstructionsItem } from "@/types/tawala";
import { useProjectStore } from "@/store/projectStore";
import { skipCanvasSummary } from "@/lib/skipSummary";
import { skipDialogSessionKey } from "@/lib/skipDialogSession";
import { SkipInstructionsDialog } from "./SkipInstructionsDialog";

interface Props {
  item: SkipInstructionsItem;
  index: number;
  formName: string;
  selected: boolean;
}

/**
 * Skip instructions — fixed SKIP badge, peach body, Edit link + summary (legacy SkipItemView).
 */
export function SkipCanvasRow({ item, index, formName, selected }: Props) {
  const project = useProjectStore((s) => s.project);
  const setSelectedItemIndex = useProjectStore((s) => s.setSelectedItemIndex);
  const updateFormItem = useProjectStore((s) => s.updateFormItem);
  const [dialogOpen, setDialogOpen] = useState(false);

  const form = project.forms.find((f) => f.name === formName);
  const summary = skipCanvasSummary(item.commands);
  const sessionKey = skipDialogSessionKey(formName, index);

  const saveCommands = (commands: SkipInstructionsItem["commands"]) => {
    updateFormItem(formName, index, { ...item, commands });
  };

  return (
    <>
      <div
        className={`skip-canvas-row${selected ? " selected" : ""}`}
        onClick={(e) => {
          e.stopPropagation();
          setSelectedItemIndex(index);
        }}
      >
        <div className="skip-badge" title="Skip instructions">
          SKIP
        </div>
        <div className="skip-main">
          <button
            type="button"
            className="skip-edit-link"
            onClick={(e) => {
              e.stopPropagation();
              setSelectedItemIndex(index);
              setDialogOpen(true);
            }}
          >
            Edit
          </button>
          <span className="skip-summary">{summary}</span>
        </div>
      </div>
      {dialogOpen && form ? (
        <SkipInstructionsDialog
          sessionKey={sessionKey}
          projectName={project.name}
          project={project}
          form={form}
          commands={Array.isArray(item.commands) ? item.commands : []}
          onSave={(commands) => {
            saveCommands(commands);
            setDialogOpen(false);
          }}
        />
      ) : null}
    </>
  );
}
