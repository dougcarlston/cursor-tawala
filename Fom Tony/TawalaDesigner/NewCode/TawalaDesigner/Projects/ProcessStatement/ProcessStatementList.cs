// $Workfile: ProcessStatementList.cs $
// $Revision: 6 $	$Date: 7/04/07 7:06p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Collections;
using System.Text;
using Tawala.XmlSupport;

namespace Tawala.Projects
{
	[Serializable]
	public class ProcessStatementList : Collection<IProcessStatement>, IProcessStatement
	{
		protected static Factory<IProcessStatement> processStatementFactory = new Factory<IProcessStatement>();

		static ProcessStatementList()
		{
			processStatementFactory.Register("addTo", typeof(AddStatement));
			processStatementFactory.Register("append", typeof(AppendStatement));
			processStatementFactory.Register("comment", typeof(CommentStatement));
			processStatementFactory.Register("delete", typeof(DeleteStatement));
			processStatementFactory.Register("divideBy", typeof(DivideStatement));
			processStatementFactory.Register("foreach", typeof(ForEachRecordStatement));
			#region ForEachQuestionStatement unused
#if false
			processStatementFactory.Register("forEachMc", typeof(ForEachQuestionStatement));
#endif
			#endregion
			processStatementFactory.Register("get", typeof(GetStatement));
			processStatementFactory.Register("if", typeof(IfStatement));
			processStatementFactory.Register("multiplyBy", typeof(MultiplyStatement));
			processStatementFactory.Register("send", typeof(SendStatement));
			processStatementFactory.Register("set", typeof(SetStatement));
			processStatementFactory.Register("edit", typeof(ShowRecordStatement));
			processStatementFactory.Register("show", "document", typeof(ShowDocumentStatement));
			processStatementFactory.Register("show", "form", typeof(ShowFormStatement));
			processStatementFactory.Register("show", typeof(ShowUrlStatement));
			processStatementFactory.Register("skip", typeof(SkipToStatement));
			processStatementFactory.Register("subtractFrom", typeof(SubtractStatement));
		}

		public ProcessStatementList()
		{
		}

		public ProcessStatementList(IXmlElement element, string processName) : this(element, Project.Current.GetProcess(processName))
		{
		}

		public ProcessStatementList(IXmlElement element, Process process)
		{
			int i = 0;

			while (element.GetChild(i) != XmlElement.NULL)
			{
				Add(processStatementFactory.MakeObject(element.GetChild(i++), process));
			}
		}

		public string ToXml()
		{
			StringBuilder xmlString = new StringBuilder();

			foreach (ProcessStatement statement in this.Items)
			{
				xmlString.Append(statement.ToXml());
			}

			return xmlString.ToString();
		}
	}
}
