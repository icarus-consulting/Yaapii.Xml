#tool nuget:?package=OpenCover&version=4.7.922
#tool nuget:?package=Codecov&version=1.12.3
#addin nuget:?package=Cake.Figlet&version=1.3.1
#addin nuget:?package=Cake.Codecov&version=0.9.1
#addin nuget:?package=Cake.Incubator&version=5.1.0

var target                  = Argument("target", "Default");
var configuration           = "Release";

///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////
// We define where the build artifacts should be places
// this is relative to the project root folder
var buildArtifacts          = Directory("./artifacts");
var deployment              = Directory("./artifacts/deployment");
var version                 = "1.0.0";

///////////////////////////////////////////////////////////////////////////////
// MODULES
///////////////////////////////////////////////////////////////////////////////
var modules                 = Directory("./src");
// To skip building a project in the source folder add the project folder name
// as string to the list e.g. "Yaapii.SimEngine.Tmx.Setup".
var blacklistedModules      = new List<string>() { };

// Unit tests
var unitTests               = Directory("./tests");
// To skip executing a test in the tests folder add the test project folder name
// as string to the list e.g. "TmxTest.Yaapii.Olp.Tmx.AllInOneRobot".
var blacklistedUnitTests    = new List<string>() { }; 

///////////////////////////////////////////////////////////////////////////////
// CONFIGURATION VARIABLES
///////////////////////////////////////////////////////////////////////////////
var isAppVeyor              = AppVeyor.IsRunningOnAppVeyor;
var isWindows               = IsRunningOnWindows();

// For GitHub release
var owner                   = "icarus-consulting";
var repository              = "Yaapii.Xml";

// For NuGetFeed
var nuGetSource             = "https://api.nuget.org/v3/index.json";
var appVeyorNuGetFeed       = "https://ci.appveyor.com/nuget/icarus/api/v2/package";

// API key tokens for deployment
var nugetReleaseToken       = "";
var appVeyorFeedToken       = "";
var codeCovToken            = "";

///////////////////////////////////////////////////////////////////////////////
// Version
///////////////////////////////////////////////////////////////////////////////
Task("Version")
.WithCriteria(() => isAppVeyor && BuildSystem.AppVeyor.Environment.Repository.Tag.IsTag)
.Does(() => 
{
    Information(Figlet("Version"));
    
    version = BuildSystem.AppVeyor.Environment.Repository.Tag.Name;
    Information($"Set version to '{version}'");
});

///////////////////////////////////////////////////////////////////////////////
// Clean
///////////////////////////////////////////////////////////////////////////////
Task("Clean")
.Does(() =>
{
    Information(Figlet("Clean"));
    
    CleanDirectories(new DirectoryPath[] { buildArtifacts });
    foreach(var module in GetSubDirectories(modules))
    {
        var name = module.GetDirectoryName();
        if(!blacklistedModules.Contains(name))
        {
            CleanDirectories(
                new DirectoryPath[] 
                { 
                    $"{module}/bin",
                    $"{module}/obj",
                }
            );
        }
    }
});

///////////////////////////////////////////////////////////////////////////////
// Restore
///////////////////////////////////////////////////////////////////////////////
Task("Restore")
.Does(() =>
{
    Information(Figlet("Restore"));
    
    NuGetRestore($"./{repository}.sln");
});

///////////////////////////////////////////////////////////////////////////////
// Build
///////////////////////////////////////////////////////////////////////////////
Task("Build")
.IsDependentOn("Version")
.IsDependentOn("Clean")
.IsDependentOn("Restore")
.Does(() =>
{
    Information(Figlet("Build"));

    var settings = 
        new DotNetCoreBuildSettings()
        {
            Configuration = configuration,
            NoRestore = true,
            MSBuildSettings = new DotNetCoreMSBuildSettings().SetVersionPrefix(version)
        };
        var skipped = new List<string>();
    foreach(var module in GetSubDirectories(modules))
    {
        var name = module.GetDirectoryName();
        if(!blacklistedModules.Contains(name))
        {
            Information($"Building {name}");
            
            DotNetCoreBuild(
                module.FullPath,
                settings
            );
        }
        else
        {
            skipped.Add(name);
        }
    }
    if (skipped.Count > 0)
    {
        Warning("The following builds have been skipped:");
        foreach(var name in skipped)
        {
            Warning($"  {name}");
        }
    }
});

