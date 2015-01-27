var gulp = require('gulp');
var del = require('del'); // the funkee homosapien
var args = require('yargs').argv;
var nunit = require('gulp-nunit-runner');
var nuget = require('nuget-runner')({ apiKey: args.nugetApiKey });
var shell = require('gulp-shell');
var msbuild = require('gulp-msbuild');


gulp.task('clean', function() {
  del.sync([
    'src/**/bin/*', 
    'src/**/obj/*',
    '*.nupkg'
  ]);
});

gulp.task('build',[ 'clean' ], function() {
  return gulp
    .src(['src/EFTestable.sln'])
    .pipe(msbuild({
      targets: ['Clean', 'Build'],
      errorOnFail: true,
      stdout: true,
      properties: { Configuration: 'Release' }
    }));
});

// Ensures the NUnit console runner is present.
gulp.task('restoreSlnPackages', shell.task([
  'src\\.nuget\\NuGet.exe restore src\\.nuget\\packages.config -PackagesDirectory src\\packages'
]));

gulp.task('test', ['restoreSlnPackages', 'build'], function () {
  return gulp
    .src(['src/**/bin/Release/*.Tests.dll'], { read: false })
    .pipe(nunit({
      executable: 'src/packages/NUnit.Runners.2.6.4/tools/nunit-console.exe',
      teamcity: true
    }));
});

var deploy = function(assembly) {
  // Copy all package files into a staging folder
  gulp.src('src/' + assembly + '/bin/Release/' + assembly + '/*')
    .pipe(gulp.dest('build/' + assembly));

  return nuget
    .pack({
      spec: 'src/' + assembly + '/' + assembly + '.csproj',
      build: true,
      basePath: 'build/' + assembly,
      properties: {
        Configuration: 'Release'
      }
    })
    .then(function() { return nuget.push('*.nupkg'); });
};

gulp.task('deployMain', ['test'], function() {
  return deploy('EntityFrameworkTestable')
});

gulp.task('deployTesting', ['test'], function() {
  return deploy('EntityFrameworkTestable.Testing')
});

gulp.task('deploy', ['deployMain', 'deployTesting']);

