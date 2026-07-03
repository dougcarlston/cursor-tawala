// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.ObjectModel;
using Tawala.Projects.Factories;
using Tawala.XmlSupport;

namespace Tawala.Projects.Deployment
{
    public class ProjectDeployment : IDeploymentItem
    {
        private Collection<StartPoint> startPoints;

        public ProjectDeployment(IXmlElement element)
        {
            ProjectName = element.GetAttribute("project");
            makeStartPoints(element);
        }

        public string ProjectName { get; private set; }

        public Collection<StartPoint> StartPoints
        {
            get { return startPoints; }
        }

        private void makeStartPoints(IXmlElement element)
        {
            startPoints = new Collection<StartPoint>();

            foreach (var item in DeploymentFactory.MakeChildren(element))
            {
                startPoints.Add(item as StartPoint);
            }
        }
    }
}