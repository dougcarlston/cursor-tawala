// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using Tawala.Functions.Runtime;
using Tawala.XmlSupport;

namespace Tawala.Projects
{
	[Serializable]
	[TypeConverter(typeof(FunctionCompoundExpression.BindingConverter))]
	public class FunctionCompoundExpression : IFunctionParameterXml
	{
		///<summary> TypeConverter for Function Configuration Data Bindings - the above attribute is inherited by derived classes </summary>
		private class BindingConverter : StringConverter
		{
			public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
			{
				if (destinationType == typeof(string))
				{
					UrlExpression expression = ((FunctionCompoundExpression)value).Expression;
					return expression != null ? expression.ToString() : string.Empty;
				}

				return base.ConvertTo(context, culture, value, destinationType);
			}
		}

		public FunctionCompoundExpression()
		{
		}

		public FunctionCompoundExpression(string expr)
		{
			expression = new UrlExpression(expr);
		}

		public FunctionCompoundExpression(IXmlElement expr)
		{
			expression = new UrlExpression(expr);
		}

		public UrlExpression Expression
		{
			get { return expression; }
		}

		public override string ToString()
		{
			return expression.ToXml();
		}

		public static FunctionCompoundExpression Parse(object o)
		{
			string expr = o as string;

			if (expr == null)
			{
				return null;
			}

			return new FunctionCompoundExpression(expr);
		}

		private UrlExpression expression;

        #region IFunctionParameterXml Members

        public string ToFunctionParameterXml()
        {
            return ToString();
        }

        #endregion
    }
}
