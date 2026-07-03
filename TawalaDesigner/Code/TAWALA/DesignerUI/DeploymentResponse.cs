// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Xml;
using Tawala.DesignerUI.Properties;
using Tawala.Projects;

namespace Tawala.DesignerUI
{
    /// <summary>
    /// Summary description for DeploymentResponse.
    /// </summary>
    public class DeploymentResponse
    {
        private string errorID;
        private string errorMessage;
        private XmlNodeList nodeList;
        private StartingPointList startingPoints = new StartingPointList();
        private bool successful;

        public DeploymentResponse()
        {
        }

        public DeploymentResponse(string responseString)
        {
            // guilty until proven innocent
            successful = false;

            // place received string in xml document
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(responseString);

            // get the response node
            nodeList = xmlDoc.GetElementsByTagName("response");

            Debug.Assert(nodeList.Count == 1);

            // extract deployment success string
            XmlNode node = nodeList[0];
            XmlAttributeCollection attrColl = node.Attributes;
            var attr = (XmlAttribute)attrColl.GetNamedItem("status");

            if (attr.Value.ToUpper() == "SUCCESS")
            {
                // get list of deployment nodes
                nodeList = xmlDoc.GetElementsByTagName("deployment");

                // for each node in list...
                for (int i = 0; i < nodeList.Count && !successful; i++)
                {
                    // extract project string
                    node = nodeList[i];
                    attrColl = node.Attributes;
                    attr = (XmlAttribute)attrColl.GetNamedItem("project");

                    // if it matches the current project
                    if (attr.Value == Project.Current.Name)
                    {
                        // get the startpoints
                        XmlNodeList childNodes = node.ChildNodes;

                        for (int j = 0; j < childNodes.Count; j++)
                        {
                            if (childNodes[j].Name == "startpoint")
                            {
                                // extract startpoint info
                                attrColl = childNodes[j].Attributes;
                                attr = (XmlAttribute)attrColl.GetNamedItem("form");
                                var attrUrl = (XmlAttribute)attrColl.GetNamedItem("url");
                                var info = new StartingPointInfo(attr.Value, attrUrl.Value);
                                startingPoints.Add(info);
                            }
                        }
                    }
                }

                successful = startingPoints.Count > 0;
                if (!successful)
                {
                    errorMessage = Resources.ProjectNotFound;
                }
            }
            else
            {
                // get list of error nodes
                nodeList = xmlDoc.GetElementsByTagName("error");

                // for each node in list...
                for (int i = 0; i < nodeList.Count; i++)
                {
                    // extract error message string
                    node = nodeList[i];
                    attrColl = node.Attributes;
                    attr = (XmlAttribute)attrColl.GetNamedItem("id");
                    errorID = attr.Value;
                    attr = (XmlAttribute)attrColl.GetNamedItem("message");
                    errorMessage = attr.Value;
                }
            }
        }

        public StartingPointList StartingPoints { get { return startingPoints; } }

        public bool Successful { get { return successful; } }

        public string ErrorID { get { return errorID; } }

        public string ErrorMessage { get { return errorMessage; } }

        #region Nested type: StartingPointInfo

        /// <summary>
        /// class to encapsulate Starting Point data
        /// </summary>
        public class StartingPointInfo
        {
            private string name;

            private string url;

            public StartingPointInfo(string name, string url)
            {
                this.name = name;
                this.url = url;
            }

            public string Name { get { return name; } }

            public string URL { get { return url; } }
        }

        #endregion

        #region Nested type: StartingPointList

        public class StartingPointList : Collection<StartingPointInfo>
        {
        }

        #endregion
    }
}