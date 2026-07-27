// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace Tawala.Projects.Images
{
	public interface IImageDefinitionCollection
	{
		string Add(string url);
		void Remove(string id);
		string ToXml();
		IImageDefinition this[int index] { get; }
		IImageDefinition this[string id] { get; }
		IImageDefinition GetImageDefinitionByPathName(string pathName);
	}
}
