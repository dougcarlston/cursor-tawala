// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Forms;
using Tawala.Functions.Runtime;
using Tawala.Projects;
using Tawala.ProjectUI;

namespace Tawala.Functions.Controls
{
    public static class ControlManager
    {
        private static Collection<QueryOKEnableState> queryOKEnableStateDelegates;

        internal static IConfigureFunctionControl ConfigureFunctionControl { get; private set; }
        internal static IFunction Function { get; private set; }

        public static bool IsFunctionDialogOpen()
        {
            return ConfigureFunctionControl != null && Function != null;
        }

        internal static void BeginSession(IFunction function, IConfigureFunctionControl container)
        {
            queryOKEnableStateDelegates = new Collection<QueryOKEnableState>
            {
                delegate { return ConfigureFunctionControl != null; },
                delegate { return Function != null; },
                delegate { return ((Control)ConfigureFunctionControl).Visible; },
                delegate { return Function.HasValidParameterValues(); }
            };

            ConfigureFunctionControl = container;
            Function = function;

            map = new ControlInstanceMap();

            Function.BeginEdit();

            updateFieldPaletteReferencedForms();

            Function.PropertyChanged += function_PropertyChanged;
        }

        internal static void EndSession()
        {
            if (Function != null)
            {
                queryOKEnableStateDelegates.Clear();
                queryOKEnableStateDelegates = null;

                Function.PropertyChanged -= function_PropertyChanged;
                Function.EndEdit();
                Function = null;

                ConfigureFunctionControl = null;
                map = null;
            }
        }

        internal static IParameterControl CreateControl(IParameterInfo parameterInfo)
        {
            IParameterControl parameterControl = createControl(parameterInfo.MapInfo.ControlType);
            IParameterBinder parameterBinder = createBinder(parameterInfo);

            RegisterControl(parameterControl, parameterInfo, parameterBinder);

            if (parameterBinder != null)
            {
                parameterBinder.Initialize(parameterControl);
            }

            return parameterControl;
        }

        internal static void CommitPendingChanges()
        {
            map.CommitPendingChanges();
        }

        internal static void RegisterControl(IParameterControl control, IParameterInfo info, IParameterBinder binder)
        {
            map.Add(control, info, binder);
        }

        internal static IParameterInfo LookupParameterInfo(IParameterControl ipc)
        {
            return map.GetParameterInfo(ipc);
        }

        internal static void SetCurrentParameterInfo(IParameterInfo info, Dictionary<string, string> replacements)
        {
            ConfigureFunctionControl.SetCurrentParameterInfo(info, replacements);
        }

        internal static void RegisterQueryOkEnableStateCallback(QueryOKEnableState callback)
        {
            if (queryOKEnableStateDelegates != null) // null if in design mode
            {
                queryOKEnableStateDelegates.Add(callback);
            }
        }

        internal static bool QueryOKEnabled()
        {
            if (queryOKEnableStateDelegates.Count == 0)
            {
                return false;
            }

            foreach (QueryOKEnableState ok in queryOKEnableStateDelegates)
            {
                if (!ok())
                {
                    return false;
                }
            }

            return true;
        }

        #region Private

        private static ControlInstanceMap map = new ControlInstanceMap();

        private static IParameterControl createControl(string controlFullTypeName)
        {
            return RuntimeTypeResolver.Construct(controlFullTypeName) as IParameterControl;
        }

        private static IParameterBinder createBinder(IParameterInfo info)
        {
            return RuntimeTypeResolver.Construct(info.MapInfo.BindingType, new object[] {info}) as IParameterBinder;
        }

        private static void function_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            updateFieldPaletteReferencedForms();
        }

        private static void updateFieldPaletteReferencedForms()
        {
            FunctionFormCollection referencedForms = ReferencedFormsHelper.Get(Function);

            if (referencedForms.Count > 0)
            {
                FieldsPalette.Palette.ConditionsForms = referencedForms;
            }
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

                ipc.HandleDestroyed += Control_HandleDestroyed;
            }

            public IParameterBinder GetBinder(IParameterControl ipc)
            {
                return dictionary[ipc].Binder;
            }

            public IParameterInfo GetParameterInfo(IParameterControl ipc)
            {
                return dictionary.ContainsKey(ipc) ? dictionary[ipc].ParameterInfo : null;
            }

            public void CommitPendingChanges()
            {
                foreach (IParameterControl parameterControl in dictionary.Keys)
                {
                    parameterControl.CommitPendingChanges();
                }
            }

            private void Control_HandleDestroyed(object sender, EventArgs e)
            {
                var ipc = sender as IParameterControl;
                ipc.HandleDestroyed -= Control_HandleDestroyed;
                if (dictionary.ContainsKey(ipc))
                {
                    dictionary.Remove(ipc);
                }
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

        #region Nested type: QueryOKEnableState

        internal delegate bool QueryOKEnableState();

        #endregion
    }
}