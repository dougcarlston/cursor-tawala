using System;
using System.Collections.Generic;
using System.Text;
using Tawala.Functions.Runtime;
using Tawala.XmlSupport;

namespace Tawala.Projects
{
	public static class FunctionUtil
	{
		public static bool HasParameterRestrictions(IParameterInfo parameterInfo)
		{
			return parameterInfo != null && parameterInfo.Restrictions != ParameterRestrictions.Default;
		}

		public static IPaletteField ApplyParameterRestrictions(IParameterInfo parameterInfo, IPaletteField field)
		{
			if (parameterInfo.Restrictions == ParameterRestrictions.RecordIterationAlways || parameterInfo.Restrictions == ParameterRestrictions.RecordIterationNever)
			{
				RecordField recordField = field as RecordField;

				if (parameterInfo.Restrictions == ParameterRestrictions.RecordIterationAlways)
				{
					if (recordField != null)
					{
						field = recordField.ReferenceField as IPaletteField;
					}
					field = new RecordField(new Record(parameterInfo.RecordListName), field);
				}
				else if (parameterInfo.Restrictions == ParameterRestrictions.RecordIterationNever)
				{
					if (recordField != null)
					{
						field = recordField.ReferenceField as IPaletteField;
					}
				}
			}
			return field;
		}
	}
}
