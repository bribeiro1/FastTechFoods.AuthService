# FastTechFoods - AuthService

Este é o serviço de autenticação da plataforma **FastTechFoods**, responsável pelo cadastro, login e controle de acesso de usuários, incluindo Gerentes, Atendentes e Clientes.

## Tecnologias Utilizadas

- .NET 8
- Entity Framework Core
- SQL Server
- FluentValidation
- JWT (Json Web Token)
- Swagger/OpenAPI
- xUnit + Moq (para testes unitários)

---

## Estrutura de Projeto

```
FastTechFoods.AuthService/
├── Api/                   # Controllers e configuração do ASP.NET
├── Application/           # DTOs, Services e Validators
├── Domain/                # Entidades e constantes (ex: UserRoles)
├── Infrastructure/        # EF DbContext, Repositórios e Serviços auxiliares
├── Tests/                 # Testes unitários com xUnit
```

---

## Executando os Testes

```bash
dotnet test
```

---

## Executando o Projeto

1. Execute a aplicação:
```bash
dotnet run --project FastTechFoods.AuthService.Api
```

2. Acesse o Swagger:
```
https://localhost:{porta}/swagger
```

---

## Perfis de Usuário

| Perfil     | Permissões                                       |
|------------|--------------------------------------------------|
| `Gerente`  | Pode registrar qualquer perfil e visualizar todos os usuários |
| `Atendente`| Pode visualizar apenas os clientes               |
| `Cliente`  | Pode se registrar livremente                     |

---

## Segurança

- Autenticação baseada em JWT.
- Validações via FluentValidation.
- Apenas Gerentes podem registrar novos atendentes e gerentes.
- Tokens devem ser incluídos no header como:
```
Authorization: Bearer {seu_token}
```

---

## Endpoints Principais

### POST `/api/user/register`
Registra novo usuário. Apenas clientes podem se registrar sem token.

### POST `/api/user/login`
Login por CPF ou Email + senha. Retorna token JWT.

### GET `/api/user`
- **Gerente**: vê todos os usuários.
- **Atendente**: vê apenas clientes.

---

## Cobertura de Testes

O serviço possui testes unitários para:
- Autenticação com sucesso e falha
- Registro com validação e falha de dados
- Verificações de existência de usuário
