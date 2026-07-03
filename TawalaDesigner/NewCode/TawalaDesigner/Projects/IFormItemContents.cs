// $Workfile: IFormItemContents.cs $
// $Revision: 1 $	$Date: 1/25/08 11:54p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

using Tawala.XmlSupport;

namespace Tawala.Proj
{
	public interface IFormItemContents
	{
		void SetContents(IXmlElement element);
		IXmlElement GetContents();
	}
}
