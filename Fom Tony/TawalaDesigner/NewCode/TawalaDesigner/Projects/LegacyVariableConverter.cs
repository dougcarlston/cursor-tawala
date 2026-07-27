// $Workfile: LegacyVariableConverter.cs $
// $Revision: 11 $	$Date: 8/03/07 9:54a $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml;
using System.Text;

namespace Tawala.Projects
{
	public static class LegacyVariableConverter
	{
		private static XmlDocument xmlDocument;

		public static Stream Convert(Stream inputXmlStream)
		{
			inputXmlStream.Position = 0;

			xmlDocument = new XmlDocument();
			xmlDocument.PreserveWhitespace = true;
			xmlDocument.Load(inputXmlStream);

			if (projectXmlIsPreHiddenField())
			{
				convertProjectXml();

				MemoryStream outputXmlStream = new MemoryStream();
				xmlDocument.Save(outputXmlStream);

				outputXmlStream.Position = 0;
				return outputXmlStream;
			}

			return inputXmlStream;
		}

		private static bool projectXmlIsPreHiddenField()
		{
			XmlElement projectElement = getProjectElement();

			string formatNumber = projectElement.GetAttribute("format");

			int dotPosition = formatNumber.IndexOf(".");
			int majorVersion = System.Convert.ToInt32(formatNumber.Substring(0, dotPosition++));
			int minorVersion = System.Convert.ToInt32(formatNumber.Substring(dotPosition, formatNumber.Length - dotPosition));

			return majorVersion == 1 && minorVersion < 6;
		}

		private static XmlElement getProjectElement()
		{
			foreach (XmlNode node in xmlDocument.ChildNodes)
			{
				if (node.Name == "project")
				{
					return (XmlElement)node;
				}
			}

			foreach (XmlNode node in xmlDocument.DocumentElement.ChildNodes)
			{
				if (node.Name == "project")
				{
					return (XmlElement)node;
				}
			}

			return null;
		}

		private static void convertProjectXml()
		{
			foreach (XmlElement formElement in getFormElements())
			{
				addHiddenFieldsForPreProcess(formElement);
				addHiddenFieldsForPostProcess(formElement);
			}
		}

		private static void addHiddenFieldsForPreProcess(XmlElement formElement)
		{
			XmlElement preProcessElement = getProcessElement(formElement.GetAttribute("preProcess"));
			addHiddenFields(formElement, preProcessElement);
		}

		private static void addHiddenFieldsForPostProcess(XmlElement formElement)
		{
			XmlElement processElement = getProcessElement(formElement.GetAttribute("process"));
			addHiddenFields(formElement, processElement);
		}

		private static void addHiddenFields(XmlElement formElement, XmlElement processElement)
		{
			ISetElement setElements = getSetElements(processElement);

			foreach (ISetElement setElement in setElements.RecursiveEnumerator)
			{
				addHiddenFieldElementToForm(setElement.FieldName, formElement);
			}

			addSetHiddenFieldElementsToProcess(setElements, formElement.GetAttribute("name"));
		}

		private static void addHiddenFieldElementToForm(string fieldName, XmlElement formElement)
		{
			if (!existsInForm(fieldName, formElement))
			{
				XmlElement fieldElement = xmlDocument.CreateElement("field");
				fieldElement.SetAttribute("name", fieldName);

				XmlElement itemsElement = getItemsElement(formElement);

				if (itemsElement == null)
				{
					itemsElement = addItemsElement(formElement);
				}

				itemsElement.AppendChild(fieldElement);
			}
		}

		private static bool existsInForm(string fieldName, XmlElement formElement)
		{
			XmlElement itemsElement = getItemsElement(formElement);

			if (itemsElement != null)
			{
				foreach (XmlNode node in itemsElement)
				{
					XmlElement element = node as XmlElement;

					if (element != null)
					{
						if (element.Name == "field")
						{
							if (element.GetAttribute("name") == fieldName)
							{
								return true;
							}
						}
					}
				}
			}

			return false;
		}

		private static XmlElement getItemsElement(XmlElement formElement)
		{
			foreach (XmlNode node in formElement)
			{
				if (node.Name == "items")
				{
					return node as XmlElement;
				}
			}

			return null;
		}

		private static XmlElement addItemsElement(XmlElement formElement)
		{
			XmlNode itemsElement = xmlDocument.CreateElement("string");
			formElement.AppendChild(itemsElement);

			return (XmlElement)itemsElement;
		}


		private static void addSetHiddenFieldElementsToProcess(ISetElement setElements, string formName)
		{
			foreach (ISetElement setElement in setElements.RecursiveEnumerator)
			{
				XmlElement statementElement = setElement.Element;

				if (statementElement.GetAttribute("field") == setElement.FieldName)
				{
					XmlElement duplicateStatementElement = convertSetStatementToSetHiddenFieldStatement(formName, setElement, statementElement);
					setElement.ContainerElement.InsertAfter(duplicateStatementElement, statementElement);
				}
			}
		}

