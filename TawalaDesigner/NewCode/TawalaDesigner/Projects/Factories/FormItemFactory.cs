// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.XmlSupport;

namespace Tawala.Projects.Factories
{
	/// <summary>
	/// Produces Tawala FormItem objects from XML elements.
	/// </summary>
	/// <remarks>
	/// This class is used by the new form designer and will eventually 
	/// replace corresponding functionality in the old Tawala Project Converter.
	/// </remarks>
	public static class FormItemFactory
	{
		private static Factory<IFormItem> formItemFactory = new Factory<IFormItem>();

		static FormItemFactory()
		{
			formItemFactory.Register("heading", typeof(NewHeadingItem));
			formItemFactory.Register("text", typeof(NewTextItem));
			formItemFactory.Register("fib", typeof(NewFibItem));
			formItemFactory.Register("mc", typeof(NewMcqItem));
			formItemFactory.Register("field", typeof(NewHiddenField));
			formItemFactory.Register("break", typeof(BreakItem));
			formItemFactory.Register("skipInstructions", typeof(NewSkipInstructionsItem));
		}

		public static IFormItem MakeObject(IXmlElement element)
		{
			IFormItem formItem = formItemFactory.MakeObject(element);

			if (formItem == null)
			{
				return FormItem.NULL;
			}

			return formItem;
		}

		public static void MakeChildren(IXmlElement element, FormItemList formItemList)
		{
			foreach (IXmlElement childElement in element.GetChildren())
			{
				formItemList.Add(formItemFactory.MakeObject(childElement));
			}
		}
	}
}
