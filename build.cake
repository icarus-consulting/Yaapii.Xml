#tool nuget:?package=GitReleaseManager
#tool nuget:?package=OpenCover
#tool nuget:?package=xunit.runner.console
#tool nuget:?package=Codecov
#tool nuget:?package=ReportGenerator
#addin nuget:?package=Cake.Codecov

var target = Argument("target", "Default");
var configuration   = Argument<string>("configuration", "Release");
var coverageReport = Argument<bool>("report", false);

///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////
var buildArtifacts					= Directory("./artifacts/");
var deployment						= Directory("./artifacts/deployment");

///////////////////////////////////////////////////////////////////////////////
// YAAPII MODULES
///////////////////////////////////////////////////////////////////////////////
var ypXml							= Directory("./src/Yaapii.Xml");
var ypXmlTests						= Directory("./tests/Test.Yaapii.Xml");
var version							= "0.1.1";

///////////////////////////////////////////////////////////////////////////////
// CONFIGURATION VARIABLES
///////////////////////////////////////////////////////////////////////////////
var isAppVeyor          = AppVeyor.IsRunningOnAppVeyor;
var isWindows           = IsRunningOnWindows();
var netcore             = "netcoreapp2.1";
var net                 = "net461";
var netstandard         = "netstandard2.0";

// Important for github release
var owner = "icarus-consulting";
var repository = "Yaapii.Xml";

var githubToken = "";
var codecovToken = "";


///////////////////////////////////////////////////////////////////////////////
// Clean
///////////////////////////////////////////////////////////////////////////////
Task("Clean")
    .Does(() =>
{
    CleanDirectories(new DirectoryPath[] { buildArtifacts });
});


///////////////////////////////////////////////////////////////////////////////
// Restore
///////////////////////////////////////////////////////////////////////////////
Task("Restore")
  .Does(() =>
{
	var projects = GetFiles("./**/*.csproj");

	foreach(var project in projects)
	{
	    DotNetCoreRestore(project.GetDirectory().FullPath);
    }
});

///////////////////////////////////////////////////////////////////////////////
// Version
///////////////////////////////////////////////////////////////////////////////
Task("Version")
  .WithCriteria(() => isAppVeyor && BuildSystem.AppVeyor.Environment.Repository.Tag.IsTag)
  .Does(() => 
{
    version = BuildSystem.AppVeyor.Environment.Repository.Tag.Name;
});

///////////////////////////////////////////////////////////////////////////////
// Build
///////////////////////////////////////////////////////////////////////////////
Task("Build Yaapii")
  .IsDependentOn("Clean")
  .IsDependentOn("Restore")
  .IsDependentOn("Version")
  .Does(() =>
{
	Information("__   __                _ _ ");
	Information("\\ \\ / /_ _  __ _ _ __ (_|_)");
	Information(" \\ V / _` |/ _` | '_ \\| | |");
    Information("  | | (_| | (_| | |_) | | |");
    Information("  |_|\\__,_|\\__,_| .__/|_|_|");
	Information("                |_|");

	DotNetCoreBuild(
        ypXml,
        new DotNetCoreBuildSettings()
        {
            Configuration = configuration,
            Framework = netstandard,
            MSBuildSettings = new DotNetCoreMSBuildSettings().SetVersionPrefix(version)
        }
    );

});

///////////////////////////////////////////////////////////////////////////////
// Code Coverage
///////////////////////////////////////////////////////////////////////////////
Task("Generate-Coverage")
.IsDependentOn("Build Yaapii")
.WithCriteria(() => isAppVeyor || coverageReport)
.Does(() => 
{
	try
	{
		var projectsToCover = new [] {
                ypXmlTests
		};

        var dotNetCoreTestSettings =
            new DotNetCoreTestSettings
            {
                Configuration = "Debug"
            };

        // remove coverage.xml if it exists.
        if (FileExists("./coverage.xml"))
        {
            DeleteFile("./coverage.xml");
        }
       
        
        foreach(var proj in projectsToCover)
        {
           OpenCover(tool => 
			{ 
				tool.DotNetCoreTest(proj, dotNetCoreTestSettings);
			
			},
			new FilePath("./coverage.xml"),
			new OpenCoverSettings(){ OldStyle = true, MergeOutput = true }
				.WithFilter("+[*]*")
                .WithFilter("-[Test.*]*")
                .WithFilter("-[*]*.Fk*") 
                .WithFilter("-[*]*.Error.*") 
		    );
        }	
	}
	catch(Exception ex)
	{
		Information("Error: " + ex.ToString());
	}
});


