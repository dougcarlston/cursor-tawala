// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.Xml.XPath;

namespace Tawala.Functions.Runtime
{
    public interface IXPathNavigatorProvider
    {
        XPathNavigator Xml { get; }
    }
}