///////////////////////////////////////////////////////////////////////////////
// Unit Tests
///////////////////////////////////////////////////////////////////////////////
Task("UnitTests")
.IsDependentOn("Build")
.Does(() => 
{
    Information(Figlet("Unit Tests"));

    var settings = 
        new DotNetCoreTestSettings()
        {
            Configuration = configuration,
            NoRestore = true
        };
    var skipped = new List<string>();   
    foreach(var test in GetSubDirectories(unitTests))
    {
        var name = test.GetDirectoryName();
        if(blacklistedUnitTests.Contains(name))
        {
            skipped.Add(name);
        }
        else if(!name.StartsWith("TmxTest"))
        {
            Information($"Testing {name}");
            DotNetCoreTest(
                test.FullPath,
                settings
            );
        }
    }
    if (skipped.Count > 0)
    {
        Warning("The following tests have been skipped:");
        foreach(var name in skipped)
        {
            Warning($"  {name}");
        }
    }
});

///////////////////////////////////////////////////////////////////////////////
// Generate Coverage
///////////////////////////////////////////////////////////////////////////////
Task("GenerateCoverage")
.IsDependentOn("Build")
.Does(() => 
{
    Information(Figlet("Generate Coverage"));
    
    try
    {
        OpenCover(
            tool => 
            {
                tool.DotNetCoreTest(
                    "./tests/Test.Yaapii.Xml/",
                    new DotNetCoreTestSettings
                    {
                        Configuration = configuration
                    }
                );
            },
            new FilePath($"{buildArtifacts.Path}/coverage.xml"),
            new OpenCoverSettings()
            {
                OldStyle = true
            }.WithFilter("+[Yaapii.Xml]*")
        );
    }
    catch(Exception ex)
    {
        Information("Error: " + ex.ToString());
    }
});

///////////////////////////////////////////////////////////////////////////////
// Upload Coverage
///////////////////////////////////////////////////////////////////////////////
Task("UploadCoverage")
.IsDependentOn("GenerateCoverage")
.IsDependentOn("Credentials")
.WithCriteria(() => isAppVeyor)
.Does(() =>
{
    Information(Figlet("Upload Coverage"));
    
    Codecov($"{buildArtifacts.Path}/coverage.xml", codeCovToken);
});

///////////////////////////////////////////////////////////////////////////////
// Assert Packages
///////////////////////////////////////////////////////////////////////////////
Task("AssertPackages")
.Does(() => 
{
    Information(Figlet("Assert Packages"));

    foreach (var module in GetSubDirectories(modules))
    {
        var name = module.GetDirectoryName();
        if(!blacklistedModules.Contains(name))
        {
            var project = ParseProject(new FilePath($"{module}/{name}.csproj"), configuration);
            var packageVersion = new Dictionary<string, string>();
            foreach (var package in project.PackageReferences)
            {
                packageVersion.Add(package.Name, package.Version);
            }

            foreach (var package in packageVersion)
            {
                if (package.Key.Contains(".Sources"))
                {
                    var nonSourcesPackage = package.Key.Replace(".Sources", string.Empty);
                    if (packageVersion[nonSourcesPackage] != package.Value)
                    {
                        throw new Exception(
                            $"Reference nuget packages must have equal version in project {name}:{Environment.NewLine}"
                            + $"\t{package.Key} {package.Value} and {nonSourcesPackage} {packageVersion[nonSourcesPackage]}.{Environment.NewLine}"
                            + $"\tUpdate nuget package in the {name}.csproj file.{Environment.NewLine}"
                            + $"\tHint: search for '<PackageReference Include=\"{package.Key}\" Version=\"{package.Value}\" Condition=\"'$(Configuration)' == 'ReleaseSources'\">'."
                        );    
                    }
                }
            }
        }
    }
    Information("Package validation passed.");
});

