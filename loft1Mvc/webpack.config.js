﻿const path = require('path');

module.exports = {
    entry: './wwwroot/ts/main.ts',
    output: {
        filename: 'main.bundle.js',
        path: path.join(__dirname, 'wwwroot/dist')
    },
    externals: {
        // shows how we can rely on browser globals instead of bundling these dependencies,
        // in case we want to access jQuery from a CDN or if we want an easy way to
        // avoid loading all moment locales: https://github.com/moment/moment/issues/1435
        jquery: 'jQuery'
    },
    devtool: 'sourcemap',
    resolve: {
        extensions: ['.ts', '.js']
    },
    module: {
        rules: [
            { test: /\.tsx?$/, loader: 'awesome-typescript-loader' },
            { enforce: 'pre', test: /\.js$/, loader: 'source-map-loader' },
            {
                test: /\.css$/,
                use: ['style-loader', 'css-loader']
            }
        ]
    }
};