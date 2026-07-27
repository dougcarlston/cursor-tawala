using System;
using System.Collections.Generic;
using System.Text;

namespace Tawala.Dialogs
{
    public interface IInsertTableView
    {
        int TableWidthInPoints { get; }
        int Columns { get; }
        int Rows { get; }
    }
}
