using ChristianSchulz.MultitenancyMonolith.ObjectValidation.Ticker.ConcreteAnnotations;
using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedAnnotations;
using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker.Requests;

public class TickerMessageRequest
{
    [Display(Name = "text")]
    [TickerMessageText]
    public required string Text { get; set; }

    [Display(Name = "priority")]
    [TickerMessagePriority]
    public required string Priority { get; set; }

    [MailAddress]
    public required string TickerUser { get; set; }

    public required string Object { get; set; }
}