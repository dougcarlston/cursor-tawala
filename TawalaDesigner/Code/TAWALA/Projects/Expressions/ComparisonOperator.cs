// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Tawala.Projects.Expressions
{
    /// <summary>
    /// Operator for arithmetic comparisons.
    /// </summary>
    [DebuggerDisplay("ArithmeticOperator.{Op}")]
    [Serializable]
    public sealed class ArithmeticOperator : ComparisonOperator, IObjectReference
    {
        #region Ops enum

        public enum Ops // see rule #6
        {
            equals,
            doesNotEqual,
            isLessThan,
            isLessThanOrEqualTo,
            isGreaterThan,
            isGreaterThanOrEqualTo
        }

        #endregion

        public static readonly Collection List = Collection.Singleton;
        public readonly Ops Op;

        private ArithmeticOperator(Ops op, string name) : base(op, name, 2)
        {
            Op = op;
        }

        #region IObjectReference Members

        /// <summary>
        /// On deserialization replace this deserialized instance in the object graph 
        /// with the appropriate instance in the static internal list.
        /// This way only one instance of this class exists for each Ops enum value.
        /// All references in the object graph for this derived class with a particular enum value will
        /// be the same as the one in the static internal list.
        /// </summary>
        object IObjectReference.GetRealObject(StreamingContext context)
        {
            return List[Op];
        }

        #endregion

        #region Nested type: Collection

        public sealed class Collection : ComparisonOperatorCollection<ArithmeticOperator, Ops>
        {
            internal static readonly Collection Singleton = new Collection();

            private Collection() : base // see rules #5, #6, #7, #8
                (
                new ArithmeticOperator(Ops.equals, "equals"),
                new ArithmeticOperator(Ops.doesNotEqual, "does not equal"),
                new ArithmeticOperator(Ops.isLessThan, "is less than"),
                new ArithmeticOperator(Ops.isLessThanOrEqualTo, "is less than or equal to"),
                new ArithmeticOperator(Ops.isGreaterThan, "is greater than"),
                new ArithmeticOperator(Ops.isGreaterThanOrEqualTo, "is greater than or equal to")
                )
            {
            }
        }

        #endregion
    }

    /// <summary>
    /// Operator for string comparisons.
    /// </summary>
    [DebuggerDisplay("StringOperator.{Op}")]
    [Serializable]
    public sealed class StringOperator : ComparisonOperator, IObjectReference
    {
        #region Ops enum

        public enum Ops // see rule #6
        {
            equals,
            doesNotEqual,
            contains,
            doesNotContain,
            beginsWith,
            endsWith
        }

        #endregion

        public static readonly Collection List = Collection.Singleton;
        public readonly Ops Op;

        private StringOperator(Ops op, string name) : base(op, name, 2)
        {
            Op = op;
        }

        #region IObjectReference Members

        /// <summary>
        /// On deserialization replace this deserialized instance in the object graph 
        /// with the appropriate instance in the static internal list.
        /// This way only one instance of this class exists for each Ops enum value.
        /// All references in the object graph for this derived class with a particular enum value will
        /// be the same as the one in the static internal list.
        /// </summary>
        object IObjectReference.GetRealObject(StreamingContext context)
        {
            return List[Op];
        }

        #endregion

        #region Nested type: Collection

        public sealed class Collection : ComparisonOperatorCollection<StringOperator, Ops>
        {
            internal static readonly Collection Singleton = new Collection();

            private Collection() : base // see rules #5, #6, #7, #8
                (
                new StringOperator(Ops.equals, "equals"),
                new StringOperator(Ops.doesNotEqual, "does not equal"),
                new StringOperator(Ops.contains, "contains"),
                new StringOperator(Ops.doesNotContain, "does not contain"),
                new StringOperator(Ops.beginsWith, "begins with"),
                new StringOperator(Ops.endsWith, "ends with")
                )
            {
            }
        }

        #endregion
    }

    /// <summary>
    /// Operator for string and arithmetic comparisons.
    /// </summary>
    [DebuggerDisplay("HybridOperator.{Op}")]
    [Serializable]
    public sealed class HybridOperator : ComparisonOperator, IObjectReference
    {
        #region Ops enum

        public enum Ops // see rule #6
        {
            equals,
            doesNotEqual,
            contains,
            doesNotContain,
            beginsWith,
            endsWith,
            isLessThan,
            isLessThanOrEqualTo,
            isGreaterThan,
            isGreaterThanOrEqualTo,
            isBlank,
            isNotBlank
        }

        #endregion

        public static readonly Collection List = Collection.Singleton;
        public readonly Ops Op;

        private HybridOperator(Ops op, string name, int numOperands)
            : base(op, name, numOperands)
        {
            Op = op;
        }

        #region IObjectReference Members

        /// <summary>
        /// On deserialization replace this deserialized instance in the object graph 
        /// with the appropriate instance in the static internal list.
        /// This way only one instance of this class exists for each Ops enum value.
        /// All references in the object graph for this derived class with a particular enum value will
        /// be the same as the one in the static internal list.
        /// </summary>
        object IObjectReference.GetRealObject(StreamingContext context)
        {
            return List[Op];
        }

        #endregion

        #region Nested type: Collection

        public sealed class Collection : ComparisonOperatorCollection<HybridOperator, Ops>
        {
            internal static readonly Collection Singleton = new Collection();

            private Collection()
                : base // see rules #5, #6, #7, #8
                    (
                    new HybridOperator(Ops.equals, "equals", 2),
                    new HybridOperator(Ops.doesNotEqual, "does not equal", 2),
                    new HybridOperator(Ops.contains, "contains", 2),
                    new HybridOperator(Ops.doesNotContain, "does not contain", 2),
                    new HybridOperator(Ops.beginsWith, "begins with", 2),
                    new HybridOperator(Ops.endsWith, "ends with", 2),
                    new HybridOperator(Ops.isLessThan, "is less than", 2),
                    new HybridOperator(Ops.isLessThanOrEqualTo, "is less than or equal to", 2),
                    new HybridOperator(Ops.isGreaterThan, "is greater than", 2),
                    new HybridOperator(Ops.isGreaterThanOrEqualTo, "is greater than or equal to", 2),
                    new HybridOperator(Ops.isBlank, "is blank", 1),
                    new HybridOperator(Ops.isNotBlank, "is not blank", 1)
                    )
            {
            }
        }

        #endregion
    }

    [DebuggerDisplay("MCOneOperator.{Op}")]
    [Serializable]
    public sealed class MCOneOperator : ComparisonOperator, IObjectReference
    {
        #region Ops enum

        public enum Ops // see rule #6
        {
            mcEquals,
            mcDoesNotEqual,
            mcIsBlank,
            mcIsNotBlank
        }

        #endregion

        public static readonly Collection List = Collection.Singleton;
        public readonly Ops Op;

        private MCOneOperator(Ops op, string name, int numOperands) : base(op, name, numOperands)
        {
            Op = op;
        }

        #region IObjectReference Members

        /// <summary>
        /// On deserialization replace this deserialized instance in the object graph 
        /// with the appropriate instance in the static internal list.
        /// This way only one instance of this class exists for each Ops enum value.
        /// All references in the object graph for this derived class with a particular enum value will
        /// be the same as the one in the static internal list.
        /// </summary>
        object IObjectReference.GetRealObject(StreamingContext context)
        {
            return List[Op];
        }

        #endregion

        #region Nested type: Collection

        public sealed class Collection : ComparisonOperatorCollection<MCOneOperator, Ops>
        {
            internal static readonly Collection Singleton = new Collection();

            private Collection() : base // see rules #5, #6, #7, #8
                (
                new MCOneOperator(Ops.mcEquals, "equals", 2),
                new MCOneOperator(Ops.mcDoesNotEqual, "does not equal", 2),
                new MCOneOperator(Ops.mcIsBlank, "is blank", 1),
                new MCOneOperator(Ops.mcIsNotBlank, "is not blank", 1)
                )
            {
            }
        }

        #endregion
    }

    [DebuggerDisplay("MCManyOperator.{Op}")]
    [Serializable]
    public sealed class MCManyOperator : ComparisonOperator, IObjectReference
    {
        #region Ops enum

        public enum Ops // see rule #6
        {
            mcEquals,
            mcDoesNotEqual,
            mcIsBlank,
            mcIsNotBlank,
            mcContains,
            mcDoesNotContain
        }

        #endregion

        public static readonly Collection List = Collection.Singleton;
        public readonly Ops Op;

        private MCManyOperator(Ops op, string name, int numOperands)
            : base(op, name, numOperands)
        {
            Op = op;
        }

        #region IObjectReference Members

        /// <summary>
        /// On deserialization replace this deserialized instance in the object graph 
        /// with the appropriate instance in the static internal list.
        /// This way only one instance of this class exists for each Ops enum value.
        /// All references in the object graph for this derived class with a particular enum value will
        /// be the same as the one in the static internal list.
        /// </summary>
        object IObjectReference.GetRealObject(StreamingContext context)
        {
            return List[Op];
        }

        #endregion

        #region Nested type: Collection

        public sealed class Collection : ComparisonOperatorCollection<MCManyOperator, Ops>
        {
            internal static readonly Collection Singleton = new Collection();

            private Collection() : base // see rules #5, #6, #7, #8
                (
                new MCManyOperator(Ops.mcEquals, "equals", 2),
                new MCManyOperator(Ops.mcDoesNotEqual, "does not equal", 2),
                new MCManyOperator(Ops.mcIsBlank, "is blank", 1),
                new MCManyOperator(Ops.mcIsNotBlank, "is not blank", 1),
                new MCManyOperator(Ops.mcContains, "contains", 2),
                new MCManyOperator(Ops.mcDoesNotContain, "does not contain", 2)
                )
            {
            }
        }

        #endregion
    }

    /// <summary>
    /// Base class for comparison operators in Condition objects.
    /// </summary>
    [Serializable]
    public abstract class ComparisonOperator
    {
        [NonSerialized]
        internal readonly int enumAsInt = -1;

        /// <summary>
        /// Friendly operator name for display purposes.  See Rule #8
        /// </summary>
        [NonSerialized]
        private readonly string name;

        private readonly int numOperands;

        /// <summary>
        /// Operator name for use in XML tag.  See Rule #7
        /// </summary>
        [NonSerialized]
        private readonly string xmlTagName;

        internal ComparisonOperator(object e, string name, int numOperands)
        {
            enumAsInt = Convert.ToInt32(e);
            xmlTagName = e.ToString();
            this.name = name;
            this.numOperands = numOperands;
        }

        public int NumOperands { get { return numOperands; } }

        public string Name { get { return name; } }

        public string XmlTagName { get { return xmlTagName; } }
    }

    /// <summary>
    /// Generic collection class for Comparison Operators.  Do not use directly.
    /// </summary>
    public class ComparisonOperatorCollection<T, E> where T : ComparisonOperator where E : struct
    {
        private readonly Collection<T> items = new Collection<T>();

        internal ComparisonOperatorCollection(params T[] list)
        {
            int index = 0;

            foreach (T o in list)
            {
                items.Add(o);
                Debug.Assert(index == o.enumAsInt); // enum and order must match
                ++index;
            }
        }

        public int Count { get { return items.Count; } }

        /// <summary>
        /// Use this for now when you want to use the collection as a data source.
        /// </summary>
        public IList DataSource { get { return items; } }

        /// <summary>
        /// The enum doesn't work well as a type parameter so we have to treat as 
        /// just thing that that can be converted to an int which is used as an index.  Rules #5, #6 allow this to work
        /// </summary>
        public T this[E opEnum]
        {
            get
            {
                int index = Convert.ToInt32(opEnum);
                return items[index];
            }
        }

        /// <summary>
        /// Indexer for operator Lists
        /// </summary>
        /// <param name="opName">Friendly name or XML tag name of operator (e.g. "is blank", "isBlank", "does not contain", "doesNotContain", etc.)</param>
        /// <returns></returns>
        public T this[string opName]
        {
            get
            {
                for (int i = 0; i < Count; i++)
                {
                    if (items[i].Name == opName || items[i].XmlTagName == opName)
                    {
                        return items[i];
                    }
                }

                return null;
            }
        }
    }
}