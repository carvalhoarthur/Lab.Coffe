---
name: Lab.Coffe - Clean Architecture Setup
overview: Criar uma solução .NET Core 8 seguindo Clean Architecture com suporte a SQL Server (Dapper), RabbitMQ, Swagger, observabilidade (ELK/Kibana) e testes unitários, preparada para deployment em Kubernetes.
todos:
  - id: create-solution-structure
    content: Criar solução .NET 8 e todos os projetos (Domain, Application, Infrastructure, API, Tests) com referências corretas
    status: completed
  - id: setup-domain-layer
    content: Implementar camada Domain com entidades base, value objects e interfaces de domínio
    status: completed
    dependencies:
      - create-solution-structure
  - id: setup-application-layer
    content: Implementar camada Application com interfaces, DTOs, casos de uso, validações e mapeamentos
    status: completed
    dependencies:
      - setup-domain-layer
  - id: setup-infrastructure-persistence
    content: Implementar repositórios com Dapper e SQL Server, incluindo factories e configurações
    status: completed
    dependencies:
      - setup-application-layer
  - id: setup-infrastructure-messaging
    content: Implementar publicação de mensagens no RabbitMQ com configurações e publishers
    status: completed
    dependencies:
      - setup-application-layer
  - id: setup-observability
    content: Configurar Serilog com sink para Elasticsearch, correlation IDs e logging estruturado
    status: completed
    dependencies:
      - setup-infrastructure-persistence
  - id: setup-api-layer
    content: Configurar API com Swagger, controllers, middleware de tratamento de erros e health checks
    status: completed
    dependencies:
      - setup-infrastructure-persistence
      - setup-infrastructure-messaging
      - setup-observability
  - id: setup-docker-kubernetes
    content: Criar Dockerfile multi-stage e configurações Kubernetes (deployment, service, configmap, secrets)
    status: completed
    dependencies:
      - setup-api-layer
  - id: setup-unit-tests
    content: Criar estrutura de testes unitários com xUnit, Moq e FluentAssertions com exemplos de testes
    status: completed
    dependencies:
      - setup-application-layer
  - id: create-documentation
    content: Criar README.md completo com documentação da arquitetura, setup local, deploy e configurações
    status: completed
    dependencies:
      - setup-docker-kubernetes
      - setup-unit-tests
---

# Lab.Coffe - Projeto Backend .NET Core 8 com Clean Architecture

## Arquitetura e Estrutura de Camadas

A solução seguirá os princípios de Clean Architecture, com dependências apontando para dentro (camadas mais externas dependem das mais internas).

### Estrutura de Camadas e Dependências

```
┌─────────────────────────────────────────────────┐
│   Presentation Layer (Lab.Coffe.API)            │
│   - Controllers                                  │
│   - Swagger Configuration                        │
│   - Middleware (Error Handling, Logging)        │
└─────────────────┬───────────────────────────────┘
                  │ depende de
┌─────────────────▼───────────────────────────────┐
│   Infrastructure Layer (Lab.Coffe.Infrastructure)│
│   - Dapper Repositories                         │
│   - RabbitMQ Publishers                         │
│   - DbContext/SqlConnection                     │
│   - External Services                           │
│   - Logging (Serilog → Elasticsearch)           │
└─────────────────┬───────────────────────────────┘
                  │ depende de
┌─────────────────▼───────────────────────────────┐
│   Application Layer (Lab.Coffe.Application)     │
│   - Use Cases / Handlers                        │
│   - DTOs (Request/Response)                     │
│   - Interfaces (IRepository, IMessagePublisher) │
│   - Validation                                  │
│   - AutoMapper Profiles                         │
└─────────────────┬───────────────────────────────┘
                  │ depende de
┌─────────────────▼───────────────────────────────┐
│   Domain Layer (Lab.Coffe.Domain)               │
│   - Entities                                    │
│   - Value Objects                               │
│   - Domain Events                               │
│   - Interfaces (Domain)                         │
└─────────────────────────────────────────────────┘
```

### Projetos da Solução

1. **Lab.Coffe.Domain** (Class Library)

   - Camada mais interna, sem dependências externas
   - Contém entidades, value objects e interfaces de domínio

2. **Lab.Coffe.Application** (Class Library)

   - Depende apenas de: `Lab.Coffe.Domain`
   - Contém casos de uso, DTOs e interfaces de aplicação

