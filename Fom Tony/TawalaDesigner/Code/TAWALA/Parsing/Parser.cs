// $Workfile: Parser.cs $
// $Revision: 7 $	$Date: 11/25/05 5:00p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Tawala.Parsing
{
	public class Parser
	{
		/// <summary>
		/// Mathematical or boolean expression in infix notation
		/// (e.g. "a + b * c", "this AND that")
		/// </summary>
		protected string infixExpression;

		/// <summary>
		/// Hash tables for translating operators to XML tags
		/// </summary>
		protected static Hashtable xmlOperatorStartTags = new Hashtable();
		protected static Hashtable xmlOperatorEndTags = new Hashtable();

		static Parser()
		{
			// fill operator start tags hashtable
			xmlOperatorStartTags.Add("+", "<add>\r\n");
			xmlOperatorStartTags.Add("-", "<sub>\r\n");
			xmlOperatorStartTags.Add("*", "<mul>\r\n");
			xmlOperatorStartTags.Add("/", "<div>\r\n");
			xmlOperatorStartTags.Add("AND", "<and>\r\n");
			xmlOperatorStartTags.Add("OR", "<or>\r\n");

			// fill operator end tags hashtable
			xmlOperatorEndTags.Add("+", "</add>\r\n");
			xmlOperatorEndTags.Add("-", "</sub>\r\n");
			xmlOperatorEndTags.Add("*", "</mul>\r\n");
			xmlOperatorEndTags.Add("/", "</div>\r\n");
			xmlOperatorEndTags.Add("AND", "</and>\r\n");
			xmlOperatorEndTags.Add("OR", "</or>\r\n");
		}


		/// <summary>
		/// Return an operation in XML format.
		/// </summary>
		/// <param name="operatorString">single-character arihmetic operator</param>
		/// <param name="operand1">first operand in XML format</param>
		/// <param name="operand2">second operand in XML format</param>
		protected string xmlOperation(string operatorString, string operand1, string operand2)
		{
			StringBuilder sb = new StringBuilder();

			// append XML start tag
			string startTag = (string)xmlOperatorStartTags[operatorString];
			sb.Append(startTag);

			// append operands in XML format
			sb.Append(operand1);
			sb.Append(operand2);

			// append XML end tag
			string endTag = (string)xmlOperatorEndTags[operatorString];
			sb.Append(endTag);

			return sb.ToString();
		}



		protected virtual bool isOperator(string token)
		{
			return (token == "+" || token == "-" || token == "*" || token == "/");
		}

		protected bool isLeftParen(string token)
		{
			return (token == "(");
		}

		protected bool isRightParen(string token)
		{
			return (token == ")");
		}

		protected bool isOperand(string token)
		{
			return (!isOperator(token) && !isLeftParen(token) && !isRightParen(token));
		}

		protected bool isField(string token)
		{
			return Regex.Match(token, "<<(.*)>>").Success;
		}

		/// <summary>
		/// Return a value representing the precedence of an operator.
		/// </summary>
		/// <param name="token">operator string</param>
		/// <returns>Precedence value, where 0 is lowest precedence.</returns>
		protected virtual int precedence(string token)
		{
			if (token == "+" || token == "-")
			{
				return 1;
			}

			if (token == "*" || token == "/")
			{
				return 2;
			}

			return 0;
		}

		/// <summary>
		/// Get a value representing the balance of parentheses in the infix expression.
		/// </summary>
		/// <returns>
		/// 0 if parentheses balance
		/// greater than 0 if more left than right parentheses
		/// less than 0 if more right than left parentheses
		/// </returns>
		public int Balance
		{
			get
			{
				// get match collection of parenthesis tokens
				string pattern = @"\(|\)";
				MatchCollection mc = Regex.Matches(infixExpression, pattern);

				int balance = 0;

				foreach (Match m in mc)
				{
					if (m.ToString() == "(")
					{
						balance++;
					}
					else if (m.ToString() == ")")
					{
						balance--;
					}
				}

				return balance;
			}
		}

	}

}
