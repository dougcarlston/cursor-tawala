// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Windows.Forms;

namespace Tawala.DesignerUI
{
    public interface INewProjectPresenter
    {
        void Initialize(string templatesPath);
        void PopulateCategories();
        void SelectCategory(string categoryId);
        void SelectedTemplateChanged();
        bool EmptyTemplateSelected();
        string OK(ListViewItem lviSelected);
    }
}