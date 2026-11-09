// Layouts, boosted navigation and htmx 4 history restore (RIZZY-006, RIZZY-036).
import { test, expect, navigate, markSidebar, sidebarIsOriginal } from './fixtures.mjs';

test('a full page carries the htmx config before the htmx script', async ({ page, issues }) => {
    await page.goto('/home/');

    const order = await page.evaluate(() => {
        const head = [...document.head.children];
        return {
            config: head.findIndex((e) => e.matches('meta[name="htmx-config"]')),
            htmx: head.findIndex((e) => e.matches('script[src*="htmx"]')),
        };
    });
    expect(order.config).toBeGreaterThan(-1);
    expect(order.config).toBeLessThan(order.htmx);

    // The home page has its own <HeadContent>; it must not displace the config (RIZZY-036).
    await expect(page.locator('meta[name="description"]')).toHaveCount(1);
    expect(await page.evaluate(() => Boolean(htmx.config.antiforgery?.requestToken))).toBe(true);
});

test('boosted navigation swaps only the article and pushes the URL', async ({ page, issues }) => {
    await page.goto('/home/');
    await markSidebar(page);

    await navigate(page, 'Counter');

    await expect(page).toHaveURL(/\/home\/counter$/);
    await expect(page).toHaveTitle('Counter');
    await expect(page.locator('article h1')).toHaveText('Counter');
    await expect(page.locator('article .sidebar, article nav')).toHaveCount(0);
    expect(await sidebarIsOriginal(page)).toBe(true);
});

test('back and forward restore the article without replacing the page', async ({ page, issues }) => {
    await page.goto('/home/counter');
    await markSidebar(page);

    await navigate(page, 'Examples');
    await expect(page.locator('article h3').first()).toHaveText('Examples');

    await page.goBack();
    await expect(page).toHaveURL(/\/home\/counter$/);
    await expect(page.locator('article h1')).toHaveText('Counter');
    await expect(page.locator('article')).toHaveCount(1);
    expect(await sidebarIsOriginal(page), 'hx-history-elt restores only the article').toBe(true);

    await page.goForward();
    await expect(page).toHaveURL(/\/home\/examples$/);
    await expect(page.locator('article h3').first()).toHaveText('Examples');
});

test('reloading a pushed URL renders the full page', async ({ page, issues }) => {
    await page.goto('/home/');
    await navigate(page, 'Validation');
    await expect(page).toHaveURL(/\/home\/information$/);

    await page.reload();
    await expect(page.locator('#sidebar nav')).toBeVisible();
    await expect(page.locator('article #information')).toBeVisible();
});

test('the counter posts with the antiforgery token', async ({ page, issues }) => {
    await page.goto('/home/counter');
    const status = page.locator('.counter [role="status"]');
    const before = Number((await status.textContent()).match(/\d+/)[0]);

    const response = page.waitForResponse((r) => r.url().endsWith('/home/count'));
    await page.locator('.counter button').click();
    expect((await response).status(), '[ValidateAntiForgeryToken] accepted the request').toBe(200);

    await expect(status).toHaveText(`Current count: ${before + 1}`);
});
