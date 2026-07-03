// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.ObjectModel;
using Tawala.Projects.Factories;
using Tawala.XmlSupport;

namespace Tawala.Projects.Deployment
{
    public class ProjectDeployments : Collection<ProjectDeployment>, IDeploymentItem
    {
        public ProjectDeployments(IXmlElement element)
        {
            UserName = element.GetAttribute("user");
            makeDeployments(element);
        }

        public string UserName { get; private set; }

        private void makeDeployments(IXmlElement element)
        {
            foreach (var item in DeploymentFactory.MakeChildren(element))
            {
                Add(item as ProjectDeployment);
            }
        }
    }
}