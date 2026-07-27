// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.ComponentModel;
using System.Globalization;
using Tawala.Functions.Runtime;
using Tawala.Projects;

namespace Tawala.Functions.Controls
{
    // FunctionConditons actually
    public class FunctionConditionsBinder : ParameterBinder
    {
        private EditBinding binding;

        protected FunctionConditionsBinder(IFunction instance, IParameterInfo info)
            : base(instance, info)
        {
            if (info.GetValue(instance) == null)
            {
                info.SetValue(instance, new FunctionConditions());
            }
        }

        protected override void Bind(IParameterControl c)
        {
            c.GetType().GetProperty("WhereText").SetValue(c, parameterInfo.Name + " where", null); // static so don't need a binding

            binding = new EditBinding(c, "Conditions", this);

            function.PropertyChanged += function_PropertyChanged;
        }

        private void function_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // any change 
            binding.WriteValue();
        }

        #region BindingConverter   --  Converts FunctionConditions to/from Conditions

        // called by data binding engine automatically

        public class BindingConverter : TypeConverter
        {
            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                if (destinationType == typeof(Conditions))
                {
                    return true;
                }

                return base.CanConvertTo(context, destinationType);
            }

            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
            {
                if (destinationType == typeof(Conditions) && value != null)
                {
                    var fc = value as FunctionConditions;
                    if (fc.Conditions == null)
                    {
                        fc.Conditions = new Conditions();
                    }
                    return fc.Conditions;
                }

                return base.ConvertTo(context, culture, value, destinationType);
            }

            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                if (sourceType == typeof(Conditions))
                {
                    return true;
                }

                return base.CanConvertFrom(context, sourceType);
            }

            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            {
                if (value != null && value.GetType() == typeof(Conditions))
                {
                    var fc = new FunctionConditions();
                    fc.Conditions = value as Conditions;
                    fc.Forms = ParameterControlManager.GetReferencedForms();
                    return fc;
                }
                return base.ConvertFrom(context, culture, value);
            }
        }

        #endregion
    }
}