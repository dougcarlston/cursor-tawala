// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Tawala.Projects;
using Tawala.Projects.Images;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.XmlSupport;

namespace Tawala.Projects.Factories
{
	public static class ProjectComponentFactory
	{
		private static Factory<IProjectComponent> projectComponentFactory = new Factory<IProjectComponent>();

		static ProjectComponentFactory()
		{
			projectComponentFactory.Register("images", typeof(ImageDefinitionCollection));
			projectComponentFactory.Register("imagedef", typeof(ImageDefinition));
		}

		public static IProjectComponent MakeObject(IXmlElement element)
		{
			return projectComponentFactory.MakeObject(element);
		}

		public static ProjectComponentCollection MakeChildren(IXmlElement element)
		{
			ProjectComponentCollection children = new ProjectComponentCollection();

			foreach (IXmlElement childElement in element.GetChildren())
			{
				children.Add(ProjectComponentFactory.MakeObject(childElement));
			}

			return children;
		}
	}
}
