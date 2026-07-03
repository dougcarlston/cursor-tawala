// Copyright © 2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Windows.Forms;
using Tawala.Projects.Forms.NewFormItems;

namespace Tawala.FormDesigner.FormItemOptions
{
    public partial class HeadingOptionsView : UserControl, IHeadingOptionsView
    {
        private readonly IHeadingOptionsPresenter presenter;

        public HeadingOptionsView(IHeadingItem headingItem)
        {
            InitializeComponent();

            comboBoxHeadingType.DataSource = Enum.GetValues(typeof(HeadingType));

            presenter = new HeadingOptionsPresenter(this, headingItem);

            textBoxHeadingLabel.TextChanged += textBoxHeadingLabel_TextChanged;
        }

        #region IHeadingOptionsView Members

        public string HeadingLabel
        {
            get { return textBoxHeadingLabel.Text; }
            set { textBoxHeadingLabel.Text = value; }
        }

        public string LabelStatusText
        {
            set { labelStatus.Text = value; }
        }

        public HeadingType HeadingType
        {
            get { return (HeadingType)comboBoxHeadingType.SelectedItem; }
            set { comboBoxHeadingType.SelectedItem = value; }
        }

        #endregion

        private void textBoxHeadingLabel_TextChanged(object sender, EventArgs e)
        {
            presenter.HeadingLabelChanged(textBoxHeadingLabel.Text);
        }

        private void comboBoxHeadingType_SelectionChangeCommitted(object sender, EventArgs e)
        {
            presenter.HeadingTypeChanged();
        }
    }
}