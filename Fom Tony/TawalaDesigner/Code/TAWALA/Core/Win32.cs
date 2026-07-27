// Copyright © 2005 - 2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;

namespace Tawala
{
	/// <summary>
	/// Win32 Interop
	/// </summary>
	public static class Win32
	{
		public const int WM_SETFOCUS = 0x0007;
		public const int WM_KILLFOCUS = 0x0008;
		public const int WM_PAINT = 0x000F;
		public const int WM_ERASEBKGND = 0x0014;
		public const int WM_KEYDOWN = 0x100;
		public const int WM_KEYUP = 0x101;
		public const int WM_VSCROLL = 0x115;
		public const int WM_COMMAND = 0x0111;
		public const int WM_HOTKEY = 0x0312;
        public const int WM_NCHITTEST = 0x0084;

		// Application reserved message range
		public const int WM_APP = 0x8000;

		// ListBox message constants
		public const int LB_SETCARETINDEX = 0x19E;
		public const int LB_GETTOPINDEX = 0x018E;
		public const int LB_SETTOPINDEX = 0x197;

		public const int LB_SETCURSEL = 0x0186;

		public const int WM_NOTIFY = 0x004E;
		public const int WM_REFLECTNOTIFY = 0x204E;

//		private const int WM_MOUSEFIRST =                  0x0200;
//		private const int WM_MOUSEMOVE    =                0x0200;
		private const int WM_LBUTTONDOWN =                 0x0201;
//		private const int WM_LBUTTONUP =                   0x0202;
//		private const int WM_LBUTTONDBLCLK =               0x0203;
//		private const int WM_RBUTTONDOWN =                  0x0204;
//		private const int WM_RBUTTONUP =                    0x0205;
//		private const int WM_RBUTTONDBLCLK =               0x0206;
//		private const int WM_MBUTTONDOWN =                  0x0207;
//		private const int WM_MBUTTONUP =                   0x0208;
		private const int WM_MBUTTONDBLCLK  =              0x0209;
//		private const int WM_MOUSEWHEEL =                   0x020A;
//		private const int WM_XBUTTONDOWN  =                0x020B;
//		private const int WM_XBUTTONUP =                   0x020C;
//		private const int WM_XBUTTONDBLCLK   =             0x020D;
//		private const int WM_MOUSELAST  =                  0x020D;

		private const int WM_NCBUTTONFIRST = 0x00A1;
		private const int WM_NCBUTTONLAST = 0x00AD;


		// Virtual Keys

		public const int VK_BACK = 0x08;
		public const int VK_RETURN = 0xD;
		public const int VK_DELETE = 0x2E;
		public const int VK_END   =        0x23;
		public const int VK_HOME  =        0x24;
		public const int VK_LEFT  =        0x25;
		public const int VK_UP    =        0x26;
		public const int VK_RIGHT =        0x27;
		public const int VK_DOWN = 0x28;	

		// Helpers

		public static bool IsClientButtonClick(ref Message m)
		{
			return m.Msg >= WM_LBUTTONDOWN && m.Msg <= WM_MBUTTONDBLCLK;
		}

		public static bool IsNonClientButtonClick(ref Message m)
		{
			return m.Msg >= WM_NCBUTTONFIRST && m.Msg <= WM_NCBUTTONLAST;
		}

		public static bool IsButtonClick(ref Message m)
		{
			return IsClientButtonClick(ref m) || IsNonClientButtonClick(ref m);
		}

		public static void ListBox_SetCaretIndex(ListBox lb, int index)
		{
			SendMessage(lb.Handle, LB_SETCARETINDEX, index, IntPtr.Zero);
		}

		public static void ListBox_SetCurSel(ListBox lb, int index)
		{
			SendMessage(lb.Handle, LB_SETCURSEL, index, IntPtr.Zero);
		}

		public static int ListBox_GetTopIndex(ListBox lb)
		{
			return SendMessage(lb.Handle, LB_GETTOPINDEX, 0, IntPtr.Zero);
		}

		public static void ListBox_SetTopIndex(ListBox lb, int index)
		{
			SendMessage(lb.Handle, LB_SETTOPINDEX, index, IntPtr.Zero);
		}

		public static bool ActivateProcessWindow(Process p)
		{
			try
			{
				p.Refresh();
				ShowWindowAsync(p.MainWindowHandle, WS_SHOWNORMAL);
				SetForegroundWindow(p.MainWindowHandle);
			}
			catch (Exception)
			{
				return false;
			}

			return true;
		}

		public static bool IsDescendant(IntPtr parent, IntPtr child)
		{
			return IsChild(parent, child);
		}

		// Interop functions

		// Use Win32 SendMessage to get functionality not currently exposed in .NET
		[DllImport("user32.dll")]
		private static extern Int32 SendMessage(IntPtr hWnd, Int32 msg, Int32 wParam, IntPtr lParam);

		[DllImport("User32.dll")]
		private static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);
		private const int WS_SHOWNORMAL = 0x01;

		[DllImport("User32.dll")]
		private static extern bool SetForegroundWindow(IntPtr hWnd);

		[DllImport("User32.dll")]
		private static extern bool RegisterHotKey(IntPtr hWnd, Int32 id, UInt32 modifiers, UInt32 vk);

		[DllImport("User32.dll")]
		private static extern bool UnregisterHotKey(IntPtr hWnd, Int32 id);

		[DllImport("User32.dll")]
		private static extern bool IsChild(IntPtr hWndParent, IntPtr hWndChild);

	}
}
