package com.tawala.project.theme;

import java.io.StringReader;
import java.util.Map;
import java.util.TreeMap;

import org.json.JSONException;
import org.json.JSONObject;
import org.w3c.css.sac.AttributeCondition;
import org.w3c.css.sac.CSSException;
import org.w3c.css.sac.CombinatorCondition;
import org.w3c.css.sac.Condition;
import org.w3c.css.sac.ConditionalSelector;
import org.w3c.css.sac.DescendantSelector;
import org.w3c.css.sac.DocumentHandler;
import org.w3c.css.sac.ElementSelector;
import org.w3c.css.sac.InputSource;
import org.w3c.css.sac.LexicalUnit;
import org.w3c.css.sac.Parser;
import org.w3c.css.sac.SACMediaList;
import org.w3c.css.sac.Selector;
import org.w3c.css.sac.SelectorList;
import org.w3c.css.sac.SiblingSelector;
import org.w3c.css.sac.helpers.ParserFactory;

import com.scissor.Log;

public class CSSDocumentHandler implements DocumentHandler {
	static {
		System.setProperty("org.w3c.css.sac.parser",
				org.w3c.flute.parser.Parser.class.getName());
	}

	private static ParserFactory parserFactory = new ParserFactory();

	private Map<String, Map<String, String>> selectorsMap = new TreeMap<String, Map<String, String>>();
	private Map<String, String> currentSelector;

	public Map<String, Map<String, String>> getSelectorsMap() {
		return selectorsMap;
	}

	public void comment(String arg0) throws CSSException {
		// -- Ignore
	}

	public void endDocument(InputSource arg0) throws CSSException {
		// -- Ignore

	}

	public void endFontFace() throws CSSException {
		// -- Ignore

	}

	public void endMedia(SACMediaList arg0) throws CSSException {
		// -- Ignore

	}

	public void endPage(String arg0, String arg1) throws CSSException {
		// -- Ignore

	}

	public void endSelector(SelectorList arg0) throws CSSException {
		// TODO Auto-generated method stub

	}

	public void ignorableAtRule(String arg0) throws CSSException {
		// TODO Auto-generated method stub

	}

	public void importStyle(String arg0, SACMediaList arg1, String arg2)
			throws CSSException {
		// -- Ignore

	}

	public void namespaceDeclaration(String arg0, String arg1)
			throws CSSException {
		// TODO Auto-generated method stub

	}

	public void property(String name, LexicalUnit lexicalUnit, boolean important)
			throws CSSException {
		if (currentSelector != null) {
			StringBuilder value = new StringBuilder();

			do {
				addToValue(lexicalUnit, value);
				lexicalUnit = lexicalUnit.getNextLexicalUnit();
			} while (lexicalUnit != null);

			currentSelector.put(name, value.toString());
		}

	}

	private void addToValue(LexicalUnit lexicalUnit, StringBuilder value) {
		String attributeValue = lexicalUnit.getStringValue();
		switch (lexicalUnit.getLexicalUnitType()) {
		case LexicalUnit.SAC_OPERATOR_COMMA:
			value.append(", ");
			break;

		case LexicalUnit.SAC_ATTR:
		case LexicalUnit.SAC_IDENT:
			value.append(attributeValue);
			break;

		case LexicalUnit.SAC_RGBCOLOR:
			int r = lexicalUnit.getParameters().getIntegerValue();
			int g = lexicalUnit.getParameters().getNextLexicalUnit()
					.getNextLexicalUnit().getIntegerValue();
			int b = lexicalUnit.getParameters().getNextLexicalUnit()
					.getNextLexicalUnit().getNextLexicalUnit()
					.getNextLexicalUnit().getIntegerValue();
			value.append('#').append(toHexString(r)).append(toHexString(g))
					.append(toHexString(b));
			break;

		case LexicalUnit.SAC_STRING_VALUE:
			value.append(quoteAttributeValue(attributeValue));
			break;
			
		default:
			value.append(lexicalUnit.toString());
			break;
		}
	}

	private static String quoteAttributeValue(String value) {
		if(value.indexOf(' ') > -1) {
			return "'" + value + "'";
		} else {
			return value;
		}
	}
	
	private String toHexString(int r) {
		String result = Integer.toHexString(r);
		return result.length() == 1 ? ('0' + result.toUpperCase()) : result
				.toUpperCase();
	}

	public void startDocument(InputSource arg0) throws CSSException {
		// -- Ignore

	}

	public void startFontFace() throws CSSException {
		// -- Ignore
	}

	public void startMedia(SACMediaList arg0) throws CSSException {
		// -- Ignore
	}

	public void startPage(String arg0, String arg1) throws CSSException {
		// -- Ignore
	}

