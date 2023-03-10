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

        public async Task<AccountRegistration> GetAsync(Guid processToken)
        {
            AccountRegistrationValidation.EnsureProcessToken(processToken);

            var @object = await _repository.GetAsync(@object => @object.ProcessToken == processToken);

            return @object;
        }

        public IQueryable<AccountRegistration> GetAll()
            => _repository.GetQueryable();

        public async Task InsertAsync(AccountRegistration @object)
        {
            AccountRegistrationValidation.EnsureInsertable(@object);

            await _repository.InsertAsync(@object);
        }


        public async Task UpdateAsync(Guid processToken, Action<AccountRegistration> action)
        {
            AccountRegistrationValidation.EnsureProcessToken(processToken);

            var validatedAction = (AccountRegistration @object) =>
            {
                action.Invoke(@object);

                AccountRegistrationValidation.EnsureUpdatable(@object);
            };

            await _repository.UpdateOrThrowAsync(@object => @object.ProcessToken == processToken, validatedAction);
        }

        public async Task DeleteAsync(Guid processToken)
        {
            AccountRegistrationValidation.EnsureProcessToken(processToken);

            await _repository.DeleteOrThrowAsync(@object => @object.ProcessToken == processToken);
        }
    }
}
