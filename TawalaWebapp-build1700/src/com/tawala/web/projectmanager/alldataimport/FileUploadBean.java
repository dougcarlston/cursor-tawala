/**
 * 
 */
package com.tawala.web.projectmanager.alldataimport;

import java.io.IOException;

import org.apache.poi.hssf.usermodel.HSSFWorkbook;
import org.springframework.web.multipart.MultipartFile;

import com.tawala.project.UserProject;
import com.tawala.project.data.DataImporter;

public class FileUploadBean {
	private MultipartFile data;
	private boolean confirmDataDeletion;
	private HSSFWorkbook workbook;
	private UserProject userProject;
	private DataImporter dataImporter;

	public FileUploadBean(UserProject userProject) {
		this.userProject = userProject;
	}
	
	public HSSFWorkbook getWorkbook() throws IOException {
		if (workbook == null) {
			workbook = new HSSFWorkbook(data.getInputStream());
		}
		return workbook;
	}

	public MultipartFile getData() {
		return data;
	}

	public void setData(MultipartFile data) {
		this.data = data;
		this.workbook = null;
		this.dataImporter = null;
	}

	public boolean isEmptyFile() throws IOException {
		return data == null || data.getBytes().length == 0;
	}

	public UserProject getUserProject() {
		return userProject;
	}

	public boolean isConfirmDataDeletion() {
		return confirmDataDeletion;
	}

	public void setConfirmDataDeletion(boolean confirmDataDeletion) {
		this.confirmDataDeletion = confirmDataDeletion;
	}

	public DataImporter getDataImporter() throws IOException {
		if(dataImporter == null) {
			dataImporter = new DataImporter(getUserProject(), getWorkbook());
		}
		return dataImporter;
	}
}