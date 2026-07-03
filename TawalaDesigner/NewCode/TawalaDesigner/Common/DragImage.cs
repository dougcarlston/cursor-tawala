// $Workfile: DragImage.cs $
// $Revision: 6 $	$Date: 12/21/05 1:02p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Tawala.Common
{
	static public class DragImage
	{
		private static Control window = null;
		private static bool bEntered = false;

		public static bool Begin(Control window, Image image, int dxHotSpot, int dyHotSpot)
		{
			if (DragImage.window != null)
				End();

			DragImage.window = window;

			// Make sure the window and its children finish up any pending updates first so we 
			// avoid potential for drawing artifacts.
			window.Update();

			imageList.Images.Clear();
			imageList.ColorDepth = ColorDepth.Depth32Bit;
			imageList.ImageSize = new Size(image.Width, image.Height);
			imageList.Images.Add(image);

			ImageList_BeginDrag(imageList.Handle, 0, dxHotSpot, dyHotSpot);

			// doing this prevents the image from being semi-transparent -- it combines the image
			// with the image that ImageList_BeginDrag created.
			return ImageList_SetDragCursorImage(imageList.Handle, 0, dxHotSpot, dyHotSpot);
		}

		public static void End()
		{
			ImageList_EndDrag();
			window = null;
			bEntered = false;
		}

		public static bool Move(int x, int y)
		{
			return ImageList_DragMove(x, y);
		}

		public static bool Enter()
		{
			if (window != null && !bEntered)
			{
				bEntered = true;
				Point pt = window.PointToClient(Cursor.Position);
				return ImageList_DragEnter(window.Handle, pt.X, pt.Y);
			}
			return false;
		}

		public static bool Leave()
		{
			if (window != null && bEntered)
			{
				bEntered = false;
				return ImageList_DragLeave(window.Handle);
			}
			return false;
		}

//		public static bool Show(bool show)
//		{
//			return active ? ImageList_DragShowNolock(show) : false;
//		}

		private static ImageList imageList = new ImageList();
	
		// Win32 interop calls to ImageList functions not available via .NET ImageList class
		// These are private, use the public wrapper methods above

		[DllImport("comctl32.dll")]
		private static extern bool ImageList_BeginDrag(IntPtr handle, int index, int dxHotSpot, int dyHotSpot);
		[DllImport("comctl32.dll")]
		private static extern bool ImageList_DragMove(int x, int y);
		[DllImport("comctl32.dll")]
		private static extern void ImageList_EndDrag();
		[DllImport("comctl32.dll")]
		private static extern bool ImageList_DragEnter(IntPtr hwnd, int x, int y);
		[DllImport("comctl32.dll")]
		private static extern bool ImageList_DragLeave(IntPtr hwnd);
		[DllImport("comctl32.dll")]
		private static extern bool ImageList_DragShowNolock(bool show);
		[DllImport("comctl32.dll")]
		private static extern bool ImageList_SetDragCursorImage(IntPtr handle, int index, int dxHotspot, int dyHotspot);

	}
}
