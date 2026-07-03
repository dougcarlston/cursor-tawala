// $Workfile: SendLine.cs $
// $Revision: 9 $	$Date: 5/24/06 7:20p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;
using Tawala.XmlSupport;

namespace Tawala.Projects
{
	/// <summary>
	/// Implements single SEND line for display in Process window.
	/// </summary>
	[Serializable]
	public class SendLine : ProcessLine
	{
		public SendLine()
		{
		}

		public SendLine(SendStatement statement) : base (statement, statement.ToString())
		{
			this.selectsGroup = false;
			this.isDeletable = true;
			this.isSelectable = true;
			this.canInsertBefore = true;
		}

		private bool isEmpty(string textString)
		{
			return (textString == null || textString == "");
		}

		/// <summary>
		/// Boolean indicating whether this line is valid.
		/// </summary>
		public override bool IsValidLine(FieldList fieldResolver)
		{
			Boolean isValid = true;

			SendStatement sendStatement = statement as SendStatement;

			// To: address and Subject line are always required
			if (isEmpty(sendStatement.AddressTo.Text) || isEmpty(sendStatement.Subject))
			{
				isValid = false;
			}
			else
			{
				isValid = sendStatement.SendBody.IsValid(sendStatement);
			}

			if (sendStatement.AddressTo.Type == FieldOrLiteral.StringType.field)
			{
				// if this statement's to address does not appear in field list...
				if (fieldResolver[sendStatement.AddressTo.Text] == null)
				{
					isValid = false;
				}
			}

			return isValid;
		}
	}

}
