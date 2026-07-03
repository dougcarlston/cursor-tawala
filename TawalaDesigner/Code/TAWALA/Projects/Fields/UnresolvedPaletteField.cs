// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.Projects.Fields;

namespace Tawala.Projects
{
    [Serializable]
    public class UnresolvedPaletteField : PaletteField
    {
        private readonly string fullFieldName = "";

        public UnresolvedPaletteField(string fullFieldName)
        {
            this.fullFieldName = fullFieldName;
        }

        public override string QualifiedFieldName { get { return fullFieldName; } }

        public override string FieldName { get { return FieldUtil.GetFieldName(fullFieldName); } }

        public override IField this[string name] { get { return this; } }
    }
}