// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.XmlSupport;

namespace Tawala.Projects.Deployment
{
    public class DeploymentError : IDeploymentItem
    {
        public DeploymentError(IXmlElement element)
        {
            Id = element.GetAttribute("id");
            Message = element.GetAttribute("message");
        }

        public string Id { get; private set; }

        public string Message { get; private set; }
    }
}