///////////////////////////////////////////////////////////////////////////////
// TOOLS / ADDINS
///////////////////////////////////////////////////////////////////////////////

#tool xunit.runner.console

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var verbosity = Argument("verbosity", Verbosity.Minimal);
var version = Argument("version", "1.0.16");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

var solution = "./HN.Controls.ImageEx.sln";

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() => 
{
    CleanDirectories("./src/*/bin");
    CleanDirectories("./test/*/bin");
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
        MSBuild(solution, configurator =>
            configurator.SetConfiguration(configuration)
                .SetVerbosity(verbosity));
    }
    else
    {
        // Use XBuild
        XBuild(solution, configurator =>
            configurator.SetConfiguration(configuration)
                .SetVerbosity(verbosity));
    }
});

Task("Run-Unit-Tests")
    .IsDependentOn("Build")
    .Does(() => 
{
    XUnit2("./test/*/bin/*/*.Tests.dll");
});


Task("Package")
    .IsDependentOn("Run-Unit-Tests")
    .Does(() =>
{
    var nuGetPackSettings = new NuGetPackSettings
    {
        Version = version
    };

    var nuspecFiles = GetFiles("./src/*/*.nuspec");
    NuGetPack(nuspecFiles, nuGetPackSettings);
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