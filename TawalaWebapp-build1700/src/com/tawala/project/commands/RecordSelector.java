package com.tawala.project.commands;

import java.util.ArrayList;
import java.util.Collections;
import java.util.HashMap;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

import com.scissor.Log;
import com.scissor.xmlconfig.ConfigElement;
import com.scissor.xmlconfig.Factory;
import com.tawala.project.CompositeFormSubmission;
import com.tawala.project.Form;
import com.tawala.project.FormSubmission;
import com.tawala.project.Project;

public class RecordSelector {
	public static final String EXTERNAL_SHARED_DATA_ATTRIBUTE_NAME = "externalSharedData";
	public static Factory<FormDataProvider> FORM_DATA_PROVIDER_FACTORY = new Factory<FormDataProvider>();
	static {
		FORM_DATA_PROVIDER_FACTORY.register("form",
				EXTERNAL_SHARED_DATA_ATTRIBUTE_NAME, "true",
				SharedStorageFormDataProvider.class);
		FORM_DATA_PROVIDER_FACTORY.register("form",
				CurrentProjectFormDataProvider.class);
	}

	private final List<FormDataProvider> formDataProviders;
	private final Map<String, FormDataProvider> formDataProviderMap;
	private final BooleanExpression condition;
	private final String recordListName;

	public static final String DEFAULT_RECORD_LIST_NAME = "Record";

	public static RecordSelector instantiateFrom(ConfigElement configElement) {
		if (configElement == null) {
			return null;
		}
		List<FormDataProvider> formDataProviders = FORM_DATA_PROVIDER_FACTORY
				.makeChildren(configElement.children("form"));

		ConfigElement conditionElement = configElement.child("conditions");

		BooleanExpression condition = conditionElement == null
				|| conditionElement.childElements().size() == 0 ? BooleanExpression.TRUE
				: BooleanExpression.load(conditionElement.childElement(0));

		return new RecordSelector(formDataProviders, condition,
				DEFAULT_RECORD_LIST_NAME);
	}

	@Override
	public int hashCode() {
		return formDataProviders.hashCode() + recordListName.hashCode()
				+ condition.hashCode();
	}

	@Override
	public boolean equals(Object obj) {
		if (obj.getClass() != RecordSelector.class) {
			return false;
		}

		RecordSelector other = (RecordSelector) obj;
		return this.recordListName.equals(other.recordListName)
				&& this.formDataProviders.equals(other.formDataProviders)
				&& this.condition.equals(other.condition);
	}

	public RecordSelector(List<FormDataProvider> formDataProviders,
			BooleanExpression condition, String recordListName) {
		this.formDataProviders = formDataProviders;
		this.condition = condition == null ? BooleanExpression.TRUE : condition;
		this.recordListName = recordListName;
		this.formDataProviderMap = new HashMap<String, FormDataProvider>(
				formDataProviders.size());
		for (FormDataProvider provider : formDataProviders) {
			formDataProviderMap.put(provider.getFormName(), provider);
		}
	}

