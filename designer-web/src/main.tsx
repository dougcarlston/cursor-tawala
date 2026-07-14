import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import App from "./App";
import { ErrorBoundary } from "./components/ErrorBoundary";
import { installDesignerShellGuards } from "./lib/shellCommands";
import "./styles.css";

// Install before React mount so Cmd/Ctrl+S + dirty leave-warning survive App HMR.
installDesignerShellGuards();

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <ErrorBoundary>
      <App />
    </ErrorBoundary>
  </StrictMode>,
);
