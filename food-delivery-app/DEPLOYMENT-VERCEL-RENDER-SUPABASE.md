# Deployment Guide: Angular (Vercel) + API (Render) + Supabase PostgreSQL

This guide deploys:
- Frontend (Angular) -> Vercel
- Backend (.NET API) -> Render (Web Service)
- Database -> Supabase PostgreSQL

---

## 1) Frontend deployment (Vercel)

### What was prepared
- SPA rewrite config added: `frontend/vercel.json`
- Build-time API URL injection script added: `frontend/scripts/set-prod-api-url.cjs`
- Vercel env variable template added: `frontend/.env.vercel.example`
- Vercel build uses `npm run build:prod` (updates `environment.prod.ts` when `NG_APP_API_URL` exists)

### Vercel settings
- Root Directory: `frontend`
- Framework Preset: Angular
- Install Command: `npm ci`
- Build Command: `npm run build:prod`
- Output Directory: `dist/food-delivery-app`

### Vercel environment variable
- `NG_APP_API_URL=https://YOUR-RENDER-SERVICE.onrender.com/api`

### Deploy command (optional via CLI)
```bash
cd food-delivery-app/frontend
npm i -g vercel
vercel
vercel --prod
```

---

## 2) Backend deployment (Render)

### What was prepared
- Render blueprint updated: `render.yaml`
  - Deploys API only (no frontend/static service)
  - Uses env vars for DB/JWT/CORS
  - Includes `healthCheckPath: /health`
- API binds to Render `PORT` variable in `backend/Program.cs`
- Backend target framework aligned to .NET 8 in `backend/FoodDeliveryAPI.csproj`
- Render env template added: `backend/.env.render.example`

### Render service configuration
- Type: Web Service
- Runtime: Docker (`backend/Dockerfile`)
- Root directory: `backend`

### Required Render environment variables
- `DATABASE_URL` = Supabase PostgreSQL connection string
- `Jwt__Key` = strong JWT secret key
- `Jwt__Issuer` = `FoodDeliveryAPI`
- `Jwt__Audience` = `FoodDeliveryAPI.Client`
- `CORS__ALLOWED_ORIGINS` = your Vercel domain (comma-separated if multiple)
  - Example: `https://your-app.vercel.app`

Optional:
- `EnableSwagger=false`
- `ASPNETCORE_ENVIRONMENT=Production`

---

## 3) Supabase PostgreSQL setup

### Create project
1. Go to Supabase Dashboard.
2. Create a new project.
3. Open **Project Settings -> Database**.

### Get connection string
Use a pooled/direct Postgres URI from Supabase, for example:

```text
postgresql://postgres.<ref>:<password>@aws-0-<region>.pooler.supabase.com:6543/postgres?sslmode=require
```

Use this as `DATABASE_URL` in Render.

### Create tables
This API uses EF Core and runs `EnsureCreated()` on startup, so tables are auto-created from models when first run.
For production-grade schema evolution, switch to EF Core migrations.

---

## 4) Connect backend to Supabase

Already supported in `Program.cs`:
- Reads `DATABASE_URL`
- Converts URL to Npgsql connection string
- Uses PostgreSQL provider

So just set `DATABASE_URL` in Render and redeploy.

Health check endpoint available at:
- `https://YOUR-RENDER-SERVICE.onrender.com/health`

---

## 5) Point Angular to deployed Render API

Preferred (no file edits needed each deploy):
- Set `NG_APP_API_URL` in Vercel project environment variables
- Deploy frontend

Fallback manual option:
- Edit `frontend/src/environments/environment.prod.ts`
- Set `apiUrl: 'https://YOUR-RENDER-SERVICE.onrender.com/api'`

---

## 6) Deployment order

1. Deploy backend on Render with env vars (especially `DATABASE_URL`).
2. Confirm backend health:
  - `https://YOUR-RENDER-SERVICE.onrender.com/health`
3. Confirm API:
   - `https://YOUR-RENDER-SERVICE.onrender.com/api/restaurants`
4. Set `NG_APP_API_URL` in Vercel.
5. Deploy frontend on Vercel.
6. Set `CORS__ALLOWED_ORIGINS` in Render to Vercel URL and redeploy backend.

---

## 7) Suggested folder structure

```text
food-delivery-app/
  backend/
    Program.cs
    FoodDeliveryAPI.csproj
    Dockerfile
  frontend/
    vercel.json
    angular.json
    src/environments/
      environment.ts
      environment.prod.ts
  render.yaml
  DEPLOYMENT-VERCEL-RENDER-SUPABASE.md
```

---

## 8) Production best practices

1. Never commit real secrets (`DATABASE_URL`, `Jwt__Key`).
2. Use strong JWT key (>= 32 chars random).
3. Restrict CORS to exact frontend domains.
4. Disable Swagger in production unless required.
5. Add health-check endpoint and monitor uptime.
6. Add centralized logging and error monitoring.
7. Prefer EF migrations over `EnsureCreated()` for schema updates.
8. Use connection pooling and SSL-required DB connections.
9. Configure environment-specific settings only via env vars.
10. Enable automatic backups in Supabase.

---

## 9) Quick env var checklist

### Render
- `DATABASE_URL`
- `Jwt__Key`
- `Jwt__Issuer`
- `Jwt__Audience`
- `CORS__ALLOWED_ORIGINS`
- `ASPNETCORE_ENVIRONMENT=Production`
- `EnableSwagger=false`

### Vercel
- No secret required for this frontend build unless you add runtime config.
- Ensure `environment.prod.ts` points to Render API.
