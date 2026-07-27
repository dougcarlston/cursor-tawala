using System;
using System.Reflection;
using System.IO;
using NUnit.Framework;
using Tawala.Proj;

namespace TawalaTest.ProjectTest
{
	/// <summary>
	/// Testing utility methods
	/// </summary>
	public class Util
	{
        [Obsolete("Use Equivalent class in TestSupport project")]
		public static void IsSerializable(Type type, bool checkDerived)
		{
			Assert.IsTrue(type.IsSerializable, "Class " + type.Name + " is missing  [Serializable] attribute");

			if (checkDerived)
			{
				Type[] types = type.Assembly.GetTypes();
				foreach (Type t in types)
				{
					Type baseType = t.BaseType;
					while (baseType != null)
					{
						if (baseType == type)
						{
							IsSerializable(t, false);
							break;
						}
						baseType = baseType.BaseType;
					}
				}
			}
		}

		/// <summary>
		/// Get an internal string that is part of an assembly's Strongly Typed Resources
		/// </summary>
		/// <remarks>
		/// Microsoft's StrongTypedResoureBuilder has an option to make resources public but its not exposed in the
		/// interface.  Regardless, it may not be appropriate for them to be public.
		/// </remarks>
		/// <param name="?"></param>
		/// <returns></returns>
        [Obsolete("Use Equivalent class in TestSupport project")]
        public static string GetInternalString(string assemblyName, string resourceClassName, string propertyName)
		{
			Type t = Type.GetType(resourceClassName + "," + assemblyName);

			if (t == null)
			{
				return null;
			}

			return t.InvokeMember(propertyName, BindingFlags.NonPublic | BindingFlags.GetProperty | BindingFlags.Static, null, null, null) as string;
		}

        [Obsolete("Use Equivalent class in TestSupport project")]
        public static void SetField(object o, string name, object val)
		{
			FieldInfo fieldInfo = o.GetType().GetField(name,
				BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public);
			fieldInfo.SetValue(o, val);
		}

		/// <summary>
		/// Use this function so that we can remove NewTestProject from the Project class.
		/// </summary>
        [Obsolete("Use Equivalent class in TestSupport project")]
        public static void NewTestProject()
		{
			// Create a new ProjectEvents object and set Project.projectEvents
			Type t = typeof(Tawala.Proj.Project);
			FieldInfo piEvents = t.GetField("projectEvents", BindingFlags.Static | BindingFlags.NonPublic);
			piEvents.SetValue(null, new Tawala.Proj.ProjectEvents());

			// Now call Project.New()
			Tawala.Proj.Project.New();
		}

		/// <summary>
		/// Returns the full path corresponding to the specified filename. It is presumed that the test file resides
		/// in the same directory as the calling assembly.
		/// </summary>
        [Obsolete("Use Equivalent class in TestSupport project")]
        public static string GetTestFilePath(string testFileName)
		{
			string codeBase = Assembly.GetCallingAssembly().CodeBase;
			Uri uri = new Uri(codeBase);
			string testDirectory = Path.GetDirectoryName(uri.AbsolutePath);
			string testFilePath = Path.Combine(testDirectory, testFileName);

			return testFilePath;
		}
	}

	class TestSetField
	{
		private int foo = 0;

		public int Foo
		{
			get { return foo; }
		}
	}

	[TestFixture]
	public class UtilTest
	{
		[Test]
		public void GetInternalString()
		{
			string s = Util.GetInternalString("Project", "Tawala.Proj.Properties.Resources", "SkipSummaryAlwaysSkip");
			Assert.IsNotNull(s);
			Assert.IsTrue(s.Length > 4);
		}

		[Test]
		public void NewTestProject()
		{
			Type t = typeof(Tawala.Proj.Project);
			FieldInfo piEvents = t.GetField("projectEvents", BindingFlags.Static | BindingFlags.NonPublic);
			FieldInfo piCurrent = t.GetField("current", BindingFlags.Static | BindingFlags.NonPublic);

			Util.NewTestProject();

			Project proj1 = piCurrent.GetValue(null) as Project;
			ProjectEvents pe1 = piEvents.GetValue(null) as ProjectEvents;

			Util.NewTestProject();

			Project proj2 = piCurrent.GetValue(null) as Project;
			ProjectEvents pe2 = piEvents.GetValue(null) as ProjectEvents;

			Assert.IsNotNull(proj1);
			Assert.IsNotNull(proj2);
			Assert.IsNotNull(pe1);
			Assert.IsNotNull(pe2);
			Assert.IsTrue(proj1 != proj2);
			Assert.IsTrue(pe1 != pe2);
		}

		[Test]
		public void SetField()
		{
			TestSetField tsf = new TestSetField();
			Util.SetField(tsf, "foo", 3141);
			Assert.AreEqual(3141, tsf.Foo);
		}
	}
}
