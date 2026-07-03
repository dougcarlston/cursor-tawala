// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Tawala.XmlSupport;

namespace Tawala.Projects.Forms.FormItemContents
{
	public class ImageList : Collection<IImageReference>
	{
		public ImageList()
		{
			nextIdNumber = 1;
		}

		public void AddUnique(IImageReference newImage)
		{
			foreach(IImageReference image in this)
			{
				if (image.Url == newImage.Url)
				{
					newImage.Id = image.Id;
					return;
				}
			}

			newImage.Id = getNextImageId();
			Add(newImage);
		}

		private static int nextIdNumber = 1;

		private string getNextImageId()
		{
			return string.Format("image{0}", nextIdNumber++);
		}

		public string ToXml()
		{
			StringBuilder xmlString = new StringBuilder();

			if (Count > 0)
			{
				xmlString.Append("<images>");

				foreach (IImageReference image in this)
				{
					xmlString.AppendFormat("<imagedef id=\"{0}\">", image.Id);
					xmlString.AppendFormat("<imagedata imageFormat=\"{0}\">", image.ImageFormat);
					xmlString.Append(base64ImageDataString(image));
					xmlString.Append("</imagedata>");
					xmlString.Append("</imagedef>");
				}

				xmlString.Append("</images>");
			}

			return xmlString.ToString();
		}

		private string base64ImageDataString(IImageReference image)
		{
			FileStream imageFile;
			byte[] binaryData;

			imageFile = new System.IO.FileStream(image.PathName, FileMode.Open, FileAccess.Read);
			binaryData = new Byte[imageFile.Length];
			long bytesRead = imageFile.Read(binaryData, 0, (int)imageFile.Length);
			imageFile.Close();

			return Convert.ToBase64String(binaryData, 0, binaryData.Length);
		}
	}
}
