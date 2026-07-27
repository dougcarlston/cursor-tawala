// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using Tawala.XmlSupport;

namespace Tawala.Projects.Images
{
	public class ImageDefinition : IImageDefinition, IProjectComponent
	{
		private string imageId;
		IImageData imageData;

		public ImageDefinition(IXmlElement element)
		{
			this.imageId = element.GetAttribute("id");
			this.imageData = new ImageData(element.GetChild("imagedata"));
		}

		public ImageDefinition(string path, string id)
		{
			this.imageId = id;
			this.pathName = path;

			this.imageData = new ImageData(path);
		}

		#region IProjectComponent Members

		public string ToXml()
		{
			StringBuilder xmlString = new StringBuilder();

			xmlString.AppendFormat("<imagedef id=\"{0}\">", imageId);
			xmlString.Append(imageData.ToXml());
			xmlString.Append("</imagedef>");

			return xmlString.ToString();
		}

		#endregion

		private string pathName = string.Empty;

		#region IImageDefinition Members

		public string PathName
		{
			get
			{
				if (string.IsNullOrEmpty(pathName))
				{
					pathName = imageData.CreateImageFile();
				}

				return pathName;
			}
		}

		public string Id
		{
			get { return imageId; }
		}

		#endregion
	}
}
