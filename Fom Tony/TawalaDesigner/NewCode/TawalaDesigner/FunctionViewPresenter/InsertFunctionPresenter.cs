using System;
using System.Collections.ObjectModel;
using Tawala.Functions.Runtime;

namespace Tawala.Functions.ViewPresenter
{
	public class InsertFunctionPresenter : IInsertFunctionPresenter
	{
		private IInsertFunctionView view;
        private IFunctionRepository functionAssembly;

		public InsertFunctionPresenter(IInsertFunctionView view)
		{
			this.view = view;
            functionAssembly = FunctionLoader.Current;
            

            this.view.Setup(functionAssembly.Categories);
		}
	}
}
