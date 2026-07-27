// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections;
using System.Text;

namespace Tawala.Projects
{
	public interface IField : IEnumerable, IRecursiveEnumerable, IAnyField
	{
		// Returns the  name of the field, such as "Q1:a", "Record:Q1:a", "Score"
		string FieldName
		{
			get;
		}

		// Returns the bracketed name of the field, such as "<<Q1:a>>", "<<Record:Q1:a>>", "<<Score>>"
		string FieldString
		{
			get;
		}

		// Indexer - get field by name
		IField this[string name]
		{
			get;
		}
	}
}