3. **Lab.Coffe.Infrastructure** (Class Library)

   - Depende de: `Lab.Coffe.Application` e `Lab.Coffe.Domain`
   - Implementa interfaces definidas na camada Application

4. **Lab.Coffe.API** (Web API)

   - Depende de: `Lab.Coffe.Application`, `Lab.Coffe.Infrastructure`
   - Não deve referenciar diretamente `Lab.Coffe.Domain`

5. **Lab.Coffe.Tests** (xUnit Test Project)

   - Depende de todos os projetos para testes

## Plano de Execução Detalhado

### Fase 1: Estrutura Base da Solução

#### 1.1 Criar Solução e Projetos

- Criar solução `.sln`
- Criar projetos:
  - `Lab.Coffe.Domain` (Class Library, .NET 8.0)
  - `Lab.Coffe.Application` (Class Library, .NET 8.0)
  - `Lab.Coffe.Infrastructure` (Class Library, .NET 8.0)
  - `Lab.Coffe.API` (ASP.NET Core Web API, .NET 8.0)
  - `Lab.Coffe.Tests` (xUnit Test Project, .NET 8.0)

#### 1.2 Configurar Referências entre Projetos

- `Application` → `Domain`
- `Infrastructure` → `Application`, `Domain`
- `API` → `Application`, `Infrastructure`
- `Tests` → Todos os projetos

### Fase 2: Domain Layer (Core)

#### 2.1 Estrutura Base do Domain

- Criar pasta `Entities` com exemplo de entidade base
- Criar pasta `ValueObjects` para objetos de valor
- Criar pasta `Interfaces` para contratos de domínio
- Criar pasta `Events` para domain events (opcional)

#### 2.2 Entidades de Exemplo

- Criar `BaseEntity` com Id, CreatedAt, UpdatedAt
- Criar exemplo de entidade (ex: `Coffee` ou `Product`)

### Fase 3: Application Layer

#### 3.1 Estrutura Base da Application

- Criar pasta `Interfaces` para:
  - `IRepository<T>` (genérico)
  - `IMessagePublisher` (para RabbitMQ)
  - `IUnitOfWork` (para transações)
- Criar pasta `DTOs` para Request/Response
- Criar pasta `UseCases` ou `Features` (CQRS-style)
- Criar pasta `Mappings` para AutoMapper profiles
- Criar pasta `Validators` para FluentValidation

#### 3.2 Configurações

- Adicionar pacotes NuGet:
  - `AutoMapper`
  - `FluentValidation`
  - `MediatR` (opcional, para CQRS)

### Fase 4: Infrastructure Layer

#### 4.1 Configuração de Banco de Dados (SQL Server + Dapper)

- Criar pasta `Persistence/Repositories`
- Implementar `Repository<T>` usando Dapper
- Criar pasta `Persistence/Dapper` com:
  - `IDbConnectionFactory`
  - `SqlServerConnectionFactory`
- Configurar Connection Strings em `appsettings.json`
- Adicionar pacotes:
  - `Dapper`
  - `System.Data.SqlClient` ou `Microsoft.Data.SqlClient`

#### 4.2 Configuração RabbitMQ

- Criar pasta `Messaging`
- Implementar `RabbitMQMessagePublisher` (implementa `IMessagePublisher`)
- Criar `RabbitMQConfiguration`
- Adicionar pacote:
  - `RabbitMQ.Client`

#### 4.3 Configuração de Logging (Observabilidade - ELK/Kibana)

- Configurar Serilog
- Configurar sink para Elasticsearch
- Criar `SerilogExtensions`
- Adicionar pacotes:
  - `Serilog.AspNetCore`
  - `Serilog.Sinks.Console`
  - `Serilog.Sinks.File`
  - `Serilog.Sinks.Elasticsearch`

#### 4.4 Dependency Injection

- Criar `InfrastructureServiceRegistration` para registrar serviços

### Fase 5: API Layer (Presentation)

#### 5.1 Configuração Base

- Configurar Swagger/OpenAPI:
  - Versão da API
  - XML Comments
  - Autenticação (se necessário)
- Configurar CORS
- Configurar Health Checks (importante para Kubernetes)

#### 5.2 Middleware

