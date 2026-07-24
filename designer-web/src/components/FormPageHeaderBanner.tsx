/**
 * Design-canvas stand-in for Project → Page Header… (not a Form Heading item).
 * Shows <<Project Header>> when the project has banner text and/or image.
 * Form Heading / Text items remain below; Preview/Deploy still render the real banner.
 */

import { pageHeaderHasContent, pageHeaderImage } from "@/lib/pageHeader";
import { openPageHeaderDialog } from "@/lib/pageHeaderDialog";
import { dataUrlForImage } from "@/lib/projectImages";
import type { TawalaProject } from "@/types/tawala";

interface Props {
  project: TawalaProject;
}

export function FormPageHeaderBanner({ project }: Props) {
  const header = project.pageHeader;
  if (!pageHeaderHasContent(header)) return null;

  const img = pageHeaderImage(project);
  const previewUrl = img ? dataUrlForImage(img) : null;
  const tip = [
    header?.text?.trim() ? `Text: ${header.text.trim()}` : null,
    img ? "Image set" : null,
    "Click to edit Project → Page Header…",
  ]
    .filter(Boolean)
    .join(" · ");

  return (
    <div
      className="form-page-header-banner"
      role="button"
      tabIndex={0}
      title={tip}
      onClick={(e) => {
        e.stopPropagation();
        openPageHeaderDialog();
      }}
      onKeyDown={(e) => {
        if (e.key === "Enter" || e.key === " ") {
          e.preventDefault();
          openPageHeaderDialog();
        }
      }}
    >
      {previewUrl ? (
        <div
          className="form-page-header-banner-thumb"
          style={{ backgroundImage: `url(${previewUrl})` }}
          aria-hidden
        />
      ) : null}
      <span className="form-page-header-banner-chip">{`<<Project Header>>`}</span>
      {header?.text?.trim() ? (
        <span className="form-page-header-banner-text">{header.text.trim()}</span>
      ) : null}
    </div>
  );
}
