// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Text;
using Tawala.XmlSupport;
using Tawala.Projects.Factories;

namespace Tawala.Projects.Deployment
{
	public class ProjectDeployments : Collection<ProjectDeployment>, IDeploymentItem
	{
		public ProjectDeployments(IXmlElement element)
		{
			this.UserName = element.GetAttribute("user");
			makeDeployments(element);
		}

		private void makeDeployments(IXmlElement element)
		{
			foreach (IDeploymentItem item in DeploymentFactory.MakeChildren(element))
			{
				this.Add(item as ProjectDeployment);
			}
		}

		public string UserName
		{
			get;
			private set;
		}
	}
}
