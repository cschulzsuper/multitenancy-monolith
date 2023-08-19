using System;

namespace ChristianSchulz.MultitenancyMonolith.Application.Admission;

public interface IContextAuthenticationIdentityCommandClient : IContextAuthenticationIdentityCommandHandler, IDisposable
{
}