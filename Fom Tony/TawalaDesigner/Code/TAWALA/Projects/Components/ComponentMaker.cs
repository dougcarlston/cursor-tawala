// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Reflection;
using Tawala.Projects.Documents;
using Tawala.XmlSupport;

namespace Tawala.Projects.Components
{
    public static class ComponentMaker
    {
        private static Type documentType = typeof(RtfDocument);
        private static Type formType = typeof(Form);

        public static IForm MakeFormObject(IXmlElement element)
        {
            return formType.InvokeMember("", BindingFlags.CreateInstance, null, null, new object[] {element}) as IForm;
        }

        public static IForm MakeFormObject(string name)
        {
            return formType.InvokeMember("", BindingFlags.CreateInstance, null, null, new object[] {name}) as IForm;
        }

        public static IDocument MakeDocumentObject(IXmlElement element)
        {
            return documentType.InvokeMember("", BindingFlags.CreateInstance, null, null, new object[] {element}) as IDocument;
        }

        public static IDocument MakeDocumentObject(string name)
        {
            return documentType.InvokeMember("", BindingFlags.CreateInstance, null, null, new object[] {name}) as IDocument;
        }

        public static void UseNewComponents(bool useNewComponents)
        {
            formType = typeof(Form);
            documentType = typeof(RtfDocument);
        }
    }
}