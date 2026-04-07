import { HttpInterceptorFn } from '@angular/common/http';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const renderApiBase = 'https://ziggy-u65z.onrender.com';

  let url = req.url;

  if (url.startsWith('/api/')) {
    url = `${renderApiBase}${url}`;
  } else if (url.startsWith('http')) {
    try {
      const parsed = new URL(url);
      if (parsed.pathname.startsWith('/api/') && parsed.hostname.endsWith('vercel.app')) {
        url = `${renderApiBase}${parsed.pathname}${parsed.search}`;
      }
    } catch {
      // keep original URL
    }
  }

  const token = localStorage.getItem('token');
  const isAuthEndpoint = url.includes('/api/auth/');
  const authReq = req.clone(
    token && !isAuthEndpoint
      ? {
          url,
          setHeaders: { Authorization: `Bearer ${token}` }
        }
      : {
          url
        }
  );

  return next(authReq);
};
