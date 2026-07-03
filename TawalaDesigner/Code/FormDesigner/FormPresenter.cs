// Copyright © 2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using Tawala.Common;
using Tawala.FormDesigner.FormItemOptions;
using Tawala.Functions.Runtime;
using Tawala.Proj;
using Tawala.XmlSupport;
using Tawala.ProjectUI;
using Tawala.Proj.Forms.FormItemContents;
using Tawala.Proj.Forms.NewFormItems;
using Tawala.Proj.Factories;
using Tawala.Interfaces;

namespace Tawala.FormDesigner
{
	public class FormPresenter : IFormPresenter
	{
		public const int InsertAtEnd = -1;
		public const int InsertAtInsertionPoint = -2;

		public FormPresenter(IFormView view, IForm form)
		{
			Form = form;
			View = view;
		}

		#region IFormPresenter

		public void ViewInitializationCompleted()
		{
			Project.Events.SynchronizeProject += projectEvents_SynchronizeProject;

			addExistingFormItemsToView();

			Project.Events.FormChanged += events_FormChanged;
			Project.Events.FormItemChanged += events_FormItemChanged;
		}

		private void addExistingFormItemsToView()
		{
			foreach (var formItem in Form.ItemList)
			{
				invokePrivateMethod("insertFormItemIntoView", formItem, InsertAtEnd);
			}
		}

		public IFormItem GetFormItem(string htmlId)
		{
			return Form.GetFormItem(Convert.ToInt32(htmlId));
		}

		public void InsertHeadingItem(int index)
		{
			IHeadingItem headingItem = new NewHeadingItem();
			insertFormItemIntoModel(headingItem, index);
			insertFormItemIntoView(headingItem, index);
		}

		public void InsertTextItem(int index)
		{
			ITextItem textItem = new NewTextItem();
			insertTextItemIntoModelAndView(textItem, index);
		}

		private void insertTextItemIntoModelAndView(ITextItem textItem, int index)
		{
			insertFormItemIntoModel(textItem, index);
			insertFormItemIntoView(textItem, index);
		}

		public void InsertFibItem(int index)
		{
			NewFibItem fibItem = new NewFibItem();
			
			insertFormItemIntoModel(fibItem, index);
			fibItem.InsertBlanksIntoFieldMapByName();

			insertFormItemIntoView(fibItem, index);
		}

		public void InsertBlank(string fibHtmlId)
		{
			NewFibItem fibItem = GetFormItem(fibHtmlId) as NewFibItem;

			string blankElementHtml = createBlankElementHtml();
			View.InsertBlank(blankElementHtml);

			syncFormItemToModel(fibItem);
			fibItem.InsertBlanksIntoFieldMapByName();

			updateAllLabelsInView();

			Project.Events.RaiseFormItemChangedEvent(new FormItemEventArgs(null, fibItem, -1));
		}

		private const int BLANK_PIXELS_PER_LINE = 21;
		private const int DEFAULT_BLANK_WIDTH = 20;

		private string createBlankElementHtml()
		{
			string elementHtml = View.CreateBlank();

			string inputHtml = String.Format("<INPUT class=\"blank\" size=\"{0}\" style=\"HEIGHT:{1}px;\" />", DEFAULT_BLANK_WIDTH, BLANK_PIXELS_PER_LINE);
			elementHtml = Regex.Replace(elementHtml, "></t:Blank", ">" + inputHtml + "</t:Blank");

			return elementHtml;
		}

		public void InsertMcqItem(int index)
		{
			IMcqItem mcqItem = new NewMcqItem();
			insertMcqItemIntoModelAndView(mcqItem, index);
		}

		private void insertMcqItemIntoModelAndView(IMcqItem mcqItem, int index)
		{
			insertFormItemIntoModel(mcqItem, index);
			insertFormItemIntoView(mcqItem, index);
		}

		public void InsertFieldItem(int index)
		{
			IHiddenField fieldItem = new NewHiddenField();
			insertFormItemIntoModel(fieldItem, index);
			insertFormItemIntoView(fieldItem, index);
		}

