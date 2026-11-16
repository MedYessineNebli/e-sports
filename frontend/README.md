# ArenaHQ frontend

React + TypeScript dashboard for the Tournois E-Sport API.

## Run locally

```powershell
npm install
npm run dev
```

The app uses `http://localhost:5192/api` by default. To point it at another
API instance, create `.env.local`:

```env
VITE_API_URL=https://localhost:7007/api
```

Sign in with a user created by `POST /api/auth/register`. The JWT is kept in
browser storage and attached to protected API requests automatically.

## Checks

```powershell
npm run lint
npm run build
```
