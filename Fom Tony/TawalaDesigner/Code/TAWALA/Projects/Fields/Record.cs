// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.Projects.Fields;

namespace Tawala.Projects
{
    [Serializable]
    public class Record : Variable
    {
		public Record(string name) : this(name, true)
		{
		}

		public Record(string name, bool addToFieldMap): base(name, addToFieldMap)
		{
		}

        public override string QualifiedFieldName { get { return FieldName; } }
    }
}