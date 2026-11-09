// Client diagnostics (RM-D3): one clear console warning on a misconfigured page, and errors
// reported through htmx's detail.error logging convention.
import test from 'node:test';
import assert from 'node:assert/strict';
import fs from 'node:fs';
import path from 'node:path';
import { fileURLToPath } from 'node:url';
import { JSDOM } from 'jsdom';

const here = path.dirname(fileURLToPath(import.meta.url));
const source = fs.readFileSync(path.resolve(here, '../src/js/rizzy-diagnostics.js'), 'utf8');

/** Boots a page, loads rizzy-diagnostics.js and returns the captured console output. */
async function boot(head = '') {
    const dom = new JSDOM(`<!doctype html><html><head>${head}</head><body><div id="t"></div></body></html>`,
        { url: 'http://localhost/' });
    const logs = { warn: [], error: [] };
    const events = [];
    global.window = dom.window;
    global.document = dom.window.document;
    global.htmx = {
        trigger(elt, name, detail) { events.push({ elt, name, detail }); return true; },
    };
    const original = { warn: console.warn, error: console.error };
    console.warn = (...args) => logs.warn.push(args);
    console.error = (...args) => logs.error.push(args);
    try {
        (0, eval)(source);
        // The page check runs at DOMContentLoaded (as it does for a script at the end of <body>).
        if (dom.window.document.readyState === 'loading') {
            await new Promise((resolve) => dom.window.document.addEventListener('DOMContentLoaded', resolve, { once: true }));
        }
    } finally {
        console.warn = original.warn;
        console.error = original.error;
    }
    return { window: dom.window, logs, events, original };
}

const meta = (config) => `<meta name="htmx-config" content='${JSON.stringify(config)}'>`;

test('a page without the htmx-config meta tag logs exactly one warning', async () => {
    const { logs, events } = await boot();
    assert.equal(logs.warn.length, 1);
    assert.match(logs.warn[0][0], /^htmx: htmx:rizzy:config:warn: Rizzy: no <meta name="htmx-config">/);
    assert.match(logs.warn[0][0], /Render <RzHeadOutlet \/> instead of <HeadOutlet \/>/);
    assert.equal(events.length, 1);
    assert.equal(events[0].name, 'htmx:rizzy:config:warn');
    assert.equal(logs.error.length, 0);
});

test('a correctly configured page logs nothing', async () => {
    const { logs, events } = await boot(meta({ rizzyVersion: '6.0.0-dev', antiforgery: {} }));
    assert.deepEqual(logs, { warn: [], error: [] });
    assert.equal(events.length, 0);
});

test('a server on another Rizzy major version logs one warning', async () => {
    const { logs } = await boot(meta({ rizzyVersion: '5.4.0' }));
    assert.equal(logs.warn.length, 1);
    assert.match(logs.warn[0][0], /server runs Rizzy 5\.4\.0 but rizzy\.js is for Rizzy 6\.x/);
});

test('invalid meta JSON logs one warning', async () => {
    const { logs } = await boot(`<meta name="htmx-config" content="{not json">`);
    assert.equal(logs.warn.length, 1);
    assert.match(logs.warn[0][0], /not contain valid JSON/);
});

test('rizzyDiagnostics: false silences the page check', async () => {
    const { logs } = await boot(meta({ rizzyVersion: '5.0.0', rizzyDiagnostics: false }));
    assert.equal(logs.warn.length, 0);
});

test('report() logs detail.error at error level and dispatches the event', async () => {
    const { window, events, original } = await boot(meta({ rizzyVersion: '6.0.0' }));
    const errors = [];
    console.error = (...args) => errors.push(args);
    try {
        const elt = window.document.getElementById('t');
        const error = new Error('stream broke');
        window.Rizzy.diagnostics.report(elt, 'htmx:error', { error });
        assert.equal(errors.length, 1);
        assert.equal(errors[0][0], 'htmx: htmx:error: stream broke');
        assert.equal(errors[0][1], error);
        assert.equal(events.at(-1).name, 'htmx:error');
        assert.equal(events.at(-1).elt, elt);
    } finally {
        console.error = original.error;
    }
});

test('CLIENT_MAJOR matches the major version in Directory.Build.props', async () => {
    const props = fs.readFileSync(path.resolve(here, '../../../Directory.Build.props'), 'utf8');
    const major = Number(/<Version>(\d+)\./.exec(props)[1]);
    const { window } = await boot(meta({}));
    assert.equal(window.Rizzy.diagnostics.clientMajor, major);
});
