﻿using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker.Requests;

public class TickerUserRequest
{
    [MailAddress]
    public required string MailAddress { get; set; }

    [DisplayName]
    public required string DisplayName { get; set; }
}