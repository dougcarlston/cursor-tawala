// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Text;
using Tawala.Projects.Factories;
using Tawala.XmlSupport;

namespace Tawala.Projects.Images
{
	public class ImageDefinitionCollection : Collection<IImageDefinition>, IImageDefinitionCollection, IProjectComponent
	{
		public ImageDefinitionCollection()
		{
			nextIdNumber = 1;
		}

		public ImageDefinitionCollection(IXmlElement element) : this()
		{
			foreach (IProjectComponent component in ProjectComponentFactory.MakeChildren(element))
			{
				Add(component as IImageDefinition);
			}
		}

		#region IProjectComponent Members

		public string ToXml()
		{
			StringBuilder xmlString = new StringBuilder();

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

		#endregion

		#region IImageDefinitionCollection Members

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

		private static int nextIdNumber = 1;

		private string getNextImageId()
		{
			return string.Format("image{0}", nextIdNumber++);
		}
	}
}
