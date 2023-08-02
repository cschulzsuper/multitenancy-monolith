using ChristianSchulz.MultitenancyMonolith.Application.Ticker.Requests;
using ChristianSchulz.MultitenancyMonolith.Application.Ticker.Responses;
using ChristianSchulz.MultitenancyMonolith.Objects.Ticker;
using ChristianSchulz.MultitenancyMonolith.ObjectValidation.Ticker.ConcreteValidators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker;

internal sealed class TickerUserRequestHandler : ITickerUserRequestHandler
{
    private readonly ITickerUserManager _tickerUserManager;

    public TickerUserRequestHandler(ITickerUserManager tickerUserManager)
    {
        _tickerUserManager = tickerUserManager;
    }

    public async Task<TickerUserResponse> GetAsync(long tickerUser)
    {
        var @object = await _tickerUserManager.GetAsync(tickerUser);

        var response = new TickerUserResponse
        {
            Snowflake = @object.Snowflake,
            MailAddress = @object.MailAddress,
            DisplayName = @object.DisplayName,
            SecretState = @object.SecretState
        };

        return response;
    }

    public async IAsyncEnumerable<TickerUserResponse> GetAll(string? query, int? skip, int? take)
    {
        var objects = _tickerUserManager.GetAsyncEnumerable(
            query =>
            {
                query = skip != null ? query.Skip(skip.Value) : query;
                query = take != null ? query.Take(take.Value) : query;

                return query;
            });

        await foreach (var @object in objects)
        {
            var response = new TickerUserResponse
            {
                Snowflake = @object.Snowflake,
                MailAddress = @object.MailAddress,
                DisplayName = @object.DisplayName,
                SecretState = @object.SecretState
            };

            yield return response;
        }
    }

    public async Task<TickerUserResponse> InsertAsync(TickerUserRequest request)
    {
        var @object = new TickerUser
        {
            MailAddress = request.MailAddress,
            DisplayName = request.DisplayName,
            Secret = $"{Guid.NewGuid()}",
            SecretToken = Guid.NewGuid(),
            SecretState = TickerUserSecretStates.Invalid
        };

        await _tickerUserManager.InsertAsync(@object);

        var response = new TickerUserResponse
        {
            Snowflake = @object.Snowflake,
            MailAddress = @object.MailAddress,
            DisplayName = @object.DisplayName,
            SecretState = @object.SecretState
        };

        return response;
    }

    public async Task UpdateAsync(long tickerUser, TickerUserRequest request)
    {
        await _tickerUserManager.UpdateAsync(tickerUser,
            @object =>
            {
                @object.MailAddress = request.MailAddress;
                @object.DisplayName = request.DisplayName;
            });
    }

    public async Task DeleteAsync(long tickerUser)
        => await _tickerUserManager.DeleteAsync(tickerUser);
}