// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;

namespace Tawala.Functions.Runtime
{
    /// <summary>Describes and manipulates a specific parameter (property) on a function (class)</summary>
    public interface IParameterInfo : IXPathNavigatorProvider
    {
        IParameterInfo Parent { get; }

        /// <summary> The IParameterInfos that describe a composite's properties.
        /// A composite is exposed as a property on the function -- either simply the composite or a collection of composite.
        /// The contents of this collection are the same in either case<summary>
        IParameterInfoCollection Parameters { get; }

        string Id { get; }

        string Name { get; }

        string Description { get; }

        bool Required { get; }

        /// <summary> Links this IParameterInfo back to it's related IFunctionInfo</summary>
        IFunctionInfo FunctionInfo { get; }

        /// <summary>The value of the type attribute in the corresponding repository xml</summary>
        IFunctionParameterTypeInfo MapInfo { get; }

        /// <summary>Name of parameter (property) on function (class)</summary>
        string PropertyName { get; }

        /// <summary>The parameter's .NET Type object</summary>
        Type PropertyType { get; }

        ParameterRestrictions Restrictions { get; }

        string RecordListName { get; }

        /// <summary>Set the value of this parameter on an instance of the function class that it is a property of.</summary>
        void SetValue(IFunction instance, object value);

        /// <summary>Get the value of this parameter on an instance of the function class that it is a property of.</summary>
        object GetValue(IFunction instance);

        bool HasValidValue(IFunction instance);
    }
}