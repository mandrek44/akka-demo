/// <binding />
var gulp = require('gulp');
var run = require('gulp-run');

gulp.task('git-next', function () {
    run('sh --login -c git-next').exec();
});

gulp.task('git-prev', function () {
    run("sh --login -c git-prev").exec();
});