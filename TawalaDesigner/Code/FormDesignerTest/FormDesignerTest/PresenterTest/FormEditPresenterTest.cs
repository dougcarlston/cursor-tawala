using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Tawala.FormDesigner;
using Tawala.Proj;
using TawalaTest.TestSupport;
using Tawala.Interfaces;

namespace TawalaTest.FormDesignerTest.PresenterTest
{
	[TestFixture]
	public class FormEditPresenterTest
	{
		private IForm form;
		private FormView view;
		private IFormPresenter presenter;
		private WebBrowser webBrowser;

		[SetUp]
		public void SetUp()
		{
			Util.NewTestProject();

			form = Project.Current.AddForm();

			view = new FormView(form);
			presenter = view.Presenter;
			webBrowser = TestUtil.PumpMessagesUntilUIReady(view);
		}

        [TearDown]
        public void TearDown()
        {
            view.Close();
            view = null;
            webBrowser = null;
            presenter = null;
            form = null;
        }

		[Test]
		public void GetUniqueIdReturnsUniqueIdString()
		{
			string idString1 = presenter.GetUniqueId();
			string idString2 = presenter.GetUniqueId();

			Assert.AreNotEqual(idString1, idString2);
		}

		[Test]
		public void GetBlankIdReturnsIdStringFromBlankHtml()
		{
			string blankHtml =
				@"<?import namespace = t urn = ""tawala"" implementation = " +
				@"""file:///C:/Steve/Work/JDF/20Q/SVN/Code/FormDesigner/bin/Debug/html/fi_blank.htc"" " +
				@"declareNamespace />" +
				@"<t:Blank id=1011><INPUT class=blank style=""HEIGHT: 21px"" value=Q1:a></t:Blank>";

			Assert.AreEqual("1011", presenter.GetBlankId(blankHtml));
		}
	}
}
