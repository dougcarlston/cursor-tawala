declare module "../../server/jsonToXml.mjs" {
  export function buildUploadRequest(
    credentials: { user: string; password: string },
    project: unknown,
  ): string;
}
