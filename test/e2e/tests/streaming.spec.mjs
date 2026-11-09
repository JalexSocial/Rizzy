// Blazor streaming rendering over htmx (RIZZY-003). The Weather page waits 500 ms for the forecast
// and 3 s for the mood; both arrive as <blazor-ssr> blocks after the initial HTML is swapped.
import { test, expect, navigate } from './fixtures.mjs';

const loading = (page) => page.locator('article em', { hasText: 'Loading...' });
const rows = (page) => page.locator('article table tbody tr');
const mood = (page) => page.locator('article .alert-info');

test('boosted navigation shows the placeholder first, then streams the updates', async ({ page, issues }) => {
    await page.goto('/home/');
    await navigate(page, 'Weather');

    await expect(loading(page)).toBeVisible();
    await expect(mood(page)).toHaveText('Fetching current mood...');

    await expect(rows(page)).toHaveCount(5);
    await expect(loading(page)).toHaveCount(0);
    await expect(mood(page)).toHaveText('My mood is happy', { timeout: 10_000 });

    await expect(page.locator('[data-rizzy-stream-sink]')).toHaveCount(0);
    await expect(page.locator('article blazor-ssr, article template[blazor-component-id]')).toHaveCount(0);
});

test('a direct page load streams too', async ({ page, issues }) => {
    await page.goto('/home/weather');

    await expect(rows(page)).toHaveCount(5);
    await expect(mood(page)).toHaveText('My mood is happy', { timeout: 10_000 });
    await expect(page.locator('blazor-ssr')).toHaveCount(0);
});

test('streaming fires htmx:rizzy:stream events and closes the stream', async ({ page, issues }) => {
    await page.goto('/home/');
    await page.evaluate(() => {
        window.__streamEvents = [];
        for (const name of ['htmx:rizzy:stream:open', 'htmx:rizzy:stream:block', 'htmx:rizzy:stream:close'])
            document.addEventListener(name, () => window.__streamEvents.push(name.split(':').pop()));
    });

    await navigate(page, 'Weather');
    await expect(mood(page)).toHaveText('My mood is happy', { timeout: 10_000 });

    await expect.poll(() => page.evaluate(() => window.__streamEvents)).toContain('close');
    const events = await page.evaluate(() => window.__streamEvents);
    expect(events[0]).toBe('open');
    expect(events.filter((e) => e === 'block').length).toBeGreaterThanOrEqual(2);
});
