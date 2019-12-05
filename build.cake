///////////////////////////////////////////////////////////////////////////////
// TOOLS / ADDINS
///////////////////////////////////////////////////////////////////////////////

#tool vswhere

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var msBuildPath = GetFiles(VSWhereLatest() + "/**/MSBuild.exe").FirstOrDefault();

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var verbosity = Argument("verbosity", Verbosity.Minimal);
var version = Argument("version", "1.0.21");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

var solution = "./HN.Controls.ImageEx.sln";

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .ContinueOnError()
    .Does(() => 
{
    CleanDirectories("./src/*/bin");
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    NuGetRestore(solution);
});

Task("Version")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
    var file = "./src/SolutionInfo.cs";
    CreateAssemblyInfo(file, new AssemblyInfoSettings
    {
        Version = version,
        FileVersion = version,
        InformationalVersion = version,
        Copyright = string.Format("Copyright © h82258652 2018 - {0}", DateTime.Now.Year)
    });
});

Task("Build")
    .IsDependentOn("Version")
    .Does(() =>
{
    if(IsRunningOnWindows())
    {
        // Use MSBuild
        var settings = new MSBuildSettings
        {
            ToolPath = msBuildPath
        }
        .SetConfiguration(configuration)
        .SetVerbosity(verbosity);
        MSBuild(solution, settings);
    }
    else
    {
        // Use XBuild
        XBuild(solution, configurator =>
            configurator.SetConfiguration(configuration)
                .SetVerbosity(verbosity));
    }
});

Task("Package")
    .IsDependentOn("Build")
    .Does(() =>
{
    var nugetPackSettings = new NuGetPackSettings
    {
        Version = version,
        OutputDirectory = "./artifacts"
    };

    var nuspecFiles = GetFiles("./src/*/*.nuspec");
    NuGetPack(nuspecFiles, nugetPackSettings);
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Package");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);