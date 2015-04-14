module.exports = function (grunt) {

    grunt.loadNpmTasks('grunt-contrib-uglify');
    grunt.loadNpmTasks('grunt-contrib-cssmin');
    grunt.loadNpmTasks('grunt-contrib-concat');
    grunt.loadNpmTasks('grunt-contrib-copy');

    // configure plugins
    grunt.initConfig({
        pkg: grunt.file.readJSON('package.json'),
        jquery: 'bower_components/jquery/dist/jquery.js',
        mainJs: 'scripts/main.js',
        mainCss: 'content/Site.css',
        bootstrapTheme: 'content/gauntlet-leaderboards.bootstrap.theme.css',
        dataTables: 'bower_components/datatables/media/js/jquery.dataTables.js',
        dataTablesBs: 'bower_components/datatables-bootstrap3/BS3/assets/js/datatables.js',
        bootstrapJs: 'bower_components/bootstrap/dist/js/bootstrap.js',
        bootstrapCss: 'bower_components/bootstrap/dist/css/bootstrap.css',
        fontAwesomeCss: 'bower_components/components-font-awesome/css/font-awesome.css',
        fontAwesomeFonts: 'bower_components/components-font-awesome/fonts/*',
        dataTablesBsCss: 'bower_components/datatables-bootstrap3/BS3/assets/css/datatables.css',
        copy: {
            main: {
                files: [
                    { src: ['<%= jquery %>'], dest: "tmp/jquery.js" },
                    { src: ['<%= bootstrapJs %>'], dest: "tmp/bootstrap.js" },
                    { src: ['<%= dataTables %>'], dest: "tmp/dataTables.js" },
                    { src: ['<%= dataTablesBs %>'], dest: "tmp/datatables-bootstrap.js" },
                    { src: ['<%= bootstrapCss %>'], dest: "tmp/css/bootstrap.css" },
                    { src: ['<%= fontAwesomeCss %>'], dest: "tmp/css/font-awesome.css" },
                    { src: ['<%= fontAwesomeFonts %>'], dest: "tmp/fonts/", expand: true, flatten: true },
                    { src: ['<%= dataTablesBsCss %>'], dest: "tmp/css/datatables-bootstrap.css" },
                ]
            }
        },
        concat: {
            options: {
                separator: ';'
            },
            
            js: {
                src: [
                    '<%= dataTables %>',
                    '<%= dataTablesBs %>',
                    '<%= mainJs %>',
                ],
                dest: 'tmp/<%= pkg.name %>.js'
            },
            css: {
                src: [
                    '<%= bootstrapTheme %>',
                    '<%= dataTablesBsCss %>',
                    '<%= mainCss %>',
                ],
                dest: 'tmp/<%= pkg.name %>.css'
            }
        },
        uglify: {
            options: {
                banner: '/*! <%= pkg.name %> <%= grunt.template.today("dd-mm-yyyy") %> */'
            },
            dist: {
                files: {
                    'dist/<%= pkg.name %>.min.js': ['<%= concat.js.dest %>']
                }
            }
        },
        cssmin: {
            target: {
                files: {
                    'dist/<%= pkg.name %>.min.css': ['<%= concat.css.dest %>']
                }
            }
        }
    });

    // define tasks
    grunt.registerTask('default', ['copy']);
    grunt.registerTask('prod', ['concat', 'uglify', 'cssmin']);
};