	public void startSelector(SelectorList selectorList) throws CSSException {
		StringBuilder selectorNameBuilder = new StringBuilder();

		for (int i = 0; i < selectorList.getLength(); i++) {
			if (selectorNameBuilder.length() > 0) {
				selectorNameBuilder.append(' ');
			}

			Selector selector = selectorList.item(i);
			buildSelectorName(selector, selectorNameBuilder);
		}

		if (selectorNameBuilder.length() > 0) {
			String key = selectorNameBuilder.toString();
			currentSelector = selectorsMap.get(key);
			if(currentSelector == null) {
				currentSelector = new TreeMap<String, String>();
				selectorsMap.put(key, currentSelector);
			}
		} else {
			currentSelector = null;
		}
	}

	private void buildSelectorName(Selector selector,
			StringBuilder selectorBuilder) {
		switch (selector.getSelectorType()) {
		case Selector.SAC_ELEMENT_NODE_SELECTOR:
			ElementSelector elementSelector = (ElementSelector) selector;
			selectorBuilder.append(elementSelector.getLocalName());
			return;

		case Selector.SAC_CONDITIONAL_SELECTOR:
			ConditionalSelector conditionalSelector = (ConditionalSelector) selector;

			buildSelectorName(conditionalSelector.getSimpleSelector(),
					selectorBuilder);
			buildSelectorName(conditionalSelector.getCondition(),
					selectorBuilder);

			return;

		case Selector.SAC_DESCENDANT_SELECTOR:
			DescendantSelector descendantSelector = (DescendantSelector) selector;
			buildSelectorName(descendantSelector.getAncestorSelector(),
					selectorBuilder);
			selectorBuilder.append(' ');
			buildSelectorName(descendantSelector.getSimpleSelector(),
					selectorBuilder);
			return;
			
		case Selector.SAC_DIRECT_ADJACENT_SELECTOR:
			SiblingSelector siblingSelector = (SiblingSelector)selector;
			buildSelectorName(siblingSelector.getSelector(), selectorBuilder);
			selectorBuilder.append('+');
			buildSelectorName(siblingSelector.getSiblingSelector(), selectorBuilder);
			return;

		default:
			Log.warn(this, "Unhandled selector: " + selector);
		}
	}

	private void buildSelectorName(Condition condition,
			StringBuilder selectorBuilder) {
		switch (condition.getConditionType()) {
		case Condition.SAC_CLASS_CONDITION:
			selectorBuilder.append('.').append(
					((AttributeCondition) condition).getValue());
			return;

		case Condition.SAC_ID_CONDITION:
			selectorBuilder.append('#').append(
					((AttributeCondition) condition).getValue());
			return;

		case Condition.SAC_AND_CONDITION:
			CombinatorCondition combinatorCondition = (CombinatorCondition) condition;
			buildSelectorName(combinatorCondition.getFirstCondition(),
					selectorBuilder);
			buildSelectorName(combinatorCondition.getSecondCondition(),
					selectorBuilder);
			return;

		case Condition.SAC_PSEUDO_CLASS_CONDITION:
			selectorBuilder.append(':').append(
					((AttributeCondition) condition).getValue());
			return;

		case Condition.SAC_ATTRIBUTE_CONDITION:
			selectorBuilder.append('[').append(
					((AttributeCondition) condition).getLocalName()).append(']');
			return;
			
		default:
			Log.warn(this, "Unhandled condition: " + condition);
		}
	}

	public static JSONObject convertStyleDetailsToJSONObject(Map<String, Map<String, String>> styleDetails) throws JSONException {
		JSONObject result = new JSONObject();
		for (Map.Entry<String, Map<String, String>> styleEntry : styleDetails
				.entrySet()) {
			JSONObject styleDefintion = new JSONObject();
			for (Map.Entry<String, String> attributeEntry : styleEntry
					.getValue().entrySet()) {
				styleDefintion.put(attributeEntry.getKey(), attributeEntry
						.getValue());
			}
			result.put(styleEntry.getKey(), styleDefintion);
		}
	
		return result;
	}

	public static Map<String, Map<String, String>> parseCSS(String css)
			throws Exception {
		CSSDocumentHandler handler = new CSSDocumentHandler();

		Parser parser = parserFactory.makeParser();
		parser.setDocumentHandler(handler);

		StringReader reader = new StringReader(css);
		try {
			InputSource inputSource = new InputSource(reader);
			parser.parseStyleSheet(inputSource);
		} finally {
			reader.close();
		}

		return handler.getSelectorsMap();
	}
	
	public static JSONObject parseCSSAndReturnJSONObject(String css) throws Exception {
		return convertStyleDetailsToJSONObject(parseCSS(css));
	}
}
