using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using Tawala.Projects;

namespace Tawala.Dialogs
{
    public static class LinkEditor
    {
        public static bool IsDialogActive
        {
            get { return linkEditForm != null; }
        }

        public static void NewHyperlink(EventHandler<LinkEditCompleteEventArgs> completed)
        {
            link = new Hyperlink();
            EditLink(link.Id, completed);
        }

        public static void NewInvitation(EventHandler<LinkEditCompleteEventArgs> completed)
        {
            link = new InvitationField();
            EditLink(link.Id, completed);
        }

        public static void EditLink(int id, EventHandler<LinkEditCompleteEventArgs> completed)
        {
            if (!IsDialogActive)
            {
                link = Project.InvitationMapById[id];

                if (link is Hyperlink)
                {
                    linkEditForm = new HyperlinkView(link as Hyperlink);
                }
                else if (link is InvitationField)
                {
                    linkEditForm = new InvitationView(link as InvitationField);
                }

                if (linkEditForm != null)
                {
                    completedCallback = completed;
                    linkEditForm.FormClosed += new FormClosedEventHandler(linkEditForm_FormClosed);
                    System.Windows.Forms.Form appForm = Application.OpenForms[0];
                    linkEditForm.Left = appForm.Left + (appForm.Width / 2 - linkEditForm.Width / 2);
                    linkEditForm.Top = appForm.Top + (appForm.Height / 2 - linkEditForm.Height / 2);
                    linkEditForm.Show(appForm);
                }
            }
        }

        private static void linkEditForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            completedCallback(null, new LinkEditCompleteEventArgs(link, linkEditForm.DialogResult == DialogResult.Cancel));
            cleanup();
        }

        private static void cleanup()
        {
            link = null;
            linkEditForm = null;
            completedCallback = null;
        }

        private static ILink link;
        private static System.Windows.Forms.Form linkEditForm;
        private static EventHandler<LinkEditCompleteEventArgs> completedCallback;
    }
}
