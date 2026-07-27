using System;
using System.Collections.Generic;
using System.Text;

namespace Tawala.Functions.Controls
{
    [Flags]
    public enum AcceptDataActions
    {
        None = 0x0,
        NextControl = 0x01,
        Focus = 0x04,
        NoSelection = 0x08
    }
}
