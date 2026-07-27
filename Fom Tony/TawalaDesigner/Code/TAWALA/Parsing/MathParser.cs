// $Workfile: MathParser.cs $
// $Revision: 9 $	$Date: 8/10/06 2:34p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Tawala.Common;

namespace Tawala.Parsing
{
	public class MathParser : Parser
	{
		/// <param name="expression">Expression string to parse.</param>
		public MathParser(string expression)
		{
			this.infixExpression = expression;
		}

		/// <summary>
		/// Tokenize the infixExpression field.
		/// </summary>
		public MatchCollection Tokenize()
		{
			return tokenize(infixExpression);
		}

		/// <summary>
		/// Tokenize an expression string.
		/// </summary>
		// A token is one of:
		//  *	a positive or negative integer or floating point number
		//		(NOTE: the regular expression distinguishes between unary and binay minus)
		//			e.g. "3", "37.5", "-90"
		//  *	a literal string without spaces
		//			e.g. "Name"
		//  *	a field identifier in angle brackets
		//			e.g. "<<Score 1>>", <<aNy+CHARacteRS.at_ALL>>
		//  *	one of the arithmetic operators
		//			e.g. "+", "-", "*", "/"
		//  *	a left or right parenthesis
		//			e.g. "(", ")"
		protected virtual MatchCollection tokenize(string expression)
		{
			// get match collection of tokens
			string pattern = @"^-?\d*\.?\d+|(?<=[\s\(]{1})-?\d*\.?\d+|[a-zA-Z0-9]+|<<.+?>>|\+|\-|\*|/|\(|\)";

			MatchCollection mc = Regex.Matches(expression, pattern);

			return mc;
		}


		/// <summary>
		/// Convert the infix expression string to prefix notation
		/// </summary>
		public string ToPostfix()
		{
			// get tokens as match collection
			MatchCollection mc = tokenize(infixExpression);

			// convert match collection to string array
			string[] tokens = new string[mc.Count];

			for (int i = 0; i < mc.Count; i++)
			{
				tokens[i] = mc[i].ToString();
			}

			// convert tokens to string in postfix notation
			return toPostfix(tokens);
		}

		/// <summary>
		/// Convert an array of tokens in infix notation
		/// to a string in postfix notation
		/// </summary>
		/// <remarks>
		/// This method employs operator precedence.
		/// </remarks>
		private string toPostfix(string[] tokens)
		{
			StringBuilder sb = new StringBuilder();
			Stack stack = new Stack();

			// for each input token...
			for (int i = 0; i < tokens.Length; i++)
			{
				string token = tokens[i];

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
							sb.Append(stack.Pop().ToString() + " ");
						}

						// push operator onto stack
						stack.Push(token);
					}
				}
				else if (isOperand(token))
				{
					// append operand to output
					sb.Append(token + " ");
				}
				else if (isLeftParen(token))
				{
					// push parenthesis onto stack
					stack.Push(token);
				}
				else if (isRightParen(token))
				{
					string stackToken;

					do
					{
						// pop token from stack
						stackToken = stack.Pop().ToString();

						// if token is not a left parenthesis...
						if (!isLeftParen(stackToken))
						{
							// append operator to output
							sb.Append(stackToken + " ");
						}

					} while (!isLeftParen(stackToken));

				}
			}

			// while operators remain on stack...
			while (stack.Count > 0)
			{
				// pop operator from operator stack and append to output
				sb.Append(stack.Pop().ToString() + " ");
			}

			// remove leading and trailing spaces before returning output
			return (sb.ToString().Trim());
		}

		private static readonly string xmlOperandValueTag = "<operand value=\"$VALUE\"/>\r\n";
		private static readonly string xmlOperandFieldTag = "<operand field=\"$FIELD\"/>\r\n";
		private static readonly string xmlOperandInvalidTag = "<operand invalidExpression=\"$EXPRESSION\"/>\r\n";

		/// <summary>
		/// Return expression in XML format.
		/// </summary>
		public string ToXml()
		{
			StringBuilder sb = new StringBuilder();
			Stack stack = new Stack();

			// convert infix expression to postfix notation
			string postfixExpression = ToPostfix();

			// tokenize postfix expression and convert to string array
			MatchCollection mc = tokenize(postfixExpression);
			string[] tokens = new string[mc.Count];

			for (int i = 0; i < mc.Count; i++)
			{
				tokens[i] = mc[i].ToString();
			}

			// for each token...
			for (int i = 0; i < tokens.Length; i++)
			{
				string token = tokens[i];

				if (isOperand(token))
				{
					StringBuilder sbOperand = new StringBuilder();

					if (isField(token))
					{
						// use field tag
						sbOperand.Append(xmlOperandFieldTag);

						// remove angle brackets, then any invalid characters, before building XML operand
						string rawToken = Regex.Replace(token, "<<(.*)>>", "$1");
						sbOperand.Replace("$FIELD", XMLStringFormatter.EscapeAttributeText(rawToken));
					}
					else
					{
						// use value tag
						sbOperand.Append(xmlOperandValueTag);

						// build XML operand
						sbOperand.Replace("$VALUE", token);
					}

					// push operand in XML onto stack
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
						string operation = xmlOperation(token, operand1, operand2);

						// push operation onto stack as single operand
						stack.Push(operation);
					}
					else
					{
						// return early with error indication in XML string
						sb = new StringBuilder(xmlOperandInvalidTag);
						sb.Replace("$EXPRESSION", infixExpression);
						return (sb.ToString());
					}
				}
			}
			
			// return XML representation of entire expression
			return (stack.Pop().ToString());

		}
	}
}
