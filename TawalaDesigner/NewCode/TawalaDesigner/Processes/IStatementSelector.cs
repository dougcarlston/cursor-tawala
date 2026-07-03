// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;

namespace Tawala.Processes
{
	public interface IStatementSelector
	{
		void Init(Type[] statementViewTypes);
		void SyncButtonToCurrentStatementType(Type t);
		IProcessEditor ProcessEditor { get; set; }
	}
}
