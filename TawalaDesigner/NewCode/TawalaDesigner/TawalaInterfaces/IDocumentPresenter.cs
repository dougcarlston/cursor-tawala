// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.Functions.ViewPresenter;
using Tawala.Projects.Documents;

namespace Tawala.Interfaces
{
    public interface IDocumentPresenter
    {
        IDocumentView View { get; }
        IDocument Document { get; set; }

        void ViewInitializationCompleted();
        void ViewActivated();
        void ViewClosed();
        void ViewDeactivated();

        void InsertFunction(FunctionConfiguredEventArgs args);
        void UpdateFunction(FunctionConfiguredEventArgs args);

        void SynchonizeModelWithView();
    }
}