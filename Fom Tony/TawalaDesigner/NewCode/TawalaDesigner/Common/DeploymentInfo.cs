// Copyright © 2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace Tawala.Common
{
	/// <summary>
	/// Form names (starting points only) to URLs
	/// </summary>
	public class StartingPoints : Dictionary<string, string>
	{
	}

	/// <summary>
	/// Deployed project names to Starting Points
	/// </summary>
	public class Deployments : Dictionary<string, StartingPoints>
	{
	}

	public static class DeploymentInfo
	{
		public enum Error { None, Authentication, General };

		private static Deployments projects = new Deployments();

		public static Deployments Projects
		{
			get
			{
				return projects;
			}
		}

		private static Error lastError = Error.None;

		public static Error LastError
		{
			get
			{
				return lastError;
			}
		}

		private static readonly string queryXML = 
			"<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
			"<request type=\"queryDeployments\" protocol=\"1.0\">" +
			    "{0}" +
			"</request>";


		/// <summary>
		/// Get Deployments Info from Server.  Intended to be part of a two part process used with
		/// background worker thread in Invitation Manager and Designer
		/// </summary>
		public static Error QueryServer(string credentialsXml)
		{
			lastError = Error.General;

			try
			{
				XMLTransceiver transceiver = new XMLTransceiver(Config.ClientURL);

				// send query to URL - credentials come from GlobalSettings
				StringBuilder sb = new StringBuilder();
				sb.AppendFormat(queryXML, credentialsXml);
				transceiver.Transmit(sb.ToString());

				// get text from URL
				string result = transceiver.Receive();

				XPathDocument xml = new XPathDocument(new StringReader(result));
				XPathNavigator nav = xml.CreateNavigator();
				XPathNavigator status = nav.SelectSingleNode("/response/@status");

				if (status != null)
				{
					if (status.Value.CompareTo("failure") == 0)
					{
						XPathNavigator id = nav.SelectSingleNode("/response/error/@id");

						if (id != null && id.Value.CompareTo("auth.failed") == 0)
						{
							lastError = Error.Authentication;
						}
					}
					else if (status.Value.CompareTo("success") == 0)
					{
						// parse to temp projects in case of errors
						Deployments tempProjects = new Deployments();

						XPathNodeIterator deps = nav.Select("//deployment");

						foreach (XPathNavigator dep in deps)
						{
							string projName = dep.GetAttribute("project", "");
							StartingPoints startPoints = new StartingPoints();
							tempProjects.Add(projName, startPoints);

							foreach (XPathNavigator sp in dep.Select("startpoint"))
							{
								startPoints.Add(sp.GetAttribute("form", ""), sp.GetAttribute("url", ""));
							}
						}

						// no errors so now assign to projects
						projects = tempProjects;
						lastError = Error.None;
					}
				}
			}
			catch (Exception)
			{
			}

			return lastError;
		}
	}
}
