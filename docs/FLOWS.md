# Flows

Notes on some flows that are implemented.

## `TickerUser`

### Secret Flow

1. `TickerUser` is created
    1. `SecretState` is set to `invalid`
1. `TickerUser` is reset
    1. `SecretState` is set to `reset`
1. `TickerUser` authenticates on `/auth`
    1. `Secret` is set to passed value
    1. `SecretState` is set to `pending`
    1. `SecretToken` is set to `Guid.NewGuid()`
1. `TickerUser` recieves `SecretToken` (not implemented)
1. `TickerUser` confirms `SecretToken` on `/confirm`
    1. `SecretState` is set to `confirmed`
