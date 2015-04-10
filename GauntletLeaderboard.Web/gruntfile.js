module.exports = function (grunt) {

    grunt.loadNpmTasks('grunt-contrib-uglify');
    grunt.loadNpmTasks('grunt-contrib-concat');
    grunt.loadNpmTasks('grunt-contrib-copy');

    // configure plugins
    grunt.initConfig({
        pkg: grunt.file.readJSON('package.json'),
        jquery: 'bower_components/jquery/dist/jquery.js',
        copy: {
            main: {
                files: [
                    { src: ['<%= jquery %>'], dest: "scripts/jquery.js" }
                ]
            }
        },
        concat: {
            options: {
                separator: ';'
            },
            dist: {
                src: [
                    '<%= jquery %>',
                ],
                dest: 'tmp/<%= pkg.name %>.js'
            }
        },
        uglify: {
            options: {
                banner: '/*! <%= pkg.name %> <%= grunt.template.today("dd-mm-yyyy") %> */\n'
            },
            dist: {
                files: {
                    'scripts/<%= pkg.name %>.min.js': ['<%= concat.dist.dest %>']
                }
            }
        }, 
    });

    // define tasks
    grunt.registerTask('default', ['copy']);
    grunt.registerTask('prod', ['concat', 'uglify']);
};