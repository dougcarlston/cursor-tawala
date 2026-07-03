// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Text;

namespace Tawala.Projects.Deployment
{
	public interface IDeploymentResponse
	{
		string Status { get; }
		ProjectDeployments Deployments { get; }
		Collection<StartPoint> GetStartPoints(string projectName);
		DeploymentError Error { get; }
	}
}
