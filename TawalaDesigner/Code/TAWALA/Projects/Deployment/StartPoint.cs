// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.XmlSupport;

namespace Tawala.Projects.Deployment
{
    public class StartPoint : IDeploymentItem
    {
        public StartPoint(IXmlElement element)
        {
            FormName = element.GetAttribute("form");
            Url = element.GetAttribute("url");
        }

        public string FormName { get; private set; }

        public string Url { get; private set; }
    }
}