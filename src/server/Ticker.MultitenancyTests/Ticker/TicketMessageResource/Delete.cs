﻿using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Ticker;
using ChristianSchulz.MultitenancyMonolith.Server.Ticker;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Ticker.TicketMessageResource;

public sealed class Delete : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public Delete(WebApplicationFactory<Program> factory)
    {
        _factory = factory.Mock();
    }

    [Fact]
    public async Task Delete_ShouldRespectMultitenancy_WhenSuccessful()
    {
            // Arrange
            var existingTickerMessage = new TickerMessage
            {
                Snowflake = 1,
                Text = $"existing-ticker-message-{Guid.NewGuid()}",
                Priority = "default",
                TickerUser = $"{Guid.NewGuid()}@localhost",
                Timestamp = 0
            };

            using (var scope = _factory.CreateMultitenancyScope(MockWebApplication.Group1))
            {
                scope.ServiceProvider
                    .GetRequiredService<IRepository<TickerMessage>>()
                    .Insert(existingTickerMessage);
            }

            using (var scope = _factory.CreateMultitenancyScope(MockWebApplication.Group2))
            {
                scope.ServiceProvider
                    .GetRequiredService<IRepository<TickerMessage>>()
                    .Insert(existingTickerMessage);
            }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/ticker/ticker-messages/{existingTickerMessage.Snowflake}");
            request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(MockWebApplication.Group1);

            var postTickerMessage = new
            {
                Text = $"post-ticker-message-{Guid.NewGuid()}",
                Priority = "low",
                TickerUser = MockWebApplication.Mail
            };

            request.Content = JsonContent.Create(postTickerMessage);

            var client = _factory.CreateClient();

            // Act
            var response = await client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            using (var scope = _factory.CreateMultitenancyScope(MockWebApplication.Group2))
            {
                var updatedTickerMessage = scope.ServiceProvider
                    .GetRequiredService<IRepository<TickerMessage>>()
                    .GetQueryable()
                    .SingleOrDefault();

                Assert.NotNull(updatedTickerMessage);
                Assert.Equal(existingTickerMessage.TickerUser, updatedTickerMessage.TickerUser);
                Assert.Equal(existingTickerMessage.Text, updatedTickerMessage.Text);
                Assert.Equal(existingTickerMessage.Priority, updatedTickerMessage.Priority);
                Assert.Equal(existingTickerMessage.Snowflake, updatedTickerMessage.Snowflake);
                Assert.Equal(existingTickerMessage.Timestamp, updatedTickerMessage.Timestamp);
            }
        }
}