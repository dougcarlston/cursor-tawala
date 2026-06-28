package com.tawala.web.projectmanager.dataimport;

import java.util.Collection;

import javax.servlet.http.HttpServletRequest;

import com.tawala.domain.User;
import com.tawala.project.Project;
import com.tawala.project.data.DataSource;
import com.tawala.project.data.FormSubmissionCreator;
import com.tawala.project.data.SharedDataSourceFormSubmissionCreator;
import com.tawala.project.data.StoredField;
import com.tawala.web.WorldInitializer;
import com.tawala.web.controller.UserInfoPreparationInterceptor;
import com.tawala.web.controller.WellKnown;

public class SharedDataImportController extends ImportDataController {
	public static final String PARAMETER_DATASOURCE_NAME = "datasource";

	public static class SharedDataFileUploadBean extends FileUploadBean {
		private String dataSourceName;
		// --- Used as cache.
		private DataSource dataSource;

		public String getDataSourceName() {
			return dataSourceName;
		}

		public void setDataSourceName(String dataSourceName) {
			this.dataSourceName = dataSourceName;
		}

		@Override
		public String getDataSourceDescription() {
			return "shared data store \"" + dataSourceName + "\"";
		}

		@Override
		public String getReturnURL() {
			return WellKnown.urls.getViewSharedDatasources();
		}

		public DataSource getDataSource() {
			return dataSource;
		}

		public void setDataSource(DataSource dataSource) {
			this.dataSource = dataSource;
		}
	}

	@Override
	protected Object formBackingObject(HttpServletRequest request)
			throws Exception {
		SharedDataFileUploadBean fileUploadBean = new SharedDataFileUploadBean();
		fileUploadBean.setDataSourceName(request
				.getParameter(PARAMETER_DATASOURCE_NAME));
		fileUploadBean.setFields(getFields(UserInfoPreparationInterceptor
				.getSessionUser(request), fileUploadBean.getDataSourceName()));
		return fileUploadBean;
	}

	private static Collection<StoredField> getFields(User user,
			String dataSourceName) {
		Project project = WorldInitializer.getDefaultWorld().domain().users()
				.getSharedStorageForUser(user);

		DataSource dataSource = project.getDataSourceNamed(dataSourceName);
		if (dataSource == null) {
			throw new IllegalStateException("Unable to find datasource named "
					+ dataSourceName);
		}
		return dataSource.getFields();
	}

	@Override
	protected void deleteOldData(User user, Object command) {
		SharedDataFileUploadBean fileUploadBean = (SharedDataFileUploadBean) command;
		WorldInitializer.getDefaultWorld().domain().storedData()
				.eraseResponsesFor(user.getSharedStorage(),
						fileUploadBean.getDataSourceName());
	}

	@Override
	protected FormSubmissionCreator getFormSubmissionCreator(Object command,
			User user) {
		SharedDataFileUploadBean uploadBean = (SharedDataFileUploadBean) command;

		DataSource dataSource = uploadBean.getDataSource();
		if (dataSource == null) {
			dataSource = WorldInitializer.getDefaultWorld().domain().users()
					.getSharedStorageForUser(user).getDataSourceNamed(
							uploadBean.getDataSourceName());
			uploadBean.setDataSource(dataSource);
		}
		return new SharedDataSourceFormSubmissionCreator(user, dataSource);
	}
}
