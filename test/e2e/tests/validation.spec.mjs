// 422 validation with MVC model binding and RzModelStateProvider (RIZZY-019, RIZZY-037).
import { test, expect } from './fixtures.mjs';

async function submit(page, name) {
    await page.locator('#name').fill(name);
    const response = page.waitForResponse((r) => r.url().endsWith('/home/information') && r.request().method() === 'POST');
    await page.locator('#information button[type="submit"]').click();
    return (await response).status();
}

const fieldMessage = (page) => page.locator('[data-valmsg-for="Person.Name"]');

test.beforeEach(async ({ page }) => {
    await page.goto('/home/information');
});

test('attribute errors come back as 422 next to the field', async ({ page, issues }) => {
    expect(await submit(page, 'averyveryverylongname')).toBe(422);
    await expect(fieldMessage(page)).toHaveText('The field Name must be a string with a maximum length of 10.');
    await expect(page.locator('#name')).toHaveValue('averyveryverylongname');
    await expect(page.locator('#name')).toHaveClass(/invalid/);

    expect(await submit(page, '')).toBe(422);
    await expect(fieldMessage(page)).toHaveText('The Name field is required.');
});

test('server-side rules from the controller are shown too', async ({ page, issues }) => {
    expect(await submit(page, 'admin')).toBe(422);
    await expect(fieldMessage(page)).toHaveText('That name is reserved.');
    await expect(page.locator('#information .validation-errors')).toContainText('That name is reserved.');
});

test('a valid name is saved and the errors clear', async ({ page, issues }) => {
    expect(await submit(page, 'admin')).toBe(422);
    expect(await submit(page, 'Ada')).toBe(200);

    await expect(page.locator('#information .alert-success')).toHaveText('Saved "Ada".');
    await expect(fieldMessage(page)).toHaveText('');
    await expect(page.locator('#information')).toHaveCount(1);
});
