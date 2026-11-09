import test from 'node:test';
import assert from 'node:assert/strict';
import fs from 'node:fs';
import { JSDOM } from 'jsdom';

function bootDom() {
  const dom = new JSDOM('<!doctype html><html><head></head><body><div id="target"></div></body></html>', { url: 'http://localhost' });
  global.window = dom.window; global.document = dom.window.document; global.DOMParser = dom.window.DOMParser;
  global.Headers = dom.window.Headers; global.customElements = dom.window.customElements; global.Node = dom.window.Node;
  global.NodeFilter = dom.window.NodeFilter; global.Range = dom.window.Range; global.HTMLElement = dom.window.HTMLElement;
  return dom;
}

function load(file, htmx) { global.htmx = htmx; (0,eval)(fs.readFileSync(file,'utf8')); }

test('antiforgery injects by method and header types', () => {
  bootDom();
  let ext;
  const htmx = { config:{ antiforgery:{headerName:'X-CSRF',requestToken:'abc',formFieldName:'__RequestVerificationToken'}}, registerExtension:(n,e)=>ext=e };
  load('src/js/rizzy-antiforgery.js', htmx);
  const run=(method, headers={})=>{ const detail={ctx:{request:{method,headers,body:new URLSearchParams()}}}; ext.htmx_config_request(null,detail); return detail; };
  assert.equal(run('GET').ctx.request.headers['X-CSRF'], undefined);
  assert.equal(run('HEAD').ctx.request.headers['X-CSRF'], undefined);
  assert.equal(run('POST').ctx.request.headers['X-CSRF'], 'abc');
  const hs=new Headers(); run('DELETE', hs); assert.equal(hs.get('X-CSRF'),'abc');
});

test('nonce rewriting promotes the response nonce and strips a stolen page nonce', () => {
  bootDom();
  let ext;
  const htmx = { config:{documentNonce:'docnonce'}, registerExtension:(_n,e)=>ext=e };
  load('src/js/rizzy-nonce.js', htmx);
  const headers = new Headers([['HX-Nonce','resnonce']]);
  const dirty = '<script nonce="resnonce">ok()</script><script nonce="docnonce">stolen()</script><script>injected()</script><style nonce=resnonce></style>';
  const clean = window.Rizzy.nonce.processResponseHtml(dirty,{headers});
  assert.equal(clean, '<script nonce="docnonce">ok()</script><script>stolen()</script><script>injected()</script><style nonce=docnonce></style>');
  const d={ctx:{text:dirty,response:{headers}}}; ext.htmx_after_request(null,d); assert.equal(d.ctx.text, clean);
});

test('nonce rewriting never re-parses markup', () => {
  bootDom();
  const htmx = { config:{documentNonce:'docnonce'}, registerExtension:()=>{} };
  load('src/js/rizzy-nonce.js', htmx);
  const rows = '<tr id="r"><td nonce="x">Cell</td></tr><link rel="stylesheet" href="/a.css">';
  assert.equal(window.Rizzy.nonce.processResponseHtml(rows, { headers: new Headers() }), rows);
});

test('nonce rewriting is a no-op when the page has no nonce', () => {
  bootDom();
  const htmx = { config:{}, registerExtension:()=>{} };
  load('src/js/rizzy-nonce.js', htmx);
  const html = '<script nonce="resnonce"></script><style></style>';
  assert.equal(window.Rizzy.nonce.processResponseHtml(html, { headers: new Headers([['HX-Nonce','resnonce']]) }), html);
});

test('page nonce is read from the script nonce property', () => {
  const dom = new JSDOM('<!doctype html><html><head><script nonce="fromscript"></script></head><body></body></html>', { url: 'http://localhost' });
  global.window = dom.window; global.document = dom.window.document; global.Headers = dom.window.Headers;
  const htmx = { config:{documentNonce:'fromconfig'}, registerExtension:()=>{} };
  load('src/js/rizzy-nonce.js', htmx);
  assert.equal(window.Rizzy.nonce.getDocumentNonce(), 'fromscript');
});