		public void InsertSkipItem(int index)
		{
			ISkipInstructionsItem skipItem = new NewSkipInstructionsItem();
			insertFormItemIntoModel(skipItem, index);
			insertFormItemIntoView(skipItem, index);
		}

		public void InsertBreakItem(int index)
		{
			BreakItem breakItem = new BreakItem();
			insertFormItemIntoModel(breakItem, index);
			insertFormItemIntoView(breakItem, index);
		}

		public void InsertField(IField field)
		{
			View.InsertField(field.Id, field.ToString());
		}

		public void InsertFunction(IFunction function)
		{
			Project.FunctionMapById.AddUnique(function);
			View.InsertFunction(function.InstanceId, function.ToDisplayString());
		}

		public void InsertImage(string imageName)
		{
			View.InsertImage(imageName);
		}

        public string CreateFormPreviewAnchor(string firstVisibleFormItemId)
        {
            const string htmlAnchorPrefix = "#anchor-";
            IFormItem formItem = GetFormItem(firstVisibleFormItemId);

            while (formItem != null)
            {
                if (formItem is ITextItem || formItem is IHeadingItem || formItem is IMcqItem)
                    return htmlAnchorPrefix + Form.ItemList.GetDefaultLabel(formItem);

                if (formItem is IFibItem)
                {
                    IFibItem fib = formItem as IFibItem;
                    if (fib.BlankList.Count > 0)
                    {
                        return htmlAnchorPrefix + fib.BlankList[0].FieldName;
                    }
                }

                int index = Form.ItemList.IndexOf(formItem) + 1;
                if (Form.ItemList.Count <= index)
                    break;

                formItem = Form.ItemList[index];
            }

            return string.Empty;
        }

		public void CopyFormItems(Collection<string> ids)
		{
			synchronizeFormItems();

			IFormItem[] formItems = new IFormItem[ids.Count];
			int index = 0;

			foreach (string id in ids)
			{
				IFormItem formItem = Cloner.Clone<FormItem>((FormItem)GetFormItem(id));
				formItem.ClearId();
				formItems[index++] = formItem;
			}

			DataObject dataObject = new DataObject();
			dataObject.SetData(formItems.GetType(), formItems);
			Clipboard.SetDataObject(dataObject);
		}

		public void CutFormItems(Collection<string> ids)
		{
			synchronizeFormItems();

			IFormItem[] formItems = new IFormItem[ids.Count];
			int index = 0;

			foreach (string id in ids)
			{
				IFormItem formItem = GetFormItem(id);
				deleteItem(id);
				formItem.ClearId();
				formItems[index++] = formItem;
			}

			updateAllLabelsInView();

			DataObject dataObject = new DataObject();
			dataObject.SetData(formItems.GetType(), formItems);
			Clipboard.SetDataObject(dataObject);
		}

		public void DeleteFormItems(Collection<string> ids)
		{
			foreach (string id in ids)
			{
				deleteItem(id);
			}

			updateAllLabelsInView();
		}

		public void PasteFormItems()
		{
			IDataObject dataObject = Clipboard.GetDataObject();

			if (dataObject.GetDataPresent(typeof(IFormItem[])))
			{
				IFormItem[] items = dataObject.GetData(typeof(IFormItem[])) as IFormItem[];
				foreach (IFormItem item in items)
				{
					fixUpAlternateLabelBeforePasting(item);
					insertFormItemIntoModel(item, View.GetInsertionIndex());
					invokePrivateMethod("insertFormItemIntoView", item, InsertAtInsertionPoint);
				}
			}
		}

		public bool ContainsTextItem(Collection<string> ids)
		{
			foreach (string id in ids)
			{
				if (GetFormItem(id) is ITextItem)
				{
					return true;
				}
			}

			return false;
		}

		public bool ContainsOnlyOneTextItem(Collection<string> ids)
		{
			int count = 0;

			foreach (string id in ids)
			{
				if (GetFormItem(id) is ITextItem)
				{
					count++;
				}
			}

			return count == 1;
		}

