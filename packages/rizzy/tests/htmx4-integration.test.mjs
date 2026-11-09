// Integration tests: the built Rizzy bundle running against the REAL htmx 4 runtime in jsdom.
//
// Defects from the 2026-11-08 audit (docs/audits/2026-11-08-rizzy-initial-audit) are locked in as
// regular tests, named after their finding IDs. Mark a test `todo` only for a newly found,
// not-yet-fixed defect.
import test from 'node:test';
import assert from 'node:assert/strict';
import { bootHtmx, waitFor, delay, htmxVersion, controlledStream } from './support/htmx-dom.mjs';

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

test('table row fragments survive the swap', async () => {
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

test('un-nonced <style> survives when no CSP nonce is configured', async () => {
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

test('HX-Trigger fires exactly once, after the swap', async () => {
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

test('hx-status:422 retargets the response', async () => {
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

test('htmx:response:error fires for 4xx responses', async () => {
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

test('hx-confirm falls back to window.confirm without a custom dialog (RIZZY-005)', async () => {
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

test('hx-confirm cancelled in the native dialog drops the request', async () => {
    const env = await bootHtmx({
        config: rizzyConfig,
        body: '<button id="del" hx-post="/delete" hx-confirm="Sure?" hx-swap="none">delete</button>',
        respond: () => ({ status: 204 }),
    });
    env.window.confirm = () => false;

    env.document.getElementById('del').click();
    await delay(100);

    assert.equal(env.requests.length, 0);
    await env.close();
});

test('hx-confirm uses a custom dialog that claims rz:confirm-dialog', async () => {
    const env = await bootHtmx({
        config: rizzyConfig,
        body: `<button id="del" hx-post="/delete" hx-swap="none"
                 hx-confirm='{"title":"Delete?","text":"Really?","showCancelButton":true}'>delete</button>`,
        respond: () => ({ status: 204 }),
    });

    let nativeCalls = 0;
    env.window.confirm = () => { nativeCalls++; return true; };
    let dialog;
    env.window.addEventListener('rz:confirm-dialog', (e) => { e.preventDefault(); dialog = e.detail; });

    env.document.getElementById('del').click();
    assert.ok(await waitFor(() => dialog), 'dialog event should be dispatched');
    assert.equal(dialog.title, 'Delete?');
    assert.equal(dialog.showCancelButton, true);
    assert.equal(env.requests.length, 0, 'request waits for the dialog');

    dialog.confirm();
    assert.ok(await waitFor(() => env.requests.length === 1), 'request issued after confirm()');
    assert.equal(nativeCalls, 0, 'native confirm is not shown');
    await env.close();
});

test('hx-confirm custom dialog deny() drops the request', async () => {
    const env = await bootHtmx({
        config: rizzyConfig,
        body: '<button id="del" hx-post="/delete" hx-confirm="Sure?" hx-swap="none">delete</button>',
        respond: () => ({ status: 204 }),
    });
    env.window.addEventListener('rz:confirm-dialog', (e) => { e.detail.handled = true; setTimeout(() => e.detail.deny(), 10); });

    env.document.getElementById('del').click();
    await delay(100);

    assert.equal(env.requests.length, 0);
    await env.close();
});

test('JSON hx-confirm without a dialog shows a readable native message', async () => {
    const env = await bootHtmx({
        config: rizzyConfig,
        body: `<button id="del" hx-post="/delete" hx-swap="none" hx-confirm='{"title":"Delete?","text":"This cannot be undone."}'>delete</button>`,
        respond: () => ({ status: 204 }),
    });
    let message;
    env.window.confirm = (m) => { message = m; return true; };

    env.document.getElementById('del').click();
    assert.ok(await waitFor(() => env.requests.length === 1));
    assert.equal(message, 'Delete?\n\nThis cannot be undone.');
    await env.close();
});

// ---- RIZZY-003: streaming cooperates with htmx core --------------------------------------------

const streamingInitial =
    '<html><body><div id="page"><!--bl:7--><p id="loading">loading</p><!--/bl:7--></div></body></html>';
const streamingBlock =
    '<blazor-ssr><template blazor-component-id="7"><p id="loaded">loaded</p></template>' +
    '<blazor-ssr-end></blazor-ssr-end></blazor-ssr>';

test('streamed <blazor-ssr> blocks patch the swapped content; HX-Trigger fires once at the end', async () => {
    const stream = controlledStream();
    const env = await bootHtmx({
        config: rizzyConfig,
        body: '<button id="go" hx-get="/stream" hx-target="#target">go</button><div id="target"></div>',
        respond: () => ({ body: stream.body, headers: { 'HX-Trigger': 'streamDone' } }),
    });

    const triggers = [];
    env.document.body.addEventListener('streamDone', () => triggers.push(env.document.getElementById('loaded') ? 'after-block' : 'before-block'));
    const button = env.document.getElementById('go');

    button.click();
    stream.push(streamingInitial);

    assert.ok(await waitFor(() => env.document.getElementById('loading')), 'initial HTML is swapped before the stream ends');
    assert.ok(button.classList.contains('htmx-request'), 'request indicator stays on while streaming');
    assert.deepEqual(triggers, [], 'HX-Trigger waits for the stream');

    stream.push(streamingBlock);
    assert.ok(await waitFor(() => env.document.getElementById('loaded')), 'streamed block patches the component');
    assert.equal(env.document.getElementById('loading'), null);

    stream.end();
    assert.ok(await waitFor(() => triggers.length > 0));
    await delay(30);
    assert.deepEqual(triggers, ['after-block']);
    assert.ok(!button.classList.contains('htmx-request'), 'indicator removed when the stream ends');
    assert.equal(env.document.querySelector('[data-rizzy-stream-sink]'), null, 'sink is cleaned up');
    assert.deepEqual(env.errors, []);
    await env.close();
});

test('a complete response passes through htmx core unchanged', async () => {
    const env = await bootHtmx({
        config: rizzyConfig,
        body: '<button id="go" hx-get="/doc" hx-target="#target">go</button><div id="target"></div>',
        respond: () => ({ body: '<html><body><p id="whole">whole</p></body></html>' }),
    });

    let afterRequest = 0;
    env.document.body.addEventListener('htmx:after:request', () => afterRequest++);

    env.document.getElementById('go').click();
    assert.ok(await waitFor(() => env.document.getElementById('whole')));
    await delay(30);
    assert.equal(afterRequest, 1);
    await env.close();
});

test('streamed blocks are dropped when the response is not swapped', async () => {
    const stream = controlledStream();
    const env = await bootHtmx({
        config: { ...rizzyConfig, noSwap: [204, 304, '5xx'] },
        body: '<div id="page"><!--bl:7--><p id="loading">loading</p><!--/bl:7--></div>' +
              '<button id="go" hx-get="/stream" hx-target="#target">go</button><div id="target"></div>',
        respond: () => ({ status: 500, body: stream.body }),
    });

    env.document.getElementById('go').click();
    stream.push('<html><body>error</body></html>');
    stream.push(streamingBlock);
    stream.end();
    await delay(150);

    assert.ok(env.document.getElementById('loading'), 'existing page content is untouched');
    assert.equal(env.document.getElementById('loaded'), null);
    await env.close();
});

// ---- RIZZY-004: CSP nonces ---------------------------------------------------------------------

test('scripts carrying the response nonce are promoted to the page nonce', async () => {
    const env = await bootHtmx({
        config: { antiforgery: rizzyConfig.antiforgery },
        head: '<script nonce="PAGENONCE"></script>',
        body: '<button id="go" hx-get="/s" hx-target="#target">go</button><div id="target"></div>',
        respond: () => ({
            headers: { 'HX-Nonce': 'RESPNONCE' },
            body: '<div id="ok"></div><script id="trusted" nonce="RESPNONCE">1</script><script id="injected">2</script>',
        }),
    });

    env.document.getElementById('go').click();
    assert.ok(await waitFor(() => env.document.getElementById('trusted')));

    assert.equal(env.document.getElementById('trusted').getAttribute('nonce'), 'PAGENONCE');
    assert.equal(env.document.getElementById('injected').hasAttribute('nonce'), false);
    await env.close();
});
