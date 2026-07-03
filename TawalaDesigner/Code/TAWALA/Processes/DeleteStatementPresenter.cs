// $Workfile: DeleteStatementPresenter.cs $
// $Revision: 4 $	$Date: 6/04/07 3:40p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.   
#define CODE_ANALYSIS
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using Tawala.Projects;

namespace Tawala.Processes
{
    public class DeleteStatementPresenter
    {
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification="KM: Don't want to eliminate presenter, we should use it some day")]
		private IStatementView view;

        public DeleteStatementPresenter(IStatementView view)
        {
            this.view = view;
        }
	}
}