		public void SetStyleOfTextItems(Collection<string> ids, string style)
		{
			foreach (string id in ids)
			{
				ITextItem textItem = GetFormItem(id) as ITextItem;
				if (textItem != null)
				{
					textItem.Style = style;
				}
			}
		}

		public void SetStyleOfAllTextItems(string style)
		{
			Project.Current.SetAllTextItemStyles(style);
		}

		public string GetStyleOfSelectedTextItem(Collection<string> ids)
		{
			if (ContainsOnlyOneTextItem(ids))
			{
				foreach (string id in ids)
				{
					ITextItem textItem = GetFormItem(id) as ITextItem;
					if (textItem != null)
					{
						return textItem.Style;
					}
				}
			}

			return string.Empty;
		}

		public bool ContainsFibItem(Collection<string> ids)
		{
			foreach (string id in ids)
			{
				if (GetFormItem(id) is IFibItem)
				{
					return true;
				}
			}

			return false;
		}

		public bool ContainsOnlyOneFibItem(Collection<string> ids)
		{
			int count = 0;

			foreach (string id in ids)
			{
				if (GetFormItem(id) is IFibItem)
				{
					count++;
				}
			}

			return count == 1;
		}

		public bool SelectedFibItemsHaveSameStyle(Collection<string> ids)
		{
			string firstStyle = string.Empty;

			foreach (string id in ids)
			{
				IFibItem fibItem = GetFormItem(id) as IFibItem;

				if (fibItem != null)
				{
					if (firstStyle == string.Empty)
					{
						firstStyle = fibItem.Style;
					}
					else if (fibItem.Style != firstStyle)
					{
						return false;
					}
				}
			}

			return firstStyle != string.Empty;
		}

		public void SetStyleOfFibItems(Collection<string> ids, string style)
		{
			foreach (string id in ids)
			{
				IFibItem fibItem = GetFormItem(id) as IFibItem;

				if (fibItem != null)
				{
					fibItem.Style = style;
				}
			}
		}

		public void SetStyleOfAllFibItems(string style)
		{
			Project.Current.SetAllFibStyles(style);
		}

		public string GetStyleOfFirstSelectedFibItem(Collection<string> ids)
		{
			foreach (string id in ids)
			{
				IFibItem fibItem = GetFormItem(id) as IFibItem;
				if (fibItem != null)
				{
					return fibItem.Style;
				}
			}

			return string.Empty;
		}

		public bool ContainsMcqItem(Collection<string> ids)
		{
			foreach (string id in ids)
			{
				if (GetFormItem(id) is IMcqItem)
				{
					return true;
				}
			}

			return false;
		}

		public bool ContainsOnlyOneMcqItem(Collection<string> ids)
		{
			int count = 0;

			foreach (string id in ids)
			{
				if (GetFormItem(id) is IMcqItem)
				{
					count++;
				}
			}

			return count == 1;
		}

		public void SetStyleOfMcqItems(Collection<string> ids, string style)
		{
			foreach (string id in ids)
			{
				IMcqItem mcqItem = GetFormItem(id) as IMcqItem;

				if (mcqItem != null)
				{
					mcqItem.Style = style;
				}
			}
		}

		public void SetStyleOfMcqItems(Collection<string> ids, string style, int columnCount)
		{
			foreach (string id in ids)
			{
				IMcqItem mcqItem = GetFormItem(id) as IMcqItem;

				if (mcqItem != null)
				{
					mcqItem.Style = style;
					mcqItem.ColumnCount = columnCount;
				}
			}
		}

		public void SetStyleOfAllMcqItems(string style)
		{
			Project.Current.SetAllMCQStyles(style);
		}

		#region IFormEditPresenter Members


		public void SetStyleOfAllMcqItems(string style, int columnCount)
		{
			Project.Current.SetAllMCQStyles(style, columnCount);
		}

		#endregion

