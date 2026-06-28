var DOM = YAHOO.util.Dom;
var Event = YAHOO.util.Event;

if(!Tawala) { var Tawala = {}; }

// Patch for width and/or minWidth Column values bug in non-scrolling DataTables
(function(){var B=YAHOO.widget.DataTable,A=YAHOO.util.Dom;B.prototype._setColumnWidth=function(I,D,J){I=this.getColumn(I);if(I){J=J||"hidden";if(!B._bStylesheetFallback){var N;if(!B._elStylesheet){N=document.createElement("style");N.type="text/css";B._elStylesheet=document.getElementsByTagName("head").item(0).appendChild(N)}if(B._elStylesheet){N=B._elStylesheet;var M=".yui-dt-col-"+I.getId();var K=B._oStylesheetRules[M];if(!K){if(N.styleSheet&&N.styleSheet.addRule){N.styleSheet.addRule(M,"overflow:"+J);N.styleSheet.addRule(M,"width:"+D);K=N.styleSheet.rules[N.styleSheet.rules.length-1]}else{if(N.sheet&&N.sheet.insertRule){N.sheet.insertRule(M+" {overflow:"+J+";width:"+D+";}",N.sheet.cssRules.length);K=N.sheet.cssRules[N.sheet.cssRules.length-1]}else{B._bStylesheetFallback=true}}B._oStylesheetRules[M]=K}else{K.style.overflow=J;K.style.width=D}return }B._bStylesheetFallback=true}if(B._bStylesheetFallback){if(D=="auto"){D=""}var C=this._elTbody?this._elTbody.rows.length:0;if(!this._aFallbackColResizer[C]){var H,G,F;var L=["var colIdx=oColumn.getKeyIndex();","oColumn.getThEl().firstChild.style.width="];for(H=C-1,G=2;H>=0;--H){L[G++]="this._elTbody.rows[";L[G++]=H;L[G++]="].cells[colIdx].firstChild.style.width=";L[G++]="this._elTbody.rows[";L[G++]=H;L[G++]="].cells[colIdx].style.width="}L[G]="sWidth;";L[G+1]="oColumn.getThEl().firstChild.style.overflow=";for(H=C-1,F=G+2;H>=0;--H){L[F++]="this._elTbody.rows[";L[F++]=H;L[F++]="].cells[colIdx].firstChild.style.overflow=";L[F++]="this._elTbody.rows[";L[F++]=H;L[F++]="].cells[colIdx].style.overflow="}L[F]="sOverflow;";this._aFallbackColResizer[C]=new Function("oColumn","sWidth","sOverflow",L.join(""))}var E=this._aFallbackColResizer[C];if(E){E.call(this,I,D,J);return }}}else{}};B.prototype._syncColWidths=function(){var J=this.get("scrollable");if(this._elTbody.rows.length>0){var M=this._oColumnSet.keys,C=this.getFirstTrEl();if(M&&C&&(C.cells.length===M.length)){var O=false;if(J&&(YAHOO.env.ua.gecko||YAHOO.env.ua.opera)){O=true;if(this.get("width")){this._elTheadContainer.style.width="";this._elTbodyContainer.style.width=""}else{this._elContainer.style.width=""}}var I,L,F=C.cells.length;for(I=0;I<F;I++){L=M[I];if(!L.width){this._setColumnWidth(L,"auto","visible")}}for(I=0;I<F;I++){L=M[I];var H=0;var E="hidden";if(!L.width){var G=L.getThEl();var K=C.cells[I];if(J){var N=(G.offsetWidth>K.offsetWidth)?G.firstChild:K.firstChild;if(G.offsetWidth!==K.offsetWidth||N.offsetWidth<L.minWidth){H=Math.max(0,L.minWidth,N.offsetWidth-(parseInt(A.getStyle(N,"paddingLeft"),10)|0)-(parseInt(A.getStyle(N,"paddingRight"),10)|0))}}else{if(K.offsetWidth<L.minWidth){E=K.offsetWidth?"visible":"hidden";H=Math.max(0,L.minWidth,K.offsetWidth-(parseInt(A.getStyle(K,"paddingLeft"),10)|0)-(parseInt(A.getStyle(K,"paddingRight"),10)|0))}}}else{H=L.width}if(L.hidden){L._nLastWidth=H;this._setColumnWidth(L,"1px","hidden")}else{if(H){this._setColumnWidth(L,H+"px",E)}}}if(O){var D=this.get("width");this._elTheadContainer.style.width=D;this._elTbodyContainer.style.width=D}}}this._syncScrollPadding()}})();
// Patch for initial hidden Columns bug
(function(){var A=YAHOO.util,B=YAHOO.env.ua,E=A.Event,C=A.Dom,D=YAHOO.widget.DataTable;D.prototype._initTheadEls=function(){var X,V,T,Z,I,M;if(!this._elThead){Z=this._elThead=document.createElement("thead");I=this._elA11yThead=document.createElement("thead");M=[Z,I];E.addListener(Z,"focus",this._onTheadFocus,this);E.addListener(Z,"keydown",this._onTheadKeydown,this);E.addListener(Z,"mouseover",this._onTableMouseover,this);E.addListener(Z,"mouseout",this._onTableMouseout,this);E.addListener(Z,"mousedown",this._onTableMousedown,this);E.addListener(Z,"mouseup",this._onTableMouseup,this);E.addListener(Z,"click",this._onTheadClick,this);E.addListener(Z.parentNode,"dblclick",this._onTableDblclick,this);this._elTheadContainer.firstChild.appendChild(I);this._elTbodyContainer.firstChild.appendChild(Z)}else{Z=this._elThead;I=this._elA11yThead;M=[Z,I];for(X=0;X<M.length;X++){for(V=M[X].rows.length-1;V>-1;V--){E.purgeElement(M[X].rows[V],true);M[X].removeChild(M[X].rows[V])}}}var N,d=this._oColumnSet;var H=d.tree;var L,P;for(T=0;T<M.length;T++){for(X=0;X<H.length;X++){var U=M[T].appendChild(document.createElement("tr"));P=(T===1)?this._sId+"-hdrow"+X+"-a11y":this._sId+"-hdrow"+X;U.id=P;for(V=0;V<H[X].length;V++){N=H[X][V];L=U.appendChild(document.createElement("th"));if(T===0){N._elTh=L}P=(T===1)?this._sId+"-th"+N.getId()+"-a11y":this._sId+"-th"+N.getId();L.id=P;L.yuiCellIndex=V;this._initThEl(L,N,X,V,(T===1))}if(T===0){if(X===0){C.addClass(U,D.CLASS_FIRST)}if(X===(H.length-1)){C.addClass(U,D.CLASS_LAST)}}}if(T===0){var R=d.headers[0];var J=d.headers[d.headers.length-1];for(X=0;X<R.length;X++){C.addClass(C.get(this._sId+"-th"+R[X]),D.CLASS_FIRST)}for(X=0;X<J.length;X++){C.addClass(C.get(this._sId+"-th"+J[X]),D.CLASS_LAST)}var Q=(A.DD)?true:false;var c=false;if(this._oConfigs.draggableColumns){for(X=0;X<this._oColumnSet.tree[0].length;X++){N=this._oColumnSet.tree[0][X];if(Q){L=N.getThEl();C.addClass(L,D.CLASS_DRAGGABLE);var O=D._initColumnDragTargetEl();N._dd=new YAHOO.widget.ColumnDD(this,N,L,O)}else{c=true}}}for(X=0;X<this._oColumnSet.keys.length;X++){N=this._oColumnSet.keys[X];if(N.resizeable){if(Q){L=N.getThEl();C.addClass(L,D.CLASS_RESIZEABLE);var G=L.firstChild;var F=G.appendChild(document.createElement("div"));F.id=this._sId+"-colresizer"+N.getId();N._elResizer=F;C.addClass(F,D.CLASS_RESIZER);var e=D._initColumnResizerProxyEl();N._ddResizer=new YAHOO.util.ColumnResizer(this,N,L,F.id,e);var W=function(f){E.stopPropagation(f)};E.addListener(F,"click",W)}else{c=true}}}if(c){}}else{}}for(var a=0,Y=this._oColumnSet.keys.length;a<Y;a++){if(this._oColumnSet.keys[a].hidden){var b=this._oColumnSet.keys[a];var S=b.getThEl();b._nLastWidth=S.offsetWidth-(parseInt(C.getStyle(S,"paddingLeft"),10)|0)-(parseInt(C.getStyle(S,"paddingRight"),10)|0);this._setColumnWidth(b.getKeyIndex(),"1px")}}if(B.webkit&&B.webkit<420){var K=this;setTimeout(function(){K._elThead.style.display=""},0);this._elThead.style.display="none"}}})();

