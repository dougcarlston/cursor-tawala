package com.tawala.web.projectmanager.dataimport;

import java.io.IOException;
import java.util.ArrayList;
import java.util.Collection;
import java.util.HashMap;
import java.util.HashSet;
import java.util.LinkedHashSet;
import java.util.List;
import java.util.Map;
import java.util.Set;

import javax.servlet.ServletException;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.apache.poi.hssf.usermodel.HSSFWorkbook;
import org.springframework.validation.BindException;
import org.springframework.validation.Errors;
import org.springframework.web.bind.ServletRequestDataBinder;
import org.springframework.web.multipart.support.StringMultipartFileEditor;
import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.AbstractWizardFormController;

import com.tawala.domain.User;
import com.tawala.project.data.DataImporter;
import com.tawala.project.data.DataIterator;
import com.tawala.project.data.FieldSetter;
import com.tawala.project.data.FormSubmissionCreator;
import com.tawala.project.data.ImportStatistics;
import com.tawala.project.data.StoredField;
import com.tawala.web.controller.UserInfoPreparationInterceptor;

public abstract class ImportDataController extends AbstractWizardFormController {
	private static final String COMMAND_NAME = "fileUpload";

	public static final int PAGE_COUNT = 3;

	public ImportDataController() {
		setAllowDirtyBack(true);
		setAllowDirtyForward(false);
		setPages(new String[] { "projectmanager.dataimport.start",
				"projectmanager.dataimport.map",
				"projectmanager.dataimport.approve" });
		setCommandName(COMMAND_NAME);
		setPageAttribute("pageNumber");
	}

	protected void initBinder(HttpServletRequest request,
			ServletRequestDataBinder binder) throws ServletException {
		binder.registerCustomEditor(String.class,
				new StringMultipartFileEditor());
	}

	@Override
	protected ModelAndView processFinish(HttpServletRequest request,
			HttpServletResponse response, Object command, BindException errors)
			throws Exception {
		FileUploadBean fileUploadBean = (FileUploadBean) command;

		User user = UserInfoPreparationInterceptor.getSessionUser(request);
		FormSubmissionCreator formSubmissionCreator = getFormSubmissionCreator(
				command, user);

		int columnCount = fileUploadBean.getColumnCount();
		FieldSetter[] fieldSetters = new FieldSetter[columnCount];
		for (Map.Entry<String, String> mappingEntry : fileUploadBean
				.getMapping().entrySet()) {
			int columnNumber = Integer.parseInt(mappingEntry.getKey());
			String fieldId = mappingEntry.getValue();
			if (fieldId.length() == 0) {
				fieldSetters[columnNumber] = null;
			} else {
				fieldSetters[columnNumber] = formSubmissionCreator
						.getFieldSetter(fieldId);
			}
		}

		if (fileUploadBean.isDeleteOldData()) {
			deleteOldData(user, command);
		}

		ImportStatistics statistics = DataImporter.importData(fileUploadBean
				.getDataIterator(), fieldSetters, columnCount, fileUploadBean
				.isSkipFirstRow(), formSubmissionCreator);

		ModelAndView result = new ModelAndView(
				"projectmanager.dataimport.confirm");
		result.addObject(COMMAND_NAME, fileUploadBean);
		result.addObject("statistics", statistics);

		return result;
	}

	abstract protected void deleteOldData(User user, Object command);

	protected abstract FormSubmissionCreator getFormSubmissionCreator(
			Object command, User user);

	@Override
	protected ModelAndView processCancel(HttpServletRequest request,
			HttpServletResponse response, Object command, BindException errors)
			throws Exception {

		response.sendRedirect(((FileUploadBean) command).getReturnURL());
		return null;
	}

	@Override
	protected Map referenceData(HttpServletRequest request, Object command,
			Errors errors, int page) throws Exception {
		Map<String, Object> result = new HashMap<String, Object>();
		result.put("totalPageCount", PAGE_COUNT);
		return result;
	}

