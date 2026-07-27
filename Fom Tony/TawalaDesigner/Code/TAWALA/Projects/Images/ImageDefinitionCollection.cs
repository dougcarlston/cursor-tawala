// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.ObjectModel;
using System.Text;
using Tawala.Projects.Components;
using Tawala.Projects.Factories;
using Tawala.XmlSupport;

namespace Tawala.Projects.Images
{
    public class ImageDefinitionCollection : Collection<IImageDefinition>, IImageDefinitionCollection, IProjectComponentXml
    {
        private static int nextIdNumber = 1;

        public ImageDefinitionCollection()
        {
            nextIdNumber = 1;
        }

        public ImageDefinitionCollection(IXmlElement element) : this()
        {
            foreach (IProjectComponentXml component in ProjectComponentFactory.MakeChildren(element))
            {
                Add(component as IImageDefinition);
            }
        }

        #region IImageDefinitionCollection Members

        public string ToXml()
        {
            var xmlString = new StringBuilder();

            if (Count > 0)
            {
                xmlString.Append("<images>");

                foreach (IImageDefinition imageDefinition in this)
                {
                    xmlString.Append(imageDefinition.ToXml());
                }

                xmlString.Append("</images>");
            }

            return xmlString.ToString();
        }

        public string Add(string pathName)
        {
            foreach (IImageDefinition imageDefinition in this)
            {
                if (imageDefinition.PathName == pathName)
                {
                    return imageDefinition.Id;
                }
            }

            string id = getNextImageId();

            Add(new ImageDefinition(pathName, id));

            return id;
        }

        public void Remove(string id)
        {
            throw new NotImplementedException();
        }

        public IImageDefinition this[string id]
        {
            get
            {
                foreach (IImageDefinition imageDefinition in this)
                {
                    if (imageDefinition.Id == id)
                    {
                        return imageDefinition;
                    }
                }

                return null;
            }
        }

        public IImageDefinition GetImageDefinitionByPathName(string pathName)
        {
            foreach (IImageDefinition imageDefinition in this)
            {
                if (imageDefinition.PathName == pathName)
                {
                    return imageDefinition;
                }
            }

            IImageDefinition newImageDefinition = new ImageDefinition(pathName, getNextImageId());
            Add(newImageDefinition);

            return newImageDefinition;
        }

        #endregion

        private string getNextImageId()
        {
            return string.Format("image{0}", nextIdNumber++);
        }
    }
}