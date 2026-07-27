// $Workfile: IfStatement.cs $
// $Revision: 27 $	$$
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;
using System.Text;
using Tawala.XmlSupport;

namespace Tawala.Projects
{
	/// <summary>
	/// Implements an If statement in the Process
	/// </summary>
	[Serializable]
	public class IfStatement : ProcessStatement
	{
		protected ProcessStatementList trueSet = new ProcessStatementList();
		public ProcessStatementList TrueSet
		{
			get
			{
				return trueSet;
			}
		}

		protected ProcessStatementList falseSet = new ProcessStatementList();
		public ProcessStatementList FalseSet
		{
			get
			{
				return falseSet;
			}
		}


		static IfStatement()
		{
		}

		public IfStatement()
		{
			name = "If";
		}

		public IfStatement(Conditions conditions)
		{
			name = "If";
			this.conditions = conditions;
			this.hasOtherwise = false;
		}

		public IfStatement(Conditions conditions, bool hasOtherwise)
		{
			name = "If";
			this.conditions = conditions;
			this.hasOtherwise = hasOtherwise;
		}

		/// <summary>
		/// Construct IfStatement from XML "<if>" element.
		/// </summary>
		public IfStatement(IXmlElement element, string processName) : this(element, Project.Current.GetProcess(processName))
		{
		}

		/// <summary>
		/// Construct IfStatement from XML "<if>" element.
		/// </summary>
		public IfStatement(IXmlElement element, Process process) : this()
		{
			this.conditions = new Conditions(element.GetChild("conditions"), process);
			this.hasOtherwise = (element.GetChild("falseSet") != XmlElement.NULL);

			this.trueSet = new ProcessStatementList(element.GetChild("trueSet"), process);

			if (hasOtherwise)
			{
				this.falseSet = new ProcessStatementList(element.GetChild("falseSet"), process);
			}
		}

		/// <summary>
		/// Provide statement in plain text form.
		/// </summary>
		public override string ToString()
		{
			return (name + " " + conditions.ToString());
		}

		private static readonly string xmlIfStartTag = "<if>\r\n";

		public override string ToXml()
		{
			// start with if start tag
			StringBuilder xmlString = new StringBuilder(xmlIfStartTag);

			// append XML for conditions
			xmlString.Append(conditions.ToXml());

			return xmlString.ToString();
		}

		/// <summary>
		/// Single condition
		/// </summary>
		protected Conditions conditions = new Conditions();

		public Conditions Conditions
		{
			get
			{
				return conditions;
			}
			set
			{
				conditions = value;
			}
		}

		/// <summary>
		/// Boolean indicating whether the IF statement has an Otherwise clause
		/// </summary>
		protected bool hasOtherwise;

		public bool HasOtherwise
		{
			get
			{
				return hasOtherwise;
			}
			set
			{
				hasOtherwise = value;
			}
		}

		/// <summary>
		/// Boolean indicating whether the IF statement is simple or advanced
		/// </summary>
		public bool IsSimple
		{
			get
			{
				return (conditions.Count == 1);
			}
		}

		public override IProcessElement AsProcessElement()
		{
			ProcessElementList list = new ProcessElementList();

			// make top line from if statement and conditions
			list.Add(new IfLine(this));

			// make opening parenthesis line
			list.Add(new BlockOpenLine(this, "(", "<trueSet>"));

			if (!this.HasOtherwise)
			{
				// make closing parenthesis line
				list.Add(new BlockCloseLine(this, ")", "</trueSet>\r\n</if>"));
			}
			else
			{
				// make closing parenthesis line
				list.Add(new BlockCloseLine(this, ")", "</trueSet>"));

				// make otherwise line
				list.Add(new OtherwiseLine(this, "Otherwise"));

				// make opening parenthesis line
				list.Add(new BlockOpenLine(this, "(", "<falseSet>"));

				// make closing parenthesis line
				list.Add(new BlockCloseLine(this, ")", "</falseSet>\r\n</if>"));
			}

			return list;
		}
	}
}
