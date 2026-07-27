// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;

namespace Tawala.Projects.Images
{
    public interface IImageDefinition
    {
        string PathName { get; }
        string Id { get; }
        string ToXml();
    }
}