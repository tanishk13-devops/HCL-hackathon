# Deployment Guide: Angular (Vercel) + API (Koyeb) + Supabase PostgreSQL

This guide deploys:
- Frontend (Angular) -> Vercel
- Backend (.NET API) -> Koyeb (Docker Web Service)
- Database -> Supabase PostgreSQL

---

## 1) Frontend deployment (Vercel)

### What is already prepared
- SPA rewrite config: `frontend/vercel.json`
- Build-time API URL injection script: `frontend/scripts/set-prod-api-url.cjs`
- Vercel env template: `frontend/.env.vercel.example`
- Production build command: `npm run build:prod`

### Vercel settings
- Root Directory: `frontend`
- Framework Preset: Angular
- Install Command: `npm ci`
- Build Command: `npm run build:prod`
- Output Directory: `dist/food-delivery-app`

### Vercel environment variable
- `NG_APP_API_URL=https://YOUR-KOYEB-SERVICE.koyeb.app/api`

---

## 2) Backend deployment (Koyeb)

### What is already prepared
- Docker-ready backend: `backend/Dockerfile`
- API listens on provider `PORT` env var: `backend/Program.cs`
- .NET 8 target: `backend/FoodDeliveryAPI.csproj`
- Koyeb env template: `backend/.env.koyeb.example`

### Koyeb service setup
1. Open Koyeb Dashboard -> Create App.
2. Choose GitHub repository.
3. Select this repo and branch (`main`).
4. Service type: **Web Service**.
5. Build method: **Dockerfile**.
6. Dockerfile path: `food-delivery-app/backend/Dockerfile`.
7. Build context / workdir: `food-delivery-app/backend`.

### Required Koyeb environment variables
- `DATABASE_URL` = Supabase PostgreSQL URI
- `Jwt__Key` = strong JWT secret (32+ chars)
- `Jwt__Issuer` = `FoodDeliveryAPI`
- `Jwt__Audience` = `FoodDeliveryAPI.Client`
- `CORS__ALLOWED_ORIGINS` = your Vercel URL (comma-separated if multiple)

Optional:
- `ASPNETCORE_ENVIRONMENT=Production`
- `EnableSwagger=false`

---

## 3) Supabase PostgreSQL setup

1. Create a Supabase project.
2. Open **Project Settings -> Database**.
3. Copy PostgreSQL URI, example:

`postgresql://postgres.<ref>:<password>@aws-0-<region>.pooler.supabase.com:6543/postgres?sslmode=require`

4. Set it as `DATABASE_URL` in Koyeb.

> This API uses EF Core `EnsureCreated()` on startup, so tables are created on first run.

---

## 4) Connect frontend to backend

1. Deploy backend on Koyeb.
2. Verify backend:
   - `https://YOUR-KOYEB-SERVICE.koyeb.app/health`
   - `https://YOUR-KOYEB-SERVICE.koyeb.app/api/restaurants`
3. In Vercel set:
   - `NG_APP_API_URL=https://YOUR-KOYEB-SERVICE.koyeb.app/api`
4. Deploy frontend.
5. Update backend `CORS__ALLOWED_ORIGINS` in Koyeb to your Vercel URL.
6. Redeploy backend.

---

## 5) Deployment order

1. Supabase database
2. Koyeb backend
3. Vercel frontend
4. Fix exact CORS origin and redeploy backend
5. Test on laptop + mobile

---

## 6) Quick env checklist

### Koyeb
- `DATABASE_URL`
- `Jwt__Key`
- `Jwt__Issuer`
- `Jwt__Audience`
- `CORS__ALLOWED_ORIGINS`
- `ASPNETCORE_ENVIRONMENT=Production` (optional)
- `EnableSwagger=false` (optional)

### Vercel
- `NG_APP_API_URL=https://YOUR-KOYEB-SERVICE.koyeb.app/api`
