import { useEffect } from "react";
import {
  SignedIn,
  SignedOut,
  SignInButton,
  SignUpButton,
  UserButton,
  useUser,
} from "@clerk/clerk-react";
import { setGlobalAuthUser } from "@/lib/clerkAuth";

export function UserAccountButton() {
  const isClerkConfigured = Boolean(import.meta.env.VITE_CLERK_PUBLISHABLE_KEY);
  const { user, isLoaded } = useUser();

  useEffect(() => {
    if (!isLoaded) return;
    if (user) {
      setGlobalAuthUser({
        id: user.id,
        fullName: user.fullName || user.firstName || null,
        primaryEmail: user.primaryEmailAddress?.emailAddress || null,
        username: user.username || null,
      });
    } else {
      setGlobalAuthUser(null);
    }
  }, [user, isLoaded]);

  if (!isClerkConfigured) {
    return null;
  }


  return (
    <div className="designer-auth-bar">
      <SignedIn>
        <UserGreeting />
        <UserButton
          afterSignOutUrl="/"
          appearance={{
            elements: {
              avatarBox: "designer-user-avatar",
            },
          }}
        />
      </SignedIn>
      <SignedOut>
        <SignInButton mode="modal">
          <button type="button" className="designer-auth-btn sign-in">
            Sign In
          </button>
        </SignInButton>
        <SignUpButton mode="modal">
          <button type="button" className="designer-auth-btn sign-up">
            Sign Up
          </button>
        </SignUpButton>
      </SignedOut>
    </div>
  );
}

function UserGreeting() {
  const { user } = useUser();
  const displayName =
    user?.firstName ||
    user?.username ||
    user?.primaryEmailAddress?.emailAddress?.split("@")[0] ||
    "Author";

  return (
    <span className="designer-auth-greeting" title={`Signed in as ${user?.primaryEmailAddress?.emailAddress ?? displayName}`}>
      {displayName}
    </span>
  );
}
