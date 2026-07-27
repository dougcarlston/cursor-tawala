// Copyright © 2005 - 2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Tawala.Common
{
	public class Log
	{
		private const string xmlLogStart =
			"<log>" +
				"<events>";

		private const string xmlEntryStart =
			"<event level=\"{0}\" ns=\"{1}\" method=\"{2}.{3}\" sec=\"{4}\" assembly=\"{5} {6}\" domain=\"{7}\" thread=\"{8}\" >";

		private const string xmlData =
			"<![CDATA[{0}]]>";

		private const string xmlEntryEnd =
			"</event>";

		private const string xmlLogEnd =
				"</events>" +
			"</log>";

		private static volatile bool closed = true;
		private static int bufferLength = 0;

		private static DateTime startTime;

		public static void Open()
		{
			try
			{
				bufferLength = xmlEntryStart.Length + xmlData.Length + xmlEntryEnd.Length + 64;

				startTime = DateTime.Now;
				string path = Path.Combine(Config.LocalApplicationData, "Log");
				string YMD = startTime.ToString("yyyyMMdd");

				if (!Directory.Exists(path))
				{
					Directory.CreateDirectory(path);
				}

				path = Path.Combine(path, YMD);
				if (!Directory.Exists(path))
				{
					Directory.CreateDirectory(path);
				}

				path = Path.Combine(path, string.Format("DESIGNER_{0}.xml", startTime.ToString("yyyyMMdd_HHmmss.ff")));
				textWriter = TextWriter.Synchronized(File.CreateText(path));
				textWriter.Write(xmlLogStart);
				Flush();
				closed = false;
			}
			catch (Exception)
			{
				textWriter = null;
				closed = true;
			}
		}

		public static void Close()
		{
				if (!closed)
				{
					textWriter.Write(xmlLogEnd);

					closed = true;

					textWriter.Flush();
					textWriter.Close();
					textWriter = null;
				}
		}

		public static void Flush()
		{
				if (!closed)
				{
					textWriter.Flush();
				}
		}

		public static long LogEnter(string format, params object[] objects)
		{
			if (!closed)
			{
				writeLogEntry("enter", format, objects);
				return DateTime.Now.Ticks;
			}
			return 0;
		}

		public static void LogExit(string format, params object[] objects)
		{
			if (!closed)
			{
				writeLogEntry("exit", format, objects);
			}
		}

		public static void LogExit(long enterTicks)
		{
			if (!closed)
			{
				long diff = DateTime.Now.Ticks - enterTicks;

				writeLogEntry("exit", "[Elapsed seconds = {0}] ", diff != 0 ? diff / 10000000.0 : 0.0);
			}
		}

		public static void LogExit(long enterTicks, string format, params object[] objects)
		{
			if (!closed)
			{
				long diff = DateTime.Now.Ticks - enterTicks;

				string elapsed = string.Format("[Elapsed seconds = {0}] ", diff != 0 ? diff / 10000000.0 : 0.0);
				writeLogEntry("exit", elapsed + format, objects);
			}
		}

		public static void LogInfo(string format, params object[] objects)
		{
			if (!closed)
			{
				writeLogEntry("info", format, objects);
			}
		}

		public static void LogWarning(string format, params object[] objects)
		{
			if (!closed)
			{
				writeLogEntry("warning", format, objects);
			}
		}

		public static void LogError(string format, params object[] objects)
		{
			if (!closed)
			{
				writeLogEntry("error", format, objects);
			}
		}

		public static void LogException(Exception e)
		{
			if (!closed)
			{
				writeLogEntry("exception", "{0}", e);
			}
		}

		private static void writeLogEntry(string level, string format, params object[] objects)
		{
			if (closed)
				return;

			DateTime now = DateTime.Now;

			StackFrame frame = new StackFrame(2);
			MethodBase method = frame.GetMethod();

			if (method == null)
				return;

			string assemblyName = string.Empty;
			string assemblyVer = string.Empty;

			Assembly assembly = method.DeclaringType.Assembly;

			if (assembly != null)
			{
				assemblyName = assembly.GetName().Name;
				assemblyVer = assembly.GetName().Version.ToString();
			}

			StringBuilder sb = new StringBuilder(bufferLength + format.Length);
			sb.AppendFormat(xmlEntryStart,
				level,
				method.DeclaringType.Namespace,
				method.DeclaringType.Name, 
				method.Name,
				(DateTime.Now.Ticks - startTime.Ticks) / 10000000.0,
				assemblyName, 
				assemblyVer, 
				AppDomain.CurrentDomain.Id,
				Thread.CurrentThread.ManagedThreadId);

			if (format.Length > 0)
			{
				sb.AppendFormat(xmlData, string.Format(format, objects));
			}

			sb.AppendFormat(xmlEntryEnd);

			textWriter.Write(sb.ToString());
		}

		private static object syncObject = new object();
		private static TextWriter textWriter = null;
	}
}
