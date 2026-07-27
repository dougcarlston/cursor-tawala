using Tawala.Projects;
using Tawala.Common;
using Tawala.Projects.Processes;
using Tawala.XmlSupport;

using NUnit.Framework;

namespace TawalaTest.BugTest
{
    [TestFixture]
	[Ignore("KM 20070704 - TODO")]
    public class NoFormsAndDataSourceInForEachBadSave527
    {
		private IForm form;
		private FormList formList;
		private Process process;
		private RecordSet recordSet;
		private Record record;

		[SetUp]
		public void Setup()
		{
			TestSupport.Util.NewTestProject();

//			initializeTestDataSources();

			form = Project.Current.AddForm();
			process = Project.Current.AddProcess();

			formList = new FormList();
			formList.Add(form);

			recordSet = new RecordSet("RecordSet", formList);
			record = new Record("rec");
		}

		//[Test]
		//[Ignore("KM: In progress")]
		//public void NoFormsAndSharedDataSource()
		//{
		//    GetStatement get = new GetStatement(new RecordSet("RecordSet", list));
		//    process.Lines.Add(new ProcessLineList(get));

		//    ForEachRecordStatement forEach = new ForEachRecordStatement(record, recordSet);
		//    process.Lines.Add(new ProcessLineList(forEach));

		//    SetStatement set = new SetStatement();
		//    set.Variable = new AssignableField("ClientInfo:Q1:a");
		//    set.Expression = one;

		//    forEach.EnclosedStatements.Add(set);

		//    string tmpFile = TestSupport.Util.SaveCurrentProject();

		//    bool opened = false;

		//    try
		//    {
		//        opened = TestSupport.Util.LoadProject(tmpFile);
		//    }
		//    catch (System.Reflection.AmbiguousMatchException e)
		//    {
		//        ambiguous = e;
		//    }

		//    Assert.IsNull(ambiguous, ambiguous.ToString());
		//}

		//private const string testDataSourceXml =
		//    "<datasources>" +
		//    "<datasource name=\"ClientInfo\">" +
		//    "<field name=\"Q1:a\" type=\"string\"/>" +
		//    "</datasource>" +
		//    "</datasources>";

		//private void initializeTestDataSources()
		//{
		//    MethodInfo init = typeof(FieldProvidersSupport).GetMethod("initialize", BindingFlags.NonPublic | BindingFlags.Static);
		//    init.Invoke(null, new object[] { new XmlElement(testDataSourceXml) });
		//}
	}
}
