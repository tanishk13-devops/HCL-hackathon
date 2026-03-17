# Deployment Guide: Angular (Vercel) + API (Render) + Supabase PostgreSQL


This guide deploys:
- Frontend (Angular) -> Vercel
- Backend (.NET API) -> Render (Web Service)
- Database -> Supabase PostgreSQL

Goal: users access the app with one URL (your Vercel URL) from mobile or laptop.

---

## 1) Frontend deployment (Vercel)

### What is already prepared
- SPA rewrite config: `frontend/vercel.json`
- Build-time API URL injection script: `frontend/scripts/set-prod-api-url.cjs`
- Vercel env template: `frontend/.env.vercel.example`
- Production `apiUrl` default is same-origin: `/api`

### Vercel settings
- Root Directory: `frontend`
- Framework Preset: Angular
- Install Command: `npm ci`
- Build Command: `npm run build:prod`
- Output Directory: `dist/food-delivery-app`

### Vercel environment variable
- `NG_APP_API_URL=/api`

### Required Vercel rewrite for single URL
In Vercel Project -> Settings -> Rewrites, add:
- Source: `/api/:path*`
- Destination: `https://YOUR-RENDER-SERVICE.onrender.com/api/:path*`

This makes browser calls stay on your Vercel domain while Vercel proxies API to Render.

---

## 2) Backend deployment (Render)

### What is already prepared
- Render blueprint: `render.yaml` (API service only)
- API binds to provider `PORT` in `backend/Program.cs`
- Docker setup in `backend/Dockerfile`
- Render env template in `backend/.env.render.example`

### Render setup
1. Render -> New -> Blueprint.
2. Connect your GitHub repo.
3. Deploy from root `render.yaml`.

### Required Render env vars
- `DATABASE_URL` = Supabase Postgres URI
- `Jwt__Key` = strong JWT key (32+ chars)
- `Jwt__Issuer` = `FoodDeliveryAPI`
- `Jwt__Audience` = `FoodDeliveryAPI.Client`
- `CORS__ALLOWED_ORIGINS` = your Vercel app URL

Optional:
- `ASPNETCORE_ENVIRONMENT=Production`
- `EnableSwagger=false`

---

## 3) Supabase PostgreSQL setup

1. Create project in Supabase.
2. Open **Project Settings -> Database**.
3. Copy URI, e.g.:

`postgresql://postgres.<ref>:<password>@aws-0-<region>.pooler.supabase.com:6543/postgres?sslmode=require`

4. Set it in Render as `DATABASE_URL`.

---

## 4) Verification

1. Backend checks:
   - `https://YOUR-RENDER-SERVICE.onrender.com/health`
   - `https://YOUR-RENDER-SERVICE.onrender.com/api/restaurants`
2. Frontend check:
   - `https://YOUR-APP.vercel.app`
3. Open same Vercel URL on laptop and mobile.

---

## 5) Final production order

1. Deploy backend on Render (with Supabase env vars)
2. Deploy frontend on Vercel
3. Add Vercel rewrite `/api/:path*` -> Render API
4. Set backend `CORS__ALLOWED_ORIGINS` = Vercel domain
5. Redeploy backend and test

---

## 6) Quick env checklist

### Render
- `DATABASE_URL`
- `Jwt__Key`
- `Jwt__Issuer`
- `Jwt__Audience`
- `CORS__ALLOWED_ORIGINS`

### Vercel
- `NG_APP_API_URL=/api`
