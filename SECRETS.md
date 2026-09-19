# Secret configuration

This project reads secrets from standard .NET configuration providers. Environment
variables override `appsettings.json`; nested configuration keys use double
underscores.

| Configuration key | Environment variable |
| --- | --- |
| `ConnectionStrings:DefaultConnection` | `ConnectionStrings__DefaultConnection` |
| `AWS:AccessKey` | `AWS__AccessKey` |
| `AWS:SecretKey` | `AWS__SecretKey` |

For local development, use .NET User Secrets (they are stored outside the
repository):

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<SQL Server connection string>"
dotnet user-secrets set "AWS:AccessKey" "<AWS access key ID>"
dotnet user-secrets set "AWS:SecretKey" "<AWS secret access key>"
```

For deployed environments, configure the equivalent environment variables in
the hosting platform or use its managed secret store. When the AWS keys are not
configured, the S3 client uses the AWS SDK default credential chain, which
supports IAM roles and other managed credential providers.

The previously committed database and AWS credentials must be rotated: removing
them from the current files does not remove them from Git history.
