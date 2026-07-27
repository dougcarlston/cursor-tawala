// $Workfile: IRecursiveEnumerable.cs $
// $Revision: 1 $	$Date: 12/03/05 2:46p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections;
using System.Text;

namespace Tawala.Projects
{
	/// <summary>
	/// Interface for recursive enumeration.
	/// </summary>
	public interface IRecursiveEnumerable
	{
		IEnumerable RecursiveEnumerator
		{
			get;
		}
	}
}
