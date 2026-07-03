using System;
using System.Collections.Generic;
using System.Text;

namespace TawalaTest.TestingSupport
{
	public static class XmlConstants
	{
		public static string DefaultTabsXml
		{
			get { return defaultTabsXml; }
		}

		public static string BeginFont
		{
			get { return beginFont; }
		}

		public static string FullBeginFont
		{
			get { return fullBeginFont; }
		}

		public static string FaceColorBeginFont
		{
			get { return faceColorBeginFont; }
		} 

		public static string DefaultBeginFont
		{
			get { return defaultBeginFont; }
		} 

		public static string EndFont
		{
			get { return endFont; }
		}

		public static string ComponentRepository
		{
            get { return Properties.Resources.display_component_repository; }
		}

		#region Private

		private const string defaultTabsXml =
			"<tabPositions>" +
			"<tabStop position=\"2880\"/>" +
			//"<tabStop position=\"720\"/><tabStop position=\"1440\"/><tabStop position=\"2160\"/>" +
			//"<tabStop position=\"2880\"/><tabStop position=\"3600\"/><tabStop position=\"4320\"/>" +
			//"<tabStop position=\"5040\"/><tabStop position=\"5760\"/><tabStop position=\"6480\"/>" +
			//"<tabStop position=\"7200\"/><tabStop position=\"7920\"/><tabStop position=\"8640\"/>" +
			//"<tabStop position=\"9360\"/><tabStop position=\"10080\"/>" +
			"</tabPositions>";

		private const string fullBeginFont = "<font face=\"Arial\" size=\"200\" color=\"000000\">";
		private const string beginFont = "<font face=\"Arial\">";
		private const string faceColorBeginFont = "<font face=\"Arial\" color=\"000000\">";

		private const string defaultBeginFont = "<font>";

		private const string endFont = "</font>";

		#endregion
	}
}
