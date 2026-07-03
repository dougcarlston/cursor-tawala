// $Workfile: ProcessLineList.cs $
// $Revision: 77 $	$Date: 2/28/08 2:01p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.
#if DEBUG
#define CODE_ANALYSIS
#endif
using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Reflection;
using System.Collections.ObjectModel;
using Tawala.Projects.Forms;
using Tawala.Projects.Forms.NewFormItems;

namespace Tawala.Projects
{
	/// <summary>
	/// List of process lines.
	/// </summary>Warning	

	[Serializable]
	public sealed class ProcessLineList : Collection<ProcessLine>
	{
		public ProcessLineList()
		{
		}

		/// <summary>
		/// If owned by a Process the ProcessLineList will fire events
		/// </summary>
		public ProcessLineList(Process process)
		{
			this.process = process;
		}

		private Process process = null;

		public Process Process
		{
			get
			{
				return process;
			}
			set
			{
				process = value;
			}
		}

		/// <summary>
		/// Constructor. Create process line list from any process statement
		/// </summary>
		public ProcessLineList(ProcessStatement statement)
		{
			// dynamically invoke the createLineList method that handles the fully derived type of statement
			GetType().InvokeMember("createLineList",
				BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod, null,
				this, new object[] { statement });
		}

		/// <summary>
		/// Constructor. Create process line list from portion of source list.
		/// </summary>
		/// <param name="firstIndex">Index of first line in source list to use as first line of new list</param>
		/// <param name="sourceList">Existing line list</param>
		public ProcessLineList(int firstIndex, ProcessLineList sourceList)
		{
			ProcessStatement sourceGroup = sourceList[firstIndex].Group;

			// if first source line not part of group...
			if (sourceGroup == null)
			{
				// replicate source list line in this list
				Add(sourceList[firstIndex]);
			}
			else
			{
				int lastIndex = sourceList.lastGroupIndex(sourceGroup);

				// for range of lines belonging to first line's group...
				for (int i = firstIndex; i <= lastIndex; i++)
				{
					// add line to this list
					Add(sourceList[i]);
				}
			}
		}

		/// <summary>
		/// Create process line list from Show statement
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		private void createLineList(AppendStatement statement)
		{
			// make process line from statement
			Add(new AppendLine(statement));
		}