		public string GetStyleOfSelectedMcqItem(Collection<string> ids)
		{
			if (ContainsOnlyOneMcqItem(ids))
			{
				foreach (string id in ids)
				{
					IMcqItem mcqItem = GetFormItem(id) as IMcqItem;

					if (mcqItem != null)
					{
						return mcqItem.Style;
					}
				}
			}

			return string.Empty;
		}

		public int GetColumnCountOfSelectedMcqItem(Collection<string> ids)
		{
			if (ContainsOnlyOneMcqItem(ids))
			{
				foreach (string id in ids)
				{
					IMcqItem mcqItem = GetFormItem(id) as IMcqItem;

					if (mcqItem != null)
					{
						return mcqItem.ColumnCount;
					}
				}
			}

			return 0;
		}


		public void FormItemsMoved()
		{
			Collection<string> ids = View.GetFormItemIds();

			for (int i = 0; i < ids.Count; ++i)
			{
				var formItem = GetFormItem(ids[i]);

				if (Form.ItemList[i].Id == formItem.Id)
					continue;

				Form.ItemList.Move(formItem, i);
			}

			updateAllLabelsInView();
		}

		public void UpdateMcqChoicesInView(string htmlId)
		{
			IMcqItem mcqItem = GetFormItem(htmlId) as IMcqItem;

			HtmlElement choicesElement = View.GetHtmlElementById(View.GetAttribute(htmlId, "ChoicesId"));
			choicesElement.InnerHtml = mcqItem.ChoicesXhtml;
		}

		public IFormView View { get; protected set; }
		public IForm Form { get; set; }

		public string GetUniqueId()
		{
			return Convert.ToString(Project.NextUniqueID);
		}

		public string GetBlankId(string blankHtml)
		{
			string blankXhtml = XmlUtility.ToXhtml(blankHtml);
			IXmlElement blankXmlElement = new XmlElement(blankXhtml);

			return (blankXmlElement.HasAttribute("id") ? blankXmlElement.GetAttribute("id") : GetUniqueId());
		}

		public void ViewClosed()
		{
			Project.Events.SynchronizeProject -= projectEvents_SynchronizeProject;
			Project.Events.FormChanged -= events_FormChanged;
			Project.Events.FormItemChanged -= events_FormItemChanged;
		}

		public void MakeFormActiveComponent()
		{
			Project.Current.SetCurrentComponent(Form);
		}

		#endregion

		private void fixUpAlternateLabelBeforePasting(IFormItem formItem)
		{
			if (!string.IsNullOrEmpty(formItem.AlternateLabel))
			{
				Form.ItemList.ContainsAlternateLabel(formItem.AlternateLabel);
				formItem.AlternateLabel = string.Empty;
			}

			if (formItem is IFibItem)
			{
				IFibItem fibItem = formItem as IFibItem;

				foreach (IBlank blank in fibItem.BlankList)
				{
					if (!string.IsNullOrEmpty(blank.AlternateLabel))
					{
						Form.ItemList.ContainsAlternateLabel(blank.AlternateLabel);
						blank.AlternateLabel = string.Empty;
					}

				}
			}
		}

		#region Insert Form Item into View methods - must have same name as invoked by reflection

		private void insertFormItemIntoView(IHeadingItem headingItem, int index)
		{
			View.InsertHeadingItem(headingItem.NewContents.ToXhtml(headingItem), adjustInsertionIndex(index), headingItem.Id, headingItem.HeadingType.ToString());

			updateAllLabelsInView();
		}

		private void insertFormItemIntoView(IHiddenField fieldItem, int index)
		{
			View.InsertFieldItem(fieldItem.Name, adjustInsertionIndex(index), fieldItem.Id);
		}

		private void insertFormItemIntoView(IMcqItem mcqItem, int index)
		{
			IFormItemContents question = mcqItem.Question;
			string questionHtml = question.ToXhtml(mcqItem).Replace("<question>","").Replace("</question>","");

			//FormItemContentsCollection choices = mcqItem.NewContents.GetDescendants(typeof(IChoice));
			//string choicesHtml = choices.ToXhtml(mcqItem);
			string choicesHtml = mcqItem.ChoicesXhtml;

			View.InsertMcqItem(questionHtml, choicesHtml, adjustInsertionIndex(index), mcqItem.Id);
			updateAllLabelsInView();
		}

