using System;
using System.Collections.Generic;
using System.Text;

using Tawala.Projects;

namespace Tawala.Dialogs
{
    public interface IInvitationView
    {
        string InitialFormName { get; set; }
        string ProjectName { get; set; }
        string DisplayText { get; set; }
        bool IsPrivate { get; set; }
        CompoundExpression AuthenticationTokenExpression { get; set; }

        event EventHandler HandleDestroyed;
    }
}
