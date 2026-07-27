// $Workfile: ExternalFieldProvider.cs $
// $Revision: 2 $	$Date: 6/10/07 10:28p $
// Copyright © 2007 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using Tawala.Projects.Fields;
using Tawala.XmlSupport;

namespace Tawala.Projects
{
	public class ExternalFieldProvider
	{
		private List<ExternalField> fields;
		private string name;

		internal ExternalFieldProvider(IXmlElement element)
		{
			name = element.GetAttribute("name");

			Collection<XmlElement> fieldElements = element.GetChildren("field");
			fields = new List<ExternalField>();

			foreach (IXmlElement fieldElement in fieldElements)
			{
				fields.Add(new ExternalField(this, fieldElement));
			}
		}

		public IList<ExternalField> Fields
		{
			get { return fields; }
		}

		public string Name
		{
			get { return name; }
		}
	}
}
