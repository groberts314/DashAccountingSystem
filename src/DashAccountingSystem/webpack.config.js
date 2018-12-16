let glob = require('glob');

module.exports = {
    mode: 'development',
    entry: {
        common: './Content/Scripts/src/common/index.ts',
        components: './Content/Scripts/src/components/index.ts'
    },
    output: {
        library: '[name]',
        path: require('path').resolve('./Content/Scripts/dist'),
        filename: '[name].js'
    },

    devtool: 'source-map',

    resolve: {
        extensions: ['.webpack.js', '.web.js', '.ts', '.tsx', '.js']
    },

    module: {
        rules: [
            {
                test: /\.(ts|tsx)$/,
                loader: 'ts-loader'
            },
            { enforce: "pre", test: /\.js$/, loader: "source-map-loader" }
        ]
    },

    externals: {
        'react': 'React',
        'react-dom': 'ReactDOM',
        'lodash': '_'
    }
};
