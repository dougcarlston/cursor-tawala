// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;

namespace Tawala.Projects.Processes
{
    /// <summary>
    /// Implements single "block close" line (such as closing parenthesis)
    /// for display in Process window.
    /// </summary>

	[Serializable]
	public class IfBlockCloseLine : BlockCloseLine
	{
		public IfBlockCloseLine(ProcessStatement statement, string blockCloseText, string xmlString)
			: base(statement, blockCloseText, xmlString)
		{
		}
	}

	[Serializable]
	public class ForEachBlockCloseLine : BlockCloseLine
	{
		public ForEachBlockCloseLine(ProcessStatement statement, string blockCloseText, string xmlString)
			: base(statement, blockCloseText, xmlString)
		{
		}
	}

	[Serializable]
    public class BlockCloseLine : ProcessLine
    {
        protected string xmlString;

        public BlockCloseLine(ProcessStatement statement, string blockCloseText, string xmlString) : base(null, blockCloseText)
        {
            Group = statement;
            SelectsGroup = false;
            IsDeletable = false;
            IsSelectable = true;
            CanInsertBefore = true;
            this.xmlString = xmlString;
        }

        public override string ToXml()
        {
            return xmlString;
        }
    }
}