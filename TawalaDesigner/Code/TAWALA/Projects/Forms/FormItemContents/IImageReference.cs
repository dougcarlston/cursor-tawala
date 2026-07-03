// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;

namespace Tawala.Projects.Forms.FormItemContents
{
    public interface IImageReference
    {
        string Url { get; }
        string PathName { get; }
        string Id { get; set; }
        string ImageFormat { get; }
    }
}