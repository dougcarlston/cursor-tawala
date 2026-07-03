using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Tawala.FormDesigner;
using System.Windows.Forms;
using TawalaTest.TestingSupport;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.Interfaces;

namespace TawalaTest.FormDesignerTest
{
	public class UIOrientedTestBase : CommonBase
	{
		protected FormView formView;
		protected IFormPresenter formEditPresenter;
		protected HtmlDocument document;
		protected HtmlElement body;
		protected WebBrowser browser;

		protected void UITestSetUp()
		{
			CommonSetUp();

			IForm form = Tawala.Projects.Project.Current.AddForm();
			
			formView = new FormView(form);

			formEditPresenter = formView.Presenter;

			browser = TestUtil.PumpMessagesUntilUIReady(formView);

			document = browser.Document;
			body = document.Body;
		}

		protected void UITestTearDown()
		{
			if (formView != null)
			{
				formView.Close();
				formView = null;
			}
			formEditPresenter = null;
			document = null;
			body = null;
			browser = null;
		}

		protected static T GetFormItem<T>()
			where T : class, IFormItem
		{
			if (Project.Current.FormList.Count == 0 || Project.Current.FormList[0].ItemList.Count == 0)
				return null;

			return Project.Current.FormList[0].ItemList[0] as T;
		}

		protected static T GetFormItem<T>(int itemIndex)
			where T : class, IFormItem
		{
			if (Project.Current.FormList.Count == 0 || Project.Current.FormList[0].ItemList.Count == 0)
				return null;

			return Project.Current.FormList[0].ItemList[itemIndex] as T;
		}

		protected void VerifyItemTag(string name)
		{
			VerifyItemTag(body, name);
		}
	}

	public class ModelOrientedTestBase : CommonBase
	{
		protected IForm form;
		protected WebBrowser browser;
		protected FormView formView;
		protected HtmlElement body;

		protected void ModelTestSetUp()
		{
			CommonSetUp();

			form = Tawala.Projects.Project.Current.AddForm();
		}

		protected void ModelTestTearDown()
		{
			if (formView != null)
			{
				formView.Close();
				formView = null;
			}
			body = null;
			form = null;
		}

		protected void CreateViewFromForm()
		{
			formView = new FormView(form);

			browser = TestUtil.PumpMessagesUntilUIReady(formView);

			body = browser.Document.Body;
		}

		protected void VerifyItemTag(string name)
		{
			VerifyItemTag(body, name);
		}
	}

	public class CommonBase
	{
		protected void CommonSetUp()
		{
			TestingSupport.Util.NewTestProject();
		}

		protected static void VerifyItemTag(HtmlElement body, string name)
		{
			string expectedHtml = "<t:" + name + " ";
			string actualHtml = Regex.Match(body.InnerHtml, @"<t:" + name + @" ").Value;
			Assert.AreEqual(expectedHtml, actualHtml);

			expectedHtml = "</t:" + name + ">";
			actualHtml = Regex.Match(body.InnerHtml, @"</t:" + name + @">").Value;
			Assert.AreEqual(expectedHtml, actualHtml);
		}
	}
}
