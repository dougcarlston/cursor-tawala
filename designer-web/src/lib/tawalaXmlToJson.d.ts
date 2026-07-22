declare module "@/lib/tawalaXmlToJson.mjs" {
  export interface ConvertTawalaOptions {
    /** Stored on project as `_convertedFrom` when set. */
    sourceLabel?: string;
  }

  export interface ConvertTawalaResult {
    project: Record<string, unknown>;
    warnings: string[];
  }

  export function convertTawalaXmlToProject(
    xmlString: string,
    options?: ConvertTawalaOptions,
  ): ConvertTawalaResult;

  export function countProcessCommands(cmds: unknown[] | undefined): number;

  export function isTawalaProjectFileName(filename: string): boolean;
}
