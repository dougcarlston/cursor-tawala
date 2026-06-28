import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";
import path from "path";

export default defineConfig({
  plugins: [react()],
  resolve: {
    alias: {
      "@": path.resolve(__dirname, "src"),
    },
  },
  server: {
    fs: {
      allow: [".."],
    },
    proxy: {
      "/health": { target: "http://localhost:3001/api/health", changeOrigin: true, rewrite: () => "/api/health" },
      "/client": { target: "http://localhost:3001", changeOrigin: true },
      "/api": { target: "http://localhost:3001", changeOrigin: true },
      "/p": { target: "http://localhost:3001", changeOrigin: true },
      "/preview": { target: "http://localhost:3001", changeOrigin: true },
    },
  },
});
