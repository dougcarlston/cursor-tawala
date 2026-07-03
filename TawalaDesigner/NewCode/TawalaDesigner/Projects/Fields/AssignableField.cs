// $Workfile: AssignableField.cs $
// $Revision: 1 $	$Date: 6/07/07 11:37p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.
using System;
using System.Text;
using System.Collections;
using System.Text.RegularExpressions;

namespace Tawala.Projects
{
	/// <summary>
	/// 
	/// </summary>

	[Serializable]
	public class AssignableField : Field, IAssignableField
	{
		public AssignableField() : base()
		{
		}

		public AssignableField(string name) : base(name)
		{
		}

		public AssignableField(int id) : base(id)
		{
		}

		#region IAssignableField Members

		public string AssignmentName
		{
			get { return QualifiedFieldName; }
		}

		#endregion
	}
}
