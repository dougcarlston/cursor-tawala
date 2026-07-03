// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;

namespace Tawala.Functions.Runtime.Private
{
    internal class FunctionParameterTypeInfo : IFunctionParameterTypeInfo
    {
        internal FunctionParameterTypeInfo(string xmltype, string datatype, string controltype, string bindingtype, string tagline,
                                           string initializer)
        {
            XmlType = xmltype;
            TagLine = tagline;
            Initializer = initializer;

            if (!datatype.Contains(".") && datatype.Length > 0 && char.IsUpper(datatype[0]))
            {
                datatype = "Tawala.Projects." + datatype;
            }
            DataType = datatype;

            if (!controltype.Contains("."))
            {
                controltype = "Tawala.Functions.Controls." + controltype;
            }
            ControlType = controltype;

            if (!bindingtype.Contains("."))
            {
                bindingtype = "Tawala.Functions.Controls." + bindingtype;
            }
            BindingType = bindingtype;
        }

        #region IFunctionParameterTypeInfo Members

        public string XmlType { get; private set; }

        public string DataType { get; private set; }

        public string ControlType { get; private set; }

        public string BindingType { get; private set; }

        public string TagLine { get; private set; }

        public string Initializer { get; private set; }

        #endregion
    }
}