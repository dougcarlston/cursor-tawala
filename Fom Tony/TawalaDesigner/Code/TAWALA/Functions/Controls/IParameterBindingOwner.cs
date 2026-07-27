// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Windows.Forms;

namespace Tawala.Functions.Controls
{
    public interface IParameterBindingOwner
    {
        object DataSource { get; }
        string DataMember { get; }
        bool Required { get; }
        void OnFormat(ConvertEventArgs ce);
        void OnParse(ConvertEventArgs ce);
        void OnBindingComplete(BindingCompleteEventArgs bce);
        // only relevant to text at the moment
    }
}