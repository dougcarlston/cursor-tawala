// Copyright © 2005 - 2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using Tawala.Interfaces;

namespace Tawala.ProcessDesigner
{
	public interface ITawalaProcessDesigner : IProjectComponentDesigner
	{
		void SetCurrentProcessView(IProcessView processView);
	}
}
