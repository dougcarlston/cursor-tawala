/**
 * Shared header/footer for website-mock.
 * Links with ready: false get class link-pending (greyed, non-clickable).
 *
 * Mock session (not real Auth Task #21): localStorage `tawala.mock.session`
 * `{ user, at }`. Login sets it; Logout clears it. Drives Welcome chrome,
 * Library “Copy to MyTawala” visibility, and guest vs real My Tawala routing.
 */
(function () {
  const SESSION_KEY = "tawala.mock.session";
  /** Explicit guest/browse mode — set by Logout. Cleared by Login. */
  const GUEST_KEY = "tawala.mock.guest";
  const CLERK_PUBLISHABLE_KEY = "pk_live_Y2xlcmsudGVhY2hlcmV4YW1zLm9yZyQ";
  const CLERK_JS_URL = "https://clerk.teacherexams.org/npm/@clerk/clerk-js@4.73.14/dist/clerk.browser.js";

  let clerkInstance = null;
  let clerkLoaded = false;
  let pendingClerkAction = "";
  let clerkLoadPromise = null;
  let publicSignOutStarted = false;
  let publicBindStarted = false;

  /**
   * Clerk 4's isReady is a function. A truthy check skips load(), then openSignIn throws.
   * signOut() also schedules closeSignIn(); open the window only after that, with no session left.
   * On this dev instance a leftover session makes openSignIn a silent no-op.
   */
  function clerkIsLoaded() {
    const clerk = window.Clerk;
    if (!clerk) return false;
    if (typeof clerk.isReady === "function") return clerk.isReady() === true;
    return clerk.loaded === true;
  }

  /** Home still loads scripts/json.js, which puts enumerable methods on Object.prototype. Clerk's cookie writer then calls .split on those methods and Login does nothing. */
  function hideLegacyPrototypeMethods() {
    ["toJSONString", "parseJSON"].forEach((name) => {
      [Object, String, Array, Date, Boolean, Number].forEach((ctor) => {
        try {
          const desc = Object.getOwnPropertyDescriptor(ctor.prototype, name);
          if (desc && desc.enumerable) {
            Object.defineProperty(ctor.prototype, name, { enumerable: false });
          }
        } catch {
          /* ignore */
        }
      });
    });
    try {
      const method = Object.getOwnPropertyDescriptor(Function.prototype, "method");
      if (method && method.enumerable) {
        Object.defineProperty(Function.prototype, "method", { enumerable: false });
      }
    } catch {
      /* ignore */
    }
  }

  hideLegacyPrototypeMethods();

  function ensureClerk() {
    if (typeof window === "undefined" || !CLERK_PUBLISHABLE_KEY) {
      return Promise.reject(new Error("Clerk key missing"));
    }
    if (clerkLoadPromise) return clerkLoadPromise;
    clerkLoadPromise = (async () => {
      hideLegacyPrototypeMethods();
      window.__clerk_publishable_key = CLERK_PUBLISHABLE_KEY;
      let script = document.querySelector('script[src*="clerk.browser.js"]');
      if (!script) {
        script = document.createElement("script");
        script.setAttribute("data-clerk-publishable-key", CLERK_PUBLISHABLE_KEY);
        script.async = true;
        script.crossOrigin = "anonymous";
        script.src = CLERK_JS_URL;
        document.head.appendChild(script);
      }
      if (!window.Clerk) {
        await new Promise((resolve, reject) => {
          const finish = () => {
            script.setAttribute("data-tawala-clerk-loaded", "1");
            if (window.Clerk) resolve();
            else reject(new Error("Clerk missing"));
          };
          if (script.getAttribute("data-tawala-clerk-loaded") === "1" || window.Clerk) {
            finish();
            return;
          }
          script.addEventListener("load", finish);
          script.addEventListener("error", () => reject(new Error("Clerk script failed")));
        });
      }
      hideLegacyPrototypeMethods();
      if (!clerkIsLoaded()) {
        await window.Clerk.load({
          navigate: (to) => {
            window.location.assign(to);
          },
          afterSignInUrl: absoluteReturnUrl(),
          afterSignUpUrl: absoluteReturnUrl(),
        });
      }
      clerkInstance = window.Clerk;
      clerkLoaded = true;
      return window.Clerk;
    })().catch((err) => {
      clerkLoadPromise = null;
      throw err;
    });
    return clerkLoadPromise;
  }

  function initClerk() {
    ensureClerk()
      .then(() => {
        onClerkReady();
      })
      .catch((err) => {
        console.warn("[TawalaChrome] Clerk load warning:", err);
      });
  }

  function clerkReturnPath() {
    try {
      const r = new URLSearchParams(location.search).get("r") || "";
      if (r.startsWith("/") && !r.startsWith("//") && r.indexOf("://") < 0 && r.indexOf("\\") < 0) {
        return r;
      }
    } catch {
      /* ignore */
    }
    return "";
  }

  function authHold() {
    try {
      return sessionStorage.getItem("tawala.auth.hold") === "1";
    } catch {
      return false;
    }
  }

  function authAwaiting() {
    try {
      return sessionStorage.getItem("tawala.auth.await") === "1";
    } catch {
      return false;
    }
  }

  function holdClerkAttach() {
    try {
      sessionStorage.setItem("tawala.auth.hold", "1");
      sessionStorage.removeItem("tawala.auth.await");
      sessionStorage.removeItem("tawala.clerk.bind");
      sessionStorage.removeItem("tawala.clerk.bind.attempt");
      sessionStorage.removeItem("tawala.clerk.bind.retry");
      sessionStorage.removeItem("tawala.auth.cleared");
    } catch {
      /* ignore */
    }
  }

  function clearKeepSignedInCookie() {
    document.cookie = "x=; Path=/; Max-Age=0";
  }

  function clerkSignedOut() {
    try {
      return sessionStorage.getItem("tawala.clerk.signedOut") === "1";
    } catch {
      return false;
    }
  }

  function markClerkSignedOut() {
    try {
      sessionStorage.setItem("tawala.clerk.signedOut", "1");
    } catch {
      /* ignore */
    }
  }

  function clearClerkSignedOut() {
    try {
      sessionStorage.removeItem("tawala.clerk.signedOut");
    } catch {
      /* ignore */
    }
  }

  let clerkListenerAttached = false;

  let publicBindInFlight = false;

  function showClerkHandoff(text) {
    try {
      sessionStorage.setItem("tawala.clerk.handoff", text);
    } catch {
      /* ignore */
    }
    let note = document.getElementById("tawala-clerk-handoff");
    if (!note) {
      note = document.createElement("p");
      note.id = "tawala-clerk-handoff";
      note.style.cssText = "margin:8px 16px;color:#8a1f11;font:16px Georgia,serif;position:relative;z-index:100000;";
      if (document.body) document.body.insertBefore(note, document.body.firstChild);
    }
    note.textContent = text;
    const dialog = document.querySelector("[role=dialog]");
    if (!dialog) return;
    let inline = dialog.querySelector("[data-tawala-clerk-note]");
    if (!inline) {
      inline = document.createElement("p");
      inline.setAttribute("data-tawala-clerk-note", "1");
      inline.style.cssText = "margin:12px 24px 0;color:#8a1f11;font:16px Georgia,serif;";
      dialog.insertBefore(inline, dialog.firstChild);
    }
    inline.textContent = text;
  }

  function clerkErrorText(err) {
    if (!err) return "";
    const first = err.errors && err.errors[0];
    const text = first
      ? first.longMessage || first.message || first.code || ""
      : err.message || String(err);
    return String(text).replace(/\s+/g, " ").trim().slice(0, 180);
  }

  async function submitClerkToken(token, clerkUser) {
    const person = clerkUser || {};
    const email =
      person.primaryEmailAddress && person.primaryEmailAddress.emailAddress
        ? person.primaryEmailAddress.emailAddress
        : "";
    let status = 0;
    try {
      const res = await fetch("/user/clerk-session", {
        method: "POST",
        credentials: "same-origin",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          token: token,
          email: email,
          firstName: person.firstName || "",
          lastName: person.lastName || "",
        }),
      });
      status = res.status;
      if (!res.ok) {
        showClerkHandoff("Clerk signed in, and Home refused it (" + status + ").");
        return false;
      }
    } catch {
      showClerkHandoff("Clerk signed in, and Home could not be reached.");
      return false;
    }
    clearClerkHandoff();
    const back = absoluteReturnUrl();
    if (back === location.href) location.reload();
    else location.href = back;
    return true;
  }

  function clearClerkHandoff() {
    try {
      sessionStorage.removeItem("tawala.clerk.handoff");
    } catch {
      /* ignore */
    }
    const note = document.getElementById("tawala-clerk-handoff");
    if (note && note.parentNode) note.parentNode.removeChild(note);
  }

  function restoreClerkHandoff() {
    let text = "";
    try {
      text = sessionStorage.getItem("tawala.clerk.handoff") || "";
    } catch {
      text = "";
    }
    if (text) showClerkHandoff(text);
  }

  async function bindPublicClerkAccount() {
    if (publicBindInFlight || publicSignOutStarted) return;
    if (authHold() && !authAwaiting()) return;
    const clerkUser = window.Clerk && window.Clerk.user;
    if (!clerkUser || !clerkUser.id) return;
    publicBindInFlight = true;
    try {
      let already = "";
      try {
        const res = await fetch("/home", { credentials: "same-origin", cache: "no-store" });
        const html = res.ok ? await res.text() : "";
        const match = String(html).match(/Welcome back,\s*<span class="userName">([^<]*)<\/span>/);
        already = match ? match[1].trim() : "";
      } catch {
        already = "";
      }
      const clerkName = clerkUser.username || "";
      const samePerson = already === clerkUser.id || (clerkName !== "" && already === clerkName);
      if (samePerson) {
        try {
          sessionStorage.setItem("tawala.clerk.bind", clerkUser.id);
        } catch {
          /* ignore */
        }
        clearClerkHandoff();
        return;
      }
      let token = "";
      for (let attempt = 0; attempt < 4 && !token; attempt++) {
        const session = window.Clerk.session;
        try {
          token = session
            ? await Promise.race([
                session.getToken(),
                new Promise((resolve) => setTimeout(() => resolve(""), 5000)),
              ])
            : "";
        } catch {
          token = "";
        }
        if (!token) await new Promise((resolve) => setTimeout(resolve, 400));
      }
      if (!token) {
        showClerkHandoff("Clerk signed in, and Home did not receive the sign-in.");
        return;
      }
      if (authHold() && !authAwaiting()) return;
      const email =
        clerkUser.primaryEmailAddress && clerkUser.primaryEmailAddress.emailAddress
          ? clerkUser.primaryEmailAddress.emailAddress
          : "";
      let status = 0;
      try {
        const res = await fetch("/user/clerk-session", {
          method: "POST",
          credentials: "same-origin",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({
            token: token,
            email: email,
            firstName: clerkUser.firstName || "",
            lastName: clerkUser.lastName || "",
          }),
        });
        status = res.status;
        if (!res.ok) {
          showClerkHandoff("Clerk signed in, and Home refused it (" + status + ").");
          return;
        }
      } catch {
        showClerkHandoff("Clerk signed in, and Home could not be reached.");
        return;
      }
      try {
        sessionStorage.setItem("tawala.clerk.bind", clerkUser.id);
      } catch {
        /* ignore */
      }
      clearClerkHandoff();
      const back = absoluteReturnUrl();
      if (back === location.href) location.reload();
      else location.href = back;
    } finally {
      publicBindInFlight = false;
    }
  }

  function authCleared() {
    try {
      return sessionStorage.getItem("tawala.auth.cleared") === "1";
    } catch {
      return false;
    }
  }

  function clerkSessions() {
    const clerk = window.Clerk;
    const list = clerk && clerk.client && clerk.client.sessions;
    const sessions = [];
    if (!list) return sessions;
    for (let i = 0; i < list.length; i++) {
      const session = list[i];
      if (!session || !session.id) continue;
      const status = session.status || "";
      if (
        status === "expired" ||
        status === "removed" ||
        status === "abandoned" ||
        status === "ended" ||
        status === "revoked" ||
        status === "replaced"
      ) {
        continue;
      }
      sessions.push(session);
    }
    return sessions;
  }

  function clerkActiveSession() {
    const clerk = window.Clerk;
    if (!clerk) return null;
    if (clerk.session && clerk.session.id && clerk.session.status !== "expired") return clerk.session;
    const sessions = clerkSessions();
    for (let i = 0; i < sessions.length; i++) {
      if (sessions[i].status === "active" || sessions[i].status === "pending") return sessions[i];
    }
    return sessions[0] || null;
  }

  let adoptClerkSessionStarted = false;

  async function adoptClerkSession() {
    const clerk = window.Clerk;
    if (clerkSignedOut() || adoptClerkSessionStarted || !clerk || publicSignOutStarted) return false;
    adoptClerkSessionStarted = true;
    let switched = false;
    try {
      if (!clerk.user) {
        const session = clerkActiveSession();
        if (!session || typeof clerk.setActive !== "function") return false;
        switched = true;
        let switchError = "";
        try {
          await Promise.race([
            clerk.setActive({ session: session }),
            new Promise((_, reject) => {
              setTimeout(() => reject(new Error("timeout")), 4000);
            }),
          ]);
        } catch (err) {
          switchError = clerkErrorText(err);
        }
        const deadline = Date.now() + 1500;
        while (!clerk.user && Date.now() < deadline) {
          await new Promise((resolve) => setTimeout(resolve, 50));
        }
        if (!clerk.user) {
          let token = "";
          try {
            if (typeof session.touch === "function") {
              await Promise.race([
                session.touch(),
                new Promise((resolve) => setTimeout(resolve, 4000)),
              ]);
            }
          } catch (err) {
            switchError = clerkErrorText(err) || switchError;
          }
          try {
            token = await Promise.race([
              session.getToken(),
              new Promise((resolve) => setTimeout(() => resolve(""), 5000)),
            ]);
          } catch (err) {
            switchError = clerkErrorText(err) || switchError;
            token = "";
          }
          if (token) return submitClerkToken(token, session.user);
          showClerkHandoff(
            "Clerk has a sign-in, and this page could not switch it on." +
              (switchError ? " " + switchError : "")
          );
          return false;
        }
      }
      if (!clerk.user) return false;
    try {
      if (typeof clerk.closeSignIn === "function") clerk.closeSignIn();
    } catch {
      /* ignore */
    }
    try {
      sessionStorage.removeItem("tawala.auth.hold");
      sessionStorage.setItem("tawala.auth.await", "1");
      sessionStorage.setItem("tawala.auth.cleared", "1");
    } catch {
      /* ignore */
    }
    bindPublicClerkAccount();
    return true;
    } catch {
      if (switched) showClerkHandoff("Clerk has a sign-in, and this page could not switch it on.");
      return false;
    } finally {
      adoptClerkSessionStarted = false;
    }
  }

  function maybeBindAfterPrompt() {
    if (publicSignOutStarted || !isPublicSite()) return;
    if (!window.Clerk || !window.Clerk.user) return;
    try {
      sessionStorage.removeItem("tawala.auth.hold");
      sessionStorage.removeItem("tawala.auth.await");
      sessionStorage.removeItem("tawala.auth.cleared");
      sessionStorage.removeItem("tawala.auth.return");
    } catch {
      /* ignore */
    }
    bindPublicClerkAccount();
  }

  let alreadySignedInHandled = false;

  async function finishAlreadySignedIn() {
    if (alreadySignedInHandled || publicSignOutStarted) return;
    alreadySignedInHandled = true;
    const clerk = window.Clerk;
    if (!clerk) {
      alreadySignedInHandled = false;
      return;
    }
    showClerkHandoff("Clerk already has this sign-in. Finishing it.");
    await adoptClerkSession();
  }

  function onClerkReady() {
    if (!clerkInstance) return;
    if (isPublicSite()) {
      if (pendingClerkAction === "signOut") {
        pendingClerkAction = "";
        finishPublicSignOut();
        return;
      }
      if (!clerkListenerAttached) {
        clerkListenerAttached = true;
        clerkInstance.addListener((emission) => {
          if (!emission || !emission.user || publicSignOutStarted || clerkSignedOut()) return;
          maybeBindAfterPrompt();
        });
      }
      const pending = pendingClerkAction;
      if (pending === "signIn" || pending === "signUp") {
        pendingClerkAction = "";
        continueClerkPrompt(pending);
        return;
      }
      if (clerkSignedOut()) return;
      if (!publicSignOutStarted && (clerkInstance.user || clerkActiveSession())) {
        adoptClerkSession();
        return;
      }
      if (authHold() && !authAwaiting()) return;
      maybeBindAfterPrompt();
      return;
    }
    const user = clerkInstance.user;
    if (user) {
      const displayName =
        user.fullName ||
        user.firstName ||
        user.username ||
        (user.primaryEmailAddress ? user.primaryEmailAddress.emailAddress.split("@")[0] : null) ||
        user.id ||
        "Author";
      const email = user.primaryEmailAddress ? user.primaryEmailAddress.emailAddress : "";
      setSession(displayName, {
        id: user.id,
        email: email,
        fullName: user.fullName || null,
        username: user.username || null,
        authProvider: "clerk",
      });
    }

    clerkInstance.addListener((emission) => {
      if (emission && emission.user) {
        const u = emission.user;
        const displayName =
          u.fullName ||
          u.firstName ||
          u.username ||
          (u.primaryEmailAddress ? u.primaryEmailAddress.emailAddress.split("@")[0] : null) ||
          u.id ||
          "Author";
        setSession(displayName, {
          id: u.id,
          email: u.primaryEmailAddress ? u.primaryEmailAddress.emailAddress : "",
          fullName: u.fullName || null,
          username: u.username || null,
          authProvider: "clerk",
        });
        mountHeaderOnly();
        updateSidebarAdminVisibility();
      } else if (emission && !emission.user && !isGuestMode()) {
        mountHeaderOnly();
        updateSidebarAdminVisibility();
      }
    });

    mountHeaderOnly();
    updateSidebarAdminVisibility();
  }

  function mountHeaderOnly() {
    const headerEl = document.getElementById("tawala-chrome-header");
    if (headerEl) {
      const body = document.body;
      const activePage = (body && body.dataset.tawalaPage) || "home";
      const user = currentUser() || "";
      headerEl.innerHTML = renderHeader(activePage, user);
      bindAccountMenus(headerEl);
    }
  }

  function readSession() {
    try {
      const raw = localStorage.getItem(SESSION_KEY);
      if (!raw) return null;
      const parsed = JSON.parse(raw);
      if (parsed && typeof parsed === "object" && parsed.user) return parsed;
    } catch {
      /* ignore */
    }
    return null;
  }

  /** Public host copy. Does not default every visitor to the dev session. */
  function isPublicSite() {
    return typeof window !== "undefined" && window.TAWALA_PUBLIC_SITE === true;
  }

  function publicAuthHref(path) {
    return path + "?r=" + encodeURIComponent(location.pathname + location.search);
  }

  function isGuestMode() {
    try {
      return localStorage.getItem(GUEST_KEY) === "1";
    } catch {
      return false;
    }
  }

  /**
   * Mock auth: default logged-in as "dev" (matches prior hard-coded My Tawala).
   * Logout → guest mode (Library Save hidden; MY TAWALA → sample preview).
   * Login clears guest mode and stores a session.
   */
  function isLoggedIn() {
    if (isGuestMode()) return false;
    if (readSession()) return true;
    if (isPublicSite()) return false;
    /* No session and not explicitly guest → treat as logged-in for mock UX. */
    return true;
  }

  function currentUser() {
    if (isGuestMode()) return "";
    const s = readSession();
    if (s && s.user) return String(s.user);
    if (isPublicSite()) return "";
    return "dev";
  }

  function currentAuthUser() {
    if (isGuestMode()) return null;
    return readSession();
  }

  /**
   * Check if current user is an authorized Administrator.
   * Admins include:
   * 1. Any Clerk signed-in user with role "admin" or designated admin emails/usernames
   * 2. Local fallback accounts "admin" or "dev"
   * 3. Explicit admin mode flag in localStorage
   */
  function isAdmin() {
    if (!isLoggedIn()) return false;
    const auth = currentAuthUser();
    if (!auth) return false;
    /* Public site: only the account's Administrator flag, seen as the ADMIN link. */
    if (isPublicSite()) return auth.isAdmin === true;
    const user = String(auth.user || "").trim().toLowerCase();
    const email = String(auth.email || "").trim().toLowerCase();
    const username = String(auth.username || "").trim().toLowerCase();

    // Default development & admin aliases
    if (user === "admin" || user === "dev") return true;

    // Clerk users or explicitly flagged admin
    if (auth.isAdmin === true || auth.role === "admin") return true;
    if (email && (email.includes("admin") || email.includes("tawala") || email.endsWith("@tawala.com"))) {
      return true;
    }
    // Check localStorage admin override if present
    try {
      if (localStorage.getItem("tawala.mock.libraryAdmin") === "true") return true;
    } catch {
      /* ignore */
    }

    // Any Clerk-authenticated author in the current workspace
    if (auth.authProvider === "clerk") {
      return true;
    }

    return false;
  }

  function setSession(user, extra) {
    const name = String(user || "dev").trim() || "dev";
    try {
      localStorage.removeItem(GUEST_KEY);
      const data = {
        user: name,
        at: new Date().toISOString(),
        ...(extra && typeof extra === "object" ? extra : {}),
      };
      localStorage.setItem(SESSION_KEY, JSON.stringify(data));
    } catch {
      /* ignore */
    }
    return name;
  }

  async function clearSession() {
    try {
      localStorage.removeItem(SESSION_KEY);
      localStorage.setItem(GUEST_KEY, "1");
      if (window.Clerk && typeof window.Clerk.signOut === "function") {
        await window.Clerk.signOut();
      }
    } catch {
      /* ignore */
    }
  }

  async function clearClerkSession() {
    const clerk = window.Clerk;
    if (!clerk) return;
    const client = clerk.client;
    try {
      if (client && typeof client.destroy === "function") {
        await Promise.race([
          client.destroy(),
          new Promise((resolve) => setTimeout(resolve, 4000)),
        ]);
      }
    } catch {
      /* ignore */
    }
  }

  let promptSerial = 0;
  let clerkDialogSawFields = false;

  async function continueClerkPrompt(kind) {
    clearClerkSignedOut();
    publicBindStarted = false;
    try {
      await clearClerkSession();
    } catch {
      /* ignore */
    }
    try {
      sessionStorage.setItem("tawala.auth.hold", "1");
      sessionStorage.setItem("tawala.auth.await", "1");
      sessionStorage.setItem("tawala.auth.cleared", "1");
      sessionStorage.setItem("tawala.auth.return", absoluteReturnUrl());
    } catch {
      /* ignore */
    }
    try {
      const signIn = window.Clerk && window.Clerk.client && window.Clerk.client.signIn;
      if (
        signIn &&
        signIn.status &&
        signIn.status !== "needs_identifier" &&
        typeof signIn.create === "function"
      ) {
        await Promise.race([
          signIn.create({}),
          new Promise((resolve) => setTimeout(resolve, 1500)),
        ]);
      }
    } catch {
      /* ignore */
    }
    try {
      const options = clerkOpenOptions();
      if (kind === "signUp" && window.Clerk && typeof window.Clerk.openSignUp === "function") {
        window.Clerk.openSignUp(options);
      } else if (window.Clerk && typeof window.Clerk.openSignIn === "function") {
        window.Clerk.openSignIn(options);
      }
    } catch (err) {
      console.warn("[TawalaChrome] Clerk window warning:", err);
    }
  }

  function clerkSurfaceText() {
    const bits = [];
    const visit = (node) => {
      if (!node) return;
      if (node.nodeType === 1) {
        if (node.shadowRoot) visit(node.shadowRoot);
        if (node.getAttribute && node.getAttribute("role") === "dialog") {
          bits.push(node.innerText || "");
        }
      }
      const kids = node.children;
      for (let i = 0; kids && i < kids.length; i++) visit(kids[i]);
    };
    if (document.body) visit(document.body);
    return bits.join("\n");
  }

  function watchClerkAnswer() {
    if (watchClerkAnswer.started) return;
    watchClerkAnswer.started = true;
    let stuckSince = 0;
    setInterval(() => {
      if (window.Clerk && window.Clerk.user) {
        adoptClerkSession();
        return;
      }
      const dialog = document.querySelector("[role=dialog]");
      const surface = clerkSurfaceText();
      const pageText = surface || (document.body ? document.body.innerText : "");
      if (/already signed in/i.test(pageText)) {
        finishAlreadySignedIn();
        return;
      }
      if (!dialog) {
        stuckSince = 0;
        return;
      }
      const buttons = dialog.querySelectorAll("button");
      let spinning = false;
      for (let i = 0; i < buttons.length; i++) {
        const button = buttons[i];
        if (!button.disabled) continue;
        const label = (button.getAttribute("aria-label") || button.textContent || "").replace(/\s+/g, " ").trim();
        if (/continue/i.test(label)) spinning = true;
      }
      const blank = !dialog.querySelector("input, textarea") && (dialog.innerText || "").replace(/\s+/g, " ").trim().length < 40;
      if (!spinning && !blank) {
        stuckSince = 0;
        const old = dialog.querySelector("[data-tawala-clerk-note]");
        if (old) old.remove();
        return;
      }
      if (!stuckSince) stuckSince = Date.now();
      if (Date.now() - stuckSince < 8000) return;
      const signIn = window.Clerk && window.Clerk.client && window.Clerk.client.signIn;
      const verification = signIn && signIn.firstFactorVerification;
      const clerkError = verification && verification.error;
      const message = (clerkError && (clerkError.longMessage || clerkError.message))
        || "Clerk did not answer. Close this window. Do not click Continue again.";
      let note = dialog.querySelector("[data-tawala-clerk-note]");
      if (!note) {
        note = document.createElement("p");
        note.setAttribute("data-tawala-clerk-note", "1");
        note.style.margin = "12px 24px 0";
        note.style.color = "#9b1c1c";
        note.style.font = "15px Georgia, serif";
        dialog.appendChild(note);
      }
      note.textContent = message;
    }, 500);
  }

  function rememberClerkFields() {
    if (rememberClerkFields.started) return;
    rememberClerkFields.started = true;
    setInterval(() => {
      const dialog = document.querySelector("[role=dialog]");
      if (dialog && dialog.querySelector("input, textarea")) clerkDialogSawFields = true;
    }, 400);
  }

  function leaveBlankSignInWindow(serial) {
    setTimeout(() => {
      if (serial !== promptSerial) return;
      const dialog = document.querySelector("[role=dialog]");
      if (dialog && dialog.querySelector("input, textarea")) {
        clerkDialogSawFields = true;
        return;
      }
      if (clerkDialogSawFields) return;
      try {
        if (window.Clerk && typeof window.Clerk.closeSignIn === "function") window.Clerk.closeSignIn();
      } catch {
        /* ignore */
      }
      if (location.pathname === "/login") return;
      const back = (location.pathname || "/home") + (location.search || "");
      location.href = "/login?r=" + encodeURIComponent(back);
    }, 8000);
  }

  async function finishPublicSignOut() {
    if (publicSignOutStarted) return;
    publicSignOutStarted = true;
    pendingClerkAction = "";
    markClerkSignedOut();
    holdClerkAttach();
    clearKeepSignedInCookie();
    try {
      localStorage.removeItem(SESSION_KEY);
      localStorage.setItem(GUEST_KEY, "1");
    } catch {
      /* ignore */
    }
    try {
      await Promise.race([
        ensureClerk(),
        new Promise((resolve) => setTimeout(resolve, 2500)),
      ]);
    } catch {
      /* ignore */
    }
    try {
      await clearClerkSession();
    } catch {
      /* ignore */
    }
    try {
      await fetch("/logout", { credentials: "same-origin", redirect: "follow", cache: "no-store" });
    } catch {
      /* ignore */
    }
    location.href = "/";
  }

  function signOutAndLeave() {
    pendingClerkAction = "signOut";
    initClerk();
    setTimeout(() => {
      if (pendingClerkAction === "signOut") finishPublicSignOut();
    }, 3000);
  }

  function liveReturnPath() {
    const requested = clerkReturnPath();
    if (requested && requested !== "/") return requested;
    const path = location.pathname || "/";
    if (
      path.endsWith("/login.html") ||
      path.endsWith("/signup.html")
    ) {
      return "/home";
    }
    if (path === "/") {
      return "/" + (location.search || "");
    }
    return path + (location.search || "");
  }

  function absoluteReturnUrl() {
    const path = liveReturnPath() || "/home";
    try {
      return new URL(path, location.origin).href;
    } catch {
      return location.origin + "/home";
    }
  }

  function clerkOpenOptions() {
    const target = absoluteReturnUrl();
    return {
      afterSignInUrl: target,
      afterSignUpUrl: target,
    };
  }

  function mockAuthPage(kind) {
    const onApp = typeof location !== "undefined" && location.pathname.indexOf("/app/") === 0;
    if (kind === "signUp") return onApp ? "/app/signup.html" : "signup.html";
    return onApp ? "/app/login.html" : "login.html";
  }

  function openSignIn() {
    if (!isPublicSite()) {
      if (window.Clerk && typeof window.Clerk.openSignIn === "function") window.Clerk.openSignIn();
      else location.href = mockAuthPage("signIn");
      return;
    }
    holdClerkAttach();
    pendingClerkAction = "signIn";
    initClerk();
  }

  function openSignUp() {
    if (!isPublicSite()) {
      if (window.Clerk && typeof window.Clerk.openSignUp === "function") window.Clerk.openSignUp();
      else location.href = mockAuthPage("signUp");
      return;
    }
    holdClerkAttach();
    pendingClerkAction = "signUp";
    initClerk();
  }

  function openUserProfile() {
    if (window.Clerk && typeof window.Clerk.openUserProfile === "function") {
      window.Clerk.openUserProfile();
    }
  }

  /** Guest taste My Tawala; logged-in users get the real private pile. */
  function myTawalaHref() {
    if (isPublicSite()) return "/app/mytawala.html?v=20261120-theme1";
    return isLoggedIn() ? "mytawala.html" : "mytawala-demo.html";
  }

  const LINKS = {
    about: { href: "about.html", label: "ABOUT", nav: true, ready: true },
    faq: { href: "faq.html", label: "FAQ", nav: true, ready: true },
    designer: {
      href: "http://localhost:5173",
      label: "DESIGNER",
      nav: true,
      ready: true,
      target: "_blank",
    },
    library: { href: "library.html", label: "LIBRARY", nav: true, ready: true },
    mytawala: { href: "mytawala.html", label: "MY TAWALA", nav: true, ready: true },
    home: { href: "index.html", label: "HOME", nav: true, ready: true },
    login: { href: "login.html", label: "Log in", ready: true },
    register: { href: "signup.html", label: "Register", ready: true },
    signup: {
      href: "signup.html",
      label: "Sign up for FREE account here!",
      ready: true,
    },
    signupShort: { href: "signup.html", label: "Sign up free", ready: true },
    logout: { href: "logout.html", label: "Logout", ready: true },
    changePassword: {
      href: "#",
      label: "Change Password",
      ready: false,
      title: "Change your password (not wired in mock)",
    },
    designerStub: {
      href: "designer.html",
      label: "Designer stub",
      ready: true,
    },
    websiteMock: {
      href: "http://localhost:5500/",
      label: "Website mock",
      ready: true,
    },
    signupApps: {
      href: "signup.html",
      label: "Create your own Tawala apps",
      ready: true,
    },
    companyInfo: { href: "about.html", label: "Company Info", ready: true },
    terms: { href: "terms.html", label: "Terms &amp; Conditions", ready: true },
    privacy: { href: "privacy.html", label: "Privacy Policy", ready: true },
    docs: { href: "docs.html", label: "Docs", ready: true },
  };

  /** LTR: Home · Library · My Tawala · Designer · FAQ · About */
  const NAV_ORDER = ["home", "library", "mytawala", "designer", "faq", "about"];

  function resolveHref(key) {
    if (key === "mytawala") return myTawalaHref();
    if (isPublicSite() && key === "home") return "/home";
    if (isPublicSite() && key === "library") return "/app/library.html?v=20261120-theme1";
    if (isPublicSite() && key === "designer") return "/app/designer/";
    const item = LINKS[key];
    return item ? item.href : "#";
  }

  function anchor(key, labelOverride) {
    const item = LINKS[key];
    if (!item) return "";
    const text = labelOverride || item.label;
    if (!item.ready) {
      const title = item.title ? ` title="${item.title}"` : ' title="Unavailable"';
      return `<span class="link-pending" aria-disabled="true"${title}>${text}</span>`;
    }
    const target =
      item.target === "_blank" && !(isPublicSite() && key === "designer")
        ? ' target="_blank" rel="noopener"'
        : "";
    return `<a href="${resolveHref(key)}"${target}>${text}</a>`;
  }

  function renderNav(activePage) {
    return NAV_ORDER.map((key) => {
      if (isPublicSite() && key === "designer" && !isLoggedIn()) return "";
      const item = LINKS[key];
      const selected =
        key === activePage ||
        (key === "mytawala" &&
          (activePage === "mytawala" || activePage === "mytawala-demo"));
      if (!item.ready) {
        return `<li class="${selected ? "selected" : ""}"><span class="link-pending" aria-disabled="true">${item.label}</span></li>`;
      }
      const target =
        item.target === "_blank" && !(isPublicSite() && key === "designer")
          ? ' target="_blank" rel="noopener"'
          : "";
      return `<li class="${selected ? "selected" : ""}"><a href="${resolveHref(key)}"${target}${selected ? ' aria-current="page"' : ""}>${item.label}</a></li>`;
    }).join("\n          ");
  }

  function renderAccountMenu(user) {
    const isClerk = clerkInstance && clerkInstance.user;
    const auth = currentAuthUser();
    const administratorLink =
      isPublicSite() && auth && auth.administratorPage === true
        ? `<li><a href="/app/administrator.jsp">Administrator</a></li>`
        : "";
    const items = user
      ? [
          administratorLink,
          isClerk
            ? `<li><a href="#" class="clerk-profile-action" title="Manage your Clerk profile &amp; security">Manage Account</a></li>`
            : `<li>${anchor("changePassword")}</li>`,
          `<li><a href="${isPublicSite() ? "/logout" : "logout.html"}" class="clerk-logout-action">Logout</a></li>`,
        ]
      : [
          `<li><a href="${isPublicSite() ? "#" : "signup.html"}" class="clerk-signup-action">Register</a></li>`,
          `<li><a href="${isPublicSite() ? "#" : "login.html?next=mytawala.html"}" class="clerk-login-action">Log in</a></li>`,
        ];
    return (
      `<div class="account-menu">` +
      `<button type="button" class="account-menu-toggle" aria-haspopup="true" aria-expanded="false">` +
      `My Account <span class="account-menu-caret" aria-hidden="true">▾</span>` +
      `</button>` +
      `<ul class="account-menu-panel" hidden>` +
      items.join("") +
      `</ul>` +
      `</div>`
    );
  }

  function renderGuestStatus() {
    if (isPublicSite()) {
      return (
        `Welcome. Please ` +
        `<a href="#" class="clerk-signup-action">register</a> or ` +
        `<a href="#" class="clerk-login-action">Log in</a> ` +
        renderAccountMenu("")
      );
    }
    return (
      `Welcome. Please ` +
      `<a href="signup.html" class="clerk-signup-action">register</a> or ` +
      `<a href="login.html?next=mytawala.html" class="clerk-login-action">Log in</a> ` +
      renderAccountMenu("")
    );
  }

  function renderHeader(activePage, user) {
    const adminBadge = isAdmin()
      ? `<span class="tawala-admin-badge" title="Authenticated Administrator" style="background:#d9534f;color:#fff;font-size:10px;font-weight:bold;padding:2px 6px;border-radius:3px;margin-right:6px;vertical-align:middle;letter-spacing:0.5px;">ADMIN</span>`
      : "";

    const status = user
      ? `${adminBadge}Welcome back, <span class="userName">${user}</span>. ${renderAccountMenu(user)}`
      : renderGuestStatus();

    return `
    <div id="hd">
      <div id="headingLogo"><a href="index.html"><img src="images/template/tawala-logo-white.gif" width="136" height="24" alt="Tawala" /></a></div>
      <div id="headingStatus">${status}</div>
      <div id="headingMenu">
        <ul>
          ${renderNav(activePage)}
        </ul>
      </div>
    </div>`;
  }

  function renderFooter() {
    return `
    <div id="ft" class="footer">
      <ul>
        <li>${anchor("companyInfo")}</li>
        <li>${anchor("terms")}</li>
        <li>${anchor("privacy")}</li>
        <li>${anchor("docs")}</li>
        <li><a href="mailto:info@tawala.com">Contact Us</a></li>
      </ul>
    </div>`;
  }

  function renderBanner(activePage) {
    const mtHref = myTawalaHref();
    let html =
      `<strong>Website mock</strong> — static draft from legacy JSP. Grey controls = not implemented. Test-drive → :8080. ` +
      `<a href="http://localhost:5173" target="_blank" rel="noopener">Web Designer :5173</a> · this site :5500` +
      ` · <a href="docs.html" title="Mock ops docs (Publish, Export/Import, admin…)">Docs</a>.` +
      (activePage === "mytawala"
        ? ` · <b>My Tawala</b> (private) · <a href="library.html">Library</a>`
        : activePage === "mytawala-demo"
          ? ` · <b>My Tawala</b> (guest preview) · <a href="library.html">Library</a> · <a href="signup.html">Register</a>`
          : activePage === "library"
            ? ` · <b>Library</b> (public) · <a href="${mtHref}">My Tawala</a>`
            : activePage === "docs"
              ? ` · <b>Docs</b> · <a href="README.md">README.md</a>`
              : "");
    /* localhost vs 127.0.0.1 = different origins / separate localStorage. Owner data lives on localhost. */
    try {
      if (typeof location !== "undefined" && location.hostname === "127.0.0.1") {
        const path =
          activePage === "library"
            ? "library.html"
            : activePage === "mytawala-demo"
              ? location.pathname && location.pathname.indexOf("mytawala-demo-project") >= 0
                ? "mytawala-demo-project.html" + (location.search || "")
                : "mytawala-demo.html"
              : activePage === "mytawala"
                ? location.pathname && location.pathname.indexOf("mytawala-project") >= 0
                  ? "mytawala-project.html" + (location.search || "")
                  : "mytawala.html"
                : "mytawala.html";
        const href = "http://localhost:5500/" + path.replace(/^\//, "");
        html +=
          `<span class="mock-banner-where">My Tawala / Library data is stored <b>per address</b> — ` +
          `you’re on <code>127.0.0.1</code>. If your projects are missing, open ` +
          `<a href="${href}">http://localhost:5500/…</a> (same server, different localStorage).</span>`;
      }
    } catch {
      /* ignore */
    }
    return html;
  }

  function bindAccountMenus(root) {
    const scope = root || document;

    scope.querySelectorAll(".clerk-profile-action").forEach((el) => {
      el.addEventListener("click", (e) => {
        e.preventDefault();
        openUserProfile();
      });
    });

    scope.querySelectorAll(".clerk-login-action").forEach((el) => {
      el.addEventListener("click", (e) => {
        if (isPublicSite() || window.Clerk) {
          e.preventDefault();
          openSignIn();
        }
      });
    });

    scope.querySelectorAll(".clerk-signup-action").forEach((el) => {
      el.addEventListener("click", (e) => {
        if (isPublicSite() || window.Clerk) {
          e.preventDefault();
          openSignUp();
        }
      });
    });

    scope.querySelectorAll(".clerk-logout-action").forEach((el) => {
      el.addEventListener("click", async (e) => {
        if (isPublicSite()) {
          e.preventDefault();
          try {
            sessionStorage.removeItem("tawala.clerk.bind");
          } catch {
            /* ignore */
          }
          await finishPublicSignOut();
          return;
        }
        if (window.Clerk && window.Clerk.user) {
          e.preventDefault();
          await clearSession();
          location.href = "logout.html";
        }
      });
    });

    scope.querySelectorAll(".account-menu").forEach((menu) => {
      const toggle = menu.querySelector(".account-menu-toggle");
      const panel = menu.querySelector(".account-menu-panel");
      if (!toggle || !panel || toggle.dataset.bound === "1") return;
      toggle.dataset.bound = "1";

      function close() {
        panel.hidden = true;
        toggle.setAttribute("aria-expanded", "false");
        menu.classList.remove("is-open");
      }

      function open() {
        panel.hidden = false;
        toggle.setAttribute("aria-expanded", "true");
        menu.classList.add("is-open");
      }

      toggle.addEventListener("click", (e) => {
        e.preventDefault();
        e.stopPropagation();
        if (panel.hidden) open();
        else close();
      });

      panel.addEventListener("click", (e) => e.stopPropagation());
    });

    if (!document.documentElement.dataset.accountMenuDocBound) {
      document.documentElement.dataset.accountMenuDocBound = "1";
      document.addEventListener("click", () => {
        document.querySelectorAll(".account-menu.is-open").forEach((menu) => {
          const toggle = menu.querySelector(".account-menu-toggle");
          const panel = menu.querySelector(".account-menu-panel");
          if (panel) panel.hidden = true;
          if (toggle) toggle.setAttribute("aria-expanded", "false");
          menu.classList.remove("is-open");
        });
      });
      document.addEventListener("keydown", (e) => {
        if (e.key !== "Escape") return;
        document.querySelectorAll(".account-menu.is-open").forEach((menu) => {
          const toggle = menu.querySelector(".account-menu-toggle");
          const panel = menu.querySelector(".account-menu-panel");
          if (panel) panel.hidden = true;
          if (toggle) toggle.setAttribute("aria-expanded", "false");
          menu.classList.remove("is-open");
        });
      });
    }
  }

  function updateSidebarAdminVisibility() {
    const isUserAdmin = isAdmin();
    const blocks = document.querySelectorAll("#sidebarMaintainerToolsBlock");
    blocks.forEach((el) => {
      el.style.display = isUserAdmin ? "block" : "none";
    });
  }

  /**
   * The public Library page does not share the prototype's browser login.
   * Read the real Tomcat session from /home and show that name.
   */
  function syncPublicAccount() {
    if (!isPublicSite()) return Promise.resolve("");
    return fetch("/home", { credentials: "same-origin", cache: "no-store" })
      .then((res) => (res.ok ? res.text() : ""))
      .then((html) => {
        const match = String(html).match(
          /Welcome back,\s*<span class="userName">([^<]*)<\/span>/
        );
        const name = match ? match[1].trim() : "";
        const admin = /href\s*=\s*["']\/administration\/users["']/.test(html);
        const administratorPage = /href\s*=\s*["']\/app\/administrator\.jsp["']/.test(html);
        try {
          if (name) {
            setSession(name, { authProvider: "java", isAdmin: admin, administratorPage: administratorPage });
          } else {
            localStorage.removeItem(SESSION_KEY);
            localStorage.removeItem(GUEST_KEY);
          }
        } catch {
          /* ignore */
        }
        mount();
        return name;
      })
      .catch(() => "");
  }

  function mount() {
    restoreClerkHandoff();
    initClerk();
    const body = document.body;
    const activePage = body.dataset.tawalaPage || "home";
    /* Session wins. data-tawala-user is display-only when logged in (legacy pages). */
    const sessionUser = currentUser();
    const user = sessionUser || "";
    const showBanner = body.dataset.tawalaBanner !== "false" && !isPublicSite();

    const bannerEl = document.getElementById("tawala-chrome-banner");
    const headerEl = document.getElementById("tawala-chrome-header");
    const footerEl = document.getElementById("tawala-chrome-footer");

    if (bannerEl && showBanner) {
      bannerEl.className = "mock-banner";
      bannerEl.innerHTML = renderBanner(activePage);
    } else if (bannerEl) {
      bannerEl.remove();
    }

    if (headerEl) {
      headerEl.innerHTML = renderHeader(activePage, user);
      bindAccountMenus(headerEl);
    }
    if (footerEl) {
      footerEl.innerHTML = renderFooter();
    }
    updateSidebarAdminVisibility();
  }

  if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", () => {
      if (isPublicSite()) {
        restoreClerkHandoff();
        initClerk();
      }
    });
  } else if (isPublicSite()) {
    restoreClerkHandoff();
    initClerk();
  }

  document.addEventListener("DOMContentLoaded", () => {
    const form = document.getElementById("loginForm");
    if (!form) return;
    form.addEventListener("submit", (event) => {
      if (!event.isTrusted) event.preventDefault();
    });
  });

  window.TawalaChrome = {
    mount,
    anchor,
    LINKS,
    SESSION_KEY,
    GUEST_KEY,
    isLoggedIn,
    isGuestMode,
    isAdmin,
    currentUser,
    currentAuthUser,
    setSession,
    syncPublicAccount,
    clearSession,
    openSignIn,
    openSignUp,
    signOutAndLeave,
    openUserProfile,
    myTawalaHref,
    readSession,
  };
})();
