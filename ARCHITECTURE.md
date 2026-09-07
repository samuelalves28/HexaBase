# HexaBase - Documentação da Arquitetura

## 📋 Visão Geral

HexaBase é uma aplicação backend construída com **.NET 10** seguindo os princípios de **Arquitetura Hexagonal (Ports & Adapters)** e **Clean Architecture**. O projeto é organizado em camadas bem definidas, separando concerns e facilitando testes e manutenção.

---

## 🔷 Arquitetura Hexagonal (Ports & Adapters)

### O que é Arquitetura Hexagonal?

A Arquitetura Hexagonal, criada por **Alistair Cockburn**, é um padrão que:

1. **Isola o core business** (Domain) do mundo externo
2. **Define contratos (Ports)** para comunicação com o exterior
3. **Implementa adaptadores (Adapters)** para diferentes tecnologias

### Conceito Visual

```
                    MUNDO EXTERNO
                ┌─────────────────────┐
                │   HTTP Controller   │  ← Adapter (In/Left)
                │   RabbitMQ Consumer │  ← Adapter (In/Left)
                └──────────┬──────────┘
                           │ (Porta)
         ┌─────────────────┴─────────────────┐
         │                                   │
    ┌────────────────────────────────────────────┐
    │                                            │
    │       CORE DO NEGÓCIO (Domain)            │
    │  ┬─────────────────────────────────────┬ │
    │  │  - User (Entidade)                  │ │
    │  │  - Business Rules                   │ │
    │  │  - Casos de Uso                     │ │
    │  └─────────────────────────────────────┘ │
    │                                          │
    └────────────────────────────────────────────┘
         │                    │
         │(Porta)             │(Porta)
         │                    │
    ┌────────────────┐   ┌──────────────────┐
    │  PostgreSQL    │   │  Email Service   │
    │  (Persistence) │   │  (External API)  │
    └────────────────┘   └──────────────────┘
    Adapter (Out/Right)   Adapter (Out/Right)
```

### Ports & Adapters no HexaBase

#### **Ports (Interfaces/Contratos)**

São as **interfaces** que definem como o core conversa com o mundo externo:

| Porta | Tipo | Interface | Responsabilidade |
|-------|------|-----------|------------------|
| `IUserRepository` | Out | `Domain/` | Persistência de usuários |
| `IPasswordHasher` | Out | `Application/` | Hash de senhas |
| `IViaCepService` | Out | `Application/` | Buscar endereço |
| `IEmailService` | Out | `Application/` | Enviar emails |
| `IUserCreatedPublisher` | Out | `Application/` | Publicar eventos |
| `UsersController` | In | `Api/` | HTTP requests |

#### **Adapters (Implementações)**

São as **implementações concretas** que usam frameworks:

| Adapter | Tipo | Implementação | Tecnologia |
|---------|------|---|---|
| `UserRepository` | Out | `Infrastructure/Persistence/Repositories/` | EF Core + PostgreSQL |
| `PasswordHasher` | Out | `Infrastructure/Security/` | BCrypt |
| `ViaCepService` | Out | `Infrastructure/External/Cep/` | HttpClient |
| `UserCreatedPublisher` | Out | `Infrastructure/Messaging/` | RabbitMQ |
| `UsersController` | In | `Api/Controllers/` | ASP.NET Core |

### Fluxo Hexagonal em Ação

#### Exemplo: Criar Usuário