function dataTableSetup() {
	var dt = new Tawala.DataTable();
	dt.container = document.getElementById("records");
	dt.columnDefs = dt.resolveColumnDefinitionReferences(columnDefinitions);
	dt.createDataSourceSchema();
	dt.createDataTable();
	dt.showHideColumnDialog();
	dt.container.style.width = "auto";				
}

YAHOO.util.Event.onDOMReady(dataTableSetup);	

Tawala.DataTableFormat = {
   	formatMultilineText: function(elCell, oRecord, oColumn, oData) { 
           elCell.innerHTML = "<pre style=\"font-family: arial;\">" + oData + "</pre>"; 
    },
    formatDeleteRecordCheckbox: function(el, oRecord, oColumn, oData) {
        el.innerHTML = '<input name="submission_id" value="' + oData + '" id="checkbox' + oData + '" type="checkbox">';
    }
}
Tawala.DataParser = {
	parseDate: YAHOO.util.DataSource.parseDate
}

Tawala.DataTable = function() {
	
	return {
		container: "",
		columnDefs: [],
		dataSource: {},
		dataTable: {},
		shDialog: {},
		dialogBdEl: {},
		init: function() {
			
		},
		
		resolveColumnDefinitionReferences: function(definitions) {
				//--- JSON library can't generate direct function reference, just a string.
				//--- That's why this double reference.
			for(var i=0; i< definitions.length; i++) {
				var definition = definitions[i];
				if(definition.formatter != undefined) {
					var formatterFunction = Tawala.DataTableFormat[definition.formatter];
					definition.formatter = formatterFunction;
				}
			}
			return definitions;
		},
		
		createDataSourceSchema: function() {
			this.dataSource = new YAHOO.util.DataSource(records);
			this.dataSource.responseType = YAHOO.util.DataSource.TYPE_JSARRAY;
			this.dataSource.responseSchema = responseSchema;

				//--- Resolve references into functions.			
			for(var i=0; i<responseSchema.fields.length; i++) {
				var field = responseSchema.fields[i];
				if(field.parser != undefined) {
					field.parser = Tawala.DataParser[field.parser];
				}
			} 			
		},

		createDataTable: function() {
			var highlightEditableCell = function(oArgs) { 
		            var elCell = oArgs.target.getElementsByTagName("div")[0];
		            if(elCell && YAHOO.util.Dom.hasClass(elCell, "yui-dt-editable")) { 
		                this.highlightCell(elCell); 
		            } 
		     };

			var dataTableOptions = {	
				scrollable:true, 
				width:"950px", 
				height:"30em", 
				draggableColumns:true
			};
			
			if(recordCount > 25) {
				dataTableOptions.paginator = new YAHOO.widget.Paginator({
			        rowsPerPage : 25
			    });
			}

			this.dataTable = new YAHOO.widget.DataTable(this.container, this.columnDefs, 
														this.dataSource, dataTableOptions );						
			this.dataTable.subscribe("cellMouseoverEvent", highlightEditableCell);
			this.dataTable.subscribe("cellMouseoutEvent", this.dataTable.onEventUnhighlightCell); 
	        this.dataTable.subscribe("cellClickEvent", this.dataTable.onEventShowCellEditor); 
		},


		showHideColumnDialog: function(){
			this.newCols = true;
			this.columnList = [];
			
			this.hideUncheckedColumns = function(){
				var colSet = this.dataTable.getColumnSet();
				for (var i = 0; i < colSet.keys.length; i++) {
					if (colSet.keys[i].checked == false) {
						this.dataTable.hideColumn(colSet.keys[i]);
					}
				}
			}
			
			this.showAllColumns = function(){
				var colSet = this.dataTable.getColumnSet();
				for (var i = 0; i < colSet.keys.length; i++) {
					this.dataTable.showColumn(colSet.keys[i]);
				}
				
			}
			
			this.handleCheckboxClick = function(e, oSelf){
				var target = YAHOO.util.Event.getTarget(e);
				var col = this.dataTable.getColumn(target.id);
				if (target.checked) {
					oSelf.dataTable.showColumn(col);
					col.checked = true;
				}
				else {
					oSelf.dataTable.hideColumn(col);
					col.checked = false;
				}
			}
			
			this.populateDialog = function(){
				// create list element
				var newList = document.createElement("ul");
				
				// create list item template
				var itemTemplate = document.createElement("li");
				var itemLabel = document.createElement("label");
				var itemInput = document.createElement("input");
				itemInput.setAttribute("type", "checkbox");
				itemLabel.appendChild(itemInput);
				itemTemplate.appendChild(itemLabel);
				
				var allColumns = this.dataTable.getColumnSet().keys;
				for (var i = 0, l = allColumns.length; i < l; i++) {
					var oColumn = allColumns[i];
					if(oColumn.getKey().substring(0,1) != "_"){					
						// Use the template
						elColumn = itemTemplate.cloneNode(true);
						var stripe = (i % 2 == 0) ? "even" : "odd";
						YAHOO.util.Dom.addClass(elColumn, stripe);
						elLabel = elColumn.firstChild;
						elInput = elLabel.firstChild;
						var cKey = oColumn.getKey();
						if (oColumn.checked) {
							elInput.checked = true;
						}
						else {
							elInput.checked = false;
						}
						elLabel.setAttribute("for", cKey);
						elInput.id = cKey;
						elInput.name = cKey;
						YAHOO.util.Event.addListener(elInput, "click", this.handleCheckboxClick, this, true);
						elLabel.appendChild(elInput);
						elLabel.appendChild(document.createTextNode(cKey));
						newList.appendChild(elColumn);
					}
				}
				return (newList);
			}
			
			this.showDlg = function(e) {
				YAHOO.util.Event.stopEvent(e);
				
				// Populate the column list
				var container = $("dt-columnSettings");
				if ($("dt-columnList").childNodes.length != 0) {
					$("dt-columnList").removeChild($("dt-columnList").childNodes[0]);
				}
				
				$("dt-columnList").appendChild(this.populateDialog());
				
				// Toggle container visibility
				if (container.style.visibility == "") {
					container.style.visibility = "hidden";
				}
				
				if (container.style.visibility == "hidden") {
					var attributes = {
						height: {
							from: 0,
							to: 240
						},
						opacity: {
							from: 0,
							to: 1
						}
					};
					var anim = new YAHOO.util.Anim(container, attributes, 1, YAHOO.util.Easing.easeOut);
					anim.animate();
					
					container.style.visibility = "visible";
				}
				else {
					var attributes = {
						height: {
							to: 0
						},
						opacity: {
							to: 0
						}
					};
					var anim = new YAHOO.util.Anim(container, attributes, 1, YAHOO.util.Easing.easeOut);
					anim.onComplete.subscribe(function(){
						container.style.visibility = "hidden";
					});
					
					anim.animate();
				}				
			}

			// Hook up the SimpleDialog to the link
			YAHOO.util.Event.addListener("dt-options-link", "click", this.showDlg, this, true);
			
			// Setup button events
			YAHOO.util.Event.addListener("buttonDone", "click", this.showDlg, this, true);
			YAHOO.util.Event.addListener("buttonHideChecked", "click", this.hideUncheckedColumns, this, true);
			YAHOO.util.Event.addListener("buttonShowAll", "click", this.showAllColumns, this, true);				
		}
	}
}


