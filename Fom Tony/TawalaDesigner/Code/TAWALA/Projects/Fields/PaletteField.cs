// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections;

namespace Tawala.Projects.Fields
{
    public class PaletteField : IPaletteField
    {
        public static readonly PaletteField NULL = new PaletteField();

        protected PaletteField()
        {
        }

        #region IPaletteField Members

        public virtual string QualifiedFieldName { get { return "Form:" + FieldName; } }

        public virtual string FieldName { get { return "Field"; } }

        public virtual string FieldString { get { return "<<" + FieldName + ">>"; } }

        public virtual IField this[string name] { get { return null; } }

        public int Id
        {
            get
            {
                return Int32.MaxValue;
                ;
            }
        }

        #endregion

        #region IEnumerable Interface

        public IEnumerator GetEnumerator()
        {
            yield return this;
        }

        #endregion

        #region IRecursiveEnumerable Interface

        public IEnumerable RecursiveEnumerator { get { yield return this; } }

        #endregion
    }
}