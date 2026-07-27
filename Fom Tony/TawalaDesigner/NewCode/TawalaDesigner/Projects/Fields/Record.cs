// $Workfile: Record.cs $
// $Revision: 3 $	$Date: 8/29/06 5:34p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace Tawala.Projects
{
	[Serializable]
	public class Record : Variable
	{
		public Record(string name) : base(name)
		{
		}

		public override string QualifiedFieldName
		{
			get
			{
				return FieldName;
			}
		}
	}
}
