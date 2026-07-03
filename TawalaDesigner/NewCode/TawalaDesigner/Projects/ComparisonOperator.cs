// $Workfile: ComparisonOperator.cs $
// $Revision: 19 $	$Date: 3/10/08 10:12a $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.Serialization;

/// =======================================================================
/// READ THIS - ComparisonOperator classes must adhere to these rules
/// 
/// 1. Each class declares an enum called Ops which is unique to the derived class.
/// 2. Each instance has a read-only Op field which is of type Ops.  
/// 3. There should be one instance of a derived class with a corresponding value for Op for every value in the Ops enum.
/// 4. Each derived class exposes a static collection call List which conforms to rule #3.
/// 5. The List collection is populated in order of the enums for the particular derived class.  
///		So the enum maps directly to a list index as an Int32.  Keep this in mind if changing the code.
/// 6. Do not change order, name or number of enums.  Only add to the end. Enum values should always 
///		start at 0 and have no gaps.  Only delete or re-arrange enums if you 
///		are willing to break deserialization of older project files -- you could always leave obsolete enums in.
/// 7. The enum's name is used as the name for xml tags (exposed as XmlTagName property).
/// 8. Each instance also exposes a Name property which is for display to the user.
/// 9. Only the Op field (containing an Ops enum value) is serialized.  Don't serialize anything else.
/// 10. On deserialization the objects are always recreated from the static lists in the code which aren't serialized. 
///		A comparison operator, such as ArithmeticOperator with an enum value of Ops.equals must be unique with a project.
///		So the a reference to ArithmeticOperator with Ops.equals is always a reference to the same object.
///		An ArithmeticOperator with Ops.doesNotEqual is a difference object but it should similarly have only one instance.
/// =======================================================================

namespace Tawala.Projects
{
	/// <summary>
	/// Operator for arithmetic comparisons.
	/// </summary>
	[DebuggerDisplay("ArithmeticOperator.{Op}")]
	[Serializable]
	public sealed class ArithmeticOperator : ComparisonOperator, IObjectReference
	{
		public readonly Ops Op;

		public enum Ops  // see rule #6
		{
			equals, doesNotEqual, isLessThan, isLessThanOrEqualTo, isGreaterThan, isGreaterThanOrEqualTo
		}

		public static readonly Collection List = Collection.Singleton;

		private ArithmeticOperator(Ops op, string name) : base(op, name, 2)
		{
			Op = op;
		}

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
			) {}	
		}

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
	}
	
	/// <summary>
	/// Operator for string comparisons.
	/// </summary>
	[DebuggerDisplay("StringOperator.{Op}")]
	[Serializable]
	public sealed class StringOperator : ComparisonOperator, IObjectReference
	{
		public enum Ops // see rule #6
		{
			equals, doesNotEqual, contains, doesNotContain, beginsWith, endsWith
		}

		public readonly Ops Op;

		public static readonly Collection List = Collection.Singleton;

		private StringOperator(Ops op, string name) : base(op, name, 2)
		{
			Op = op;
		}

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
			) {}	
		}

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
	}

	/// <summary>
	/// Operator for string and arithmetic comparisons.
	/// </summary>
	[DebuggerDisplay("HybridOperator.{Op}")]
	[Serializable]
	public sealed class HybridOperator : ComparisonOperator, IObjectReference
	{
		public enum Ops // see rule #6
		{
			equals, doesNotEqual, contains, doesNotContain, beginsWith, endsWith, isLessThan, isLessThanOrEqualTo, isGreaterThan, isGreaterThanOrEqualTo, isBlank, isNotBlank
		}

		public readonly Ops Op;

		public static readonly Collection List = Collection.Singleton;

		private HybridOperator(Ops op, string name, int numOperands)
			: base(op, name, numOperands)
		{
			Op = op;
		}

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
			) { }
		}

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
	}


	[DebuggerDisplay("MCOneOperator.{Op}")]
	[Serializable]
	public sealed class MCOneOperator : ComparisonOperator, IObjectReference
	{
		public enum Ops // see rule #6
		{
			mcEquals, mcDoesNotEqual, mcIsBlank, mcIsNotBlank
		}

		public readonly Ops Op;

		public static readonly Collection List = Collection.Singleton;

		private MCOneOperator(Ops op, string name, int numOperands) : base(op, name, numOperands)
		{
			Op = op;
		}

		public sealed class Collection : ComparisonOperatorCollection<MCOneOperator, Ops>
		{
			internal static readonly Collection Singleton = new Collection();

			private Collection() : base // see rules #5, #6, #7, #8
			(
				new MCOneOperator(Ops.mcEquals, "equals", 2),
				new MCOneOperator(Ops.mcDoesNotEqual, "does not equal", 2),
				new MCOneOperator(Ops.mcIsBlank, "is blank", 1),
				new MCOneOperator(Ops.mcIsNotBlank, "is not blank", 1)
			) {}	
		}

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
	}

	[DebuggerDisplay("MCManyOperator.{Op}")]
	[Serializable]
	public sealed class MCManyOperator : ComparisonOperator, IObjectReference
	{
		public readonly Ops Op;

		public enum Ops // see rule #6
		{
			mcEquals, mcDoesNotEqual, mcIsBlank, mcIsNotBlank, mcContains, mcDoesNotContain
		}

		public static readonly Collection List = Collection.Singleton;

		private MCManyOperator(Ops op, string name, int numOperands)
			: base(op, name, numOperands)
		{
			Op = op;
		}

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
			) {}	
		}

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
	}

	/// <summary>
	/// Base class for comparison operators in Condition objects.
	/// </summary>
	[Serializable]
	public abstract class ComparisonOperator
	{
		internal ComparisonOperator(object e, string name, int numOperands)
		{
			enumAsInt = Convert.ToInt32(e);
			this.xmlTagName = e.ToString();
			this.name = name;
			this.numOperands = numOperands;
		}

		[NonSerialized]
		internal readonly int enumAsInt = -1;

		private readonly int numOperands;

		public int NumOperands
		{
			get { return numOperands; }
		} 

		/// <summary>
		/// Friendly operator name for display purposes.  See Rule #8
		/// </summary>
		[NonSerialized]
		private readonly string name;

		public string Name 
		{
			get
			{
				return name;
			}
		}

		/// <summary>
		/// Operator name for use in XML tag.  See Rule #7
		/// </summary>
		[NonSerialized]
		private readonly string xmlTagName;

		public string XmlTagName
		{
			get
			{
				return xmlTagName;
			}
		}
	}

	/// <summary>
	/// Generic collection class for Comparison Operators.  Do not use directly.
	/// </summary>
	public class ComparisonOperatorCollection<T,E> where T : ComparisonOperator where E : struct
	{
		private Collection<T> items = new Collection<T>();

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

		public int Count
		{
			get
			{
				return items.Count;
			}
		}

		/// <summary>
		/// Use this for now when you want to use the collection as a data source.
		/// </summary>
		public IList DataSource
		{
			get
			{
				return items;
			}
		}

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
