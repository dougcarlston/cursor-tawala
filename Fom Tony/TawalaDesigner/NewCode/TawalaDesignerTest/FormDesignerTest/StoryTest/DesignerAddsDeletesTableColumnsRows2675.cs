using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Tawala.FormDesigner;
using System.Windows.Forms;
using TawalaTest.TestingSupport;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.XmlSupport;
using Tawala.Interfaces;
using Tawala.Browser;

namespace TawalaTest.FormDesignerTest.StoryTest
{
	[TestFixture]
	public class DesignerAddsDeletesTableColumnsRows2675
	{
		private ITextItem textItem;
		private IFormPresenter formEditPresenter;
		private FormView formView;
		private BrowserControl browser;

		private HtmlElement table;

		[SetUp]
		public void SetUp()
		{
			Util.NewTestProject();
			setupAndInsertTextItemInView();

			table = insertTableIntoTextItem();
			markCellsWithIdentifyingText();
		}

		[TearDown]
		public void TearDown()
		{
			if (formView != null)
			{
				((System.Windows.Forms.Form)formView).Close();
			}
			textItem = null;
			formEditPresenter = null;
			formView = null;
			browser = null;
		}

		[Test]
		public void InsertingRowBeforeCurrentRowProducesNewRowInHtml()
		{
			int initialRowCount = getRowCount();

			TestUtil.SelectViewText(formView, "R1,C1");

			TestUtil.ClickFormEditViewToolStripButton(formView, "insertRowBeforeToolStripMenuItem");

			Assert.AreEqual(initialRowCount + 1, getRowCount());

			Assert.AreEqual("R1,C1", getCellText(1, 0));
			Assert.AreEqual("R1,C2", getCellText(1, 1));
		}

		[Test]
		public void InsertingRowBeforeCurrentRowGeneratesProperXml()
		{
			TestUtil.SelectViewText(formView, "R1,C1");

			TestUtil.ClickFormEditViewToolStripButton(formView, "insertRowBeforeToolStripMenuItem");

			Project.Events.RaiseSynchronizeProjectEvent();

			string expectedXml =
				@"<text label=""T1"" style=""normal"">" +
				@"<table indent=""0"">" +
				@"<row>" +
				@"<cell width=""2160""><division indent=""0"" align=""left""><font></font></division></cell>" +
				@"<cell width=""2160""><division indent=""0"" align=""left""><font></font></division></cell>" +
				@"</row>" +
				@"<row>" +
				@"<cell width=""2160""><division indent=""0"" align=""left""><font>R1,C1</font></division></cell>" +
				@"<cell width=""2160""><division indent=""0"" align=""left""><font>R1,C2</font></division></cell>" +
				@"</row>" +
				@"<row>" +
				@"<cell width=""2160""><division indent=""0"" align=""left""><font>R2,C1</font></division></cell>" +
				@"<cell width=""2160""><division indent=""0"" align=""left""><font>R2,C2</font></division></cell>" +
				@"</row>" +
				@"</table>" +
				@"[Replace this with text of your own.]" +
				@"</text>" + Environment.NewLine;

			Assert.AreEqual(expectedXml, textItem.ToXml("T1"));
		}

		[Test]
		public void InsertingRowAfterCurrentRowProducesNewRowInHtml()
		{
			int initialRowCount = getRowCount();

			TestUtil.SelectViewText(formView, "R1,C1");

			TestUtil.ClickFormEditViewToolStripButton(formView, "insertRowAfterToolStripMenuItem");

			Assert.AreEqual(initialRowCount + 1, getRowCount());

			Assert.AreEqual("R1,C1", getCellText(0, 0));
			Assert.AreEqual("R1,C2", getCellText(0, 1));
			Assert.AreEqual("R2,C1", getCellText(2, 0));
			Assert.AreEqual("R2,C2", getCellText(2, 1));
		}

