using System;
using System.Collections.Generic;
using System.Text;

using Tawala.Projects;

namespace Tawala.Dialogs
{
    public interface IHyperlinkPresenter
    {
        IHyperlinkView View { get; }
        Hyperlink Hyperlink { get; }

        void ApplyChanges();
    }
}
