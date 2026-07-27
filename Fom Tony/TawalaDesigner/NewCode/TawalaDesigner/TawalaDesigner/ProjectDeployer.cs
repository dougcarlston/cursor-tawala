// Copyright © 2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Tawala.Common;
using Tawala.Projects;
using Tawala.Projects.Factories;
using Tawala.Projects.Deployment;
using Tawala.XmlSupport;

namespace TawalaDesigner
{
	public class ProjectDeployer
	{
		private string projectXml;

		public ProjectDeployer(string projectXml)
		{
			this.projectXml = projectXml;
		}

		public IDeploymentResponse Deploy()
		{
			string uploadResultXml = uploadProjectXml();
			IDeploymentResponse response = DeploymentFactory.MakeObject(new XmlElement(uploadResultXml)) as IDeploymentResponse;

			return response;
		}

		private string uploadProjectXml()
		{
			XMLTransceiver transceiver = new XMLTransceiver(Config.ClientURL);

			transceiver.Transmit(projectXml);
			string resultXml = transceiver.Receive();

			resultXml = Regex.Match(resultXml, @"<response.*</response>", RegexOptions.Singleline).Value;
			return resultXml;
		}
	}
}
