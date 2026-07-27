// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;
using Tawala.Projects.Factories;
using Tawala.XmlSupport;

namespace Tawala.Projects.Documents
{
    /// <summary>
    /// List of image definitions.
    /// </summary>
    [Serializable]
    public class GraphicImageList : Collection<GraphicImage>
    {
        private static readonly Factory<GraphicImage> factory = new Factory<GraphicImage>();

        static GraphicImageList()
        {
            factory.Register("imagedef", typeof(GraphicImageDefinition));
        }

        public GraphicImageList()
        {
        }

        public GraphicImageList(IXmlElement element)
        {
            foreach (IXmlElement childElement in element.GetChildren("imagedef"))
            {
                if (!childElement.GetAttribute("id").StartsWith(PageHeader.HeaderImageIdPrefix))
                {
                    Add(factory.MakeObject(childElement));
                }
                else
                {
                    Project.Current.PageHeader.SetImageDefXml(childElement);
                }
            }
        }

        public GraphicImage this[string id]
        {
            get
            {
                foreach (GraphicImage currentImage in this)
                {
                    if (currentImage.Id == id)
                    {
                        return currentImage;
                    }
                }

                return null;
            }
        }

        public GraphicImage this[GraphicImage image]
        {
            get
            {
                foreach (GraphicImage currentImage in this)
                {
                    if (imagesMatch(image, currentImage))
                    {
                        return currentImage;
                    }
                }

                return null;
            }
        }

        public void AddUnique(GraphicImageDefinition image)
        {
            foreach (GraphicImageDefinition currentImage in this)
            {
                if (imagesMatch(image, currentImage))
                {
                    return;
                }
            }

            if (image.Id == "Unidentified Image")
            {
                image.Id = getNextImageId();
            }

            Add(image);
        }

        private string getNextImageId()
        {
            return String.Format("image{0}", getGreatestImageNumber() + 1);
        }

        private int getGreatestImageNumber()
        {
            int greatestNumber = 0;

            foreach (GraphicImage currentImage in this)
            {
                int number = Convert.ToInt32(Regex.Match(currentImage.Id, @"image(\d+)").Groups[1].Value);

                greatestNumber = (number > greatestNumber ? number : greatestNumber);
            }

            return greatestNumber;
        }

        public bool ContainsMatchWith(GraphicImage image)
        {
            foreach (GraphicImage currentImage in this)
            {
                if (imagesMatch(image, currentImage))
                {
                    return true;
                }
            }

            return false;
        }

        public int IndexMatching(GraphicImage image)
        {
            for (int i = 0; i < Count; i++)
            {
                GraphicImage currentImage = this[i];

                if (imagesMatch(image, currentImage))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Returns a Boolean indicating whether the specified images are identical.
        /// </summary>
        private bool imagesMatch(GraphicImage image, GraphicImage currentImage)
        {
            return (image.Base64PngString.GetHashCode() == currentImage.Base64PngString.GetHashCode());
        }

        public string ToXml()
        {
            var xmlString = new StringBuilder();

            string headerXmlDef = Project.Current.PageHeader.ToImageDefXml();

            if (Count > 0 || !string.IsNullOrEmpty(headerXmlDef))
            {
                xmlString.Append("<images>");

                foreach (GraphicImage image in this)
                {
                    xmlString.Append(image.ToXml());
                }

                if (!string.IsNullOrEmpty(headerXmlDef))
                {
                    xmlString.Append(headerXmlDef);
                }

                xmlString.Append("</images>");
            }

            return xmlString.ToString();
        }

        public GraphicImage GetImageById(string searchId)
        {
            foreach (GraphicImage image in this)
            {
                if (image.Id.Equals(searchId))
                {
                    return image;
                }
            }

            return GraphicImage.NULL;
        }
    }
}