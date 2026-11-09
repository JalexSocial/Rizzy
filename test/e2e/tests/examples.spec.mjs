// The demo's Examples page: table rows, confirm + antiforgery, CSP nonces, server errors.
import { test, expect } from './fixtures.mjs';

test.beforeEach(async ({ page }) => {
    await page.goto('/home/examples');
});

test('table rows append and the button replaces itself out of band', async ({ page, issues }) => {
    const rows = page.locator('#rows tr');
    await expect(rows).toHaveCount(5);

    await page.locator('#load-more').click();
    await expect(rows).toHaveCount(10);
    await expect(page.locator('#load-more')).toHaveText('Load page 3');

    await page.locator('#load-more').click();
    await expect(rows).toHaveCount(15);
    await expect(rows.last()).toContainText('Item 15');
    await expect(page.locator('#load-more')).toHaveCount(1);
});

test('hx-confirm uses the native dialog; dismissing it sends nothing', async ({ page, issues }) => {
    const result = page.locator('#confirm-result');
    let deletes = 0;
    page.on('request', (r) => { if (r.method() === 'DELETE') deletes++; });

    page.once('dialog', (dialog) => {
        expect(dialog.type()).toBe('confirm');
        expect(dialog.message()).toBe('Delete item 42?');
        return dialog.dismiss();
    });
    await page.locator('button[hx-delete]').click();
    await page.waitForTimeout(300);
    expect(deletes).toBe(0);
    await expect(result).toHaveText('Item 42 exists.');

    page.once('dialog', (dialog) => dialog.accept());
    const response = page.waitForResponse((r) => r.request().method() === 'DELETE');
    await page.locator('button[hx-delete]').click();
    const del = await response;
    expect(del.status(), '[ValidateAntiForgeryToken] accepted the DELETE').toBe(200);
    expect(del.request().headers()['requestverificationtoken']).toBeTruthy();
    await expect(result).toHaveText('Item 42 was deleted. The antiforgery token was checked.');
});

test('CSP: server-rendered scripts run, injected scripts are blocked', async ({ page, issues }) => {
    issues.expectIssue(/Content Security Policy/);   // the deliberately injected script

    await page.locator('button[hx-get="/home/examples/scripts"]').click();

    await expect(page.locator('#csp-server strong')).toHaveText('ran');
    await expect(page.locator('#csp-injected strong')).toHaveText('blocked');

    // The violation event is dispatched asynchronously after the blocked script.
    await expect.poll(() => issues.cspViolations()).toHaveLength(1);
    expect(issues.cspViolations()[0]).toMatch(/inline script/i);
});

test('a server error is not swapped into the target', async ({ page, issues }) => {
    const result = page.locator('#error-result');

    const response = page.waitForResponse((r) => r.url().endsWith('/home/examples/error'));
    await page.locator('button[hx-get="/home/examples/error"]').click();
    const error = await response;
    expect(error.status()).toBe(500);
    expect(error.headers()['hx-reswap']).toBe('none');

    await expect(result).toHaveText('The server answered 500; the error page was not swapped in.');
    await expect(page.locator('article h3').first()).toHaveText('Examples');
});
