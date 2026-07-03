// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace Tawala.Projects.Forms
{
	[Serializable]
	internal class ExternalForm : Form, IExternalForm
	{
		internal ExternalForm(string name)
			: base(name)
		{
		}
	}
}
