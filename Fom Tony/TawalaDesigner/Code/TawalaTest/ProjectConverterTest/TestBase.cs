using System;
using System.Reflection;
using System.IO;
using System.Xml;
using System.Collections.ObjectModel;
using NUnit.Framework;
using Tawala.Projects;

using TawalaTest.ProjectTest;

namespace ProjectConverterTest
{
	public class TestBase
	{
		protected TawalaProjectConverter GetConverter(string projectFileName)
		{
			TawalaProjectConverter converter = null;
		    Stream stream = null;

		    try
		    {
		        stream = File.OpenRead(GetTestFilePath(projectFileName));
                converter = new TawalaProjectConverter(stream);

		    }
		    catch (Exception e)
		    {
		       if (stream != null)
		       {
		           stream.Close();
		       }
		       throw;
		    }

			return converter;
		}

		protected string GetTestFilePath(string projectFileName)
		{
			return TawalaTest.TestSupport.Util.GetTestFilePath(projectFileName);
		}

		protected void RoundtripProjectXml()
		{
			string randomTempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
			Directory.CreateDirectory(randomTempPath);

			string savePath1 = Path.Combine(randomTempPath, "validate.save1");
			string savePath2 = Path.Combine(randomTempPath, "validate.save2");

			Assert.IsTrue(Directory.Exists(randomTempPath));
			Assert.IsFalse(File.Exists(savePath1));
			Assert.IsFalse(File.Exists(savePath2));

			try
			{
				Project.Save(savePath1);
				Assert.AreEqual(Project.Current.Name, "validate");
				string saved1Xml = File.ReadAllText(savePath1);

				TawalaTest.TestSupport.Util.NewTestProject();

				Project.ProjectFileOpenResult opened = Project.Open(savePath1);
				Assert.AreEqual(Project.ProjectFileOpenResult.OK, opened);

				Project.Save(savePath2);
				string saved2Xml = File.ReadAllText(savePath2);

				Assert.AreEqual(saved1Xml, saved2Xml, "Project XML Validation Failed");
			}
			finally
			{
				if (File.Exists(savePath1))
				{
					File.Delete(savePath1);
				}
				if (File.Exists(savePath2))
				{
					File.Delete(savePath2);
				}
				Directory.Delete(randomTempPath, false);
			}
		}

		protected BindingFlags flags =
			BindingFlags.NonPublic |
			BindingFlags.Public |
			BindingFlags.Static |
			BindingFlags.Instance;

		protected Type tConverter = typeof(TawalaProjectConverter);


		protected FieldInfo GetConverterFieldInfo(string fieldName)
		{
			return tConverter.GetField(fieldName, flags);
		}

		protected MethodInfo GetConverterMethodInfo(string methodName)
		{
			return tConverter.GetMethod(methodName, flags);
		}
	}
}
