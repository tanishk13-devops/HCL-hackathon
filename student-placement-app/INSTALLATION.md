# 🎯 Installation Instructions

Follow these steps to get your Online Bookstore up and running!

## 📋 Prerequisites

Before you begin, ensure you have:

1. **Node.js** (v14 or higher)
   - Download: https://nodejs.org/
   - Verify: `node --version`

2. **npm** (comes with Node.js)
   - Verify: `npm --version`

3. **Angular CLI** (v15.x)
   - Install: `npm install -g @angular/cli`
   - Verify: `ng version`

## 🚀 Step-by-Step Installation

### Step 1: Navigate to Project Directory

```bash

```

### Step 2: Install Dependencies

```bash
npm install
```

This will install:
- @angular/router (for routing)
- All other Angular dependencies
- Development dependencies

**Expected output:**
```
added XXX packages in XXs
```

### Step 3: Verify Installation

```bash
ng version
```

You should see Angular CLI and all packages listed.

### Step 4: Configure Backend URL (if needed)

If your backend is NOT on `http://localhost:5000`:

**Update these files:**
```
src/app/services/auth.service.ts (line 13)
src/app/services/book.service.ts (line 11)
src/app/services/cart.service.ts (line 13)
src/app/services/order.service.ts (line 11)
src/app/services/review.service.ts (line 11)
```

**Or use environment files** (recommended):
```typescript
// src/environments/environment.ts
export const environment = {
  production: false,
  apiUrl: 'http://YOUR-BACKEND-URL/api'
};
```

### Step 5: Start Development Server

```bash
npm start
```

Or:

```bash
ng serve
```

**Expected output:**
```
** Angular Live Development Server is listening on localhost:4200 **
✔ Compiled successfully.
```

### Step 6: Open in Browser

The application will automatically open at:
```
http://localhost:4200
```

If not, manually navigate to that URL.

## ✅ Verification Checklist

After installation, verify:

- [ ] No compilation errors in terminal
- [ ] Application loads in browser
- [ ] You can see the navigation bar
- [ ] You can navigate to /books
- [ ] No console errors (F12 → Console)

## 🔧 Troubleshooting

### Error: "Cannot find module '@angular/router'"

**Solution:**
```bash
npm install @angular/router@^15.2.0
```

### Error: "ng is not recognized"

**Solution:**
```bash
npm install -g @angular/cli
```

### Error: Port 4200 already in use

**Solution:**
```bash
ng serve --port 4201
```

### Error: "Cannot GET /"

**Solution:**
- Stop the server (Ctrl+C)
- Run `ng serve` again

### Error: npm install fails

**Solution:**
```bash
# Clear npm cache
npm cache clean --force

# Delete node_modules and package-lock.json
rm -rf node_modules package-lock.json

# Reinstall
npm install
```

## 📦 What Gets Installed

### Dependencies (Production)
- `@angular/animations` - Angular animations
- `@angular/common` - Angular common utilities
- `@angular/compiler` - Angular template compiler
- `@angular/core` - Angular core framework
- `@angular/forms` - Form handling
- `@angular/platform-browser` - Browser support
- `@angular/platform-browser-dynamic` - Dynamic compilation
- `@angular/router` - Routing and navigation
- `rxjs` - Reactive programming
- `zone.js` - Change detection

### Dev Dependencies
- `@angular/cli` - Angular command-line tools
- `@angular/compiler-cli` - Compiler for production builds
- `@angular-devkit/build-angular` - Build tools
- `typescript` - TypeScript compiler

## 🎯 Next Steps After Installation

1. ✅ **Backend Setup**
   - Follow `BACKEND-IMPLEMENTATION-GUIDE.md`
   - Ensure backend is running
   - Test API endpoints

2. ✅ **Test Application**
   - Register a new user
   - Browse books
   - Test cart functionality
   - Place an order

3. ✅ **Create Admin User**
   - Use backend to create admin user
   - Test admin features

4. ✅ **Customize**
   - Update branding
   - Modify colors/styles
   - Add your books

## 📂 Folder Structure After Installation

```
student-placement-app/
├── node_modules/          # ← Installed packages (don't commit)
├── src/
│   ├── app/
│   │   ├── components/    # All UI components
│   │   ├── models/        # TypeScript interfaces
│   │   ├── services/      # Business logic
│   │   ├── guards/        # Route protection
│   │   └── interceptors/  # HTTP interceptors
│   ├── environments/      # Environment configs
│   └── assets/           # Static files
├── package.json          # ← Updated with dependencies
├── package-lock.json     # ← Lock file (auto-generated)
└── README files          # Documentation
```

## 🚀 Quick Commands Reference

```bash
# Install dependencies
npm install

# Start development server
npm start
# or
ng serve

# Build for production
npm run build
# or
ng build --configuration production

# Run tests
npm test

# Check for errors
ng lint

# Generate new component
ng generate component component-name
```

## 🌐 Port Configuration

Default: `http://localhost:4200`

**To change port:**
```bash
ng serve --port 4201
```

**Or update package.json:**
```json
"start": "ng serve --port 4201 --open"
```

## 🔒 Important Notes

1. **Don't commit node_modules/** - Already in .gitignore
2. **Keep package-lock.json** - Ensures consistent installs
3. **Backend Required** - Frontend alone won't work
4. **CORS Must Be Configured** - In backend for http://localhost:4200
5. **Environment Files** - Don't commit with sensitive data

## ✨ Ready to Go!

After following these steps, you should have:
- ✅ All dependencies installed
- ✅ Development server running
- ✅ Application accessible in browser
- ✅ No compilation errors

**Next:** Read `QUICK-START.md` for testing the application!

---

**Need Help?** Check the troubleshooting section or refer to other documentation files.
