function fnEval(script)
{
    eval(script);
}

function fnImageLoad()
{
	var img = event.srcElement;
	img.ImageWidth = img.clientWidth;
	img.ImageHeight = img.clientHeight;
}

var prefix = "#Field# id=";

function getDraggedFieldId()
{
    var data = event.dataTransfer.getData("Text");
    if (data == null) return 0;
    if (data.indexOf(prefix) != 0) return 0;

    return data.substr(prefix.length);
}


//function fnInsertTable()
//{
//	var range = document.selection.createRange();
//	var rowHeight = " style='height: 12pt;' ";
//	var cellWidth = " style='width: 108pt;' "; // 72pt = 1 inch, 108pt = 1.5 inch
//	
//	var table = "<table class='user'><tbody>";
//	table += "<tr " + rowHeight + ">";
//	table += "<td " + cellWidth + "></td>";
//	table += "<td " + cellWidth + "></td>";
//	table += "</tr>";
//	table += "<tr " + rowHeight + ">";
//	table += "<td " + cellWidth + "></td>";
//	table += "<td " + cellWidth + "></td>";
//	table += "</tr>";
//	table += "</tbody></table>";
//	
//	range.pasteHTML(table);
//	range.select();
//}

function setTableColumnWidths(table)
{
	var tbody = table.getElementsByTagName("TBODY")[0];
    
    var row = tbody.rows[0];
    var columnCount = row.cells.length;
	var tableWidth = parseInt(table.style.width);
    var columnWidth = tableWidth / columnCount;

    for (var rowIndex = 0; rowIndex < tbody.rows.length; rowIndex++)
    {
        var row = tbody.rows[rowIndex];
        
        for (var columnIndex = 0; columnIndex < row.cells.length; columnIndex++)
        {
			var cell = row.cells[columnIndex];
			
			cell.style.width = columnWidth.toString() + "pt";
        }
    }
}

function onResizeEnd()
{
	var table = event.srcElement;
	setTableColumnWidths(table);
}

function fnInsertTable(tableHtml)
{
	var range = document.selection.createRange();
	
	range.pasteHTML(tableHtml);
	range.select();
	
	var table = range.parentElement().getElementsByTagName("TABLE")[0];
	table.onresizeend = onResizeEnd;
}

function fnDeleteTable()
{
	var range = document.selection.createRange();
	if (fnIsTableCell(range)) 
	{
	    var table = range.parentElement().parentElement.parentElement.parentElement;
	    table.parentElement.removeChild(table);
	}
}
		
function fnInsertTableColumnBefore()
{
    fnInsertTableColumn(true);
}

function fnInsertTableColumnAfter()
{
    fnInsertTableColumn(false);
}

function fnInsertTableColumn(insertBefore)
{
    var range = document.selection.createRange();
    if (fnIsTableCell(range))
    {
        var td = range.parentElement();
        var cellIndex = td.cellIndex + (insertBefore ? 0 : 1);

        var tr = td.parentElement;
        var tbody = tr.parentElement;
        
        for (var i = 0; i < tbody.rows.length; ++i)
        {
            var r = tbody.rows[i];
            var newCell = r.insertCell(cellIndex);
            newCell.style.width="36pt";
        }
    }
}

function fnDeleteTableColumn()
{
    var range = document.selection.createRange();
    if (fnIsTableCell(range))
    {
        var td = range.parentElement();
        var cellIndex = td.cellIndex;

        var tr = td.parentElement;
        var tbody = tr.parentElement;
        
        for (var i = 0; i < tbody.rows.length; ++i)
        {
            var r = tbody.rows[i];
            r.deleteCell(cellIndex);
        }
    }
}

function fnInsertTableRowBefore()
{
    fnInsertTableRow(true);
}

function fnInsertTableRowAfter()
{
    fnInsertTableRow(false);
}

function fnInsertTableRow(insertBefore)
{
	var range = document.selection.createRange();
	if (fnIsTableCell(range)) 
	{
        var tr = range.parentElement().parentElement;
	    var tbody = tr.parentElement;
	    
        var rowIndex = insertBefore == true ? tr.sectionRowIndex : tr.sectionRowIndex + 1;
        var newRow = tbody.insertRow(rowIndex);
        newRow.style.cssText = tr.style.cssText;
        for (var i = 0; i < tr.cells.length; ++i)
        {
            var newCell = newRow.insertCell();
            newCell.style.cssText = tr.cells[i].style.cssText;
	    }
	}
}

function fnDeleteTableRow()
{
	var range = document.selection.createRange();
	if (fnIsTableCell(range)) 
	{
        var tr = range.parentElement().parentElement;
	    var tbody = tr.parentElement;
	    
        tbody.deleteRow(tr.sectionRowIndex);
	}
}

function fnIsTableCell(range)
{
	if (range.parentElement().tagName == "TD") 
	{
		var table = range.parentElement().parentElement.parentElement.parentElement;
		if (table.className == "user")
		{
			return true;
		}
	}
    
    return false;
}

function fnTableCellIsSelected()
{
	var range = document.selection.createRange();
	return fnIsTableCell(range);
}

function fnGetCellWidth()
{
	var range = document.selection.createRange();
	
	if (fnIsTableCell(range))
	{
		return range.parentElement().style.width;
	}
}

function fnSetColumnWidth(widthStringInPoints)
{
	var range = document.selection.createRange();
	
	if (fnIsTableCell(range))
	{
        var td = range.parentElement();
        var sourceColumnIndex = td.cellIndex;

        var tr = td.parentElement;
        var tbody = tr.parentElement;
        
        for (var rowIndex = 0; rowIndex < tbody.rows.length; rowIndex++)
        {
            var row = tbody.rows[rowIndex];
            
            for (var columnIndex = 0; columnIndex < row.cells.length; columnIndex++)
            {
				if (columnIndex == sourceColumnIndex)
				{
					var cell = row.cells[columnIndex];
				
					cell.style.width = widthStringInPoints + "pt";
				}
            }
        }
        
        var row = tbody.rows[0];
        var tableWidth = 0;
        
        for (var columnIndex = 0; columnIndex < row.cells.length; columnIndex++)
        {
			tableWidth += parseInt(row.cells[columnIndex].style.width);
        }
        
        var table = tbody.parentElement;
        
        table.style.width = tableWidth.toString() + "pt";
	}
}
