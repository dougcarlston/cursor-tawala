using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Tawala.XmlSupport;

namespace Tawala.Projects
{
	[Serializable]
	[TypeConverter(typeof(FunctionFieldWrapper.BindingConverter))]
	public abstract class FunctionFieldWrapper
	{
		///<summary> TypeConverter for Function Configuration Data Bindings - the above attribute is inherited by derived classes </summary>
		private class BindingConverter : StringConverter
		{
			public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
			{
				if (destinationType == typeof(string))
				{
					IPaletteField field = ((FunctionFieldWrapper)value).Field;
					return field != null ? field.QualifiedFieldName : string.Empty;
				}					

				return base.ConvertTo(context, culture, value, destinationType);
			}
		}

		public abstract IPaletteField Field
		{
			get;
			set;
		}
	}
}
