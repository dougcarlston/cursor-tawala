// $Workfile: ProcessLine.cs $
// $Revision: 33 $	$Date: 11/15/06 9:06a $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections;
using System.Diagnostics;

namespace Tawala.Projects
{
	/// <summary>
	/// Implements single line of Process for display in Process window.
	/// </summary>
	[Serializable]
	public abstract class ProcessLine : IProcessElement
	{
        protected ProcessLine()
		{
		}

		protected ProcessLine(ProcessStatement statement, string text)
		{
			this.statement = statement;
			this.text = text;
		}

		/// <summary>
		/// Make a shallow copy of this line, including a shallow copy of the line's statement, if present
		/// </summary>
		/// <returns></returns>
		public ProcessLine Copy()
		{
			ProcessLine copiedLine = (ProcessLine)MemberwiseClone();

			if (statement != null)
			{
				copiedLine.statement = statement.Copy();
			}

			return copiedLine;
		}

		/// <summary>
		/// Process line text.
		/// </summary>
		protected string text;

		/// <summary>
		/// Process statement that this line was created from.
		/// </summary>
		protected ProcessStatement statement;

		public ProcessStatement Statement
		{
			get
			{
				return statement;
			}
			set
			{
				statement = value;
			}
		}

		/// <summary>
		/// Group that this line belongs to.
		/// </summary>
		protected ProcessStatement group;

		public ProcessStatement Group
		{
			get
			{
				return group;
			}
			set
			{
				group = value;
			}
		}

		/// <summary>
		/// Boolean indicating whether selecting this line should
		/// cause selection of all lines in its group.
		/// </summary>
		protected bool selectsGroup;

		public bool SelectsGroup
		{
			get
			{
				return selectsGroup;
			}

			set
			{
				selectsGroup = value;
			}
		}

		/// <summary>
		/// Boolean indicating whether this line is directly deletable.
		/// </summary>
		/// <remarks>
		/// All lines are deletable in a sense, but some, such as the bracket lines in an
		/// IF statement, are not directly deletable. That is, they may not simply be
		/// selected and deleted. Their deletion can only occur when the statement
		/// they belong to is deleted.
		/// 
		/// The isDeletable field is used to mark those fields that may be directly
		/// selected and deleted.
		/// </remarks>
		protected bool isDeletable;

		public bool IsDeletable
		{
			get
			{
				return isDeletable;
			}

			set
			{
				isDeletable = value;
			}
		}

		/// <summary>
		/// Boolean indicating whether this line is selectable.
		/// </summary>
		protected bool isSelectable;

		public bool IsSelectable
		{
			get
			{
				return isSelectable;
			}

			set
			{
				isSelectable = value;
			}
		}

		/// <summary>
		/// Boolean indicating whether lines can be inserted before this line.
		/// </summary>
		protected bool canInsertBefore;

		public bool CanInsertBefore
		{
			get
			{
				return canInsertBefore;
			}

			set
			{
				canInsertBefore = value;
			}
		}

		/// <summary>
		/// Indent level for display in process window.
		/// </summary>
		/// <remarks>
		/// 0 = no indent
		/// </remarks>
		protected int indentLevel;

		public int IndentLevel
		{
			get
			{
				return indentLevel;
			}

			set
			{
				indentLevel = value;
			}
		}

		public override string ToString()
		{
			if (statement != null)
			{
				if (IsValid)
				{
					return statement.ToString();
				}
				else
				{
//					return (lastValidLineString.Length > 0 ? lastValidLineString : statement.ToString());
					return statement.ToString();
				}
			}
			else
			{
				return text;
			}
		}

		public virtual string ToXml()
		{
			return statement.ToXml();
		}

		#region IProcessElement Interface

		/// <summary>
		/// Boolean indicating whether this line is valid.
		/// </summary>
		public virtual bool IsValidLine(FieldList fieldResolver)
		{
			// override this in derived classes
			return false;
		}

		private bool isValid = true;

		/// <summary>
		/// Boolean indicating whether this line is valid.
		/// </summary>
		public bool IsValid
		{
			get
			{
				return isValid;
#if false
				bool isValid = false;

				Process parentProcess = Project.Current.GetProcessOrSkipInstructions(statement);

				if (parentProcess != null)
				{
					int lineIndex = parentProcess.Lines.GetIndex(statement);

					FieldList fieldResolver = getValidNativeFields(parentProcess, lineIndex);

					isValid = IsValidLine(fieldResolver);
				}

				if (isValid)
				{
					preserveLineString();
				}

				return isValid;
#endif
			}
		}

		public void Validate()
		{
			isValid = false;

			Process parentProcess = Project.Current.GetProcessOrSkipInstructions(statement);

			if (parentProcess != null)
			{
				int lineIndex = parentProcess.Lines.GetIndex(statement);

				FieldList fieldResolver = getValidNativeFields(parentProcess, lineIndex);

				isValid = IsValidLine(fieldResolver);
			}

			if (isValid)
			{
				preserveLineString();
			}
		}

		private string lastValidLineString = "";

		private void preserveLineString()
		{
			lastValidLineString = statement.ToString();
		}

		/// <summary>
		/// Returns a list of valid fields from the Form to which the Process containing this line is attached.
		/// </summary>
		private static FieldList getValidNativeFields(Process parentProcess, int lineIndex)
		{
			return parentProcess.GetValidFields(lineIndex);
		}


		#endregion

		#region IRecursiveEnumerable Interface

		public IEnumerable RecursiveEnumerator
		{
			get
			{
				yield return this;
			}
		}

		#endregion
	}
}
