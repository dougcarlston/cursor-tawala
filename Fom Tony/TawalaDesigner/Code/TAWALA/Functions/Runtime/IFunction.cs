// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.ComponentModel;

namespace Tawala.Functions.Runtime
{
    /// <summary>Interface for generated function classes.</summary>
    public interface IFunction : IPropertyCollection, INotifyPropertyChanged, IRequiresEditNotification
    {
        /// <summary>Get this instance's associated IFunctionInfo which provide properties like Id, Name, Description, etc</summary>
        IFunctionInfo Info { get; }

        /// <summary>A unique integer for this function object instance</summary>
        int InstanceId { get; }

        /// <summary>Set a parameter (property) value by id.  Alternative is to use IParameterInfo.SetValue</summary>
        void SetValue(string id, object value);

        /// <summary>Get a parameter (property) value by id.  Alternative is to use IParameterInfo.GetValue</summary>
        object GetValue(string id);

        /// <summary>Return an XML representation of the function object and current state.</summary>
        string ToXml();

        bool HasValidParameterValues();

        string ToDisplayString();

        string ToString();
    }
}