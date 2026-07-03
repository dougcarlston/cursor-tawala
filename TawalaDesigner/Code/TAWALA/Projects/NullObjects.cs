// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.Projects.Documents;

namespace Tawala.Projects
{
    public static class NullObjects
    {
        public static readonly IDocument Document = new NullDocument();
        public static readonly IForm Form = new NullForm();

        #region Nested type: NullDocument

        [Serializable]
        private class NullDocument : Document
        {
            public NullDocument() : base("Null Document")
            {
            }
        }

        #endregion

        #region Nested type: NullForm

        [Serializable]
        private class NullForm : Form
        {
            public NullForm() : base("Null Form")
            {
            }
        }

        #endregion
    }
}