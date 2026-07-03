// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.Projects.Forms;

namespace Tawala.Forms
{
    public class FormItemCreator
    {
        private readonly Type formItemType;

        public FormItemCreator(Type type)
        {
            formItemType = type;
        }

        public FormItem CreateItem()
        {
            return (FormItem)Activator.CreateInstance(formItemType);
        }
    }
}