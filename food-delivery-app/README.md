# Online Food Delivery System

A complete web application for online food delivery with Angular frontend and ASP.NET Core backend.

## Live Demo

- App: `https://ziggy-frontend.onrender.com`
- API Swagger: `https://ziggy-api.onrender.com/swagger`

Update these links after your first deployment if your Render service names are different.

## Public Access From GitHub

Use the included `render.yaml` to deploy both frontend and backend so anyone visiting your GitHub can open and test Ziggy.

### One-time setup

1. Push this `food-delivery-app` folder to GitHub.
2. In Render, click `New +` -> `Blueprint`.
3. Connect your GitHub repository and choose the `food-delivery-app/render.yaml` blueprint.
4. Create both services when prompted.
5. After deploy completes, copy your real URLs and update the `Live Demo` section above.

### Notes

- Backend CORS can be controlled with `CORS__ALLOWED_ORIGINS` (comma-separated origins).
- Swagger in hosted environments is controlled by `EnableSwagger` (set to `true` in `render.yaml`).
- Frontend production API URL is injected at build time through `API_URL` in Render.
- Backend is configured for PostgreSQL. On Render, database connection is provisioned via `ziggy-db` in `render.yaml`.

## Features

### Customer Features
- Browse food menu by category
- Add items to cart
- Place orders
- Track order status
- View order history

### Admin Features
- Manage food items (Add/Update/Delete)
- Monitor all orders
- Update order status
- Track delivery activity

### Technical Features
- Real-time cart updates
- Order status tracking
- User authentication
- Responsive design
- RESTful API

## Project Structure

```
food-delivery-app/
├── frontend/              # Angular application
│   ├── src/
│   │   ├── app/
│   │   │   ├── components/
│   │   │   ├── services/
│   │   │   ├── models/
│   │   │   └── guards/
│   │   └── index.html
│   └── package.json
├── backend/               # ASP.NET Core API
│   ├── Models/
│   ├── Controllers/
│   ├── Data/
│   ├── Services/
│   └── Program.cs
├── .gitlab-ci.yml         # CI/CD Pipeline
└── README.md
```

## Tech Stack

### Frontend
- **Framework**: Angular 17
- **Language**: TypeScript
- **Styling**: CSS3
- **HTTP Client**: HttpClientModule

### Backend
- **Framework**: ASP.NET Core 8.0
- **Language**: C#
- **Database**: SQL Server / MySQL
- **ORM**: Entity Framework Core

### DevOps
- **Version Control**: Git
- **CI/CD**: GitLab CI/CD
- **Containerization**: Docker (Optional)

## Installation & Setup

### Prerequisites
- Node.js 18+
- .NET SDK 8.0
- SQL Server or MySQL
- Git

### Backend Setup

1. Navigate to backend folder:
```bash
cd backend
```

2. Update connection string in `appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Your_Connection_String_Here"
}
```

3. Restore dependencies:
```bash
dotnet restore
```

4. Create database:
```bash
dotnet ef database update
```

5. Run the API:
```bash
dotnet run
```

API will be available at `https://localhost:7001`

### Frontend Setup

1. Navigate to frontend folder:
```bash
cd frontend
```

2. Install dependencies:
```bash
npm install
```

3. Start development server:
```bash
npm start
```

Application will be available at `http://localhost:4200`

## API Endpoints

### Foods
- `GET /api/foods` - Get all foods
- `GET /api/foods/{id}` - Get food by ID
- `GET /api/foods/category/{category}` - Get foods by category
- `POST /api/foods` - Create food (Admin)
- `PUT /api/foods/{id}` - Update food (Admin)
- `DELETE /api/foods/{id}` - Delete food (Admin)

### Orders
- `GET /api/orders` - Get all orders
- `GET /api/orders/{id}` - Get order by ID
- `GET /api/orders/customer/{customerId}` - Get customer orders
- `POST /api/orders` - Place new order
- `PATCH /api/orders/{id}/status` - Update order status
- `DELETE /api/orders/{id}` - Cancel order

### Customers
- `GET /api/customers` - Get all customers
- `GET /api/customers/{id}` - Get customer details
- `POST /api/customers` - Create customer
- `PUT /api/customers/{id}` - Update customer
- `DELETE /api/customers/{id}` - Delete customer

### Authentication
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - Login user

## CI/CD Pipeline

The project includes a GitLab CI/CD pipeline with the following stages:

### 1. Restore Stage
- Restores .NET dependencies
- Installs npm packages

### 2. Build Stage
- Builds ASP.NET Core API
- Builds Angular application

### 3. Test Stage
- Runs unit tests for backend
- Runs unit tests for frontend

### 4. Publish Stage
- Publishes API artifacts
- Builds distribution package
- Optional Docker image build

### Running Pipeline Locally

```bash
# Backend build & test
cd backend
dotnet restore
dotnet build
dotnet test

# Frontend build & test
cd ../frontend
npm install
npm run build
npm test
```

## Database Schema

### Food Table
- Id (int, PK)
- Name (string)
- Description (string)
- Price (decimal)
- Category (string)
- ImageUrl (string, nullable)
- Availability (bool)
- CreatedAt (datetime)
- UpdatedAt (datetime)

### Order Table
- Id (int, PK)
- CustomerId (int, FK)
- CustomerName (string)
- CustomerPhone (string)
- TotalAmount (decimal)
- Status (string)
- CreatedAt (datetime)
- UpdatedAt (datetime)

### OrderItem Table
- Id (int, PK)
- OrderId (int, FK)
- FoodId (int)
- Quantity (int)
- Price (decimal)
- Subtotal (decimal)

## Usage Guide

### For Customers
1. Register/Login with email and password
2. Browse menu by category
3. Add items to cart
4. Proceed to checkout
5. Enter delivery details
6. Place order
7. Track order status

### For Admin
1. Login with admin credentials
2. Go to Admin Dashboard
3. Manage food items (Add/Edit/Delete)
4. View all orders
5. Update order status

## Performance Considerations

- Cart stored in browser localStorage
- Pagination ready for large datasets
- Optimized queries with Entity Framework
- Response caching enabled
- CORS enabled for frontend communication

## Security Features

- Password hashing with SHA256
- Email validation
- Input validation on both frontend and backend
- HTTPS enabled
- CORS policy configured

## Future Enhancements

- Payment gateway integration
- Real-time order notifications (SignalR)
- Email notifications
- Rating and review system
- Delivery tracking with map
- Mobile app version
- Advanced analytics dashboard
- Multi-language support

## Troubleshooting

### Database Connection Issues
- Check connection string in `appsettings.json`
- Ensure SQL Server is running
- Verify credentials

### Frontend Not Loading
- Clear browser cache
- Check if Angular development server is running
- Verify API URL in environment.ts

### API Not Responding
- Check if .NET API is running
- Verify port 7001 is available
- Check firewall settings

## Contributing

1. Clone the repository
2. Create a feature branch
3. Make changes
4. Test thoroughly
5. Commit with clear messages
6. Push and create pull request

## License

MIT License - Feel free to use this project for learning and development.

## Support

For issues and questions, please open an issue in the repository.

---

**Happy Coding! 🚀**
