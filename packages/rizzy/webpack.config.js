const path = require('path');
const CopyPlugin = require("copy-webpack-plugin");

const targetWwwRoot = path.resolve(__dirname, '../../src/Rizzy/wwwroot/');

module.exports = [
    {
        name: 'rizzy',
        entry: path.resolve(__dirname, 'src/js/rizzy.js'),
        output: {
            filename: 'rizzy.js',
            path: path.resolve(__dirname, 'dist/js/'),
        },
        mode: 'development', // Enables unminified output
        devtool: 'source-map', // Optional: Generates source maps for easier debugging
        module: {
            rules: [
                {
                    test: /\.js$/,
                    exclude: /node_modules/,
                    use: {
                        loader: 'babel-loader',
                        options: {
                            presets: ['@babel/preset-env']
                        }
                    }
                }
            ]
        },
        resolve: {
            extensions: ['.js'], // Automatically resolve these extensions
            modules: ['node_modules'], // Allow imports from node_modules
        },
        experiments: {
            outputModule: true,
        },
        plugins: [
            new CopyPlugin({
                patterns: [
                    {
                        from: path.resolve(__dirname, 'dist'), // Copy FROM the Webpack output dir
                        to: targetWwwRoot,                   // Copy TO the target wwwroot/dist dir
                        globOptions: {
                            // Optional: ignore files like source maps
                            // ignore: ["**/*.map"],
                        },
                        noErrorOnMissing: true, // Don't error if 'dist' doesn't exist before first build
                        force: true, // Overwrite files in the destination
                    },
                ],
            }),
        ]
    },
    {
        name: 'rizzy',
        entry: path.resolve(__dirname, 'src/js/rizzy.js'),
        output: {
            filename: 'rizzy.min.js',
            path: path.resolve(__dirname, 'dist/js/'),
        },
        mode: 'production', // Enables unminified output
        //devtool: 'source-map', // Optional: Generates source maps for easier debugging
        module: {
            rules: [
                {
                    test: /\.js$/,
                    exclude: /node_modules/,
                    use: {
                        loader: 'babel-loader',
                        options: {
                            presets: ['@babel/preset-env']
                        }
                    }
                }
            ]
        },
        resolve: {
            extensions: ['.js'], // Automatically resolve these extensions
            modules: ['node_modules'], // Allow imports from node_modules
        },
        experiments: {
            outputModule: true,
        },
        plugins: [
            new CopyPlugin({
                patterns: [
                    {
                        from: path.resolve(__dirname, 'dist'), // Copy FROM the Webpack output dir
                        to: targetWwwRoot,                   // Copy TO the target wwwroot/dist dir
                        globOptions: {
                            // Optional: ignore files like source maps
                            // ignore: ["**/*.map"],
                        },
                        noErrorOnMissing: true, // Don't error if 'dist' doesn't exist before first build
                        force: true, // Overwrite files in the destination
                    },
                ],
            }),
        ]
    }
];