///////////////////////////////////////////////////////////////////////////////
// NuGet
///////////////////////////////////////////////////////////////////////////////
Task("NuGet")
.IsDependentOn("Version")
.IsDependentOn("Clean")
.IsDependentOn("AssertPackages")
.IsDependentOn("Restore")
.IsDependentOn("Build")
.Does(() =>
{
    Information(Figlet("NuGet"));
    
    var settings = new DotNetCorePackSettings()
    {
        Configuration = configuration,
        OutputDirectory = buildArtifacts,
        NoRestore = false,
        NoBuild = false,
        VersionSuffix = ""
    };
    settings.ArgumentCustomization = args => args.Append("--include-symbols").Append("-p:SymbolPackageFormat=snupkg");
    settings.MSBuildSettings = new DotNetCoreMSBuildSettings().SetVersionPrefix(version);

    var settingsSources = new DotNetCorePackSettings()
    {
        Configuration = "ReleaseSources",
        OutputDirectory = buildArtifacts,
        NoRestore = false,
        NoBuild = false,
        VersionSuffix = ""
    };
    settingsSources.MSBuildSettings = new DotNetCoreMSBuildSettings().SetVersionPrefix(version);

    foreach (var module in GetSubDirectories(modules))
    {
        var name = module.GetDirectoryName();
        if(!blacklistedModules.Contains(name))
        {
            DotNetCorePack(
                module.ToString(),
                settings
            );

            settingsSources.ArgumentCustomization = args => args.Append($"-p:PackageId={name}.Sources").Append("-p:IncludeBuildOutput=false");
            DotNetCorePack(
                module.ToString(),
                settingsSources
            );
        }
        else
        {
            Warning($"Skipping NuGet package for {name}");
        }
    }
});

///////////////////////////////////////////////////////////////////////////////
// Credentials
///////////////////////////////////////////////////////////////////////////////
Task("Credentials")
.WithCriteria(() => isAppVeyor)
.Does(() =>
{
    Information(Figlet("Credentials"));
    
    nugetReleaseToken = EnvironmentVariable("NUGET_TOKEN");
    if (string.IsNullOrEmpty(nugetReleaseToken))
    {
        throw new Exception("Environment variable 'NUGET_TOKEN' is not set");
    }
    appVeyorFeedToken = EnvironmentVariable("APPVEYOR_TOKEN");
    if (string.IsNullOrEmpty(appVeyorFeedToken))
    {
        throw new Exception("Environment variable 'APPVEYOR_TOKEN' is not set");
    }
    codeCovToken = EnvironmentVariable("CODECOV_TOKEN");
    if (string.IsNullOrEmpty(codeCovToken))
    {
        throw new Exception("Environment variable 'CODECOV_TOKEN' is not set");
    }
});

///////////////////////////////////////////////////////////////////////////////
// NuGet Feed
///////////////////////////////////////////////////////////////////////////////
Task("NuGetFeed")
.WithCriteria(() => isAppVeyor && BuildSystem.AppVeyor.Environment.Repository.Tag.IsTag)
.IsDependentOn("NuGet")
.IsDependentOn("Credentials")
.Does(() => 
{
    Information(Figlet("NuGet Feed"));
    
    var nugets = GetFiles($"{buildArtifacts.Path}/*.nupkg");
    foreach(var package in nugets)
    {
        if (package.GetFilename().ToString().Contains(".Sources"))
        {
            NuGetPush(
                package,
                new NuGetPushSettings {
                    Source = appVeyorNuGetFeed,
                    ApiKey = appVeyorFeedToken
                }
            );
        }
        else
        {
            // pushes symbols package too (see https://docs.microsoft.com/en-us/nuget/create-packages/symbol-packages-snupkg)
            NuGetPush(
                package,
                new NuGetPushSettings {
                    Source = nuGetSource,
                    ApiKey = nugetReleaseToken
                }
            );
        }
    }
});

///////////////////////////////////////////////////////////////////////////////
// Default
///////////////////////////////////////////////////////////////////////////////
Task("Default")
.IsDependentOn("Credentials")
.IsDependentOn("Version")
.IsDependentOn("Clean")
.IsDependentOn("Restore")
.IsDependentOn("Build")
.IsDependentOn("UnitTests")
.IsDependentOn("GenerateCoverage")
.IsDependentOn("UploadCoverage")
.IsDependentOn("AssertPackages")
.IsDependentOn("NuGet")
.IsDependentOn("NuGetFeed");

RunTarget(target);