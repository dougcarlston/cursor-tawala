// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.Generic;

namespace Tawala.Functions.Runtime.Private
{
    public static class FunctionParameterTypeMapper
    {
        private static readonly Mappings map = new Mappings
        {
            new FunctionParameterTypeInfo("tawala-form", "Tawala.Projects.IForm", "Tawala.Functions.Controls.ComboBoxControl",
                                          "Tawala.Functions.Controls.FormBinder", "Choose from the drop-down list", null),
            new FunctionParameterTypeInfo("tawala-local-form", "Tawala.Projects.IForm", "Tawala.Functions.Controls.ComboBoxControl",
                                          "Tawala.Functions.Controls.LocalFormBinder", "Choose from the drop-down list", null),
            new FunctionParameterTypeInfo("numeric-list", typeof(int).FullName, "Tawala.Functions.Controls.ComboBoxControl",
                                          "Tawala.Functions.Controls.NumericListBinder", // shared with enumeration
                                          "Choose from the drop-down list", null),
            new FunctionParameterTypeInfo("enumeration", typeof(string).FullName, "Tawala.Functions.Controls.ComboBoxControl",
                                          "Tawala.Functions.Controls.EnumerationBinder", "Choose from the drop-down list", null),
            new FunctionParameterTypeInfo("tawala-mcq", "Tawala.Projects.FunctionMCItem", "Tawala.Functions.Controls.McqFieldControl",
                                          "Tawala.Functions.Controls.FieldBaseBinder", "Multipe choice", null),
            new FunctionParameterTypeInfo("tawala-blank", "Tawala.Projects.FunctionBlank", "Tawala.Functions.Controls.BlankFieldControl",
                                          "Tawala.Functions.Controls.FieldBaseBinder", "Blank or hidden field", null),
            new FunctionParameterTypeInfo("tawala-conditions", "Tawala.Projects.FunctionFilterConditions",
                                          "Tawala.Functions.Controls.FunctionConditionListControl", "", "",
                                          "Utility.CreateDefaultFunctionFilterConditions()"),
            new FunctionParameterTypeInfo("tawala-contents-field", "Tawala.Projects.FunctionContentsField",
                                          "Tawala.Functions.Controls.ContentFieldControl", "Tawala.Functions.Controls.FieldBaseBinder",
                                          "Any field", null),
            new FunctionParameterTypeInfo("text", typeof(string).FullName, "Tawala.Functions.Controls.TextControl", "TextBinder",
                                          "Enter text", "string.Empty"),
            new FunctionParameterTypeInfo("expression", "Tawala.Projects.FunctionCompoundExpression",
                                          "Tawala.Functions.Controls.CompoundExpressionControl",
                                          "Tawala.Functions.Controls.CompoundExpressionBinder", "A compound expression", null),
            new FunctionParameterTypeInfo("parameter-collection", "", "Tawala.Functions.Controls.ColumnListControl", "", "", null),
            new FunctionParameterTypeInfo("ColumnListControl", "", "Tawala.Functions.Controls.ColumnControl", "", "", null),
            new FunctionParameterTypeInfo("boolean-expression", "Tawala.Projects.FunctionParameterConditions", "", "", "",
                                          "Utility.CreateDefaultFunctionParameterConditions()")
        };

        public static Mappings Map
        {
            get
            {
                return map;
            }
        }

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