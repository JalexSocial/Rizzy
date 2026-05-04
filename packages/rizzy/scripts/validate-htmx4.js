const fs = require('node:fs');
const path = require('node:path');

const root = path.resolve(__dirname, '..');
const jsRoot = path.join(root, 'src/js');
const dist = path.join(root, 'dist/js/rizzy.js');

function fail(msg){ console.error(msg); process.exitCode = 1; }
function read(p){ return fs.existsSync(p) ? fs.readFileSync(p,'utf8') : ''; }

const antiMin = path.join(jsRoot, 'rizzy-antiforgery.min.js');
if (fs.existsSync(antiMin)) {
  const s = read(antiMin);
  [
    'document.addEventListener("htmx:config:request"',
    'document.addEventListener("htmx:after:request"',
    'e.detail.boosted',
    't.headers[o]=r'
  ].forEach(x => { if (s.includes(x)) fail(`stale antiforgery minified behavior found: ${x}`); });
}

const files = ['rizzy-antiforgery.js','rizzy-nonce.js','rizzy-streaming.js','rizzy-confirmation.js'];
for (const f of files) {
  const s = read(path.join(jsRoot,f));
  if (!s.includes('htmx.registerExtension(')) fail(`${f} must use htmx.registerExtension`);
  ['htmx.defineExtension(','onEvent','transformResponse','htmx:afterRequest','htmx:afterSwap','htmx:beforeRequest','htmx:beforeSwap','htmx:configRequest'].forEach(b => {
    if (s.includes(b)) fail(`${f} includes disallowed legacy API/event: ${b}`);
  });
}

const bundled = read(dist);
['rizzy-nonce','rizzy-streaming','rizzy-antiforgery','rizzy-confirm'].forEach(n=>{
  if (!bundled.includes(n)) fail(`bundle missing extension registration: ${n}`);
});

if (process.exitCode) process.exit(1);
console.log('validate-htmx4 passed');
