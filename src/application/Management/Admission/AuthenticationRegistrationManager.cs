using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Admission;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Admission
{
    internal sealed class AuthenticationRegistrationManager : IAuthenticationRegistrationManager
    {
        private readonly IRepository<AuthenticationRegistration> _repository;

        public AuthenticationRegistrationManager(IRepository<AuthenticationRegistration> repository)
        {
            _repository = repository;
        }

        public async Task<AuthenticationRegistration> GetAsync(Guid processToken)
        {
            AuthenticationRegistrationValidation.EnsureProcessToken(processToken);

            var @object = await _repository.GetAsync(@object => @object.ProcessToken == processToken);

            return @object;
        }

        public IQueryable<AuthenticationRegistration> GetAll()
            => _repository.GetQueryable();

        public async Task InsertAsync(AuthenticationRegistration @object)
        {
            AuthenticationRegistrationValidation.EnsureInsertable(@object);

            await _repository.InsertAsync(@object);
        }


        public async Task UpdateAsync(Guid processToken, Action<AuthenticationRegistration> action)
        {
            AuthenticationRegistrationValidation.EnsureProcessToken(processToken);

            var validatedAction = (AuthenticationRegistration @object) =>
            {
                action.Invoke(@object);

                AuthenticationRegistrationValidation.EnsureUpdatable(@object);
            };

            await _repository.UpdateOrThrowAsync(@object => @object.ProcessToken == processToken, validatedAction);
        }

        public async Task DeleteAsync(Guid processToken)
        {
            AuthenticationRegistrationValidation.EnsureProcessToken(processToken);

            await _repository.DeleteOrThrowAsync(@object => @object.ProcessToken == processToken);
        }
    }
}