		[Test]
		public void InsertingRowAfterCurrentRowGeneratesProperXml()
		{
			TestUtil.SelectViewText(formView, "R1,C1");

			TestUtil.ClickFormEditViewToolStripButton(formView, "insertRowAfterToolStripMenuItem");

			Project.Events.RaiseSynchronizeProjectEvent();

			string expectedXml =
				@"<text label=""T1"" style=""normal"">" +
				@"<table indent=""0"">" +
				@"<row>" +
				@"<cell width=""2160""><division indent=""0"" align=""left""><font>R1,C1</font></division></cell>" +
				@"<cell width=""2160""><division indent=""0"" align=""left""><font>R1,C2</font></division></cell>" +
				@"</row>" +
				@"<row>" +
				@"<cell width=""2160""><division indent=""0"" align=""left""><font></font></division></cell>" +
				@"<cell width=""2160""><division indent=""0"" align=""left""><font></font></division></cell>" +
				@"</row>" +
				@"<row>" +
				@"<cell width=""2160""><division indent=""0"" align=""left""><font>R2,C1</font></division></cell>" +
				@"<cell width=""2160""><division indent=""0"" align=""left""><font>R2,C2</font></division></cell>" +
				@"</row>" +
				@"</table>" +
				@"[Replace this with text of your own.]" +
				@"</text>" + Environment.NewLine;

			Assert.AreEqual(expectedXml, textItem.ToXml("T1"));
		}

		[Test]
		public void DeletingCurrentRowRemovesRowFromHtml()
		{
			int initialRowCount = getRowCount();

			TestUtil.SelectViewText(formView, "R1,C1");

			TestUtil.ClickFormEditViewToolStripButton(formView, "deleteRowToolStripMenuItem");

			Assert.AreEqual(initialRowCount - 1, getRowCount());

			Assert.AreEqual("R2,C1", getCellText(0, 0));
			Assert.AreEqual("R2,C2", getCellText(0, 1));
		}

		[Test]
		public void InsertingColumnBeforeCurrentColumnProducesNewColumnInHtml()
		{
			int initialColumnCount = getColumnCount();

			TestUtil.SelectViewText(formView, "R1,C2");

			TestUtil.ClickFormEditViewToolStripButton(formView, "insertColumnBeforeToolStripMenuItem");

			Assert.AreEqual(initialColumnCount + 1, getColumnCount());

			Assert.AreEqual("R1,C1", getCellText(0, 0));
			Assert.AreEqual("R1,C2", getCellText(0, 2));
		}

		[Test]
		public void InsertingColumnBeforeCurrentColumnGeneratesProperXml()
		{
			TestUtil.SelectViewText(formView, "R1,C1");

			TestUtil.ClickFormEditViewToolStripButton(formView, "insertColumnBeforeToolStripMenuItem");

			Project.Events.RaiseSynchronizeProjectEvent();

			string expectedXml =
				@"<text label=""T1"" style=""normal"">" +
				@"<table indent=""0"">" +
				@"<row>" +
				@"<cell width=""720""><division indent=""0"" align=""left""><font></font></division></cell>" +
				@"<cell width=""2160""><division indent=""0"" align=""left""><font>R1,C1</font></division></cell>" +
				@"<cell width=""2160""><division indent=""0"" align=""left""><font>R1,C2</font></division></cell>" +
				@"</row>" +
				@"<row>" +
				@"<cell width=""720""><division indent=""0"" align=""left""><font></font></division></cell>" +
				@"<cell width=""2160""><division indent=""0"" align=""left""><font>R2,C1</font></division></cell>" +
				@"<cell width=""2160""><division indent=""0"" align=""left""><font>R2,C2</font></division></cell>" +
				@"</row>" +
				@"</table>" +
				@"[Replace this with text of your own.]" +
				@"</text>" + Environment.NewLine;

			Assert.AreEqual(expectedXml, textItem.ToXml("T1"));
		}

		[Test]
		public void InsertingColumnAfterCurrentColumnProducesNewColumnInHtml()
		{
			int initialColumnCount = getColumnCount();

			TestUtil.SelectViewText(formView, "R1,C1");

			TestUtil.ClickFormEditViewToolStripButton(formView, "insertColumnAfterToolStripMenuItem");

			Assert.AreEqual(initialColumnCount + 1, getColumnCount());

			Assert.AreEqual("R1,C1", getCellText(0, 0));
			Assert.AreEqual("R1,C2", getCellText(0, 2));
		}