	public List<CompositeFormSubmission> getRecords(ExecutionContext context) {
		if (formDataProviders.size() == 0) {
			return null;
		}

		Map<FormDataProvider, FormDataProvider> translatedDataProviders = translateDataProvidersInCaseOfSharedDataReferences(context);

		List<List<FormSubmission>> allFormData = new ArrayList<List<FormSubmission>>(
				formDataProviders.size());

		// Retrieve all data. Currently there is no indexing; we can't limit the
		// sets of rows to retrieve without scanning them in memory
		int i = 0;
		for (Map.Entry<FormDataProvider, FormDataProvider> providers : translatedDataProviders
				.entrySet()) {
			FormDataProvider translatedDataProvider = providers.getValue();

			List<FormSubmission> allResponsesForForm = translatedDataProvider
					.getAllSubmissions(context);
			if (allResponsesForForm.size() == 0) {
				// Short circuit the execution if at least one data set is empty
				// - no data is going to be produced in this case.
				return null;
			}

			FormDataProvider originalDataProvider = providers.getKey();
			String formName = originalDataProvider.getFormName();
			if (!translatedDataProvider.getFormName().equals(formName)) {
				adjustResponsesToBeKnownUnderAnotherFormName(
						allResponsesForForm, formName);
			}

			allFormData.add(i++, allResponsesForForm);
		}

		int[] currentPositions = new int[formDataProviders.size()];
		// Set the position to "-1" to ensure correct iterations. See
		// getNextCandidate() for details.
		currentPositions[allFormData.size() - 1] = -1;

		context.setEvaluatingWhereClause(true);

		List<CompositeFormSubmission> result = new ArrayList<CompositeFormSubmission>();
		CompositeFormSubmission candidateSubmission = null;
		while ((candidateSubmission = getNextCandidate(allFormData,
				currentPositions, recordListName)) != null) {
			context.setCurrentWhereClauseCandidate(candidateSubmission);

			// --- This is needed to properly resolve references.
			context.mapRecord(recordListName, candidateSubmission);
			if (condition.isTrue(context)) {
				result.add(candidateSubmission);
			}
		}

		context.removeRecordMapping(recordListName);
		context.setEvaluatingWhereClause(false);
		context.setCurrentWhereClauseCandidate(null);
		return result;
	}

	public long getRecordCount(ExecutionContext context) {
		if (formDataProviders.size() == 1
				&& condition == BooleanExpression.TRUE) {
			return formDataProviders.get(0).getRecordCount(context);
		}

		List<CompositeFormSubmission> results = getRecords(context);
		return results == null ? 0 : results.size();
	}

	private void adjustResponsesToBeKnownUnderAnotherFormName(
			List<FormSubmission> allResponsesForForm, String formName) {
		for (FormSubmission submission : allResponsesForForm) {
			submission.setFormName(formName);
		}
	}

	private Map<FormDataProvider, FormDataProvider> translateDataProvidersInCaseOfSharedDataReferences(
			ExecutionContext context) {
		Map<FormDataProvider, FormDataProvider> translatedDataProviders = new LinkedHashMap<FormDataProvider, FormDataProvider>(
				formDataProviders.size());

		for (FormDataProvider originalDataProvider : formDataProviders) {
			FormDataProvider translatedDataProvider = originalDataProvider;

			if (originalDataProvider.canBeShared()) {
				Form form = context.getProject().getForm(
						originalDataProvider.getFormName());
				if (form == null) {
					Log.warn(this, "Unable to find form '"
							+ originalDataProvider.getFormName()
							+ " for project #" + context.getProject().getId());
				} else if (form.isSharedData()) {
					translatedDataProvider = new SharedStorageFormDataProvider(
							form.getDataSourceName());
				}
			}

			translatedDataProviders.put(originalDataProvider,
					translatedDataProvider);
		}
		return translatedDataProviders;
	}

	private CompositeFormSubmission getNextCandidate(
			List<List<FormSubmission>> allFormData, int[] currentPositions,
			String recordListName) {
		// Move to the next record
		int dataSetToAdvance = allFormData.size() - 1;

		while (dataSetToAdvance >= 0) {
			if (++currentPositions[dataSetToAdvance] >= allFormData.get(
					dataSetToAdvance).size()) {
				currentPositions[dataSetToAdvance] = 0;
				--dataSetToAdvance;
			} else {
				break;
			}
		}

		// Test for the end of iteration
		if (dataSetToAdvance < 0)
			return null;

		CompositeFormSubmission result = new CompositeFormSubmission(
				recordListName);
		for (int i = 0; i < allFormData.size(); i++) {
			result.add(allFormData.get(i).get(currentPositions[i]));
		}
		return result;
	}

	public static interface FormDataProvider {
		List<FormSubmission> getAllSubmissions(ExecutionContext context);

