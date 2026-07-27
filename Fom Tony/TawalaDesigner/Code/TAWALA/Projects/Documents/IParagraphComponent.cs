// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections;

namespace Tawala.Projects.Documents
{
    public interface IParagraphComponent : IEnumerable, IRecursiveEnumerable
    {
        string Text { get; }

        string ToXml();

        string ToHtml();

        string ToRtf();
    }
}