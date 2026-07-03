// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.ObjectModel;

namespace Tawala.Projects.Deployment
{
    public interface IDeploymentResponse
    {
        string Status { get; }
        ProjectDeployments Deployments { get; }
        DeploymentError Error { get; }
        Collection<StartPoint> GetStartPoints(string projectName);
    }
}