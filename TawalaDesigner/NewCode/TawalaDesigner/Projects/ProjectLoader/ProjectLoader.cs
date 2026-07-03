// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Tawala.XmlSupport;

namespace Tawala.Projects.ProjectLoader
{
	public class ProjectLoader
	{
		private static string projectXmlString;

		public static void Load(string filePath)
		{
			projectXmlString = new StreamReader(open(filePath)).ReadToEnd();
		}

		public static string ProjectXmlString
		{
			get { return ProjectLoader.projectXmlString; }
		}

		public static IXmlElement ProjectXmlElement
		{
			get
			{
				return new XmlElement(projectXmlString, true);
			}
		}

		private static Stream open(string filePath)
		{
			return File.OpenRead(filePath);
		}
	}
}
