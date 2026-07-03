// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using Tawala.Common;
using Tawala.Projects;
using Component=System.ComponentModel.Component;

namespace Tawala.Forms
{
    internal class BackgroundGetFormPreviewUrl : Component
    {
        private static readonly XPathExpression xpErrorAuthenticate = XPathExpression.Compile("/response/error[@id = 'auth.failed']");
        private static readonly XPathExpression xpSuccess = XPathExpression.Compile("/response[@status = 'success']");

        private static string xmlQueryFormat = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                                               "<request type=\"previewForm\" form=\"{0}\" protocol=\"1.0\">\r\n" + "{1}{2}</request>";

        private readonly FormPreview preview;

        private bool cancelling;
        private string previewFormName = "";
        private string projectXml;
        private BackgroundWorker worker;

        internal BackgroundGetFormPreviewUrl(FormPreview control)
        {
            preview = control;

            worker = new BackgroundWorker();
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
        }

        public bool Busy { get { return worker.IsBusy; } }

        internal bool RunAsync(string formName)
        {
            previewFormName = formName;

            Project.Events.RaiseSynchronizeProjectEvent();
            projectXml = Project.Current.ToXml();
            return run();
        }

        // This runs on background thread and must not access UI.

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = null;

            var transceiver = new XMLTransceiver(Config.ClientURL);

            var xmlString = new StringBuilder();
            xmlString.AppendFormat(xmlQueryFormat, previewFormName, GlobalSettings.CredentialsElement(), projectXml);
            transceiver.Transmit(xmlString.ToString());

            // get back the result
            // remember we don't need to catch exceptions here
            var result = new StringReader(transceiver.Receive());
            XPathNavigator xml = new XPathDocument(result).CreateNavigator();

            if (xml.SelectSingleNode(xpSuccess) != null)
            {
                e.Result = xml.SelectSingleNode("//@url").Value;
            }
            else if (xml.SelectSingleNode(xpErrorAuthenticate) != null)
            {
                throw new AuthenticationFailedException();
            }
            else
            {
                throw new XmlException("Unexpected Xml returned from server.");
            }
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (e.Cancelled || cancelling)
                {
                    return;
                }

                if (e.Error is XmlException)
                {
                    var failureForm = new WebRequestFailure();
                    failureForm.ErrorMessage = e.Error.Message;

                    // if Retry button was pressed
                    if (failureForm.ShowDialog(Application.OpenForms[0]) == DialogResult.Retry)
                    {
                        run();
                    }
                }
                else if (e.Error is AuthenticationFailedException)
                {
                    if (GlobalSettings.PromptForCredentials(Application.OpenForms[0]) == DialogResult.OK)
                    {
                        run();
                    }
                }
                else if (e.Error != null)
                {
                    preview.SetFormPreviewExceptionHtml(e.Error);
                }
                else if (e.Error == null && e.Result is string)
                {
                    preview.SetUrl(e.Result as string);
                }
            }
            catch (Exception exception)
            {
                preview.SetFormPreviewExceptionHtml(exception);
            }
            finally
            {
                cancelling = false;
            }
        }

        public void Cancel()
        {
            cancelling = true;
        }

        private bool run()
        {
            if (worker.IsBusy)
            {
                cancelling = true;
                return false;
            }
            else
            {
                cancelling = false;
                worker.RunWorkerAsync();
                return true;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (worker != null))
            {
                worker.Dispose();
                worker = null;
            }
            base.Dispose(disposing);
        }

        #region Nested type: AuthenticationFailedException

        public class AuthenticationFailedException : Exception
        {
            public AuthenticationFailedException() : base("Authentication Failed")
            {
            }
        }

        #endregion
    }
}