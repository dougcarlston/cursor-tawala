// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;

namespace Tawala.Functions.Runtime.CodeGen
{
    public interface IGenerator
    {
        CompilationInfo Build(FunctionLoader.BuildInfo buildInfo);
    }
}