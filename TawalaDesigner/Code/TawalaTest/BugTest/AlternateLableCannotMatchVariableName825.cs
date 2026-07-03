using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;
using TawalaTest.TestSupport;

namespace TawalaTest.BugTest
{
	[TestFixture]
	public class AlternateLableCannotMatchVariableName825
	{
		[Test]
		public void AlternateItemLabelThatDuplicatesVariableNameIsValid()
		{
			Util.NewTestProject();

			IForm form = Project.Current.AddForm();
			FormItemList list = form.ItemList;
			IFibItem fibItem = new FibItem();
			list.Add(fibItem);

			Process process = Project.Current.AddProcess();
			process.Variables.AddUnique("foo");

			Assert.IsTrue(list.ValidAlternateLabel(fibItem, null, "foo"));
		}
	}
}