// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.

using System;
using Tawala.Projects.Processes;

namespace Tawala.Processes
{
	public interface IProcessEditor
	{
		System.Collections.ObjectModel.Collection<System.Windows.Forms.ToolStripItem> Init(IStatementSelector statementSelector, Type[] statementViewTypes);
		void StatementButtonClicked(System.ComponentModel.Component component);
		Process Process { get; set; }
	}
}
