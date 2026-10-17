# DigitalOcean preview deployment

The Dockerfile builds the .NET 10 API on Linux and runs the published output on the ASP.NET Core runtime image as the non-root `app` user. `.do/app.yaml` is a preview App Platform spec: it uses this feature branch, disables deploy-on-push, and does not claim a production domain.

Build from the repository root:

```sh
docker build -t dcws-api:preview .
docker run --rm -p 8080:8080 \
  -e ConnectionStrings__DefaultConnection='<SQL Server connection string>' \
  -e Smtp__TenantId='<tenant>' -e Smtp__ClientId='<client>' \
  -e Smtp__ClientSecret='<secret>' dcws-api:preview
```

Set actual values in DigitalOcean runtime environment settings, marking credentials as secrets. Required settings:

| Setting | Purpose |
| --- | --- |
| `ConnectionStrings__DefaultConnection` | Existing SQL Server database connection; use encryption and certificate validation |
| `Smtp__TenantId`, `Smtp__ClientId`, `Smtp__ClientSecret` | Microsoft 365 SMTP OAuth application |
| `Smtp__Username`, `Smtp__FromAddress`, `Smtp__FromName` | Sender identity; defaults are in appsettings.json |
| `Testimonials__NotificationEmail` | Review destination |
| `Cors__AllowedOrigins__0` | Preview MVC HTTPS origin, if preview requests need CORS |

The API has a database-independent `/health/live` endpoint for platform health checks. `/api/status/get` remains the database check and should be tested separately with real settings. The existing SQL Server database is not migrated by this branch. Confirm connectivity, firewall rules, TLS, and credentials before deploying. The SMTP options validator requires nonempty credentials at startup.

App Platform terminates TLS and forwards HTTP to port 8080. Before exposing the API publicly, verify HTTPS handling and client IP rate limits behind the platform proxy. The current rate limiter uses `RemoteIpAddress`; proxy behavior can cause multiple visitors to share a limit. Do not trust arbitrary forwarded IP headers without a verified proxy boundary. Keep Azure deployment and production DNS in place during preview testing.

Security review: MailKit is updated to 4.18.1 and the transitive PKCS package is pinned to patched 10.0.13. Keep rebuilding against patched .NET 10 base images. The unit suite runs on Linux in branch CI.
