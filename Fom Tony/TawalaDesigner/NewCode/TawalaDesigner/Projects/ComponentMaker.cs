// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Tawala.XmlSupport;
using Tawala.Projects.Documents;
using Tawala.Projects.Forms;

namespace Tawala.Projects
{
	public static class ComponentMaker
	{
		public static IForm MakeFormObject(IXmlElement element)
		{
			return formType.InvokeMember("", BindingFlags.CreateInstance, null, null, new object[] { element }) as IForm;
		}

		public static IForm MakeFormObject(string name)
		{
			return formType.InvokeMember("", BindingFlags.CreateInstance, null, null, new object[] { name }) as IForm;
		}

		public static IDocument MakeDocumentObject(IXmlElement element)
		{
			return documentType.InvokeMember("", BindingFlags.CreateInstance, null, null, new object[] { element }) as IDocument;
		}

		public static IDocument MakeDocumentObject(string name)
		{
			return documentType.InvokeMember("", BindingFlags.CreateInstance, null, null, new object[] { name }) as IDocument;
		}

		public static void UseNewComponents(bool useNewComponents)
		{
		}

        private static Type formType = typeof(Form);
        private static Type documentType = typeof(NewDocument);
	}
}
