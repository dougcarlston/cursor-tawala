using System;
using System.Collections.Generic;
using System.Text;

namespace TawalaDesigner.Dialogs
{
    public interface IStartupTasksView
    {
        void SetTaskDescription(string text);
        void TasksCompleted();
    }
}
