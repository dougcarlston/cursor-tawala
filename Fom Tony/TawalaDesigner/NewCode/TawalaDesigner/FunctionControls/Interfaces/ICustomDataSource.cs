// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;

namespace Tawala.Functions.Controls
{
    public interface ICustomDataSource
    {
        object CustomDataSource { get; set; }

        event EventHandler CustomDataSourceChanged;
    }
}