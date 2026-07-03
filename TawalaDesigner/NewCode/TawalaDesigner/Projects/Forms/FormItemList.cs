// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections;
using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.Projects.Forms.FormItemContents;

namespace Tawala.Projects.Forms
{
	/// <summary>
	/// Maintains a collection of FormItems.
	/// If it is owned by a Form it will fire events off of Project.Events --
	/// the FormItemAdded and FormItemRemoved events
	/// </summary>
	[Serializable]
	public class FormItemList : Collection<IFormItem>, IFormItem
	{
		private IForm form;
		private FormItemMap map;

		public FormItemList()
		{
			map = new FormItemMap(this);
		}

		public FormItemList(IForm form)
		{
			this.form = form;
			map = new FormItemMap(this);
		}

		public new bool Contains(IFormItem item)
		{
			return map.Contains(item);
		}

		public bool Contains(int id)
		{
			return map.Contains(id);
		}

		public IFormItem GetItemById(int formItemId)
		{
			return (IFormItem) (map[formItemId] ?? Project.FieldMapById[formItemId]);
		}

		public string GetDefaultLabel(IFormItem item)
		{
			return map[item];
		}

		private static readonly string xmlItemsStartTag = "<items>\r\n";
		private static readonly string xmlItemsEndTag = "</items>\r\n";

		public string ToXml()
		{
			StringBuilder xmlString = new StringBuilder();

			if (Count > 0)
			{
				xmlString.Append(xmlItemsStartTag);

				foreach (IFormItem formItem in Items)
				{
					if (formItem is IHeadingItem)
					{
						IHeadingItem headingItem = formItem as IHeadingItem;
						xmlString.Append(headingItem.ToXml(map[headingItem]));
					}
					else if (formItem.IsTextItem)
					{
						ITextItem textItem = formItem as ITextItem;
						xmlString.Append(textItem.ToXml(map[textItem]));
					}
					else if (formItem.IsQuestionItem)
					{
						if (formItem is IFibItem)
						{
                            IFibItem fibItem = formItem as IFibItem;
							xmlString.Append(fibItem.ToXml(map[fibItem]));
						}
						else if (formItem is IMcqItem)
						{
							IMcqItem mcItem = formItem as IMcqItem;
							xmlString.Append(mcItem.ToXml(map[mcItem]));
						}
						else
						{
							Debug.Assert(false);
						}
					}
					else
					{
						xmlString.Append(formItem.ToXml());
					}
				}

				xmlString.Append(xmlItemsEndTag);
			}

			return xmlString.ToString();
		}

		/// <summary>
		/// handles pasting of Form Items
		/// to insure that duplicate Alternate Labels are not generated
		/// </summary>
		public void Paste(int index, IFormItem item)
		{
			if (!string.IsNullOrEmpty(item.AlternateLabel))
			{
				string newName = item.AlternateLabel;

				// check existing question/text labels
				for (int count = 1; ContainsAlternateLabel(newName); ++count)
				{
					// if match was found, create new label with numeric suffix
					newName = item.AlternateLabel + count;
				}

				// if that's not already the item's label
				if (newName.CompareTo(item.AlternateLabel) != 0)
				{
					// assign it
					item.AlternateLabel = newName;
				}
			}

			if (item is IFibItem)
			{
				// follow the same procedure for blanks in FIBs
				IFibItem fibItem = item as IFibItem;

				foreach (IBlank b in fibItem.BlankList)
				{
					if (!string.IsNullOrEmpty(b.AlternateLabel))
					{
						string newName = b.AlternateLabel;
						for (int count = 1; ContainsAlternateLabel(newName); ++count)
						{
							newName = b.AlternateLabel + count;
						}

						if (newName.CompareTo(b.AlternateLabel) != 0)
						{
							b.AlternateLabel = newName;
						}
					}
				}
			}

			Insert(index, item);
		}

		public void Move(IFormItem item, int targetIndex)
		{
			// if dropping at end of list...
			if (targetIndex >= Count)
			{
				// move item to end of list
				Remove(item);
				Add(item);
			}
			else
			{
				if (IndexOf(item) != targetIndex)
				{
					IFormItem targetItem = this[targetIndex];
					Remove(item);

					int newTargetIndex = IndexOf(targetItem);
					Insert(newTargetIndex, item);
				}
			}
		}


		/// <summary>
		/// returns true if the passed name is already in use as an Alternate Label in this list
		/// </summary>
		public bool ContainsAlternateLabel(string name)
		{
			foreach (IFormItem item in this)
			{
				// check question/text labels
				if (name.CompareTo(item.AlternateLabel) == 0)
					return true;

				if (item is NewFibItem)
				{
					NewFibItem fibItem = item as NewFibItem;

					foreach (IBlank blank in fibItem.BlankList)
					{
						if (blank.AlternateLabel.CompareTo(name) == 0)
							return true;
					}
				}
			}

			return false;
		}

