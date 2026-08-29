import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import { ClerkProvider } from "@clerk/clerk-react";
import App from "./App";
import { ErrorBoundary } from "./components/ErrorBoundary";
import { installDesignerShellGuards } from "./lib/shellCommands";
import "./styles.css";

// Install before React mount so Cmd/Ctrl+S + dirty leave-warning survive App HMR.
installDesignerShellGuards();

const PUBLISHABLE_KEY = import.meta.env.VITE_CLERK_PUBLISHABLE_KEY;

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <ErrorBoundary>
      {PUBLISHABLE_KEY ? (
        <ClerkProvider publishableKey={PUBLISHABLE_KEY} afterSignOutUrl="/">
          <App />
        </ClerkProvider>
      ) : (
        <App />
      )}
    </ErrorBoundary>
  </StrictMode>,
);

