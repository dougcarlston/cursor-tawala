// $Workfile: Memento.cs $
// $Revision: 3 $	$Date: 12/10/07 6:06p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace Tawala.UndoSupport
{
	[Serializable]
	public class Memento : IMemento
	{
		/// <summary>
		/// Constructor. Create memento.
		/// </summary>
		/// <param name="actionText">Text associated with undoable action (e.g. "Delete Line 1")</param>
		protected Memento(string actionText)
		{
			this.actionText = actionText;
		}

		/// <summary>
		/// Text associated with undoable action
		/// </summary>
		public string ActionText
		{
			get
			{
				return actionText;
			}
		}

		private string actionText;
	}
}
