// $Workfile: Conditions.cs $
// $Revision: 12 $	$Date: 3/10/08 10:12a $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;
using System.Text;
using System.Collections.ObjectModel;
using Tawala.Functions.Runtime;
using Tawala.Projects.Forms.FormItemContents;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.XmlSupport;

namespace Tawala.Projects
{
	[Serializable]
	public class ConditionComponentList : Collection<IConditionComponent>, IConditionComponent
	{
		/// <summary>
		/// Removes each of the specified components from the list
		/// </summary>
		public void RemoveComponents(Conditions components)
		{
			foreach (IConditionComponent c in components)
			{
				Remove(c);
			}
		}

		/// <summary>
		/// Adds each of the specified components individually to the list
		/// </summary>
		public void AddComponent(IConditionComponent component)
		{
			if (component is ConditionComponentList)
			{
				foreach (IConditionComponent c in (ConditionComponentList)component)
				{
					Add(c);
				}
			}
			else
			{
				Add(component);
			}
		}


    }

	[Serializable]
	public class Conditions : ConditionComponentList, IFunctionParameterXml
	{
		private static Factory<IConditionComponent> componentFactory = new Factory<IConditionComponent>();

		static Conditions()
		{
			componentFactory.Register("equals", typeof(FibCondition));
			componentFactory.Register("doesNotEqual", typeof(FibCondition));
			componentFactory.Register("contains", typeof(FibCondition));
			componentFactory.Register("doesNotContain", typeof(FibCondition));
			componentFactory.Register("beginsWith", typeof(FibCondition));
			componentFactory.Register("endsWith", typeof(FibCondition));
			componentFactory.Register("isLessThan", typeof(FibCondition));
			componentFactory.Register("isLessThanOrEqualTo", typeof(FibCondition));
			componentFactory.Register("isGreaterThan", typeof(FibCondition));
			componentFactory.Register("isGreaterThanOrEqualTo", typeof(FibCondition));

			componentFactory.Register("isBlank", typeof(FibNoExpressionCondition));
			componentFactory.Register("isNotBlank", typeof(FibNoExpressionCondition));

			componentFactory.Register("mcEquals", "value", typeof(MCValueCondition));
			componentFactory.Register("mcDoesNotEqual", "value", typeof(MCValueCondition));
			componentFactory.Register("mcContains", "value", typeof(MCValueCondition));
			componentFactory.Register("mcDoesNotContain", "value", typeof(MCValueCondition));
			componentFactory.Register("mcEquals", typeof(MCFieldCondition));
			componentFactory.Register("mcDoesNotEqual", typeof(MCFieldCondition));
			componentFactory.Register("mcContains", typeof(MCFieldCondition));
			componentFactory.Register("mcDoesNotContain", typeof(MCFieldCondition));

			componentFactory.Register("mcIsBlank", typeof(MCFieldNoExpressionCondition));
			componentFactory.Register("mcIsNotBlank", typeof(MCFieldNoExpressionCondition));

			componentFactory.Register("and", typeof(LogicalOperator));
			componentFactory.Register("or", typeof(LogicalOperator));
		}

		public Conditions()
		{
		}

		public Conditions(Condition condition)
		{
			Add(condition);
		}

		public Conditions(IField field, ComparisonOperator compOp, Expression expression) : this(new Condition(field, compOp, expression))
		{
		}

		public Conditions(IField field, ComparisonOperator compOp, IChoice choice): this(new Condition(field, compOp, choice))
		{
		}

		public Conditions(IField field, ComparisonOperator compOp) : this(new Condition(field, compOp))
		{
		}

		public Conditions(IXmlElement element, string processName) : this()
		{
			if (containsSingleCondition(element))
			{
				Add(componentFactory.MakeObject(element.GetChild(0), processName));
			}
			else
			{
				ConditionBuilderWithProcessName conditionBuilder = new ConditionBuilderWithProcessName(element.GetChild(0), processName);
				AddComponent(conditionBuilder.GetCondition());
			}
		}

		public Conditions(IXmlElement element, Process process) : this()
		{
			if (containsSingleCondition(element))
			{
				Add(componentFactory.MakeObject(element.GetChild(0), process));
			}
			else
			{
				ConditionBuilderWithProcessReference conditionBuilder = new ConditionBuilderWithProcessReference(element.GetChild(0), process);
				AddComponent(conditionBuilder.GetCondition());
			}
		}

		public Conditions(IXmlElement element, IField fieldResolver)
			: this()
		{
			if (containsSingleCondition(element))
			{
				Add(componentFactory.MakeObject(element.GetChild(0), fieldResolver));
			}
			else
			{
				ConditionBuilderWithFieldResolver conditionBuilder = new ConditionBuilderWithFieldResolver(element.GetChild(0), fieldResolver);
				AddComponent(conditionBuilder.GetCondition());
			}
		}

		private Boolean containsSingleCondition(IXmlElement conditionsElement)
		{
			return (!isLogicalOperator(conditionsElement.GetChild(0)));
		}

