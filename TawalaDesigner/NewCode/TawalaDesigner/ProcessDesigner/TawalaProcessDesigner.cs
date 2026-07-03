// Copyright © 2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using Tawala.Interfaces;
using Tawala.MainApplication;
using Tawala.ComponentDesigner;
using Tawala.Processes;
using Tawala.Projects;

namespace Tawala.ProcessDesigner
{
	public class TawalaProcessDesigner : ProjectComponentDesigner, ITawalaProcessDesigner
	{
		private IProcessView currentProcessView;

		private static readonly Type[] statementViewTypes =
		{
			null,
			typeof(IfStatementView), 
			null,
			typeof(ShowStatementView),
			typeof(SendStatementView),
			null,
			typeof(AppendStatementView),
			null,
			typeof(GetStatementView),
			typeof(ForEachStatementView),
			typeof(DeleteStatementView),
			null,
			typeof(SetStatementView),
			null,
			typeof(CommentStatementView),
			null
		};

		public TawalaProcessDesigner()
		{
			this.ItemsPalette = new ProcessStatementsPalette(statementViewTypes);
		}

		#region ITawalaProcessDesigner Members

		public void SetCurrentProcessView(IProcessView processView)
		{
			currentProcessView = processView;

			(ItemsPalette as IStatementSelector).ProcessEditor = currentProcessView.ProcessEditor;
			currentProcessView.ProcessEditor.Init(ItemsPalette as IStatementSelector, statementViewTypes);
			currentProcessView.ProcessEditor.Process = currentProcessView.Process as Process;
			currentProcessView.ProcessEditor.ConnectProjectEvents(true);

			activateProcessView();
		}

		#endregion

		private void activateProcessView()
		{
			currentProcessView.Show();
			currentProcessView.Activate();

			setDesignerPalette();
		}
	}
}
