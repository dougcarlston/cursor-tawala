// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.ComponentModel;

namespace Tawala.Functions.Runtime
{
    public interface ICompositeParameter : IPropertyCollection, INotifyPropertyChanged, IFunctionParameterValue
    {
        string ToString();
    }
}