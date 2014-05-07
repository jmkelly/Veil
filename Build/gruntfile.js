module.exports = function (grunt) {
    var pkg = grunt.file.readJSON("package.json");

    grunt.initConfig({
        pkg: pkg,
        assemblyinfo: {
            options: {
                files: ['../Src/Veil.sln'],

                // Standard assembly info
                info: {
                    version: pkg.assemblyVersion, 
                    fileVersion: pkg.assemblyVersion
                }
            }
        },        
        msbuild: {
            core: {
                src: ['../src/Veil/Veil.csproj'],
                options: {
                    projectConfiguration: 'Release',
                    target: ['Clean', 'Rebuild'],
                    stdout: true,
                    buildParameters: {
                        documentationFile: 'Veil.xml'
                    }
                }
            },
            supersimple: {
                src: ['../src/Veil.SuperSimple/Veil.SuperSimple.csproj'],
                options: {
                    projectConfiguration: 'Release',
                    target: ['Clean', 'Rebuild'],
                    stdout: true
                }
            },
            handlebars: {
                src: ['../src/Veil.Handlebars/Veil.Handlebars.csproj'],
                options: {
                    projectConfiguration: 'Release',
                    target: ['Clean', 'Rebuild'],
                    stdout: true
                }
            },
            nancysupersimple: {
                src: ['../src/Nancy.ViewEngines.Veil.SuperSimple/Nancy.ViewEngines.Veil.SuperSimple.csproj'],
                options: {
                    projectConfiguration: 'Release',
                    target: ['Clean', 'Rebuild'],
                    stdout: true
                }
            }
        },
        clean: {
            dist: {
                src: ["./dist"]
            }
        },        
        copy: {
            nuspec: {
                files: [{
                    expand: true,
                    cwd: '../Src/Nuspec/',
                    src: ['*.nuspec'],
                    dest: 'dist/'
                }]
            },
            lib: {
                files: [{
                    expand: true,
                    cwd: '../src/Veil/bin/Release/',
                    src: ['Veil.*'],
                    dest: 'dist/lib/net40'
                },{
                    expand: true,
                    cwd: '../src/Veil/',
                    src: ['Veil.xml'],
                    dest: 'dist/lib/net40'
                },{
                    expand: true,
                    cwd: '../src/Veil.SuperSimple/bin/Release/',
                    src: ['Veil.SuperSimple.*'],
                    dest: 'dist/lib/net40'
                },{
                    expand: true,
                    cwd: '../src/Veil.Handlebars/bin/Release/',
                    src: ['Veil.Handlebars.*'],
                    dest: 'dist/lib/net40'
                },{
                    expand: true,
                    cwd: '../src/Nancy.ViewEngines.Veil.SuperSimple/bin/Release/',
                    src: ['Nancy.ViewEngines.Veil.SuperSimple.*'],
                    dest: 'dist/lib/net40'
                }]
            }
        },
        nugetpack: {
            core: {
                src: 'dist/Veil.nuspec',
                dest: 'dist/',
                options: {
                    version: pkg.version
                }
            },
            supersimple: {
                src: 'dist/Veil.SuperSimple.nuspec',
                dest: 'dist/',
                options: {
                    version: pkg.version
                }                
            },
            handlebars: {
                src: 'dist/Veil.Handlebars.nuspec',
                dest: 'dist/',
                options: {
                    version: pkg.version
                }                
            },
            nancysupersimple: {
                src: 'dist/Nancy.ViewEngines.Veil.SuperSimple.nuspec',
                dest: 'dist/',
                options: {
                    version: pkg.version
                }                
            }            
        }
    });
    grunt.loadNpmTasks('grunt-nuget');
    grunt.loadNpmTasks('grunt-msbuild');
    grunt.loadNpmTasks('grunt-contrib-copy');
    grunt.loadNpmTasks('grunt-contrib-clean');
    grunt.loadNpmTasks('grunt-dotnet-assembly-info');
 
    grunt.registerTask("default", ["clean", "assemblyinfo", "msbuild", "copy", "nugetpack"]);
};