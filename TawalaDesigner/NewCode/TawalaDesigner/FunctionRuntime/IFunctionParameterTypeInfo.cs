// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;

namespace Tawala.Functions.Runtime
{
    public interface IFunctionParameterTypeInfo
    {
        string XmlType { get; }
        string DataType { get; }
        string ControlType { get; }
        string BindingType { get; }
        string TagLine { get; }
        string Initializer { get; }
    }
}