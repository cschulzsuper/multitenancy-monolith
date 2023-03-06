﻿using ChristianSchulz.MultitenancyMonolith.Objects.Access;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Access;

public interface IAccountGroupManager
{
    Task<AccountGroup> GetAsync(long accountGroup);

    Task<AccountGroup> GetAsync(string accountGroup);

    IQueryable<AccountGroup> GetAll();

    Task InsertAsync(AccountGroup @object);

    Task UpdateAsync(long accountGroup, Action<AccountGroup> action);

    Task UpdateAsync(string accountGroup, Action<AccountGroup> action);

    Task DeleteAsync(long accountGroup);

    Task DeleteAsync(string accountGroup);

}