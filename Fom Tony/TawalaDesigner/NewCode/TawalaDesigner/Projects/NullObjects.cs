using System;
using System.Collections.Generic;
using System.Text;

using Tawala.Projects.Documents;
using Tawala.Projects.Forms;

namespace Tawala.Projects
{
    public static class NullObjects
    {
        public static readonly IForm Form = new NullForm();
        public static readonly IDocument Document = new NullDocument();

        [Serializable]
        private class NullForm :Form
        {
            public NullForm() : base("Null Form")
            {
            }
        }


        [Serializable]
        private class NullDocument : NewDocument
        {
            public NullDocument()
                : base("Null Document")
            {
            }
        }

    }
}
