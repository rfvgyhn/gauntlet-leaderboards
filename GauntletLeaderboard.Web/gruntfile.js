module.exports = function (grunt) {

    grunt.loadNpmTasks('grunt-contrib-uglify');
    grunt.loadNpmTasks('grunt-contrib-concat');
    grunt.loadNpmTasks('grunt-contrib-copy');

    // configure plugins
    grunt.initConfig({
        pkg: grunt.file.readJSON('package.json'),
        jquery: 'bower_components/jquery/dist/jquery.js',
        mainJs: 'scripts/main.js',
        mainCss: 'content/site.css',
        bootstrapJs: 'bower_components/bootstrap/dist/js/bootstrap.js',
        bootstrapCss: 'bower_components/bootstrap/dist/css/bootstrap.css',
        copy: {
            main: {
                files: [
                    { src: ['<%= jquery %>'], dest: "tmp/jquery.js" },
                    { src: ['<%= bootstrapJs %>'], dest: "tmp/bootstrap.js" },
                    { src: ['<%= bootstrapCss %>'], dest: "tmp/css/bootstrap.css" },
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
                    '<%= bootstrapJs %>',
                    '<%= mainJs %>',
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
                    'dist/<%= pkg.name %>.min.js': ['<%= concat.dist.dest %>']
                }
            }
        }, 
    });

    // define tasks
    grunt.registerTask('default', ['copy']);
    grunt.registerTask('prod', ['concat', 'uglify']);
};