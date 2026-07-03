// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Windows.Forms;
using Tawala.Projects;

namespace Tawala.Functions.Controls
{
    public interface IPaletteFieldAccepted 
    {
        bool IsAcceptedData(IPaletteField field);
        void AcceptData(IPaletteField field);
        AcceptDataActions AcceptActions { get; }
    }
}