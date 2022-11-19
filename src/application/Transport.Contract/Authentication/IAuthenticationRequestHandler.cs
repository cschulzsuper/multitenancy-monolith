﻿using ChristianSchulz.MultitenancyMonolith.Application.Authentication.Request;
using System.Security.Claims;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authentication;

public interface IAuthenticationRequestHandler
{
    ClaimsIdentity SignIn(string uniqueName, SignInRequest request);
}
