// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.IO;
using System.Text;
using Tawala.XmlSupport;

namespace Tawala.Projects.Images
{
    public class ImageData : IImageData
    {
        private readonly string imageDataString;
        private readonly string imageFormat;

        public ImageData(IXmlElement element)
        {
            imageFormat = element.GetAttribute("imageFormat");
            imageDataString = element.Text;
        }

        public ImageData(string path)
        {
            imageFormat = Path.GetExtension(path).Replace(".", "").ToUpper();
            imageDataString = base64ImageDataString(path);
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
            var xmlString = new StringBuilder();

            xmlString.AppendFormat("<imagedata imageFormat=\"{0}\">", imageFormat);
            xmlString.Append(imageDataString);
            xmlString.Append("</imagedata>");

            return xmlString.ToString();
        }

        #endregion

        private string base64ImageDataString(string path)
        {
            FileStream imageFile;
            byte[] binaryData;

            imageFile = new FileStream(path, FileMode.Open, FileAccess.Read);
            binaryData = new Byte[imageFile.Length];
            long bytesRead = imageFile.Read(binaryData, 0, (int)imageFile.Length);
            imageFile.Close();

            return Convert.ToBase64String(binaryData, 0, binaryData.Length);
        }
    }
}