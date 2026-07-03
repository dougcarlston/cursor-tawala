// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.XPath;
using Tawala.Functions.Runtime;
using Tawala.Projects;

namespace Tawala.Functions.Controls
{
    public class ColumnListBinder : ParameterBinder
    {
        private readonly IBindingList bindingList;
        private readonly ICompositeParameterCollection collection;
        private readonly IParameterInfo countInfo;
        private bool inSyncList;

        protected ColumnListBinder(IFunction instance, IParameterInfo info)
            : base(instance, info)
        {
            collection = instance.GetValue(info.Id) as ICompositeParameterCollection;
            bindingList = collection.CreateBindingList();

            XPathNavigator nav = info.Xml.SelectSingleNode("child::source-parameter/attribute::source-id");
            if (nav != null)
            {
                countInfo = instance.Info.Parameters[nav.Value];
            }

            function.PropertyChanged += function_PropertyChanged;
        }

        protected override void Bind(IParameterControl container)
        {
            var compositeParameterListControl = container as ColumnListParameterControl;
            if (!inSyncList)
            {
                syncListSize();
            }
            compositeParameterListControl.DataSource = bindingList;
        }

        private void function_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!inSyncList)
            {
                syncListSize();
            }
        }

        private void syncListSize()
        {
            try
            {
                if (!inSyncList)
                {
                    inSyncList = true;
                    int oldCount = bindingList.Count;
                    var count = (int)countInfo.GetValue(function);

                    if (count != oldCount)
                    {
                        while (bindingList.Count > count)
                        {
                            bindingList.RemoveAt(bindingList.Count - 1);
                        }

                        while (bindingList.Count < count)
                        {
                            ICompositeParameter item = collection.CreateItem();
                            item.GetType().GetProperty("DisplayConditions").SetValue(item, new Conditions(), null);
                            bindingList.Add(item);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                throw;
            }
            finally
            {
                inSyncList = false;
            }
        }
    }
}