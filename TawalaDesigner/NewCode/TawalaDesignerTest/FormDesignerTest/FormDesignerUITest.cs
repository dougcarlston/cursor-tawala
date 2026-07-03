using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Tawala.FormDesigner;
using System.Windows.Forms;
using TawalaTest.TestingSupport;
using Tawala.Projects;
using Tawala.Projects.Forms;

namespace TawalaTest.FormDesignerTest
{
	[TestFixture]
	public class FormDesignerUITest : UIOrientedTestBase
	{
		[SetUp]
		public void SetUp()
		{
			UITestSetUp();
		}

		[TearDown]
		public void TearDown()
		{
			UITestTearDown();
		}

		[Test]
		public void FormEditViewHasPresenter()
		{
			Assert.IsNotNull(formEditPresenter);
		}

		[Test]
		public void PresenterHasWebBrowserAndDocument()
		{
			Assert.IsNotNull(browser);
			Assert.IsNotNull(document);
			Assert.IsNotNull(body);
		}

		[Ignore ("resurrect after View base class is MDIComponentView or MDIComponentForm")]
		public void FormEditViewTagIsProjectForm()
		{
			Assert.IsTrue(formView.Tag is IForm);
		}
	}
}
