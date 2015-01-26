var gulp = require('gulp');
var del = require('del'); // the funkee homosapien
var args = require('yargs').argv,
    Nuget = require('nuget-runner');

gulp.task('clean', function() {
  del.sync(['src/**/bin/*', 'src/**/obj/*']);
});

gulp.task('build', 'clean', function() {
  return gulp
    .src(['src/EFTestable.sln'])
    .pipe(msbuild({
      targets: ['Clean', 'Build'],
      errorOnFail: true,
      stdout: true,
      properties: { Configuration: 'Release' }
    }));
});

gulp.task('deploy', ['build'], function() {

    // Copy all package files into a staging folder
    gulp.src('src/MyLibrary/bin/Release/MyLibrary.*')
        .pipe(gulp.dest('build'));

    var nuget = Nuget({ apiKey: args.nugetApiKey });

    return nuget
        .pack({
            spec: 'EFTestable.nuspec',
            basePath: 'build', // Specify the staging folder as the base path
            version: args.buildVersion
        })
        .then(function() { return nuget.push('*.nupkg'); });
});