Tawala.DataView = function(){
	
		
	return {
		
		selectedItems: "",
		
		deleteSelectedItems: function(e){
			Event.stopEvent(e);
			Tawala.DataView.confirmationDialog.show();
			return(false);
		},

		init: function() {			
			YAHOO.util.Event.addListener("deleteSelectedRecords", "click", 
				Tawala.DataView.deleteSelectedItems, Tawala.DataView, true);

			// Define various event handlers for Dialog
			var dialogHandleYes = function() {
				var form = document.getElementById("deleteSubmissionForm");
				form.submit();
				this.hide();
			}
		
			var dialogHandleNo = function() {
				this.hide();
			}
		
			Tawala.DataView.confirmationDialog =  
				new YAHOO.widget.SimpleDialog("confirmationDialog", 
					 { width: "350px",
					   fixedcenter: true,
					   visible: false,
					   draggable: false,
					   modal: true,
					   close: true,
					   text: "You are about to delete the selected items." +  
					   			"<br /><br />Are you sure you want to do this?",
					   icon: YAHOO.widget.SimpleDialog.ICON_WARN,
					   constraintoviewport: true,
					   effect:{effect:YAHOO.widget.ContainerEffect.FADE,duration:0.5},
					   buttons: [ { text:"Yes", handler:dialogHandleYes, isDefault:true },
								  { text:"No",  handler:dialogHandleNo } ]
					 } )
					 
			Tawala.DataView.confirmationDialog.setHeader('<div class="tl"></div><div class="ti">Delete Submission</div><div class="tr"></div>');
			Tawala.DataView.confirmationDialog.render(document.body);								
		}
	};

}();

