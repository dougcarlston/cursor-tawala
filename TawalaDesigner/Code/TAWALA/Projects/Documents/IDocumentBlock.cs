// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;

namespace Tawala.Projects.Documents
{
    public interface IDocumentBlock : IDocumentConversions
    {
        string Text { get; }
    }
}