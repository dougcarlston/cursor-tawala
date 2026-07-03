// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using Tawala.FormDesigner;
using System.Windows.Forms;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.Projects.Forms.NewFormItems;
using TawalaTest.TestingSupport;
using Tawala.Interfaces;

namespace TawalaTest.FormDesignerTest
{
	internal static class TestUtil
	{
		internal static WebBrowser GetWebBrowser(IFormView view)
		{
			return Reflect<FormView>.GetField<WebBrowser>("browser", view as FormView);
		}

		internal static void SelectFormItem(IFormView view, IFormItem formItem)
		{
			HtmlElement label = view.GetHtmlElementById("label_" + formItem.Id);
			label.Focus();
			label.RaiseEvent("onmousedown");
			label.RaiseEvent("onmouseup");
		}

		internal static void SelectFormItemContents(IFormView view, IFormItem formItem)
		{
			HtmlElement element = view.GetHtmlElementById(formItem.Id.ToString());
			element.Focus();
			element.RaiseEvent("onmousedown");
			element.RaiseEvent("onmouseup");
		}

		internal static void SelectViewText(IFormView view, string text)
		{
			WebBrowser browser = GetWebBrowser(view);
			string scriptFormat = @"var range = document.body.createTextRange(); range.findText('{0}'); range.select();";
			string script = string.Format(scriptFormat, text);
			browser.Document.InvokeScript("fnEval", new object[] { script });
		}

		internal static void PositionCursorAtStartOfText(IFormView view, string text)
		{
			WebBrowser browser = GetWebBrowser(view);
			string scriptFormat = @"var range = document.body.createTextRange(); range.findText('{0}'); range.select(); var selRange = document.selection.createRange(); selRange.collapse();";
			string script = string.Format(scriptFormat, text);
			browser.Document.InvokeScript("fnEval", new object[] { script });
		}

		internal static void SetBookmark(IFormView view, string text)
		{
			WebBrowser browser = GetWebBrowser(view);
			string scriptFormat = @"var range = document.body.createTextRange(); range.findText('{0}'); range.select(); range.pasteHTML('<t:bookmark/>');";
			string script = string.Format(scriptFormat, text);
			browser.Document.InvokeScript("fnEval", new object[] { script });
		}

		internal static void SetHtmlElementFocus(HtmlElement element)
		{
			PumpMessages();
			element.Focus();
			PumpMessages();
		}

		internal static void PumpMessages()
		{
			Application.DoEvents();

			System.Threading.Thread.Sleep(10);

			Application.DoEvents();
		}

		internal static WebBrowser PumpMessagesUntilUIReady(IFormView view)
		{
			Application.DoEvents();

			while (!isBrowserReady(view))
			{
				Application.DoEvents();
			}

			Application.DoEvents();
			return GetWebBrowser(view);
		}

		private static bool isBrowserReady(IFormView view)
		{
			WebBrowser browser = GetWebBrowser(view);

			if (browser == null)
				return false;

			HtmlDocument document = browser.Document;

			if (document == null)
				return false;

			if (document.Body == null)
				return false;

			return !browser.IsBusy && browser.ObjectForScripting != null;
		}

		public static void ClickFormEditViewToolStripButton(IFormView view, string buttonName)
		{
			Reflect<FormView>.GetField<ToolStripItem>(buttonName, view as FormView).PerformClick();
		}
	}
}
