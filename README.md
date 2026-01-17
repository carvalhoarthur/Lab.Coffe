# Lab.Coffe - Backend API

Backend API desenvolvido em .NET Core 8 seguindo os princípios de Clean Architecture, com suporte a SQL Server (Dapper), RabbitMQ, observabilidade (ELK/Kibana) e testes unitários.

## Arquitetura

Este projeto segue os princípios de **Clean Architecture**, organizando o código em camadas independentes:

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
│   - SQL Server Connection                       │
│   - Logging (Serilog → Elasticsearch)           │
└─────────────────┬───────────────────────────────┘
                  │ depende de
┌─────────────────▼───────────────────────────────┐
│   Application Layer (Lab.Coffe.Application)     │
│   - Use Cases / Handlers (CQRS)                 │
│   - DTOs (Request/Response)                     │
│   - Interfaces (IRepository, IMessagePublisher) │
│   - Validation (FluentValidation)               │
│   - AutoMapper Profiles                         │
└─────────────────┬───────────────────────────────┘
                  │ depende de
┌─────────────────▼───────────────────────────────┐
│   Domain Layer (Lab.Coffe.Domain)               │
│   - Entities                                    │
│   - Value Objects                               │
│   - Domain Events                               │
└─────────────────────────────────────────────────┘
```

### Estrutura de Projetos

- **Lab.Coffe.Domain**: Camada de domínio (entidades, value objects, eventos)
- **Lab.Coffe.Application**: Casos de uso, DTOs, validações, interfaces
- **Lab.Coffe.Infrastructure**: Implementações (Dapper, RabbitMQ, Serilog)
- **Lab.Coffe.API**: Camada de apresentação (Controllers, Middleware, Swagger)
- **Lab.Coffe.Tests**: Testes unitários

## Requisitos

- .NET 8.0 SDK
- SQL Server (LocalDB ou SQL Server)
- RabbitMQ
- Elasticsearch (para observabilidade)
- Docker (opcional, para containers)

## Configuração Local

### 1. Banco de Dados (SQL Server)

Crie o banco de dados:

```sql
CREATE DATABASE LabCoffe;
```

Configure a connection string no `appsettings.json` ou `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=LabCoffe;User Id=sa;Password=YourPassword123!;TrustServerCertificate=true;"
  }
}
```

Crie a tabela de exemplo:

```sql
CREATE TABLE Coffees (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    Description NVARCHAR(1000) NOT NULL,
    Price DECIMAL(18,2) NOT NULL,
    Stock INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL,
    UpdatedAt DATETIME2 NULL
);
```

### 2. RabbitMQ

Instale e inicie o RabbitMQ:

```bash
# Docker
docker run -d --hostname rabbitmq --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management

# Acesse a interface de gerenciamento: http://localhost:15672
# Usuário padrão: guest / guest
```

Configure no `appsettings.json`:

```json
{
  "RabbitMQ": {
    "HostName": "localhost",
    "Port": 5672,
    "UserName": "guest",
    "Password": "guest",
    "VirtualHost": "/",
    "ExchangeName": "lab.coffe"
  }
}
```

### 3. Elasticsearch (Observabilidade)

Instale e inicie o Elasticsearch:

```bash
# Docker
docker run -d --name elasticsearch -p 9200:9200 -p 9300:9300 -e "discovery.type=single-node" elasticsearch:7.17.0

# Verifique se está rodando
curl http://localhost:9200
```

Opcionalmente, instale o Kibana:

```bash
docker run -d --name kibana -p 5601:5601 --link elasticsearch:elasticsearch -e "ELASTICSEARCH_HOSTS=http://elasticsearch:9200" kibana:7.17.0
```

Configure no `appsettings.json`:

```json
{
  "Serilog": {
    "Elasticsearch": {
      "Uri": "http://localhost:9200",
      "IndexFormat": "lab-coffe-logs-{0:yyyy.MM.dd}"
    }
  }
}
```

## Executando Localmente

### 1. Restaurar Dependências

```bash
dotnet restore
```

### 2. Executar a API

```bash
cd src/Lab.Coffe.API
dotnet run
```

A API estará disponível em:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`
- Swagger: `http://localhost:5000/swagger` (ou `https://localhost:5001/swagger`)

### 3. Executar Testes

```bash
dotnet test
```

## Docker

### Build da Imagem

```bash
docker build -t lab-coffe-api:latest .
```

### Executar Container

