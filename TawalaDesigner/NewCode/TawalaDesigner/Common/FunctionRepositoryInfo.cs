// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.Functions.Runtime;

namespace Tawala.Common
{
    public static class FunctionRepositoryInfo
    {
        private static bool unableToLoad;

        public static bool IsLoaded { get { return FunctionLoader.Current != null; } }

        public static bool UnableToLoad { get { return unableToLoad; } }

        public static string QueryServerRepository(string credentialsXml, string parameters)
        {
            try
            {
                var transceiver = new XMLTransceiver(Config.FunctionRepositoryURL + parameters);
                transceiver.Transmit(string.Empty);
                return transceiver.Receive();
            }
            catch (Exception)
            {
            }

            return string.Empty;
        }

        public static void Build()
        {
            FunctionAssemblyCompiler.Build();
        }

        #region Nested type: FunctionAssemblyCompiler

        private static class FunctionAssemblyCompiler
        {
            private static CompilationInfo compilationInfo;

            public static void Build()
            {
                if (lastKnownSignatureMatchesWeb())
                {
                    if (tryLoad(FunctionLoader.GetPossibleFunctionDllLocation()))
                    {
                        return;
                    }
                }

                if (tryLoad(rebuild()))
                {
                    return;
                }

                unableToLoad = !tryLoad(FunctionLoader.GetPossibleFunctionDllLocation());
            }

            private static string rebuild()
            {
                string repositoryXmlString = QueryServerRepository("", "");

                if ((compilationInfo = FunctionLoader.Rebuild(repositoryXmlString)) != null)
                {
                    return compilationInfo.Path;
                }
                return string.Empty;
            }

            private static bool lastKnownSignatureMatchesWeb()
            {
                string xml = QueryServerRepository("", "?signature=" + FunctionLoader.GetLastKnownSignature());
                return xml.Contains("component-repository-is-current");
            }

            private static bool tryLoad(string path)
            {
                return FunctionLoader.Load(path) != null;
            }
        }

        #endregion
    }
}