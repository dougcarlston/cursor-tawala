using System;
using System.Collections.Generic;
using System.Text;

using Tawala.Projects;

namespace Tawala.Dialogs
{
    public class InvitationPresenter : IInvitationPresenter
    {
        public InvitationPresenter(IInvitationView view, InvitationField invitation)
        {
            View = view;
            Invitation = invitation;

            View.HandleDestroyed += view_HandleDestroyed;

            View.InitialFormName = Invitation.InitialFormName;
            View.ProjectName = Invitation.ProjectName;
            View.DisplayText = Invitation.DisplayText;
            View.IsPrivate = Invitation.IsPrivate;
            View.AuthenticationTokenExpression = Invitation.IsPrivate ? Invitation.AuthenticationTokenExpression : null;
        }

        #region IHyperlinkPresenter Members

        public IInvitationView View
        {
            get;
            private set;
        }

        public InvitationField Invitation
        {
            get;
            private set;
        }

        public void ApplyChanges()
        {
            Invitation.InitialFormName = View.InitialFormName;
            Invitation.DisplayText = View.DisplayText;
            Invitation.IsPrivate = View.IsPrivate;

            if (isCurrentProject())
            {
                Invitation.ProjectName = string.Empty;
                Invitation.Form = Project.Current.GetForm(Invitation.InitialFormName);
            }
            else
            {
                Invitation.ProjectName = View.ProjectName;
                Invitation.Form = null;
            }

            if (Invitation.IsPrivate)
            {
                Invitation.AuthenticationTokenExpression = View.AuthenticationTokenExpression;
            }
            else
            {
                Invitation.AuthenticationTokenExpression = null;
            }
        }

        #endregion

        private void view_HandleDestroyed(object sender, EventArgs e)
        {
            View.HandleDestroyed -= view_HandleDestroyed;
            View = null;
            Invitation = null;
        }

        private bool isCurrentProject()
        {
            return View.ProjectName.CompareTo("(Current Project)") == 0;
        }


    }
}
