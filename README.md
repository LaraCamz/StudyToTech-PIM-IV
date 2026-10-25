# StudyToTech — Backend (API REST)

Backend em **ASP.NET Core 8 + MySQL** do módulo de Produtos, Estoque e Pedidos (PIM IV).

Orientado a objetos · JWT · CORS liberado para o front · alinhado ao contrato OpenAPI e aos requisitos do projeto.

## Arquitetura

```
Front (Web/Mobile)  →  API REST (este projeto)  →  MySQL (studytotech)
```

Camadas:
- **Controllers** — endpoints HTTP
- **Services** — regras de negócio (OO)
- **Entities + AppDbContext** — persistência (EF Core + Pomelo MySQL)
- **DTOs** — contrato de entrada/saída
- **Middleware** — erros no formato `{ codigo, mensagem, detalhes, timestamp }`

## Pré-requisitos

- .NET 8 SDK
- MySQL 8

## Banco de dados

```bash
mysql -u root -p < Scripts/schema.sql
```

Cria o database `studytotech`, tabelas, seeds (perfis, categorias, status) e views.

A API também tenta `EnsureCreated()` na primeira execução (útil em dev).

## Configuração

Edite `appsettings.json`:

```json
"ConnectionStrings": {
  "Default": "Server=localhost;Port=3306;Database=studytotech;User=root;Password=SUA_SENHA_MYSQL;"
},
"Jwt": {
  "Secret": "Study_to_Tech_Unip_III_2026_10",
  "Issuer": "StudyToTech",
  "Audience": "StudyToTechClients",
  "ExpirationMinutes": 60
}
```

## Rodar

```bash
dotnet restore
dotnet run
```

- API: **http://localhost:8080**
- Swagger: **http://localhost:8080/swagger**
- Base URL para o front: **http://localhost:8080/api/v1**

### Admin padrão (criado no startup)

| Email | Senha | Perfil |
|-------|-------|--------|
| admin@studytotech.com | Admin@123 | Administrador |

## Integração com o front

1. `POST /api/v1/auth/login` → recebe `accessToken`
2. Em todas as rotas protegidas: header `Authorization: Bearer <token>`
3. Preço sempre **string** (`"3499.90"`), nunca number
4. Paginação: `page` começa em **0**, `size` máx. 100
5. CORS: `AllowAnyOrigin` (dev)

### Exemplos

```http
POST /api/v1/auth/login
{ "email": "admin@studytotech.com", "senha": "Admin@123" }

GET /api/v1/produtos?page=0&size=20
Authorization: Bearer ...

POST /api/v1/pedidos
Authorization: Bearer ...   (perfil Cliente)

POST /api/v1/movimentacoes-estoque
{ "produtoId": 1, "tipo": "ENTRADA", "quantidade": 10 }
```

## Endpoints (resumo)

| Área | Rotas |
|------|--------|
| Auth | `POST /auth/login`, `POST /auth/registro`, `GET /auth/me` |
| Usuários | `GET/POST /usuarios`, `GET/PUT /usuarios/{id}`, `PATCH .../status`, `GET /perfis` |
| Produtos | `GET /categorias`, `GET/POST /produtos`, `GET/PUT /produtos/{id}`, `PATCH .../status` |
| Estoque | `GET /estoque`, `GET /estoque/alertas`, `GET/POST /movimentacoes-estoque` |
| Pedidos | `GET/POST /pedidos`, `GET /pedidos/em-elaboracao`, itens, `POST .../finalizar`, `PATCH .../status` |
| Admin | `GET /indicadores` |

## Regras de negócio atendidas

- JWT + perfis (Cliente / Funcionário / Administrador)
- Um pedido “Em elaboração” por cliente
- Débito de estoque atômico (`SELECT … FOR UPDATE`) na finalização
- Histórico imutável de movimentações
- Soft-status (ativo/inativo) em produto e usuário
- Estoque não fica negativo
- Erros padronizados (`ESTOQUE_INSUFICIENTE`, `PEDIDO_VAZIO`, etc.)

## Estrutura

```
StudyToTech.Backend/
├── Controllers/
├── Services/
├── Entities/
├── Data/AppDbContext.cs
├── DTOs/
├── Helpers/
├── Middleware/
├── Scripts/schema.sql
├── Program.cs
└── appsettings.json
```
