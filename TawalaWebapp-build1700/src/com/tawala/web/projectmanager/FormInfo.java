package com.tawala.web.projectmanager;

import com.tawala.project.Form;

public class FormInfo {
	private Form form;
	private long responses;
	private String size;
	private boolean startingPoint;

	public FormInfo(Form form, long responses, String size){
		this.form = form;
		this.responses = responses;
		this.size = size;
		this.startingPoint = form.isStartingPoint();
	}

	public Form getForm(){
		return form;
	}

    public long getResponses(){
		return(this.responses);
	}
	
    public String getSize(){
		return(this.size);
	}

    public boolean getStartingPoint(){
		return(this.startingPoint);
	}
}
