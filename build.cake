#tool nuget:?package=GitReleaseManager
#tool nuget:?package=OpenCover
#tool nuget:?package=Codecov
#addin "Cake.Figlet"
#addin nuget:?package=Cake.Codecov
#addin nuget:?package=Cake.AppVeyor

var target = Argument("target", "Default");
var configuration   = "Release";

///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////
// We define where the build artifacts should be places
// this is relative to the project root folder
var buildArtifacts                  = Directory("./artifacts");
var deployment                      = Directory("./artifacts/deployment");
var version                         = "99.0.0";

///////////////////////////////////////////////////////////////////////////////
// MODULES
///////////////////////////////////////////////////////////////////////////////
var modules = Directory("./src");
// To skip building a project in the source folder add the project folder name
// as string to the list e.g. "Yaapii.SimEngine.Tmx.Setup".
var blacklistModules = new List<string>() { };

// Unit tests
var unitTests = Directory("./tests");
// To skip executing a test in the tests folder add the test project folder name
// as string to the list e.g. "TmxTest.Yaapii.Olp.Tmx.AllInOneRobot".
var blacklistUnitTests = new List<string>() { }; 

///////////////////////////////////////////////////////////////////////////////
// CONFIGURATION VARIABLES
///////////////////////////////////////////////////////////////////////////////
var isAppVeyor                      = AppVeyor.IsRunningOnAppVeyor;
var isWindows                       = IsRunningOnWindows();

// For GitHub release
var owner                           = "icarus-consulting";
var repository                      = "Yaapii.Xml";

// For NuGetFeed
var nuGetSource = "https://api.nuget.org/v3/index.json";

// API key tokens for deployment
var gitHubToken                     = "";
var nugetReleaseToken               = "";
var codeCovToken                    = "";

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
// Appveyor Build
///////////////////////////////////////////////////////////////////////////////
Task("AppveyorBuild")
.WithCriteria(() => isAppVeyor)
.IsDependentOn("Version")
.Does(() =>
{
    Information(Figlet("AppveyorBuild"));
    
    var appVeyorBuild = new AppVeyorBuild();
    appVeyorBuild.Version = $"{version}.{appVeyorBuild.BuildNumber}";
    Information($"{version}.{appVeyorBuild.BuildNumber}");
});

///////////////////////////////////////////////////////////////////////////////
// Clean
///////////////////////////////////////////////////////////////////////////////
Task("Clean")
.Does(() =>
{
    Information(Figlet("Clean"));
    
	// Clean the artifacts folder to prevent old builds be present
	// https://cakebuild.net/dsl/directory-operations/
    CleanDirectories(new DirectoryPath[] { buildArtifacts });
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
.IsDependentOn("Clean")
.IsDependentOn("Restore")
.IsDependentOn("Version")
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
	foreach(var module in GetSubDirectories(modules))
	{
		if(!blacklistModules.Contains(module.GetDirectoryName()))
		{
			Information($"Building {module.GetDirectoryName()}");
			
			DotNetCoreBuild(
				module.FullPath,
				settings
			);
		}
		else
		{
			Warning($"Skipping build {module.GetDirectoryName()}");
		}
	}
});

///////////////////////////////////////////////////////////////////////////////
// Test
///////////////////////////////////////////////////////////////////////////////
Task("Test")
.IsDependentOn("Build")
.Does(() => 
{
    Information(Figlet("Unit Test"));

	var settings = 
		new DotNetCoreTestSettings()
		{
			Configuration = configuration,
			NoRestore = true
		};
	foreach(var test in GetSubDirectories(unitTests))
	{
		if(!blacklistUnitTests.Contains(test.GetDirectoryName()))
		{
			Information($"Testing {test.GetDirectoryName()}");
			DotNetCoreTest(
				test.FullPath,
				settings
			);	
		}
		else
		{
			Warning($"Skipping test {test.GetDirectoryName()}");
		}
	}
});    

