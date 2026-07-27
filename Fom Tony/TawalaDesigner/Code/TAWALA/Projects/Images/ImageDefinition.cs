// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Text;
using Tawala.Projects.Components;
using Tawala.XmlSupport;

namespace Tawala.Projects.Images
{
    public class ImageDefinition : IImageDefinition, IProjectComponentXml
    {
        private readonly IImageData imageData;
        private readonly string imageId;
        private string pathName = string.Empty;

        public ImageDefinition(IXmlElement element)
        {
            imageId = element.GetAttribute("id");
            imageData = new ImageData(element.GetChild("imagedata"));
        }

        public ImageDefinition(string path, string id)
        {
            imageId = id;
            pathName = path;

            imageData = new ImageData(path);
        }

        #region IImageDefinition Members

        public string ToXml()
        {
            var xmlString = new StringBuilder();

            xmlString.AppendFormat("<imagedef id=\"{0}\">", imageId);
            xmlString.Append(imageData.ToXml());
            xmlString.Append("</imagedef>");

            return xmlString.ToString();
        }

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

        public string Id { get { return imageId; } }

        #endregion
    }
}