Task("Generate-Coverage-Report")
.IsDependentOn("Generate-Coverage")
.WithCriteria(() => isAppVeyor || coverageReport)
.Does(() =>
{
    ReportGenerator("./coverage.xml","./artifacts/coverage-report");
});


Task("Upload-Coverage")
.IsDependentOn("Generate-Coverage")
.IsDependentOn("GetAuth")
.WithCriteria(() => isAppVeyor)
.Does(() =>
{
    Codecov("coverage.xml", codecovToken);
});

///////////////////////////////////////////////////////////////////////////////
// Test
///////////////////////////////////////////////////////////////////////////////

Task("Test Yaapii")
  .IsDependentOn("Build Yaapii")
  .Does(() => 
{
	DotNetCoreTest(
		ypXmlTests,
		new DotNetCoreTestSettings()
		{
			Configuration = configuration,
			Framework = netcore
		}
	);

});

///////////////////////////////////////////////////////////////////////////////
// Pack
///////////////////////////////////////////////////////////////////////////////
Task("Pack")
    .IsDependentOn("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
{
	
	var settings = new DotNetCorePackSettings()
    {
        Configuration = configuration,
        OutputDirectory = buildArtifacts,
	  	VersionSuffix = ""
    };
   
	settings.MSBuildSettings = new DotNetCoreMSBuildSettings().SetVersionPrefix(version);
	settings.ArgumentCustomization = args => args.Append("--include-symbols");

   if (isAppVeyor)
   {

       var tag = BuildSystem.AppVeyor.Environment.Repository.Tag;
       if(!tag.IsTag) 
       {
			settings.VersionSuffix = "build" + AppVeyor.Environment.Build.Number.ToString().PadLeft(5,'0');
       } 
	   else 
	   {     
			settings.MSBuildSettings = new DotNetCoreMSBuildSettings().SetVersionPrefix(tag.Name);
       }
   }

	
	DotNetCorePack(
		ypXml.ToString(),
		settings
    );
});

///////////////////////////////////////////////////////////////////////////////
// Release
///////////////////////////////////////////////////////////////////////////////
Task("GetAuth")
	.WithCriteria(() => isAppVeyor)
    .Does(() =>
{
    githubToken = EnvironmentVariable("GITHUB_TOKEN");
	codecovToken = EnvironmentVariable("CODECOV_TOKEN");
});

Task("Release")
  .WithCriteria(() => isAppVeyor && BuildSystem.AppVeyor.Environment.Repository.Tag.IsTag)
  .IsDependentOn("Version")
  .IsDependentOn("Pack")
  .IsDependentOn("GetAuth")
  .Does(() => {
     GitReleaseManagerCreate(githubToken, owner, repository, new GitReleaseManagerCreateSettings {
        Milestone         = version,
        Name              = version,
        Prerelease        = false,
        TargetCommitish   = "master"
    });

	var nugetFiles = string.Join(", ", GetFiles("./artifacts/**/*.nupkg").Select(f => f.FullPath) );
	Information("Nuget artifacts: " + nugetFiles);

	GitReleaseManagerAddAssets(
		githubToken,
		owner,
		repository,
		version,
		nugetFiles
	);
});

Task("Default")
  .IsDependentOn("Build Yaapii")
  .IsDependentOn("Test Yaapii")
  .IsDependentOn("Generate-Coverage")
  .IsDependentOn("Generate-Coverage-Report")
  //.IsDependentOn("Upload-Coverage") codecov cannot be used, we have a private repo.
  .IsDependentOn("Pack")
  .IsDependentOn("Release");

RunTarget(target);
