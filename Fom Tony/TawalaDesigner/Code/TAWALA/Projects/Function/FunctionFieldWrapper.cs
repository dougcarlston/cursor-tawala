// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using Tawala.Functions.Runtime;

namespace Tawala.Projects
{
    [Serializable]
    public abstract class FunctionFieldWrapper : IFunctionParameterValue
    {
        protected IPaletteField field;

        public IPaletteField Field
        {
            get
            {
                resolveField();

                return field;
            }
            set { field = value; }
        }

        #region IFunctionParameterValue Members

        public string ToValueXml()
        {
            return ToString();
        }

        public string FormattedString { get { return Field != null ? Field.FieldString : string.Empty; } }

        #endregion

        protected abstract IPaletteField ResolveField(string qualifiedFieldName);

        private void resolveField()
        {
            var unresolved = field as UnresolvedPaletteField;
            if (unresolved == null)
            {
                return;
            }

            string qualifiedFieldName = unresolved.QualifiedFieldName;

            if (string.IsNullOrEmpty(qualifiedFieldName))
            {
                Debugger.Break();
                return;
            }

            IPaletteField result = ResolveField(qualifiedFieldName);
            if (result != null)
            {
                field = result;
            }
        }

        #region Nested type: BindingConverter

        ///<summary> TypeConverter for Function Configuration Data Bindings - the above attribute is inherited by derived classes </summary>
        internal class BindingConverter : StringConverter
        {
            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
            {
                if (destinationType == typeof(string))
                {
                    return ((IFunctionParameterValue)value).FormattedString;
                }

                return base.ConvertTo(context, culture, value, destinationType);
            }
        }

        #endregion
    }
}