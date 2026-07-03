// $Workfile: XmlElement.cs $
// $Revision: 19 $	$Date: 2/04/08 1:09p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.
using System;
using System.Xml;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Tawala.XmlSupport
{
	/// <summary>
	/// Implements an XML hierarchy containing only System.Xml.XmlElement and System.Xml.XmlText nodes.
	/// </summary>
	public class XmlElement : IXmlElement
	{
		public static IXmlElement NULL = new XmlElement("<nullElement/>");

		private System.Xml.XmlNode rawElement;
		private Collection<XmlElement> children = new Collection<XmlElement>();

		public XmlElement(XmlReader reader)
			: this(reader.ReadOuterXml(), false)
		{
		}

		public XmlElement(XmlReader reader, bool preserveWhitespace)
			: this(reader.ReadOuterXml(), preserveWhitespace)
		{
		}

		public XmlElement(string xmlString)
			: this(xmlString, false)
		{
		}

		public XmlElement(string xmlString, bool preserveWhitespace)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.PreserveWhitespace = preserveWhitespace;
			xmlDocument.LoadXml(xmlString);
			this.rawElement = xmlDocument.DocumentElement;
			addChildren();
		}

		/// <summary>
		/// Construct XmlElement from XML node.
		/// </summary>
		private XmlElement(XmlNode node)
		{
			this.rawElement = node;
			addChildren();
		}

		/// <summary>
		/// Make Element nodes available as children of this XmlElement.
		/// </summary>
		private void addChildren()
		{
			foreach (XmlNode node in rawElement)
			{
				if (node is System.Xml.XmlText || node is System.Xml.XmlElement || node is System.Xml.XmlWhitespace || node is System.Xml.XmlCDataSection)
				{
					children.Add(new XmlElement(node));
				}
			}
		}

		#region IXmlElement Interface

		public string Name
		{
			get
			{
				return rawElement.Name;
			}
		}

		public IXmlElement GetChild(string childName)
		{
			foreach (XmlElement element in children)
			{
				if (element.Name == childName)
				{
					return element;
				}
			}

			return XmlElement.NULL;
		}

		public IXmlElement GetChild(int elementIndex)
		{
			if (elementIndex >= children.Count)
			{
				return XmlElement.NULL;
			}

			return (children[elementIndex] != null ? children[elementIndex] : XmlElement.NULL);
		}

		public Collection<XmlElement> GetChildren()
		{
			return children;
		}

		public Collection<XmlElement> GetChildren(string childName)
		{
			Collection<XmlElement> matchingChildren = new Collection<XmlElement>();

			foreach (XmlElement element in children)
			{
				if (element.Name == childName)
				{
					matchingChildren.Add(element);
				}
			}

			return matchingChildren;
		}

		public Collection<XmlElement> GetDescendants(string descendantName)
		{
			return getDescendants(children, descendantName);
		}

		private Collection<XmlElement> getDescendants(Collection<XmlElement> elements, string descendantName)
		{
			Collection<XmlElement> matchingDescendants = new Collection<XmlElement>();

			foreach (XmlElement element in elements)
			{
				if (element.Name == descendantName)
				{
					matchingDescendants.Add(element);
				}

				Collection<XmlElement> descendants = getDescendants(element.GetChildren(), descendantName);

				foreach (XmlElement descendant in descendants)
				{
					matchingDescendants.Add(descendant);
				}
			}

			return matchingDescendants;
		}

		public Collection<XmlElement> GetDescendants(params string[] descendantNames)
		{
			return getDescendants(children, descendantNames);
		}

		public bool HasDescendants(params string[] descendantNames)
		{
			return (getDescendants(children, descendantNames).Count > 0);
		}

		private Collection<XmlElement> getDescendants(Collection<XmlElement> elements, params string[] descendantNames)
		{
			Collection<XmlElement> matchingDescendants = new Collection<XmlElement>();

			foreach (XmlElement element in elements)
			{
				foreach (string descendantName in descendantNames)
				{
					if (element.Name == descendantName)
					{
						matchingDescendants.Add(element);
					}
				}

				Collection<XmlElement> descendants = getDescendants(element.GetChildren(), descendantNames);

				foreach (XmlElement descendant in descendants)
				{
					matchingDescendants.Add(descendant);
				}
			}

			return matchingDescendants;
		}

		public string GetAttribute(string attributeName)
		{
			if (HasAttribute(attributeName))
			{
				return rawElement.Attributes[attributeName].Value;
			}

			return null;
		}

		public string GetAttribute(string attributeName, string defaultAttribute)
		{
			return (HasAttribute(attributeName) ? GetAttribute(attributeName) : defaultAttribute);
		}

		public int GetAttribute(string attributeName, int defaultAttribute)
		{
			return (HasAttribute(attributeName) ? Convert.ToInt32(GetAttribute(attributeName)) : defaultAttribute);
		}

		public StringCollection GetAttributeNames()
		{
			StringCollection attributeNames = new StringCollection();

			foreach (XmlAttribute attribute in rawElement.Attributes)
			{
				attributeNames.Add(attribute.Name);
			}

			return attributeNames;
		}

		public bool HasAttribute(string attributeName)
		{
			if (rawElement.Attributes[attributeName] == null)
			{
				return false;
			}

			return (rawElement.Attributes[attributeName].Value != null);
		}

		public bool HasChild(string childName)
		{
			return (GetChild(childName) != XmlElement.NULL);
		}

		public string Value
		{
			get
			{
				return rawElement.Value;
			}
		}

		public string InnerXml
		{
			get
			{
				return rawElement.InnerXml;
			}
		}

		public string OuterXml
		{
			get
			{
				return rawElement.OuterXml;
			}
		}

		public string Text
		{
			get
			{
				if (GetChild("#text") != null)
				{
					if (GetChild("#text").Value != null)
					{
						return GetChild("#text").Value;
					}
				}

				if (GetChild("#whitespace") != null)
				{
					if (GetChild("#whitespace").Value != null)
					{
						return GetChild("#whitespace").Value;
					}
				}

				return null;
			}
		}

		#endregion

	}
}
