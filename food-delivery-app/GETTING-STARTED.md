# Food Delivery App - Getting Started

## Quick Start Guide

### Option 1: Local Development (Recommended)

#### Prerequisites
- Node.js 18+ (https://nodejs.org/)
- .NET 8 SDK (https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server or MySQL

#### Backend Setup

1. **Open PowerShell/Command Prompt and navigate to backend folder:**
   ```powershell
   cd food-delivery-app\backend
   ```

2. **Restore NuGet packages:**
   ```powershell
   dotnet restore
   ```

3. **Update database connection string in `appsettings.json`:**
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=.;Database=FoodDeliveryDB;Integrated Security=true;TrustServerCertificate=true;"
   }
   ```

4. **Create database:**
   ```powershell
   dotnet ef database update
   ```

5. **Run the API:**
   ```powershell
   dotnet run
   ```

   The API will be available at: `https://localhost:7001`
   Swagger docs at: `https://localhost:7001/swagger`

#### Frontend Setup

1. **Open another terminal and navigate to frontend folder:**
   ```powershell
   cd food-delivery-app\frontend
   ```

2. **Install npm dependencies:**
   ```powershell
   npm install
   ```

3. **Start development server:**
   ```powershell
   npm start
   ```

   The app will open at: `http://localhost:4200`

#### Default Test Credentials
- **Admin Login**: admin@test.com / password
- **Customer Login**: user@test.com / password

### Option 2: Docker Compose (Container Deployment)

#### Prerequisites
- Docker Desktop (https://www.docker.com/products/docker-desktop)

#### Steps

1. **Navigate to project root:**
   ```powershell
   cd food-delivery-app
   ```

2. **Build and run with Docker Compose:**
   ```powershell
   docker-compose up --build
   ```

3. **Access the application:**
   - Frontend: http://localhost:4200
   - Backend API: https://localhost:7001
   - Swagger: https://localhost:7001/swagger

4. **Stop the services:**
   ```powershell
   docker-compose down
   ```

### Option 3: Manual Build & Publish

#### Backend

1. **Build Release:**
   ```powershell
   cd backend
   dotnet publish -c Release -o .\publish
   ```

2. **Run published version:**
   ```powershell
   .\publish\FoodDeliveryAPI.exe
   ```

#### Frontend

1. **Build for production:**
   ```powershell
   cd frontend
   npm run build
   ```

2. **Serve the build:**
   ```powershell
   npm install -g http-server
   http-server dist -p 4200
   ```

## Available npm Commands (Frontend)

```bash
npm start          # Start development server
npm run build      # Build for production
npm test           # Run tests
npm run lint       # Run linter
```

## Available dotnet Commands (Backend)

```bash
dotnet run         # Run in development
dotnet build       # Build the project
dotnet test        # Run tests
dotnet publish     # Publish for production
```

## Project Structure

```
food-delivery-app/
├── frontend/
│   ├── src/
│   │   ├── app/
│   │   │   ├── components/    # Angular components
│   │   │   ├── services/      # Angular services
│   │   │   ├── models/        # TypeScript interfaces
│   │   │   └── guards/        # Route guards
│   │   ├── environments/       # Environment configs
│   │   └── index.html         # Main HTML
│   ├── angular.json           # Angular config
│   ├── package.json           # NPM dependencies
│   └── Dockerfile            # Docker config
│
├── backend/
│   ├── Models/               # C# models
│   ├── Controllers/          # API controllers
│   ├── Data/                 # DbContext
│   ├── Services/             # Business logic
│   ├── Program.cs            # Entry point
│   ├── appsettings.json      # Config
│   ├── FoodDeliveryAPI.csproj
│   └── Dockerfile            # Docker config
│
├── .gitlab-ci.yml            # CI/CD Pipeline
├── docker-compose.yml        # Docker Compose
└── README.md                 # Documentation
```

## API Endpoints Quick Reference

### Foods
- `GET /api/foods` - List all foods
- `POST /api/foods` - Create food (Admin)
- `PUT /api/foods/{id}` - Update food (Admin)
- `DELETE /api/foods/{id}` - Delete food (Admin)

### Orders
- `GET /api/orders` - List all orders
- `POST /api/orders` - Create order
- `PATCH /api/orders/{id}/status` - Update status
- `DELETE /api/orders/{id}` - Cancel order

### Auth
- `POST /api/auth/register` - Register user
- `POST /api/auth/login` - Login user

## Environment Configuration

### Frontend (.env)
```
ANGULAR_ENVIRONMENT=development
API_URL=https://localhost:7001/api
```

### Backend (appsettings.json)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Your_Connection_String"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

## Troubleshooting

### Port Already in Use
```powershell
# Kill process on port
netstat -ano | findstr :7001
taskkill /PID <PID> /F
```

### Database Connection Error
- Check SQL Server is running
- Verify connection string
- Check firewall settings

### npm Module Not Found
```powershell
npm install
npm cache clean --force
```

### CORS Issues
- Ensure backend CORS is configured
- Check API URL in frontend environment files
- Verify requests are from allowed origin

## Performance Tips

1. **Frontend:**
   - Enable production mode
   - Use lazy loading for routes
   - Optimize bundle size

2. **Backend:**
   - Enable response caching
   - Use async/await
   - Index database columns

3. **Database:**
   - Create indexes on frequently queried columns
   - Use connection pooling
   - Monitor query performance

## Security Checklist

- [ ] Change default connection strings
- [ ] Update authentication credentials
- [ ] Enable HTTPS in production
- [ ] Set strong passwords
- [ ] Configure firewall rules
- [ ] Update NuGet packages
- [ ] Update npm packages
- [ ] Review CORS policy
- [ ] Use environment variables for secrets
- [ ] Enable rate limiting

## Next Steps

1. Set up your database
2. Start the backend API
3. Start the frontend development server
4. Register a new account
5. Browse the menu
6. Place test orders
7. Check admin dashboard

---

For more details, see [README.md](./README.md)

**Happy Coding! 🚀**
