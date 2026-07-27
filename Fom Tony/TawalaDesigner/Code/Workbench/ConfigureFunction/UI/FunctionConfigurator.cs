using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Tawala.XmlSupport;

namespace UI
{
	public partial class FunctionConfigurator : UserControl
	{
		private object functionObject;
		private IXmlElement element;

		public FunctionConfigurator()
		{
			InitializeComponent();
		}

		public FunctionConfigurator(Object functionObject) : this()
		{
			this.functionObject = functionObject;
		}

		public FunctionConfigurator(IXmlElement element) : this()
		{
			this.element = element;
		}

		public FunctionConfigurator(string xmlString) : this(new XmlElement(xmlString))
		{
		}
	}
}
