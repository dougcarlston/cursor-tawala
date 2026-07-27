package com.tawala.component.repository;

import java.util.ArrayList;
import java.util.Collection;
import java.util.LinkedHashSet;
import java.util.List;
import java.util.Locale;
import java.util.Set;

import org.dom4j.Document;
import org.dom4j.DocumentHelper;
import org.dom4j.Element;
import org.dom4j.Namespace;
import org.dom4j.QName;
import org.springframework.context.MessageSource;

import com.scissor.Log;
import com.scissor.xmlconfig.ConfigElement;
import com.scissor.xmlconfig.Factory;
import com.tawala.component.ComponentMetadata;
import com.tawala.component.function.Function;
import com.tawala.component.function.FunctionMetadata;
import com.tawala.component.function.PopularChoiceCountFunction;
import com.tawala.component.function.ProjectEmailCountFunction;
import com.tawala.component.function.RecordCountFunction;
import com.tawala.component.function.SumFunction;
import com.tawala.component.validator.DollarAmountValidatorMetadata;
import com.tawala.component.validator.EmailValidatorMetadata;
import com.tawala.component.validator.FieldValidator;
import com.tawala.component.validator.FIBValidatorMetadata;
import com.tawala.component.validator.IntegerRangeValidatorMetadata;
import com.tawala.component.validator.PhoneNumberValidatorMetadata;
import com.tawala.component.validator.ProperValidatorMetadata;
import com.tawala.component.validator.USStateValidatorMetadata;
import com.tawala.component.validator.ZIPCodeValidatorMetadata;
import com.tawala.component.web.WebComponentMetadata;
import com.tawala.component.web.display.Categorizer;
import com.tawala.component.web.display.ChoiceTallyTable;
import com.tawala.component.web.display.DisplayImage;
import com.tawala.component.web.display.DisplayMultipleChoiceLabel;
import com.tawala.component.web.display.LinkToProjectDetails;
import com.tawala.component.web.display.PrepareTeamRoster;
import com.tawala.component.web.display.ResponseTotalsTable;
import com.tawala.component.web.display.DisplayPopularChoice;
import com.tawala.component.web.display.ItemizationTable;
import com.tawala.component.web.display.PayPalSingleItemButton;
import com.tawala.component.web.display.PopularChoiceTable;
import com.tawala.component.web.display.QuestionCorrelationTable;
import com.tawala.component.web.display.SimpleList;
import com.tawala.component.web.form.DynamicMultiChoiceDataProvider;
import com.tawala.project.FormRenderable;

public class Repository {
	public static final String REPOSITORY_IS_CURRENT_ROOT_ELEMENT_NAME = "component-repository-is-current";
	public static final String ROOT_ELEMENT_NAME = "component-repository";
	public static final Namespace NAMESPACE = new Namespace("tr",
			"http://www.tawala.com/component-repository");
	private static MessageSource messageSource;
	public static Locale DEFAULT_LOCALE = Locale.US;

	private static String XML_PRESENTATION;
	private static String XML_PRESENTATION_SIGNATURE;

	private static Collection<Category<WebComponentMetadata>> TOP_LEVEL_WEB_COMPONENT_CATEGORIES = new ArrayList<Category<WebComponentMetadata>>();
	private static Collection<Category<FunctionMetadata>> TOP_LEVEL_FUNCTION_CATEGORIES = new ArrayList<Category<FunctionMetadata>>();
	private static Collection<Category<ComponentMetadata>> TOP_LEVEL_CATEGORIES = new ArrayList<Category<ComponentMetadata>>();
	private static Collection<FIBValidatorMetadata> FIB_VALIDATORS = new LinkedHashSet<FIBValidatorMetadata>();
	private static Factory<FieldValidator> FIB_VALIDATOR_FACTORY = new Factory<FieldValidator>();

	private static final String REPOSITORY_IS_CURRENT_XML = getRepositoryIsCurrentXML();

	public void setMessageSource(MessageSource messageSource) {
		Log.info(this, "Setup message source " + messageSource);
		Repository.messageSource = messageSource;
	}

	public static MessageSource getMessageSource() {
		if (messageSource == null) {
			throw new IllegalStateException("Message source is not set.");
		}
		return messageSource;
	}

	public static Locale getDefaultLocale() {
		return DEFAULT_LOCALE;
	}

	public static String getXMLPresentation() {
		if (XML_PRESENTATION == null) {
			try {
				buildXmlPresentation(DEFAULT_LOCALE);
			} catch (Exception e) {
				Log
						.error(
								Repository.class,
								"Failed to build XML presentation of the component repository:",
								e);
			}
		}
		return XML_PRESENTATION;
	}

