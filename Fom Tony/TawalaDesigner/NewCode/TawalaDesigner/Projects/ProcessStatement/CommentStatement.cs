// $Workfile: CommentStatement.cs $
// $Revision: 3 $	$Date: 9/22/06 5:28p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using Tawala.Common;
using Tawala.XmlSupport;

namespace Tawala.Projects
{
	/// <summary>
	/// Implements a Comment statement in the Process
	/// </summary>
	[Serializable]
	public class CommentStatement : Tawala.Projects.ProcessStatement
	{
		public CommentStatement()
		{
			name = "Comment";
		}

		public CommentStatement(string text) : this()
		{
			this.text = text;
		}

		public CommentStatement(IXmlElement element) : this(element.Text)
		{
		}

		public CommentStatement(IXmlElement element, string processName) : this(element, Project.Current.GetProcess(processName))
		{
		}

		public CommentStatement(IXmlElement element, Process process) : this(element)
		{
		}

		private string text = "";

		public string Text
		{
			get { return text; }
			set { text = value; }
		}

		public override string ToString()
		{
			return text;
		}

		public override string ToXml()
		{
			return string.Format("<comment>{0}</comment>", XMLStringFormatter.EscapeElementText(text));
		}
	}
}
