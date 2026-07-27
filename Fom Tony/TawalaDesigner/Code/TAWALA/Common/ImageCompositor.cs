// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Drawing;

namespace Tawala.Common
{
    public class ImageCompositor
    {
        private readonly Image dest;

        public ImageCompositor(Image imageOnBottom)
        {
            dest = imageOnBottom;
        }

        public Image Render(params Image[] images)
        {
            var result = (Image)(dest.Clone());

            if (images.Length >= 1)
            {
                using (Graphics g = Graphics.FromImage(result))
                {
                    foreach (Image image in images)
                    {
                        g.DrawImageUnscaled(image, 0, 0);
                    }
                }
            }

            return result;
        }
    }
}