- Criar `ExceptionHandlingMiddleware` para tratamento global de erros
- Configurar logging de requisições/respostas
- Adicionar correlation IDs para rastreamento

#### 5.3 Controllers

- Criar controller base
- Criar exemplo de controller seguindo padrões REST
- Configurar AutoMapper no pipeline

#### 5.4 Configuração de Observabilidade

- Integrar Serilog no pipeline
- Configurar métricas para Prometheus (opcional, para Kubernetes)
- Adicionar correlation IDs nos logs

### Fase 6: Configuração para Cloud/Kubernetes

#### 6.1 Docker

- Criar `Dockerfile` multi-stage
- Criar `.dockerignore`
- Configurar para produção

#### 6.2 Kubernetes

- Criar `deployment.yaml`
- Criar `service.yaml`
- Criar `configmap.yaml` para configurações
- Criar `secret.yaml` para secrets (Connection Strings, etc.)
- Configurar probes (liveness, readiness)

#### 6.3 Configurações de Ambiente

- Criar `appsettings.Development.json`
- Criar `appsettings.Production.json`
- Configurar variáveis de ambiente para:
  - Connection Strings
  - RabbitMQ settings
  - Elasticsearch settings

### Fase 7: Testes Unitários

#### 7.1 Estrutura de Testes

- Criar pasta `Domain.Tests`
- Criar pasta `Application.Tests`
- Criar pasta `Infrastructure.Tests` (com mocks)
- Criar pasta `API.Tests` (integration tests)

#### 7.2 Configuração de Testes

- Adicionar pacotes:
  - `xUnit`
  - `Moq` (para mocks)
  - `FluentAssertions`
  - `AutoFixture` (opcional)

#### 7.3 Exemplos de Testes

- Testes de entidades de domínio
- Testes de casos de uso (Application)
- Testes de repositórios (com banco em memória ou mocks)
- Testes de controllers (com TestServer)

## Arquivos Principais a Criar

### Estrutura de Pastas Completa

```
Lab.Coffe/
├── src/
│   ├── Lab.Coffe.Domain/
│   │   ├── Entities/
│   │   ├── ValueObjects/
│   │   ├── Interfaces/
│   │   └── Events/
│   ├── Lab.Coffe.Application/
│   │   ├── Interfaces/
│   │   ├── DTOs/
│   │   ├── UseCases/ (ou Features/)
│   │   ├── Mappings/
│   │   └── Validators/
│   ├── Lab.Coffe.Infrastructure/
│   │   ├── Persistence/
│   │   │   ├── Dapper/
│   │   │   └── Repositories/
│   │   ├── Messaging/
│   │   └── Logging/
│   └── Lab.Coffe.API/
│       ├── Controllers/
│       ├── Middleware/
│       ├── Program.cs
│       └── appsettings.json
├── tests/
│   └── Lab.Coffe.Tests/
├── k8s/
│   ├── deployment.yaml
│   ├── service.yaml
│   ├── configmap.yaml
│   └── secret.yaml
├── Dockerfile
├── .dockerignore
└── Lab.Coffe.sln
```

## Dependências NuGet Principais

### Domain

- Nenhuma dependência externa

### Application

- AutoMapper
- FluentValidation
- MediatR (opcional)

### Infrastructure

- Dapper
- Microsoft.Data.SqlClient
- RabbitMQ.Client
- Serilog.AspNetCore
- Serilog.Sinks.Elasticsearch

### API

- Swashbuckle.AspNetCore (Swagger)
- Serilog.AspNetCore
- HealthChecks (Microsoft.Extensions.Diagnostics.HealthChecks)

### Tests

- xUnit
- Moq
- FluentAssertions
- Microsoft.AspNetCore.Mvc.Testing

## Observabilidade com ELK Stack

### Configuração

1. Serilog enviará logs para Elasticsearch
2. Kibana visualizará os logs
3. Correlation IDs permitirão rastrear requisições
4. Métricas opcionais via Prometheus para Kubernetes

### Integração

- Middleware para adicionar correlation IDs
- Serilog enriquecido com contexto HTTP
- Logs estruturados (JSON) para melhor parsing no Elasticsearch

## Documentação

Criar `README.md` com:

- Descrição do projeto
- Arquitetura explicada
- Como executar localmente
- Como fazer deploy no Kubernetes
- Como configurar RabbitMQ e SQL Server
- Como visualizar logs no Kibana