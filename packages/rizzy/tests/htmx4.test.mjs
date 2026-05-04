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

test('nonce helper sanitizes and rewrites response nonce', () => {
  bootDom();
  let ext;
  const htmx = { config:{documentNonce:'docnonce'}, registerExtension:(_n,e)=>ext=e };
  load('src/js/rizzy-nonce.js', htmx);
  const headers = new Headers([['HX-Nonce','resnonce']]);
  const dirty = '<html><body><script nonce="resnonce"></script><script nonce="bad"></script><style nonce="resnonce"></style><link nonce="bad"></body></html>';
  const clean = window.Rizzy.nonce.processResponseHtml(dirty,{headers});
  assert.match(clean,/nonce="docnonce"/);
  assert.ok(!clean.includes('nonce="bad"'));
  const d={ctx:{text:dirty,response:{headers}}}; ext.htmx_after_request(null,d); assert.ok(d.ctx.text.includes('docnonce'));
});

test('streaming uses nonce sanitizer before sink insertion', () => {
  const dom=bootDom();
  const events=[];
  let streaming;
  const htmx = { config:{documentNonce:'docnonce'}, registerExtension:(n,e)=>{ if(n==='rizzy-streaming') streaming=e; }, trigger:(elt,name)=>events.push(name), swap: async()=>{}, process:()=>{}, find:()=>null, ajax:()=>{} };
  load('src/js/rizzy-nonce.js', htmx);
  load('src/js/rizzy-streaming.js', htmx);
  const src=document.createElement('button');
  const ctx={sourceElement:src,target:document.getElementById('target'),response:{raw:{body:{getReader(){ return {read: async()=>({done:true}), cancel(){} };}},headers:new Headers([['Content-Type','text/html'],['HX-Nonce','resnonce']]),status:200}},hx:{},request:{action:'/x'}};
  streaming.htmx_before_response(src,{ctx});
  const sanitized=window.Rizzy.nonce.processResponseHtml('<blazor-ssr><script nonce="bad"></script><blazor-ssr-end></blazor-ssr-end></blazor-ssr>',ctx.response.raw);
  assert.ok(!sanitized.includes('nonce="bad"'));
});