		private void insertFormItemIntoView(ITextItem textItem, int index)
		{
			View.InsertTextItem(textItem.NewContents.ToXhtml(textItem), adjustInsertionIndex(index), textItem.Id);
			updateAllLabelsInView();
		}

		private void insertFormItemIntoView(NewFibItem fibItem, int index)
		{
			View.InsertFibItem(fibItem.NewContents.ToXhtml(fibItem), adjustInsertionIndex(index), fibItem.Id);
			updateAllLabelsInView();
		}

		private void insertFormItemIntoView(BreakItem breakItem, int index)
		{
			View.InsertBreakItem(adjustInsertionIndex(index), breakItem.Id);
		}

		private void insertFormItemIntoView(ISkipInstructionsItem skipItem, int index)
		{
			View.InsertSkipItem(adjustInsertionIndex(index), skipItem.Id);
			View.SetAttribute(skipItem.Id.ToString(), "Summary", skipItem.GetSummary());
		}

		#endregion

		private void invokePrivateMethod(string method, params object[] args)
		{
			GetType().InvokeMember(method, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod, null, this, args);
		}

		private void deleteItem(string htmlId)
		{
			IFormItem formItem = GetFormItem(htmlId);
			Form.ItemList.Remove(formItem);
		}

		private int adjustInsertionIndex(int index)
		{
			if (index == InsertAtInsertionPoint)
			{
				return View.GetInsertionIndex();
			}

			return index;
		}

		private void events_FormItemChanged(object sender, FormItemEventArgs e)
		{
			updateAllLabelsInView();
			updateTextInAllFields();
		}

		private void events_FormChanged(object sender, ComponentEventArgs e)
		{
			updateTextInAllFields();
		}

		private void updateTextInAllFields()
		{
			HtmlElementCollection collection = View.GetElementsByTagName("field");

			foreach (HtmlElement element in collection)
			{
				int fieldId = Convert.ToInt32(element.GetAttribute("FieldID"));

				if (Project.FieldMapById.ContainsKey(fieldId))
				{
					IAnyField field = Project.FieldMapById[fieldId];
					element.SetAttribute("FieldName", field.QualifiedFieldName);
				}
			}
		}

		private void updateAllFunctionsInView()
		{
			HtmlElementCollection collection = View.GetElementsByTagName("function");

			foreach (HtmlElement element in collection)
			{
				int functionId = Convert.ToInt32(element.Id.Replace("func_", ""));
				IFunction function = Project.FunctionMapById[functionId];
				element.InnerText = function.ToDisplayString().Replace("<<","").Replace(">>","");
			}
		}

		private void updateAllLabelsInView()
		{
			foreach (var formItem in Form.ItemList)
			{
				setItemLabelText(formItem);

				if (formItem is NewFibItem)
				{
					setAllBlankLabels(formItem as NewFibItem);
				}
			}
			
			updateAllFunctionsInView();
		}

		private void setItemLabelText(IFormItem formItem)
		{
			View.SetLabelText(formItem.Id, getItemLabelFromModel(formItem));
		}

		private string getItemLabelFromModel(IFormItem formItem)
		{
			string labelText = formItem.AlternateLabel;
			if (labelText.Equals(string.Empty))
			{
				labelText = Form.GetDefaultLabel(formItem);
			}
			return labelText;
		}

		private void setAllBlankLabels(NewFibItem fibItem)
		{
			HtmlElement fibElement = View.GetHtmlElementById(fibItem.Id.ToString());

			if (fibElement != null)
			{
				HtmlElementCollection blankElements = getElementsByTagName(fibElement, "Blank");

				for (int i = 0; i < blankElements.Count; i++)
				{
					blankElements[i].SetAttribute("Label", fibItem.BlankList[i].FieldName);
				}
			}
		}

