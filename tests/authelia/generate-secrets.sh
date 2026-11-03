#!/bin/sh
# Generates Authelia's TLS certificate, OIDC signing key and secrets into
# /secrets on first run, so nothing secret is committed. Runs in the Authelia
# image as a one-shot compose service.
set -eu

if [ -f /secrets/tls/public.crt ]; then
  echo "Secrets already exist."
  exit 0
fi

mkdir -p /secrets/tls /secrets/jwks /secrets/trust

authelia crypto certificate rsa generate \
  --common-name auth.localtest.me \
  --sans auth.localtest.me \
  --directory /secrets/tls

# A directory holding only the certificate, for the web app to trust.
cp /secrets/tls/public.crt /secrets/trust/authelia.crt

authelia crypto pair rsa generate --directory /secrets/jwks

for name in session storage hmac jwt; do
  authelia crypto rand --length 64 --charset alphanumeric --file "/secrets/$name"
done

# Readable by the non-root web and Authelia processes.
chmod -R a+rX /secrets
echo "Secrets generated."