//--- Adopted from http://yuiblog.com/blog/2007/09/26/satyam-datatable-2/
//--- It had to be updated based on the latest code in database-beta.js
YAHOO.widget.DataTable.prototype.saveCellEditor = function() {

    var onFailure = function (msg) {
        alert(msg);
        this.resetCellEditor();
        this.fireEvent("editorRevertEvent",
            {editor:this._oCellEditor, oldData:oldData, newData:newData}
        );
    };
    

    if(this._oCellEditor.isActive) {
        var newData = this._oCellEditor.value;
        var oldData = YAHOO.widget.DataTable._cloneObject(this._oCellEditor.record.getData(this._oCellEditor.column.key));

        if(this._oCellEditor.validator) {
            newData = this._oCellEditor.value = this._oCellEditor.validator.call(this, newData, oldData, this._oCellEditor);
            if(newData === null ) {
                onFailure('Validation failed. Please try again.');
                return;
            }
        }

		var valuesParameters = "";
		if(YAHOO.lang.isArray(this._oCellEditor.value)) {
			var i;
			for(i=0;i<this._oCellEditor.value.length; i++) {
				valuesParameters += '&values=' + escape(this._oCellEditor.value[i]);
			}
		} else {
			valuesParameters = '&values=' + escape(newData);
		}
            //--- see ChangeSubmissionFieldValueController for details
		var urlToChangeValue = changeFieldValueURL + '&record_id=' + this._oCellEditor.record.getData('_primaryKey') + '&field=' + escape(this._oCellEditor.column.key) + valuesParameters;
		
        YAHOO.util.Connect.asyncRequest(
            'POST',
            urlToChangeValue,
            {
                success: function (o) {
                    var r = YAHOO.lang.JSON.parse(o.responseText);
                    if (r.success) {
				        this._oRecordSet.updateRecordValue(this._oCellEditor.record, this._oCellEditor.column.key, this._oCellEditor.value);
				        this.formatCell(this._oCellEditor.cell.firstChild);

				        // Bug fix 1764044
				        this._oChainRender.add({
				            method: function() {
				                this._syncColWidths();
				            },
				            scope: this
				        });
				        this._oChainRender.run();
				        // Clear out the Cell Editor
				        this.resetCellEditor();

                        this.fireEvent("editorSaveEvent",
                            {editor:this._oCellEditor, oldData:oldData, newData:newData}
                        );
                    } else {
                        onFailure("Failed to update record");
                    }
                },
                failure: function(o) {
                    onFailure(o.statusText);
                },
                scope: this
            }
        );
    } else {
    }
};

YAHOO.util.Event.addListener(window, "load", 
		Tawala.DataView.init, Tawala.DataView, true);
 
