// Integration tests: the built Rizzy bundle running against the REAL htmx 4 runtime in jsdom.
//
// Tests marked `todo` document known defects from the 2026-11-08 audit
// (docs/audits/2026-11-08-rizzy-initial-audit). They run on every build but do not fail it.
// When a fix lands, the todo test starts passing: remove the `todo` option to lock it in.
import test from 'node:test';
import assert from 'node:assert/strict';
import { bootHtmx, waitFor, delay, htmxVersion } from './support/htmx-dom.mjs';

const rizzyConfig = {
    documentNonce: 'DOCNONCE',
    antiforgery: {
        headerName: 'RequestVerificationToken',
        requestToken: 'test-token',
        formFieldName: '__RequestVerificationToken',
    },
};

test(`runs against htmx ${htmxVersion}`, async () => {
    const env = await bootHtmx({ body: '', config: rizzyConfig, respond: () => ({}) });
    assert.match(env.htmx.version, /^4\./);
    await env.close();
});

test('basic hx-get swap works with Rizzy loaded', async () => {
    const env = await bootHtmx({
        config: rizzyConfig,
        body: '<button id="go" hx-get="/hello" hx-target="#target">go</button><div id="target"></div>',
        respond: () => ({ body: '<span id="hello">hello</span>' }),
    });

    env.document.getElementById('go').click();

    assert.ok(await waitFor(() => env.document.getElementById('hello')), 'response should be swapped in');
    assert.deepEqual(env.errors, []);
    await env.close();
});

test('antiforgery token header is added to unsafe methods only', async () => {
    const env = await bootHtmx({
        config: rizzyConfig,
        body: `
            <button id="get" hx-get="/a" hx-swap="none">get</button>
            <button id="post" hx-post="/b" hx-swap="none">post</button>
            <button id="delete" hx-delete="/c" hx-swap="none">delete</button>`,
        respond: () => ({ status: 204 }),
    });

    for (const id of ['get', 'post', 'delete']) {
        env.document.getElementById(id).click();
        await waitFor(() => env.requests.some((r) => r.url.endsWith({ get: '/a', post: '/b', delete: '/c' }[id])));
    }

    const token = (suffix) => env.requests.find((r) => r.url.endsWith(suffix))?.headers['requestverificationtoken'];
    assert.equal(token('/a'), undefined);
    assert.equal(token('/b'), 'test-token');
    assert.equal(token('/c'), 'test-token');
    await env.close();
});

test('table row fragments survive the swap', { todo: 'RIZZY-004: rizzy-nonce DOMParser round-trip drops <tr>/<td>' }, async () => {
    const env = await bootHtmx({
        config: rizzyConfig,
        body: `<table><tbody id="rows"></tbody></table>
               <button id="add" hx-get="/row" hx-target="#rows" hx-swap="beforeend">add</button>`,
        respond: () => ({ body: '<tr id="row1"><td>Cell</td></tr>' }),
    });

    env.document.getElementById('add').click();
    await waitFor(() => env.document.getElementById('rows').textContent.includes('Cell'));

    assert.ok(env.document.getElementById('row1'), `expected <tr id="row1">, got: ${env.document.getElementById('rows').innerHTML}`);
    await env.close();
});

test('un-nonced <style> survives when no CSP nonce is configured', { todo: 'RIZZY-004: rizzy-nonce strips all un-nonced style/link/script' }, async () => {
    const env = await bootHtmx({
        config: {},
        body: '<button id="go" hx-get="/styled" hx-target="#target">go</button><div id="target"></div>',
        respond: () => ({ body: '<div id="styled">x</div><style>.styled { color: red; }</style>' }),
    });

    env.document.getElementById('go').click();
    await waitFor(() => env.document.getElementById('styled'));

    assert.ok(env.document.querySelector('#target style'), `style was removed: ${env.document.getElementById('target').innerHTML}`);
    await env.close();
});

test('HX-Trigger fires exactly once, after the swap', { todo: 'RIZZY-003: rizzy-streaming fires HX-Trigger before the swap and core fires it again' }, async () => {
    const env = await bootHtmx({
        config: rizzyConfig,
        body: '<button id="go" hx-get="/t" hx-target="#target">go</button><div id="target"></div>',
        respond: () => ({ body: '<span id="swapped">s</span>', headers: { 'HX-Trigger': 'saved' } }),
    });

    const seen = [];
    env.document.body.addEventListener('saved', () => seen.push(Boolean(env.document.getElementById('swapped'))));

    env.document.getElementById('go').click();
    await waitFor(() => env.document.getElementById('swapped'));
    await delay(50);

    assert.deepEqual(seen, [true], `HX-Trigger handler calls (true = content already swapped): ${JSON.stringify(seen)}`);
    await env.close();
});

test('hx-status:422 retargets the response', { todo: 'RIZZY-003: rizzy-streaming bypasses htmx core status handling (hx-status)' }, async () => {
    const env = await bootHtmx({
        config: rizzyConfig,
        body: `<button id="save" hx-post="/save" hx-target="#result" hx-status:422="target:#errors">save</button>
               <div id="result"></div><div id="errors"></div>`,
        respond: () => ({ status: 422, body: '<p id="invalid">Name is required</p>' }),
    });

    env.document.getElementById('save').click();
    await waitFor(() => env.document.getElementById('invalid'));

    assert.ok(env.document.querySelector('#errors #invalid'), `422 content landed in: ${env.document.getElementById('invalid')?.parentElement?.id}`);
    await env.close();
});

test('htmx:response:error fires for 4xx responses', { todo: 'RIZZY-003: rizzy-streaming skips htmx:after:request and htmx:response:error' }, async () => {
    const env = await bootHtmx({
        config: rizzyConfig,
        body: '<button id="go" hx-get="/missing" hx-target="#target">go</button><div id="target"></div>',
        respond: () => ({ status: 404, body: '<p>not found</p>' }),
    });

    let errorEvents = 0;
    env.document.body.addEventListener('htmx:response:error', () => errorEvents++);

    env.document.getElementById('go').click();
    await waitFor(() => env.requests.length > 0);
    await delay(100);

    assert.equal(errorEvents, 1);
    await env.close();
});

test('hx-confirm falls back to window.confirm without a custom dialog', { todo: 'RIZZY-005: rizzy-confirm waits forever for an rz:confirm-dialog listener' }, async () => {
    const env = await bootHtmx({
        config: rizzyConfig,
        body: '<button id="del" hx-post="/delete" hx-confirm="Sure?" hx-swap="none">delete</button>',
        respond: () => ({ status: 204 }),
    });

    let confirmCalls = 0;
    env.window.confirm = () => { confirmCalls++; return true; };

    env.document.getElementById('del').click();
    await waitFor(() => env.requests.length > 0, 500);

    assert.equal(confirmCalls, 1, 'window.confirm should be shown');
    assert.equal(env.requests.length, 1, 'request should be issued after confirming');
    await env.close();
});
