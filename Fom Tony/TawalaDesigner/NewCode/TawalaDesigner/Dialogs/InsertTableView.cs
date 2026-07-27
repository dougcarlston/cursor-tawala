// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.Windows.Forms;

namespace Tawala.Dialogs
{
    public partial class InsertTableView : Form, IInsertTableView
    {
        public InsertTableView()
        {
            InitializeComponent();
        }

        #region IInsertTableView Members

        public int TableWidthInPoints
        {
            get { return Convert.ToInt32(Decimal.ToDouble(numericUpDownTableWidth.Value)*72.0); }
        }

        public int Columns
        {
            get { return Convert.ToInt32(numericUpDownColumns.Value); }
        }

        public int Rows
        {
            get { return Convert.ToInt32(numericUpDownRows.Value); }
        }

        #endregion
    }
}