		[Test]
		public void InsertingColumnAfterCurrentColumnGeneratesProperXml()
		{
			TestUtil.SelectViewText(formView, "R1,C1");

			TestUtil.ClickFormEditViewToolStripButton(formView, "insertColumnAfterToolStripMenuItem");

			Project.Events.RaiseSynchronizeProjectEvent();

			string expectedXml =
				@"<text label=""T1"" style=""normal"">" +
				@"<table indent=""0"">" +
				@"<row>" +
				@"<cell width=""2160""><division indent=""0"" align=""left""><font>R1,C1</font></division></cell>" +
				@"<cell width=""720""><division indent=""0"" align=""left""><font></font></division></cell>" +
				@"<cell width=""2160""><division indent=""0"" align=""left""><font>R1,C2</font></division></cell>" +
				@"</row>" +
				@"<row>" +
				@"<cell width=""2160""><division indent=""0"" align=""left""><font>R2,C1</font></division></cell>" +
				@"<cell width=""720""><division indent=""0"" align=""left""><font></font></division></cell>" +
				@"<cell width=""2160""><division indent=""0"" align=""left""><font>R2,C2</font></division></cell>" +
				@"</row>" +
				@"</table>" +
				@"[Replace this with text of your own.]" +
				@"</text>" + Environment.NewLine;

			Assert.AreEqual(expectedXml, textItem.ToXml("T1"));
		}

		[Test]
		public void DeletingCurrentColumnRemovesColumnFromHtml()
		{
			int initialColumnCount = getColumnCount();

			TestUtil.SelectViewText(formView, "R1,C1");

			TestUtil.ClickFormEditViewToolStripButton(formView, "deleteColumnToolStripMenuItem");

			Assert.AreEqual(initialColumnCount - 1, getColumnCount());

			Assert.AreEqual("R1,C2", getCellText(0, 0));
		}

		private void setupAndInsertTextItemInView()
		{
			IForm form = Tawala.Projects.Project.Current.AddForm();

			formView = new FormView(form);
			formEditPresenter = formView.Presenter;

            browser = TestUtil.PumpMessagesUntilUIReady(formView) as BrowserControl;

			formEditPresenter.InsertTextItem(0);
			textItem = form.ItemList[0] as ITextItem;
		}

		private HtmlElement insertTableIntoTextItem()
		{
			HtmlElement div = getTextItemDiv();
			TestUtil.SetHtmlElementFocus(div);
            browser.InsertTable(216, 2, 2);
			return div.GetElementsByTagName("TABLE")[0];
		}

		private HtmlElement getTextItemDiv()
		{
			HtmlElementCollection tags = browser.Document.GetElementsByTagName("TextItem");
			Assert.AreEqual(1, tags.Count);

			HtmlElement element = tags[0];
			HtmlElement div = element.Children[0];
			return div;
		}

		private void markCellsWithIdentifyingText()
		{
			int rowNum = 1, columnNum = 1;

			foreach (HtmlElement row in table.GetElementsByTagName("TR"))
			{
				foreach (HtmlElement cell in row.GetElementsByTagName("TD"))
				{
					cell.InnerText = string.Format("R{0},C{1}", rowNum, columnNum);
					columnNum++;
				}
				rowNum++;
				columnNum = 1;
			}
		}

		private int getRowCount()
		{
			return getTableRows().Count;
		}

		private int getColumnCount()
		{
			return getTableRows()[0].GetElementsByTagName("TD").Count;
		}

		private HtmlElementCollection getTableRows()
		{
			return table.GetElementsByTagName("TR");
		}

		private string getCellText(int rowIndex, int columnIndex)
		{
			return getTableRows()[rowIndex].GetElementsByTagName("TD")[columnIndex].InnerText;
		}
	}
}