	public static String getSignature(Locale locale) {
		if (XML_PRESENTATION_SIGNATURE == null)
			buildXmlPresentation(locale);
		return XML_PRESENTATION_SIGNATURE;
	}

	// ------------------ Web Components ----------------------
	public static Category<WebComponentMetadata> WEB_CATEGORY_ALL = new Category<WebComponentMetadata>(
			"category.web.all.name", new WebComponentMetadata[] {
					new SimpleList(), new ItemizationTable(),
					new ChoiceTallyTable(), new ResponseTotalsTable(),
					new PopularChoiceTable(), new DisplayPopularChoice(),
					new QuestionCorrelationTable(),
					new PayPalSingleItemButton(), new Categorizer(),
					new DynamicMultiChoiceDataProvider(),
					new DisplayMultipleChoiceLabel(),
					new LinkToProjectDetails(), new DisplayImage(),
					new PrepareTeamRoster() });

	// ------------------ Functions ----------------------
	public static Category<FunctionMetadata> FUNCTION_CATEGORY_ALL = new Category<FunctionMetadata>(
			"category.function.all.name", new FunctionMetadata[] {
					new SumFunction(), new RecordCountFunction(),
					new PopularChoiceCountFunction(),
					new ProjectEmailCountFunction() });

	// ------------------ "All" Category ----------------------
	public static Category<ComponentMetadata> CATEGORY_ALL = new Category<ComponentMetadata>(
			"category.all.name", new ComponentMetadata[] { new SimpleList(),
					new ItemizationTable(), new ChoiceTallyTable(),
					new ResponseTotalsTable(), new PopularChoiceTable(),
					new DisplayPopularChoice(), new SumFunction(),
					new RecordCountFunction(),
					new PopularChoiceCountFunction(),
					new QuestionCorrelationTable(),
					new PayPalSingleItemButton(), new Categorizer(),
					new DisplayMultipleChoiceLabel(),
					new LinkToProjectDetails(), new DisplayImage(),
					new ProjectEmailCountFunction(), new PrepareTeamRoster() });

	// ------------------ Tables Category ----------------------
	public static Category<ComponentMetadata> CATEGORY_TABLES = new Category<ComponentMetadata>(
			"category.tables.name", new ComponentMetadata[] {
					new ItemizationTable(), new ChoiceTallyTable(),
					new ResponseTotalsTable(), new PopularChoiceTable(),
					new SimpleList(), new QuestionCorrelationTable(),
					new Categorizer() });

	// ------------------ Math Category ----------------------
	public static Category<ComponentMetadata> CATEGORY_MATH = new Category<ComponentMetadata>(
			"category.math.name", new ComponentMetadata[] { new SumFunction() });

	// ------------------ Database Category ----------------------
	public static Category<ComponentMetadata> CATEGORY_DATABASE = new Category<ComponentMetadata>(
			"category.database.name", new ComponentMetadata[] {
					new DisplayPopularChoice(), new RecordCountFunction(),
					new PopularChoiceCountFunction() });

	// ------------------ Payments Category ----------------------
	public static Category<ComponentMetadata> CATEGORY_PAYMENTS = new Category<ComponentMetadata>(
			"category.payments.name",
			new ComponentMetadata[] { new PayPalSingleItemButton() });

	static {
		// --- Web Components
		TOP_LEVEL_WEB_COMPONENT_CATEGORIES.add(WEB_CATEGORY_ALL);

		// --- Functions
		TOP_LEVEL_FUNCTION_CATEGORIES.add(FUNCTION_CATEGORY_ALL);

		// --- All Category
		TOP_LEVEL_CATEGORIES.add(CATEGORY_ALL);

		// --- Tables Category
		TOP_LEVEL_CATEGORIES.add(CATEGORY_TABLES);

		// --- Math Category
		TOP_LEVEL_CATEGORIES.add(CATEGORY_MATH);

		// --- Database Category
		TOP_LEVEL_CATEGORIES.add(CATEGORY_DATABASE);

		// --- Payments Category
		TOP_LEVEL_CATEGORIES.add(CATEGORY_PAYMENTS);
	}

