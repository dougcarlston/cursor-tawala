// Copyright © 2005-2007 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.Generic;
using System.Text;
using Tawala.Projects.Processes;

namespace Tawala.Processes
{
    public interface IStatementEditor
    {
        void SwitchToAddMode();

        void Edit(ProcessStatement ps);

        void DoIdle();

        void MDIWindowActivated();

        void MDIWindowDeactivated();

        // currently duplicates StatementType property but StatementType might change
        Type BaseStatementType
        {
            get;
        }

        Type StatementType
        {
            get;
        }

    	void ParentWindowClosed();
    }
}
