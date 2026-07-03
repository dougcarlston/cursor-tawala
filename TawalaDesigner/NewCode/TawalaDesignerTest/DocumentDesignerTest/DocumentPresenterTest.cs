using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

using NUnit.Framework;
using NMock2;

using Tawala.DocumentDesigner;
using Tawala.Projects;
using Tawala.Projects.Documents;
using Tawala.Functions.Runtime;
using Tawala.Functions.Controls;
using Tawala.Functions.ViewPresenter;
using Tawala.Interfaces;
using TawalaTest.TestingSupport;


namespace TawalaTest.DocumentDesignerTest
{
	[TestFixture]
	public class DocumentPresenterTest
	{
		IDocumentPresenter presenter;
		IDocument document;
		IDocumentView view;
		Mockery mockery = new Mockery();
		
		[SetUp]
		public void SetUp()
		{
			mockery = new Mockery(); 
			document = new NewDocument("Document 1");
			view = mockery.NewMock<IDocumentView>();
			presenter = new DocumentPresenter(view, document);
		}

		[TearDown]
		public void TearDown()
		{
			mockery.VerifyAllExpectationsHaveBeenMet();
			document = null;
			view = null;
			presenter = null;
			mockery.Dispose();
			mockery = null;
		}

		[Test]
		public void InsertFunctionCallViewInsertFunction()
		{
			IFunction function = FunctionLoader.Current.Functions["record-count"].CreateInstance();
			Expect.Once.On(view).Method("InsertFunction").With(function.InstanceId, function.ToDisplayString());

			FunctionConfiguredEventArgs args = new FunctionConfiguredEventArgs(function, function, false);
			presenter.InsertFunction(args);
		}

        [Test]
        public void UpdateFunctionCallViewIUpdateFunction()
        {
            IFunction function = FunctionLoader.Current.Functions["record-count"].CreateInstance();
            Expect.Once.On(view).Method("UpdateFunction").With(function.InstanceId, function.InstanceId, function.ToDisplayString());

            FunctionConfiguredEventArgs args = new FunctionConfiguredEventArgs(function, function, false);
            presenter.UpdateFunction(args);
        }
    }
}
