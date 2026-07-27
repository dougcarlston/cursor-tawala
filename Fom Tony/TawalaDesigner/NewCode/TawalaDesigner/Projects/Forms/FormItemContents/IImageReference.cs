// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using Tawala.Projects.Forms.NewFormItems;

namespace Tawala.Projects.Forms.FormItemContents
{
	public interface IImageReference
	{
		string Url { get; }
		string PathName { get; }
		string Id { get; set; }
		string ImageFormat { get; }
	}
}
