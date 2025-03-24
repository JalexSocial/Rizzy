const path = require('path');

module.exports = [
    {
        name: 'rizzy',
        entry: path.resolve(__dirname, 'wwwroot/js/rizzy.js'),
        output: {
            filename: 'rizzy.js',
            path: path.resolve(__dirname, 'wwwroot/dist/'),
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
        }
    }
];
