using System;
using System.Collections.Generic;
using System.Text;

namespace TawalaTest.TestingSupport
{
#pragma warning disable 414
	///
	/// This class is for testing the ObjectDelegator class
	/// Its tests which are in TestSupportTest need a class in another
	/// assembly that is not visibile
	//
	internal class InternalClassForTesting
	{
		private class PrivateNestedClass
		{
			private double pi = 3.1415926;
			private string message = string.Empty;

			private static PrivateNestedClass staticRef = new PrivateNestedClass();

			internal string Message
			{
				get { return message; }
				set { message = value; }
			}

			internal double PI
			{
				get { return pi; }
				set { pi = value; }
			}

			private string combine(string s, int i)
			{
				return s + i;
			}
		}
	}
}
