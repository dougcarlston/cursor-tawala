// $Workfile: GlobalSettingsTest.cs $
// $Revision: 3 $	$Date: 10/29/07 11:02a $
// Copyright © 2005 Douglas G. Carlston. All rights reserved.

using System;
using NUnit.Framework;
using Tawala.Common;

namespace TawalaTest.CommonTest
{
	[TestFixture]
	public class GlobalSettingsTest
	{
		private string originalUserName;
		private string originalPassword;

		// execute this once at beginning of tests
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			// remember the real settings so that our test don't screw things up
			originalUserName = Tawala.Common.GlobalSettings.UserName;
			originalPassword = Tawala.Common.GlobalSettings.Password;
		}

		// execute this before each test method runs
		[SetUp]
		public void SetUp()
		{
		}

		[Test]
		public void UserNameTest()
		{
			Tawala.Common.GlobalSettings.UserName = "A cOmpletelY BoguS nAme";

			Assert.AreEqual(Tawala.Common.GlobalSettings.UserName, "A cOmpletelY BoguS nAme");
			Assert.IsFalse(Tawala.Common.GlobalSettings.UserName == originalUserName);
		}

		[Test]
		public void PasswordTest()
		{
			Tawala.Common.GlobalSettings.Password = "boGUs pasword";

			Assert.AreEqual(Tawala.Common.GlobalSettings.Password, "boGUs pasword");
			Assert.IsFalse(Tawala.Common.GlobalSettings.Password == originalPassword);
		}

		// execute this after each test method runs
		[TearDown]
		public void TearDown()
		{
		}

		// execute this once at end of all tests
		[TestFixtureTearDown]
		public void TestFixtureTearDown()
		{
			// restore the original settings
			Tawala.Common.GlobalSettings.UserName = originalUserName;
			Tawala.Common.GlobalSettings.Password = originalPassword;
		}
	}
}
