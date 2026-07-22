# Unit Tests Types

Este repositório contém uma solução .NET com arquitetura em camadas, API Web, testes unitários, testes de integração com Testcontainers, Docker Compose para PostgreSQL, RabbitMQ e Redis, e uma suíte de performance com k6.

## Estrutura

- src/UnitTestsTypes.Api: Web API
- src/UnitTestsTypes.Application: serviços de aplicação
- src/UnitTestsTypes.Domain: entidades e contratos de domínio
- src/UnitTestsTypes.Infrastructure: repositórios, Dapper, RabbitMQ e DI
- tests/UnitTestsTypes.UnitTests: testes unitários
- tests/UnitTestsTypes.IntegrationTests: testes de integração com Testcontainers
- performance/k6: scripts de performance

## Requisitos

- .NET SDK 10
- Docker Desktop ou Docker Engine
- Optional: k6

## Executar ambiente local com Docker

```bash
docker compose up -d
```

## Executar a API

```bash
dotnet run --project src/UnitTestsTypes.Api/UnitTestsTypes.Api.csproj
```

## Executar testes

```bash
dotnet test tests/UnitTestsTypes.UnitTests/UnitTestsTypes.UnitTests.csproj

dotnet test tests/UnitTestsTypes.IntegrationTests/UnitTestsTypes.IntegrationTests.csproj
```

## Executar testes de mutação com Stryker

```bash
dotnet tool install --global dotnet-stryker

dotnet stryker --configFile stryker-config.json
```

## Executar testes de performance com k6

```bash
k6 run performance/k6/customer-api.js
```

## Formatação

```bash
dotnet tool install --global dotnet-format
dotnet-format --folder .
```

## Fluxo de CI/CD

O repositório inclui workflows em [.github/workflows/ci.yml](.github/workflows/ci.yml) e [.github/workflows/merge-to-main.yml](.github/workflows/merge-to-main.yml).
