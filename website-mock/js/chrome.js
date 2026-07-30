/**
 * Shared header/footer for website-mock.
 * Links with ready: false get class link-pending (greyed, non-clickable).
 */
(function () {
  const LINKS = {
    about: { href: "about.html", label: "ABOUT", nav: true, ready: true },
    faq: { href: "faq.html", label: "FAQ", nav: true, ready: true },
    library: { href: "library.html", label: "LIBRARY", nav: true, ready: true },
    mytawala: { href: "mytawala.html", label: "MY TAWALA", nav: true, ready: true },
    home: { href: "index.html", label: "HOME", nav: true, ready: true },
    login: { href: "login.html", label: "Login", ready: true },
    signup: {
      href: "signup.html",
      label: "Sign up for FREE account here!",
      ready: true,
    },
    signupShort: { href: "signup.html", label: "Sign up free", ready: true },
    logout: { href: "logout.html", label: "Logout", ready: true },
    designer: {
      href: "http://localhost:5173",
      label: "Open Web Designer",
      ready: true,
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
  };

  const NAV_ORDER = ["about", "faq", "library", "mytawala", "home"];

  function anchor(key, labelOverride) {
    const item = LINKS[key];
    if (!item) return "";
    const text = labelOverride || item.label;
    if (!item.ready) {
      return `<span class="link-pending" aria-disabled="true" title="Unavailable">${text}</span>`;
    }
    return `<a href="${item.href}">${text}</a>`;
  }

  function renderNav(activePage) {
    return NAV_ORDER.map((key) => {
      const item = LINKS[key];
      const selected = key === activePage;
      if (!item.ready) {
        return `<li class="${selected ? "selected" : ""}"><span class="link-pending" aria-disabled="true">${item.label}</span></li>`;
      }
      return `<li class="${selected ? "selected" : ""}"><a href="${item.href}"${selected ? ' aria-current="page"' : ""}>${item.label}</a></li>`;
    }).join("\n          ");
  }

  function renderGuestStatus(compact) {
    if (compact) {
      return `Hello. ${anchor("login")} · ${anchor("signupShort")}`;
    }
    return `Hello. ${anchor("login")} to see your projects. New to Tawala? ${anchor("signup")}`;
  }

  function renderHeader(activePage, user, statusMode) {
    const status = user
      ? `Welcome back, <span class="userName">${user}</span>. ${anchor("logout")}`
      : renderGuestStatus(statusMode === "compact");

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
        <li><a href="mailto:info@tawala.com">Contact Us</a></li>
      </ul>
    </div>`;
  }

  function renderBanner(activePage) {
    return (
      `<strong>Website mock</strong> — static draft from legacy JSP. Grey controls = not implemented. Test-drive → :8080. ` +
      `<a href="http://localhost:5173" target="_blank" rel="noopener">Web Designer :5173</a> · this site :5500.` +
      (activePage === "mytawala"
        ? ` · <b>My Tawala</b> (private) · <a href="library.html">Library</a>`
        : activePage === "library"
          ? ` · <b>Library</b> (public) · <a href="mytawala.html">My Tawala</a>`
          : "")
    );
  }

  function mount() {
    const body = document.body;
    const activePage = body.dataset.tawalaPage || "home";
    const user = body.dataset.tawalaUser || "";
    const statusMode = body.dataset.tawalaStatus || "";
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
      headerEl.innerHTML = renderHeader(activePage, user, statusMode);
    }
    if (footerEl) {
      footerEl.innerHTML = renderFooter();
    }
  }

  window.TawalaChrome = { mount, anchor, LINKS };
})();
