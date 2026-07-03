using System;

namespace Tawala.ProjectUI
{
    [Flags]
    public enum FieldsPaletteStatusChange
    {
        BeginNodeDrag = 1,
        EndNodeDrag = 2
    } ;

    public class FieldsPaletteStatusEventArgs : EventArgs
    {
        public FieldsPaletteStatusEventArgs(FieldsPaletteStatusChange changeFlags)
        {
            Status = changeFlags;
        }

        public FieldsPaletteStatusChange Status { get; private set; }
    }
}