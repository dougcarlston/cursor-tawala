using System;
using System.Collections.Generic;
using System.Text;

using Tawala.Projects;

namespace Tawala.Dialogs
{
    public interface IInvitationPresenter
    {
        IInvitationView View { get; }
        InvitationField Invitation { get; }

        void ApplyChanges();
    }
}
