# Online Bookstore - Angular Frontend

A full-featured online bookstore web application built with Angular 15, designed to work with an ASP.NET Core backend. This application allows users to browse books, manage their shopping cart, place orders, and leave reviews. Administrators can manage books and orders through a dedicated admin panel.

## 🚀 Features

### Customer Features
- **User Authentication**
  - User registration with email and password
  - Secure login system with JWT authentication
  - Role-based access control (Customer/Admin)

- **Book Browsing**
  - Browse all available books
  - Search by title or author
  - Filter by author and price range
  - View detailed book information
  - See average ratings and reviews

- **Shopping Cart**
  - Add books to cart
  - Update quantities
  - Remove items
  - Real-time cart item count in navbar
  - Persistent cart across sessions

- **Order Management**
  - Create orders from cart
  - View order history
  - Track order status (Pending, Confirmed, Shipped, Delivered, Cancelled)
  - Cancel pending orders

- **Reviews**
  - Write reviews for books
  - Rate books (1-5 stars)
  - View all reviews for a book

### Admin Features
- **Dashboard**
  - View total books, orders, and revenue
  - See pending orders count
  - Quick access to management tools
  - View recent orders

- **Book Management**
  - Add new books
  - Edit existing books
  - Delete books
  - Manage stock quantities

- **Order Management**
  - View all orders
  - Filter orders by status
  - Update order status
  - Track order details

## 📋 Prerequisites

