using System;
using System.Collections.Generic;
using System.Text;
using Tawala.Projects;

namespace Tawala.Processes
{
    public class GetStatementPresenter
    {
        private IStatementView view;

        public GetStatementPresenter(IStatementView view)
        {
            this.view = view;
        }

		public FormList GetFormList()
		{
			return Project.Current.AllForms;
		}

		public Projects.FormList GetDefaultCheckedFormList()
        {
            FormList list = new FormList();

			// Note: To address Bug 678, don't populate the default Form List.
			//       That can cause a Record List node to appear in the Fields Palette
			//       even when not editing a Get statement, and that allows Record Set
			//       Fields to be used illegally in other statements.
			//												jdf - 2/1/08
			//
			//foreach (Proj.Form form in Project.Current.FormList)
			//{
			//    if (form.ConnectedProcess == view.Process)
			//    {
			//        list.Add(form);
			//    }
			//}
			//
			//if (list.Count == 0 && Project.Current.FormList.Count > 0)
			//{
			//    list.Add(Project.Current.FormList[0] as Proj.Form);
			//}

            return list;
        }
	}
}