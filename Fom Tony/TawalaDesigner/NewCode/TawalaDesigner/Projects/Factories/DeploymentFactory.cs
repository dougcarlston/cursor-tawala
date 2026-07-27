// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Diagnostics;
using Tawala.Projects;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.XmlSupport;
using Tawala.Projects.Deployment;

namespace Tawala.Projects.Factories
{
	/// <summary>
	/// Produces Tawala project deployment objects from XML elements.
	/// </summary>
	public static class DeploymentFactory
	{
		private static Factory<IDeploymentItem> deploymentFactory = new Factory<IDeploymentItem>();

		static DeploymentFactory()
		{
			deploymentFactory.Register("response", typeof(DeploymentResponse));
			deploymentFactory.Register("deployments", typeof(ProjectDeployments));
			deploymentFactory.Register("deployment", typeof(ProjectDeployment));
			deploymentFactory.Register("startpoint", typeof(StartPoint));
			deploymentFactory.Register("error", typeof(DeploymentError));
		}

		public static IDeploymentItem MakeObject(IXmlElement element)
		{
			IDeploymentItem deploymentItem = deploymentFactory.MakeObject(element);

			return deploymentItem;
		}

		public static Collection<IDeploymentItem> MakeChildren(IXmlElement element)
		{
			Collection<IDeploymentItem> children = new Collection<IDeploymentItem>();

			foreach (IXmlElement childElement in element.GetChildren())
			{
				children.Add(MakeObject(childElement));
			}

			return children;
		}
	}
}
