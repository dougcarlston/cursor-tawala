// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections;
using Tawala.Projects.Processes;
using Tawala.XmlSupport;

namespace Tawala.Projects.Expressions
{
    [Serializable]
    public class LogicalOperator : IConditionComponent
    {
        private static readonly Hashtable operatorTable = new Hashtable();

        private readonly string operatorString = "";

        static LogicalOperator()
        {
            operatorTable.Add("and", "AND");
            operatorTable.Add("or", "OR");
        }

        public LogicalOperator(string operatorString)
        {
            this.operatorString = operatorString;
        }

        public LogicalOperator(IXmlElement element)
        {
            operatorString = element.Name;
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

        #region IConditionComponent Members

        public bool IsValid()
        {
            return true;
        }

        public override string ToString()
        {
            return (string)operatorTable[operatorString.ToLower()];
        }

        #endregion
    }
}