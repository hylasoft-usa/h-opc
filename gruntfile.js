'use strict';

module.exports = function(grunt) {

  // Load grunt tasks automatically
  require('load-grunt-tasks')(grunt);

  // Time how long tasks take. Can help when optimizing build times
  require('time-grunt')(grunt);

  grunt.initConfig({

    // Set this variables for different projects
    projectName: 'h-opc',
    testProjectPath: 'tests/',
    nuspecFile: 'package.nuspec',

    // These variables shouldn't be changed, but sometimes it might be necessary
    solutionName: '<%= projectName %>.sln',
    srcPath: './',
    dotNetVersion: '4.5.0',
    platform: 'Any CPU',
    styleCopRules: 'Settings.StyleCop',
    ruleSet: 'rules.ruleset',

    pkg: grunt.file.readJSON('package.json'),

    assemblyinfo: {
      options: {
        files: ['<%= srcPath %><%= solutionName %>'],
        info: {
          version: '<%= pkg.version %>',
          fileVersion: '<%= pkg.version %>',
          company: 'hylasoft',
          copyright: ' ',
          product: '<%= projectName %>'
        }
      }
    },

    msbuild: {
      release: {
        src: ['<%= srcPath %><%= solutionName %>'],
        options: {
          projectConfiguration: 'Release',
          platform: '<%= platform %>',
          targets: ['Clean', 'Rebuild'],
          version: 12,
          buildParameters: {
            StyleCopEnabled: false
          }
        }
      },
      debug: {
        src: ['<%= srcPath %><%= solutionName %>'],
        options: {
          projectConfiguration: 'Debug',
          platform: '<%= platform %>',
          targets: ['Clean', 'Rebuild'],
          version: 12,
          buildParameters: {
            StyleCopEnabled: true,
            StyleCopTreatErrorsAsWarnings: false,
            StyleCopOverrideSettingsFile: process.cwd() + '/<%= styleCopRules %>',
            RunCodeAnalysis: true,
            CodeAnalysisRuleSet: process.cwd() + '/<%= ruleSet %>',
            TreatWarningsAsErrors: true
          },
        }
      }
    },

    mstest: {
      debug: {
        src: ['<%= srcPath %>/<%= testProjectPath %>/bin/Debug/*.dll'] // Points to test dll
      }
    },

    nugetrestore: {
      restore: {
        src: '<%= srcPath %><%= solutionName %>',
        dest: 'packages/',
				options: {
					msbuildversion: 4
				}
      }
    },

    nugetpack: {
      dist: {
        src: '<%= nuspecFile %>',
        dest: '.',

        options: {
          version: '<%= pkg.version %>',
          basePath: '<%= projectName %>/bin/Release',
          includeReferencedProjects: true,
          excludeEmptyDirectories: true
        }
      }
    },

    nugetpush: {
      src: '*.<%= pkg.version %>.nupkg',
      options: {}
    },

    clean: {
      nuget: ['*.nupkg'],
    },

    shell: {
      options: {
        stderr: false
      },
      nunit: {
        command: '.\\packages\\NUnit.ConsoleRunner.3.4.1\\tools\\nunit3-console.exe tests\\bin\\Debug\\tests.dll'
      }
    }

  });



  grunt.registerTask('default', ['build']);
  grunt.registerTask('build', ['nugetrestore','msbuild:debug','msbuild:release']);
  grunt.registerTask('test', ['nugetrestore','msbuild:debug', 'shell:nunit']);
  grunt.registerTask('release', ['assemblyinfo', 'test']);
  grunt.registerTask('publishNuget', ['release', 'msbuild:release', 'nugetpack', 'nugetpush', 'clean:nuget']);
	grunt.registerTask('coverage', '', function() {
		var exec = require('child_process').execSync;
		var result = exec('packages\\OpenCover.4.6.519\\tools\\OpenCover.Console.exe -register:user -target:.\\packages\\NUnit.ConsoleRunner.3.4.1\\tools\\nunit3-console.exe "-targetargs:""tests\\bin\\Debug\\tests.dll"" "--x86 -output:opencoverCoverage.xml', { encoding: 'utf8' });
		grunt.log.writeln(result);
	});
	grunt.registerTask('viewCoverage', '', function() {
		var exec = require('child_process').execSync;
		var result = exec('packages\\ReportGenerator.2.4.5.0\\tools\\ReportGenerator.exe -reports:"opencoverCoverage.xml" -targetdir:"code_coverage"', { encoding: 'utf8' });
		exec('explorer code_coverage\\index.htm', { encoding: 'utf8' });
	});
	grunt.registerTask('coverageAll', ['coverage', 'viewCoverage']);
};
