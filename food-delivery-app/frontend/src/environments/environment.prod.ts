export const environment = {
  production: true,
  // Use same-origin API path so frontend can stay on a single public URL.
  // Configure a Vercel rewrite from /api/* -> Render backend /api/*.
  apiUrl: '/api'
};
