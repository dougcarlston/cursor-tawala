// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.ComponentModel;

namespace Tawala.Functions.Runtime
{
    public interface ICompositeParameter : INotifyPropertyChanged, IFunctionParameterXml
    {
        string ToString();
    }
}