// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.XPath;
using NUnit.Framework;
using Tawala.Functions.Runtime;
using TawalaTest.TestSupport;

namespace TawalaTest.FunctionTests
{
    public abstract class FunctionTestBase
    {
        protected static IFunctionRepository functionRepository;
        protected static XPathNavigator resourceXml;

        protected static int xmlCategoryCount;

        protected void FixtureSetup()
        {
            if (functionRepository == null)
            {
                functionRepository = FunctionLoader.Repository;
                Assert.IsTrue(functionRepository is IFunctionRepository);

                string xml = stripNamespacesFromXml(XmlConstants.FunctionRepositoryXml);

                using (var sr = new StringReader(xml))
                {
                    resourceXml = new XPathDocument(sr).CreateNavigator();
                }
            }
        }

        protected int categoryCount()
        {
//			return resourceXml.Select("//category").Count + 1;
            return resourceXml.Select("//category").Count;
        }

        protected string[] categoryStrings()
        {
            XPathExpression expr = XPathExpression.Compile("//category");
            expr.AddSort("@name", StringComparer.InvariantCulture);

            XPathNodeIterator iterator = resourceXml.Select(expr);

//			string[] strings = new string[iterator.Count + 1];
            var strings = new string[iterator.Count];

//			strings[0] = "All";

//			int i = 1;
            int i = 0;
            while (iterator.MoveNext())
            {
                strings[i++] = iterator.Current.GetAttribute("name", "");
            }

            return strings;
        }

        protected int sumOfFunctionsByCategoryExceptAll()
        {
            return resourceXml.Select("//category/element-id").Count;
        }

        protected XPathNodeIterator GetAllComponentTypesFromXmlSortedByName()
        {
            XPathExpression expr = XPathExpression.Compile("/component-repository/child::node()");
            expr.AddSort("@name", StringComparer.Ordinal);
            XPathNodeIterator iterator = resourceXml.Select(expr);
            return iterator;
        }

        private static string stripNamespacesFromXml(string xml)
        {
            xml = xml.Replace("<tr:", "<").Replace("</tr:", "</");
            xml = Regex.Replace(xml, @" xmlns:[^=]+=[^\s]+", "");
            return xml;
        }

        protected Dictionary<Type, Collection<MemberInfo>> GetMemberInfoForAllInterfaces(Type t)
        {
            var map = new Dictionary<Type, Collection<MemberInfo>>();

            Type[] types = t.GetInterfaces();

            foreach (Type tInterface in types)
            {
                map[tInterface] = new Collection<MemberInfo>(tInterface.GetMembers());
            }

            return map;
        }
    }
}