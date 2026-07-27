using System;
using System.Collections.Generic;
using System.Text;

using Tawala.Projects;

namespace Tawala.Dialogs
{
    public class LinkEditCompleteEventArgs : EventArgs
    {
        public LinkEditCompleteEventArgs(ILink link, bool canceled)
        {
            Link = link;
            Canceled = canceled;
        }

        public ILink Link
        {
            get;
            private set;
        }

        public bool Canceled
        {
            get;
            private set;
        }
    }
}
