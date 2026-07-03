// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Tawala.DesignerUI.Properties;

namespace Tawala.DesignerUI
{
    public partial class PageHeaderDialog : Form, IPageHeaderView
    {
        private readonly string groupBoxImageDefaultText;
        private readonly IPageHeaderPresenter presenter;
        private string imageFile;

        public PageHeaderDialog()
        {
            InitializeComponent();

            groupBoxImageDefaultText = groupBoxImage.Text;

            presenter = new PageHeaderPresenter(this);
        }

        #region IPageHeaderView Members

        string IPageHeaderView.Text { get { return textBoxHeader.Text; } set { textBoxHeader.Text = value; } }

        string IPageHeaderView.ImageLocation { get { return imageFile; } set { changeImage(value); } }

        #endregion

        private void buttonOK_Click(object sender, EventArgs e)
        {
            presenter.OK();
        }

        private void buttonBrowseImage_Click(object sender, EventArgs e)
        {
            presenter.BrowseForImage();
        }

        private void buttonRemoveImage_Click(object sender, EventArgs e)
        {
            presenter.RemoveImage();
        }

        private void changeImage(string newFile)
        {
            panelImage.SuspendLayout();

            if (pictureBox.Image != null)
            {
                pictureBox.Image.Dispose();
                pictureBox.Image = null;
            }

            imageFile = string.IsNullOrEmpty(newFile) ? null : newFile;

            if (imageFile != null)
            {
                using (var fs = new FileStream(imageFile, FileMode.Open, FileAccess.Read))
                {
                    pictureBox.Image = Image.FromStream(fs);
                }

                groupBoxImage.Text = string.Format(Resources.PageHeaderImageInfo, pictureBox.Image.Width, pictureBox.Image.Height);
            }
            else
            {
                pictureBox.Size = new Size(100, 100);
                groupBoxImage.Text = groupBoxImageDefaultText;
            }

            pictureBox.AutoScrollOffset = new Point(0, 0);
            panelImage.ScrollControlIntoView(pictureBox);

            panelImage.ResumeLayout(false);
            panelImage.PerformLayout();
        }
    }
}