// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Windows.Forms;
using Tawala.DesignerUI.Properties;
using Tawala.Projects;

namespace Tawala.DesignerUI
{
    internal class PageHeaderPresenter : IPageHeaderPresenter
    {
        private IPageHeaderView view;

        public PageHeaderPresenter(IPageHeaderView view)
        {
            this.view = view;
            view.Text = Project.Current.PageHeader.Text;
            view.ImageLocation = Project.Current.PageHeader.GetImage();
        }

        #region IPageHeaderPresenter Members

        public void BrowseForImage()
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = Resources.PageHeaderOpenImageFilter;

            if (ofd.ShowDialog(view) == DialogResult.OK)
            {
                view.ImageLocation = ofd.FileName;
            }
        }

        public void RemoveImage()
        {
            view.ImageLocation = null;
        }

        public void OK()
        {
            Project.Current.PageHeader.Text = view.Text;
            Project.Current.PageHeader.SetImage(view.ImageLocation);
            Project.Events.RaisePageHeaderChangedEvent();
        }

        #endregion
    }
}