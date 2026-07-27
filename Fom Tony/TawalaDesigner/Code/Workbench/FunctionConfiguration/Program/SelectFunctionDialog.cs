using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Tawala.FunctionConfiguration;
using Tawala.Common;
using Tawala.XmlSupport;

namespace Program
{
	public partial class SelectFunctionDialog : Form
	{
		private IConfigureFunctionPresenter presenter;
		private IXmlElement repositoryElement;
		private IXmlElement functionSpecificationElement;

		public SelectFunctionDialog()
		{
			InitializeComponent();

			showXmlVersion();
			showFunctionCategories();
			selectFirstCategory();
		}

		public IXmlElement FunctionSpecificationElement
		{
			get { return functionSpecificationElement; }
		}

		private void showXmlVersion()
		{
			repositoryElement = new XmlElement(RepositoryXmlRetriever.RepositoryXml);
			labelRepositoryXmlVersion.Text = "Repository XML Signature = " + repositoryElement.GetAttribute("signature");
		}

		private void showFunctionCategories()
		{
			Collection<XmlElement> displayComponentCategoryElements = repositoryElement.GetChild("tr:display-component-categories").GetChildren("tr:category");

			foreach (IXmlElement element in displayComponentCategoryElements)
			{
				listBoxCategories.Items.Add(new FunctionCategory(element));
			}

			Collection<XmlElement> functionCategoryElements = repositoryElement.GetChild("tr:function-categories").GetChildren("tr:category");

			foreach (IXmlElement element in functionCategoryElements)
			{
				listBoxCategories.Items.Add(new FunctionCategory(element));
			}
		}

		private void selectFirstCategory()
		{
			listBoxCategories.SelectedIndex = 0;
		}

		private void showFunctions()
		{
			listBoxFunctions.Items.Clear();

			FunctionCategory functionCategory = (FunctionCategory)listBoxCategories.SelectedItem;

			Collection<XmlElement> displayComponentElements = repositoryElement.GetDescendants("tr:display-component");

			foreach (IXmlElement element in displayComponentElements)
			{
				if (functionCategory.FunctionIds.Contains(element.GetAttribute("id")))
				{
					listBoxFunctions.Items.Add(new Function(element));
				}
			}

			Collection<XmlElement> functionElements = repositoryElement.GetDescendants("tr:function");

			foreach (IXmlElement element in functionElements)
			{
				if (functionCategory.FunctionIds.Contains(element.GetAttribute("id")))
				{
					listBoxFunctions.Items.Add(new Function(element));
				}
			}
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			//if (functionSpecificationElement.GetAttribute("name") == "ITEMIZATION TABLE")
			//{
			//    ItemizationTableParameterFlattener flattener = new ItemizationTableParameterFlattener(functionSpecificationElement);
			//    functionSpecificationElement = new XmlElement(flattener.ToXml());
			//}

			presenter = new ConfigureFunctionPresenter(new ConfigureFunctionDialogPhase2(), functionSpecificationElement);

			presenter.ConfigureFunction();
		}

		private void listBoxCategories_SelectedIndexChanged(object sender, EventArgs e)
		{
			showFunctions();
		}

		private void listBoxFunctions_SelectedIndexChanged(object sender, EventArgs e)
		{
			functionSpecificationElement = ((Function)listBoxFunctions.SelectedItem).Element;
		}

		private class FunctionCategory
		{
			private IXmlElement element;
			private string name;
			private Collection<string> functionIds;

			/// <summary>
			/// Constructs a FunctionCategory object from a &lt;tr:category&gt; XML element.
			/// </summary>
			public FunctionCategory(IXmlElement element)
			{
				this.element = element;
				this.name = element.GetAttribute("name");
				this.functionIds = getFunctionIds();
			}

			private Collection<string> getFunctionIds()
			{
				Collection<string> ids = new Collection<string>();

				Collection<XmlElement> idElements = element.GetChildren("tr:element-id");

				foreach (IXmlElement idElement in idElements)
				{
					ids.Add(idElement.Text);
				}

				return ids;
			}

			public string Name
			{
				get { return name; }
			}

			public Collection<string> FunctionIds
			{
				get { return functionIds; }
			}

			public override string ToString()
			{
				return name;
			}
		}

		private class Function
		{
			private IXmlElement element;
			private string id;
			private string name;
			private string version;

			/// <summary>
			/// Constructs a Function object from a &lt;tr:display-component&gt; or &lt;tr:function&gt; XML element.
			/// </summary>
			public Function(IXmlElement element)
			{
				this.element = element;
				this.id = element.GetAttribute("id");
				this.name = element.GetAttribute("name");
				this.version = element.GetAttribute("version");
			}

			public IXmlElement Element
			{
				get { return element; }
			}

			public string Id
			{
				get { return id; }
			}

			public string Name
			{
				get { return name; }
			}

			public string Version
			{
				get { return version; }
			}

			public override string ToString()
			{
				return name;
			}
		}

	}

	//public class ItemizationTableParameterFlattener
	//{
	//    private int numberOfColumns;
	//    private IXmlElement contentsElement;
	//    private IXmlElement headerElement;

	//    /// <summary>
	//    /// Constructs an ItemizationTableParameterFlattener object from a &lt;component-repository&gt; XML element.
	//    /// </summary>
	//    public ItemizationTableParameterFlattener(IXmlElement element)
	//    {
	//        this.numberOfColumns = getNumberOfColumns(element);
	//        this.contentsElement = getContentsElement(element);
	//        this.headerElement = getHeaderElement(element);
	//    }

	//    private int getNumberOfColumns(IXmlElement element)
	//    {
	//        return Convert.ToInt32(element.GetDescendants("tr:maximum")[0].GetAttribute("value"));
	//    }

	//    /// <summary>
	//    /// Extracts a &lt;contents&gt; XML element from the specified XML element
	//    /// </summary>
	//    private IXmlElement getContentsElement(IXmlElement element)
	//    {
	//        Collection<XmlElement> parameterElements = element.GetDescendants("tr:parameter");

	//        foreach (IXmlElement parameterElement in parameterElements)
	//        {
	//            if (parameterElement.GetAttribute("id") == "contents")
	//            {
	//                return parameterElement;
	//            }
	//        }

	//        return null;
	//    }

	//    /// <summary>
	//    /// Extracts a &lt;header&gt; XML element from the specified XML element
	//    /// </summary>
	//    private IXmlElement getHeaderElement(IXmlElement element)
	//    {
	//        Collection<XmlElement> parameterElements = element.GetDescendants("tr:parameter");

	//        foreach (IXmlElement parameterElement in parameterElements)
	//        {
	//            if (parameterElement.GetAttribute("id") == "header")
	//            {
	//                return parameterElement;
	//            }
	//        }

	//        return null;
	//    }

	//    public string ToXml()
	//    {
	//        StringBuilder sb = new StringBuilder();

	//        string id = "itemization-table";
	//        string name = "ITEMIZATION TABLE";
	//        string version = "1";

	//        sb.AppendFormat("<tr:display-component id=\"{0}\" name=\"{1}\" version=\"{2}\" xmlns:tr=\"http://www.tawala.com/component-repository\">", id, name, version);

	//        for (int i = 0; i < numberOfColumns; i++)
	//        {
	//            sb.Append(contentsElement.OuterXml);
	//            sb.Append(headerElement.OuterXml);
	//            sb.Replace("$n$", String.Format("{0}", i + 1));
	//        }

	//        sb.Append("</tr:display-component>");

	//        return sb.ToString();
	//    }
	//}
}