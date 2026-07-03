// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Text;
using Tawala.XmlSupport;
using Tawala.Projects.Factories;

namespace Tawala.Projects.Deployment
{
	public class DeploymentResponse : IDeploymentItem, IDeploymentResponse
	{
		public DeploymentResponse(IXmlElement element)
		{
			this.Status = element.GetAttribute("status");
			this.Deployments = DeploymentFactory.MakeObject(element.GetChild("deployments")) as ProjectDeployments;

			if (element.HasChild("error"))
			{
				this.Error = DeploymentFactory.MakeObject(element.GetChild("error")) as DeploymentError;
			}
		}

		#region IDeploymentResponse Members

		public string Status
		{
			get;
			private set;
		}

		public ProjectDeployments Deployments
		{
			get;
			private set;
		}

		public DeploymentError Error
		{
			get;
			private set;
		}

		public Collection<StartPoint> GetStartPoints(string projectName)
		{
			Collection<StartPoint> startPoints = new Collection<StartPoint>();

			foreach (ProjectDeployment deployment in Deployments)
			{
				if (deployment.ProjectName == projectName)
				{
					foreach (StartPoint startPoint in deployment.StartPoints)
					{
						startPoints.Add(startPoint);
					}
				}
			}

			return startPoints;
		}

		#endregion
	}
}