	// ----------------- FIB Validators --------------------------------
	static {
		FIB_VALIDATORS.add(new EmailValidatorMetadata());
		FIB_VALIDATORS.add(new PhoneNumberValidatorMetadata());
		FIB_VALIDATORS.add(new IntegerRangeValidatorMetadata());
		FIB_VALIDATORS.add(new USStateValidatorMetadata());
		FIB_VALIDATORS.add(new ZIPCodeValidatorMetadata());
		FIB_VALIDATORS.add(new ProperValidatorMetadata());
		FIB_VALIDATORS.add(new DollarAmountValidatorMetadata());

		for (FIBValidatorMetadata fibValidatorMetadata : FIB_VALIDATORS) {
			FIB_VALIDATOR_FACTORY.register(fibValidatorMetadata.getId(),
					fibValidatorMetadata.getFibValidator());
		}
	}

	private static <T extends ComponentMetadata> Set<T> getComponents(
			Collection<Category<T>> topLevelCategories) {
		Set<T> result = new LinkedHashSet<T>();

		for (Category<T> category : topLevelCategories) {
			result.addAll(category.getAllComponents());
		}
		return result;
	}

	private synchronized static void buildXmlPresentation(Locale locale) {
		Document document = DocumentHelper.createDocument();
		Element root = document.addElement(new QName(ROOT_ELEMENT_NAME,
				NAMESPACE));

		addCategories(locale, root, "categories", TOP_LEVEL_CATEGORIES);

		addComponentGroup(locale, root, "display-component",
				TOP_LEVEL_WEB_COMPONENT_CATEGORIES);
		addComponentGroup(locale, root, "function",
				TOP_LEVEL_FUNCTION_CATEGORIES);

		addComponents(locale, root, "blank-validator", FIB_VALIDATORS);

		XML_PRESENTATION_SIGNATURE = String
				.valueOf(document.asXML().hashCode());

		root.addAttribute(new QName("signature"), XML_PRESENTATION_SIGNATURE);

		XML_PRESENTATION = document.asXML();
	}

	@SuppressWarnings("unchecked")
	private static void addCategories(Locale locale, Element root,
			String categoryElementName, Collection componentCategories) {

		Element categories = root.addElement(new QName(categoryElementName,
				NAMESPACE));

		for (Category<ComponentMetadata> category : (Collection<Category<ComponentMetadata>>) componentCategories) {
			categories.add(category.toElement(locale));
		}
	}

	@SuppressWarnings("unchecked")
	private static void addComponentGroup(
			Locale locale,
			Element root,
			String componentElementName,
			Collection<? extends Category<? extends ComponentMetadata>> componentCategories) {

		Set<ComponentMetadata> components = new LinkedHashSet<ComponentMetadata>();

		for (Category<ComponentMetadata> category : (Collection<Category<ComponentMetadata>>) componentCategories) {
			components.addAll(category.getAllComponents());
		}

		for (ComponentMetadata component : components) {
			root.add(component.toElement(componentElementName, locale));
		}
	}

	private static void addComponents(Locale locale, Element root,
			String componentElementName,
			Collection<? extends ComponentMetadata> components) {

		for (ComponentMetadata component : components) {
			root.add(component.toElement(componentElementName, locale));
		}
	}

	public static void registerWebComponentsWith(Factory<FormRenderable> factory) {
		for (WebComponentMetadata webComponent : getComponents(TOP_LEVEL_WEB_COMPONENT_CATEGORIES)) {
			factory.register(webComponent.getId(), webComponent
					.getRuntimeProcessingClass());
		}
	}

	public static void registerFunctionsWith(Factory<Function> factory) {
		for (FunctionMetadata functionMetadata : getComponents(TOP_LEVEL_FUNCTION_CATEGORIES)) {
			factory.register(functionMetadata.getId(), functionMetadata
					.getRuntimeClass());
		}
	}

	public static Set<FunctionMetadata> getAllFunctionMetadata() {
		return getComponents(TOP_LEVEL_FUNCTION_CATEGORIES);
	}

	public static String getXMLPresentation(String clientSignature) {
		if (clientSignature == null
				|| !clientSignature.equals(getSignature(DEFAULT_LOCALE))) {
			return getXMLPresentation();
		} else {
			return REPOSITORY_IS_CURRENT_XML;
		}
	}

	private static String getRepositoryIsCurrentXML() {
		Document document = DocumentHelper.createDocument();
		document.addElement(new QName(REPOSITORY_IS_CURRENT_ROOT_ELEMENT_NAME,
				NAMESPACE));
		return document.asXML();
	}

	public static List<FieldValidator> instantiateFIBValidators(
			ConfigElement configElement) {
		if (configElement == null) {
			return null;
		}
		return FIB_VALIDATOR_FACTORY.makeChildren(configElement);
	}
}
