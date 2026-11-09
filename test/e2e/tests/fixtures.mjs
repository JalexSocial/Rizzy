// Shared fixture: every test fails on unexpected console errors, page errors, Rizzy config
// warnings or CSP violations. Tests that provoke one on purpose declare it with `expectIssue`.
import { test as base, expect } from '@playwright/test';

// Errors the demo produces on purpose: failed-resource lines for 4xx/5xx responses (validation
// returns 422, the error example returns 500).
const EXPECTED_RESOURCE_ERRORS = /Failed to load resource: the server responded with a status of (422|500)/;

export const test = base.extend({
    issues: async ({ page }, use) => {
        const issues = [];
        const allowed = [];
        const csp = [];

        page.on('console', (msg) => {
            const text = msg.text();
            if (/Content Security Policy/.test(text)) csp.push(text);
            if (msg.type() === 'error' && !EXPECTED_RESOURCE_ERRORS.test(text)) issues.push(`console.error: ${text}`);
            if (msg.type() === 'warning' && text.includes('htmx:rizzy:config:warn')) issues.push(`rizzy warning: ${text}`);
        });
        page.on('pageerror', (error) => issues.push(`page error: ${error.message}`));

        const api = {
            /** Declares an issue the test provokes on purpose (matched by substring or RegExp). */
            expectIssue: (pattern) => allowed.push(pattern),
            /** CSP violations the browser reported on the console during this test. */
            cspViolations: () => [...csp],
        };

        await use(api);

        const unexpected = issues.filter((issue) =>
            !allowed.some((p) => (p instanceof RegExp ? p.test(issue) : issue.includes(p))));
        expect(unexpected, 'unexpected console errors, page errors or Rizzy warnings').toEqual([]);
    },
});

export { expect };

/** Clicks a sidebar link (boosted: hx-target="article", pushes the URL). */
export async function navigate(page, label) {
    await page.locator('nav a', { hasText: label }).click();
}

/** Marks the sidebar element so a test can tell whether htmx replaced it. */
export async function markSidebar(page) {
    await page.locator('#sidebar').evaluate((el) => { el.dataset.e2eMarker = 'original'; });
}

export async function sidebarIsOriginal(page) {
    return page.locator('#sidebar').evaluate((el) => el.dataset.e2eMarker === 'original');
}
