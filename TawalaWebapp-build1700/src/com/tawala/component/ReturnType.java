package com.tawala.component;

public enum ReturnType {
	STRING {
		public String getId() {
			return "string";
		}
	},
	INT {
		public String getId() {
			return "int";
		}
	},
	DOUBLE {
		public String getId() {
			return "double";
		}
	}	
	;

	abstract public  String getId(); 
}
