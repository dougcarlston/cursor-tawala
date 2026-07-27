using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;

using System.IO;

using Tawala.XmlSupport;
using Tawala.Common;

namespace Tawala.Projects
{
	public class PageHeader
	{
		public PageHeader()
		{
		}

		public PageHeader(IXmlElement e)
		{
			IXmlElement headerElement = e.GetChild(headerTagName);

			if (headerElement == null)
				return;

			IXmlElement textElement = headerElement.GetChild(textTagName);

			if (textElement != null)
			{
				text = textElement.Text;
			}

			IXmlElement imageElement = headerElement.GetChild(imageTagName);

			if (imageElement != null)
			{
				headerImageId = imageElement.GetAttribute("id");

				width = Convert.ToInt32(imageElement.GetAttribute("width"));
				height = Convert.ToInt32(imageElement.GetAttribute("height"));
			}
		}

		public string Text
		{
			get { return text; }
			set { text = value; }
		}

		public string GetImage()
		{
			return tempImageFile;
		}

		public void SetImage(string fileName)
		{
			headerImageId = string.Empty;

			tempImageFile = fileName;
			width = height = 0;
			imageFormat = string.Empty;

			if (fileName != null)
			{
				copyImageToTemp(fileName);
				updateImageInfo();
				headerImageId = HeaderImageIdPrefix + DateTime.Now.Ticks.ToString("x");
			}
		}

		public bool HasContent()
		{
			return !string.IsNullOrEmpty(text) || !string.IsNullOrEmpty(tempImageFile);
		}

		private void updateImageInfo()
		{
			using (Image image = Image.FromFile(tempImageFile))
			{
				width = image.Width;
				height = image.Height;
				imageFormat = getFormat(image);
			}
		}

		public void SetImageDefXml(IXmlElement element)
		{
			IXmlElement data = element.GetChild("imagedata");

			Debug.Assert(element.GetAttribute("id").CompareTo(headerImageId) == 0);
			
			imageFormat = data.GetAttribute("imageFormat");

			tempImageFile = createTempFile();
			File.WriteAllBytes(tempImageFile, Convert.FromBase64String(data.Text));
		}

		public string ToXml()
		{
			StringBuilder sb = new StringBuilder();

			if (!string.IsNullOrEmpty(text))
			{
				sb.AppendFormat(xmlTextFormat, Common.XMLStringFormatter.EscapeElementText(text));
			}

			if (!string.IsNullOrEmpty(tempImageFile))
			{
				sb.AppendFormat(xmlImageFormat, headerImageId, width, height);
			}

			return string.Format(xmlFormat, sb);
		}

		public string ToImageDefXml()
		{
			if (!string.IsNullOrEmpty(tempImageFile))
			{
				return string.Format(imageDefFormat, headerImageId, imageFormat, Convert.ToBase64String(File.ReadAllBytes(tempImageFile)));
			}
			else
			{
				return string.Empty;
			}
		}

		public const string HeaderImageIdPrefix = "__HEADER__";

		#region private

		private int nextImageNum = 1;

		private string headerImageId = string.Empty;

		private string createTempFile()
		{
			string ticks = DateTime.Now.Ticks.ToString("X");
			string name = string.Format("~phi-{0}-{1}.tmp", ticks, nextImageNum++);
			string fullName = Path.Combine(Config.TemporaryFiles, name);
			using (FileStream fs = File.Create(fullName))
			{
				fs.Close();
			}
			return fullName;
		}

		private void copyImageToTemp(string fileName)
		{
			tempImageFile = createTempFile();
			File.Copy(fileName, tempImageFile, true);
		}

		private static string getFormat(Image image)
		{
			if (image.RawFormat.Equals(ImageFormat.Gif))
				return "GIF";
			if (image.RawFormat.Equals(ImageFormat.Jpeg))
				return "JPEG";
			if (image.RawFormat.Equals(ImageFormat.Png))
				return "PNG";
			return "UNKNOWN";
		}

		private const string headerTagName = "pageHeader";
		private const string textTagName = "text";
		private const string imageTagName = "image";

		private static readonly string xmlFormat = "<" + headerTagName + ">{0}</" + headerTagName + ">";
		private static readonly string xmlTextFormat = "<" + textTagName + ">{0}</" + textTagName + ">";
		private static readonly string xmlImageFormat = "<" + imageTagName + " id=\"{0}\" width=\"{1}\" height=\"{2}\"/>";
		private static readonly string imageDefFormat = "<imagedef id=\"{0}\"><imagedata imageFormat=\"{1}\">{2}</imagedata></imagedef>";

		private string text = string.Empty;
		private string imageFormat = string.Empty;
		private int width = 0, height = 0;

		private string tempImageFile;

		#endregion
	}
}