		/// <summary>
		/// Create process line list from Show Document statement
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		private void createLineList(ShowDocumentStatement statement)
		{
			// make process line from statement
			Add(new ShowDocumentLine(statement));
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		private void createLineList(ShowFormStatement statement)
		{
			Add(new ShowFormLine(statement));
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		private void createLineList(ShowRecordStatement statement)
		{
			Add(new ShowRecordLine(statement));
		}

		/// <summary>
		/// Create process line list from Show Url statement
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		private void createLineList(ShowUrlStatement statement)
		{
			// make process line from statement
			Add(new ShowUrlLine(statement));
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		private void createLineList(SendStatement statement)
		{
			Add(new SendLine(statement));
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		private void createLineList(SetStatement statement)
		{
			Add(new SetLine(statement));
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		private void createLineList(ArithmeticStatement statement)
		{
			Add(new ArithmeticLine(statement));
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		private void createLineList(SkipToStatement statement)
		{
			Add(new SkipToLine(statement));
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		private void createLineList(IfStatement statement)
		{
			ProcessLineList tmp = new ProcessLineList();

			// make top line from if statement and conditions
			tmp.Add(new IfLine(statement));

			// make opening parenthesis line
			tmp.Add(new BlockOpenLine(statement, "(", "<trueSet>"));

			foreach (ProcessStatement enclosedStatement in statement.TrueSet)
			{
				tmp.Add(new ProcessLineList(enclosedStatement));
			}

			if (!statement.HasOtherwise)
			{
				// make closing parenthesis line
				tmp.Add(new BlockCloseLine(statement, ")", "</trueSet>\r\n</if>"));
			}
			else
			{
				// make closing parenthesis line
				tmp.Add(new BlockCloseLine(statement, ")", "</trueSet>"));

				// make otherwise line
				tmp.Add(new OtherwiseLine(statement, "Otherwise"));

				// make opening parenthesis line
				tmp.Add(new BlockOpenLine(statement, "(", "<falseSet>"));

				foreach (ProcessStatement enclosedStatement in statement.FalseSet)
				{
					tmp.Add(new ProcessLineList(enclosedStatement));
				}

				// make closing parenthesis line
				tmp.Add(new BlockCloseLine(statement, ")", "</falseSet>\r\n</if>"));
			}

			Add(tmp);
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		private void createLineList(GetStatement statement)
		{
			Add(new GetLine(statement));
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		private void createLineList(DeleteStatement statement)
		{
			// make process line from statement
			Add(new DeleteLine(statement));
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		private void createLineList(ForEachRecordStatement statement)
		{
			ProcessLineList tmp = new ProcessLineList();

			// make top line
			tmp.Add(new ForEachRecordLine(statement));

			// make opening parenthesis line
			tmp.Add(new BlockOpenLine(statement, "(", ""));

			foreach (ProcessStatement enclosedStatement in statement.EnclosedStatements)
			{
				tmp.Add(new ProcessLineList(enclosedStatement));
			}

			// make closing parenthesis line
			tmp.Add(new BlockCloseLine(statement, ")", "</foreach>"));

			Add(tmp);
		}

		#region ForEachQuestionStatement unused
#if false
		/// <summary>
		/// Create process line list from ForEachRecord statement
		/// </summary>
		private void createLineList(ForEachQuestionStatement statement)
		{
			ProcessLineList tmp = new ProcessLineList();

			// make top line
			tmp.Add(new ForEachQuestionLine(statement));

			// make opening parenthesis line
			tmp.Add(new BlockOpenLine(statement, "(", ""));

			foreach (ProcessStatement enclosedStatement in statement.EnclosedStatements)
			{
				tmp.Add(new ProcessLineList(enclosedStatement));
			}

			// make closing parenthesis line
			tmp.Add(new BlockCloseLine(statement, ")", "</forEachMc>"));

			Add(tmp);
		}
#endif
		#endregion


		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		private void createLineList(CommentStatement statement)
		{
			Add(new CommentLine(statement));
		}

		/// <summary>
		/// Replace the group identifiers of the lines in the given range
		/// in this list with the specified group identifier
		/// </summary>
		/// <remarks>
		/// Only replaces non-null group identifiers
		/// </remarks>
		private void replaceGroupIDs(int firstIndex, int lastIndex, ProcessStatement oldGroup, ProcessStatement newGroup)
		{
			// for each process line in range...
			for (int i = firstIndex; i <= lastIndex; i++)
			{
				// if line belongs to a group...
				if (this[i].Group != null)
				{
					if (this[i].Group == oldGroup)
					{
						// update group identifier
						this[i].Group = newGroup;
					}
				}
			}
		}

		private void replaceIfLines(int index, IfStatement statement)
		{
			ProcessLine oldLine = this[index];
			IfStatement oldStatement = (IfStatement)oldLine.Statement;

			// get first and last indexes of lines belonging to group
			int firstIndex = firstGroupIndex(oldLine.Group);
			int lastIndex = lastGroupIndex(oldLine.Group);

			// replace top line of if statement
			ProcessLine newLine = new IfLine((IfStatement)statement);
			this[index] = newLine;

			// replace group identifiers for all lines belonging to if statement
			replaceGroupIDs(firstIndex, lastIndex, oldLine.Group, newLine.Group);

			// if "otherwise" clause condition has changed...
			if (oldStatement.HasOtherwise != statement.HasOtherwise)
			{
				ProcessLineList tmp = new ProcessLineList();

				if (statement.HasOtherwise)
				{
					// make new closing parenthesis line
					tmp.Add(new BlockCloseLine(statement, ")", "</trueSet>"));

					// make otherwise line
					tmp.Add(new OtherwiseLine(statement, "Otherwise"));

					// make opening parenthesis line
					tmp.Add(new BlockOpenLine(statement, "(", "<falseSet>"));

					// make closing parenthesis line
					tmp.Add(new BlockCloseLine(statement, ")", "</falseSet>\r\n</if>"));

					// remove existing closing parenthesis line and
					// insert new lines in its place
					this.RemoveAt(lastIndex);
					this.Insert(lastIndex, tmp);
				}
				else
				{
					int closingIndex = indexOfClosingLinePrecedingOtherwise(firstIndex, lastIndex);

					// get count of lines to remove
					int lineCount = lastIndex - closingIndex + 1;

					for (int i = 0; i < lineCount; i++)
					{
						// remove line from "otherwise" clause
						this.RemoveAt(closingIndex);
					}

					// insert new closing parenthesis line
					tmp.Add(new BlockCloseLine(statement, ")", "</trueSet>\r\n</if>"));
					this.Insert(closingIndex, tmp);
				}
			}

		}


		/// <summary>
		/// Add new list to end of this list.
		/// </summary>
		/// <param name="newList">List to add</param>
		public void Add(ProcessLineList newList)
		{
			Insert(Count, newList);
		}

		/// <summary>
		/// Insert new list at specified index in this list.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="newList">List to insert</param>
		public void Insert(int index, ProcessLineList newList)
		{
			// for each line in new list...
			for (int i = newList.Count-1; i >= 0; i--)
			{
				// insert line into this list, bypassing event triggering
				base.InsertItem(index, newList[i]);
			}

			if (raiseEvent() == true)
			{
				// raise "statement added" event
				Project.Events.RaiseStatementAddedEvent(new StatementEventArgs(newList[0].Statement, process));
			}
		}

		/// <summary>
		/// Insert shallow copies of items from new list at specified index in this list.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="newList">List to insert</param>
		public void InsertCopy(int index, ProcessLineList newList)
		{
			ProcessLineList tmpList = new ProcessLineList();
			ProcessStatement destGroup = null;

			// for each line in new list...
			for (int i = 0; i < newList.Count; i++)
			{
				// make copy of line
				ProcessLine destLine = newList[i].Copy();

				if (destLine.SelectsGroup)
				{
					// get group identifier
					destGroup = destLine.Group;
				}
				else
				{
					// set group identifier
					destLine.Group = destGroup;
				}

				tmpList.Add(destLine);
			}

			// for each copied line...
			for (int i = tmpList.Count - 1; i >= 0; i--)
			{
				// insert line into this list, bypassing event triggering
				base.InsertItem(index, tmpList[i]);
			}

			if (raiseEvent() == true)
			{
				// raise "statement added" event
				Project.Events.RaiseStatementAddedEvent(new StatementEventArgs(newList[0].Statement, process));
			}
		}


		/// <summary>
		/// Return a shallow copy of this list.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="newList">List to insert</param>
		public ProcessLineList Copy()
		{
			ProcessLineList copiedList = new ProcessLineList();

			Hashtable groupIDs = new Hashtable();

			// for each line in list...
			foreach (ProcessLine line in this)
			{
				// make copy of line
				ProcessLine copiedLine = line.Copy();

				if (line.SelectsGroup)
				{
					if (!groupIDs.ContainsKey(line.Group))
					{
						// store group ID
						groupIDs.Add(line.Group, copiedLine.Statement);
					}
				}
				
				if (line.Group != null)
				{
					// assign group ID
					copiedLine.Group = (ProcessStatement)groupIDs[line.Group];
				}

				copiedList.Add(copiedLine);
			}

			return copiedList;
		}

		/// <summary>
		/// Indicates whether updating of this list is currently occurring.
		/// </summary>
		private Boolean updating = false;

		private void beginUpdate()
		{
			updating = true;
		}

		private void endUpdate()
		{
			updating = false;
		}

		/// <summary>
		/// Remove all lines belonging to the specified group from list.
		/// </summary>
		/// <param name="index">Index at which to begin looking for lines to remove</param>
		/// <param name="group">Group to remove</param>
		public void Remove(int index, ProcessStatement group)
		{
			int lastIndex = lastGroupIndex(group);

			int lineCount = lastIndex - index + 1;

			beginUpdate();

			while (lineCount-- > 0)
			{
				RemoveAt(index);
			}

			endUpdate();

			// if this is other than a transient line list...
			if (raiseEvent() == true)
			{
				// raise "statement removed" event
				Project.Events.RaiseStatementRemovedEvent(new StatementEventArgs(group, process));
			}
		}

		/// <summary>
		/// Replace line(s) in list with line(s) created from statement.
		/// </summary>
		/// <param name="statement">New statement with which to replace existing one</param>
		public void Replace(int index, ProcessStatement statement)
		{
			if (index >= 0)
			{
				if (statement is AppendStatement)
				{
					this[index] = new AppendLine((AppendStatement)statement);
				}
				else if (statement is ShowDocumentStatement)
				{
					this[index] = new ShowDocumentLine((ShowDocumentStatement)statement);
				}
				else if (statement is ShowFormStatement)
				{
					this[index] = new ShowFormLine((ShowFormStatement)statement);
				}
				else if (statement is ShowRecordStatement)
				{
					this[index] = new ShowRecordLine((ShowRecordStatement)statement);
				}
				else if (statement is ShowUrlStatement)
				{
					this[index] = new ShowUrlLine((ShowUrlStatement)statement);
				}
				else if (statement is SendStatement)
				{
					this[index] = new SendLine((SendStatement)statement);
				}
				else if (statement is IfStatement)
				{
					replaceIfLines(index, (IfStatement)statement);
				}
				else if (statement is SetStatement)
				{
					this[index] = new SetLine((SetStatement)statement);
				}
				else if (statement is ArithmeticStatement)
				{
					this[index] = new ArithmeticLine((ArithmeticStatement)statement);
				}
				else if (statement is SkipToStatement)
				{
					this[index] = new SkipToLine((SkipToStatement)statement);
				}
				else if (statement is GetStatement)
				{
					this[index] = new GetLine((GetStatement)statement);
				}
				else if (statement is DeleteStatement)
				{
					this[index] = new DeleteLine((DeleteStatement)statement);
				}
				else if (statement is ForEachRecordStatement)
				{
					this[index] = new ForEachRecordLine((ForEachRecordStatement)statement);
				}
				#region ForEachQuestionStatement unused
#if false
				else if (statement is ForEachQuestionStatement)
				{
					this[index] = new ForEachQuestionLine((ForEachQuestionStatement)statement);
				}
#endif
				#endregion
				else if (statement is CommentStatement)
				{
					this[index] = new CommentLine((CommentStatement)statement);
				}
				else
				{
					Debug.Assert(false, "Unhandled statement in ProcessLineList:Replace()");
				}
			}
			else
			{
				Debug.Assert(false, "Index out of range in ProcessLineList:Replace()");
			}
		}

		/// <summary>
		/// Get index of first line associated with specified statement.
		/// </summary>
		public int GetIndex(ProcessStatement statement)
		{
			if (statement != null)
			{
				for (int i = 0; i < Count; i++)
				{
					ProcessLine line = this[i];
					// if line is associated with statement...
					if (line.Statement == statement)
					{
						return i;
					}
				}
			}

			return -1;
		}

		/// <summary>
		/// Drag and drop line within list.
		/// </summary>
		public void DragDrop(int sourceIndex, int targetIndex)
		{
			// get source line
			ProcessLine sourceLine = this[sourceIndex];

			ProcessStatement sourceGroup = sourceLine.Group;
			ProcessStatement sourceStatement = sourceLine.Statement;

			// if moving a multi-line statement...
			if (sourceGroup != null && sourceStatement != null)
			{
				// make line list from lines in source group
				ProcessLineList sourceList = new ProcessLineList(sourceIndex, this);

				// if dropping at end of list...
				if (targetIndex == -1)
				{
					// remove source lines
					Remove(sourceIndex, sourceGroup);

					// add source lines at end of list
					Add(sourceList);

				}
				else
				{
					ProcessLine targetLine = this[targetIndex];

					// remove source lines
					Remove(sourceIndex, sourceGroup);

					// get current index of target line
					int newTargetIndex = IndexOf(targetLine);

					// if source line same as target line...
					if (newTargetIndex == -1)
					{
						newTargetIndex = sourceIndex;
					}

					// insert source lines at target location
					Insert(newTargetIndex, sourceList);
				}
			}
			else
			{
				// if dropping at end of list...
				if (targetIndex == -1)
				{
					// remove source line
					Remove(sourceLine);

					// add source line at end of list
					Add(sourceLine);
				}
				else
				{
					// get target line
					ProcessLine targetLine = this[targetIndex];

					// remove source line
					Remove(sourceLine);

					// get current index of target line
					int newTargetIndex = IndexOf(targetLine);

					// if source line same as target line...
					if (newTargetIndex == -1)
					{
						newTargetIndex = sourceIndex;
					}

					// insert source line at current target index
					Insert(newTargetIndex, sourceLine);
				}
			}
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			foreach (ProcessLine line in this)
			{
				sb.Append(line.ToString() + "\r\n");
			}

			return sb.ToString();
		}

		public string ToXml()
		{
			StringBuilder sb = new StringBuilder();

			// for each line in process line list...
			foreach (ProcessLine line in this)
			{
				string xmlString = line.ToXml();

				if (xmlString.Length > 0)
				{
					// append that line's XML
					sb.Append(xmlString + "\r\n");
				}
			}

			return sb.ToString();
		}

		public int LineGroupStartIndex(ProcessLine line)
		{
			return IndexOf(line);
		}

		public int LineGroupEndIndex(ProcessLine line)
		{
			if (line.SelectsGroup)
			{
				return lastGroupIndex(line.Group);
			}
			return IndexOf(line);
		}

		/// <summary>
		/// Get the index of the first line in the list
		/// associated with the specified group.
		/// </summary>
		/// <param name="group">Group in question</param>
		private int firstGroupIndex(ProcessStatement group)
		{
			// for each line in list...
			for (int i = 0; i < Count; i++)
			{
				// if line is part of group in question...
				if (this[i].Group == group)
				{
					return i;
				}
			}

			return -1;
		}

		/// <summary>
		/// Get the index of the last line in the list
		/// associated with the specified group.
		/// </summary>
		/// <param name="group">Group in question</param>
		private int lastGroupIndex(ProcessStatement group)
		{
			// for each line in list, starting from the end...
			for (int i = Count-1; i >= 0; i--)
			{
				// if line is part of group in question...
				if (this[i].Group == group)
				{
					if (i <= 0)
					{
						break;
					}
					return i;
				}
			}

			return -1;
		}

		private int indexOfClosingLinePrecedingOtherwise(int firstLineIndex, int lastLineIndex)
		{
			for (int i = firstLineIndex; i <= lastLineIndex; i++)
			{
				if (this[i] is OtherwiseLine)
				{
					return i - 1;
				}
			}

			return -1;
		}

		public Boolean IsInsideForEach(int processLineIndex)
		{
			return GetEnclosingForEachStatement(processLineIndex) != null;
		}


		/// <summary>
		/// Returns the index of the top ForEachLine that encloses the line indicated by the specified process line index
		/// </summary>
		/// <param name="processLineIndex"></param>
		/// <returns>-1 if process line index not enclosed by ForEachLine</returns>
		private int forEachTopLineIndex(int processLineIndex)
		{
			int enclosingForEachIndex = -1;
			Boolean enclosingForEachFound = false;
			int outdentLevel = 0;
			int i = processLineIndex;

			do
			{
				--i;

				if (i >= 0 && i < this.Count)
				{
					if (this[i] is BlockCloseLine)
					{
						outdentLevel--;
					}
					else if (this[i] is BlockOpenLine)
					{
						outdentLevel++;
					}
					else if (this[i] is IfLine || this[i] is OtherwiseLine)
					{
						outdentLevel = 0;
					}

					if (outdentLevel >= 1 && this[i] is ForEachLine)
					{
						enclosingForEachFound = true;
						enclosingForEachIndex = i;
					}
				}

			} while (i >= 0 && !enclosingForEachFound);

			return enclosingForEachIndex;
		}

		/// <summary>
		/// Returns the index of the BlockCloseLine corresponding to the ForEachLine that encloses the line indicated by the specified process line index
		/// </summary>
		/// <param name="processLineIndex"></param>
		/// <returns>-1 if process line index not enclosed by ForEachLine</returns>
		private int forEachClosingLineIndex(int processLineIndex)
		{
			int index = -1;
			Boolean found = false;
			int parenCount = 0;
			int i = forEachTopLineIndex(processLineIndex);

			do
			{
				if (i >= 0 && i < Count)
				{
					if (this[i] is BlockOpenLine)
					{
						parenCount++;
					}
					else if (this[i] is BlockCloseLine)
					{
						parenCount--;
					}

					if (parenCount == 0 && this[i] is BlockCloseLine)
					{
						found = true;
						index = i;
					}
				}

				i++;

			} while (i < Count && !found);

			return index;
		}

		/// <summary>
		/// Returns the ForEachStatement corresponding to the ForEachLine that encloses the line indicated by the specified process line index
		/// </summary>
		/// <param name="processLineIndex"></param>
		/// <returns>null if process line index not enclosed by ForEachLine</returns>
		public ProcessStatement GetEnclosingForEachStatement(int processLineIndex)
		{
			ProcessStatement forEachStatement = null;

			int firstIndex = forEachTopLineIndex(processLineIndex);
			int lastIndex = forEachClosingLineIndex(processLineIndex);

			if (firstIndex != -1 && lastIndex != -1)
			{
				if (processLineIndex > firstIndex && processLineIndex <= lastIndex)
				{
					forEachStatement = this[firstIndex].Statement as ForEachStatement;
				}
			}

			return forEachStatement;
		}

		public ProcessStatementList GetEnclosingForEachStatements(int processLineIndex)
		{
			ProcessStatementList list = new ProcessStatementList();

			int index = processLineIndex;

			while (index != -1)
			{
				ProcessStatement statement = GetEnclosingForEachStatement(index);

				if (statement != null)
				{
					list.Insert(0, statement);
				}

				index = GetIndex(statement);
			}

			return list;
		}

		/// <summary>
		/// Establishes the indent level for each line in list.
		/// </summary>
		public void SetIndentLevels()
		{
			int indentLevel = 0;

			// for each line in list...
			for (int i = 0; i < Count; i++)
			{
				if (this[i] is BlockCloseLine)
				{
					indentLevel--;
				}

				this[i].IndentLevel = indentLevel;

				if (this[i] is BlockOpenLine)
				{
					indentLevel++;
				}
			}
		}

		protected override void InsertItem(int index, ProcessLine item)
		{
			base.InsertItem(index, item);

			if (item.Statement != null && raiseEvent() == true)
			{
				Project.Events.RaiseStatementAddedEvent(new StatementEventArgs(item.Statement, process));
			}
		}

		protected override void RemoveItem(int index)
		{
			ProcessLine line = this[index];

			base.RemoveItem(index);

			if (line.Statement != null && raiseEvent() == true && !updating)
			{
				// raise "statement removed" event
				Project.Events.RaiseStatementRemovedEvent(new StatementEventArgs(line.Statement, process));
			}
		}

		protected override void SetItem(int index, ProcessLine item)
		{
			base.SetItem(index, item);

			if (item.Statement != null && raiseEvent() == true)
			{
				Project.Events.RaiseStatementModifiedEvent(new StatementEventArgs(item.Statement, process));
			}
		}

		public void ValidateLines()
		{
			for (int i = 0; i < Count; i++)
			{
				this[i].Validate();
			}
		}

		private bool enableEvents = true;

		public bool EnableEvents
		{
			get
			{
				return enableEvents;
			}
			set
			{
				enableEvents = value;
			}
		}

		/// <summary>
		/// determine if we need to raise an event
		/// </summary>
		private bool raiseEvent()
		{
			bool raiseEvent = false;

			if (enableEvents && process != null)
			{
				// if the owning Process is in the Process list for this Project
				if (Project.Current.ProcessList.Contains(process))
				{
					// we want to broadcast
					raiseEvent = true;
				}
				else
				{
					// also if the current component is a Form
					if (Project.Current.CurrentComponent is IForm)
					{
						// and the Form is displaying a Skip Instructions view
						IForm form = Project.Current.CurrentComponent as IForm;

						foreach (IFormItem formItem in form.ItemList)
						{
							if (formItem is ISkipInstructionsItem)
							{
								ISkipInstructionsItem skip = formItem as ISkipInstructionsItem;
								if (process == skip.Instructions)
								{
									raiseEvent = true;
									break;
								}
							}
						}
					}
				}
			}

			return raiseEvent;
		}
	}
}
