// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;

namespace Tawala.Functions.Runtime
{
    [Flags]
    public enum ParameterRestrictions
    {
        Default = 0x0,
        RecordIterationAlways = 0x01,
        RecordIterationNever = 0x02
    } ;
}