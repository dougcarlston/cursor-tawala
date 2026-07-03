// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.ComponentModel;
using System.Globalization;
using Tawala.Functions.Runtime;
using Tawala.Projects.Expressions;
using Tawala.XmlSupport;

namespace Tawala.Projects
{
    [Serializable]
    [TypeConverter(typeof(BindingConverter))]
    public class FunctionCompoundExpression : IFunctionParameterValue
    {
        private readonly FieldsAndLiteralsExpression expression;

        public FunctionCompoundExpression()
        {
        }

        public FunctionCompoundExpression(string expr)
        {
            expression = new FieldsAndLiteralsExpression(expr);
        }

        public FunctionCompoundExpression(IXmlElement expr)
        {
            expression = new FieldsAndLiteralsExpression(expr);
        }

        public FieldsAndLiteralsExpression Expression { get { return expression; } }

        #region IFunctionParameterValue Members

        public string ToValueXml()
        {
            return ToString();
        }

        public string FormattedString { get { return Expression != null ? Expression.ToString() : string.Empty; } }

        #endregion

        public override string ToString()
        {
            return expression.ToXml();
        }

        public static FunctionCompoundExpression Parse(object o)
        {
            var expr = o as string;

            if (String.IsNullOrEmpty(expr))
            {
                return null;
            }

            return new FunctionCompoundExpression(expr);
        }

        #region Nested type: BindingConverter

        ///<summary> TypeConverter for Function Configuration Data Bindings - the above attribute is inherited by derived classes </summary>
        private class BindingConverter : TypeConverter
        {
            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
            {
                if (destinationType == typeof(string))
                {
                    FieldsAndLiteralsExpression expression = ((FunctionCompoundExpression)value).Expression;
                    return expression != null ? expression.ToString() : string.Empty;
                }

                return base.ConvertTo(context, culture, value, destinationType);
            }

            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                if (destinationType == typeof(string))
                {
                    return true;
                }
                return base.CanConvertTo(context, destinationType);
            }

            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            {
                return Parse(value);
            }

            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                if (sourceType == typeof(string))
                {
                    return true;
                }
                return base.CanConvertFrom(context, sourceType);
            }
        }

        #endregion
    }
}