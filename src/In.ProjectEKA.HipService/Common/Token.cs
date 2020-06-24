namespace In.ProjectEKA.HipService.Common
{
	using System.Collections.Generic;

	public class Token
	{
		public IEnumerable<string> Roles { get; }

		public Token(IEnumerable<string> roles)
		{
			Roles = roles;
		}
	}
}