		private static Boolean isLogicalOperator(IXmlElement element)
		{
			return (element.Name == "and" || element.Name == "or");
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			foreach (IConditionComponent component in this)
			{
				if (component != null)
				{
					sb.Append(component.ToString() + " ");
				}
			}

			return sb.ToString().Trim();
		}

		private const string defaultConditionsTagName = "conditions";

		/// <summary>
		/// Returns XML for the conditions with outer tag containing the default name ("conditions").
		/// </summary>
		public string ToXml()
		{
			return ToXml(defaultConditionsTagName);
		}

		private const string xmlStartFormat = "<{0}>\r\n";
		private const string xmlEndFormat = "</{0}>";

		/// <summary>
		/// Returns XML for the conditions with outer tag containing the passed name.
		/// </summary>
		public string ToXml(string tagName)
		{
			if (Count > 0)
			{
				var xmlString = new StringBuilder();
				xmlString.AppendFormat(xmlStartFormat, tagName);

				var parser = new ConditionsParser(this);
				xmlString.Append(parser.ToXml());

				xmlString.AppendFormat(xmlEndFormat, tagName);

				return xmlString.ToString();
			}

			return string.Empty;
		}

        #region IFunctionParameterXml Members

        public string ToFunctionParameterXml()
        {
            if (Count > 0)
            {
                var parser = new ConditionsParser(this);
                return parser.ToXml();
            }
            return string.Empty;
        }

        #endregion
        /// <summary>
		/// Returns a Boolean indicating whether the character index range of the specified component intersects
		/// with the specified starting and ending indexes.
		/// </summary>
		private bool intersectsWith(IConditionComponent component, int startIndex, int endIndex)
		{
			bool intersects = true;

			if (startIndex > GetEndCharacterIndex(component) || endIndex < GetStartCharacterIndex(component))
			{
				intersects = false;
			}

			return intersects;
		}

		/// <summary>
		/// Returns a list of the components corresponding to the specified range of character indexes
		/// </summary>
		/// <param name="startIndex">Zero-based index of first character in string formed by all components in conditions</param>
		/// <param name="endIndex">Zero-based index of last character in string formed by all components in conditions</param>
		public Conditions GetComponents(int startIndex, int endIndex)
		{

			Conditions components = new Conditions();

			foreach (IConditionComponent c in Items)
			{
				if (intersectsWith(c, startIndex, endIndex))
				{
					components.Add(c);
				}
			}

			return components;
		}

		/// <summary>
		/// Returns the starting character index corresponding to the specified component
		/// </summary>
		public int GetStartCharacterIndex(IConditionComponent component)
		{
			int startIndex = -1;
			StringBuilder sb = new StringBuilder();

			foreach (IConditionComponent c in Items)
			{
				if (c == component)
				{
					startIndex = sb.Length;
					break;
				}

				sb.Append(c.ToString() + " ");
			}

			return startIndex;
		}


		/// <summary>
		/// Returns the ending character index corresponding to the specified component
		/// </summary>
		public int GetEndCharacterIndex(IConditionComponent component)
		{
			int endIndex = -1;
			StringBuilder sb = new StringBuilder();

			foreach (IConditionComponent c in Items)
			{
				if (c == component)
				{
					endIndex = sb.Length + c.ToString().Length - 1;
					break;
				}

				sb.Append(c.ToString() + " ");
			}

			return endIndex;
		}

		/// <summary>
		/// Returns the component corresponding to the specified character index
		/// </summary>
		/// <param name="characterIndex">Zero-based index of character in string formed by all components in conditions</param>
		public IConditionComponent GetComponent(int characterIndex)
		{
			IConditionComponent component = null;
			StringBuilder sb = new StringBuilder();
			int startIndex = 0;

			if (characterIndex > ToString().Length - 1)
			{
				characterIndex = ToString().Length - 1;
			}

			foreach (IConditionComponent c in Items)
			{
				sb.Append(c.ToString());

				if (characterIndex >= startIndex && characterIndex < sb.Length)
				{
					component = c;
					break;
				}

				sb.Append(" ");

				startIndex += c.ToString().Length + 1;
			}

			return component;
		}

		/// <summary>
		/// Returns the component index corresponding to the specified character index
		/// </summary>
		/// <param name="characterIndex">Zero-based index of character in string formed by all components in conditions</param>
		/// <returns>
		/// Index of element, or -1 if character index is between components, or
		/// component count if character index is after last component
		/// </returns>
		public int GetComponentIndex(int characterIndex)
		{
			int componentIndex = -1;

			StringBuilder sb = new StringBuilder();
			int startIndex = 0;

			for (int i = 0; i < Count; i++)
			{
				IConditionComponent c = Items[i];

				sb.Append(c.ToString());

				if (characterIndex >= startIndex && characterIndex < sb.Length)
				{
					componentIndex = i;
					break;
				}

				sb.Append(" ");

				startIndex += c.ToString().Length + 1;

				if (startIndex > characterIndex)
				{
					if (i == (Items.Count - 1))
					{
						componentIndex = Items.Count;
					}
					else
					{
						componentIndex = -1;
					}

					break;
				}
			}

			return componentIndex;
		}