		protected override void ClearItems()
		{
			foreach (IFormItem item in Items)
			{
				item.Eliminate();
				item.Form = null;
			}
			base.ClearItems();

			map = new FormItemMap(this);

			Project.Events.RaiseFormChangedEvent(new ComponentEventArgs(form));
		}

		protected override void InsertItem(int index, IFormItem item)
		{
			base.InsertItem(index, item);
			map.Inserted(item, index);

			if (item.Form != form)
			{
				item.Form = form;
			}


			if (form != null)
			{
				if (!Project.CreatingProject)
				{
					Project.Events.RaiseFormItemAddedEvent(new FormItemEventArgs(form, (IFormItem)item, index));
					Project.Events.RaiseFormChangedEvent(new ComponentEventArgs(form));
				}
			}
		}

		protected override void SetItem(int index, IFormItem newItem)
		{
			IFormItem oldItem = this[index];
			base.SetItem(index, newItem);
			map.SetItem(oldItem, newItem);
		}

		protected override void RemoveItem(int index)
		{
			IFormItem item = this[index];
			item.Form = null;

			base.RemoveItem(index);

			map.Removed(item, index);

			if (form != null)
			{
				Project.Events.RaiseFormItemRemovedEvent(new FormItemEventArgs(form, (FormItem)item, index));
				Project.Events.RaiseFormChangedEvent(new ComponentEventArgs(form));
			}
		}

		/// <summary>
		/// determines if a label is valid and, if so, sets it in the passed Form item
		/// </summary>
		/// <param name="item">the Form item</param>
		/// <param name="blank">a blank if test a label for a FIBItem blank, otherwise null</param>
		/// <param name="label">the proposed label</param>
		/// <returns>
		/// true if the label was valid and therefore set in the item
		/// </returns>
		public bool ValidAlternateLabel(IFormItem item, IBlank blank, string label)
		{
			Debug.Assert((blank == null) || (blank != null && item is IFibItem));

			bool validLabel = true;

			// strip off leading and trailing whitespace because existing labels won't have them
			string testLabel = label.Trim();

			// first, make sure that the proposed label is not a duplicate in this list
			if (this.Contains(item))
			{
				foreach (IFormItem compItem in this)
				{
					if (compItem != item &&
						compItem.AlternateLabel.Length > 0 &&
						compItem.AlternateLabel == testLabel)
					{
						// if a match is found, we can't allow a duplicate
						validLabel = false;
						break;
					}

                    IFibItem fibItem = compItem as IFibItem;
                    if (fibItem != null)
					{
						// check blanks' labels, too
                        foreach (IBlank compBlank in fibItem.BlankList)
						{
							if (compBlank != blank &&
								compBlank.AlternateLabel.Length > 0 &&
								compBlank.AlternateLabel == testLabel)
							{
								validLabel = false;
								break;
							}
						}
					}

					if (validLabel == false)
					{
						break;
					}
				}
			}

			if (validLabel)
			{
				// next, check to see that the name is not already in use by a Process variable
				foreach (Variable var in Project.Current.AllVariables)
				{
					if (var.FieldName == testLabel)
					{
						validLabel = false;
						break;
					}
				}
			}

#if false
			if (validLabel)
			{
				// also check to see if it duplicates a document name
				foreach (Document doc in Project.Current.RealOrVirtualDocumentList)
				{
					if (doc.Name == testLabel)
					{
						validLabel = false;
						break;
					}
				}
			}
#endif

			// now check for illegal name formats
			if (validLabel && testLabel.Length > 0)
			{
				//validLabel = Project.Current.ValidLabelFormat(testLabel);
				validLabel = Project.Current.ValidFieldLabelFormat(testLabel);
			}

			return validLabel;
		}


		#region IField Interface

		public string FieldName
		{
			get
			{
				return "";
			}
		}

		public string FieldString
		{
			get
			{
				return "";
			}
		}

		public IField this[string name]
		{
			get
			{
				foreach (IField field in this.RecursiveEnumerator)
				{
					if (field.FieldName == name)
					{
						return field;
					}
				}

				// field name not found
				return null;
			}
		}

		private int id = Project.NextUniqueID;

		public int Id
		{
			get { return id; }
		}


		#endregion

		#region IRecursiveEnumerable Interface

		public IEnumerable RecursiveEnumerator
		{
			get
			{
				foreach (IField item in Items)
				{
					foreach (IField field in item.RecursiveEnumerator)
					{
						yield return field;
					}
				}
			}
		}

		#endregion

		[Serializable]
		private class FormItemMap
		{
			public FormItemMap(FormItemList list)
			{
				formItemList = list;

				if (list.Count != 0)
				{
					updateMap();
				}
			}

			public void SetItem(IFormItem oldItem, IFormItem newItem)
			{
				if (oldItem != newItem)
				{
					string label = string.Empty;

					if (oldItem != null)
					{
						label = formItemToDefaultLabel[oldItem];
						formItemToDefaultLabel.Remove(oldItem);
						idToFormItem.Remove(oldItem.Id);
					}

					if (areSameDefaultLabelType(oldItem, newItem))
					{
						formItemToDefaultLabel[newItem] = label;
						idToFormItem[newItem.Id] = newItem;
					}
					else
					{
						updateMap();
					}
				}

				Debug.Assert(formItemList.Count == formItemToDefaultLabel.Keys.Count);
				Debug.Assert(formItemList.Count == idToFormItem.Keys.Count);
			}

