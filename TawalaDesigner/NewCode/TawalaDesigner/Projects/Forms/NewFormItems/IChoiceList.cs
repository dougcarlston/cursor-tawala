// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Tawala.Projects.Forms.NewFormItems
{
	public interface IChoiceList : IField
	{
		void Add(IChoice item);
		void Clear();
		int Count { get; }
		IChoice this[int index] { get; set; }

		string GetLabel(int choiceIndex);
		string ToXhtml(IFormItem formItem);
	}
}
