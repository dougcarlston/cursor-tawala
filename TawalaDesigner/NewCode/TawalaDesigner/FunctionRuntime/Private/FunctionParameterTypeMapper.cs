// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.Generic;

namespace Tawala.Functions.Runtime.Private
{
    public static class FunctionParameterTypeMapper
    {
        private static readonly Mappings map =
            new Mappings
            {
                new FunctionParameterTypeInfo("tawala-form",
                                              "Tawala.Projects.Forms.IForm",
                                              "ParameterComboBox",
                                              "FormBinder",
                                              " - pick one",
                                              null),
                new FunctionParameterTypeInfo("tawala-local-form",
                                              "Tawala.Projects.Forms.IForm",
                                              "ParameterComboBox",
                                              "LocalFormBinder",
                                              " - pick one",
                                              null),
                new FunctionParameterTypeInfo("numeric-list",
                                              typeof(int).FullName,
                                              "ParameterComboBox",
                                              "NumericListBinder", // shared with enumeration
                                              " - pick one",
                                              null),
                new FunctionParameterTypeInfo("enumeration",
                                              typeof(string).FullName,
                                              "ParameterComboBox",
                                              "EnumerationBinder",
                                              " - pick one",
                                              null),
                new FunctionParameterTypeInfo("tawala-mcq",
                                              "FunctionMCItem",
                                              "MCItemParameterTextBox",
                                              "FieldTypeConverterBinder",
                                              " - multiple choice",
                                              null),
                new FunctionParameterTypeInfo("tawala-blank",
                                              "FunctionBlank",
                                              "BlankParameterTextBox",
                                              "FieldTypeConverterBinder",
                                              " - blank or hidden field",
                                              null),
                new FunctionParameterTypeInfo("tawala-conditions",
                                              "FunctionConditions",
                                              "FunctionConditionsParameterControl",
                                              "FunctionConditionsBinder",
                                              "",
                                              null),
                new FunctionParameterTypeInfo("tawala-contents-field",
                                              "FunctionContentsField",
                                              "ContentsFieldParameterTextBox",
                                              "FieldTypeConverterBinder",
                                              " - any field",
                                              null),
                new FunctionParameterTypeInfo("text",
                                              typeof(string).FullName,
                                              "TextParameterTextBox",
                                              "TextBinder",
                                              " - type any text",
                                              "string.Empty"),
                new FunctionParameterTypeInfo("expression",
                                              "FunctionCompoundExpression",
                                              "CompoundExpressionParameterTextBox",
                                              "CompoundExpressionBinder",
                                              " - expression",
                                              null),
                new FunctionParameterTypeInfo("parameter-collection",
                                              "",
                                              "ColumnListParameterControl",
                                              "ColumnListBinder",
                                              "",
                                              ""),
                new FunctionParameterTypeInfo("CompositeParameterListControl",
                                              "",
                                              "ColumnParameterControl",
                                              "ColumnBinder",
                                              "",
                                              ""),
                new FunctionParameterTypeInfo("boolean-expression",
                                              "Conditions",
                                              "",
                                              "",
                                              "",
                                              null)
            };

        public static Mappings Map { get { return map; } }

        public static string GetObjectTypeFromXmlType(string xmlType)
        {
            return Map[xmlType].DataType;
        }

        public static string GetControlTypeFromXmlType(string xmlType)
        {
            return Map[xmlType].ControlType;
        }

        #region Nested type: Mappings

        public class Mappings : Dictionary<string, IFunctionParameterTypeInfo>
        {
            public void Add(IFunctionParameterTypeInfo info)
            {
                Add(info.XmlType, info);
            }
        }

        #endregion
    }
}