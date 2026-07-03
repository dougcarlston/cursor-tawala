// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Tawala.Functions.Runtime
{
    public interface ICompositeParameterCollection : IList<ICompositeParameter>, IFunctionParameterValue
    {
        BindingList<ICompositeParameter> CreateBindingList();
        ICompositeParameter CreateItem();
        string ToString();
    }
}