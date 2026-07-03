// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Tawala.Functions.Runtime;
using Tawala.Projects.Forms;
using Tawala.Projects.Forms.FormItemContents;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.XmlSupport;

namespace Tawala.Projects.Function
{
    public class XmlToFunctionConverter
    {
        private static readonly Record recordForQualifiedFields = new Record(FieldUtil.DefaultRecordQualifierName);
        private static SimpleLookupTableWithDefault<Type, parse> parameterParseMap;
        private readonly IFunctionRepository functionAssembly;
        private IFunction function;
        private IFunctionInfo functionInfo;
        private IXmlElement functionXml;
        private string functionXmlId;

        public XmlToFunctionConverter()
        {
            if (FunctionLoader.Current == null)
            {
                throw new InvalidOperationException("FunctionLoader.Current is null.");
            }

            functionAssembly = FunctionLoader.Current;
            initializeMap();
        }

        public IFunction ConvertFrom(IXmlElement element)
        {
            recreate(element);
            return function;
        }

        public IFunction CloneFunction(IFunction instance)
        {
            recreate(new XmlElement(instance.ToXml()));
            return function;
        }

        private void recreate(IXmlElement element)
        {
            function = null;
            functionInfo = null;
            functionXml = element;
            functionXmlId = functionXml.Name;

            if (functionAssembly.Functions.Contains(functionXmlId))
            {
                functionInfo = functionAssembly.Functions[functionXmlId];
                function = functionInfo.CreateInstance();

                getParameters();
            }
        }

        private void getParameters()
        {
            if (function.Info.Id == "itemization-table" || function.Info.Id == "categorizer")
            {
                handleCompositeParametersAsSpecialCase();
            }
            else
            {
                Collection<XmlElement> parameters = functionXml.GetChildren();
                foreach (IXmlElement parameter in parameters)
                {
                    string id = parameter.Name;

                    try
                    {
                        IParameterInfo pInfo = functionInfo.Parameters[id];
                        pInfo.SetValue(function, parameterParseMap[pInfo.PropertyType](parameter));
                    }
                    catch
                    {
                        Console.WriteLine("Parameter ID \"{0}\" not found for function \"{1}\".", id, function.Info.Id);
                    }
                }
            }
        }

        private void handleCompositeParametersAsSpecialCase()
        {
            IParameterInfo compositeCollection = function.Info.Parameters["column"];
            var collection = Activator.CreateInstance(compositeCollection.PropertyType) as ICompositeParameterCollection;

            Collection<XmlElement> parameters = functionXml.GetChildren();
            foreach (IXmlElement parameter in parameters)
            {
                string id = parameter.Name;
                if (id.Equals("number-of-columns"))
                {
                    // for now, get this value by counting the columns - jdf 4/10/07

                    //IParameterInfo pInfo = functionInfo.Parameters[id];
                    //pInfo.SetValue(function, parameterParseMap[pInfo.PropertyType](parameter));
                }
                else if (id.Equals("column"))
                {
                    ICompositeParameter composite = collection.CreateItem();

                    Collection<XmlElement> subParameters = parameter.GetChildren();
                    foreach (IXmlElement subParameter in subParameters)
                    {
                        switch (subParameter.Name)
                        {
                            case "header":
                            {
                                IXmlElement element = getHeaderElement(subParameter);
                                var expression = new FunctionCompoundExpression(element);
                                composite.GetType().GetProperty("Header").SetValue(composite, expression, null);
                                break;
                            }
                            case "contents":
                            {
                                IPaletteField field = getField(subParameter.GetChild("field").GetAttribute("name"));
                                var contents = new FunctionContentsField(field);
                                composite.GetType().GetProperty("Contents").SetValue(composite, contents, null);
                                break;
                            }
                            case "display-conditions":
                            {
                                var displayConditions = new Conditions(subParameter, FieldUtil.GetGlobalFieldList(subParameter));
                                composite.GetType().GetProperty("DisplayConditions").SetValue(composite, displayConditions, null);
                                break;
                            }
                        }
                    }

                    collection.Add(composite);
                }
                else
                {
                    if (collection.Count > 0)
                    {
                        compositeCollection.SetValue(function, collection);
                    }

                    IParameterInfo pInfo = functionInfo.Parameters[id];
                    pInfo.SetValue(function, parameterParseMap[pInfo.PropertyType](parameter));
                }
            }

            functionInfo.Parameters["number-of-columns"].SetValue(function, collection.Count);
        }

        private static IXmlElement getHeaderElement(IXmlElement subParameter)
        {
            string xmlString;

            if (containsOnlyText(subParameter))
            {
                xmlString = string.Format(@"<container><string value=""{0}""/></container>", subParameter.Text);
            }
            else
            {
                xmlString = subParameter.OuterXml;
            }

            return new XmlElement(xmlString);
        }

