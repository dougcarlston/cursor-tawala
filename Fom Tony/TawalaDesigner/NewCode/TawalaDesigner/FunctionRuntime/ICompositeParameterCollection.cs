// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Tawala.Functions.Runtime
{
    public interface ICompositeParameterCollection : IList<ICompositeParameter>, IFunctionParameterXml
    {
        ICompositeParameter CreateItem();
        IBindingList CreateBindingList();
        string ToString();
    }
}