// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml.XPath;
using Tawala.Functions.Runtime;

namespace Tawala.Functions.Controls
{
    public class NumericListBinder : ParameterBinder
    {
        private readonly BindingList<IntToString> intToStrings = new BindingList<IntToString>();
        private int max = 1;
        private int min;

        protected NumericListBinder(IParameterInfo info) : base(info)
        {
            createList();
        }

        protected override object RangeCheck(object o)
        {
            var value = (int)o;
            if (value < min || value > max)
            {
                o = min;
            }
            return o;
        }

        protected override void Bind(IParameterControl c)
        {
            if (c is ListControl)
            {
                var lc = c as ListControl;
                lc.DataSource = intToStrings;
                lc.DisplayMember = "Display";
                lc.ValueMember = "Value";
                new EditBinding(c, "SelectedValue", this);
            }
        }

        private void createList()
        {
            intToStrings.AllowEdit = false;
            intToStrings.AllowNew = false;
            intToStrings.AllowRemove = false;

            XPathNodeIterator iterator = parameterInfo.Xml.Select("descendant::choice");

            if (iterator.Count != 0)
            {
                while (iterator.MoveNext())
                {
                    intToStrings.Add(new IntToString(iterator.Current));
                }
                min = 1;
                max = intToStrings.Count;
            }
            else
            {
                min = parameterInfo.Xml.SelectSingleNode("descendant::minimum/attribute::value").ValueAsInt;
                max = parameterInfo.Xml.SelectSingleNode("descendant::maximum/attribute::value").ValueAsInt;

                for (int i = min; i <= max; ++i)
                {
                    intToStrings.Add(new IntToString(i));
                }
            }
        }
#pragma warning disable 0067

        #region Nested type: IntToString

        private class IntToString : INotifyPropertyChanged
        {
            private readonly string display = string.Empty;
            private readonly int value = -1;

            public IntToString(XPathNavigator nav)
            {
                value = Convert.ToInt32(nav.GetAttribute("value", ""));
                display = nav.GetAttribute("description", "");
            }

            public IntToString(int i)
            {
                value = i;
                display = Convert.ToString(i);
            }

            public int Value
            {
                get
                {
                    return value;
                }
            }

            public string Display
            {
                get
                {
                    return display;
                }
            }

            // readonly list, bindings need this but we ignore it

            #region INotifyPropertyChanged Members

            public event PropertyChangedEventHandler PropertyChanged;

            #endregion
        }

        #endregion
    }
#pragma warning restore
}