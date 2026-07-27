namespace TawalaTest.TestSupport
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

		public static string FunctionRepositoryXml
		{
			get { return Properties.Resources.display_component_repository; }
		}

		public static string DefaultTextItemStyleAttribute
		{
			get { return defaultTextItemStyleAttribute; }
		}

		public static string DefaultMcqItemStyleAttribute
		{
			get { return defaultMcqItemStyleAttribute; }
		}

		#region Private

		private const string defaultTabsXml =
			"<tabPositions>" +
			"<tabStop position=\"2880\"/>" +
			"</tabPositions>";

		private const string fullBeginFont = "<font face=\"Arial\" size=\"200\" color=\"000000\">";
		private const string beginFont = "<font face=\"Arial\">";
		private const string faceColorBeginFont = "<font face=\"Arial\" color=\"000000\">";

		private const string defaultBeginFont = "<font>";

		private const string endFont = "</font>";

		private const string defaultTextItemStyleAttribute = " style=\"normal\"";

		private const string defaultMcqItemStyleAttribute = " style=\"vertical\"";

		#endregion
	}
}
