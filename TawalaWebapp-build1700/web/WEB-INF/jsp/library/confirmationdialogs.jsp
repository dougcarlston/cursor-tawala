<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>

<!-- Library confirmation dialogs -->
    <div id="deleteprojectConfirm" class="popup" >
         <div class="title">Delete Project</div>
         Are you sure you want to delete this project from the Library?
         <br /><br />
         <form method="post" action="/library/deleteproject">
             <input type="submit" name="action" value="Delete" />
             <input type="button" name="cancel" value="Cancel" />
         </form>
    </div>

   <div id="deleteprojectversionConfirm" class="popup" >
         <div class="title">Delete Project Version</div>
         Are you sure you want to delete this version of the project from the Library?
         <br /><br />
         <form method="post" action="/library/deleteversion">
             <input type="submit" name="action" value="Delete" />
             <input type="button" name="cancel" value="Cancel" />
         </form>
    </div>

    <div id="deletecommentConfirm" class="popup" >
         <div class="title">Delete Comment</div>
         Are you sure you want to delete this comment?
         <br /><br />
         <form method="post" action="/library/deletecomment">
             <input type="submit" name="action" value="Delete" />
             <input type="button" name="cancel" value="Cancel" />
         </form>
    </div>

    <div id="deletecategoryConfirm" class="popup" >
         <div class="title">Delete Category</div>
         Are you sure you want to delete this category?
         <br /><br />
         <form method="post" action="/library/deletecategory">
             <input type="submit" name="action" value="Delete" />
             <input type="button" name="cancel" value="Cancel" />
         </form>
    </div>

<!-- end confirmation dialogs -->

