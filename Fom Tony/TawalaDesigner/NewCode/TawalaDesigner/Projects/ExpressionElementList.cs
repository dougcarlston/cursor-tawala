// $Workfile: ExpressionElementList.cs $
// $Revision: 2 $	$Date: 11/25/05 5:00p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;

namespace Tawala.Projects
{
	/// <summary>
	/// Implments a list of ExpressionElement objects
	/// </summary>
	
	[Serializable]
	public class ExpressionElementList : Collection<ExpressionElement>
	{
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			foreach (ExpressionElement e in Items)
			{
				sb.Append(e.ToString());
			}

			return sb.ToString();
		}
	}
}
