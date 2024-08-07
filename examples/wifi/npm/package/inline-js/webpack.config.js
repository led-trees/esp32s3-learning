"use strict";

const path = require('path');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const CleanCSSPlugin = require("less-plugin-clean-css");
const TerserPlugin = require("terser-webpack-plugin");
const HtmlWebpackPlugin = require("html-webpack-plugin");
const HtmlInlineScriptPlugin = require('html-inline-script-webpack-plugin');
const HTMLInlineCSSWebpackPlugin = require("html-inline-css-webpack-plugin").default;

let bundleOutputDir = '../../../client';
const sourceDir = path.resolve(__dirname, "src");

const lessLoaderOptions = { webpackImporter: true, lessOptions: { math: 'always', plugins: [new CleanCSSPlugin({ advanced: true })] } };

module.exports = (env) => {
    const isDevBuild = process.env.NODE_ENV !== "production";

    console.log(`NODE_ENV: "${process.env.NODE_ENV}"`);
    console.log(`isDevBuild: ${isDevBuild}`);

    return [{
        mode: isDevBuild ? "development" : "production",
        entry: {
            app: path.resolve(__dirname, 'src', 'index.ts')
        },
        resolve: { 
			extensions: ['.js', '.jsx', '.ts', '.tsx', '.less'],
		},
        output: {
            path: path.join(__dirname, bundleOutputDir),
            filename: '[name].js',
            iife: true,
            clean: true,
        },
        module: {
            rules: [
                {
                    test: /\.(?:ts|js|mjs|cjs)$/,
                    exclude: /node_modules/,
                    use: {
                        loader: 'ts-loader'
                    }
                },
                {
                    test: /\.(le|c)ss$/,
                    use: [
                        { loader: MiniCssExtractPlugin.loader },
                        { loader: 'css-loader', options: { importLoaders: 2 } },
                        { loader: 'less-loader', options: lessLoaderOptions }
                    ]
                },
                {
                    test: /\.html$/,
					include: /pages/,
                    use: [ { loader: "raw-loader" } ]
                },
                {
                    test: /\.svg$/,
                    use: [
                        { loader: "raw-loader" },
                        {
                            loader: "svgo-loader",
                            options: {
                                configFile: __dirname + "/svgo.config.mjs",
                                floatPrecision: 2,
                            }
                        }
                    ]
                },
                {
                    test: /\.(png|jpg|jpeg|gif)$/,
                    use: 'url-loader?limit=25000'
                },
            ]
        },
        optimization: {
            minimize: !isDevBuild,
            minimizer: [
                new TerserPlugin({
                    terserOptions: {
                        compress: true,
                        keep_classnames: false,
                        keep_fnames: false,
                        format: {
                            comments: false
                        },
						sourceMap: false
                    },
                    extractComments: false
                })
            ],
            removeAvailableModules: false,
            removeEmptyChunks: true,
            providedExports: false,
            usedExports: true
        },
        plugins: [
            new MiniCssExtractPlugin(),
            new HtmlWebpackPlugin({
                filename: "index.html",
                template: path.join(sourceDir, "template.html"),
                inject: 'body'
            }),
            new HtmlInlineScriptPlugin(),
            new HTMLInlineCSSWebpackPlugin(),
        ]
    }];
};