		String getFormName();

		Form getForm(ExecutionContext context);

		boolean canBeShared();

		long getRecordCount(ExecutionContext context);
	}

	private static class FormDataProviderSupport {
		private final String formName;

		protected FormDataProviderSupport(ConfigElement configElement) {
			this.formName = configElement.attribute("name").stringValue();
		}

		protected FormDataProviderSupport(String formName) {
			this.formName = formName;
		}

		public String getFormName() {
			return formName;
		}

		@Override
		public int hashCode() {
			return formName.hashCode();
		}

		@Override
		public boolean equals(Object obj) {
			if (this == obj)
				return true;
			if (obj == null)
				return false;
			if (getClass() != obj.getClass())
				return false;
			final FormDataProviderSupport other = (FormDataProviderSupport) obj;
			if (formName == null) {
				if (other.formName != null)
					return false;
			} else if (!formName.equals(other.formName))
				return false;
			return true;
		}
	}

	public static class CurrentProjectFormDataProvider extends
			FormDataProviderSupport implements FormDataProvider {
		public CurrentProjectFormDataProvider(ConfigElement configElement) {
			super(configElement);
		}

		public CurrentProjectFormDataProvider(String formName) {
			super(formName);
		}

		public List<FormSubmission> getAllSubmissions(ExecutionContext context) {
			return context.getDomain().storedData().responsesFor(
					context.getProject(), getFormName());
		}

		public long getRecordCount(ExecutionContext context) {
			return context.getDomain().storedData().responseCount(
					context.getProject(), getFormName());
		}

		public boolean canBeShared() {
			return true;
		}

		public Form getForm(ExecutionContext context) {
			return context.getProject().getForm(getFormName());
		}
	}

	public static class SharedStorageFormDataProvider extends
			FormDataProviderSupport implements FormDataProvider {
		public SharedStorageFormDataProvider(ConfigElement configElement) {
			super(configElement);
		}

		public SharedStorageFormDataProvider(String formName) {
			super(formName);
		}

		@SuppressWarnings("unchecked")
		public List<FormSubmission> getAllSubmissions(ExecutionContext context) {
			Project sharedStorageProject = context.getUserProject().getUser()
					.getSharedStorage();
			if (sharedStorageProject == null) {
				return Collections.EMPTY_LIST;
			}
			return context.getDomain().storedData().responsesFor(
					sharedStorageProject, getFormName());
		}

		public long getRecordCount(ExecutionContext context) {
			Project sharedStorageProject = context.getUserProject().getUser()
					.getSharedStorage();
			if (sharedStorageProject == null) {
				return 0;
			}
			return context.getDomain().storedData().responseCount(
					sharedStorageProject, getFormName());
		}

		public boolean canBeShared() {
			return false;
		}

		public Form getForm(ExecutionContext context) {
			// --- Shared datasources don't have any forms.
			return null;
			// return
			// context.getUserProject().getUser().getSharedStorage().getForm(getFormName());
		}

	}

	public Form getForm(ExecutionContext context, String formName) {
		FormDataProvider dataProvider = formDataProviderMap.get(formName);
		return dataProvider == null ? null : dataProvider.getForm(context);
	}

	public void removeFormDataProvidersOtherThan(String formName) {
		List<FormDataProvider> providersToRemove = new ArrayList<FormDataProvider>();
		for (FormDataProvider provider : formDataProviders) {
			if (!provider.getFormName().equals(formName)) {
				providersToRemove.add(provider);
			}
		}
		for (FormDataProvider provider : providersToRemove) {
			formDataProviders.remove(provider);
			formDataProviderMap.remove(provider.getFormName());
		}
	}

	// --- TODO: this is too simplistic. A more complex analysis with a walk
	// down the condition tree is needed.
	public boolean hasConditionsDependentOnExecutionContext() {
		return condition != BooleanExpression.TRUE;
	}
}
