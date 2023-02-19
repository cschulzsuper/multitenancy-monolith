﻿using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedAnnotations;
using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker.Commands;

public class TickerUserAuthCommand
{

    [Required]
    [StringLength(140)]
    public required string Client { get; init; }

    [UniqueName]
    public required string Group { get; init; }

    [MailAddress]
    public required string Mail { get; init; }

    [Secret]
    public required string Secret { get; init; }
}