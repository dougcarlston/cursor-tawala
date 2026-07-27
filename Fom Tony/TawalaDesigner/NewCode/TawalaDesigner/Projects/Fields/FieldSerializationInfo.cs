// Copyright © 2005 - 2008 Tawala Systems, Inc. All rights reserved.

using System;

namespace Tawala.Projects.Fields
{
	[Serializable]
	public class FieldSerializationInfo
	{
		private readonly int targetFieldId;

		public FieldSerializationInfo(IField field)
		{
			targetFieldId = field.Id;
		}

		public IPaletteField Deserialized()
		{
			IPaletteField field = null;

			if (Project.FieldMapById.ContainsKey(targetFieldId))
			{
				IPaletteField mappedField = Project.FieldMapById[targetFieldId] as IPaletteField;

				if (mappedField is RecordField)
				{
					RecordField recordField = (RecordField)mappedField;
					field = new RecordField(recordField.Record, (IPaletteField)recordField.ReferenceField);
				}
				else if (mappedField is RecordSetField)
				{
					RecordSetField recordSetField = (RecordSetField)mappedField;
					field = new RecordSetField(recordSetField.RecordSet, (IPaletteField)recordSetField.ReferenceField);
				}
				else
				{
					field = mappedField;
				}
			}

			if (field == null)
			{
				field = new Variable(FieldUtil.UnknownFieldName);
			}

			return field;
		}
	}
}