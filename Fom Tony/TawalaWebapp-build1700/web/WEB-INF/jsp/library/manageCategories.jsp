<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core"%>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt"%>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions"%>

<div id="categories">
		<div id="categoryDetails">
		<jsp:include page="/WEB-INF/jsp/library/editCategory.jsp" />
		</div>
</div>

<script type="text/javascript">     
	function showCategorySection(sectionId) {
		var oldSection = document.getElementById("displayCategory");
		var newSection = document.getElementById(sectionId);
		newSection.style.display = 'block';
		newSection.style.visibility = 'visible';
		oldSection.style.display = 'none';
	}     

	function cancelEdit(sectionId) {
		var newSection = document.getElementById(sectionId);
		var oldSection = document.getElementById("displayCategory");
		newSection.style.visibility = 'hidden';
		newSection.style.display = 'none';
		oldSection.style.display = 'block';
	}     
</script>
