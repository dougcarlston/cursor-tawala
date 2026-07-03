// $Workfile: FieldSerializationInfo.cs $
// $Revision: 3 $	$Date: 12/17/07 3:37p $
// Copyright © 2005 - 2007 Tawala Systems, Inc. All rights reserved.

using System;
using System.Diagnostics;
using Tawala.Projects.Fields;

namespace Tawala.Projects
{
	[Serializable]
	public class FieldSerializationInfo
	{
		private IField targetField;

		public FieldSerializationInfo(IField field)
		{
			this.targetField = field;
		}

		public IPaletteField Deserialized()
		{
//			Debug.Assert(Project.FieldMapById.ContainsKey(myField.Id));

			IPaletteField field = null;

			if (Project.FieldMapById.ContainsKey(targetField.Id))
			{
				IPaletteField mappedField = Project.FieldMapById[targetField.Id] as IPaletteField;

				if (this.targetField is RecordField)
				{
					RecordField recordField = (RecordField)mappedField;
					field = new RecordField(recordField.Record, (IPaletteField)recordField.ReferenceField);
				}
				else if (this.targetField is RecordSetField)
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
