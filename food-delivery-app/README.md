# Ziggy - Production-Style Online Food Delivery System

This project is upgraded to a Swiggy/Zomato-style architecture with restaurant-first browsing, JWT auth, role-based APIs, cart + checkout + payment flow, and admin operations.

## Stack

- Frontend: Angular 17
- Backend: ASP.NET Core Web API + EF Core
- Database: PostgreSQL by default (compatible patterns for SQL Server/MySQL)
- Auth: JWT + Roles (`Admin`, `Customer`, `DeliveryAgent`)
- Deployment: Vercel (frontend), Render/Azure (backend)
- CI/CD: GitHub Actions ([.github/workflows/ci-cd.yml](.github/workflows/ci-cd.yml))

## Architecture

Controller -> Service Layer -> Repository -> EF Core -> Database

Backend folders:

- Controllers
- Services/Interfaces, Services/Implementations
- Repositories/Interfaces, Repositories/Implementations
- Models
- DTOs
- Helpers

## Core Modules Delivered

1. Restaurant module (`Restaurant` -> many `Food` items)
2. Food menu management (admin create/update/delete)
3. Cart module (`Cart`, `CartItem`)
4. Order module + status workflow (`Pending -> Accepted -> Preparing -> OutForDelivery -> Delivered`)
5. Address module (multiple addresses per user)
6. Payment module (`UPI`, `Card`, `CashOnDelivery`)
7. Reviews & ratings
8. JWT Authentication + Authorization with roles
9. Admin dashboard APIs
10. Angular pages for customer/admin flows

## API Endpoints

### Auth
- `POST /api/auth/register`
- `POST /api/auth/login`

### Restaurants
- `GET /api/restaurants?search=`
- `GET /api/restaurants/{id}`
- `POST /api/restaurants` (Admin)
- `PUT /api/restaurants/{id}` (Admin)
- `DELETE /api/restaurants/{id}` (Admin)

### Food Items
- `GET /api/food-items/restaurant/{restaurantId}`
- `GET /api/food-items/{id}`
- `POST /api/food-items` (Admin)
- `PUT /api/food-items/{id}` (Admin)
- `DELETE /api/food-items/{id}` (Admin)

### Cart
- `GET /api/cart`
- `POST /api/cart/items`
- `PUT /api/cart/items/{foodItemId}`
- `DELETE /api/cart/items/{foodItemId}`

### Orders
- `POST /api/orders` (Place order from cart)
- `GET /api/orders/my-history`
- `GET /api/orders/{id}`
- `GET /api/orders/{id}/track`
- `GET /api/orders` (Admin/DeliveryAgent)
- `PATCH /api/orders/{id}/status` (Admin/DeliveryAgent)

### Addresses
- `GET /api/addresses`
- `POST /api/addresses`
- `DELETE /api/addresses/{id}`

### Payments
- `GET /api/payments/order/{orderId}`
- `PATCH /api/payments/order/{orderId}` (Admin)

### Reviews
- `GET /api/reviews/restaurant/{restaurantId}`
- `POST /api/reviews`

### Admin
- `GET /api/admin/dashboard`
- `GET /api/admin/orders`
- `GET /api/admin/customer-activity`

## Database Schema (High Level)

- `Users` (Id, Name, Email, PasswordHash, Role, CreatedAt)
- `Restaurants` (Id, Name, Description, Location, Rating, ImageUrl, CreatedAt)
- `FoodCategories` (Id, Name)
- `Foods` (Id, Name, Description, Price, CategoryId, RestaurantId, ImageUrl, IsAvailable, CreatedAt)
- `Carts` (Id, UserId, CreatedAt)
- `CartItems` (Id, CartId, FoodItemId, Quantity, Price)
- `Addresses` (Id, UserId, Street, City, State, Pincode)
- `Orders` (Id, UserId, AddressId, TotalAmount, Status, CreatedAt)
- `OrderItems` (Id, OrderId, FoodItemId, Quantity, Price)
- `Payments` (Id, OrderId, PaymentMethod, PaymentStatus, Amount, CreatedAt)
- `Reviews` (Id, UserId, RestaurantId, Rating, Comment, CreatedAt)

## Angular Service Files

- [src/app/services/auth.service.ts](frontend/src/app/services/auth.service.ts)
- [src/app/services/restaurant.service.ts](frontend/src/app/services/restaurant.service.ts)
- [src/app/services/food.service.ts](frontend/src/app/services/food.service.ts)
- [src/app/services/cart.service.ts](frontend/src/app/services/cart.service.ts)
- [src/app/services/address.service.ts](frontend/src/app/services/address.service.ts)
- [src/app/services/order.service.ts](frontend/src/app/services/order.service.ts)
- [src/app/services/payment.service.ts](frontend/src/app/services/payment.service.ts) *(can be added similarly as needed)*
- [src/app/services/review.service.ts](frontend/src/app/services/review.service.ts)
- [src/app/services/admin.service.ts](frontend/src/app/services/admin.service.ts)

## Sample Seed Users

- Admin: `admin@ziggy.com` / `Admin@123`
- Customer: `customer@ziggy.com` / `Customer@123`
- DeliveryAgent: `delivery@ziggy.com` / `Delivery@123`

## Local Run

Backend:

1. `cd backend`
2. `dotnet restore`
3. `dotnet run`

Frontend:

1. `cd frontend`
2. `npm install`
3. `npm start`

## Deployment

### Frontend -> Vercel

- Build command: `npm run build`
- Output: `dist/food-delivery-app`
- Set `environment.prod.ts` API URL to backend URL

### Backend -> Render/Azure

- Configure `DATABASE_URL` or `ConnectionStrings__DefaultConnection`
- Configure JWT values:
  - `Jwt__Key`
  - `Jwt__Issuer`
  - `Jwt__Audience`
- Optional CORS env: `CORS__ALLOWED_ORIGINS`

### CI/CD

GitHub Actions pipeline includes:

1. Restore dependencies
2. Build backend/frontend
3. Run tests
4. Publish and upload artifacts

## License

MIT License - Feel free to use this project for learning and development.

## Support

For issues and questions, please open an issue in the repository.

---

**Happy Coding! 🚀**
