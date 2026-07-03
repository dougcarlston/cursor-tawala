// $Workfile: Enums.cs $
// $Revision: 5 $	$Date: 11/10/06 10:02a $
// Copyright © 2005-2006 Tawala Systems, Inc. All rights reserved.
using System;

namespace Tawala.TextEditor
{
    public enum Tristate { False = 0, True = 1, Undefined = 2 };

    public enum HorizontalAlignment { Undefined = 0, Left = 1, Center = 2, Right = 3, Justify = 4 };

    public enum ViewMode { Unlimited = 1, Normal = 2, Page = 3, Simple = 4 };
}
