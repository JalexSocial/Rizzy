// Test support: boots a jsdom window running the REAL htmx runtime from node_modules plus the
// built Rizzy bundle (dist/js/rizzy.js), with window.fetch answered by a test responder.
import fs from 'node:fs';
import path from 'node:path';
import { fileURLToPath } from 'node:url';
import { createRequire } from 'node:module';
import { JSDOM } from 'jsdom';

const here = path.dirname(fileURLToPath(import.meta.url));
const packageRoot = path.resolve(here, '../..');
const require = createRequire(path.join(packageRoot, 'package.json'));

const htmxSource = fs.readFileSync(require.resolve('htmx.org/dist/htmx.js'), 'utf8');
const rizzySource = fs.readFileSync(path.join(packageRoot, 'dist/js/rizzy.js'), 'utf8');

export const htmxVersion = JSON.parse(
    fs.readFileSync(require.resolve('htmx.org/package.json'), 'utf8')).version;

/**
 * @param {object} options
 * @param {string} options.body           Initial <body> markup.
 * @param {object} [options.config]       Object serialized into <meta name="htmx-config">.
 * @param {(request: {url: string, method: string, headers: Record<string,string>, body: any}) =>
 *         {status?: number, headers?: Record<string,string>, body?: string}} options.respond
 * @param {boolean} [options.rizzy=true]  Load the Rizzy bundle after htmx.
 * @param {string} [options.head]         Extra <head> markup (e.g. a nonced script).
 * @param {string[]} [options.extensions] Official extension files (e.g. 'hx-ptag') loaded after Rizzy,
 *                                        in the order <RzScripts /> uses.
 */
export async function bootHtmx({ body, config, respond, rizzy = true, head = '', extensions = [] }) {
    const meta = config
        ? `<meta name="htmx-config" content='${JSON.stringify(config).replace(/'/g, '&#39;')}'>`
        : '';
    const dom = new JSDOM(`<!doctype html><html><head>${meta}${head}</head><body>${body}</body></html>`, {
        url: 'http://localhost/',
        runScripts: 'outside-only',
        pretendToBeVisual: true,
    });
    const { window } = dom;

    // APIs jsdom does not provide (or provides only in Node's realm).
    window.CSS ??= {};
    window.CSS.escape ??= (s) => String(s).replace(/[^a-zA-Z0-9_-]/g, (c) => '\\' + c);
    window.TextDecoder ??= TextDecoder;
    window.TextEncoder ??= TextEncoder;
    window.Response ??= Response;
    window.Headers = Headers;

    // htmx calls XPathExpression.evaluate(root) without a result type, which browsers allow
    // (it defaults to ANY_TYPE) but jsdom rejects.
    const xpathExpression = new window.XPathEvaluator().createExpression('.', null);
    const xpathProto = Object.getPrototypeOf(xpathExpression);
    const originalEvaluate = xpathProto.evaluate;
    xpathProto.evaluate = function (node, type = 0, result = null) {
        return originalEvaluate.call(this, node, type, result);
    };

    const requests = [];
    window.fetch = async (url, init = {}) => {
        const headers = {};
        new Headers(init.headers ?? {}).forEach((value, key) => { headers[key.toLowerCase()] = value; });
        const request = { url: String(url), method: (init.method ?? 'GET').toUpperCase(), headers, body: init.body };
        requests.push(request);
        const result = respond(request) ?? {};
        return new Response(result.body ?? '', {
            status: result.status ?? 200,
            headers: { 'Content-Type': 'text/html; charset=utf-8', ...(result.headers ?? {}) },
        });
    };

    const errors = [];
    window.addEventListener('error', (e) => errors.push(e.error ?? e.message));
    window.console.error = (...args) => errors.push(args.map(String).join(' '));

    window.eval(htmxSource);
    if (rizzy) window.eval(rizzySource);
    for (const ext of extensions) {
        window.eval(fs.readFileSync(require.resolve(`htmx.org/dist/ext/${ext}.js`), 'utf8'));
    }

    // htmx initializes on the next tick once the document has loaded.
    await waitFor(() => window.document.readyState !== 'loading', 1000);
    await delay(20);

    // Let htmx finish in-flight work (swaps, settle, finally handlers) before tearing down.
    const close = async () => {
        await delay(100);
        window.close();
    };

    return { window, document: window.document, htmx: window.htmx, requests, errors, close };
}

/** A response body the test writes to chunk by chunk. */
export function controlledStream() {
    const encoder = new TextEncoder();
    let controller;
    const body = new ReadableStream({ start(c) { controller = c; } });
    return {
        body,
        push: (text) => controller.enqueue(encoder.encode(text)),
        end: () => controller.close(),
        fail: (error) => controller.error(error),
    };
}

export const delay = (ms) => new Promise((resolve) => setTimeout(resolve, ms));

export async function waitFor(predicate, timeout = 1500) {
    const start = Date.now();
    while (Date.now() - start < timeout) {
        if (predicate()) return true;
        await delay(10);
    }
    return predicate();
}
