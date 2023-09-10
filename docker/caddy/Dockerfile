FROM caddy:latest AS base

COPY Caddyfile /etc/caddy/Caddyfile

COPY multitenancy-monolith.crt /etc/caddy/cert/multitenancy-monolith.crt
COPY multitenancy-monolith.rsa /etc/caddy/cert/multitenancy-monolith.rsa