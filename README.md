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
./Start-FullStack.ps1           # uses docker-compose.full-stack.yml

# Or with local Ollama LLM (no external AI provider needed)
./Start-FullStack-Ollama.ps1    # uses docker-compose.ollama.yml
```

> **Note on local AI model:** The Ollama stack uses `lfm2.5-thinking` by default — a small, lightweight model chosen for easy local demo setup. Because of its limited size it can occasionally produce inaccurate or incomplete responses. For production-quality answers, configure an external provider (`OpenAI` or `Gemini`) via the `AI__Provider` environment variable.

### Dependencies only (for native local dev)

```powershell
./Start-Deps.ps1    # uses docker-compose.deps.yml
# Then run each service individually with dotnet run / yarn dev
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

## Screenshots

### Login (Keycloak)

![Login page](docs/screenshots/login%20page.jpg)

### Chat UI — new conversation

![New chat screen](docs/screenshots/new%20chat%20screen%20having%20some%20template%20messages.jpg)

### Chat UI — live exchange rate query

![Asking 1 USD to EUR](docs/screenshots/request%201%20usd%20to%20eur%20in%20ai%20chat.jpg)

### Copying the JWT token for direct API access

![Copy Token button](docs/screenshots/Copy%20Token%20button.jpg)

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

### AI Agent API — Layers _(bonus)_

```
WebApi → Application → Domain
              ↓
         Infrastructure → Microsoft.Extensions.AI (LLM)
              ↓                     ↓
           Redis              CurrencyConverterPlugin
         (chat history)       (AI tool → Currency API)
```

The AI Agent API follows the same Clean Architecture layering as the Currency API. Its Infrastructure layer wires up two AI tools that the LLM can invoke during a conversation:

- **`CurrencyConverterPlugin`** — calls the Currency API using the [published `Client` NuGet package](Practice.Backend.CurrencyConverter/src/Client/) (`ICurrencyConverterClient`), giving the LLM access to latest rates, conversion, and historical data without duplicating any HTTP logic.
- **`DatePlugin`** — supplies the LLM with the current date, enabling accurate reasoning about historical queries.

Conversation history is persisted per-session in Redis so the agent maintains context across multiple turns. The LLM provider is swappable at runtime via `AI__Provider` (supports `Ollama`, `OpenAI`, and `Gemini`).

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

Four workflows run on pull requests and pushes to `develop` / `main`:

| Workflow | Triggers on | Jobs |
|---|---|---|
| **Backend · Currency Converter** | Changes under `Practice.Backend.CurrencyConverter/` | Build → Unit tests (per layer, with coverage) → Integration tests (Redis service) → Publish `Client` + `Messages` NuGet packages (pre-release on `develop`, stable on `main` + GitHub Release) |
| **Chatbot · Currency Converter** | Changes under `Practice.Chatbot.CurrencyConverter/` | Build → Unit tests → Integration tests |
| **Web · Currency Converter** | Changes under `Practice.Web.CurrencyConverter/` | Install → Lint → Build |
| **Root · Infra** | Changes to root `*.md`, `*.ps1`, `docker-compose*.yml`, `keycloak/`, `docs/`, or the workflow file itself | No-op acknowledgement (infra-only change, no service build needed) |

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
- Historical rate pagination is computed server-side: business days for the full requested range are calculated locally, then only the date window for the requested page is forwarded to Frankfurter — avoiding over-fetching.
- Keycloak is the identity provider for demo purposes only and is fully replaceable.

### Endpoint Authorization

| Service | Method | Route | Required Role |
|---|---|---|---|
| Currency API | `GET` | `/api/v1/exchange-rate/latest` | `currency:read` |
| Currency API | `GET` | `/api/v1/exchange-rate/conversion` | `currency:read` |
| Currency API | `GET` | `/api/v1/exchange-rate/historical` | `currency:admin` |
| AI Agent API | `POST` | `/api/v1/chat/message` | `ai:chat` |
| AI Agent API | `GET` | `/api/v1/chat/{conversationId}/history` | `ai:chat` |

The `currency:admin` role is intentionally restricted to historical rates as this endpoint exposes broader data access (paginated multi-day ranges). Both demo users have `currency:read` and `ai:chat`; only `admin@currencyconverter.com` has `currency:admin`.

---

## Possible Future Enhancements

- Plug in additional exchange-rate providers (e.g., Open Exchange Rates) via the existing factory.
- AI-powered exchange rate insights — extend the AI agent with a forecasting tool that analyses historical rate trends and provides conversion timing recommendations (e.g. "EUR/USD has trended down 2% this week — you may get a better rate by waiting").
