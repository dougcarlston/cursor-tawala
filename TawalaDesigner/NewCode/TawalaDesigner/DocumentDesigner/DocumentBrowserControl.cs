using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Tawala.Browser;
using Tawala.Interfaces;

namespace Tawala.DocumentDesigner
{
	public class DocumentBrowserControl : BrowserControl
	{
		public void LoadDocument()
		{
			ObjectForScripting = new DocumentScriptingObject(Parent as IDocumentView);

			LoadDocument(getHtmlFilePath());
		}

		public void ViewSource(IWin32Window owner)
		{
            new SourceView(documentContainer.OuterHtml).ShowDialog(owner);
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string Contents
		{
			get { return documentContainer != null ? documentContainer.OuterHtml : null; }
			set { documentContainer.InnerHtml = value; }
		}

		private static string getHtmlFilePath()
		{
			if (isExecutingInTestMode())
			{
				return getTestModeHtmlFilePath();
			}
			
			return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"html\document.htm");
		}

		private static string getTestModeHtmlFilePath()
		{
			return Path.Combine(Path.GetTempPath(), @"FormDesignerTest\html\document.htm");
		}

		private static bool isExecutingInTestMode()
		{
			return Assembly.GetEntryAssembly() == null;
		}

		protected override void OnDocumentCompleted(WebBrowserDocumentCompletedEventArgs e)
		{
			documentContainer = GetElementById("documentContainer");

			base.OnDocumentCompleted(e);
		}

		private HtmlElement documentContainer;
	}
}
