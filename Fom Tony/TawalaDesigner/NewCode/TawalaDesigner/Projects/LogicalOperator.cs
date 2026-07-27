using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Tawala.XmlSupport;

namespace Tawala.Projects
{
	[Serializable]
	public class LogicalOperator : IConditionComponent
	{
		private static Hashtable operatorTable = new Hashtable();

		static LogicalOperator()
		{
			operatorTable.Add("and", "AND");
			operatorTable.Add("or", "OR");
		}

		private string operatorString = "";
		public LogicalOperator(string operatorString)
		{
			this.operatorString = operatorString;
		}


		public LogicalOperator(IXmlElement element)
		{
			this.operatorString = element.Name;
		}

		public LogicalOperator(IXmlElement element, string processName) : this(element)
		{
		}

		public LogicalOperator(IXmlElement element, Process process) : this(element)
		{
		}

		public LogicalOperator(IXmlElement element, IField fieldResolver) : this(element)
		{
		}

		public override string ToString()
		{
			return (string)operatorTable[operatorString.ToLower()];
		}

	}
}
