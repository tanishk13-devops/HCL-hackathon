const fs = require('node:fs');
const path = require('node:path');

const envPath = path.join(__dirname, '..', 'src', 'environments', 'environment.prod.ts');
const directRenderApiUrl = 'https://ziggy-u65z.onrender.com/api';
const configuredApiUrl = process.env.NG_APP_API_URL || process.env.API_URL;

let apiUrl = configuredApiUrl;

if (!apiUrl || apiUrl.trim() === '/api' || apiUrl.trim() === '/api/') {
  apiUrl = directRenderApiUrl;
}

console.log(`[set-prod-api-url] Using apiUrl=${apiUrl}`);

const normalized = apiUrl.replace(/\/$/, '');
const content = `export const environment = {\n  production: true,\n  apiUrl: '${normalized}'\n};\n`;

fs.writeFileSync(envPath, content, 'utf8');
console.log(`[set-prod-api-url] environment.prod.ts updated with apiUrl=${normalized}`);