```
┌─────────────────────────────────────┐
│  ADAPTER IN (HTTP)                  │
│  POST /api/users                    │
└──────────────┬──────────────────────┘
               │ (CreateUserRequestDTO)
               ↓
┌──────────────────────────────────────┐
│  PORT (ISender do MediatR)           │
│  Send(CreateUserCommand)             │
└──────────────┬───────────────────────┘
               ↓
    ┌──────────────────────────────┐
    │   CORE DO NEGÓCIO            │
    │  CreateUserHandler           │
    │  - Validar entrada           │
    │  - Criar User (Entidade)     │
    │  - Aplicar regras de negócio │
    └──────────────┬───────────────┘
                   ↓
    ┌──────────────────────────────────────────┐
    │  ADAPTERS OUT (Infraestrutura)           │
    │  ┌────────────────────────────────────┐  │
    │  │ PORT: IPasswordHasher              │  │
    │  │ ADAPTER: PasswordHasher (BCrypt)   │  │
    │  └────────────────────────────────────┘  │
    │  ┌────────────────────────────────────┐  │
    │  │ PORT: IUserRepository              │  │
    │  │ ADAPTER: UserRepository (EF Core)  │  │
    │  └────────────────────────────────────┘  │
    │  ┌────────────────────────────────────┐  │
    │  │ PORT: IUserCreatedPublisher        │  │
    │  │ ADAPTER: UserCreatedPublisher (RMQ)  │
    │  └────────────────────────────────────┘  │
    └────────────────────────────────────────────┘
               ↓
┌──────────────────────────────────────┐
│  ADAPTER OUT (HTTP Response)         │
│  201 Created + { publicId }          │
└──────────────────────────────────────┘
```

### Vantagens da Arquitetura Hexagonal no HexaBase

✅ **Core Business Isolado**
- Domain não depende de frameworks
- Pode ser testado/reutilizado em qualquer contexto

✅ **Fácil Trocar Implementações**
- Trocar PostgreSQL por MongoDB? Só muda o adapter `UserRepository`
- RabbitMQ por Kafka? Só muda o adapter `UserCreatedPublisher`
- O Domain e Application não mudam!

✅ **Alta Testabilidade**
- Mock dos ports facilita testes unitários
- Testa lógica sem depender de BD ou APIs externas

✅ **Múltiplas Entradas**
- HTTP via `UsersController` (Adapter In)
- Mensageria via `UserCreatedMessageConsumer` (Adapter In)
- CLI, gRPC, etc. - todos usam os mesmos handlers

---

## 🏗️ Estrutura de Projetos

```
HexaBase/
├── src/
│   ├── HexaBase.Api              # Camada de Apresentação (HTTP)
│   ├── HexaBase.Application      # Camada de Casos de Uso (Business Logic)
│   ├── HexaBase.Domain           # Camada de Domínio (Core Business)
│   └── HexaBase.Infrastructure   # Camada de Infraestrutura (Externals)
└── HexaBase.slnx                 # Solução
```

---

## 🎯 Camadas e Responsabilidades

### 1️⃣ **Domain** (`HexaBase.Domain`)
**Responsabilidade:** Lógica de negócio pura, independente de frameworks

**Componentes principais:**
- **Aggregates:** Entidades do domínio (`User`)
- **Repositories (Interfaces):** Contratos de persistência (`IUserRepository`)
- **Base Entities:** Classe base para entidades do domínio

**Características:**
- ✅ Sem dependências externas
- ✅ Encapsula regras de negócio
- ✅ Reutilizável em qualquer contexto

**Exemplo:**
```
User (Agregado)
├── Name
├── Email
└── PasswordHash
```

---

### 2️⃣ **Application** (`HexaBase.Application`)
**Responsabilidade:** Orquestração de casos de uso e lógica de aplicação

**Componentes principais:**

#### **Commands** (Escrita)
- `CreateUserCommand` - DTO do comando
- `CreateUserHandler` - Handler que executa a lógica

#### **Queries** (Leitura)
- `GetUserByPublicIdQuery` - DTO da query
- `GetUserByPublicIdHandler` - Handler que busca dados
- `GetUserByPublicIdResponse` - Response DTO

#### **Shared Services** (Interfaces)
- `IPasswordHasher` - Contrato para hash de senhas
- `IViaCepService` - Contrato para buscar endereço via CEP
- `IEmailService` - Contrato para envio de emails
- `IUserCreatedPublisher` - Contrato para publicar eventos

