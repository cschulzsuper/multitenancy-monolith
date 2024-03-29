﻿using ChristianSchulz.MultitenancyMonolith.Objects.Admission;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Admission;

public interface IAuthenticationRegistrationManager
{
    Task<AuthenticationRegistration> GetAsync(long authenticationRegistration);

    IQueryable<AuthenticationRegistration> GetQueryable();

    Task InsertAsync(AuthenticationRegistration @object);

    Task UpdateAsync(long authenticationRegistration, Action<AuthenticationRegistration> action);

    Task UpdateAsync(string authenticationIdentity, Action<AuthenticationRegistration> action);

    Task DeleteAsync(long authenticationRegistration);
}