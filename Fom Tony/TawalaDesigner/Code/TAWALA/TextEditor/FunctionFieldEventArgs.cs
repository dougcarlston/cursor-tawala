// $Workfile: FunctionFieldEventArgs.cs $
// $Revision: 1 $	$Date: 2/15/07 4:29p $
// Copyright © 2005-2006 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace Tawala.TextEditor
{
    public class FunctionFieldEventArgs : EventArgs
    {
		public FunctionFieldEventArgs(int instanceId)
		{
			this.instanceId = instanceId;
        }

        private int instanceId;
        public int InstanceId
        {
            get { return instanceId; }
        }
    }
}
