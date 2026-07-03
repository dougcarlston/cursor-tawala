// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Forms;
using Tawala.Projects;
using Tawala.Projects.Expressions;
using Tawala.Projects.Forms;

namespace Tawala.Functions.Controls
{
    public partial class ConditionControl : UserControl
    {
        private const int buttonMarginLeft = 6;
        private const int comboBoxOperatorMargin = 6;
        private const int height = 25;
        private const int margin = 2;
        private const int minWidth = 451;
        private static readonly Size defaultMinimumSize = new Size(minWidth, height);
        private readonly int fixedWidth;
        private ComparisonOperator lastSelectedOperator;

        public ConditionControl()
        {
            InitializeComponent();
            textBoxField.Left = margin;
            textBoxField.ContentsChanged += textBoxField_ContentsChanged;

            fixedWidth = margin*2;
            fixedWidth += comboBoxOperatorMargin*2 + comboBoxOperator.Width;
            fixedWidth += buttonAdd.Width + buttonMarginLeft;
            fixedWidth += buttonDelete.Width + buttonMarginLeft;

            base.MinimumSize = defaultMinimumSize;
            base.Dock = DockStyle.None;
            base.Anchor = AnchorStyles.Left;
            TabStop = true;
        }

        public override Size MinimumSize
        {
            get
            {
                return defaultMinimumSize;
            }
            set
            {
                base.MinimumSize = defaultMinimumSize;
            }
        }

        internal ConditionLeftFieldControl TextBoxField
        {
            get
            {
                return textBoxField;
            }
        }

        internal ConditionOperatorControl ComboBoxOperator
        {
            get
            {
                return comboBoxOperator;
            }
        }

        internal ConditionRightExpressionControl TextBoxExpression
        {
            get
            {
                return textBoxExpression;
            }
        }

        public event EventHandler<EventArgs> AddClicked;
        public event EventHandler<EventArgs> DeleteClicked;

        public void EnablePlus()
        {
            buttonAdd.Enabled = true;
        }

        public void DisablePlus()
        {
            buttonAdd.Enabled = false;
        }

        public bool IsEntirelyFull()
        {
            if (textBoxField.Text.Length == 0)
            {
                return false;
            }

            if (comboBoxOperator.Text.Length == 0)
            {
                return false;
            }

            if (textBoxExpression.Enabled && textBoxExpression.Text.Length == 0)
            {
                return false;
            }

            return true;
        }

        public bool IsEntirelyEmpty()
        {
            if (textBoxField.Text.Length > 0)
            {
                return false;
            }

            if (comboBoxOperator.Text.Length > 0)
            {
                return false;
            }

            if (textBoxExpression.Enabled && textBoxExpression.Text.Length > 0)
            {
                return false;
            }

            return true;
        }

        public bool IsComplete()
        {
            return IsEntirelyEmpty() || IsEntirelyFull();
        }

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);

            int width = (Width - fixedWidth)/2;

