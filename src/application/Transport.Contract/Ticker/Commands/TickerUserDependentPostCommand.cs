using ChristianSchulz.MultitenancyMonolith.ObjectValidation.Ticker.ConcreteAnnotations;
using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker.Commands;

public class TickerUserDependentPostCommand
{
    [Display(Name = "text")]
    [TickerMessageText]
    public required string Text { get; set; }
}