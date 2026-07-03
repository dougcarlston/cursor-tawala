// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Tawala.Projects;
using Tawala.ProjectUI;

namespace Tawala.Processes
{
	/// <summary>
	/// SetDetails for SetStatement
	/// </summary>
    public sealed partial class SetStatementView : SetStatementViewBase
	{
		// control size ratio for layout as tabpage changes width

        private double ratioTextBoxVariable;
        private int lastTabPageSetWidth;
        private bool textBoxExpressionSelected;
        private bool textBoxVariableSelected;

		private const int detailsPanelHeight = 170;

    	public SetStatementView()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			ratioTextBoxVariable = textBoxVariable.Width / (double)tabPageSet.Width;

			tabPageSet.Layout += new LayoutEventHandler(tabPageSet_Layout);

			tabPageSet.PerformLayout();

			FieldsPalette.Palette.FieldNodeDoubleClick += new TreeNodeMouseClickEventHandler(palette_FieldNodeDoubleClick);
		}

		private void palette_FieldNodeDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			if (textBoxVariableSelected)
			{
				IAssignableField assignable = e.Node != null ? e.Node.Tag as IAssignableField : null;

				if (assignable != null)
				{
					textBoxVariable.Text = assignable.AssignmentName;
					textBoxExpression.Focus();
				}
			}

