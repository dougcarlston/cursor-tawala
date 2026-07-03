// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;

namespace Tawala.Functions.Runtime.Private
{
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

        public string Gid
        {
            get
            {
                return gid;
            }
            set
            {
                gid = value;
            }
        }

        public Type CompositeType
        {
            get
            {
                return composite;
            }
        }

        public Type PropertyType
        {
            get
            {
                return property;
            }
        }
    }
}