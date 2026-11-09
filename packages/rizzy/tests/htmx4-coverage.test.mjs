// Drift tests: Rizzy's typed htmx surface must cover everything the installed htmx.org (core and
// official extensions) reads. They fail when an htmx upgrade adds an attribute, event or extension,
// or when the generated SRI table is stale, so "full coverage" stays true.
import test from 'node:test';
import assert from 'node:assert/strict';
import fs from 'node:fs';
import path from 'node:path';
import { fileURLToPath } from 'node:url';
import { createRequire } from 'node:module';
import { computeAssets, render } from '../scripts/generate-htmx-assets.mjs';

const here = path.dirname(fileURLToPath(import.meta.url));
const packageRoot = path.resolve(here, '..');
const repoRoot = path.resolve(packageRoot, '../..');
const require = createRequire(path.join(packageRoot, 'package.json'));
const dist = path.dirname(require.resolve('htmx.org/dist/htmx.js'));

const read = (p) => fs.readFileSync(p, 'utf8');
const core = read(path.join(dist, 'htmx.js'));
const extFiles = fs.readdirSync(path.join(dist, 'ext'))
    .filter((f) => f.endsWith('.js') && !f.endsWith('.min.js'))
    .map((f) => ({ file: f.replace(/\.js$/, ''), source: read(path.join(dist, 'ext', f)) }));

const csharp = (rel) => read(path.join(repoRoot, rel));
const hxAttributes = csharp('src/Rizzy.Htmx/HxAttributes.cs') + csharp('src/Rizzy.Htmx/HtmxStatus.cs');
const htmxEvents = csharp('src/Rizzy.Htmx/HtmxEvents.cs');
const htmxExtension = csharp('src/Rizzy.Htmx/HtmxExtension.cs');

const unique = (items) => [...new Set(items)].sort();

/** hx-* attribute names a source reads: quoted names, [hx-*] selectors and the hx-<verb> attributes. */
function attributeNames(source) {
    const names = [...source.matchAll(/['"`](hx-[a-z-]+(?::[a-z-]+)?)['"`]/g)].map((m) => m[1]);
    names.push(...[...source.matchAll(/\[(hx-[a-z-]+)/g)].map((m) => m[1]));
    const verbs = source.match(/#verbs\s*=\s*\[([^\]]+)\]/);
    if (verbs) names.push(...[...verbs[1].matchAll(/"([a-z]+)"/g)].map((m) => `hx-${m[1]}`));
    return unique(names);
}

// Not attributes: a header name an extension compares in lowercase, and the hx-csp extension's
// name, which appears as "[hx-csp]" in its log messages.
const NOT_ATTRIBUTES = new Set(['hx-last-part-id', 'hx-csp']);

test('every htmx core attribute has an HxAttributes method', () => {
    const names = attributeNames(core);
    assert.ok(names.length >= 30, `found ${names.length} core attributes`);
    const missing = names.filter((n) => !hxAttributes.includes(`"${n}"`) && !hxAttributes.includes(`"${n}:`));
    assert.deepEqual(missing, []);
});

test('every official extension attribute has an HxAttributes method', () => {
    const names = unique(extFiles.flatMap(({ source }) => attributeNames(source)))
        .filter((n) => !NOT_ATTRIBUTES.has(n));
    const missing = names.filter((n) => !hxAttributes.includes(`"${n}"`) && !hxAttributes.includes(`"${n}:`));
    assert.deepEqual(missing, []);
});

/** htmx:* event names a source dispatches or listens to. */
const eventNames = (source) => unique([...source.matchAll(/['"`](htmx:[a-zA-Z:]+[a-zA-Z])['"`]/g)].map((m) => m[1]));

test('every htmx core event has an HtmxEvents constant', () => {
    const names = eventNames(core);
    assert.ok(names.length >= 30, `found ${names.length} core events`);
    assert.ok(core.includes("'htmx:process:'"), 'dynamic htmx:process:<type> events still exist');
    assert.ok(htmxEvents.includes('"htmx:process:"'), 'HtmxEvents.ProcessPrefix covers htmx:process:<type>');
    const missing = names.filter((n) => !htmxEvents.includes(`"${n}"`));
    assert.deepEqual(missing, []);
});

test('every official extension event has an HtmxEvents constant', () => {
    // htmx-2-compat re-dispatches events under their htmx 2 names, which are not htmx 4 events.
    const names = unique(extFiles.filter(({ file }) => file !== 'htmx-2-compat')
        .flatMap(({ source }) => eventNames(source)));
    const missing = names.filter((n) => !htmxEvents.includes(`"${n}"`));
    assert.deepEqual(missing, []);
});

test('every official extension is in the HtmxExtension catalog with its registered name', () => {
    assert.equal(extFiles.length, 17, 'htmx 4.0.1 ships 17 extensions');
    for (const { file, source } of extFiles) {
        const name = source.match(/registerExtension\(\s*['"]([a-z0-9-]+)['"]/)?.[1];
        assert.ok(name, `${file} registers an extension`);
        assert.ok(htmxExtension.includes(`new("${name}", "${file}"`), `HtmxExtension has new("${name}", "${file}", ...)`);
    }
});

test('HtmxAssets.g.cs matches the installed htmx.org (run npm run generate:htmx-assets)', () => {
    const expected = render(computeAssets());
    const actual = csharp('src/Rizzy.Htmx/HtmxAssets.g.cs').replace(/\r\n/g, '\n');
    assert.equal(actual, expected);
});

test('the htmx peer dependency range includes the tested version', () => {
    const pkg = JSON.parse(read(path.join(packageRoot, 'package.json')));
    const version = JSON.parse(read(require.resolve('htmx.org/package.json'))).version;
    assert.equal(pkg.peerDependencies['htmx.org'], `^${version}`, 'peer dependency matches the tested htmx version');
});
