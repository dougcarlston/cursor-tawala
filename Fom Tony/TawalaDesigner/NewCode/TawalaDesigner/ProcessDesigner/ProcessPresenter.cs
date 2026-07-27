// Copyright © 2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using Tawala.Projects.Processes;
using Tawala.Interfaces;

namespace Tawala.ProcessDesigner
{
	public class ProcessPresenter : IProcessPresenter
	{
		public ProcessPresenter(IProcessView view, IProcess process)
		{
			this.View = view;
			this.Process = process;
		}

		#region IProcessPresenter Members

		public IProcessView View { get; protected set; }
		public IProcess Process { get; set; }

		#endregion
	}
}
