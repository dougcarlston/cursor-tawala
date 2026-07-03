// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;

namespace Tawala.Projects.Expressions
{
    public interface IConditionComponent
    {
        bool IsValid();
        string ToString();
    }
}