#### **Shared Messages**
- `UserCreatedMessage` - Evento de user criado

**Padrão:** CQRS + MediatR

---

### 3️⃣ **Infrastructure** (`HexaBase.Infrastructure`)
**Responsabilidade:** Implementação de integrações externas e persistência

**Componentes principais:**

#### **Persistence** (Banco de Dados)
```
Adapters/Out/Persistence/
├── HexaBaseDbContext         # EF Core Context
├── HexaBaseDbContextFactory  # Factory para migrações
├── Migrations/               # EF Core Migrations
├── Configurations/           # Fluent API (UserConfiguration)
└── Repositories/             # Implementação (UserRepository)
```

#### **External Services**
```
Adapters/Out/External/
├── Cep/                      # Via CEP API
│   └── ViaCepService        # Busca endereço por CEP
└── Messaging/                # RabbitMQ
	└── UserCreatedPublisher  # Publica eventos
```

#### **Security**
```
Adapters/Out/Security/
└── Authentication/
	└── PasswordHasher        # Hash bcrypt
```

#### **Dependency Injection**
- `DependencyInjection.cs` - Registra todos os serviços

**Tecnologias:**
- Entity Framework Core (PostgreSQL)
- RabbitMQ
- HttpClient (Via CEP)

---

### 4️⃣ **API** (`HexaBase.Api`)
**Responsabilidade:** Exposição de endpoints HTTP

**Componentes principais:**

#### **Controllers**
- `UsersController` - Endpoints para gerenciar usuários
  - `POST /api/users` - Criar usuário
  - `GET /api/users/{publicId}` - Obter usuário por ID

#### **DTOs (Data Transfer Objects)**
- `CreateUserRequestDTO` - Request para criar usuário
- `GetUserResponse` - Response ao obter usuário

#### **Configuração**
- `Program.cs` - Setup da aplicação
- `appsettings.json` - Configurações
- Swagger/OpenAPI - Documentação auto-gerada

**Stack:**
- ASP.NET Core Web API
- MediatR (CQRS)
- Swagger
- EF Core (Migrations automáticas)

---

## 🔗 Fluxo de Dados

### Criar Usuário (POST /api/users)

```
1. UsersController
   ↓
2. CreateUserRequestDTO (desserializado)
   ↓
3. CreateUserCommand (enviado via MediatR)
   ↓
4. CreateUserHandler (executa lógica)
   ├─ IPasswordHasher.Hash() → hash da senha
   ├─ User domain entity criado
   ├─ IUserRepository.AddAsync() → persistido
   ├─ IUserCreatedPublisher.PublishAsync() → evento publicado
   ├─ ✅ Commit (Transaction)
   └─ Retorna PublicId
   ↓
5. UsersController retorna 201 Created
```

### Obter Usuário (GET /api/users/{publicId})

```
1. UsersController
   ↓
2. GetUserByPublicIdQuery (enviado via MediatR)
   ↓
3. GetUserByPublicIdHandler (executa lógica)
   ├─ IUserRepository.GetByPublicIdAsync()
   └─ GetUserByPublicIdResponse (mapeado)
   ↓
4. UsersController retorna 200 OK + dados
```

---

## 📦 Dependências Entre Projetos

```
HexaBase.Api
	↓ depends on
HexaBase.Application
HexaBase.Infrastructure
	↓ depends on
HexaBase.Domain
	↓ (stands alone - no external deps)
```

**Regra:** Dependências fluem de fora para dentro (Dependency Inversion Principle)

---

## 🛠️ Padrões e Técnicas

