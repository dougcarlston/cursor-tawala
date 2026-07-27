// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Tawala.Projects;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.ProjectUI;

namespace Tawala.Controls
{
    [Designer(typeof(ParentControlDesigner))]
    public partial class ConditionGroup : UserControl
    {
        protected static Color selectedColor = Color.FromArgb(210, 255, 210);
        protected static Color unSelectedColor = Color.White;
        private ComparisonOperator lastSelectedOperator;
        private bool textBoxExpressionSelected;
        private bool textBoxFieldSelected;

        public ConditionGroup()
        {
            InitializeComponent();

            textBoxField.BackColor = unSelectedColor;
        }

        public bool TextBoxFieldSelected
        {
            get { return textBoxFieldSelected; }
        }

        public bool TextBoxExpressionSelected
        {
            get { return textBoxExpressionSelected; }
        }

        public FieldTextBox TextBoxField
        {
            get { return textBoxField; }
        }

        public ComboBox ComboBoxOperator
        {
            get { return comboBoxOperator; }
        }

        public ExpressionTextBox TextBoxExpression
        {
            get { return textBoxExpression; }
        }

        public void EnablePlus()
        {
            buttonPlus.Enabled = true;
        }

        public void DisablePlus()
        {
            buttonPlus.Enabled = false;
        }

        public void SyncSelectedField(IField field)
        {
            if (textBoxFieldSelected)
            {
                if (field as ChoiceField == null)
                {
                    syncTextBoxField(field);
                }
            }

            if (textBoxExpressionSelected)
            {
                syncTextBoxExpression(new Expression(field));
            }
        }

        public bool IsEntirelyFull()
        {
            bool full = true;

            if (TextBoxField.Text.Length == 0)
            {
                full = false;
            }
            else
            {
                if (TextBoxExpression.Visible)
                {
                    if (TextBoxExpression.Text.Length == 0)
                    {
                        full = false;
                    }
                }
            }

            return full;
        }

        public bool IsEntirelyEmpty()
        {
            bool empty = false;

            if (TextBoxField.Text.Length == 0)
            {
                if (TextBoxExpression.Visible)
                {
                    if (TextBoxExpression.Text.Length == 0)
                    {
                        empty = true;
                    }
                }
                else
                {
                    empty = true;
                }
            }

            return empty;
        }

        public bool IsComplete()
        {
            return (IsEntirelyEmpty() || IsEntirelyFull());
        }

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);

            if (comboBoxOperator.Left - textBoxField.Right != 4)
            {
                textBoxField.Width = comboBoxOperator.Left - textBoxField.Left - 4;
            }

            if (comboBoxOperator.Right - textBoxExpression.Left != 4)
            {
                textBoxExpression.Left = comboBoxOperator.Right + 4;
            }

