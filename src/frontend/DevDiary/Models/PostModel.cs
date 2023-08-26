using System;

namespace ChristianSchulz.MultitenancyMonolith.Frontend.DevDiary.Models;

public class PostModel
{
    public required string Title { get; set; }

    public required DateTime DateTime { get; set; }

    public required string Text { get; set; }

    public required string Link { get; set; }

    public required string Tag { get; set; }

}
