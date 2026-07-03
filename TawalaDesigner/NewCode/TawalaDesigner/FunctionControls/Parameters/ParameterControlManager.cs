// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Tawala.Functions.Runtime;
using Tawala.Projects;
using Tawala.ProjectUI;

namespace Tawala.Functions.Controls
{
    internal static class ParameterControlManager
    {
        public static IConfigureFunctionControl ConfigureFunctionControl { get; private set; }
        public static IFunction Function { get; private set; }

        public static void BeginSession(IFunction function, IConfigureFunctionControl container)
        {
            ConfigureFunctionControl = container;
            map = new ControlInstanceMap();

            Function = function;
            Function.BeginEdit();

            updateFieldPaletteReferencedForms();

            Function.PropertyChanged += function_PropertyChanged;
        }

        public static void EndSession()
        {
            Function.PropertyChanged -= function_PropertyChanged;
            Function.EndEdit();
            Function = null;
            ConfigureFunctionControl = null;
        }

        public static IParameterControl CreateBoundControl(IParameterInfo parameterInfo)
        {
            IParameterControl parameterControl = createControl(parameterInfo.MapInfo.ControlType);
            IParameterBinder parameterBinder = createBinder(Function, parameterInfo);

            RegisterBoundControl(parameterControl, parameterInfo, parameterBinder);

            parameterBinder.Initialize(parameterControl);

            return parameterControl;
        }

        public static void CommitPendingChanges()
        {
            map.CommitPendingChanges();
        }

        public static void RegisterBoundControl(IParameterControl control, IParameterInfo info, IParameterBinder binder)
        {
            map.Add(control, info, binder);
        }

        public static IParameterBinder LookupBinder(IParameterControl ipc)
        {
            return map.GetBinder(ipc);
        }

        public static IParameterInfo LookupParameterInfo(IParameterControl ipc)
        {
            return map.GetParameterInfo(ipc);
        }

        public static FunctionFormCollection GetReferencedForms()
        {
            FunctionFormCollection referencedForms = ReferencedFormsHelper.Get(Function);

            if (referencedForms.Count > 0)
            {
                FieldsPalette.Palette.ConditionsForms = referencedForms;
            }

            return referencedForms;
        }

        #region Private

        private static ControlInstanceMap map = new ControlInstanceMap();

        private static IParameterControl createControl(string controlFullTypeName)
        {
            return RuntimeTypeResolver.Construct(controlFullTypeName) as IParameterControl;
        }

        private static IParameterBinder createBinder(IFunction instance, IParameterInfo info)
        {
            return RuntimeTypeResolver.Construct(info.MapInfo.BindingType, new object[] {instance, info}) as IParameterBinder;
        }

        private static void function_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            updateFieldPaletteReferencedForms();
        }

        private static void updateFieldPaletteReferencedForms()
        {
            GetReferencedForms();
        }

        private class ControlInstanceMap
        {
            private readonly Dictionary<IParameterControl, Info> dictionary = new Dictionary<IParameterControl, Info>();

            public void Add(IParameterControl ipc, IParameterInfo ipi, IParameterBinder ipb)
            {
                var info = new Info();
                info.ParameterInfo = ipi;
                info.Binder = ipb;

                dictionary.Add(ipc, info);

                ipc.GetControl().HandleDestroyed += Control_HandleDestroyed;
            }

            public IParameterBinder GetBinder(IParameterControl ipc)
            {
                return dictionary[ipc].Binder;
            }

            public IParameterInfo GetParameterInfo(IParameterControl ipc)
            {
                return dictionary[ipc].ParameterInfo;
            }

            public void CommitPendingChanges()
            {
                foreach (IParameterControl ipc in dictionary.Keys)
                {
                    var ipec = ipc as IParameterEditControl;
                    if (ipec != null)
                    {
                        ipec.CommitPendingChanges();
                    }
                }
            }

            private void Control_HandleDestroyed(object sender, EventArgs e)
            {
                var ipc = sender as IParameterControl;
                dictionary.Remove(ipc);
            }

            #region Nested type: Info

            public struct Info
            {
                public IParameterBinder Binder;
                public IParameterInfo ParameterInfo;
            }

            #endregion
        }

        #endregion
    }
}