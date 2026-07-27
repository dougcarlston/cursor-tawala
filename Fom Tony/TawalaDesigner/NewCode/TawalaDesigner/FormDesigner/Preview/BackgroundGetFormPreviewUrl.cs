using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using System.Threading;
using Tawala.Common;
using Tawala.Projects;

namespace Tawala.FormDesigner.Preview
{
	internal class BackgroundGetFormPreviewUrl : System.ComponentModel.Component
	{
		private BackgroundWorker worker = null;
		private bool cancelling = false;

		public class AuthenticationFailedException : Exception
		{
			public AuthenticationFailedException()
				: base("Authentication Failed")
			{
			}
		}

		internal BackgroundGetFormPreviewUrl(FormPreview control)
		{
			preview = control;

			worker = new BackgroundWorker();
			worker.WorkerSupportsCancellation = true;
			worker.DoWork += worker_DoWork;
			worker.RunWorkerCompleted += worker_RunWorkerCompleted;
		}

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

			XMLTransceiver transceiver = new XMLTransceiver(Config.ClientURL);

			StringBuilder xmlString = new StringBuilder();
			xmlString.AppendFormat(xmlQueryFormat, previewFormName, GlobalSettings.CredentialsElement(), projectXml);
			transceiver.Transmit(xmlString.ToString());

			// get back the result
			// remember we don't need to catch exceptions here
			StringReader result = new StringReader(transceiver.Receive());
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
					WebRequestFailure failureForm = new WebRequestFailure();
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

		public bool Busy
		{
			get { return worker.IsBusy; }
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

		private string previewFormName = "";
		private FormPreview preview = null;
		private string projectXml = null;

		private static string xmlQueryFormat = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
									"<request type=\"previewForm\" form=\"{0}\" protocol=\"1.0\">\r\n" +
									"{1}{2}</request>";


		private static readonly XPathExpression xpSuccess = XPathExpression.Compile("/response[@status = 'success']");
		private static readonly XPathExpression xpErrorAuthenticate = XPathExpression.Compile("/response/error[@id = 'auth.failed']");

		protected override void Dispose(bool disposing)
		{
			if (disposing && (worker != null))
			{
				worker.Dispose();
				worker = null;
			}
			base.Dispose(disposing);
		}
	}
}
