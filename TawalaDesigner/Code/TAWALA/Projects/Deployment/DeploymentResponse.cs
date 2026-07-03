// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.ObjectModel;
using Tawala.Projects.Factories;
using Tawala.XmlSupport;

namespace Tawala.Projects.Deployment
{
    public class DeploymentResponse : IDeploymentItem, IDeploymentResponse
    {
        public DeploymentResponse(IXmlElement element)
        {
            Status = element.GetAttribute("status");
            Deployments = DeploymentFactory.MakeObject(element.GetChild("deployments")) as ProjectDeployments;

            if (element.HasChild("error"))
            {
                Error = DeploymentFactory.MakeObject(element.GetChild("error")) as DeploymentError;
            }
        }

        #region IDeploymentResponse Members

        public string Status { get; private set; }

        public ProjectDeployments Deployments { get; private set; }

        public DeploymentError Error { get; private set; }

        public Collection<StartPoint> GetStartPoints(string projectName)
        {
            var startPoints = new Collection<StartPoint>();

            foreach (var deployment in Deployments)
            {
                if (deployment.ProjectName == projectName)
                {
                    foreach (var startPoint in deployment.StartPoints)
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