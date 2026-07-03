// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;

namespace Tawala.Projects.Processes
{
    /// <summary>
    /// Implements single "block open" line (such as opening parenthesis)
    /// for display in Process window.
    /// </summary>
    
	[Serializable]
	public class IfBlockOpenLine : BlockOpenLine
    {
		public IfBlockOpenLine(ProcessStatement statement, string blockOpenText, string xmlString)
			: base(statement, blockOpenText, xmlString)
        {
        }		
    }

	[Serializable]
	public class ForEachBlockOpenLine : BlockOpenLine
	{
		public ForEachBlockOpenLine(ProcessStatement statement, string blockOpenText, string xmlString)
			: base(statement, blockOpenText, xmlString)
		{
		}
	}

    [Serializable]
    public class BlockOpenLine : ProcessLine
    {
        protected string xmlString;

		public BlockOpenLine(ProcessStatement statement, string blockOpenText, string xmlString)
			: base(null, blockOpenText)
		{
            Group = statement;
            SelectsGroup = false;
            IsDeletable = false;
            IsSelectable = true;
            CanInsertBefore = false;
            this.xmlString = xmlString;
        }

        public override string ToXml()
        {
            return xmlString;
        }
    }
}