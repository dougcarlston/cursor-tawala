// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;

namespace Tawala.Functions.Runtime.CodeGen
{
    internal interface IGenerator
    {
        CompilationInfo Build(FunctionLoader.BuildInfo buildInfo);
    }
}