Before you begin, ensure you have the following installed:
- **Node.js** (v14.x or higher) - [Download](https://nodejs.org/)
- **npm** (v6.x or higher) - Comes with Node.js
- **Angular CLI** (v15.x) - Install globally with `npm install -g @angular/cli`
- **ASP.NET Core Backend** - Running on http://localhost:5000

## 🛠️ Technology Stack

- **Angular**: 15.2.0
- **TypeScript**: 4.8.4
- **RxJS**: 7.5.0
- **Angular Router**: For navigation
- **Angular Forms**: Reactive Forms for form handling
- **Angular HttpClient**: For API communication
- **CSS3**: For styling

## 📁 Project Structure

```
src/
├── app/
│   ├── components/                 # All UI components
│   │   ├── navbar/                # Navigation bar
│   │   ├── login/                 # Login page
│   │   ├── register/              # Registration page
│   │   ├── book-list/             # Books listing page
│   │   ├── book-details/          # Book details page
│   │   ├── cart/                  # Shopping cart page
│   │   ├── orders/                # Order history page
│   │   ├── admin-dashboard/       # Admin dashboard
│   │   ├── admin-books/           # Admin book management
│   │   └── admin-orders/          # Admin order management
│   │
│   ├── models/                    # TypeScript interfaces
│   │   ├── user.model.ts          # User, Login, Register models
│   │   ├── book.model.ts          # Book models
│   │   ├── cart.model.ts          # Cart and CartItem models
│   │   ├── order.model.ts         # Order and OrderItem models
│   │   └── review.model.ts        # Review models
│   │
│   ├── services/                  # Business logic services
│   │   ├── auth.service.ts        # Authentication service
│   │   ├── book.service.ts        # Book operations
│   │   ├── cart.service.ts        # Cart operations
│   │   ├── order.service.ts       # Order operations
│   │   └── review.service.ts      # Review operations
│   │
│   ├── guards/                    # Route guards
│   │   ├── auth.guard.ts          # Protect authenticated routes
│   │   └── admin.guard.ts         # Protect admin routes
│   │
│   ├── interceptors/              # HTTP interceptors
│   │   └── auth.interceptor.ts    # Add JWT token to requests
│   │
│   ├── app-routing.module.ts      # Application routes
│   ├── app.module.ts              # Root module
│   ├── app.component.ts           # Root component
│   └── app.component.html         # Root template
│
├── assets/                        # Static assets
├── styles.css                     # Global styles
└── index.html                     # Main HTML file
```

## 🚀 Installation & Setup

### Step 1: Clone or Navigate to the Project

```bash
cd student-placement-app
```

### Step 2: Install Dependencies

```bash
npm install
```

### Step 3: Configure API URL

Update the API URL in all service files to match your backend server:

**Files to update:**
- `src/app/services/auth.service.ts` - Line 13
- `src/app/services/book.service.ts` - Line 11
- `src/app/services/cart.service.ts` - Line 13
- `src/app/services/order.service.ts` - Line 11
- `src/app/services/review.service.ts` - Line 11

Change `http://localhost:5000/api` to your backend URL.

Example:
```typescript
private apiUrl = 'http://localhost:5000/api/books'; // Your backend URL
```

### Step 4: Run the Development Server

```bash
npm start
```

Or:

```bash
ng serve
```

The application will be available at `http://localhost:4200/`

### Step 5: Build for Production

```bash
npm run build
```

Or:

```bash
ng build
```

The build artifacts will be stored in the `dist/` directory.

## 🔧 Backend API Requirements

The frontend expects the following API endpoints from the ASP.NET Core backend:

### Authentication Endpoints
```
POST   /api/auth/register          - Register new user
POST   /api/auth/login             - Login user
```

### Book Endpoints
```
GET    /api/books                  - Get all books (with optional filters)
GET    /api/books/{id}             - Get book by ID
POST   /api/books                  - Create book (Admin only)
PUT    /api/books/{id}             - Update book (Admin only)
DELETE /api/books/{id}             - Delete book (Admin only)
```

### Cart Endpoints
```
GET    /api/cart                   - Get user's cart
POST   /api/cart/items             - Add item to cart
PUT    /api/cart/items/{id}        - Update cart item quantity
DELETE /api/cart/items/{id}        - Remove item from cart
DELETE /api/cart                   - Clear cart
```

### Order Endpoints
```
POST   /api/orders                 - Create order
GET    /api/orders/my-orders       - Get user's orders
GET    /api/orders/{id}            - Get order by ID
GET    /api/orders                 - Get all orders (Admin only)
PUT    /api/orders/{id}/status     - Update order status (Admin only)
```

### Review Endpoints
```
GET    /api/reviews/book/{bookId}  - Get reviews for a book
POST   /api/reviews                - Create review
PUT    /api/reviews/{id}           - Update review
DELETE /api/reviews/{id}           - Delete review
GET    /api/reviews/my-reviews     - Get user's reviews
```

### Expected Response Formats

**Login/Register Response:**
```json
{
  "token": "jwt-token-here",
  "user": {
    "userId": 1,
    "firstName": "John",
    "lastName": "Doe",
    "email": "john@example.com",
    "role": "Customer",
    "createdAt": "2024-02-28T10:00:00Z"
  }
}
```

**Book Response:**
```json
{
  "bookId": 1,
  "title": "Book Title",
  "author": "Author Name",
  "price": 29.99,
  "description": "Book description...",
  "stockQuantity": 50,
  "imageUrl": "https://example.com/image.jpg",
  "averageRating": 4.5,
  "reviewCount": 10,
  "createdAt": "2024-02-28T10:00:00Z"
}
```

## 🎨 Key Components Overview

### Authentication System
- **JWT-based authentication** with token storage in localStorage
- **Auth interceptor** automatically adds tokens to HTTP requests
- **Route guards** protect authenticated and admin routes
- **Current user observable** for reactive UI updates

### State Management
- **BehaviorSubject** for current user state
- **BehaviorSubject** for cart item count
- Services maintain application state

### Routing
- **Lazy loading ready** structure
- **Route guards** for authentication and authorization
- **Redirect after login** to return URL

### Form Validation
- **Reactive Forms** with built-in validators
- **Custom validators** for password matching
- **Real-time validation feedback**

## 👥 Default User Roles

The application supports two user roles:

1. **Customer** - Can browse, purchase, and review books
2. **Admin** - Has all customer permissions plus:
   - Manage books (Create, Update, Delete)
   - Manage orders (View all, Update status)
   - Access admin dashboard

## 🔐 Security Features

- **JWT Authentication** - Secure token-based authentication
- **Route Guards** - Protect unauthorized access
- **HTTP Interceptor** - Automatically attach auth tokens
- **Role-based Access Control** - Admin vs Customer permissions
- **Password Hashing** - Passwords are never stored in plain text (backend)

## 📱 Responsive Design

The application is fully responsive and works on:
- Desktop (1200px+)
- Tablet (768px - 1199px)
- Mobile (320px - 767px)

## 🎯 User Workflows

### Customer Workflow
1. Register/Login
2. Browse books
3. View book details and reviews
4. Add books to cart
5. View/Update cart
6. Checkout and create order
7. View order history
8. Leave reviews

### Admin Workflow
1. Login as admin
2. Access admin dashboard
3. Manage books (Add/Edit/Delete)
4. View and manage all orders
5. Update order statuses

## 🐛 Troubleshooting

### Common Issues

**Issue: CORS errors**
- Solution: Ensure your backend has CORS enabled for `http://localhost:4200`

**Issue: 401 Unauthorized errors**
- Solution: Check if JWT token is being sent with requests
- Verify token hasn't expired
- Ensure user is logged in

**Issue: API not found (404)**
- Solution: Verify backend is running on the correct port
- Check API URLs in service files

**Issue: Cart count not updating**
- Solution: Ensure cart service is subscribed to in components
- Check if API returns proper cart data

## 📝 Environment Configuration

For different environments (dev, staging, production), update the API URLs in:

1. Create environment files:
   - `src/environments/environment.ts` (development)
   - `src/environments/environment.prod.ts` (production)

2. Add API URL configuration:
```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000/api'
};
```

3. Use in services:
```typescript
import { environment } from '../../environments/environment';

private apiUrl = `${environment.apiUrl}/books`;
```

## 🚀 Deployment

### Build for Production

```bash
ng build --configuration production
```

### Deploy to Azure, AWS, or any static hosting:

1. Build the application
2. Upload contents of `dist/student-placement-app/` to your hosting
3. Configure server to redirect all routes to `index.html`
4. Update environment API URLs

### Server Configuration

For Angular routing to work, configure your server to:
- Serve `index.html` for all routes
- Enable HTTPS
- Set proper CORS headers

Example nginx configuration:
```nginx
location / {
  try_files $uri $uri/ /index.html;
}
```

## 📚 Additional Resources

- [Angular Documentation](https://angular.io/docs)
- [Angular CLI Documentation](https://angular.io/cli)
- [RxJS Documentation](https://rxjs.dev/)
- [TypeScript Documentation](https://www.typescriptlang.org/docs/)

## 🤝 Contributing

This is a project template. Feel free to customize and extend based on your requirements.

## 📄 License

This project is provided as-is for educational and development purposes.

## 👨‍💻 Support

For issues or questions:
1. Check the troubleshooting section
2. Review the API documentation
3. Ensure backend is properly configured
4. Verify all dependencies are installed

---

**Happy Coding! 📚✨**
