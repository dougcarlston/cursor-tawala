// $Workfile: ShowLine.cs $
// $Revision: 11 $	$Date: 7/02/07 8:21a $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;
using Tawala.Projects.Forms;

namespace Tawala.Projects
{
	/// <summary>
	/// Implements single SHOW line for display in Process window.
	/// </summary>
	[Serializable]
	public class ShowLine : ProcessLine
	{
		public ShowLine()
		{
		}

		public ShowLine(ShowStatement statement)
			: base(statement, statement.ToString())
		{
			this.selectsGroup = false;
			this.isDeletable = true;
			this.isSelectable = true;
			this.canInsertBefore = true;
		}

		public override bool IsValidLine(FieldList fieldResolver)
		{
			// override in derived classes
			return false;
		}
	}

	[Serializable]
	public class ShowDocumentLine : ShowLine
	{
		public ShowDocumentLine()
		{
		}

		public ShowDocumentLine(ShowDocumentStatement statement) : base(statement)
		{
			this.selectsGroup = false;
			this.isDeletable = true;
			this.isSelectable = true;
			this.canInsertBefore = true;

			Project.Events.DocumentChanged += events_DocumentChanged;
		}

		/// <summary>
		/// Boolean indicating whether this line is valid.
		/// </summary>
		public override bool IsValidLine(FieldList fieldResolver)
		{
			ShowDocumentStatement showDocumentStatement = statement as ShowDocumentStatement;

			Boolean isValid = false;

			if (showDocumentStatement.Document != null)
			{
				isValid = (showDocumentStatement.ValidateDocumentReference(showDocumentStatement.Document) == ProcessStatement.StatementStatus.Valid);
			}

			return isValid;
		}

		void events_DocumentChanged(object sender, ComponentEventArgs e)
		{
			ShowDocumentStatement showDocumentStatement = statement as ShowDocumentStatement;

			if (!Project.Current.DocumentList.Contains(showDocumentStatement.Document))
			{
                showDocumentStatement.Document = NullObjects.Document;
			}
		}
	}

	[Serializable]
	public class ShowFormLine : ShowLine
	{
		public ShowFormLine()
		{
		}

		public ShowFormLine(ShowFormStatement statement)
			: base(statement)
		{
			this.selectsGroup = false;
			this.isDeletable = true;
			this.isSelectable = true;
			this.canInsertBefore = true;

			Project.Events.ComponentRemoved += new EventHandler<ComponentEventArgs>(events_ComponentRemoved);
		}

		/// <summary>
		/// Boolean indicating whether this line is valid.
		/// </summary>
		public override bool IsValidLine(FieldList fieldResolver)
		{
			ShowStatement showStatement = statement as ShowStatement;

			Boolean isValid = false;

			if (showStatement.Form != null)
			{
				isValid = (showStatement.ValidateFormReference(showStatement.Form) == ProcessStatement.StatementStatus.Valid);
			}

			return isValid;
		}

		void events_ComponentRemoved(object sender, ComponentEventArgs e)
		{
			if (e.Component is IForm)
			{
				ShowStatement showStatement = statement as ShowStatement;

				if (showStatement.Form == e.Component as IForm)
				{
                    showStatement.Form = NullObjects.Form;
				}
			}
		}
	}
}
