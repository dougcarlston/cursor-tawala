// $Workfile: InvitationList.cs $
// $Revision: 5 $	$Date: 11/25/05 4:35p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Text;

namespace Tawala.Invite
{
	/// <summary>
	/// Collection of all invitations which is not only accessible by
	/// integer index but also keyed of the Invitation objects Name property.
	/// The key is case insensitive and duplicats aren't allowed -- invitation
	/// names are "global".
	/// </summary>
	[Serializable]
	internal class InvitationList : KeyedCollection<string, Invitation>
	{
		internal InvitationList()
			: base(StringComparer.InvariantCultureIgnoreCase, 0)
		{
		}

		/// <summary>
		/// Must rename through the list so that the list's key stays in
		/// sync with the actual invitation object's name.
		/// </summary>
		public void Rename(Invitation inv, string newName)
		{
			// I tried to use ChangeItemKey to do this but it always throws an exception.

			int index = IndexOf(inv);
			// call base so that we don't fire our event
			base.RemoveItem(index);
			inv.Name = newName;

			base.InsertItem(index, inv);
		}

		protected override string GetKeyForItem(Invitation inv)
		{
			return inv.Name;
		}

		protected override void ClearItems()
		{
			base.ClearItems();
			Invitations.RaiseClearedEvent(this);
		}

		protected override void InsertItem(int index, Invitation inv)
		{
			if (inv.Name == null || inv.Name.Length == 0)
			{
				generateName(inv);
			}
			base.InsertItem(index, inv);
			Invitations.RaiseInsertedEvent(this, inv);
		}

		protected override void RemoveItem(int index)
		{
			Invitation inv = this[index];
			base.RemoveItem(index);
			Invitations.RaiseRemovedEvent(this, inv);
		}

		protected override void SetItem(int index, Invitation inv)
		{
			if (inv.Name == null || inv.Name.Length == 0)
			{
				generateName(inv);
			}
			base.SetItem(index, inv);
			Invitations.RaiseSetEvent(this, inv);
		}

		/// <summary>
		/// Generate a unique name
		/// </summary>
		private void generateName(Invitation inv)
		{
			int num = 1;
			string name;

			while (true)
			{
				name = "Invitation" + num++;
				if (!Contains(name))
				{
					inv.Name = name;
					break;
				}
			} 
		}
	}
}