	private List<List<String>> getSampleData(FileUploadBean fileUploadBean)
			throws IOException {
		int maxDataSampleSize = 5;
		List<List<String>> sampleData = new ArrayList<List<String>>(
				maxDataSampleSize);

		DataIterator dataIterator = fileUploadBean.getDataIterator();
		try {
			for (int i = 0; dataIterator.hasNext() && i < maxDataSampleSize; i++) {
				sampleData.add(dataIterator.next());
			}
		} finally {
			dataIterator.close();
		}
		return sampleData;
	}

	@SuppressWarnings("unchecked")
	@Override
	protected void validatePage(Object command, Errors errors, int page) {
		FileUploadBean fileUploadBean = (FileUploadBean) command;
		try {
			switch (page) {
			case 0:
				if (fileUploadBean.isEmptyFile()) {
					errors.reject("data.import.no.data");
					return;
				}
				if (fileUploadBean.isExcelSpreadsheet()) {
					try {
						HSSFWorkbook workbook = fileUploadBean.getWorkbook();
						if (workbook.getNumberOfSheets() > 1) {
							errors.reject("data.import.more.than.one.sheet");
						}
					} catch (Exception e) {
						errors.reject("data.import.incorrect.excel.file");
					}
				} else {
					try {
						// Crude way of testing CSV file
						getSampleData(fileUploadBean);
					} catch (RuntimeException e) {
						errors.reject("data.import.incorrect.csv.file");
					}
				}
				if (errors.hasErrors()) {
					return;
				}

				fileUploadBean.setSampleData(getSampleData(fileUploadBean));
				fileUploadBean.countColumns();

				if (fileUploadBean.getMapping().size() == 0) {
					// --- Assume the first row is the list of headers and
					// attempt to match
					boolean atLeastOneMatchFound = false;

					List<String> firstRow = fileUploadBean.getSampleData().get(0);

					int columnNumber = -1;
					for (String value : firstRow) {
						++columnNumber;
						if (value.length() == 0) {
							continue;
						}
						for (StoredField field : fileUploadBean.getFields()) {
							if (field.getName().equals(value)) {
								atLeastOneMatchFound = true;
								fileUploadBean.getMapping().put(
										String.valueOf(columnNumber),
										field.getName());
								break;
							}
						}
					}

					fileUploadBean
							.setFirstRowIsLikelyHeaders(atLeastOneMatchFound);
					fileUploadBean.setSkipFirstRow(atLeastOneMatchFound);
				}
				break;

			case 1:
				if (thereAreNoMappingsDefined(fileUploadBean.getMapping())) {
					errors.reject("data.import.no.mapping");
				}

				if (errors.hasErrors()) {
					return;
				}

				Collection<String> duplicateMappings = getDuplicateMappings(fileUploadBean
						.getMapping());
				if (duplicateMappings.size() > 0) {
					if (duplicateMappings.size() == 1) {
						errors.reject("data.import.one.duplicate.field",
								new String[] { duplicateMappings.iterator()
										.next() },
								"Field {0} is selected more than once.");
					} else {
						StringBuilder listOfFields = new StringBuilder();
						for (String field : duplicateMappings) {
							if (listOfFields.length() > 0) {
								listOfFields.append(", ");
							}
							listOfFields.append(field);
						}
						errors.reject("data.import.several.duplicate.fields",
								new String[] { listOfFields.toString() },
								"Fields {0} are selected more than once.");
					}
				}
				// --- This will avoid a message that some headers matched.
				fileUploadBean.setFirstRowIsLikelyHeaders(false);
				break;
			}
		} catch (Exception e) {
			throw new IllegalArgumentException("Error parsing data", e);
		}

		return;
	}

	private static Collection<String> getDuplicateMappings(
			Map<String, String> mapping) {
		Collection<String> result = new LinkedHashSet<String>();
		Set<String> previousFields = new HashSet<String>();
		for (String fieldName : mapping.values()) {
			if (fieldName.length() == 0) {
				continue;
			}
			if (!previousFields.add(fieldName)) {
				result.add(fieldName);
			}
		}
		return result;
	}

	private static boolean thereAreNoMappingsDefined(Map<String, String> mapping) {
		for (String value : mapping.values()) {
			if (value.length() > 0) {
				return false;
			}
		}
		return true;
	}
}
