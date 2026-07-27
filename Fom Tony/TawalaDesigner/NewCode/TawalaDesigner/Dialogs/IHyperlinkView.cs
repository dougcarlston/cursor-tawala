using System;
using System.Collections.Generic;
using System.Text;

using Tawala.Projects;

namespace Tawala.Dialogs
{
    public interface IHyperlinkView
    {
        string DisplayText { get; set; }
        string Url { get; set; }
        bool NewWindow { get; set; }

        event EventHandler HandleDestroyed;
    }
}
