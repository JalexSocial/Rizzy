// Browser tests for Rizzy, run against src/RizzyDemo (RM-A7, RIZZY-025).
// They cover what jsdom cannot: native confirm dialogs, CSP enforcement, real history
// navigation and progressive streaming over a real HTTP connection.
import { defineConfig, devices } from '@playwright/test';

const port = Number(process.env.RIZZY_E2E_PORT ?? 5088);
const baseURL = `http://localhost:${port}`;

export default defineConfig({
    testDir: './tests',
    fullyParallel: false,       // one demo server with shared in-memory state (the counter)
    workers: 1,
    retries: process.env.CI ? 1 : 0,
    reporter: process.env.CI ? [['list'], ['html', { open: 'never' }]] : 'list',
    use: {
        baseURL,
        trace: 'retain-on-failure',
    },
    projects: [{ name: 'chromium', use: { ...devices['Desktop Chrome'] } }],
    webServer: {
        // Release build of the demo; CI builds the solution first, so --no-build is safe there.
        command: `dotnet run --project ../../src/RizzyDemo/RizzyDemo.csproj -c Release ${process.env.CI ? '--no-build ' : ''}--no-launch-profile --urls ${baseURL}`,
        url: `${baseURL}/home/`,
        env: { ASPNETCORE_ENVIRONMENT: 'Development' },
        reuseExistingServer: !process.env.CI,
        timeout: 180_000,
    },
});
