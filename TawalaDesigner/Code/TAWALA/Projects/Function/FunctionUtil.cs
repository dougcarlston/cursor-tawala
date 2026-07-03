// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.Functions.Runtime;
using Tawala.Projects.Fields;

namespace Tawala.Projects
{
    public static class FunctionUtil
    {
        public static bool HasParameterRestrictions(IParameterInfo parameterInfo)
        {
            return parameterInfo != null && parameterInfo.Restrictions != ParameterRestrictions.Default;
        }

        public static IPaletteField ApplyParameterRestrictions(IParameterInfo parameterInfo, IPaletteField field)
        {
            if (field is Variable)
            {
                return field;
            }

            if (parameterInfo.Restrictions == ParameterRestrictions.RecordIterationAlways ||
                parameterInfo.Restrictions == ParameterRestrictions.RecordIterationNever)
            {
                var recordField = field as RecordField;

                if (parameterInfo.Restrictions == ParameterRestrictions.RecordIterationAlways)
                {
                    if (recordField != null)
                    {
                        field = recordField.ReferenceField as IPaletteField;
                    }
                    field = new RecordField(new Record(parameterInfo.RecordListName), field);
                }

                if (parameterInfo.Restrictions == ParameterRestrictions.RecordIterationNever)
                {
                    if (recordField != null)
                    {
                        field = recordField.ReferenceField as IPaletteField;
                    }
                }
            }
            return field;
        }
    }
}