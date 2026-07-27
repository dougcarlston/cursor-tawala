// $Workfile: IXmlElement.cs $
// $Revision: 11 $	$Date: 7/03/06 5:13p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.Specialized;
using System.Collections.ObjectModel;

namespace Tawala.XmlSupport
{
	public interface IXmlElement
	{
		/// <summary>
		/// Returns the name of this XML element
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Returns a child of this XML element
		/// </summary>
		IXmlElement GetChild(string childName);
		IXmlElement GetChild(int elementIndex);

		/// <summary>
		/// Returns a list of children of this XML element
		/// </summary>
		Collection<XmlElement> GetChildren();
		Collection<XmlElement> GetChildren(string childName);

		/// <summary>
		/// Returns a list of descendants of this XML element
		/// </summary>
		Collection<XmlElement> GetDescendants(string descendantName);
		Collection<XmlElement> GetDescendants(params string[] descendantNames);
		bool HasDescendants(params string[] descendantNames);

		/// <summary>
		/// Returns the value of the specified attribute of the XML element
		/// </summary>
		string GetAttribute(string attributeName);
		string GetAttribute(string attributeName, string defaultAttribute);
		int GetAttribute(string attributeName, int defaultAttribute);

		/// <summary>
		/// Returns an array of attribute names
		/// </summary>
		StringCollection GetAttributeNames();
	
		/// <summary>
		/// Indicates whether this XML element contains the specified attribute
		/// </summary>
		bool HasAttribute(string attributeName);

		/// <summary>
		/// Indicates whether this XML element has the specified child
		/// </summary>
		bool HasChild(string childName);

		/// <summary>
		/// Returns the value of this XML element
		/// </summary>
		string Value { get; }

		/// <summary>
		/// Returns the text contained by this XML element
		/// </summary>
		string Text { get; }

		/// <summary>
		/// Returns the XML contained by this XML element
		/// </summary>
		string InnerXml { get; }
		string OuterXml { get; }
	}
}
