// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using Tawala.XmlSupport;

namespace Tawala.Projects.Deployment
{
	public class StartPoint : IDeploymentItem
	{
		public StartPoint(IXmlElement element)
		{
			this.FormName = element.GetAttribute("form");
			this.Url = element.GetAttribute("url");
		}
	
		public string FormName
		{
			get;
			private set;
		}

		public string Url
		{
			get;
			private set;
		}

	}
}
