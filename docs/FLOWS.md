# Flows

Notes on some flows that are implemented.

## `TickerUser`

### Confirm Flow

1. `TickerUser` is created
    1. `SecretState` is set to `temporary`
1. `TickerUser` authenticates on `/auth`
    1. `Secret` is set to passed value
    1. `SecretState` is set to `pending`
    1. `SecretToken` is set to `Guid.NewGuid()`
1. `TickerUser` recieves `SecretToken` (not implemented)
1. `TickerUser` confirms `SecretToken` on `/confirm`
    1. `SecretState` is set to `pending`
