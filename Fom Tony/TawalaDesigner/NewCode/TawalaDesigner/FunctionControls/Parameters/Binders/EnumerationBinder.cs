// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml.XPath;
using Tawala.Functions.Runtime;

namespace Tawala.Functions.Controls
{
    public class EnumerationBinder : ParameterBinder
    {
        private readonly BindingList<StringToString> bindingList = new BindingList<StringToString>();

        protected EnumerationBinder(IFunction instance, IParameterInfo info)
            : base(instance, info)
        {
            createList();
        }

        protected override object RangeCheck(object o)
        {
            var value = o as string;

            if (value != null)
            {
                foreach (StringToString stringToString in bindingList)
                {
                    if (stringToString.Value.CompareTo(value) == 0)
                    {
                        return value;
                    }
                }
            }

            return bindingList[0].Value;
        }

        protected override void Bind(IParameterControl c)
        {
            if (c is ListControl)
            {
                var lc = c as ListControl;
                lc.DataSource = bindingList;
                lc.DisplayMember = "Display";
                lc.ValueMember = "Value";
                new EditBinding(c, "SelectedValue", this);
            }
        }

        private void createList()
        {
            bindingList.AllowEdit = false;
            bindingList.AllowNew = false;
            bindingList.AllowRemove = false;

            XPathNodeIterator iterator = parameterInfo.Xml.Select("descendant::choice");

            if (iterator != null && iterator.Count != 0)
            {
                while (iterator.MoveNext())
                {
                    bindingList.Add(new StringToString(iterator.Current));
                }
            }
        }
#pragma warning disable 0067

        #region Nested type: StringToString

        private class StringToString : INotifyPropertyChanged
        {
            private readonly string display = string.Empty;
            private readonly string value = string.Empty;

            public StringToString(XPathNavigator nav)
            {
                value = nav.GetAttribute("value", "");
                display = nav.GetAttribute("description", "");
            }

            public string Value { get { return value; } }

            public string Display { get { return display; } }

            // readonly list, bindings need this but we ignore it

            #region INotifyPropertyChanged Members

            public event PropertyChangedEventHandler PropertyChanged;

            #endregion
        }

        #endregion
    }
#pragma warning restore
}