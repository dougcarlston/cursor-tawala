// $Workfile: InvitationFieldEventArgs.cs $
// $Revision: 4 $	$Date: 4/27/07 1:45p $
// Copyright © 2005-2006 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace Tawala.TextEditor
{
    public class InvitationFieldEventArgs : EventArgs
    {
        public InvitationFieldEventArgs(int id)
        {
            this.id = id;
        }

        private int id;
        public int Id
        {
            get { return id; }
        }
    }
}
