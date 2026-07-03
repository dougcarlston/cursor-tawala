// $Workfile: Invitations.cs $
// $Revision: 4 $	$Date: 11/25/05 4:35p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Tawala.Invite
{
	/// <summary>
	/// Static interface to all Invitations.  This will probably go away eventually
	/// as the Invitation Manager spec needs to be re-evaluated.
	/// </summary>
	internal static class Invitations
	{
		static private InvitationList list = null;

		static internal InvitationList List
		{
			get { return list; }
		}

		/// <summary>
		/// Load all the invitations in the "Invitations" file.
		/// If said file doesn't exist or there are any deserialization
		/// errors than simply initialize the List to a new empty list without warning.
		/// </summary>
		static internal void Load(string path)
		{
			try
			{
				if (File.Exists(path))
				{
					BinaryFormatter bf = new BinaryFormatter();
					bf.AssemblyFormat = FormatterAssemblyStyle.Simple;
					using (Stream s = File.OpenRead(path))
					{
						list = (InvitationList)bf.Deserialize(s);
					}
				}
			}
			catch (Exception)
			{
			}

			if (list == null)
			{
				list = new InvitationList();
			}
		}

		/// <summary>
		/// Serialize the entire invitation list to the "Invitations" file.
		/// </summary>
		static internal void Save(string path)
		{
			BinaryFormatter bf = new BinaryFormatter();
			bf.AssemblyFormat = FormatterAssemblyStyle.Simple;
			using (Stream s = File.Create(path))
			{
				bf.Serialize(s, list);
			}
		}

		// Inserted and Removed event support for notification of changes
		// to the invitation list.

		static internal event InvitationEventHandler Cleared;
		static internal event InvitationEventHandler Inserted;
		static internal event InvitationEventHandler Removed;
		static internal event InvitationEventHandler Set;

		static internal void RaiseClearedEvent(object sender)
		{
			if (Cleared != null)
			{
				Cleared(sender, new InvitationEventArgs(null));
			}
		}

		static internal void RaiseInsertedEvent(object sender, Invitation inv)
		{
			if (Inserted != null)
			{
				Inserted(sender, new InvitationEventArgs(inv));
			}
		}

		static internal void RaiseRemovedEvent(object sender, Invitation inv)
		{
			if (Removed != null)
			{
				Removed(sender, new InvitationEventArgs(inv));
			}
		}

		static internal void RaiseSetEvent(object sender, Invitation inv)
		{
			if (Set != null)
			{
				Set(sender, new InvitationEventArgs(inv));
			}
		}
	}

	delegate void InvitationEventHandler(object sender, InvitationEventArgs e);

	internal class InvitationEventArgs : EventArgs
	{
		internal readonly Invitation Inv;

		internal InvitationEventArgs(Invitation inv)
		{
			this.Inv = inv;
		}
	}
}