```bash
docker run -d \
  --name lab-coffe-api \
  -p 8080:8080 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e ConnectionStrings__DefaultConnection="Server=host.docker.internal;Database=LabCoffe;User Id=sa;Password=YourPassword123!;TrustServerCertificate=true;" \
  -e RabbitMQ__HostName=host.docker.internal \
  -e Serilog__Elasticsearch__Uri="http://host.docker.internal:9200" \
  lab-coffe-api:latest
```

## Kubernetes

### Pré-requisitos

- Cluster Kubernetes configurado
- kubectl instalado e configurado

### Deploy

1. Aplique os recursos em ordem:

```bash
# ConfigMap
kubectl apply -f k8s/configmap.yaml

# Secrets (edite antes de aplicar)
kubectl apply -f k8s/secret.yaml

# Deployment
kubectl apply -f k8s/deployment.yaml

# Service
kubectl apply -f k8s/service.yaml
```

2. Verifique o status:

```bash
kubectl get pods
kubectl get services
kubectl logs -f deployment/lab-coffe-api
```

3. Health Checks:

```bash
# Health check geral
curl http://<service-ip>/health

# Readiness probe
curl http://<service-ip>/health/ready

# Liveness probe
curl http://<service-ip>/health/live
```

## Endpoints

### Coffee Endpoints

- `GET /api/coffee` - Listar todos os cafés
- `GET /api/coffee/{id}` - Obter café por ID
- `POST /api/coffee` - Criar novo café
- `PUT /api/coffee/{id}` - Atualizar café
- `DELETE /api/coffee/{id}` - Deletar café

### Health Checks

- `GET /health` - Health check geral
- `GET /health/ready` - Readiness probe
- `GET /health/live` - Liveness probe

### Swagger

- `GET /swagger` - Documentação Swagger UI

## Observabilidade

### Logs

Os logs são enviados para:
- **Console**: Logs estruturados no console
- **Arquivo**: Logs em `logs/lab-coffe-{date}.log`
- **Elasticsearch**: Logs indexados para visualização no Kibana

### Correlation ID

Cada requisição recebe um Correlation ID único no header `X-Correlation-ID`, permitindo rastrear todas as operações relacionadas.

### Visualizando Logs no Kibana

1. Acesse o Kibana: `http://localhost:5601`
2. Vá em **Discover**
3. Crie um index pattern: `lab-coffe-logs-*`
4. Visualize os logs com filtros por Correlation ID, nível de log, etc.

## Estrutura de Pastas

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
│   │   ├── UseCases/
│   │   ├── Mappings/
│   │   └── Validators/
│   ├── Lab.Coffe.Infrastructure/
│   │   ├── Persistence/
│   │   ├── Messaging/
│   │   └── Logging/
│   └── Lab.Coffe.API/
│       ├── Controllers/
│       ├── Middleware/
│       └── Program.cs
├── tests/
│   └── Lab.Coffe.Tests/
├── k8s/
│   ├── deployment.yaml
│   ├── service.yaml
│   ├── configmap.yaml
│   └── secret.yaml
├── Dockerfile
└── README.md
```

## Tecnologias Utilizadas

### Core
- **.NET 8.0**: Framework principal
- **ASP.NET Core**: Framework web

### Persistência
- **Dapper**: Micro-ORM para acesso a dados
- **Microsoft.Data.SqlClient**: Driver SQL Server

### Mensageria
- **RabbitMQ.Client**: Cliente RabbitMQ

### Observabilidade
- **Serilog**: Framework de logging
- **Serilog.Sinks.Elasticsearch**: Sink para Elasticsearch
- **Serilog.AspNetCore**: Integração com ASP.NET Core

### Validação e Mapeamento
- **FluentValidation**: Validação de DTOs
- **AutoMapper**: Mapeamento objeto-objeto

### Testes
- **xUnit**: Framework de testes
- **Moq**: Framework de mocking
- **FluentAssertions**: Assertions mais legíveis

### Documentação
- **Swashbuckle.AspNetCore**: Swagger/OpenAPI

## Padrões Utilizados

- **Clean Architecture**: Separação em camadas
- **CQRS**: Separação entre Commands e Queries (usando MediatR)
- **Repository Pattern**: Abstração de acesso a dados
- **Dependency Injection**: Inversão de controle
- **Middleware Pattern**: Tratamento de erros e logging

## Contribuindo

1. Faça um fork do projeto
2. Crie uma branch para sua feature (`git checkout -b feature/MinhaFeature`)
3. Commit suas mudanças (`git commit -m 'Adiciona MinhaFeature'`)
4. Push para a branch (`git push origin feature/MinhaFeature`)
5. Abra um Pull Request

## Licença

Este projeto está sob a licença MIT.