			else if (textBoxExpressionSelected)
			{
				textBoxExpression.Insert((IPaletteField)e.Node.Tag);
				textBoxExpression.Focus();
			}
		}

		/// <summary>
		/// This function handles editing a new or existing statement.
		/// </summary>
		protected override void OnEdit()
		{
			setDetailsPanelHeight(detailsPanelHeight);

			// update the controls from the statement
			if (statement.Variable.FieldName != null)
			{
				if (statement.Variable is IAssignableField)
				{
					textBoxVariable.Text = ((IAssignableField)statement.Variable).AssignmentName;
				}
				else
				{
					textBoxVariable.Text = statement.Variable.FieldName;
				}
			}
			if (statement.Expression.ToString() != null)
			{
				textBoxExpression.Text = statement.Expression.ToString();
			}

			checkBoxArithmeticAsText.Checked = statement.TreatArithmeticAsText;
		}

		/// <summary>
		/// On application idle update state of UI
		/// </summary>
		protected override void OnIdle()
		{
			bool bAlwaysCheck = AtInsertPosition() || ModifyMode;
			
			bool enabled =
				bAlwaysCheck &&
				textBoxVariable.Text.Length > 0 &&
				textBoxExpression.Text.Length > 0;

			if (enabled && ParentView != null && ParentView.Process != null)
			{
				IAssignableField assignable = null;

				if (Project.FieldMapByName.ContainsKey(textBoxVariable.Text))
				{
					assignable = Project.FieldMapByName[textBoxVariable.Text] as IAssignableField;
				}
				if (assignable == null)
				{
					if (!FieldUtil.IsRegularOrExternalRecordField(textBoxVariable.Text))
					{
						enabled = ParentView.Process.ValidVariableName(textBoxVariable.Text, ParentView.InsertionPoint);
					}
				}
				else
				{
					enabled = true;
				}
			}

			buttonAddModify.Enabled = enabled;

			checkBoxArithmeticAsText.Enabled = labelArithmeticAsText.Enabled = Regex.IsMatch(textBoxExpression.Text, @"\+|\-|\*|/");
		}

		protected override void NewStatement()
		{
			statement = new SetStatement(ParentView.Process);
		}

		#region Control Events

		/// <summary>
		/// Relayout the controls based on space available
		/// </summary>
		private void tabPageSet_Layout(object sender, LayoutEventArgs e)
		{
			if (ratioTextBoxVariable != 0.0 && lastTabPageSetWidth != tabPageSet.Width && tabPageSet.Width >= 240)
			{
				int width = tabPageSet.Width;
				int oldRight = textBoxVariable.Right;
				textBoxVariable.Width = (int)(width * ratioTextBoxVariable); ;
				int offset = textBoxVariable.Right - oldRight;
				labelTo.Left += offset;

				textBoxExpression.Left += offset;
				textBoxExpression.Width = width - 8 - textBoxExpression.Left;

				checkBoxArithmeticAsText.Left += offset;
				labelArithmeticAsText.Left += offset;
			}
			lastTabPageSetWidth = tabPageSet.Width;
		}

		/// <summary>
		/// User clicked the Add / Modify button
		/// </summary>
		private void buttonAddModify_Click(object sender, System.EventArgs e)
		{
			RememberAction();

			textBoxVariable.Text = textBoxVariable.Text.Trim();

			IAssignableField assignable = Project.FieldMapByName[textBoxVariable.Text] as IAssignableField;

			if (isVariable(textBoxVariable.Text))
			{
				statement.Variable = new Variable(textBoxVariable.Text);
			}
			else if (assignable != null)
			{
				statement.Variable = assignable;
			}
			else
			{
				statement.Variable = new AssignableField(textBoxVariable.Text);
			}

			statement.Expression = new Expression(textBoxExpression.Text, ParentView.Process.GetValidFields(ParentView.InsertionPoint));

			statement.TreatArithmeticAsText = checkBoxArithmeticAsText.Checked;

			SaveStatement();
		}

		private static bool isVariable(string fieldString)
		{
			return FieldUtil.IsVariable(fieldString);
		}

		#endregion

		private void textBoxExpression_DragDrop(object sender, DragEventArgs e)
		{
			IField field = textBoxExpression.DraggedField(e.Data);

			textBoxExpression.Insert(field);
			textBoxExpression.Focus();
		}

		private void textBoxExpression_DragEnter(object sender, DragEventArgs e)
		{
			e.Effect = (textBoxExpression.AcceptsDropOf(e.Data) ? DragDropEffects.Copy : DragDropEffects.None);
		}

		private void textBoxExpression_Enter(object sender, EventArgs e)
		{
			selectTextBoxExpression();
		}

		private void selectTextBoxVariable()
		{
			textBoxExpressionSelected = false;
			textBoxExpression.BackColor = UnselectedColor;
			textBoxVariableSelected = true;
			textBoxVariable.BackColor = SelectedColor;
		}

		private void selectTextBoxExpression()
		{
			textBoxVariableSelected = false;
			textBoxVariable.BackColor = UnselectedColor;
			textBoxExpressionSelected = true;
			textBoxExpression.BackColor = SelectedColor;
		}

		private void textBoxVariable_DragDrop(object sender, DragEventArgs e)
		{
			IAssignableField assignable = textBoxVariable.DraggedField(e.Data);

			if (assignable != null)
			{
				textBoxVariable.Text = assignable.AssignmentName;
			}

			textBoxExpression.Focus();
		}

		private void textBoxVariable_DragEnter(object sender, DragEventArgs e)
		{
			e.Effect = (textBoxVariable.AcceptsDropOf(e.Data) ? DragDropEffects.Copy : DragDropEffects.None);
		}

		private void textBoxVariable_Enter(object sender, EventArgs e)
		{
			selectTextBoxVariable();
		}

		protected override void OnMdiWindowActivated()
		{
			base.OnMdiWindowActivated();

			if (textBoxVariableSelected)
			{
				textBoxExpression.BackColor = UnselectedColor;
				textBoxVariable.BackColor = SelectedColor;
			}
			else if (textBoxExpressionSelected)
			{
				textBoxVariable.BackColor = UnselectedColor;
				textBoxExpression.BackColor = SelectedColor;
			}
		}

		protected override void OnMdiWindowDeactivated()
		{
			base.OnMdiWindowDeactivated();

			textBoxVariable.BackColor = UnselectedColor;
			textBoxExpression.BackColor = UnselectedColor;
		}

		protected override void OnEnter(EventArgs e)
		{
			FieldsPalette.Palette.FieldNodeDoubleClick += new TreeNodeMouseClickEventHandler(palette_FieldNodeDoubleClick);
			base.OnEnter(e);
		}
	}

	// Work around for VSN Form Designer issue with Generics

    public class SetStatementViewBase : StatementView<SetStatement>
	{
	}
}
