// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.Windows.Forms;

namespace Tawala.ProjectUI
{
    public static class ImageFileDialog
    {
        public static string Browse(IWin32Window owner)
        {
            var dialog = new OpenFileDialog
                         {
                             Filter = "GIF (*.gif), JPEG (*.jpg), PNG (*.png) | *.gif; *.jpg; *.png"
                         };

            if (dialog.ShowDialog(owner) == DialogResult.OK)
            {
                return dialog.FileName;
            }

            return null;
        }
    }
}