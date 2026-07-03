// $Workfile: ConditionsParser.cs $
// $Revision: 2 $	$Date: 6/04/07 2:04p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;
using Tawala.Common;
using Tawala.Projects.Parsing;

namespace Tawala.Projects
{
	public class ConditionsParser : Parser
	{
		private Collection<IConditionComponent> components = new Collection<IConditionComponent>();

		public ConditionsParser(Collection<IConditionComponent> components)
		{
			this.components = components;
		}

		public Collection<IConditionComponent> Tokenize()
		{
			return components;
		}

		/// <summary>
		/// Convert the infix conditions list to postfix notation
		/// </summary>
		public Collection<IConditionComponent> ToPostfix()
		{
			// convert tokens to string in postfix notation
			return toPostfix(components);
		}

		/// <summary>
		/// Convert an array of tokens in infix notation
		/// to an array of tokens in postfix notation
		/// </summary>
		/// <remarks>
		/// This method employs operator precedence.
		/// </remarks>
		private Collection<IConditionComponent> toPostfix(Collection<IConditionComponent> tokens)
		{
			Collection<IConditionComponent> outputTokens = new Collection<IConditionComponent>();
			
			Stack stack = new Stack();

			// for each input token...
			for (int i = 0; i < tokens.Count; i++)
			{
				IConditionComponent token = tokens[i];

				if (isOperator(token))
				{
					// if stack is empty...
					if (stack.Count == 0)
					{
						// push operator onto stack
						stack.Push(token);
					}
					else
					{
						string stackToken = stack.Peek().ToString();

						// if token on top of stack has precedence...
						if (precedence(stackToken) >= precedence(token))
						{
							// pop token from stack and append to output
							outputTokens.Add((IConditionComponent)stack.Pop());
						}

						// push operator onto stack
						stack.Push(token);
					}
				}
				else if (isOperand(token))
				{
					// append operand to output
					outputTokens.Add(token);
				}
				else if (isLeftParen(token.ToString()))
				{
					// push parenthesis onto stack
					stack.Push(token);
				}
				else if (isRightParen(token.ToString()))
				{
					IConditionComponent stackToken;

					do
					{
						// pop token from stack
						stackToken = (IConditionComponent)stack.Pop();

						// if token is not a left parenthesis...
						if (!isLeftParen(stackToken.ToString()))
						{
							// append operator to output
							outputTokens.Add(stackToken);
						}

					} while (!isLeftParen(stackToken.ToString()));

				}
			}

			// while operators remain on stack...
			while (stack.Count > 0)
			{
				// pop operator from operator stack and append to output
				outputTokens.Add((IConditionComponent)stack.Pop());
			}

			return (outputTokens);
		}


		private const string xmlOperatorInvalidTag = "<invalidOperator />\r\n";

		/// <summary>
		/// Return condition list in XML format.
		/// </summary>
		public string ToXml()
		{
			StringBuilder sb = new StringBuilder();
			Stack stack = new Stack();

			// convert infix expression to postfix notation
			Collection<IConditionComponent> tokens = ToPostfix();

			// for each token...
			for (int i = 0; i < tokens.Count; i++)
			{
				IConditionComponent token = tokens[i];

				if (isOperand(token))
				{
					StringBuilder sbOperand = new StringBuilder();

					if (token is Condition)
					{
						// make operand from condition XML
						sbOperand.Append(((Condition)token).ToXml());
					}

					// push operand onto stack
					stack.Push(sbOperand.ToString());
				}
				else if (isOperator(token))
				{
					if (stack.Count >= 2)
					{
						// pop 2 operands from stack
						string operand2 = stack.Pop().ToString();
						string operand1 = stack.Pop().ToString();

						// create XML operation from operator and operands
						string operation = xmlOperation(token.ToString(), operand1, operand2);

						// push operation onto stack as single operand
						stack.Push(operation);
					}
					else
					{
						// return early with error indication in XML string
						sb = new StringBuilder(xmlOperatorInvalidTag);
						return (sb.ToString());
					}
				}
			}

			// return XML representation of entire expression
			return (stack.Pop().ToString());

		}

		
		/// <summary>
		/// Return a value representing the precedence of an operator.
		/// </summary>
		/// <returns>Precedence value, where 0 is lowest precedence.</returns>
		protected int precedence(IConditionComponent token)
		{
			if (token.ToString() == "AND" || token.ToString() == "OR")
			{
				return 1;
			}

			return 0;
		}


		protected bool isOperator(IConditionComponent token)
		{
			if (token is LogicalOperator)
			{
				return (token.ToString() == "AND" || token.ToString() == "OR");
			}

			return false;
		}


		protected bool isOperand(IConditionComponent token)
		{
			return (!isOperator(token) && !isLeftParen(token.ToString()) && !isRightParen(token.ToString()));
		}



	}
}
