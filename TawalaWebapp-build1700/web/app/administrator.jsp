<%@ page contentType="text/html; charset=UTF-8" %>
<%@ include file="/WEB-INF/jsp/admin/administrator-access.jspf" %>
<%
  response.setHeader("Cache-Control", "no-store");
  com.tawala.domain.User administratorAccount =
      (com.tawala.domain.User) request.getAttribute("administratorUser");
  boolean administratorAllowedFlag = Boolean.TRUE.equals(request.getAttribute("administratorAllowed"));

  if (!administratorAllowedFlag || administratorAccount == null) {
    response.sendRedirect("/home");
    return;
  }

  if ("POST".equalsIgnoreCase(request.getMethod())) {
    String posted = request.getParameter("administrator");
    boolean turnOn = "true".equals(posted);
    boolean turnOff = "false".equals(posted);
    java.util.List adminsNow = com.tawala.UsersHibernateImpl.getAllAdminUsers();
    boolean anyNow = adminsNow != null && !adminsNow.isEmpty();
    boolean isBootstrapAccount = "tawalamundo".equals(
        com.tawala.domain.User.normalizeUserName(administratorAccount.getId()));
    boolean changed = false;
    if (turnOn && !administratorAccount.isAdministrator() && !anyNow && isBootstrapAccount) {
      administratorAccount.setAdministrator(true);
      changed = true;
    } else if (turnOff && administratorAccount.isAdministrator()) {
      administratorAccount.setAdministrator(false);
      changed = true;
    } else if (turnOn && administratorAccount.isAdministrator()) {
      changed = false;
    }
    if (changed) {
      com.tawala.web.WorldInitializer.getDefaultWorld().domain().users().addOrSave(administratorAccount);
      try {
        com.tawala.hibernate.TawalaSessionFactory.MAIN.getFactory().evict(
            com.tawala.domain.User.class,
            Long.valueOf(administratorAccount.getDatabaseId()));
        com.tawala.hibernate.TawalaSessionFactory.MAIN.evictCachedDataForQuery(
            "from " + com.tawala.domain.User.class.getName()
                + " where normalizedUserName = :userName");
      } catch (RuntimeException ignored) {
      }
      com.tawala.domain.User reloaded = com.tawala.web.WorldInitializer.getDefaultWorld()
          .domain().users().get(administratorAccount.getId());
      if (request.getSession(false) != null) {
        request.getSession(false).setAttribute("user", reloaded != null ? reloaded : administratorAccount);
      }
    }
    response.sendRedirect("/app/administrator.jsp");
    return;
  }
%>
<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="utf-8" />
  <title>Administrator</title>
</head>
<body>
  <h1>Administrator</h1>
  <% if (administratorAccount.isAdministrator()) { %>
  <form method="post" action="/app/administrator.jsp">
    <p>Administrator</p>
    <button type="submit" name="administrator" value="false">Turn off</button>
  </form>
  <% } else { %>
  <form method="post" action="/app/administrator.jsp">
    <p>Administrator</p>
    <button type="submit" name="administrator" value="true">Turn on</button>
  </form>
  <% } %>
</body>
</html>
