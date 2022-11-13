using System;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authentication;

public record User(string Username, string Password)
{
    public byte[] Verification { get; set; } = Guid.NewGuid().ToByteArray();
}