///////////////////////////////////////////////////////////////////////////////
// Nuget
///////////////////////////////////////////////////////////////////////////////
Task("Nuget")
.IsDependentOn("Clean")
.IsDependentOn("Version")
.Does(() =>
{
    Information(Figlet("NuGet"));
    
    var settings = new DotNetCorePackSettings()
    {
        Configuration = configuration,
        OutputDirectory = buildArtifacts,
        VersionSuffix = ""
    };
    settings.ArgumentCustomization = args => args.Append("--include-symbols");
    settings.MSBuildSettings = new DotNetCoreMSBuildSettings().SetVersionPrefix(version);
    
    foreach(var module in GetSubDirectories(modules))
	{
		if(!blacklistModules.Contains(module.GetDirectoryName()))
		{
			Information($"Creating NuGet package for {module.GetDirectoryName()}");
			
			DotNetCorePack(
				module.ToString(),
				settings
			);
		}
		else
		{
			Warning($"Skipping NuGet package for {module.GetDirectoryName()}");
		}
	}
});

///////////////////////////////////////////////////////////////////////////////
// Code Coverage
///////////////////////////////////////////////////////////////////////////////
Task("GenerateCoverage")
.IsDependentOn("Build")
.Does(() => 
{
    Information(Figlet("GenerateCoverage"));
    
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
            new FilePath("./coverage.xml"),
            new OpenCoverSettings()
            {
                OldStyle = true
            }
            .WithFilter("+[Yaapii.Xml]*")
        );
    }
    catch(Exception ex)
    {
        Information("Error: " + ex.ToString());
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
    
	gitHubToken = EnvironmentVariable("GITHUB_TOKEN");
	nugetReleaseToken = EnvironmentVariable("NUGET_TOKEN");
    codeCovToken = EnvironmentVariable("CODECOV_TOKEN");
});

///////////////////////////////////////////////////////////////////////////////
// UploadCoverage
///////////////////////////////////////////////////////////////////////////////
Task("UploadCoverage")
.IsDependentOn("GenerateCoverage")
.IsDependentOn("Credentials")
.WithCriteria(() => isAppVeyor)
.Does(() =>
{
    Information(Figlet("UploadCoverage"));
    
    Codecov("coverage.xml", codeCovToken);
});

///////////////////////////////////////////////////////////////////////////////
// GitHubRelease
///////////////////////////////////////////////////////////////////////////////
Task("GitHubRelease")
.WithCriteria(() => isAppVeyor && BuildSystem.AppVeyor.Environment.Repository.Tag.IsTag)
.IsDependentOn("Nuget")
.IsDependentOn("Version")
.IsDependentOn("Credentials")
.Does(() => 
{
    Information(Figlet("GitHub Release"));
    
    GitReleaseManagerCreate(
        gitHubToken,
        owner,
        repository, 
        new GitReleaseManagerCreateSettings {
            Milestone         = version,
            Name              = version,
            Prerelease        = false,
            TargetCommitish   = "master"
        }
    );

    // Collect here all files you need for the release on GitHub
    // e.g. you can zip the whole artifacts folder deploy the zip
    // here you see an expample to deploy all nuget packages in the root of the artifacts folder:
    var nugets = string.Join(",", GetFiles("./artifacts/*.nupkg").Select(f => f.FullPath) );
    Information($"Release files:{Environment.NewLine}  " + nugets.Replace(",", $"{Environment.NewLine}  "));
    GitReleaseManagerAddAssets(
        gitHubToken,
        owner,
        repository,
        version,
        nugets
    );
    GitReleaseManagerPublish(gitHubToken, owner, repository, version);
});

///////////////////////////////////////////////////////////////////////////////
// NuGetFeed
///////////////////////////////////////////////////////////////////////////////
Task("NuGetFeed")
.WithCriteria(() => isAppVeyor && BuildSystem.AppVeyor.Environment.Repository.Tag.IsTag)
.IsDependentOn("Nuget")
.Does(() => 
{
    Information(Figlet("NuGetFeed"));
    
    var nugets = GetFiles($"{buildArtifacts.Path}/*.nupkg");
    foreach(var package in nugets)
    {
        if(!package.GetFilename().FullPath.EndsWith("symbols.nupkg"))
        {
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
.IsDependentOn("Version")
.IsDependentOn("AppveyorBuild")
.IsDependentOn("Clean")
.IsDependentOn("Restore")
.IsDependentOn("Build")
.IsDependentOn("Test")
.IsDependentOn("Nuget")
.IsDependentOn("GenerateCoverage")
.IsDependentOn("Credentials")
.IsDependentOn("UploadCoverage")
.IsDependentOn("GitHubRelease")
.IsDependentOn("NuGetFeed")
;

RunTarget(target);