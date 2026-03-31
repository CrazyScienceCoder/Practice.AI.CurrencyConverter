# Currency Converter

A production-ready currency conversion platform built with Clean Architecture, DDD, and AI-powered chat.

> **Bonus:** Beyond the core API requirement, this submission includes two additional projects — an AI chatbot API and a Next.js chat UI — demonstrating the ability to build advanced, AI-powered applications on top of the base service.

---

## Services

| Service | Stack | Purpose |
|---|---|---|
| **Currency API** | .NET 10, C# 13 | Core exchange-rate REST API (Frankfurter-backed) |
| **AI Agent API** _(bonus)_ | .NET 10, Microsoft.Extensions.AI | LLM-powered agent that calls the Currency API as a tool |
| **Chat UI** _(bonus)_ | Next.js 14, TypeScript | Streaming chat interface with Keycloak login |
| **Keycloak** | Keycloak 25 | Identity provider (pre-configured realm, runs locally via Docker) |

---

## Quick Start

### Full stack (recommended)

```powershell
# Build & start all services (including UI, AI agent, observability stack)
./FullyRunAll.ps1

# Or with local Ollama LLM (no external AI provider needed)
./FullyRunAllWithOllama.ps1
```

### Dependencies only (for native local dev)

```powershell
./RunDepsOnly.ps1
# Then run each service individually with dotnet run / npm run dev
```

---

## Service URLs

| Service | URL |
|---|---|
| Chat UI | http://localhost:3000 |
| Currency API (Swagger) | http://localhost:9081/swagger |
| AI Agent API (Swagger) | http://localhost:9082/swagger |
| Keycloak Admin | http://localhost:8900 — `admin` / `admin` |
| Grafana | http://localhost:3001 |
| Jaeger | http://localhost:16686 |
| Prometheus | http://localhost:9090 |
| OpenSearch Dashboards | http://localhost:5601 |

---

## Authentication

Authentication is handled by **Keycloak** (running locally via Docker). The realm is auto-imported on container start — no manual setup required. In production, Keycloak can be swapped for any OIDC-compliant provider (Azure AD, Auth0, Okta, etc.) by updating the authority URL in configuration.

### Demo users

| User | Password | Roles |
|---|---|---|
| `admin@currencyconverter.com` | `Admin1234!` | `currency:read`, `currency:admin`, `ai:chat` |
| `guest@currencyconverter.com` | `Guest1234!` | `currency:read`, `ai:chat` |

### Using the Currency API directly

After logging in via the Chat UI, click the **"Copy Token"** button to copy your JWT access token. Paste it as a Bearer token in Swagger, Postman, or any HTTP client to interact with the Currency API directly.

---

## Technology Stack

| Area | Technology |
|---|---|
| Runtime | .NET 10, C# 13 |
| Web framework | ASP.NET Core 10 |
| AI orchestration | Microsoft.Extensions.AI |
| Frontend | Next.js 14, TypeScript, Tailwind CSS |
| Auth | Keycloak 25, NextAuth, JWT / RBAC |
| Messaging | MediatR (CQRS) |
| Caching | Redis |
| Resilience | `Microsoft.Extensions.Http.Resilience` (retry + circuit breaker) |
| Validation | FluentValidation |
| Logging | Serilog → OpenSearch |
| Tracing | OpenTelemetry → Jaeger |
| Metrics | Prometheus → Grafana |
| Containerisation | Docker, Docker Compose |

---

## Architecture & Design

### Domain-Driven Design

The Currency API is built around DDD principles:
- **Value objects** — `Currency`, `Amount`, `ExchangeDate` are immutable, self-validating, and encapsulate domain rules.
- **Domain policies** — `CurrencyPolicyBehavior` (MediatR pipeline) enforces forbidden currencies (TRY, PLN, THB, MXN) at the application boundary.
- **Trading calendar** — `TradingCalendar` encapsulates business-day logic.

### Applied Patterns

| Pattern | Where |
|---|---|
| Clean Architecture | All three .NET services |
| CQRS | Application layer via MediatR |
| Factory | `IExchangeRateProviderFactory` — selects provider per request |
| Decorator | Redis caching wraps the exchange-rate provider (Scrutor) |
| Pipeline Behaviour | Validation and currency policy via MediatR behaviours |
| Result type | `ErrorOr<T>` — no exception-driven control flow |
| Action Result Builder | Per-error-type HTTP response mapping |

### Currency API — Layers

```
WebApi → Application → Domain
              ↓
         Infrastructure → Frankfurter.ApiClient
              ↓
           Redis (cache)
```

Each layer has its own `src/` and `tests/` project. Infrastructure depends only on abstractions defined in Domain, keeping the core fully testable without external dependencies.

---

## Production Readiness

- **Multi-environment config** — `appsettings.{Environment}.json` + environment variable overrides; works across Dev, Test, and Prod.
- **Horizontal scaling** — stateless services; shared Redis for cache and chat history.
- **API versioning** — routes follow `/api/v{version}/exchange-rate/...`.
- **Rate limiting** — built-in ASP.NET Core rate limiter on all endpoints.
- **Observability** — structured logs (Serilog/OpenSearch), distributed traces (OpenTelemetry/Jaeger), and metrics (Prometheus/Grafana) wired out of the box.
- **Request logging** — client IP, JWT `ClientId`, HTTP method, endpoint, response code, and response time logged per request.
- **Resilience** — exponential-backoff retry + circuit breaker on all outbound HTTP calls to Frankfurter.
- **90%+ test coverage** — unit tests per layer plus integration tests (requires Redis).

---

## CI/CD (GitHub Actions)

Three workflows run on pull requests and pushes to `develop` / `main`, one per service:

| Workflow | Jobs |
|---|---|
| **Backend · Currency Converter** | Build → Unit tests (per layer, with coverage) → Integration tests (Redis service) → Publish `Client` + `Messages` NuGet packages (pre-release on `develop`, stable on `main` + GitHub Release) |
| **Chatbot · Currency Converter** | Build → Unit tests → Integration tests |
| **Web · Currency Converter** | Install → Lint → Build |

Coverage summaries are posted to the **GitHub Actions step summary** on every run — open any workflow run, select the relevant job, and scroll to the bottom of the summary to see per-layer coverage reports. Concurrent runs on the same branch are cancelled automatically.

---

## Running Tests

```bash
# Currency API — all tests
cd Practice.Backend.CurrencyConverter
dotnet test

# Integration tests (requires RunDepsOnly.ps1 first)
dotnet test tests/Integration.Tests

# With coverage report
dotnet test --collect:"XPlat Code Coverage"
```

Test coverage reports are also available in CI without any local setup — open the relevant **GitHub Actions** workflow run, go to the job summary, and the per-layer coverage breakdown is listed at the bottom of the page.

---

## Assumptions

- Frankfurter is the single exchange-rate source; the factory pattern allows adding providers without changing existing code.
- Forbidden currencies (TRY, PLN, THB, MXN) are enforced for both conversion and latest-rate requests.
- Historical rate pagination is handled in-process (Frankfurter returns the full period; results are sliced server-side).
- Keycloak is the identity provider for demo purposes only and is fully replaceable.

---

## Possible Future Enhancements

- Plug in additional exchange-rate providers (e.g., Open Exchange Rates) via the existing factory.
- AI-powered exchange rate insights — extend the AI agent with a forecasting tool that analyses historical rate trends and provides conversion timing recommendations (e.g. "EUR/USD has trended down 2% this week — you may get a better rate by waiting").
