# Thany-Marcus — Portal & Cloud

Server-side code for **Thany-Marcus**, a personal knowledge system that captures
notes from Obsidian, processes them with local AI models, and syncs the results
back to your vault. This repository holds the control-plane **Portal** and the
per-user **Cloud** backend; the Obsidian client lives in a separate repository.

## What's here

| Project | Stack | Role |
| --- | --- | --- |
| `src/ThanyMarcus.Portal.Api` | ASP.NET Core (.NET 10), Minimal APIs | Sign-in, provisioning of per-user clouds via Terraform, plugin-token issuance, serves the SPA. |
| `src/ThanyMarcus.Portal.Web` | SvelteKit + `adapter-static` | Admin/portal SPA, built static and served from `Portal.Api/wwwroot/` in production. |
| `src/ThanyMarcus.Portal.SagaWorker` | .NET worker | Runs the provisioning saga (durable, Postgres-backed). |
| `src/ThanyMarcus.Cloud.Api` | ASP.NET Core (.NET 10) | The service provisioned onto each user's cloud: ingest, the AI processing pipeline, and sync. |
| `src/ThanyMarcus.Shared` | .NET class library | Types shared between Portal and Cloud. |

`infra/docker/` holds the Dockerfiles and Compose definitions; `tests/` is split
into Docker-free unit tests and Testcontainers-backed integration tests for both
the Portal and the Cloud.

## Development

The Portal runs as a .NET API plus a SvelteKit dev server:

```bash
# terminal 1 — .NET API
dotnet watch --project src/ThanyMarcus.Portal.Api

# terminal 2 — SvelteKit dev server (Vite proxy → the API)
cd src/ThanyMarcus.Portal.Web
pnpm install
pnpm dev
```

`docker-compose.override.yml` provides a local Postgres for development, so no
`.env` file is required locally. For production, copy `.env.example` to `.env`
and fill in the values.

## Testing

xUnit v3 + Shouldly. Integration tests use Testcontainers and require a
Docker-compatible runtime (Docker Desktop, OrbStack, Colima, or Podman).

```bash
scripts/test.sh            # one-shot
scripts/test-watch.sh      # re-run on save
```

Each app has separate unit (`*.UnitTests`) and integration (`*.IntegrationTests`)
projects; the unit lanes run without Docker.

## Deployment

Both the Portal and the per-user Cloud run as Docker Compose stacks with
**nginx + certbot** terminating TLS locally on each host. The Portal's
operational database is a managed PostgreSQL instance; the Portal VM itself is
stateless and disposable.
