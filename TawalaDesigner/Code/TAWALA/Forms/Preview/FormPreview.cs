// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Tawala.Forms.Properties;
using Tawala.Projects;
using Form=Tawala.Projects.Form;

namespace Tawala.Forms
{
    [ComVisible(true)]
    public partial class FormPreview : WebBrowser
    {
        private readonly BackgroundGetFormPreviewUrl backgroundGetPreviewUrl;

        private Form form;
        private string htmlAnchor = string.Empty;

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

        private bool InDesignMode { get { return form == null; } }

        internal void SetPreviewForm(Form previewForm)
        {
            form = previewForm;
        }

        internal void Activate(string anchor)
        {
            htmlAnchor = anchor;

            enableChangeHandler(true);

            Application.DoEvents();
            tryPreview(Resources.FormPreviewRefresh1);
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
                Project.Events.ThemeChanged += update_Preview;
                Project.Events.PageHeaderChanged += update_Preview;
            }
            else
            {
                Project.Events.ThemeChanged -= update_Preview;
                Project.Events.PageHeaderChanged -= update_Preview;
            }
        }

        private void update_Preview(object sender, EventArgs e)
        {
            tryPreview(Resources.FormPreviewRefresh1);
        }

        public void HTMLRefreshButton_Clicked()
        {
            tryPreview(Resources.FormPreviewRefresh2);
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
                    SetHtml(Resources.FormPreviewGenerating);
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
            var sb = new StringBuilder(Resources.FormPreviewException);
            sb.Replace("$MESSAGE$", e.Message);

            SetHtml(sb.ToString());
        }
    }
}