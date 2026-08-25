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
    /* No session and not explicitly guest → treat as logged-in for mock UX. */
    return true;
  }

  function currentUser() {
    if (isGuestMode()) return "";
    const s = readSession();
    if (s && s.user) return String(s.user);
    return "dev";
  }

  function setSession(user) {
    const name = String(user || "dev").trim() || "dev";
    try {
      localStorage.removeItem(GUEST_KEY);
      localStorage.setItem(
        SESSION_KEY,
        JSON.stringify({ user: name, at: new Date().toISOString() })
      );
    } catch {
      /* ignore */
    }
    return name;
  }

  function clearSession() {
    try {
      localStorage.removeItem(SESSION_KEY);
      localStorage.setItem(GUEST_KEY, "1");
    } catch {
      /* ignore */
    }
  }

  /** Guest taste My Tawala; logged-in users get the real private pile. */
  function myTawalaHref() {
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
      item.target === "_blank" ? ' target="_blank" rel="noopener"' : "";
    return `<a href="${resolveHref(key)}"${target}>${text}</a>`;
  }

  function renderNav(activePage) {
    return NAV_ORDER.map((key) => {
      const item = LINKS[key];
      const selected =
        key === activePage ||
        (key === "mytawala" &&
          (activePage === "mytawala" || activePage === "mytawala-demo"));
      if (!item.ready) {
        return `<li class="${selected ? "selected" : ""}"><span class="link-pending" aria-disabled="true">${item.label}</span></li>`;
      }
      const target =
        item.target === "_blank" ? ' target="_blank" rel="noopener"' : "";
      return `<li class="${selected ? "selected" : ""}"><a href="${resolveHref(key)}"${target}${selected ? ' aria-current="page"' : ""}>${item.label}</a></li>`;
    }).join("\n          ");
  }

  function renderAccountMenu(user) {
    const items = user
      ? [
          `<li>${anchor("changePassword")}</li>`,
          `<li>${anchor("logout")}</li>`,
        ]
      : [
          `<li>${anchor("register")}</li>`,
          `<li>${anchor("login")}</li>`,
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
    return (
      `Welcome. Please ` +
      `<a href="signup.html">register</a> or ` +
      `<a href="login.html?next=mytawala.html">Log in</a> ` +
      renderAccountMenu("")
    );
  }

  function renderHeader(activePage, user) {
    const status = user
      ? `Welcome back, <span class="userName">${user}</span>. ${renderAccountMenu(user)}`
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

  function mount() {
    const body = document.body;
    const activePage = body.dataset.tawalaPage || "home";
    /* Session wins. data-tawala-user is display-only when logged in (legacy pages). */
    const sessionUser = currentUser();
    const user = sessionUser || "";
    const showBanner = body.dataset.tawalaBanner !== "false";

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
  }

  window.TawalaChrome = {
    mount,
    anchor,
    LINKS,
    SESSION_KEY,
    GUEST_KEY,
    isLoggedIn,
    isGuestMode,
    currentUser,
    setSession,
    clearSession,
    myTawalaHref,
    readSession,
  };
})();