		/// <summary>
		/// Returns the index of the next component that can be inserted ahead of corresponding to the specified character index
		/// </summary>
		/// <param name="characterIndex">Zero-based index of character in string formed by all elements in condition list</param>
		public int GetComponentInsertionIndex(int characterIndex)
		{
			if (Count == 0)
			{
				return 0;
			}

			int[] componentIndexes = new int[ToString().Length + 1];

			for (int i = 0; i < componentIndexes.Length; i++)
			{
				int j = 0;

				do
				{
					componentIndexes[i] = GetComponentIndex(i + j);

					j++;

				} while (componentIndexes[i] == -1);

			}

			return componentIndexes[characterIndex];
		}

		/// <summary>
		/// Returns a Boolean indicating whether the specified character index denotes a valid location for a logical operator
		/// </summary>
		public bool AtOperatorLocation(int characterIndex)
		{
			bool atLocation = false;

			// if index is to the right of all conditions...
			if (characterIndex >= ToString().Length)
			{
				// if component to the left of index is a condition...
				if (GetComponent(characterIndex - 1) is Condition)
				{
					atLocation = true;
				}
			}

			// if components on either side of index are conditions...
			else if (GetComponent(characterIndex - 1) is Condition && GetComponent(characterIndex + 1) is Condition)
			{
				atLocation = true;
			}

			return atLocation;
		}

		public static Conditions ShallowCopy(Conditions sourceConditions)
		{
			Conditions conditions = new Conditions();

			foreach (IConditionComponent condition in sourceConditions)
			{
				conditions.Add(condition);
			}

			return conditions;
		}

		#region Condition Builder Classes

		private abstract class ConditionBuilder
		{
			public ConditionBuilder(IXmlElement element)
			{
				this.element = element;
			}

			public IConditionComponent GetCondition()
			{
				ConditionComponentList logicalCondition = new ConditionComponentList();

				logicalCondition.AddComponent(getOperand1());
				logicalCondition.AddComponent(getLogicalOperator());
				logicalCondition.AddComponent(getOperand2());

				return logicalCondition;
			}

			private IConditionComponent getOperand1()
			{
				return getOperand(0);
			}

			private IConditionComponent getOperand2()
			{
				return getOperand(1);
			}

			protected IXmlElement element;

			protected abstract IConditionComponent getLogicalOperator();

			protected abstract IConditionComponent getOperand(int index);
		};

		private class ConditionBuilderWithProcessName : ConditionBuilder
		{
			public ConditionBuilderWithProcessName(IXmlElement element, string processName)
				: base(element)
			{
				this.processName = processName;
			}

			private string processName;

			protected override IConditionComponent getLogicalOperator()
			{
				return componentFactory.MakeObject(element, processName);
			}

			protected override IConditionComponent getOperand(int index)
			{
				IConditionComponent operand;
				if (isLogicalOperator(element.GetChild(index)))
				{
					ConditionBuilderWithProcessName childBuilder = new ConditionBuilderWithProcessName(element.GetChild(index), processName);
					operand = childBuilder.GetCondition();
				}
				else
				{
					operand = componentFactory.MakeObject(element.GetChild(index), processName);
				}

				return operand;
			}
		}

		private class ConditionBuilderWithProcessReference : ConditionBuilder
		{
			public ConditionBuilderWithProcessReference(IXmlElement element, Process process)
				: base(element)
			{
				this.process = process;
			}

			private Process process;

			protected override IConditionComponent getLogicalOperator()
			{
				return componentFactory.MakeObject(element, process);
			}

			protected override IConditionComponent getOperand(int index)
			{
				IConditionComponent operand;
				if (isLogicalOperator(element.GetChild(index)))
				{
					ConditionBuilderWithProcessReference childBuilder = new ConditionBuilderWithProcessReference(element.GetChild(index), process);
					operand = childBuilder.GetCondition();
				}
				else
				{
					operand = componentFactory.MakeObject(element.GetChild(index), process);
				}

				return operand;
			}
		}

		private class ConditionBuilderWithFieldResolver : ConditionBuilder
		{
			public ConditionBuilderWithFieldResolver(IXmlElement element, IField fieldResolver)
				: base(element)
			{
				this.fieldResolver = fieldResolver;
			}

			private IField fieldResolver;

			protected override IConditionComponent getLogicalOperator()
			{
				return componentFactory.MakeObject(element, fieldResolver);
			}

			protected override IConditionComponent getOperand(int index)
			{
				IConditionComponent operand;
				if (isLogicalOperator(element.GetChild(index)))
				{
					ConditionBuilderWithFieldResolver childBuilder = new ConditionBuilderWithFieldResolver(element.GetChild(index), fieldResolver);
					operand = childBuilder.GetCondition();
				}
				else
				{
					operand = componentFactory.MakeObject(element.GetChild(index), fieldResolver);
				}

				return operand;
			}
		}

		#endregion
	}
}

