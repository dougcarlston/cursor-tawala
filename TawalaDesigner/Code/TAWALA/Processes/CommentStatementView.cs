// $Workfile: CommentStatementView.cs $
// $Revision: 4 $	$Date: 12/12/07 4:57p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Diagnostics;
using Tawala.Projects.Processes;
using Tawala.ProjectUI;

namespace Tawala.Processes
{
    public partial class CommentStatementView : ZCommentStatementView
	{
		// control size ratio for layout as tabpage changes width

		double ratioTextBoxComment = 0.0;
		int lastTabPageSetWidth = 0;

		private const int detailsPanelHeight = 135;

        public CommentStatementView()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			ratioTextBoxComment = textBoxComment.Width / (double)tabPageSet.Width;

			tabPageSet.Layout += new LayoutEventHandler(tabPageSet_Layout);

			tabPageSet.PerformLayout();
		}

		protected override void OnEdit()
		{
			setDetailsPanelHeight(detailsPanelHeight);

			textBoxComment.Text = statement.Text;
			textBoxComment.Focus();
		}

		/// <summary>
		/// On application idle update state of UI
		/// </summary>
		protected override void OnIdle()
		{
			buttonAddModify.Enabled = (textBoxComment.Text.Length > 0);
		}

		#region Control Events

		/// <summary>
		/// Relayout the controls based on space available
		/// </summary>
		private void tabPageSet_Layout(object sender, LayoutEventArgs e)
		{
			if (ratioTextBoxComment != 0.0 && lastTabPageSetWidth != tabPageSet.Width && tabPageSet.Width >= 240)
			{
				textBoxComment.Width = (int)(tabPageSet.Width * ratioTextBoxComment); ;
			}
			lastTabPageSetWidth = tabPageSet.Width;
		}

		/// <summary>
		/// User clicked the Add / Modify button
		/// </summary>
		private void buttonAddModify_Click(object sender, System.EventArgs e)
		{
			RememberAction();

			statement.Text = textBoxComment.Text.Trim();

			textBoxComment.Text = statement.Text;

			SaveStatement();
		}

		#endregion
	}

    // Work around for VSN Form Designer issue with Generics
    public class ZCommentStatementView : StatementView<CommentStatement>
    {
    }
}
