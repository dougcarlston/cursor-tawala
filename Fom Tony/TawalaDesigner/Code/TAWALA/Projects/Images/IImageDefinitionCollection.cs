// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;

namespace Tawala.Projects.Images
{
    public interface IImageDefinitionCollection
    {
        IImageDefinition this[int index] { get; }
        IImageDefinition this[string id] { get; }
        string Add(string url);
        void Remove(string id);
        string ToXml();
        IImageDefinition GetImageDefinitionByPathName(string pathName);
    }
}