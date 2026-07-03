// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;

namespace Tawala.Functions.Controls
{
    public interface IDataAccepted : IParameterControl
    {
        bool IsAcceptedData(object o);
        void AcceptData(object o);
    }
}