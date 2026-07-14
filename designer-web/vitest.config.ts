import { defineConfig } from "vitest/config";
import path from "node:path";
import { fileURLToPath } from "node:url";

const root = path.dirname(fileURLToPath(import.meta.url));

export default defineConfig({
  test: {
    environment: "node",
    environmentMatchGlobs: [["src/**/*.dom.test.ts", "happy-dom"]],
    include: ["src/**/*.test.ts", "src/**/*.dom.test.ts", "server/**/*.test.mjs"],
  },
  resolve: {
    alias: {
      "@": path.resolve(root, "src"),
    },
  },
});
