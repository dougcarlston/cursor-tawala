// Copyright © 2008 Tawala Systems, Inc. All rights reserved.

using System;
using Tawala.Projects;
using Tawala.Projects.Factories;
using Tawala.Projects.Documents;
using Tawala.Interfaces;
using Tawala.Functions.ViewPresenter;
using Tawala.Functions.Runtime;

namespace Tawala.DocumentDesigner
{
	public class DocumentPresenter : IDocumentPresenter
	{
		public DocumentPresenter(IDocumentView view, IDocument document)
		{
			View = view;
			Document = document;
		}

		#region IDocumentPresenter Members

		public IDocumentView View { get; protected set; }
		public IDocument Document { get; set; }

		public void ViewInitializationCompleted()
		{
			if (Document.NewContents != null)
			{
				View.Contents = Document.NewContents.ToXhtml(null);
			}

		}

        public void ViewActivated()
        {
            Project.Events.SynchronizeProject += project_Synchronize;
        }

        public void ViewDeactivated()
        {
            SynchonizeModelWithView();
            Project.Events.SynchronizeProject -= project_Synchronize;
        }

        public void ViewClosed()
        {
            View = null;

        }

		public void InsertFunction(FunctionConfiguredEventArgs args)
		{
            if (!args.Canceled)
            {
                IFunction function = args.EditedInstance;
                Project.FunctionMapById.AddUnique(function);
                View.InsertFunction(function.InstanceId, function.ToDisplayString());
            }
		}

		public void UpdateFunction(FunctionConfiguredEventArgs args)
		{
			if (!args.Canceled)
			{
				IFunction originalInstance = args.OriginalInstance;
				IFunction updatedInstance = args.EditedInstance;

				Project.FunctionMapById.Remove(originalInstance.InstanceId);
				Project.FunctionMapById.AddUnique(updatedInstance);
				View.UpdateFunction(originalInstance.InstanceId, updatedInstance.InstanceId, updatedInstance.ToDisplayString());
			}
		}

		public void SynchonizeModelWithView()
		{
			if (View.Contents != null)
			{
				Document.NewContents = FormItemContentsFactory.MakeChildrenFromHtml(View.Contents);
			}
		}

		#endregion

        private void project_Synchronize(object sender, EventArgs e)
        {
            SynchonizeModelWithView();
        }

    }
}