            textBoxField.Width = width;
            comboBoxOperator.Left = textBoxField.Right + comboBoxOperatorMargin;
            textBoxExpression.Left = comboBoxOperator.Right + comboBoxOperatorMargin;
            textBoxExpression.Width = width;
            buttonAdd.Left = textBoxExpression.Right + buttonMarginLeft;
            buttonDelete.Left = buttonAdd.Right + buttonMarginLeft;
        }

        //private void palette_FieldNodeDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        //{
        //    if (e.Node != null && e.Node.Tag != null && e.Node.Tag is IField)
        //    {
        //        IField field = FieldUtil.RecordQualifySharedDataField(e.Node.Tag as IField);
        //        SyncSelectedField(field);
        //        goToNextTextBox();
        //    }
        //}

        //private void goToNextTextBox()
        //{
        //    if (TextBoxFieldSelected)
        //    {
        //        textBoxExpression.Focus();
        //    }
        //    else if (TextBoxExpressionSelected)
        //    {
        //        // This wouldn't normally be an issue but it is when ConditionTest.TextBoxExpressionPaletteDoubleClickMC() runs
        //        // It doesn't give the exact same test evironment as the app.  Don't know how to rewrite the test to get around adding this.
        //        if (Parent != null)
        //        {
        //            Parent.SelectNextControl(textBoxExpression, true, true, true, false);
        //        }
        //    }
        //}

        public void Clear()
        {
            clearTextBoxField();
            clearComboBoxOperator();
            clearTextBoxExpression();
            textBoxField.Focus();
        }

        private void clearTextBoxField()
        {
            textBoxField.ClearTextAndTag();
        }

        private void clearComboBoxOperator()
        {
            SetOperatorDataSourceBasedOnFieldType();
        }

        private void clearTextBoxExpression()
        {
            textBoxExpression.ClearTextAndTag();
        }

        /// <summary>
        /// Indicates whether the choice in the expression box is outside the range of choices for the specified field
        /// </summary>
        private bool choiceOutOfRange(IField newField)
        {
            if (newField != null && textBoxField.Tag != null)
            {
                if (newField.GetType() == typeof(McqItem) && textBoxField.Tag.GetType() == typeof(McqItem))
                {
                    if (((McqItem)newField).MaximumChoiceIndex < ChoiceList.IndexOfLabel(textBoxExpression.Text))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void SetOperatorDataSourceBasedOnFieldType()
        {
            if (textBoxField.Tag != null)
            {
                var current = comboBoxOperator.DataSource as BindingSource;

                if (current == null || current.DataSource != ((IOperatorDataSource)textBoxField.Tag).OperatorDataSource)
                {
                    var comboBoxOperatorBindingSource = new BindingSource();
                    comboBoxOperatorBindingSource.DataSource = ((IOperatorDataSource)textBoxField.Tag).OperatorDataSource;

                    comboBoxOperator.DataSource = comboBoxOperatorBindingSource;
                }
            }
            else
            {
                comboBoxOperator.DataSource = new Collection<EmptyOperatorSource>();
            }
            comboBoxOperator.DisplayMember = "Name";
            int dropDownHeight = (comboBoxOperator.Items.Count == 0 ? 1 : comboBoxOperator.Items.Count)*comboBoxOperator.ItemHeight;
            comboBoxOperator.DropDownHeight = dropDownHeight;
        }

        private void textBoxField_ContentsChanged(object sender, EventArgs e)
        {
            var field = textBoxField.Tag as IField;

            if (choiceOutOfRange(field))
            {
                clearTextBoxExpression();
            }

            SetOperatorDataSourceBasedOnFieldType();
        }

        private void buttonPlus_Click(object sender, EventArgs e)
        {
            if (AddClicked != null)
            {
                AddClicked(this, EventArgs.Empty);
            }
        }

        private void buttonMinus_Click(object sender, EventArgs e)
        {
            if (DeleteClicked != null)
            {
                DeleteClicked(this, EventArgs.Empty);
            }
        }

        private void comboBoxOperator_SelectedIndexChanged(object sender, EventArgs e)
        {
            lastSelectedOperator = comboBoxOperator.SelectedItem as ComparisonOperator;

            textBoxExpression.Enabled = lastSelectedOperator == null || lastSelectedOperator.NumOperands > 1;
        }

        private void textBoxExpression_KeyDown(object sender, KeyEventArgs e)
        {
            if (!CuttingCopyingOrPasting(e))
            {
                textBoxExpression.Tag = null;
            }
        }

        private static bool CuttingCopyingOrPasting(KeyEventArgs e)
        {
            return e.Control.Equals(true);
        }

        private void textBoxField_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Delete || e.KeyData == Keys.Back)
            {
                clearTextBoxField();
            }
        }

        #region Nested type: EmptyOperatorSource

        private class EmptyOperatorSource
        {
            public string Name { get; set; }
        }

        #endregion
    }
}