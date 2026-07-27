// $Workfile: IfLine.cs $
// $Revision: 13 $	$Date: 2/28/08 5:42p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;

namespace Tawala.Projects
{
	/// <summary>
	/// Implements top line of IF statement for display in Process window.
	/// </summary>
	[Serializable]
	public class IfLine : ProcessLine
	{
		public IfLine(IfStatement statement) : base (statement, statement.ToString())
		{
			this.group = statement;
			this.selectsGroup = true;
			this.isDeletable = true;
			this.isSelectable = true;
			this.canInsertBefore = true;
		}

		/// <summary>
		/// Boolean indicating whether this line is valid.
		/// </summary>
		public override bool IsValidLine(FieldList fieldResolver)
		{
			Boolean isValid = false;

			IfStatement ifStatement = statement as IfStatement;

			if (ifStatement.IsSimple)
			{
				isValid = true;

				int id = ((Condition)ifStatement.Conditions[0]).Field.Id;

				if (!isProjectField(id))
				{
					isValid = false;
				}

				Expression expression = ((Condition)ifStatement.Conditions[0]).Expression;

				if (expression != null && expression.HasSingleFieldElement)
				{
					IField field = ((FieldElement)expression.Elements[0]).Field;

					if (!(field is ChoiceField))
					{
						if (!isProjectField(field.Id))
						{
							isValid = false;
						}
					}
				}

				if (!isValid)
				{
					// if this statement's field name appears in field list...
					if (fieldResolver[((Condition)ifStatement.Conditions[0]).Field.FieldName] != null)
					{
						isValid = true;
					}
				}

			}
			else if (ifStatement.Conditions.Count > 0)
			{
				isValid = true;

				foreach (IConditionComponent component in ifStatement.Conditions)
				{
					if (component is Condition)
					{
						// if field name does not appear in field list...
						IField field = ((Condition)component).Field;
						if (fieldResolver[field.FieldName] == null)
						{
							isValid = false;
						}
					}
				}
			}

			return isValid;
		}

		/// <summary>
		/// Indicates whether the specified field belongs to the current Project.
		/// </summary>
		private bool isProjectField(int fieldId)
		{
			return Project.FieldMapById.ContainsKey(fieldId);
		}
	}
}
