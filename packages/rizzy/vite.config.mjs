import { defineConfig }   from 'vite';
import { visualizer }     from 'rollup-plugin-visualizer';
import fs                 from 'fs-extra';
import path               from 'node:path';

/* ———————————————————————————————
   Mirror dist → ../../src/Rizzy/wwwroot/
   wwwroot/js is emptied first so stale bundles from older builds are never
   shipped as static web assets (RIZZY-021).
   ——————————————————————————————— */
const wwwrootDest = path.resolve(__dirname, '../../src/Rizzy/wwwroot/');
function copyToWwwRoot() {
    return {
        name: 'copy-to-wwwroot',
        closeBundle() {
            fs.emptyDirSync(path.join(wwwrootDest, 'js'));
            fs.copySync(path.resolve(__dirname, 'dist'),
                wwwrootDest,
                { overwrite: true, dereference: true });
            console.log(`✔  Copied build artifacts to ${wwwrootDest}`);
        }
    };
}

export default defineConfig(({ mode }) => {
    const isProd     = mode === 'production';
    const isAnalyze  = mode === 'analyze';

    return {
        root: '.',
        publicDir: false,

        plugins: [
            isAnalyze && visualizer(),
            copyToWwwRoot()
        ].filter(Boolean),

        build: {
            outDir: 'dist/js',           // same folder webpack used
            emptyOutDir: !isProd,        // dev build cleans, prod appends
            target: 'es2020',
            sourcemap: !isProd,          // keep map only on un-minified dev build
            minify: isProd,

            /* ——  library mode  —— */
            lib: {
                entry:   path.resolve(__dirname, 'src/js/rizzy.js'),
                name:    'Rizzy',          // global var for UMD build
                formats: ['umd'],    
                fileName: (format) => {
                    return `rizzy${isProd ? '.min' : ''}.js`;
                }
            },

            rollupOptions: {
                // bundle deps exactly like webpack did (no externals)
                output: { preserveModules: false }
            }
        }
    };
});
