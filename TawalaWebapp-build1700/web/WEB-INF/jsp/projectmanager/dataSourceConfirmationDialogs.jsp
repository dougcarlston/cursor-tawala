<jsp:directive.page import="com.tawala.web.projectmanager.ViewSharedDataSourcesController"/>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<!-- confirmation dialogs -->
    <div id="deleteConfirm" class="popup" >
         <div class="title">Delete Data Source</div>
         Are you sure you want to delete this shared data source?
         <br /><br />
         <form method="post">
             <input type="submit" name="action" value="<%= ViewSharedDataSourcesController.PARAMETER_ACTION_DELETE %>" />
             <input type="button" name="cancel" value="Cancel" />
         </form>
    </div>

    <div id="eraseConfirm" class="popup" >
         <div class="title">Erase Data Source Data</div>
         Are you sure you want to erase the data for this data source?
         <br /><br />
         <form method="post">
             <input type="submit" name="action" value="<%= ViewSharedDataSourcesController.PARAMETER_ACTION_ERASE %>" />
             <input type="button" name="cancel" value="Cancel" />
         </form>
    </div>

<!-- end confirmation dialogs -->

