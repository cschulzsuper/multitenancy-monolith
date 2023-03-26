using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Access;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Access
{
    internal sealed class AccountRegistrationManager : IAccountRegistrationManager
    {
        private readonly IRepository<AccountRegistration> _repository;

        public AccountRegistrationManager(IRepository<AccountRegistration> repository)
        {
            _repository = repository;
        }

        public async Task<AccountRegistration> GetAsync(long accountRegistration)
        {
            AccountRegistrationValidation.EnsureAccountRegistration(accountRegistration);

            var @object = await _repository.GetAsync(accountRegistration);

            return @object;
        }

        public IQueryable<AccountRegistration> GetQueryable()
            => _repository.GetQueryable();

        public async Task InsertAsync(AccountRegistration @object)
        {
            AccountRegistrationValidation.EnsureInsertable(@object);

            await _repository.InsertAsync(@object);
        }


        public async Task UpdateAsync(long accountRegistration, Action<AccountRegistration> action)
        {
            AccountRegistrationValidation.EnsureAccountRegistration(accountRegistration);

            var validatedAction = (AccountRegistration @object) =>
            {
                action.Invoke(@object);

                AccountRegistrationValidation.EnsureUpdatable(@object);
            };

            await _repository.UpdateOrThrowAsync(accountRegistration, validatedAction);
        }

        public async Task UpdateAsync(string accountGroup, Action<AccountRegistration> action)
        {
            AccountRegistrationValidation.EnsureAccountGroup(accountGroup);

            var validatedAction = (AccountRegistration @object) =>
            {
                action.Invoke(@object);

                AccountRegistrationValidation.EnsureUpdatable(@object);
            };

            await _repository.UpdateOrThrowAsync(@object => @object.AccountGroup == accountGroup, validatedAction);
        }

        public async Task DeleteAsync(long accountRegistration)
        {
            AccountRegistrationValidation.EnsureAccountRegistration(accountRegistration);

            await _repository.DeleteOrThrowAsync(accountRegistration);
        }
    }
}
