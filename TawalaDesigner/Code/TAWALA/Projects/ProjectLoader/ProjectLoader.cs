// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.IO;
using Tawala.XmlSupport;

#if false
namespace Tawala.Projects.ProjectLoader
{
    public class ProjectLoader
    {
        private static string projectXmlString;

        public static string ProjectXmlString { get { return projectXmlString; } }

        public static IXmlElement ProjectXmlElement { get { return new XmlElement(projectXmlString, true); } }

        public static void Load(string filePath)
        {
            projectXmlString = new StreamReader(open(filePath)).ReadToEnd();
        }

        private static Stream open(string filePath)
        {
            return File.OpenRead(filePath);
        }
    }
}
#endif