﻿using ChristianSchulz.MultitenancyMonolith.Application.Admission.Commands;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Admission;

public interface IContextAuthenticationRegistrationCommandHandler
{
    Task ConfirmAsync(ContextAuthenticationRegistrationConfirmCommand command);

    Task RegisterAsync(ContextAuthenticationRegistrationRegisterCommand command);
}