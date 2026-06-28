function markFirstRowSkipped(checkbox) {
	var classAttribute = "";
	if(checkbox.checked == true) {
		classAttribute = "skipped";
	}
	
	var row = document.getElementById('firstDataRow');
	row.setAttribute("class", classAttribute);
	row.setAttribute("className", classAttribute);
}

