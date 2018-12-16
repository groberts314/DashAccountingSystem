const cache = require('gulp-cached');
const del = require('del');
const gulp = require('gulp');
const webpack = require('webpack');
const webpackStream = require('webpack-stream');

const webroot = 'wwwroot';
const contentRoot = 'Content';

// JavaScript
gulp.task('clean:js', () => del([`${webroot}/js/**/*`]));


gulp.task('dist:js', () => gulp.src(
    [
        'node_modules/react/dist/react.js',
        'node_modules/react/dist/react.min.js',
        'node_modules/react-dom/dist/react-dom.js',
        'node_modules/react-dom/dist/react-dom.min.js',
        `${contentRoot}/Scripts/src/**/*.js`
    ])
    .pipe(cache('dist:js'))
    .pipe(gulp.dest(webroot + '/js')));

// Webpack
gulp.task('clean:webpack', () => del([`${contentRoot}/Scripts/dist/**/*`]));

gulp.task('dist:webpack', ['clean:webpack'], () => gulp.src(`${contentRoot}/Scripts/src/**/*`)
    .pipe(webpackStream(require('./webpack.config.js'), webpack))
    .pipe(gulp.dest(`${webroot}/js`)));

// Gulp Scripts
gulp.task('clean', ['clean:webpack', 'clean:js']);
gulp.task('default', ['dist:webpack', 'dist:js']);