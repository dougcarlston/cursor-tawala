// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Tawala.XmlSupport;

namespace Tawala.Projects.Images
{
	public class ImageData : IImageData
	{
		private string imageFormat;
		private string imageDataString;

		public ImageData(IXmlElement element)
		{
			this.imageFormat = element.GetAttribute("imageFormat");
			this.imageDataString = element.Text;
		}

		public ImageData(string path)
		{
			this.imageFormat = Path.GetExtension(path).Replace(".", "").ToUpper();
			this.imageDataString = base64ImageDataString(path);
		}

		private string base64ImageDataString(string path)
		{
			FileStream imageFile;
			byte[] binaryData;

			imageFile = new System.IO.FileStream(path, FileMode.Open, FileAccess.Read);
			binaryData = new Byte[imageFile.Length];
			long bytesRead = imageFile.Read(binaryData, 0, (int)imageFile.Length);
			imageFile.Close();

			return Convert.ToBase64String(binaryData, 0, binaryData.Length);
		}

		#region IImageData Members

		public string CreateImageFile()
		{
			string path = Path.GetTempFileName().Replace(".tmp", "." + imageFormat.ToLower());

			using (FileStream imageFile = File.Create(path))
			{
				byte[] imageData = Convert.FromBase64String(imageDataString);
				imageFile.Write(imageData, 0, imageData.Length);
			}

			return path;
		}

		public string ToXml()
		{
			StringBuilder xmlString = new StringBuilder();

			xmlString.AppendFormat("<imagedata imageFormat=\"{0}\">", imageFormat);
			xmlString.Append(imageDataString);
			xmlString.Append("</imagedata>");

			return xmlString.ToString();
		}

		#endregion
	}
}
