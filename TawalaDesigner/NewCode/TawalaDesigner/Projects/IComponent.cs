// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.

using System;

namespace Tawala.Projects
{
	public interface IComponent
	{
		string Name { get; set; }
		string ToString();
		string ToXml();
		string UserVisibleComponentTypeName { get; }
	}
}