			private static bool areSameDefaultLabelType(IFormItem oldItem, IFormItem newItem)
			{
				if (oldItem == null || newItem == null)
				{
					return false;
				}

				if (oldItem is IHeadingItem && newItem is IHeadingItem)
				{
					return true;
				}

				if (oldItem.IsTextItem && newItem.IsTextItem)
				{
					return true;
				}

				if (oldItem.IsQuestionItem && newItem.IsQuestionItem)
				{
					return true;
				}

				return false;
			}

			public void Inserted(IFormItem item, int itemListIndex)
			{
				if (itemListIndex == formItemList.Count - 1 || !hasDefaultLabel(item))
				{
					formItemToDefaultLabel[item] = getNextLabel(item);
					idToFormItem[item.Id] = item;
				}
				else
				{
					updateMap();
				}

				//Debug.Assert(formItemList.Count == formItemToDefaultLabel.Keys.Count);
				//Debug.Assert(formItemList.Count == idToFormItem.Keys.Count);
			}

			public void Removed(IFormItem item, int itemListIndex)
			{
				formItemToDefaultLabel.Remove(item);
				idToFormItem.Remove(item.Id);

				if (hasDefaultLabel(item))
				{
					if (itemListIndex < formItemList.Count)
					{
						updateMap();
					}
					else
					{
						if (item is IHeadingItem)
						{
							nextHeadingLabelNum--;
						}
						else if (item.IsQuestionItem)
						{
							nextQuestionLabelNum--;
						}
						else if (item.IsTextItem)
						{
							nextTextLabelNum--;
						}
					}
				}
				Debug.Assert(formItemList.Count == formItemToDefaultLabel.Keys.Count);
				Debug.Assert(formItemList.Count == idToFormItem.Keys.Count);
			}

			public bool Contains(IFormItem item)
			{
				return item == null? false : formItemToDefaultLabel.ContainsKey(item);
			}

			public bool Contains(int id)
			{
				return idToFormItem.ContainsKey(id);
			}

			public string this[IFormItem item]
			{
				get { return formItemToDefaultLabel.ContainsKey(item)? formItemToDefaultLabel[item] : string.Empty; }
			}

			public IFormItem this[int id]
			{
				get { return idToFormItem.ContainsKey(id) ? idToFormItem[id] : null; }
			}

			private bool hasDefaultLabel(IFormItem item)
			{
				return item.IsTextItem || item.IsQuestionItem || item is IHeadingItem;
			}

			private string getNextLabel(IFormItem item)
			{
				if (item is IHeadingItem)
				{
					return "H" + nextHeadingLabelNum++;
				}
				else if (item.IsTextItem)
				{
					return "T" + nextTextLabelNum++;
				}
				else if (item.IsQuestionItem)
				{
					return "Q" + nextQuestionLabelNum++;
				}
				else
				{
					return string.Empty;
				}
			}

			private void updateMap()
			{
				nextTextLabelNum = 1;
				nextQuestionLabelNum = 1;
				nextHeadingLabelNum = 1;

				foreach (IFormItem item in formItemList)
				{
					formItemToDefaultLabel[item] = getNextLabel(item);
					idToFormItem[item.Id] = item;
				}
			}

			private Dictionary<IFormItem, string> formItemToDefaultLabel = new Dictionary<IFormItem, string>();
			private Dictionary<int, IFormItem> idToFormItem = new Dictionary<int, IFormItem>();
			private FormItemList formItemList = null;
			private int nextTextLabelNum = 1;
			private int nextQuestionLabelNum = 1;
			private int nextHeadingLabelNum = 1;
		}

		#region IFormItem Members

		public IForm Form
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		public bool IsTextItem
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsQuestionItem
		{
			get { throw new NotImplementedException(); }
		}

		public string AlternateLabel
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		public bool Selected
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		public void Eliminate()
		{
			throw new NotImplementedException();
		}

		public string Style
		{
			get	{ throw new NotImplementedException(); }
			set	{ throw new NotImplementedException(); }
		}

		public void ClearId()
		{
			throw new NotImplementedException();
		}

		public Conditions DisplayConditions
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public bool HasDisplayConditions
		{
			get { throw new NotImplementedException(); }
		}

		public void ResolveFieldReferences()
		{
			foreach (IFormItem item in this)
			{
				item.ResolveFieldReferences();
			}
		}

		public void ResolveFunctionReferences()
		{
			foreach (IFormItem item in this)
			{
				item.ResolveFunctionReferences();
			}
		}

		#endregion

		#region IAnyField Members


		public string QualifiedFieldName
		{
			get { throw new NotImplementedException(); }
		}

		#endregion

	}
}
