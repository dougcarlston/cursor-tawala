// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Text.RegularExpressions;
using Tawala.Functions.Runtime;
using Tawala.Projects;
using Tawala.Projects.Fields;

namespace Tawala.Functions.Controls
{
    internal static class ReferencedFormsHelper
    {
        public static FunctionFormCollection Get(IFunction function)
        {
            if (function != null)
            {
                return new FormsHelper(function).Accumulate();
            }

            return new FunctionFormCollection();
        }

        #region Nested type: FormsHelper

        private class FormsHelper
        {
            private readonly IFunction function;
            private readonly FunctionFormCollection referencedForms = new FunctionFormCollection();

            internal FormsHelper(IFunction function)
            {
                this.function = function;
            }

            internal FunctionFormCollection Accumulate()
            {
                search(function);
                return referencedForms;
            }

            internal void search(IPropertyCollection properties)
            {
                for (int i = 0; i < properties.PropertyCount; ++i)
                {
                    object value = properties[i];

                    if (value == null)
                    {
                        continue;
                    }

                    if (value is ICompositeParameterCollection)
                    {
                        search(value as ICompositeParameterCollection);
                    }
                    else if (value is ICompositeParameter)
                    {
                        search(value as ICompositeParameter);
                    }
                    else
                    {
                        addForm(value);
                    }
                }
            }

            private void search(ICompositeParameter composite)
            {
                if (composite == null)
                {
                    return;
                }
                for (int i = 0; i < composite.PropertyCount; ++i)
                {
                    if (composite[i] is FunctionParameterConditions)
                    {
                        continue;
                    }

                    addForm(composite[i]);
                }
            }

            private void search(ICompositeParameterCollection compositeCollection)
            {
                if (compositeCollection != null)
                {
                    for (int i = 0; i < compositeCollection.Count; ++i)
                    {
                        var composite = compositeCollection[i];
                        search(composite);
                    }
                }
            }

            private void addForm(object value)
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
                return (FieldUtil.IsFormField(valueString) || FieldUtil.IsExternalField(valueString) ||
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

        #endregion
    }
}