// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using Tawala.XmlSupport;

namespace Tawala.Projects.Deployment
{
	public class DeploymentError : IDeploymentItem
	{
		public DeploymentError(IXmlElement element)
		{
			this.Id = element.GetAttribute("id");
			this.Message = element.GetAttribute("message");
		}

		public string Id
		{
			get;
			private set;
		}

		public string Message
		{
			get;
			private set;
		}
	}
}
