// $Workfile: SkipToStatementView.cs $
// $Revision: 3 $	$Date: 12/12/07 4:57p $
// Copyright © 2005-2007 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Diagnostics;
using Tawala.Projects;
using Tawala.Projects.Forms;

namespace Tawala.Processes
{
	public partial class SkipToStatementView : SkipToStatementViewBase
	{
		private BindingSource destinationBinder = new BindingSource();

        public SkipToStatementView()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
			comboBoxDestination.DataSource = destinationBinder;
		}

		/// <summary>
		/// This function handles editing a new or existing statement.
		/// </summary>
		protected override void OnEdit()
		{
			if (Project.Current.CurrentComponent is IForm)
			{
				// we must always make sure the skip destinations list for the current form is displayed
				IForm form = Project.Current.CurrentComponent as IForm;
				UpdateDataSource(destinationBinder, form.SkipToDestinations);
			}

			destinationBinder.Position = 0;

			if (statement.Destination != null)
			{
				for (int i = 0; i < destinationBinder.Count; i++)
				{
					if (((SkipToDestinationItem)destinationBinder[i]).ItemId == statement.Destination.ItemId)
					{
						destinationBinder.Position = i;
					}
				}
			}
		}

		/// <summary>
		/// Update state of UI
		/// </summary>
		protected override void OnIdle()
		{
			bool bAlwaysCheck = AtInsertPosition() || ModifyMode;
			bool docAvailable = comboBoxDestination.Text.Length > 0;

			buttonAddModify.Enabled = docAvailable && bAlwaysCheck;
		}

		#region Project Events

		protected override void OnCurrentComponentSet(Tawala.Projects.Component c)
		{
			if (c is IForm)
			{
				IForm form = c as IForm;
				UpdateDataSource(destinationBinder, form.SkipToDestinations);
			}
		}

		#endregion

		private void buttonAddModify_Click(object sender, System.EventArgs e)
		{
			RememberAction();

			statement.Destination = ((SkipToDestinationItem)comboBoxDestination.SelectedItem);

			SaveStatement();
		}
	}

	// Work around for VSN Form Designer issue with Generics

    public class SkipToStatementViewBase : StatementView<SkipToStatement>
	{
	}
}
