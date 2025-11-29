# ⚽ Futzin - Football Match Management App

Fullstack system to organize football matches, with player control, payments, team drawing, and goal tracking.

---

## 🎯 About the Project

**Futzin** is a web application to facilitate the organization of weekly football matches. It allows player registration, payment control, automatic team drawing, and statistics tracking.

**Status:** 🚧 In Development

---

## 🛠️ Technologies Used

### Backend
- **C# .NET 9** - REST API
- **Entity Framework Core** - ORM
- **SQLite** - Database
- **JWT** - Authentication
- **AutoMapper** - Object mapping

### Frontend
- **React 19** - UI library
- **React Router DOM** - Client-side routing
- **Axios** - HTTP client
- **Vite** - Build tool and dev server

---

## 📋 Features

### Implemented
- ✅ DDD project structure
- ✅ Entities and relationships
- ✅ Repositories and services
- ✅ API Controllers

### In Development
- 🚧 Database migrations
- 🚧 JWT authentication system
- 🚧 User CRUD
- 🚧 Match CRUD
- 🚧 Participant and payment control
- 🚧 Team drawing
- 🚧 Goal tracking
- 🚧 Web interface

---

## 🚀 How to Run

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [VS Code](https://code.visualstudio.com/)
- [Node.js](https://nodejs.org/) (for development tools)

### Backend (API)

```bash
# Navigate to API folder
cd Futzin.Api

# Restore dependencies
dotnet restore

# Create database
dotnet ef migrations add InitialCreate
dotnet ef database update

# Run
dotnet run
```

The API will be available at: `https://localhost:5001`

### Frontend (React)

```bash
# Navigate to frontend folder
cd futzin-web

# Install dependencies
npm install

# Run development server
npm run dev
```

The frontend will be available at: `http://localhost:5173`

---

## 📖 Documentation

- **[GUIA-COMPLETO.md](./GUIA-COMPLETO.md)** - Complete development tutorial (PT-BR)
- **[PASSO-A-PASSO.md](./PASSO-A-PASSO.md)** - Step-by-step implementation guide (PT-BR)
- **Futzin.Api.http** - Request examples

---

## 🏗️ Architecture

The project follows **SOLID** and **DDD** principles:

```
Futzin.Api/
├── Domain/              # Entities and interfaces
├── Application/         # DTOs and business logic
├── Infrastructure/      # Repositories and database
└── Presentation/        # Controllers (API)

futzin-web/
├── src/
│   ├── components/    # React components
│   ├── pages/         # Page components
│   ├── services/      # API services
│   └── App.jsx        # Main app component
└── index.html         # Entry point
```

---

## 📝 API Endpoints

### Authentication
- `POST /api/auth/register` - Register user
- `POST /api/auth/login` - Login

### Users
- `GET /api/users` - List users
- `GET /api/users/me` - My profile
- `PUT /api/users/{id}` - Update profile

### Matches (Peladas)
- `POST /api/peladas` - Create match
- `GET /api/peladas` - List active matches
- `GET /api/peladas/my` - My matches
- `POST /api/peladas/{id}/participants` - Add player

### Teams
- `POST /api/teams/generate` - Draw teams
- `GET /api/teams/pelada/{id}` - Match teams

### Goals
- `POST /api/goals` - Register goal
- `GET /api/goals/stats/{userId}` - Player statistics

---

## 👨‍💻 Learning

This project was created as study material for Node.js developers learning C# and .NET.

**Applied concepts:**
- DDD (Domain-Driven Design)
- SOLID
- Repository Pattern
- Dependency Injection
- JWT Authentication
- Entity Framework Core

---

## 🤝 Contributing

This is a study project, but contributions are welcome!

1. Fork the project
2. Create a branch for your feature (`git checkout -b feature/MyFeature`)
3. Commit your changes (`git commit -m 'Add MyFeature'`)
4. Push to the branch (`git push origin feature/MyFeature`)
5. Open a Pull Request

---

## 📄 License

This project is under the MIT license. See the [LICENSE](LICENSE) file for more details.

---

## 📧 Contact

Developed by Bruno Santos

GitHub: [@brunnossanttos](https://github.com/brunnossanttos)

---

**Built with ⚽ and ☕**
