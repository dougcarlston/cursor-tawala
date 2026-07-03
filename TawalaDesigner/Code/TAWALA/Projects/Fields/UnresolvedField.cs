// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.Projects.Fields;

namespace Tawala.Projects
{
    [Serializable]
    public class UnresolvedField : Field, IDeserializedField
    {
        public UnresolvedField(string name)
        {
            this.name = name;
        }

        #region IDeserializedField Members

        public override IField this[string name] { get { return this; } }

        public IDeserializedField DeserializedFieldReference { get { return this; } }

        #endregion
    }
}