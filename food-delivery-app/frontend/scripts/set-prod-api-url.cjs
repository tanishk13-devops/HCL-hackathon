const fs = require('node:fs');
const path = require('node:path');

const envPath = path.join(__dirname, '..', 'src', 'environments', 'environment.prod.ts');
const apiUrl = process.env.NG_APP_API_URL || process.env.API_URL;

if (!apiUrl) {
  console.log('[set-prod-api-url] NG_APP_API_URL/API_URL not set. Keeping existing environment.prod.ts');
  process.exit(0);
}

const normalized = apiUrl.replace(/\/$/, '');
const content = `export const environment = {\n  production: true,\n  apiUrl: '${normalized}'\n};\n`;

fs.writeFileSync(envPath, content, 'utf8');
console.log(`[set-prod-api-url] environment.prod.ts updated with apiUrl=${normalized}`);
