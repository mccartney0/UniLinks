﻿using Microsoft.AspNetCore.Authorization;

using UniLink.Dependencies.Enums;

namespace UniLink.API.Attributes
{
	public class AuthorizesAttribute : AuthorizeAttribute
	{
		public AuthorizesAttribute(params UserTypeEnum[] roles) : base() => Roles = string.Join(",", roles);
	}
}