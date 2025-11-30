# 🎓 Futzin - Guia de Implementação Passo a Passo

> **Para devs Node.js aprendendo C#**

Este guia vai te levar do zero até um app fullstack funcional, implementando cada funcionalidade uma por vez.

---

## 📚 Índice

1. [Preparação do Ambiente](#1-preparação-do-ambiente)
2. [Criar o Banco de Dados](#2-criar-o-banco-de-dados)
3. [Implementar Autenticação JWT](#3-implementar-autenticação-jwt)
4. [CRUD de Usuários](#4-crud-de-usuários)
5. [CRUD de Peladas](#5-crud-de-peladas)
6. [Sistema de Participantes](#6-sistema-de-participantes)
7. [Sorteio de Times](#7-sorteio-de-times)
8. [Registro de Gols](#8-registro-de-gols)
9. [Frontend Simples](#9-frontend-simples)
10. [Testes](#10-testes)

---

## 1. Preparação do Ambiente

### ✅ O que você já tem

- [x] Estrutura DDD completa
- [x] Entidades criadas
- [x] Interfaces de repositórios
- [x] Serviços implementados
- [x] Controllers criados
- [x] Pacotes instalados

### 🎯 O que vamos fazer agora

Criar o banco de dados e testar se está tudo funcionando.

### 📝 Passo a Passo

#### 1.1 Instalar ferramenta EF Core (se não tiver)

```bash
dotnet tool install --global dotnet-ef
```

**Explicação:** O `dotnet-ef` é a ferramenta de linha de comando do Entity Framework Core que permite criar migrations e gerenciar o banco de dados.

#### 1.2 Verificar se o projeto compila

```bash
cd Futzin.Api
dotnet build
```

**O que esperar:** Deve compilar sem erros.

#### 1.3 Adicionar configuração do JWT no appsettings.json

Abra `Futzin.Api/appsettings.json` e adicione:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=futzin.db"
  },
  "Jwt": {
    "Secret": "sua-chave-secreta-super-segura-com-no-minimo-32-caracteres-futzin-2025",
    "Issuer": "FutzinApi",
    "Audience": "FutzinApp"
  }
}
```

**Explicação:** 
- `ConnectionStrings`: Define onde o SQLite vai salvar o banco
- `Jwt:Secret`: Chave para assinar os tokens (use uma diferente em produção!)

---

## 2. Criar o Banco de Dados

### 🎯 Objetivo

Criar as tabelas no banco de dados baseado nas suas entidades.

### 📝 Passo a Passo

#### 2.1 Criar a primeira migration

```bash
dotnet ef migrations add InitialCreate
```

**O que acontece:** 
- Cria uma pasta `Migrations/` com os arquivos de migração
- Analisa suas entidades e cria o SQL necessário

**Dica:** Se der erro, verifique se o `FutzinDbContext` está configurado no `Program.cs`

#### 2.2 Aplicar a migration (criar o banco)

```bash
dotnet ef database update
```

**O que acontece:**
- Cria o arquivo `futzin.db` (SQLite)
- Cria todas as tabelas (Users, Peladas, Teams, etc)

#### 2.3 Verificar o banco de dados

Você pode usar uma extensão do VS Code para visualizar o SQLite:

1. Instale a extensão "SQLite Viewer"
2. Abra o arquivo `futzin.db`
3. Veja as tabelas criadas!

**Tabelas esperadas:**
- Users
- Peladas
- PeladaParticipants
- Teams
- Goals

---

## 3. Implementar Autenticação JWT

### 🎯 Objetivo

Testar o sistema de registro e login de usuários.

### 📝 Passo a Passo

#### 3.1 Executar a API

```bash
dotnet run
```

**O que esperar:**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:5001
```

#### 3.2 Testar o registro de usuário

Abra o arquivo `Futzin.Api.http` e execute (ou use Postman):

```http
POST https://localhost:5001/api/auth/register
Content-Type: application/json

{
  "name": "Bruno Santos",
  "email": "bruno@futzin.com",
  "password": "senha123",
  "phone": "11999999999"
}
```

**Resposta esperada:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": 1,
    "name": "Bruno Santos",
    "email": "bruno@futzin.com",
    ...
  }
}
```

**✅ Sucesso!** Você criou seu primeiro usuário e recebeu um token JWT!

#### 3.3 Testar o login

```http
POST https://localhost:5001/api/auth/login
Content-Type: application/json

{
  "email": "bruno@futzin.com",
  "password": "senha123"
}
```

**Resposta esperada:** O mesmo formato do registro, com um novo token.

#### 3.4 Testar endpoint protegido

Copie o token recebido e teste:

```http
GET https://localhost:5001/api/users/me
Authorization: Bearer SEU_TOKEN_AQUI
```

**Resposta esperada:** Seus dados de usuário.

**🎓 O que você aprendeu:**
- Como funciona JWT
- Hash de senha com BCrypt
- Autenticação stateless
- Middleware de autorização

---

## 4. CRUD de Usuários

### 🎯 Objetivo

Implementar operações completas de usuários.

### 📝 Testes

#### 4.1 Listar todos os usuários

```http
GET https://localhost:5001/api/users
Authorization: Bearer SEU_TOKEN
```

#### 4.2 Buscar usuário por ID

```http
GET https://localhost:5001/api/users/1
Authorization: Bearer SEU_TOKEN
```

#### 4.3 Atualizar perfil

```http
PUT https://localhost:5001/api/users/1
Authorization: Bearer SEU_TOKEN
Content-Type: application/json

{
  "name": "Bruno Silva Santos",
  "phone": "11988888888"
}
```

#### 4.4 Deletar usuário

```http
DELETE https://localhost:5001/api/users/1
Authorization: Bearer SEU_TOKEN
```

**⚠️ Atenção:** Só pode deletar o próprio usuário!

**🎓 O que você aprendeu:**
- CRUD completo
- Validação de autorização
- DTOs para entrada/saída
- Repository Pattern

---

## 5. CRUD de Peladas

### 🎯 Objetivo

Criar e gerenciar peladas.

### 📝 Passo a Passo

#### 5.1 Criar uma pelada

```http
POST https://localhost:5001/api/peladas
Authorization: Bearer SEU_TOKEN
Content-Type: application/json

{
  "name": "Pelada de Sábado",
  "description": "Toda semana no campo do bairro",
  "date": "2025-12-06T15:00:00",
  "location": "Campo do Vila Nova",
  "price": 20.00,
  "maxPlayers": 20
}
```

**Resposta esperada:**
```json
{
  "id": 1,
  "name": "Pelada de Sábado",
  "location": "Campo do Vila Nova",
  "createdBy": {
    "id": 1,
    "name": "Bruno Santos",
    ...
  },
  ...
}
```

#### 5.2 Listar peladas ativas

```http
GET https://localhost:5001/api/peladas
Authorization: Bearer SEU_TOKEN
```

#### 5.3 Listar minhas peladas

```http
GET https://localhost:5001/api/peladas/my
Authorization: Bearer SEU_TOKEN
```

#### 5.4 Ver detalhes de uma pelada

```http
GET https://localhost:5001/api/peladas/1
Authorization: Bearer SEU_TOKEN
```

**Resposta:** Inclui lista de participantes!

#### 5.5 Atualizar pelada

```http
PUT https://localhost:5001/api/peladas/1
Authorization: Bearer SEU_TOKEN
Content-Type: application/json

{
  "location": "Campo do Centro",
  "price": 25.00
}
```

**⚠️ Atenção:** Só o criador da pelada pode editá-la!

**🎓 O que você aprendeu:**
- Relacionamentos entre entidades
- Filtros e queries
- Autorização baseada em dono do recurso

---

## 6. Sistema de Participantes

### 🎯 Objetivo

Adicionar jogadores e controlar pagamentos.

### 📝 Passo a Passo

#### 6.1 Criar mais usuários

Crie 2-3 usuários a mais para testar:

```http
POST https://localhost:5001/api/auth/register
Content-Type: application/json

{
  "name": "João Silva",
  "email": "joao@futzin.com",
  "password": "senha123"
}
```

Anote os IDs dos usuários criados!

#### 6.2 Adicionar participante à pelada

```http
POST https://localhost:5001/api/peladas/1/participants
Authorization: Bearer SEU_TOKEN
Content-Type: application/json

{
  "userId": 2
}
```

Adicione mais alguns participantes (IDs diferentes).

#### 6.3 Marcar como pago

```http
PUT https://localhost:5001/api/peladas/1/participants/2
Authorization: Bearer SEU_TOKEN
Content-Type: application/json

{
  "hasPaid": true
}
```

#### 6.4 Ver participantes

```http
GET https://localhost:5001/api/peladas/1
Authorization: Bearer SEU_TOKEN
```

Veja a lista de participantes com status de pagamento.

**🎓 O que você aprendeu:**
- Many-to-many relationships
- Tabela intermediária com dados
- Update parcial de entidades

---

## 7. Sorteio de Times

### 🎯 Objetivo

Sortear times automaticamente.

### 📝 Passo a Passo

#### 7.1 Adicionar pelo menos 4 participantes pagos

Certifique-se de ter pelo menos 4 participantes com `hasPaid: true`.

#### 7.2 Sortear times

```http
POST https://localhost:5001/api/teams/generate
Authorization: Bearer SEU_TOKEN
Content-Type: application/json

{
  "peladaId": 1,
  "onlyPaidPlayers": true
}
```

**O que acontece:**
- Pega todos os jogadores que pagaram
- Embaralha aleatoriamente
- Divide em 2 times (Time A e Time B)
- Atribui cada jogador a um time

#### 7.3 Ver times formados

```http
GET https://localhost:5001/api/teams/pelada/1
Authorization: Bearer SEU_TOKEN
```

**Resposta esperada:**
```json
[
  {
    "id": 1,
    "name": "Time A",
    "color": "#FF0000",
    "players": [
      { "id": 1, "name": "Bruno Santos", ... },
      { "id": 3, "name": "Pedro Costa", ... }
    ],
    "goalsCount": 0
  },
  {
    "id": 2,
    "name": "Time B",
    "color": "#0000FF",
    "players": [
      { "id": 2, "name": "João Silva", ... },
      { "id": 4, "name": "Maria Souza", ... }
    ],
    "goalsCount": 0
  }
]
```

**🎓 O que você aprendeu:**
- Lógica de negócio complexa em Services
- Randomização
- Transações implícitas do EF Core
- Include de relacionamentos

---

## 8. Registro de Gols

### 🎯 Objetivo

Registrar gols e ver estatísticas.

### 📝 Passo a Passo

#### 8.1 Registrar gol

```http
POST https://localhost:5001/api/goals
Authorization: Bearer SEU_TOKEN
Content-Type: application/json

{
  "peladaId": 1,
  "scorerId": 2,
  "teamId": 1,
  "notes": "Gol de cabeça após cruzamento"
}
```

Registre mais alguns gols de diferentes jogadores.

#### 8.2 Ver gols da pelada

```http
GET https://localhost:5001/api/goals/pelada/1
Authorization: Bearer SEU_TOKEN
```

#### 8.3 Ver estatísticas do jogador

```http
GET https://localhost:5001/api/goals/stats/2
Authorization: Bearer SEU_TOKEN
```

**Resposta esperada:**
```json
{
  "player": {
    "id": 2,
    "name": "João Silva",
    ...
  },
  "totalGoals": 3,
  "peladasPlayed": 2
}
```

#### 8.4 Ver meus gols

```http
GET https://localhost:5001/api/goals/my-goals
Authorization: Bearer SEU_TOKEN
```

**🎓 O que você aprendeu:**
- Agregações (Count, Sum)
- Joins complexos
- Filtros dinâmicos
- DTOs para estatísticas

---

## 9. Frontend Simples

### 🎯 Objetivo

Criar interface web para consumir a API.

### 📝 Passo a Passo

#### 9.1 Criar estrutura do frontend

```bash
# Na raiz do projeto
mkdir Futzin.Web
cd Futzin.Web
mkdir css js
```

#### 9.2 Criar página de login

Crie `index.html`:

```html
<!DOCTYPE html>
<html lang="pt-BR">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Futzin - Login</title>
    <link rel="stylesheet" href="css/style.css">
</head>
<body>
    <div class="container">
        <h1>⚽ Futzin</h1>
        <form id="login-form">
            <input type="email" id="email" placeholder="Email" required>
            <input type="password" id="password" placeholder="Senha" required>
            <button type="submit">Entrar</button>
        </form>
        <p>Não tem conta? <a href="register.html">Cadastre-se</a></p>
    </div>
    <script src="js/api.js"></script>
    <script src="js/login.js"></script>
</body>
</html>
```

#### 9.3 Criar cliente da API

Crie `js/api.js`:

```javascript
const API_URL = 'https://localhost:5001/api';

class ApiClient {
    constructor() {
        this.token = localStorage.getItem('token');
    }

    async request(endpoint, options = {}) {
        const headers = {
            'Content-Type': 'application/json',
            ...options.headers
        };

        if (this.token) {
            headers['Authorization'] = `Bearer ${this.token}`;
        }

        const response = await fetch(`${API_URL}${endpoint}`, {
            ...options,
            headers
        });

        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.message || 'Erro na requisição');
        }

        return response.json();
    }

    async login(email, password) {
        const data = await this.request('/auth/login', {
            method: 'POST',
            body: JSON.stringify({ email, password })
        });
        this.token = data.token;
        localStorage.setItem('token', data.token);
        localStorage.setItem('user', JSON.stringify(data.user));
        return data;
    }

    async getPeladas() {
        return this.request('/peladas');
    }

    // ... outros métodos
}

const api = new ApiClient();
```

#### 9.4 Criar lógica de login

Crie `js/login.js`:

```javascript
document.getElementById('login-form').addEventListener('submit', async (e) => {
    e.preventDefault();
    
    const email = document.getElementById('email').value;
    const password = document.getElementById('password').value;

    try {
        await api.login(email, password);
        window.location.href = 'dashboard.html';
    } catch (error) {
        alert('Erro ao fazer login: ' + error.message);
    }
});
```

#### 9.5 Criar CSS básico

Crie `css/style.css`:

```css
* {
    margin: 0;
    padding: 0;
    box-sizing: border-box;
}

body {
    font-family: Arial, sans-serif;
    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    min-height: 100vh;
    display: flex;
    align-items: center;
    justify-content: center;
}

.container {
    background: white;
    padding: 40px;
    border-radius: 10px;
    box-shadow: 0 10px 40px rgba(0,0,0,0.2);
    max-width: 400px;
    width: 100%;
}

h1 {
    text-align: center;
    color: #667eea;
    margin-bottom: 30px;
}

input {
    width: 100%;
    padding: 12px;
    margin-bottom: 15px;
    border: 1px solid #ddd;
    border-radius: 5px;
}

button {
    width: 100%;
    padding: 12px;
    background: #667eea;
    color: white;
    border: none;
    border-radius: 5px;
    cursor: pointer;
    font-size: 16px;
}

button:hover {
    background: #5568d3;
}
```

#### 9.6 Testar

Abra `index.html` no navegador e faça login!

**🎓 O que você aprendeu:**
- Fetch API
- LocalStorage
- Manipulação do DOM
- Async/Await no frontend

**📝 Próximos passos do frontend:**
- Criar página de dashboard
- Listar peladas
- Formulário de criar pelada
- Ver detalhes e adicionar participantes

---

## 10. Testes

### 🎯 Objetivo

Garantir qualidade do código com testes unitários.

### 📝 Passo a Passo

#### 10.1 Criar projeto de testes

```bash
# Na raiz da solution
dotnet new xunit -n Futzin.Tests
dotnet sln add Futzin.Tests/Futzin.Tests.csproj
cd Futzin.Tests
dotnet add reference ../Futzin.Api/Futzin.Api.csproj
```

#### 10.2 Adicionar pacotes de teste

```bash
dotnet add package Moq
dotnet add package FluentAssertions
```

#### 10.3 Criar teste de exemplo

Crie `UserServiceTests.cs`:

```csharp
using Xunit;
using Moq;
using FluentAssertions;
using Futzin.Api.Application.Services;
using Futzin.Api.Domain.Interfaces;
using Futzin.Api.Domain.Entities;

public class UserServiceTests
{
    [Fact]
    public async Task Register_ShouldCreateUser_WhenEmailIsUnique()
    {
        // Arrange
        var mockUserRepo = new Mock<IUserRepository>();
        mockUserRepo.Setup(r => r.EmailExistsAsync(It.IsAny<string>()))
                   .ReturnsAsync(false);
        
        var mockAuthService = new Mock<IAuthService>();
        var mockMapper = new Mock<IMapper>();

        var service = new UserService(mockUserRepo.Object, mockAuthService.Object, mockMapper.Object);

        // Act & Assert
        // ... implementar teste
    }
}
```

#### 10.4 Executar testes

```bash
dotnet test
```

**🎓 O que você aprendeu:**
- xUnit
- Mocking com Moq
- Assertions com FluentAssertions
- TDD (Test-Driven Development)

---

## ✅ Checklist Final

Marque o que você já implementou:

### Backend
- [ ] Banco de dados criado
- [ ] Registro e login funcionando
- [ ] CRUD de usuários
- [ ] CRUD de peladas
- [ ] Sistema de participantes
- [ ] Sorteio de times
- [ ] Registro de gols
- [ ] Estatísticas

### Frontend
- [ ] Página de login
- [ ] Página de registro
- [ ] Dashboard
- [ ] Criar pelada
- [ ] Adicionar participantes
- [ ] Sortear times
- [ ] Registrar gols

### Extras
- [ ] Testes unitários
- [ ] Validações de formulário
- [ ] Tratamento de erros
- [ ] Loading states
- [ ] Deploy

---

## 🎉 Parabéns!

Se você chegou até aqui, você:
- ✅ Criou uma API REST completa em C#
- ✅ Aplicou DDD e SOLID
- ✅ Implementou autenticação JWT
- ✅ Criou um frontend consumindo a API
- ✅ Aprendeu Entity Framework Core

**Continue aprendendo:**
- Adicione mais features (chat, notificações)
- Melhore o frontend (React, Vue)
- Deploy na nuvem (Azure, AWS)
- Adicione CI/CD

---

**Dúvidas?** Consulte o [GUIA-COMPLETO.md](./GUIA-COMPLETO.md) ou a documentação oficial do .NET!
