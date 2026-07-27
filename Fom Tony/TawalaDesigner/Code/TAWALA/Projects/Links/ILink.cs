// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;

namespace Tawala.Projects.Links
{
    public interface ILink
    {
        int Id { get; }

        string DisplayText { get; set; }
        string DesignerDisplayText { get; }

        string ToXml();
        string ToRtf();
    }
}