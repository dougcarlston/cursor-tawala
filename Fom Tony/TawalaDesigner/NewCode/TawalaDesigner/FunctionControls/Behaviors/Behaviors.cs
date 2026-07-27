// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using WForm = System.Windows.Forms.Form;

namespace Tawala.Functions.Controls
{
    public abstract class Behavior
    {
        public Behavior(Control c)
        {
            Behaviors.Add(c, this);
        }

        protected Control AssociatedControl { get { return Behaviors.ControlFromBehavior(this); } }

        protected void NextControl()
        {
            var f = AssociatedControl.TopLevelControl as Form;
            f.Activate();
            Application.DoEvents();
            AssociatedControl.Focus();
            f.SelectNextControl(AssociatedControl, true, true, true, false);
        }

        public abstract void Detach(Control c);

        #region Nested type: Behaviors

        private static class Behaviors
        {
            private static readonly Dictionary<Behavior, Control> behaviorToControl = new Dictionary<Behavior, Control>();
            private static readonly Dictionary<Control, List<Behavior>> manager = new Dictionary<Control, List<Behavior>>();

            public static void Add(Control c, Behavior b)
            {
                if (!manager.ContainsKey(c))
                {
                    c.HandleDestroyed += c_HandleDestroyed;
                    manager[c] = new List<Behavior>();
                }
                else
                {
                    for (int i = manager[c].Count - 1; i >= 0; i--)
                    {
                        Behavior existing = manager[c][i];
                        if (existing.GetType() == b.GetType())
                        {
                            remove(existing);
                            --i;
                        }
                    }
                }

                add(c, b);
            }

            private static void add(Control c, Behavior b)
            {
                manager[c].Add(b);
                behaviorToControl.Add(b, c);
            }

            private static void remove(Behavior b)
            {
                Control c = behaviorToControl[b];
                behaviorToControl.Remove(b);
                manager[c].Remove(b);
                detach(b, c);
            }

            public static Control ControlFromBehavior(Behavior behavior)
            {
                if (behaviorToControl.ContainsKey(behavior))
                {
                    return behaviorToControl[behavior];
                }
                return null;
            }

            private static void c_HandleDestroyed(object sender, EventArgs e)
            {
                var c = ((Control)sender);
                if (manager.ContainsKey(c))
                {
                    foreach (Behavior existing in manager[c])
                    {
                        detach(existing, c);
                        behaviorToControl.Remove(existing);
                    }
                    manager[c] = null;
                    manager.Remove(c);
                }
            }

            private static void detach(Behavior b, Control c)
            {
                b.Detach(c);
                c.HandleDestroyed -= c_HandleDestroyed;
            }
        }

        #endregion
    }
}