            if (buttonPlus.Left - textBoxExpression.Right != 4)
            {
                textBoxExpression.Width = buttonPlus.Left - textBoxExpression.Left - 4;
            }
        }

        private void palette_FieldNodeDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node != null && e.Node.Tag != null && e.Node.Tag is IField)
            {
                IField field = FieldUtil.RecordQualifySharedDataField(e.Node.Tag as IField);
                SyncSelectedField(field);
                goToNextTextBox();
            }
        }

        private void goToNextTextBox()
        {
            if (textBoxFieldSelected)
            {
                textBoxExpression.Focus();
            }
            else if (textBoxExpressionSelected)
            {
                // This wouldn't normally be an issue but it is when ConditionTest.TextBoxExpressionPaletteDoubleClickMC() runs
                // It doesn't give the exact same test evironment as the app.  Don't know how to rewrite the test to get around adding this.
                if (Parent != null)
                {
                    Parent.SelectNextControl(textBoxExpression, true, true, true, false);
                }
            }
        }

        private void clear()
        {
            clearTextBoxField();
            clearComboBoxOperator();
            clearTextBoxExpression();
            SelectTextBox(textBoxField);
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

        private void disableTextBoxExpression()
        {
            textBoxExpression.Enabled = false;
            SelectTextBox(null);
            textBoxExpression.Visible = false;
        }

        private void enableTextBoxExpression()
        {
            textBoxExpression.Visible = true;
            textBoxExpression.Enabled = true;
        }

        private void syncTextBoxField(IField field)
        {
            if (textBoxField.TypeChanging(field))
            {
                clearTextBoxExpression();
            }

            if (choiceOutOfRange(field))
            {
                clearTextBoxExpression();
            }

            textBoxField.Tag = field;
            textBoxField.Text = (field).QualifiedFieldName;

            SetOperatorDataSourceBasedOnFieldType();
        }

        /// <summary>
        /// Indicates whether the choice in the expression box is outside the range of choices for the specified field
        /// </summary>
        private bool choiceOutOfRange(IField newField)
        {
            if (newField != null && textBoxField.Tag != null)
            {
                if (newField is IMcqItem && textBoxField.Tag is IMcqItem)
                {
                    // MaximumChoiceIndex not currently supported
                    //if (((IMcqItem)newField).MaximumChoiceIndex < ChoiceList.IndexOfLabel(textBoxExpression.Text))
                    //{
                    //    return true;
                    //}
                }
            }

            return false;
        }

        /// <summary>
        /// Returns the field farthest down the hierarchy of the specified field.
        /// </summary>
        private IField getLeafField(IField field)
        {
            IField leafField = field;

            foreach (IField f in ((IField)textBoxField.Tag).RecursiveEnumerator)
            {
                leafField = f;
            }

            return leafField;
        }

        private void syncTextBoxExpression(Expression expression)
        {
            if (expression != null)
            {
                textBoxExpression.Tag = expression;
                textBoxExpression.Text = expression.Elements[0].Text;
            }
        }

        public void SetOperatorDataSourceBasedOnFieldType()
        {
            if (textBoxField.Tag != null)
            {
                var current = comboBoxOperator.DataSource as BindingSource;

                if (current == null || current.DataSource != ((IOperatorDataSource)textBoxField.Tag).OperatorDataSource)
                {
                    var comboBoxOperatorBindingSource = new BindingSource();
                    comboBoxOperatorBindingSource.DataSource =
                        ((IOperatorDataSource)textBoxField.Tag).OperatorDataSource;

                    comboBoxOperator.DataSource = comboBoxOperatorBindingSource;
                    comboBoxOperator.DisplayMember = "Name";
                }
            }
            else
            {
                var comboBoxOperatorBindingSource = new BindingSource();
                comboBoxOperatorBindingSource.DataSource = new ArrayList();

                comboBoxOperator.DataSource = comboBoxOperatorBindingSource;
            }
        }

        private void textBoxField_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Effect == DragDropEffects.Copy)
            {
                IField field = textBoxField.DraggedField(e.Data);
                field = FieldUtil.RecordQualifySharedDataField(field);

                syncTextBoxField(field);

                textBoxExpression.Focus();
            }
        }

        private void textBoxField_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = (textBoxField.AcceptsDropOf(e.Data) ? DragDropEffects.Copy : DragDropEffects.None);
        }

        private void textBoxField_Enter(object sender, EventArgs e)
        {
            FieldsPalette.Palette.FieldNodeDoubleClick += palette_FieldNodeDoubleClick;
            SelectTextBox(sender);
        }

        private void textBoxField_TextChanged(object sender, EventArgs e)
        {
            SetOperatorDataSourceBasedOnFieldType();
        }

        public void SelectTextBox(object o)
        {
            var textBox = o as TextBox;

            textBoxExpressionSelected = textBox == textBoxExpression;
            textBoxExpression.BackColor = textBoxExpressionSelected ? selectedColor : unSelectedColor;
            textBoxFieldSelected = textBox == textBoxField;
            textBoxField.BackColor = textBoxFieldSelected ? selectedColor : unSelectedColor;

            if (textBoxFieldSelected && !textBoxField.Focused)
            {
                textBoxField.Focus();
            }
            else if (textBoxExpressionSelected && !textBoxExpression.Focused)
            {
                textBoxExpression.Focus();
            }
        }

        private void textBoxExpression_DragDrop(object sender, DragEventArgs e)
        {
            IField field = textBoxExpression.DraggedField(e.Data);
            field = FieldUtil.RecordQualifySharedDataField(field);

            if (field != null)
            {
                syncTextBoxExpression(new Expression(field));

                goToNextTextBox();
            }
        }

        private void textBoxExpression_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = (textBoxExpression.AcceptsDropOf(e.Data) ? DragDropEffects.Copy : DragDropEffects.None);
        }

        private void textBoxExpression_Enter(object sender, EventArgs e)
        {
            FieldsPalette.Palette.FieldNodeDoubleClick += palette_FieldNodeDoubleClick;
            SelectTextBox(sender);
        }

        private void buttonPlus_Click(object sender, EventArgs e)
        {
            if (buttonPlus.Enabled)
            {
                var cg = new ConditionGroup();
                Parent.Controls.Add(cg);
                cg.SelectTextBox(cg.TextBoxField);
            }
        }

        private void buttonMinus_Click(object sender, EventArgs e)
        {
            if (buttonMinus.Enabled)
            {
                if (Parent.Controls.Count <= 1)
                {
                    clear();
                }
                else
                {
                    Parent.Controls.Remove(this);
                }
            }
        }

        private void comboBoxOperator_SelectedIndexChanged(object sender, EventArgs e)
        {
            lastSelectedOperator = comboBoxOperator.SelectedItem as ComparisonOperator;

            textBoxExpression.Visible =
                lastSelectedOperator == null || lastSelectedOperator.NumOperands > 1;
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
    }
}
