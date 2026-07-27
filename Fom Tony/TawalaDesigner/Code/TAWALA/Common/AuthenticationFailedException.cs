// Copyright © 2008 Tawala Systems, Inc. All rights reserved.

using System;

namespace Tawala.Common
{
	public class AuthenticationFailedException : Exception
	{
		public AuthenticationFailedException()
			: base("Authentication Failed")
		{
		}
	}
}
