using System;
using System.Collections.Generic;
using System.Text;

using Tawala.Projects;

namespace Tawala.Dialogs
{
    public class HyperlinkPresenter : IHyperlinkPresenter
    {
        public HyperlinkPresenter(IHyperlinkView view, Hyperlink hyperlink)
        {
            View = view;
            Hyperlink = hyperlink;

            View.HandleDestroyed += view_HandleDestroyed;

            View.Url = Hyperlink.Url;
            View.DisplayText = Hyperlink.DisplayText;
            View.NewWindow = Hyperlink.OpenNewWindow;
        }

        #region IHyperlinkPresenter Members

        public IHyperlinkView View
        {
            get;
            private set;
        }

        public Hyperlink Hyperlink
        {
            get;
            private set;
        }

        public void ApplyChanges()
        {
            Hyperlink.DisplayText = View.DisplayText;
            Hyperlink.Url = View.Url;
            Hyperlink.OpenNewWindow = View.NewWindow;
        }

        #endregion

        private void view_HandleDestroyed(object sender, EventArgs e)
        {
            View.HandleDestroyed -= view_HandleDestroyed;
            View = null;
            Hyperlink = null;
        }

    }
}
