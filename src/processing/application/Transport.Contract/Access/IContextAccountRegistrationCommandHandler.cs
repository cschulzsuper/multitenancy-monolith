﻿using ChristianSchulz.MultitenancyMonolith.Application.Access.Commands;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Access;

public interface IContextAccountRegistrationCommandHandler
{
    Task ConfirmAsync(ContextAccountRegistrationConfirmCommand command);

    Task RegisterAsync(ContextAccountRegistrationRegisterCommand command);
}