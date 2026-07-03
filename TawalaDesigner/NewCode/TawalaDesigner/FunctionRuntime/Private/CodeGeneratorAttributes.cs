// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;

namespace Tawala.Functions.Runtime.Private
{
    // using for searching where attribute instance must match, hence no properties
    [AttributeUsage(AttributeTargets.Property)]
    public class ParameterMarkerAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ParameterPropertyAttribute : Attribute
    {
        private readonly Type composite = typeof(object);
        private readonly Type property = typeof(object);
        private string gid = string.Empty;

        public ParameterPropertyAttribute(string _gid, Type composite, Type property)
        {
            gid = _gid;
            this.composite = composite;
            this.property = property;
        }

        public string Gid { get { return gid; } set { gid = value; } }

        public Type CompositeType { get { return composite; } }

        public Type PropertyType { get { return property; } }
    }

    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class FunctionClassAttribute : Attribute
    {
        private readonly string gid = "0";
        private readonly Type type = typeof(object);

        public FunctionClassAttribute(string _gid, Type _type)
        {
            gid = _gid;
            type = _type;
        }

        public string Gid { get { return gid; } }

        public Type FunctionType { get { return type; } }
    }
}