// $Workfile: IDeserializedField.cs $
// $Revision: 1 $	$Date: 1/25/07 4:56p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;

namespace Tawala.Projects
{
	public interface IDeserializedField : IPaletteField
	{
		/// <summary>
		/// Returns a field in its proper form after deserialization.
		/// <remarks>
		/// Since deserialization produces a deep copy of an object, and since a deep copy is not always what is required
		/// after deserialization (e.g after a clipboard paste), field classes should implement this method to return either
		/// a shallow copy, a deep copy, or whatever hybrid is appropriate for the particular field.
		/// /// </remarks>
		/// </summary>
		IDeserializedField DeserializedFieldReference
		{
			get;
		}
	}
}
