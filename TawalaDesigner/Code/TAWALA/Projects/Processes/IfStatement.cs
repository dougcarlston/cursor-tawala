// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Text;
using Tawala.XmlSupport;

namespace Tawala.Projects.Processes
{
    /// <summary>
    /// Implements an If statement in the Process
    /// </summary>
    [Serializable]
    public sealed class IfStatement : ProcessStatement
    {
        private const string xmlIfStartTag = "<if>\r\n";

        /// <summary>
        /// Boolean indicating whether the IF statement has an Otherwise clause
        /// </summary>
        private bool hasOtherwise;

        public IfStatement()
        {
            Conditions = new Conditions();
            FalseSet = new ProcessStatementList();
            TrueSet = new ProcessStatementList();
            name = "If";
        }

        public IfStatement(Conditions conditions)
        {
            FalseSet = new ProcessStatementList();
            TrueSet = new ProcessStatementList();
            name = "If";
            Conditions = conditions;
            hasOtherwise = false;
        }

        public IfStatement(Conditions conditions, bool hasOtherwise)
        {
            FalseSet = new ProcessStatementList();
            TrueSet = new ProcessStatementList();
            name = "If";
            Conditions = conditions;
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
            Conditions = new Conditions(element.GetChild("conditions"), process);
            hasOtherwise = (element.GetChild("falseSet") != XmlElement.NULL);

            TrueSet = new ProcessStatementList(element.GetChild("trueSet"), process);

            if (hasOtherwise)
            {
                FalseSet = new ProcessStatementList(element.GetChild("falseSet"), process);
            }
        }

        public ProcessStatementList TrueSet { get; private set; }

        public ProcessStatementList FalseSet { get; private set; }

        public Conditions Conditions { get; set; }

        public bool HasOtherwise
        {
            get { return hasOtherwise; }
            set { hasOtherwise = value; }
        }

        /// <summary>
        /// Boolean indicating whether the IF statement is simple or advanced
        /// </summary>
        public bool IsSimple
        {
            get { return (Conditions.Count == 1); }
        }

        /// <summary>
        /// Provide statement in plain text form.
        /// </summary>
        public override string ToString()
        {
            return (name + " " + Conditions);
        }

        public override string ToXml()
        {
            // start with if start tag
            var xmlString = new StringBuilder(xmlIfStartTag);

            // append XML for conditions
            xmlString.Append(Conditions.ToXml());

            return xmlString.ToString();
        }

        public override IProcessElement AsProcessElement()
        {
            // make top line from if statement and conditions
            // make opening parenthesis line

            var list = new ProcessElementList
                       {
                           new IfLine(this),
                           new IfBlockOpenLine(this, "(", "<trueSet>")
                       };

            if (!HasOtherwise)
            {
                // make closing parenthesis line
                list.Add(new IfBlockCloseLine(this, ")", "</trueSet>\r\n</if>"));
            }
            else
            {
                // make closing parenthesis line
                list.Add(new IfBlockCloseLine(this, ")", "</trueSet>"));

                // make otherwise line
                list.Add(new OtherwiseLine(this, "Otherwise"));

                // make opening parenthesis line
                list.Add(new IfBlockOpenLine(this, "(", "<falseSet>"));

                // make closing parenthesis line
                list.Add(new IfBlockCloseLine(this, ")", "</falseSet>\r\n</if>"));
            }

            return list;
        }
    }
}