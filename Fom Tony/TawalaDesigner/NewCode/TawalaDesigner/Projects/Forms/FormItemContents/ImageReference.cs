// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Tawala.Projects.Images;
using Tawala.XmlSupport;

namespace Tawala.Projects.Forms.FormItemContents
{
	[Serializable]
	public class ImageReference : FormItemContents, IImageReference
	{
		public override string ToXml()
		{
			return string.Format(@"<image id=""{0}"" width=""{1}"" height=""{2}""></image>", Id, Width, Height);
		}

		public override string ToXhtml(IFormItem formItem)
		{
			return string.Format(@"<img src=""{0}"" />", Url);
		}

		#region IImageReference Members

		public string Url
		{
			get; protected set;
		}

		public string PathName
		{
			get
			{
				return urlDecode(Url.Replace("file:///", "").Replace("/", "\\"));
			}
		}

		private string urlDecode(string encodedString)
		{
			string decodedString = encodedString;

			decodedString = decodedString.Replace("%20", " ");
			decodedString = decodedString.Replace("%5B", "[");
			decodedString = decodedString.Replace("%5D", "]");
			decodedString = decodedString.Replace("%5E", "^");
			decodedString = decodedString.Replace("%60", "`");
			decodedString = decodedString.Replace("%7B", "{");
			decodedString = decodedString.Replace("%7D", "}");
			decodedString = decodedString.Replace("%7E", "~");

			return decodedString;
		}

		public string Id
		{
			get;
			set;
		}

		public int Width
		{
			get;
			protected set;
		}

		public int Height
		{
			get;
			protected set;
		}

		public string ImageFormat
		{
			get { return "JPG"; }
		}

		#endregion
	}

	[Serializable]
	public class ImageXmlReference : ImageReference
	{
		public ImageXmlReference(IXmlElement element)
		{
			Width = Convert.ToInt32(element.GetAttribute("width"));
			Height = Convert.ToInt32(element.GetAttribute("height"));

			Id = element.GetAttribute("id");
			IImageDefinition imageDefinition = Project.NewImages[Id];

			Url = "file:///" + imageDefinition.PathName;
			Url = Url.Replace("\\", "/");
		}
	}

	[Serializable]
	public class ImageXhtmlReference : ImageReference
	{
		public ImageXhtmlReference(IXmlElement element)
		{
			if (element.HasAttribute("src"))
			{
				Width = Convert.ToInt32(element.GetAttribute("ImageWidth"));
				Height = Convert.ToInt32(element.GetAttribute("ImageHeight"));
				Url = element.GetAttribute("src");
				Id = Project.NewImages.GetImageDefinitionByPathName(PathName).Id;
			}
		}
	}
}
