
<jsp:directive.page import="com.tawala.web.library.ViewProjectDetailsController"/>
<jsp:directive.page import="com.tawala.web.projectmanager.ViewProjectManagerDetailsController"/><%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>

<!-- confirmation dialogs -->
    <div id="deleteallemailsConfirm" class="popup" >
         <div class="title">Delete All Project Emails</div>
         Are you sure you want to delete all project emails?
         <br /><br />
         <form method="post">
             <input type="hidden" name="action" value="<%= ViewProjectManagerDetailsController.PARAMETER_ACTION_DELETE_VERSION %>" />
             <input type="submit" value="Delete All Emails" />
             <input type="button" name="cancel" value="Cancel" />
         </form>
    </div>

    <div id="deleteConfirm" class="popup" >
         <div class="title">Delete Project</div>
         Are you sure you want to delete this project?
         <br /><br />
         <form method="post">
             <input type="submit" name="action" value="<%= ViewProjectManagerDetailsController.PARAMETER_ACTION_DELETE %>" />
             <input type="button" name="cancel" value="Cancel" />
         </form>
    </div>

    <div id="purgeConfirm" class="popup" >
         <div class="title">Purge Project Data</div>
         Are you sure you want to purge all the data from this project?
         <br /><br />
         <form method="post">
             <input type="submit" name="action" value="<%= ViewProjectManagerDetailsController.PARAMETER_ACTION_PURGE %>" />
             <input type="button" name="cancel" value="Cancel" />
         </form>
    </div>

    <div id="eraseConfirm" class="popup" >
         <div class="title">Erase Form Data</div>
         Are you sure you want to erase the data for this form?
         <br /><br />
         <form method="post">
             <input type="submit" name="action" value="<%= ViewProjectManagerDetailsController.PARAMETER_ACTION_ERASE %>" />
             <input type="button" name="cancel" value="Cancel" />
         </form>
    </div>

    <div id="deleteversionConfirm" class="popup" >
         <div class="title">Delete Project Version</div>
         Are you sure you want to delete this project version?
         <br /><br />
         <form method="post">
             <input type="hidden" name="action" value="<%= ViewProjectManagerDetailsController.PARAMETER_ACTION_DELETE_VERSION %>" />
             <input type="submit" value="Delete Version" />
             <input type="button" name="cancel" value="Cancel" />
         </form>
    </div>

    <div id="deletebackupConfirm" class="popup" >
         <div class="title">Delete Backup</div>
         Are you sure you want to delete this backup?
         <br /><br />
         <form method="post" action="${urls.projectManagerDeleteOnlineBackup}">
             <input type="submit" value="Delete" />
             <input type="button" name="cancel" value="Cancel" />
         </form>
    </div>

    <div id="deleteallprojectbackupsConfirm" class="popup" >
         <div class="title">Delete All Backups</div>
         Are you sure you want to delete all the backups for this project?
         <br /><br />
         <form method="post" action="${urls.projectManagerDeleteOnlineBackup}">
             <input type="submit" value="Delete" />
             <input type="button" name="cancel" value="Cancel" />
         </form>
    </div>

<!-- end confirmation dialogs -->

