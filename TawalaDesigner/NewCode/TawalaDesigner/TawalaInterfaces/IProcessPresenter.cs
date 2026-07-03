// Copyright © 2008 Tawala Systems, Inc. All rights reserved.

using Tawala.Projects.Processes;

namespace Tawala.Interfaces
{
	public interface IProcessPresenter
	{
		IProcessView View { get; }
		IProcess Process { get; set; }
	}
}
