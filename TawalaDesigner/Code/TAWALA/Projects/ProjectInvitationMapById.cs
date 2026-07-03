// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.Generic;
using Tawala.Projects.Links;

namespace Tawala.Projects
{
    public class ProjectInvitationMapById
    {
        private readonly InvitationMapById map = new InvitationMapById();

        public ILink this[int id] { get { return map[id]; } }

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

        public bool Remove(int id)
        {
            return map.Remove(id);
        }

        public IEnumerator<ILink> GetEnumerator()
        {
            foreach (var entry in map)
            {
                yield return entry.Value;
            }
        }

        #region Nested type: InvitationMapById

        private class InvitationMapById : Dictionary<int, ILink>
        {
        }

        #endregion
    }
}