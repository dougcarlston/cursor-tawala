// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.Projects.Components;
using Tawala.Projects.Images;
using Tawala.XmlSupport;

namespace Tawala.Projects.Factories
{
    public static class ProjectComponentFactory
    {
        private static readonly Factory<IProjectComponentXml> projectComponentFactory = new Factory<IProjectComponentXml>();

        static ProjectComponentFactory()
        {
            projectComponentFactory.Register("images", typeof(ImageDefinitionCollection));
            projectComponentFactory.Register("imagedef", typeof(ImageDefinition));
        }

        public static IProjectComponentXml MakeObject(IXmlElement element)
        {
            return projectComponentFactory.MakeObject(element);
        }

        public static ProjectComponentCollection MakeChildren(IXmlElement element)
        {
            var children = new ProjectComponentCollection();

            foreach (IXmlElement childElement in element.GetChildren())
            {
                children.Add(MakeObject(childElement));
            }

            return children;
        }
    }
}