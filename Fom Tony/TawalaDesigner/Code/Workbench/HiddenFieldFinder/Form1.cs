using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace HiddenFieldFinder
{
	public partial class Form1 : Form
	{
		private string pathName = "";
		private XmlDocument xmlDocument;

		public Form1()
		{
			InitializeComponent();

			Application.Idle += new EventHandler(Application_Idle);
			this.Disposed += new EventHandler(Form1_Disposed);
		}

		void Form1_Disposed(object sender, EventArgs e)
		{
			Application.Idle -= new EventHandler(Application_Idle);
		}

		private void Application_Idle(Object sender, EventArgs e)
		{
			buttonFindFields.Enabled = (textBoxFileName.Text != "");
		}

		private void Form1_Activated(object sender, EventArgs e)
		{
			excludedElementNames.Add("set");
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();

			openFileDialog.DefaultExt = "tawala";
			openFileDialog.Filter = "Tawala project files (*.tawala)|*.tawala";

			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				clearTextBoxFields();
				setPathName(openFileDialog.FileName);
			}
		}

		private void setPathName(string pathName)
		{
			this.pathName = pathName;
			textBoxFileName.Text = Path.GetFileName(pathName);
		}

		private void clearTextBoxFields()
		{
			textBoxFields.Lines = new string[] { "" };
		}

		private void buttonFindFields_Click(object sender, EventArgs e)
		{
			openProjectFile();
			showHiddenFields();
		}

		private void openProjectFile()
		{
			xmlDocument = new XmlDocument();
			xmlDocument.Load(pathName);
		}

		private void showHiddenFields()
		{
			Collection<string> hiddenFieldNames = getHiddenFieldNames();
			Collection<string> referencedFieldNames = getReferencedFieldNames();
			Collection<string> referencedHiddenFieldNames = getReferencedHiddenFieldNames(hiddenFieldNames, referencedFieldNames);

			setTextBoxFieldsLines(referencedHiddenFieldNames);
		}

		private Collection<string> getHiddenFieldNames()
		{
			Collection<XmlElement> formElements = getFormElements();
			Collection<string> fieldNames = new Collection<string>();

			foreach (XmlElement formElement in formElements)
			{
				Collection<XmlElement> fieldElements = getFieldElements(formElement);

				foreach (XmlElement fieldElement in fieldElements)
				{
					fieldNames.Add(formElement.GetAttribute("name") + ":" + fieldElement.GetAttribute("name"));
				}
			}

			return fieldNames;
		}

		private Collection<string> getReferencedFieldNames()
		{
			Collection<XmlElement> processElements = getProcessElements();
			Collection<string> referencedFieldNames = new Collection<string>();

			foreach (XmlElement processElement in processElements)
			{
				Collection<XmlElement> referencedFieldElements = getReferencedFieldElements(processElement);

				foreach (XmlElement referencedFieldElement in referencedFieldElements)
				{
					string referencedFieldName = referencedFieldElement.GetAttribute("field");
					referencedFieldName = Regex.Replace(referencedFieldName, @"[^:]+:([^:]+:[^:]+$)", "$1");

					referencedFieldNames.Add(referencedFieldName);
				}
			}

			return referencedFieldNames;
		}

		private static Collection<string> getReferencedHiddenFieldNames(Collection<string> hiddenFieldNames, Collection<string> referencedFieldNames)
		{
			Collection<string> referencedHiddenFieldNames = new Collection<string>();

			foreach (string fieldName in hiddenFieldNames)
			{
				if (referencedFieldNames.Contains(fieldName))
				{
					referencedHiddenFieldNames.Add(fieldName);
				}
			}

			return referencedHiddenFieldNames;
		}

		private void setTextBoxFieldsLines(Collection<string> referencedHiddenFieldNames)
		{
			if (referencedHiddenFieldNames.Count > 0)
			{
				string[] fieldLines = new string[referencedHiddenFieldNames.Count];
				int i = 0;

				foreach (string hiddenFieldName in referencedHiddenFieldNames)
				{
					fieldLines[i++] = hiddenFieldName;
				}

				textBoxFields.Lines = fieldLines;
			}
			else
			{
				textBoxFields.Lines = new string[] { "No hidden fields found" };
			}
		}

		/// <summary>
		/// Returns all the &lt;form&gt; elements that have a connected process
		/// </summary>
		/// <returns></returns>
		private Collection<XmlElement> getFormElements()
		{
			Collection<XmlElement> formElements = new Collection<XmlElement>();

			foreach (XmlElement componentElement in xmlDocument.DocumentElement.ChildNodes)
			{
				if (componentElement.Name == "forms")
				{
					foreach (XmlElement formElement in componentElement)
					{
						if (formElement.Name == "form")
						{
							if (formElement.HasAttribute("process") || formElement.HasAttribute("preProcess"))
							{
								formElements.Add(formElement);
							}
						}
					}
				}
			}

			return formElements;
		}

		/// <summary>
		/// Returns all the &lt;process&gt; elements
		/// </summary>
		/// <returns></returns>
		private Collection<XmlElement> getProcessElements()
		{
			Collection<XmlElement> processElements = new Collection<XmlElement>();

			foreach (XmlElement componentElement in xmlDocument.DocumentElement.ChildNodes)
			{
				if (componentElement.Name == "processes")
				{
					foreach (XmlElement processElement in componentElement)
					{
						if (processElement.Name == "process")
						{
							processElements.Add(processElement);
						}
					}
				}
			}

			return processElements;
		}

		/// <summary>
		/// Returns all the &lt;field&gt; elements in the specified form element
		/// </summary>
		/// <returns></returns>
		private Collection<XmlElement> getFieldElements(XmlElement formElement)
		{
			Collection<XmlElement> fieldElements = new Collection<XmlElement>();
			XmlElement itemsElement = (XmlElement)formElement.FirstChild;

			foreach (XmlElement itemElement in itemsElement.ChildNodes)
			{
				if (itemElement.Name == "field")
				{
					fieldElements.Add(itemElement);
				}
			}

			return fieldElements;
		}

		/// <summary>
		/// Returns all the referenced field elements in the specified process element
		/// </summary>
		/// <returns></returns>
		private Collection<XmlElement> getReferencedFieldElements(XmlElement processElement)
		{
			Collection<XmlElement> referencedFieldElements = new Collection<XmlElement>();

			foreach (XmlElement forEachElement in getForEachElements(processElement))
			{
				foreach (XmlElement setElement in getRelevantFieldElements(forEachElement))
				{
					referencedFieldElements.Add(setElement);
				}
			}

			return referencedFieldElements;
		}

		/// <summary>
		/// This recursive method returns all the &lt;foreach&gt; elements from within the specified container element
		/// </summary>
		/// <returns></returns>
		private Collection<XmlElement> getForEachElements(XmlElement containerElement)
		{
			Collection<XmlElement> forEachElements = new Collection<XmlElement>();

			if (containerElement != null)
			{
				foreach (XmlNode childNode in containerElement.ChildNodes)
				{
					XmlElement childElement = childNode as XmlElement;

					if (childElement != null)
					{
						if (childElement.Name == "foreach")
						{
							forEachElements.Add(childElement);
						}
					}

					foreach (XmlElement containedElement in getForEachElements(childElement))
					{
						forEachElements.Add(containedElement);
					}
				}
			}

			return forEachElements;
		}

		private static Collection<string> excludedElementNames = new Collection<string>();

		/// <summary>
		/// This recursive method returns a collection of elements having "field=" attributes from within the specified container element.
		/// </summary>
		private Collection<XmlElement> getRelevantFieldElements(XmlElement containerElement)
		{
			Collection<XmlElement> relevantFieldElements = new Collection<XmlElement>();

			if (containerElement != null)
			{
				foreach (XmlNode node in containerElement)
				{
					XmlElement element = node as XmlElement;

					if (element != null && element is XmlElement)
					{
						foreach (XmlNode childNode in element.ChildNodes)
						{
							XmlElement childElement = childNode as XmlElement;

							if (childElement != null)
							{
								if (!excludedElementNames.Contains(childElement.Name))
								{
									if (childElement.GetAttribute("field").Contains(":"))
									{
										if (childElement.Name == "string")
										{
											Console.WriteLine(childElement.GetAttribute("field"));
										}

										relevantFieldElements.Add(childElement);
									}
								}
							}
						}

						foreach (XmlElement containedElement in getRelevantFieldElements(element))
						{
							relevantFieldElements.Add(containedElement);
						}
					}
				}
			}

			return relevantFieldElements;
		}

	}
}