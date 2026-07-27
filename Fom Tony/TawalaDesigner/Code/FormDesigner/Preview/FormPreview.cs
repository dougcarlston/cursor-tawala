using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

using Tawala.Common;

namespace Tawala.FormDesigner.Preview
{
	[ComVisibleAttribute(true)]
	public partial class FormPreview : WebBrowser
	{
		private BackgroundGetFormPreviewUrl backgroundGetPreviewUrl;

		public FormPreview()
		{
			InitializeComponent();

			if (components == null)
			{
				components = new Container();
			}

			ObjectForScripting = this;

			backgroundGetPreviewUrl = new BackgroundGetFormPreviewUrl(this);
			components.Add(backgroundGetPreviewUrl);
		}

		internal void SetPreviewForm(Proj.IForm previewForm)
		{
			form = previewForm;
		}

		private Proj.IForm form = null;
		private string htmlAnchor = string.Empty;

        internal void Activate(string anchor)
        {
            htmlAnchor = anchor;

			enableChangeHandler(true);

			Application.DoEvents();
			tryPreview(Properties.Resources.FormPreviewRefresh1);
			Application.DoEvents();

			try
			{
				// can sometimes throw exception
				AllowWebBrowserDrop = false;
			}
			catch (Exception)
			{
			}
			IsWebBrowserContextMenuEnabled = false;
			WebBrowserShortcutsEnabled = false;
		}


		internal void Deactivate()
		{
			enableChangeHandler(false);
		}

		protected override void OnHandleDestroyed(EventArgs e)
		{
			try
			{
				enableChangeHandler(false);
				base.OnHandleDestroyed(e);
			}
			catch
			{
			}
		}

		private void enableChangeHandler(bool bEnabled)
		{
			if (bEnabled)
			{
				Tawala.Proj.Project.Events.ThemeChanged += update_Preview;
				Tawala.Proj.Project.Events.PageHeaderChanged += update_Preview;
			}
			else
			{
				Tawala.Proj.Project.Events.ThemeChanged -= update_Preview;
				Tawala.Proj.Project.Events.PageHeaderChanged -= update_Preview;
			}
		}

		private bool InDesignMode
		{
			get { return form == null; }
		}

		private void update_Preview(object sender, EventArgs e)
		{
			tryPreview(Properties.Resources.FormPreviewRefresh1);			
		}

		public void HTMLRefreshButton_Clicked()
		{
			tryPreview(Properties.Resources.FormPreviewRefresh2);
		}

		public void SetHtml(string html)
		{
			AllowNavigation = true;
			DocumentText = html;
		}

		public void SetUrl(string url)
		{
			AllowNavigation = true;
			string realUrl = string.Format("{0}?r={1}{2}", url, DateTime.Now.Ticks, htmlAnchor);
			Debug.WriteLine(realUrl);
			
			Navigate(realUrl);
		}

		protected override void OnDocumentCompleted(WebBrowserDocumentCompletedEventArgs e)
		{
			base.OnDocumentCompleted(e);

			AllowNavigation = false;
		}

		private void tryPreview(string retryHtml)
		{
			try
			{
				if (backgroundGetPreviewUrl.Busy)
				{
					backgroundGetPreviewUrl.Cancel();
					SetHtml(retryHtml);
				}
				else
				{
					SetHtml(Properties.Resources.FormPreviewGenerating);
					backgroundGetPreviewUrl.RunAsync(form.Name);
				}
			}
			catch (Exception e)
			{
				SetFormPreviewExceptionHtml(e);
			}
		}

		public void SetFormPreviewExceptionHtml(Exception e)
		{
			StringBuilder sb = new StringBuilder(Properties.Resources.FormPreviewException);
			sb.Replace("$MESSAGE$", e.Message);

			SetHtml(sb.ToString());
		}
	}
}
