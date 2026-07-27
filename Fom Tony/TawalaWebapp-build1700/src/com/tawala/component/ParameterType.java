package com.tawala.component;

public enum ParameterType {
	INT {
		public String getId() {
			return "int";
		}
	},
	TEXT {
		public String getId() {
			return "text";
		}
	},
	ENUMERATION {
		public String getId() {
			return "enumeration";
		}
	},
	FORM_NAME {
		public String getId() {
			return "tawala-form";
		}
	},
	FIELD_NAME {
		public String getId() {
			return "tawala-field";
		}
	},
	MCQ_FIELD_NAME {
		public String getId() {
			return "tawala-mcq";
		}
	},
	BLANK_FIELD_NAME {
		public String getId() {
			return "tawala-blank";
		}
	},
	RECORD_SELECTOR {
		public String getId() {
			return "tawala-conditions";
		}
	},
	BOOLEAN_EXPRESSION {
		public String getId() {
			return "boolean-expression";
		}
	},
	COLLECTION {
		public String getId() {
			return "parameter-collection";
		}
	},
	NUMERIC_LIST {
		public String getId() {
			return "numeric-list";
		}
	},
	DISPLAYABLE_CONTENTS {
		public String getId() {
			return "tawala-contents-field";
		}
	},
	EXPRESSION {
		public String getId() {
			return "expression";
		}
	},	;
	

	abstract public  String getId(); 
}
