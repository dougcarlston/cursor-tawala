// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Specialized;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using Tawala.Common;

namespace Tawala.Projects
{
	[Serializable]
	public class ChoiceField : Field, IDeserializedField
	{
		public ChoiceField(string name) : base(name)
		{
		}

		public IDeserializedField DeserializedFieldReference
		{
			get
			{
				return (IDeserializedField)this;
			}
		}

	}
}
