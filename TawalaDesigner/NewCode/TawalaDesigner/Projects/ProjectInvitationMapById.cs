// $Workfile: ProjectInvitationMapById.cs $
// $Revision: 3 $	$Date: 3/13/08 5:51p $
// Copyright © 2005-2007 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using Tawala.Functions.Runtime;

namespace Tawala.Projects
{
	public class ProjectInvitationMapById 
	{
		public void AddUnique(ILink invitation)
		{
			if (!map.ContainsKey(invitation.Id))
			{
				map.Add(invitation.Id, invitation);
			}
		}

		public void Clear()
		{
			map.Clear();
		}

		public bool ContainsKey(int id)
		{
			return map.ContainsKey(id);
		}

		public ILink this[int id]
		{
			get { return map[id]; }
		}

		public bool Remove(int id)
		{
			return map.Remove(id);
		}

		public IEnumerator<ILink> GetEnumerator()
		{
			foreach (KeyValuePair<int, ILink> entry in map)
			{
				yield return entry.Value;
			}
		}

		private InvitationMapById map = new InvitationMapById();

		private class InvitationMapById : Dictionary<int, ILink>
		{
		}
	}
}