### **Arquitetura Hexagonal (Ports & Adapters)**
Veja a **[seção dedicada](#-arquitetura-hexagonal-ports--adapters)** acima para detalhes completos.

- **Ports:** Interfaces que definem contratos (IUserRepository, IPasswordHasher, etc.)
- **Adapters:** Implementações concretas (UserRepository com EF Core, PasswordHasher com BCrypt, etc.)
- **Benefício:** Core business isolado e fácil trocar implementações sem afetar a lógica de negócio

### **CQRS (Command Query Responsibility Segregation)**
- **Commands:** Modificam estado (CreateUserCommand)
- **Queries:** Leem dados (GetUserByPublicIdQuery)
- Separação explícita entre escrita e leitura

### **MediatR**
- Mediatr pattern para desacoplamento
- Handlers recebem requisições e retornam respostas
- Pipeline com validação, logging, etc.

### **Repository Pattern**
- Interface no Domain, implementação na Infrastructure
- Abstração de persistência

### **Dependency Injection (DI)**
- Configurado em `Infrastructure/DependencyInjection.cs`
- Registrado no `Program.cs`

### **Entity Framework Core**
- Database-first com migrations
- PostgreSQL como banco de dados
- Fluent API para configuração de mapeamento

---

## 🔐 Segurança

- **Password Hashing:** BCrypt via `IPasswordHasher`
- **HTTPS:** Configurado por padrão
- **Validação:** DTOs + Command Validation (via MediatR pipeline)

---

## 📊 Entidades Principais

### **User (Agregado)**
```
┌─────────────────────┐
│       User          │
├─────────────────────┤
│ PublicId: Guid      │ (Identificador externo)
│ Name: string        │
│ Email: string       │
│ PasswordHash: str   │
│ CreatedAt: DateTime │
│ UpdatedAt: DateTime │
└─────────────────────┘
```

---

## 🚀 Fluxo de Startup

1. `Program.cs` criar builder
2. Registrar serviços de Infraestrutura (`AddInfrastructure()`)
3. Registrar MediatR handlers
4. Build aplicação
5. Executar migrations do EF Core
6. Iniciar servidor HTTP

---

## 📝 Configuração (appsettings.json)

```json
{
  "ConnectionStrings": {
	"DefaultConnection": "PostgreSQL connection string"
  },
  "RabbitMq": {
	"HostName": "localhost",
	"Port": 5672,
	"ExchangeName": "hexabase.events",
	"QueueName": "user.created.queue",
	"RoutingKey": "user.created"
  }
}
```

---

## 🧪 Testabilidade

Por causa da arquitetura em camadas:

✅ **Domain** pode ser testado isoladamente sem dependências  
✅ **Application** handlers podem ser testados mockando repositórios  
✅ **Infrastructure** implementações podem ser testadas com containers  
✅ **API** controllers podem ser testados mockando MediatR  

---

## 📚 Tecnologias e Versões

| Tecnologia | Versão | Propósito |
|---|---|---|
| .NET | 10 | Runtime |
| ASP.NET Core | 10 | Web Framework |
| Entity Framework Core | Latest | ORM |
| PostgreSQL | Latest | Banco de Dados |
| MediatR | Latest | CQRS Pattern |
| RabbitMQ | Latest | Message Broker |
| Swagger/OpenAPI | Latest | Documentação API |
| BCrypt | Latest | Password Hashing |

---

## 📌 Resumo da Arquitetura

```
┌─────────────────────────────────────────────────────┐
│                   API Layer (HTTP)                   │
│  UsersController → CreateUserCommand/Query          │
└─────────────────────────────────────────────────────┘
						   ↓
┌─────────────────────────────────────────────────────┐
│              Application Layer (CQRS)                │
│  Handlers → Business Logic → Services              │
└─────────────────────────────────────────────────────┘
						   ↓
┌─────────────────────────────────────────────────────┐
│              Infrastructure Layer                    │
│  DbContext, Repositories, External Services        │
└─────────────────────────────────────────────────────┘
						   ↓
┌─────────────────────────────────────────────────────┐
│                Domain Layer (Core)                   │
│  User Entity, Business Rules, Repository Contracts │
└─────────────────────────────────────────────────────┘
```

---

**Versão:** 1.0  
**Data:** 2025-01-20  
**Arquitetura:** Hexagonal + Clean Architecture + CQRS
