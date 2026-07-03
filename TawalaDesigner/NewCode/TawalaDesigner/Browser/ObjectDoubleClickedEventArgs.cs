using System;

namespace Tawala.Browser
{
    public class ObjectDoubleClickedEventArgs : EventArgs
    {
        public ObjectDoubleClickedEventArgs(int id)
        {
            Id = id;
        }

        public int Id { get; private set; }
    }
}