		private void projectEvents_SynchronizeProject(object sender, EventArgs e)
		{
			if (!Project.CreatingProject)
			{
				synchronizeFormItems();
			}
		}

		private void synchronizeFormItems()
		{
			foreach (IFormItem formItem in Form.ItemList)
			{
				invokePrivateMethod("syncFormItemToModel", formItem);
			}
		}

		private HtmlElementCollection getElementsByTagName(HtmlElement parentElement, string tagName)
		{
			return parentElement.GetElementsByTagName(tagName);
		}

		#region Synchronization Methods - Must have same method name as call through reflection

		private void syncFormItemToModel(IHeadingItem headingItem)
		{
			headingItem.NewContents = FormItemContentsFactory.MakeChildrenFromHtml(View.GetHtmlElementById(headingItem.Id.ToString()).InnerHtml);
		}

		private void syncFormItemToModel(ITextItem textItem)
		{
			textItem.NewContents = FormItemContentsFactory.MakeChildrenFromHtml(View.GetHtmlElementById(textItem.Id.ToString()).InnerHtml);
		}

		private void syncFormItemToModel(IMcqItem mcqItem)
		{
			mcqItem.NewContents = FormItemContentsFactory.MakeChildrenFromHtml(View.GetAttribute(mcqItem.Id.ToString(), "Contents"));
		}

		private void syncFormItemToModel(NewFibItem fibItem)
		{
			fibItem.NewContents = FormItemContentsFactory.MakeChildrenFromHtml(View.GetHtmlElementById(fibItem.Id.ToString()).InnerHtml);
		}

		private void syncFormItemToModel(IHiddenField fieldItem)
		{
			fieldItem.Name = View.GetAttribute(fieldItem.Id.ToString(), "FieldName");
		}

		private void syncFormItemToModel(BreakItem BreakItem)
		{
		}

		private void syncFormItemToModel(ISkipInstructionsItem skipItem)
		{
		}

		#endregion

		private void insertFormItemIntoModel(IFormItem formItem, int index)
		{
			switch (index)
			{
				case InsertAtInsertionPoint:
					int insertAt = View.GetInsertionIndex();
					insertAt = insertAt < 0 ? Form.ItemList.Count : insertAt;
					Form.ItemList.Insert(insertAt, formItem);
					break;

				case InsertAtEnd:
					Form.ItemList.Add(formItem);
					break;

				default:
					Form.ItemList.Insert(index, formItem);
					break;
			}
		}

		# region Options Panel Methods

		private IFormItem currentOptionsItem;
		private Control currentOptionsView;

		public void SetItemOptions(string htmlId, Control optionsView)
		{
			currentOptionsView = optionsView;

			if (htmlId == null || optionsView == null)
			{
				currentOptionsItem = null;
				return;
			}

			currentOptionsItem = GetFormItem(htmlId);
			if (optionsView is McqOptionsView)
			{
				McqOptionsView mcqOptionsView = optionsView as McqOptionsView;
				IMcqItem mcItem = currentOptionsItem as IMcqItem;

				mcqOptionsView.SelectMoreThanOne = !mcItem.SelectOnlyOne;
				mcqOptionsView.ResponseRequired = mcItem.RequireAtLeastOne;
			}
		}

		public void SetMcqItemOptions(IMcqItem mcqItem, IMcqOptionsView mcqOptionsView)
		{
			currentOptionsItem = mcqItem;
			currentOptionsView = mcqOptionsView as Control;
		}

		public void UpdateMcqOptionsInView(Control optionsView)
		{
			if (currentOptionsItem == null)
			{
				return;
			}

			McqOptionsView mcqOptionsView = optionsView as McqOptionsView;
			
			if (mcqOptionsView != null)
			{
				IMcqItem mcqItem = currentOptionsItem as IMcqItem;
				HtmlElement choicesElement = View.GetHtmlElementById(View.GetAttribute(mcqItem.Id.ToString(), "ChoicesId"));
				choicesElement.InnerHtml = mcqItem.Choices.ToXhtml(mcqItem);
			}
		}

		#endregion
	}

}
