// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Text;
using Tawala.XmlSupport;
using Tawala.Projects.Factories;

namespace Tawala.Projects.Deployment
{
	public class ProjectDeployment : IDeploymentItem
	{
		public ProjectDeployment(IXmlElement element)
		{
			this.ProjectName = element.GetAttribute("project");
			makeStartPoints(element);
		}

		private void makeStartPoints(IXmlElement element)
		{
			this.startPoints = new Collection<StartPoint>();

			foreach (IDeploymentItem item in DeploymentFactory.MakeChildren(element))
			{
				this.startPoints.Add(item as StartPoint);
			}
		}
	
		public string ProjectName
		{
			get;
			private set;
		}

		public Collection<StartPoint> StartPoints
		{
			get { return startPoints; }
		}

		private Collection<StartPoint> startPoints;
	}
}
