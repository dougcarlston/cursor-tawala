// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;

namespace Tawala.Functions.Runtime.CodeGen
{
    public sealed class Generator : IGenerator
    {
        #region IGenerator Members

        CompilationInfo IGenerator.Build(FunctionLoader.BuildInfo buildInfo)
        {
            string repositoryXml = buildInfo.RepositoryXml;

            var xml = new XmlDocument();
            xml.LoadXml(repositoryXml);

            assignGlobalIdsToNodes(xml);

            var ms = new MemoryStream();
            xml.Save(ms);
            ms.Seek(0, SeekOrigin.Begin);

            XPathNavigator repositoryRoot = new XPathDocument(ms).CreateNavigator();
            repositoryRoot.MoveToFirstChild();

            return new FunctionAssemblyCreator(repositoryRoot, buildInfo).BuildFunctionAssembly();
        }

        #endregion

        private void assignGlobalIdsToNodes(XmlDocument xml)
        {
            int gid = 1000;
            foreach (XmlNode n in xml.SelectNodes("//*[(@id | @name)]"))
            {
                XmlAttribute attr = xml.CreateAttribute("gid");
                attr.Value = Convert.ToString(gid++);
                n.Attributes.Append(attr);
            }
        }
    }
}