// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.Windows.Forms;
using Tawala.Functions.Runtime;
using Tawala.Functions.ViewPresenter;

namespace Tawala.Functions.Controls
{
    public partial class InsertFunctionDialog : Form, IInsertFunctionView
    {
        private readonly EventHandler<FunctionConfiguredEventArgs> notifyOwner;
        private IInsertFunctionPresenter presenter;

        public InsertFunctionDialog(EventHandler<FunctionConfiguredEventArgs> notifyOwner)
        {
            this.notifyOwner = notifyOwner;
            InitializeComponent();
            presenter = new InsertFunctionPresenter(this);
        }

        #region IInsertFunctionView Members

        public void Setup(ICategoryInfoCollection categories)
        {
            comboBoxCategory.DataSource = categories;
        }

        #endregion

        private void comboBoxCategory_SelectedValueChanged(object sender, EventArgs e)
        {
            listBoxFunctions.DataSource = (comboBoxCategory.SelectedValue as ICategoryInfo).Functions;
        }

        private void listBoxFunctions_SelectedValueChanged(object sender, EventArgs e)
        {
            var info = listBoxFunctions.SelectedValue as IFunctionInfo;
            labelSelectedFunctionName.Text = info != null ? info.Name : string.Empty;
            labelSelectedFunctionDescription.Text = info != null ? info.Description : string.Empty;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            configureSelectedFunction();
        }

        private void listBoxFunctions_DoubleClick(object sender, EventArgs e)
        {
            configureSelectedFunction();
        }

        private void configureSelectedFunction()
        {
            Close();
            var info = listBoxFunctions.SelectedValue as IFunctionInfo;
            ConfigureFunctionDialog.Presenter.CreateFunction(info, notifyOwner);
        }
    }
}