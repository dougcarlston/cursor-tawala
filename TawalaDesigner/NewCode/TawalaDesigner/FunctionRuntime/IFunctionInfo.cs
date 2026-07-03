// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;

namespace Tawala.Functions.Runtime
{
    public interface IFunctionInfo : IXPathNavigatorProvider
    {
        /// <summary>Id string from xml</summary>
        string Id { get; }

        /// <summary>Display name from xml</summary>
        string Name { get; }

        string Description { get; }

        /// <summary> Collection of IParameterInfo accessible by int index or string id with []</summary>
        IParameterInfoCollection Parameters { get; }

        string Version { get; }

        /// <summary>The Type object for the generated function class.</summary>
        Type Type { get; }

        /// <summary>Create an instance of the generated function class.</summary>
        IFunction CreateInstance();
    }
}