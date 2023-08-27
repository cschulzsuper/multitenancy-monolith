using System;

namespace ChristianSchulz.MultitenancyMonolith.Frontend.DevLog.Services.Documentation.Models;

public class DevelopmentPostModel
{
    public required string Title { get; set; }

    public required DateTime DateTime { get; set; }

    public required string Text { get; set; }

    public required string Link { get; set; }

    public required string Tag { get; set; }

}