        private static bool containsOnlyText(IXmlElement subParameter)
        {
            return subParameter.Text != null;
        }

        private IPaletteField getField(string fullFieldName)
        {
            IPaletteField field = PaletteField.NULL;

            string formName = FieldUtil.GetFormName(fullFieldName);
            string fieldName = FieldUtil.GetFieldName(fullFieldName);

            if (FieldUtil.IsUnknownField(fullFieldName))
            {
                field = new Field(fullFieldName);
            }
            else if (FieldUtil.IsFormField(fullFieldName) || FieldUtil.IsExternalField(fullFieldName))
            {
                field = (IPaletteField)Project.Current.AllForms[formName].ItemList[fieldName];
            }
            else if (FieldUtil.IsRecordField(fullFieldName) || FieldUtil.IsExternalRecordField(fullFieldName))
            {
                //field = new RecordField(recordForQualifiedFields, getFormField(fullFieldName));
                field = (IPaletteField)Project.Current.AllForms[formName].ItemList[fieldName];

                if (field == null)
                {
                    field = new UnresolvedPaletteField(fullFieldName);
                }
                else
                {
                    field = new RecordField(recordForQualifiedFields, getFormField(fullFieldName));
                }
            }
            else
            {
                field = (Project.FieldMapByName.ContainsKey(fullFieldName)
                             ? (IPaletteField)Project.FieldMapByName[fullFieldName]
                             : PaletteField.NULL);
            }

            return field;
        }

        private IPaletteField getFieldFromAttribute(IXmlElement element)
        {
            IPaletteField field = PaletteField.NULL;
            if (element.HasChild("field"))
            {
                string fieldName = element.GetChild("field").GetAttribute("name");
                field = getField(fieldName);
            }

            return field;
        }

        // REVISIT: This method is duplicated in FunctionConditions. Can it be generalized somewhere?  - jdf 3/07
        private IForm getForm(string formName)
        {
            return Project.Current.AllForms[formName] ?? NullObjects.Form;
        }

        // REVISIT: This method is duplicated in FunctionConditions. Can it be generalized somewhere?  - jdf 3/07
        private IPaletteField getFormField(string recordAndFormQualifiedName)
        {
            string formName = FieldUtil.GetFormName(recordAndFormQualifiedName);
            string qualifiedFieldName = formName + ":" + FieldUtil.GetFieldName(recordAndFormQualifiedName);

            var field = getForm(formName).GetAllFields()[qualifiedFieldName] as IPaletteField;

            return field ?? PaletteField.NULL;
        }

        private void initializeMap()
        {
            if (parameterParseMap != null)
            {
                return;
            }

            parameterParseMap = new SimpleLookupTableWithDefault<Type, parse>(e => { return e.Text; });

            parameterParseMap.Add(typeof(IForm), e => { return getForm(e.Text); });
            parameterParseMap.Add(typeof(IMcqItem), e => { return getField(e.Text); });
            parameterParseMap.Add(typeof(IBlank), e => { return getField(e.Text); });
            parameterParseMap.Add(typeof(Int32), e => { return Convert.ToInt32(e.Text); });
            parameterParseMap.Add(typeof(FunctionConditions), e => { return new FunctionConditions(e); });
            parameterParseMap.Add(typeof(FunctionContentsField),
                                  e => { return new FunctionContentsField(getFieldFromAttribute(e)); });
            parameterParseMap.Add(typeof(FunctionBlank), e => { return new FunctionBlank(e); });
            parameterParseMap.Add(typeof(FunctionMCItem), e => { return new FunctionMCItem(e); });
            parameterParseMap.Add(typeof(FunctionCompoundExpression), e => { return new FunctionCompoundExpression(e); });
        }

        #region Nested type: parse

        private delegate object parse(IXmlElement e);

        #endregion

        #region Nested type: SimpleLookupTableWithDefault

        private class SimpleLookupTableWithDefault<K, V>
        {
            private readonly V defaultValue;
            private readonly Dictionary<K, V> map = new Dictionary<K, V>();

            public SimpleLookupTableWithDefault(V valueForUnknownKey)
            {
                defaultValue = valueForUnknownKey;
            }

            public V this[K key]
            {
                get
                {
                    if (map.ContainsKey(key))
                    {
                        return map[key];
                    }
                    return defaultValue;
                }
            }

            public bool Add(K key, V value)
            {
                if (!map.ContainsKey(key))
                {
                    map.Add(key, value);
                    return true;
                }
                return false;
            }
        }

        #endregion
    }
}