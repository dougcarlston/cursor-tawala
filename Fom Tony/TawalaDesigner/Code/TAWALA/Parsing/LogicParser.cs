// $Workfile: LogicParser.cs $
// $Revision: 4 $	$Date: 2/10/06 4:44p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Tawala.Common;

namespace Tawala.Parsing
{
	public class LogicParser : MathParser
	{
		/// <param name="expression">Expression string to parse.</param>
		public LogicParser(string expression) : base(expression)
		{
		}

		/// <summary>
		/// Tokenize an expression string.
		/// </summary>
		// A token is one of:
		//  *	a positive or negative integer or floating point number
		//			e.g. "3", "37.5", "-90"
		//  *	a literal string without spaces
		//			e.g. "Name"
		//  *	a field identifier in angle brackets
		//			e.g. "<<Score 1>>", <<aNy+CHARacteRS.at_ALL>>
		//  *	one of the arithmetic operators
		//			e.g. "+", "-", "*", "/"
		//  *	a left or right parenthesis
		//			e.g. "(", ")"
		//  *	one of the logical operators
		//			e.g. "AND", "OR"
		protected override MatchCollection tokenize(string expression)
		{
//			Console.WriteLine("LogicParser.tokenize: expression = {0}", expression);

			// get match collection of tokens
			string pattern = @"begins with|is greater than or equal to|-?\d+\.?\d*|[a-zA-Z0-9]+|<<.+?>>|\+|\-|\*|/|\(|\)|AND|OR";
			MatchCollection mc = Regex.Matches(expression, pattern);

			return mc;
		}

		protected override bool isOperator(string token)
		{
//			Console.WriteLine("LogicParser.isOperator: token = {0}", token);

			return	(base.isOperator(token) || token == "AND" || token == "OR");
		}


		/// <summary>
		/// Return a value representing the precedence of an operator.
		/// </summary>
		/// <param name="token">operator string</param>
		/// <returns>Precedence value, where 0 is lowest precedence.</returns>
		protected override int precedence(string token)
		{
			if (token == "AND" || token == "OR")
			{
				return 1;
			}

			if (token == "+" || token == "-")
			{
				return 2;
			}

			if (token == "*" || token == "/")
			{
				return 3;
			}

			return 0;
		}




	}
}
