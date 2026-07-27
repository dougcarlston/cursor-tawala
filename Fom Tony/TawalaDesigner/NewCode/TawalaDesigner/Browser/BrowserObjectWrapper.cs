// Copyright © 2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.Reflection;
using System.Windows.Forms;

//using Tawala.Common;

namespace Tawala.Browser
{
    /// <summary>
    /// Helper for working with an Internet Explorer object that has no .NET Wrapper
    /// Relies on the fact the reflection works with com objects as well.
    /// <summary>
    public class BrowserObjectWrapper
    {
        private const BindingFlags getFlags = BindingFlags.GetProperty | publicInstanceFlags;
        private const BindingFlags invokeFlags = BindingFlags.InvokeMethod | publicInstanceFlags;
        private const BindingFlags publicInstanceFlags = BindingFlags.Instance | BindingFlags.Public;
        private const BindingFlags setFlags = BindingFlags.SetProperty | publicInstanceFlags;
        private readonly object ieObject;
        private readonly Type ieType;

        public BrowserObjectWrapper(object objectToWrap)
        {
            ieObject = objectToWrap;
            ieType = objectToWrap.GetType();
        }

        public T GetProperty<T>(string name) where T : class
        {
            return GetProperty(name) as T;
        }

        public object GetProperty(string name)
        {
            return ieType.InvokeMember(name, getFlags, null, ieObject, null);
        }

        public void SetProperty(string name, params object[] objects)
        {
            ieType.InvokeMember(name, setFlags, null, ieObject, objects);
        }

        public BrowserObjectWrapper GetWrapper(string name)
        {
            return new BrowserObjectWrapper(GetProperty(name));
        }

        public static BrowserObjectWrapper GetWrapper(object ownerObject, string objectName)
        {
            return new BrowserObjectWrapper(ownerObject).GetWrapper(objectName);
        }

        public static BrowserObjectWrapper GetWrapper(HtmlElement element, string objectName)
        {
            return new BrowserObjectWrapper(element.DomElement).GetWrapper(objectName);
        }

        public T InvokeMethod<T>(string name, params object[] objects) where T : class
        {
            return InvokeMethod(name, objects) as T;
        }

        public object InvokeMethod(string name, params object[] objects)
        {
            return ieType.InvokeMember(name, invokeFlags, null, ieObject, objects);
        }
    }
}