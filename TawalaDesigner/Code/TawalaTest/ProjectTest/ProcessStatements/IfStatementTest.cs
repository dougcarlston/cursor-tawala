// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Expressions;
using Tawala.Projects.Fields;
using Tawala.Projects.Processes;
using TawalaTest.TestSupport;

namespace TawalaTest.ProjectTest
{
    /// <summary>
    /// Test class for the IfStatement class
    /// </summary>
    [TestFixture]
    public class IfStatementTest
    {
        #region Setup/Teardown

        [SetUp]
        public void Setup()
        {
            Util.NewTestProject();
            Project.Current.AddForm(); // avoids exceptions when construction Conditions
        }

        #endregion

        [Test]
        public void Construct()
        {
            ProcessStatement statement = new IfStatement();

            Assert.IsNotNull(statement);
        }

        [Test]
        public void Copy()
        {
            var firstName = new Field("First Name");
            ComparisonOperator equals = StringOperator.List[StringOperator.Ops.equals];
            var conditions = new Conditions(firstName, equals, new Expression("Steve"));

            var statement = new IfStatement(conditions);

            var copiedStatement = (IfStatement)statement.Copy();

            Assert.IsFalse(copiedStatement == statement);
        }

        [Test]
        public void CopyAdvancedIf()
        {
            var firstName = new Field("First Name");
            var condition1 = new Condition(firstName, HybridOperator.List["is not blank"]);

            var lastName = new Field("Last Name");
            //FieldOrLiteral expression = new FieldOrLiteral("a", FieldOrLiteral.StringType.literal);
            //Condition condition2 = new Condition(lastName, MCOneOperator.List["equals"], expression);
            var expression = new Expression("a");
            var condition2 = new Condition(lastName, MCOneOperator.List["equals"], expression);

            var conditions = new Conditions();
            conditions.Add(condition1);
            conditions.Add(new LogicalOperator("AND"));
            conditions.Add(condition2);

            // create IF statement ('If Q1:a is not blank AND Q2 equals a')
            var statement = new IfStatement();
            statement.Conditions = conditions;

            var copiedStatement = (IfStatement)statement.Copy();

            Assert.IsFalse(copiedStatement == statement);
            Assert.AreEqual(3, copiedStatement.Conditions.Count);

            var copiedCondition1 = (Condition)copiedStatement.Conditions[0];
            Assert.AreSame(condition1.Field, copiedCondition1.Field);

            var copiedCondition2 = (Condition)copiedStatement.Conditions[2];
            Assert.AreSame(condition2.Field, copiedCondition2.Field);
        }

        [Test]
        public void GetText()
        {
            var firstName = new Field("First Name");
            ComparisonOperator equals = StringOperator.List[StringOperator.Ops.equals];
            var conditions = new Conditions(firstName, equals, new Expression("Steve"));

            ProcessStatement statement = new IfStatement(conditions);

            string expString = "If First Name equals \"Steve\"";

            Assert.AreEqual(expString, statement.ToString());
        }

        [Test]
        public void GetXml()
        {
            var firstName = new Field("First Name");
            ComparisonOperator equals = StringOperator.List[StringOperator.Ops.equals];
            var conditions = new Conditions(firstName, equals, new Expression("Steve"));

            var ifStatement1 = new IfStatement(conditions);

            string expString1 = "<if>\r\n" +
                                "<conditions>\r\n" +
                                "<equals field=\"First Name\">\r\n" +
                                "<string value=\"Steve\"/>\r\n" +
                                "</equals>\r\n" +
                                "</conditions>";

            Assert.AreEqual(expString1, ifStatement1.ToXml());
        }

        [Test]
        public void GetXmlIsBlank()
        {
            var firstName = new Field("First Name");
            ComparisonOperator isBlank = HybridOperator.List[HybridOperator.Ops.isBlank];
            var conditions = new Conditions(firstName, isBlank);

            var ifStatement1 = new IfStatement(conditions);

            string expString1 = "<if>\r\n" +
                                "<conditions>\r\n" +
                                "<isBlank field=\"First Name\" />\r\n" +
                                "</conditions>";

            Assert.AreEqual(expString1, ifStatement1.ToXml());
        }

        [Test]
        public void Name()
        {
            ProcessStatement statement = new IfStatement();

            string name = statement.Name;

            Assert.AreEqual("If", name);
        }
    }
}