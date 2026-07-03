// $Workfile: RtfToken.cs $
// $Revision: 2 $	$Date: 5/08/06 4:38p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;
using Tawala.XmlSupport;

namespace Tawala.RtfSupport
{
	public class RtfToken
	{
		public static RtfToken NULL = new NullToken();

		protected string tokenString = "";

		protected RtfToken()
		{
			this.actionMethod = defaultActionMethod;
		}

		protected RtfToken(string tokenString)
		{
			this.tokenString = tokenString;
		}

		protected RtfToken(ActionMethod actionMethod)
		{
			this.actionMethod = actionMethod;
		}

		public RtfToken(string tokenString, ActionMethod actionMethod)
		{
			this.tokenString = tokenString;
			this.actionMethod = actionMethod;
		}

		public override string ToString()
		{
			return tokenString;
		}

		/// <summary>
		/// Invoke token's action method
		/// </summary>
		public virtual void Execute()
		{
			actionMethod(tokenString);
		}

		protected ActionMethod actionMethod;

		private void defaultActionMethod(string anyText)
		{
		}
	}

	public class NullToken : RtfToken
	{
		public NullToken()
			: base("NullToken")
		{
		}
	}

}
