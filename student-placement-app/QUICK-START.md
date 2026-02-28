# 🚀 Quick Start Guide

Get the Online Bookstore running in 5 minutes!

## Prerequisites Check

Make sure you have:
- ✅ Node.js installed (v14+)
- ✅ Angular CLI installed (`npm install -g @angular/cli`)
- ✅ Backend API running (see BACKEND-IMPLEMENTATION-GUIDE.md)

## Step 1: Install Dependencies

Open terminal in project folder:

```bash
npm install
```

## Step 2: Configure API URL (Optional)

If your backend is NOT running on `http://localhost:5000`, update the API URL:

**Option A: Use Environment Files (Recommended)**

Already created for you in `src/environments/`:
- `environment.ts` - Development
- `environment.prod.ts` - Production

**Option B: Update Service Files Directly**

Update these files:
- `src/app/services/auth.service.ts` (line 13)
- `src/app/services/book.service.ts` (line 11)
- `src/app/services/cart.service.ts` (line 13)
- `src/app/services/order.service.ts` (line 11)
- `src/app/services/review.service.ts` (line 11)

Change: `private apiUrl = 'http://localhost:5000/api/...';`

## Step 3: Run the Application

```bash
npm start
```

Or:

```bash
ng serve
```

The app will open automatically at: **http://localhost:4200**

## Step 4: Test the Application

### As a Customer:
1. Click **Register** and create an account
2. Browse books at `/books`
3. Click on a book to see details
4. Add books to cart
5. Go to cart and checkout
6. View your orders

### As an Admin:
1. Login with admin credentials (create in backend)
2. Navigate to **Admin** in navbar
3. Manage books and orders

## 🎯 Quick Testing Checklist

- [ ] Can register new user
- [ ] Can login
- [ ] Can browse books
- [ ] Can view book details
- [ ] Can add to cart
- [ ] Cart counter updates
- [ ] Can checkout
- [ ] Can view orders
- [ ] Can write review
- [ ] Admin can access admin panel

## 🐛 Common Issues & Fixes

### Issue: "Cannot GET /"
**Solution**: Make sure you're running `npm start` or `ng serve`

### Issue: CORS Error
**Solution**: 
1. Check backend CORS configuration
2. Ensure backend allows `http://localhost:4200`

### Issue: 401 Unauthorized
**Solution**:
1. Make sure you're logged in
2. Check if token is being sent (Network tab in browser)
3. Verify backend JWT configuration

### Issue: API calls fail
**Solution**:
1. Check backend is running
2. Verify API URL in services
3. Check browser console for errors

### Issue: Cart not updating
**Solution**:
1. Login first
2. Check API response in Network tab
3. Verify backend cart endpoints

## 📱 Application Routes

After starting, visit:

| Route | Description | Auth Required |
|-------|-------------|---------------|
| `/` | Home (redirects to /books) | No |
| `/login` | Login page | No |
| `/register` | Registration page | No |
| `/books` | Browse books | No |
| `/books/1` | Book details (example) | No |
| `/cart` | Shopping cart | Yes |
| `/orders` | Order history | Yes |
| `/admin` | Admin dashboard | Admin only |
| `/admin/books` | Manage books | Admin only |
| `/admin/orders` | Manage orders | Admin only |

## 🔐 Default Test Users

Create these in your backend for testing:

**Customer Account:**
- Email: `customer@example.com`
- Password: `Customer123!`
- Role: `Customer`

**Admin Account:**
- Email: `admin@example.com`
- Password: `Admin123!`
- Role: `Admin`

## 📦 Build for Production

```bash
ng build --configuration production
```

Output will be in: `dist/student-placement-app/`

## 🎨 Customization Quick Tips

### Change Colors
Edit component CSS files or `src/styles.css`

### Change Logo/Title
Edit `src/app/components/navbar/navbar.component.html`

### Add New Book Fields
1. Update `src/app/models/book.model.ts`
2. Update book components
3. Update backend accordingly

### Change Default Route
Edit `src/app/app-routing.module.ts` line 14

## 📚 Next Steps

1. ✅ **Read Full Documentation**: Check `BOOKSTORE-README.md`
2. ✅ **Set Up Backend**: Follow `BACKEND-IMPLEMENTATION-GUIDE.md`
3. ✅ **Explore Code**: Review component and service files
4. ✅ **Customize**: Adapt to your needs
5. ✅ **Deploy**: Build and deploy to production

## 🆘 Need Help?

1. Check `BOOKSTORE-README.md` - Comprehensive guide
2. Check `BACKEND-IMPLEMENTATION-GUIDE.md` - API specs
3. Review `PROJECT-SUMMARY.md` - Project overview
4. Check browser console for errors
5. Verify backend is running and accessible

## 🎉 You're Ready!

Your Online Bookstore is now running. Happy coding! 📚✨

---

**Pro Tip**: Open browser DevTools (F12) → Network tab to see API calls and debug issues.
