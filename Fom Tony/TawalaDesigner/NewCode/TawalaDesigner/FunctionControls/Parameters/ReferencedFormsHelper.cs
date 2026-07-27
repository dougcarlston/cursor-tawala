// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.Reflection;
using System.Text.RegularExpressions;
using Tawala.Functions.Runtime;
using Tawala.Projects;
using Tawala.Projects.Forms;

namespace Tawala.Functions.Controls
{
    internal static class ReferencedFormsHelper
    {
        public static FunctionFormCollection Get(IFunction function)
        {
            var referencedForms = new FunctionFormCollection();

            if (function != null)
            {
                foreach (IParameterInfo parameter in function.Info.Parameters)
                {
                    addForms(function, referencedForms, parameter);
                }
            }

            return referencedForms;
        }

        private static void addForms(IFunction function, FunctionFormCollection referencedForms, IParameterInfo parameter)
        {
            if (parameter.Parameters.Count == 0)
            {
                addForm(parameter.GetValue(function), referencedForms);
            }
            else
            {
                object value = parameter.GetValue(function);
                var collection = value as ICompositeParameterCollection;
                var composite = value as ICompositeParameter;

                if (collection != null || composite != null)
                {
                    for (int i = 0; i < parameter.Parameters.Count; ++i)
                    {
                        // RESOLVE/REVISIT SOON NEED TO HAVE HELPER METHODS FOR GETTING SUBPARAMETER VALUES
                        string propertyName = parameter.Parameters[i].PropertyName;
                        if (collection != null)
                        {
                            addCompositeCollectionForms(collection, referencedForms, propertyName);
                        }
                        else
                        {
                            addCompositeForm(composite, referencedForms, propertyName);
                        }
                    }
                }
            }
        }

        private static void addCompositeCollectionForms(ICompositeParameterCollection collection, FunctionFormCollection referencedForms,
                                                        string propertyName)
        {
            for (int i = 0; i < collection.Count; ++i)
            {
                ICompositeParameter composite = collection[i];
                addCompositeForm(composite, referencedForms, propertyName);
            }
        }

        private static void addCompositeForm(ICompositeParameter composite, FunctionFormCollection referencedForms, string propertyName)
        {
            if (composite != null && formsInParameterShouldBeReferenced(propertyName))
            {
                PropertyInfo pi = composite.GetType().GetProperty(propertyName);
                object value = pi.GetValue(composite, null);
                addForm(value, referencedForms);
            }
        }

        private static bool formsInParameterShouldBeReferenced(string propertyName)
        {
            return propertyName != "DisplayConditions";
        }

        private static void addForm(object value, FunctionFormCollection referencedForms)
        {
            if (value != null && value.GetType() != typeof(string) && value.GetType() != typeof(int))
            {
                string valueString = value.ToString();

                if (isForm(valueString))
                {
                    referencedForms.AddUnique(findForm(valueString));
                }
                else if (isFieldName(valueString))
                {
                    referencedForms.AddUnique(findForm(FieldUtil.GetFormName(valueString)));
                }
                else
                {
                    MatchCollection matches = extractFieldNamesFromXmlTags(valueString);
                    foreach (Match m in matches)
                    {
                        string fieldString = m.Groups[1].Value;

                        if (isFieldName(fieldString))
                        {
                            referencedForms.AddUnique(findForm(FieldUtil.GetFormName(fieldString)));
                        }
                    }
                }
            }
        }

        private static IForm findForm(string name)
        {
            if (Project.Current.AllForms.IndexOf(name) >= 0)
            {
                return Project.Current.AllForms[name];
            }

            return null;
        }

        private static bool isFieldName(string valueString)
        {
            return (FieldUtil.IsFormField(valueString) ||
                    FieldUtil.IsExternalField(valueString) ||
                    FieldUtil.IsRegularOrExternalRecordField(valueString));
        }

        private static MatchCollection extractFieldNamesFromXmlTags(string valueString)
        {
            return Regex.Matches(valueString, "<field name=\"(.+)\"[ ]*/>");
        }

        private static bool isForm(string valueString)
        {
            return (findForm(valueString) != null);
        }
    }
}