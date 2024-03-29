﻿using ChristianSchulz.MultitenancyMonolith.Objects.Access;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Access;

public interface IAccountMemberManager
{
    Task<AccountMember> GetAsync(long accountMember);

    Task<AccountMember> GetAsync(string accountMember);

    Task<AccountMember?> GetOrDefaultAsync(string accountMember);

    IQueryable<AccountMember> GetQueryable();

    Task InsertAsync(AccountMember @object);

    Task UpdateAsync(long accountMember, Action<AccountMember> action);

    Task UpdateAsync(string accountMember, Action<AccountMember> action);

    Task DeleteAsync(long accountMember);

    Task DeleteAsync(string accountMember);
}