// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Tawala.Functions.Runtime;
using Tawala.Projects.Fields;
using Tawala.Projects.Forms;
using Tawala.XmlSupport;

namespace Tawala.Projects
{
    public class XmlToFunctionConverter
    {
        private static readonly Record recordForQualifiedFields = new Record(FieldUtil.DefaultRecordQualifierName);
        private static SimpleLookupTableWithDefault<Type, parse> parameterParseMap;
        private readonly IFunctionRepository functionRepository;
        private IFunction function;
        private IFunctionInfo functionInfo;
        private IXmlElement functionXml;
        private string functionXmlId;

        public XmlToFunctionConverter()
        {
            if (FunctionLoader.Repository == null)
            {
                throw new InvalidOperationException("FunctionLoader.Current is null.");
            }

            functionRepository = FunctionLoader.Repository;
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

            if (functionRepository.Functions.Contains(functionXmlId))
            {
                functionInfo = functionRepository.Functions[functionXmlId];
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
                if (id.Equals("column"))
                {
                    ICompositeParameter composite = collection.CreateItem();

                    Collection<XmlElement> subParameters = parameter.GetChildren();
                    foreach (IXmlElement subParameter in subParameters)
                    {
                        switch (subParameter.Name)
                        {
                            case "header":
                            {
                                composite["header"] = new FunctionCompoundExpression(getHeaderElement(subParameter));
                                break;
                            }
                            case "contents":
                            {
                                IPaletteField field = getField(subParameter.GetChild("field").GetAttribute("name"));
                                composite["contents"] = new FunctionContentsField(field);
                                break;
                            }
                            case "display-conditions":
                            {
                                composite["display-conditions"] = new FunctionParameterConditions(subParameter);
                                break;
                            }
                        }
                    }

                    collection.Add(composite);
                }
                else if (functionInfo.Parameters.Contains(id))
                {
                    IParameterInfo pInfo = functionInfo.Parameters[id];
                    pInfo.SetValue(function, parameterParseMap[pInfo.PropertyType](parameter));
                }
            }
            if (collection.Count > 0)
            {
                compositeCollection.SetValue(function, collection);
            }
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

        private static IPaletteField getField(string fullFieldName)
        {
            IPaletteField field;

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

        private static IPaletteField getFieldFromAttribute(IXmlElement element)
        {
            IPaletteField field = PaletteField.NULL;
            if (element.HasChild("field"))
            {
                string fieldName = element.GetChild("field").GetAttribute("name");
                field = getField(fieldName);
            }

            return field;
        }

        // REVISIT: This method is duplicated in FunctionFilterConditions. Can it be generalized somewhere?  - jdf 3/07
        private static IForm getForm(string formName)
        {
            return Project.Current.AllForms[formName] ?? NullObjects.Form;
        }

        // REVISIT: This method is duplicated in FunctionFilterConditions. Can it be generalized somewhere?  - jdf 3/07
        private static IPaletteField getFormField(string recordAndFormQualifiedName)
        {
            string formName = FieldUtil.GetFormName(recordAndFormQualifiedName);
            string qualifiedFieldName = formName + ":" + FieldUtil.GetFieldName(recordAndFormQualifiedName);

            var field = getForm(formName).GetAllFields()[qualifiedFieldName] as IPaletteField;

            return field ?? PaletteField.NULL;
        }

        private static void initializeMap()
        {
            if (parameterParseMap != null)
            {
                return;
            }

            parameterParseMap = new SimpleLookupTableWithDefault<Type, parse>(e => e.Text);

            parameterParseMap.Add(typeof(IForm), e => getForm(e.Text));
            parameterParseMap.Add(typeof(IMcqItem), e => getField(e.Text));
            parameterParseMap.Add(typeof(IBlank), e => getField(e.Text));
            parameterParseMap.Add(typeof(Int32), e => Convert.ToInt32(e.Text));
            parameterParseMap.Add(typeof(FunctionFilterConditions), e => new FunctionFilterConditions(e));
            parameterParseMap.Add(typeof(FunctionParameterConditions), e => new FunctionParameterConditions(e));
            parameterParseMap.Add(typeof(FunctionContentsField), e => new FunctionContentsField(getFieldFromAttribute(e)));
            parameterParseMap.Add(typeof(FunctionBlank), e => new FunctionBlank(e));
            parameterParseMap.Add(typeof(FunctionMCItem), e => FunctionMCItem.Create(e));
            parameterParseMap.Add(typeof(FunctionCompoundExpression), e => new FunctionCompoundExpression(e));
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

            public void Add(K key, V value)
            {
                if (!map.ContainsKey(key))
                {
                    map.Add(key, value);
                }
            }
        }

        #endregion
    }
}