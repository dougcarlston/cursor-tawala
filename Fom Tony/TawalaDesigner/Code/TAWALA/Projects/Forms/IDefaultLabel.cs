// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;

namespace Tawala.Projects.Forms
{
    public interface IDefaultLabel
    {
        string DefaultLabelPrefix { get; }
        string ToXml(string label);
    }
}