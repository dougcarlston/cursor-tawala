// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;

namespace Tawala.DesignerUI
{
    internal class ProjectPresenter
    {
        private IProjectExplorerView view;

        public ProjectPresenter(IProjectExplorerView view)
        {
            this.view = view;
        }
    }

    internal interface IProjectExplorerView
    {
        ProjectNode SelectedProjectNode { get; }
    }
}