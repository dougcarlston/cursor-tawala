// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace Tawala.Projects.Images
{
	public interface IImageData
	{
		string CreateImageFile();
		string ToXml();
	}
}
