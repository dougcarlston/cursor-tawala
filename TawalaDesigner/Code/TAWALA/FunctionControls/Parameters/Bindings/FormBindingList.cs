// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Tawala.Projects;

namespace Tawala.Functions.Controls
{
    internal class FormBindingList : BindingList<IForm>
    {
        public FormBindingList(IList<IForm> list) : base(list)
        {
            Project.Events.ComponentAdded += delegate(object o, ComponentEventArgs args)
                                             {
                                                 if (args.Component is IForm)
                                                 {
                                                     ResetBindings();
                                                 }
                                             };
            Project.Events.ComponentRemoved += delegate(object o, ComponentEventArgs args)
                                               {
                                                   if (args.Component is IForm)
                                                   {
                                                       ResetBindings();
                                                   }
                                               };
            Project.Events.ComponentRenamed += delegate(object o, ComponentRenamedEventArgs args)
                                               {
                                                   if (args.Component is IForm)
                                                   {
                                                       ResetBindings();
                                                   }
                                               };
        }
    }
}