		/// <summary>
		/// Converts a statement such as "SET Score to 100" to "SET Form 1:Score to Score".
		/// Converts a statement such as "SET Full to <<First>> <<Last>>" to "SET Form 1:Full to Full".
		/// </summary>
		private static XmlElement convertSetStatementToSetHiddenFieldStatement(string formName, ISetElement setElement, XmlElement statementElement)
		{
			XmlElement duplicateStatementElement = (XmlElement)statementElement.Clone();

			duplicateStatementElement.RemoveAll();
			duplicateStatementElement.SetAttribute("field", formName + ":" + setElement.FieldName);
//			duplicateStatementElement.SetAttribute("arithmeticAsText", statementElement.GetAttribute("arithmeticAsText"));

			XmlElement newChild = xmlDocument.CreateElement("string");
			newChild.SetAttribute("field", setElement.FieldName);
			duplicateStatementElement.AppendChild(newChild);

			return duplicateStatementElement;
		}

		/// <summary>
		/// Returns all the &lt;form&gt; elements that have a connected process
		/// </summary>
		/// <returns></returns>
		private static Collection<XmlElement> getFormElements()
		{
			Collection<XmlElement> formElements = new Collection<XmlElement>();

			foreach (XmlNode componentNode in xmlDocument.DocumentElement.ChildNodes)
			{
				XmlElement componentElement = componentNode as XmlElement;

				if (componentElement != null)
				{
					if (componentElement.Name == "forms")
					{
						foreach (XmlNode formNode in componentElement)
						{
							XmlElement formElement = formNode as XmlElement;

							if (formElement != null)
							{
								if (formElement.Name == "form")
								{
									formElements.Add(formElement);
								}
							}
						}
					}
				}
			}

			return formElements;
		}

		private static XmlElement getProcessElement(string processName)
		{
			foreach (XmlNode componentNode in xmlDocument.DocumentElement.ChildNodes)
			{
				XmlElement componentElement = componentNode as XmlElement;

				if (componentElement != null)
				{
					if (componentElement.Name == "processes")
					{
						foreach (XmlNode processNode in componentElement)
						{
							XmlElement processElement = processNode as XmlElement;

							if (processElement != null)
							{
								if (processElement.GetAttribute("name") == processName)
								{
									return processElement;
								}
							}
						}
					}
				}
				else
				{

				}
			}

			return null;
		}

		/// <summary>
		/// This recursive method returns a collection of SetElement objects (representing &lt;set&gt; elements) from within the specified container element.
		/// </summary>
		private static ISetElement getSetElements(XmlElement containerElement)
		{
			SetElementCollection setElements = new SetElementCollection();

			if (containerElement != null)
			{
				foreach (XmlNode node in containerElement)
				{
					XmlElement element = node as XmlElement;

					if (element != null)
					{
						if (element.Name == "set")
						{
							if (!element.GetAttribute("field").Contains(":"))
							{
								ISetElement setElement = new SetElement(element, containerElement);
								setElements.AddElement(setElement);
							}
						}
						else
						{
							setElements.AddElement(getSetElements(element));
						}
					}
				}
			}

			return setElements;
		}

		interface ISetElement : IRecursiveEnumerable
		{
			void AddElement(ISetElement element);
			XmlElement ContainerElement { get; }
			XmlElement Element { get; }
			string FieldName { get; }
		}

		private class SetElement : ISetElement
		{
			private XmlElement element;
			private XmlElement containerElement;

			public SetElement(XmlElement element, XmlElement containerElement)
			{
				this.element = element;
				this.containerElement = containerElement;
			}

			public void AddElement(ISetElement element)
			{
				throw new Exception("SetElement does not support the AddElement method");
			}

			public XmlElement ContainerElement
			{
				get { return containerElement; }
			}

			public XmlElement Element
			{
				get { return element; }
			}

			public string FieldName
			{
				get
				{
					return element.GetAttribute("field");
				}
			}

			#region IRecursiveEnumerable Interface

			public IEnumerable RecursiveEnumerator
			{
				get
				{
					yield return this;
				}
				
			}

			#endregion
		}

		private class SetElementCollection : Collection<ISetElement>, ISetElement
		{
			public void AddElement(ISetElement element)
			{
				Add(element);
			}

			public XmlElement ContainerElement
			{
				get { return null; }
			}

			public XmlElement Element
			{
				get { return null; }
			}

			public string FieldName
			{
				get
				{
					return "";
				}
			}

			#region IRecursiveEnumerable Interface

			public IEnumerable RecursiveEnumerator
			{
				get
				{
					foreach (ISetElement item in this)
					{
						foreach (ISetElement element in item.RecursiveEnumerator)
						{
							yield return element;
						}
					}
				}
			}

			#endregion
		}

	}
}
