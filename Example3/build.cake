#tool "nuget:?package=xunit.runner.console"
#tool "nuget:?package=OctopusTools"
#tool "nuget:?package=squirrel.windows&version=1.6.0" 
#addin Cake.Squirrel
#addin "nuget:?package=Cake.FileHelpers&version=1.0.4"
#addin Cake.FluentMigrator
#addin "MagicChunks"

//////////////////////////////////////////////////////////////////////
// ARGUMENTS SPECIFICATION
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

var octoServer = EnvironmentVariable("OCTOPUS_SERVER_URL");
var octoApiKey = EnvironmentVariable("OCTOPUS_API_KEY");
var packagesBuildCounter = EnvironmentVariable("PACKAGES_BUILD_COUNTER");
var buildCounter = EnvironmentVariable("BUILD_COUNTER");
var branchName = EnvironmentVariable("BRANCH_NAME");

var isRunningOnTeamCity = TeamCity.IsRunningOnTeamCity;
var isDefaultBranch = false;
bool.TryParse(EnvironmentVariable("IS_DEFAULT_BRANCH"), out isDefaultBranch);
var teamCityProjectName = TeamCity.Environment.Build.BuildConfName;

var exchangeConnectionString = EnvironmentVariable("CONNECTION_MIGRATION");

//phases
Information(teamCityProjectName);
if(string.IsNullOrWhiteSpace(teamCityProjectName) == false && target == "Default"){	
    switch (teamCityProjectName)
    {
		case "BuildTestPackPublish":
			target = "BuildTestPackPublish";
			Information("use target: BuildTestPackPublish");
            break;		
        case "DeployToQa":
        	target = "CreateQaRelease";
			Information("use target: CreateQaRelease");
            break;		
	}
}

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

var solutionFile = "./ExchangeManagement.sln";

var nugetPackageDir = Directory("./package");
var octopusAtrifactsDirectory = Directory("./octoPackages");
var squirellAtrifactsDirectory = Directory("./squirellPackages");
var buildedPackagesPatter ="./package/*.nupkg";

var nuspecFiles = GetFiles("./**/*.nuspec") - GetFiles("./**/.squirell/*.nuspec");

var assemblyInfoVersion = ParseAssemblyInfo("./src/SolutionItems/SharedAssemblyInfo.cs").AssemblyVersion;

var versionToUse = string.Join(".", assemblyInfoVersion.Split('.').Take(2))+"."+(isRunningOnTeamCity ? packagesBuildCounter: "0");
var releaseNumber = string.Join(".", assemblyInfoVersion.Split('.').Take(2))+"."+(isRunningOnTeamCity ? buildCounter +"-"+ branchName.Replace('/', '-') : "0");

var octoIdPrefix = "gis.exchange.";

var octoArtifacts = new Dictionary<string,ArtifactConfiguration>
{
	{ "CheckProductReadyService", new ArtifactConfiguration(){ Directory = Directory("src/Applications/Services/CheckProductReadyService")} } ,	
	{ "NotifyRecipientsOfFinishedProductService", new ArtifactConfiguration(){ Directory = Directory("src/Applications/Services/NotifyRecipientsOfFinishedProductService")} } ,	
	{ "SubscriptionEditor", new ArtifactConfiguration(){ Directory = Directory("src/Applications/Windows/SubscriptionEditor"), IsDesktop = true } } ,	
	{ "api", new ArtifactConfiguration(){ Directory = Directory("src/Server/Host/ExchangeManagement.Host.WebApi"), IsWeb = true} } ,	
	{ "Migrations", new ArtifactConfiguration(){ Directory = Directory("src/Server/Migrations/ExchangeManagement.Migrations"), IsMigration = true} } ,	
	{ "SystemIndexPage", new ArtifactConfiguration(){ Directory = Directory("src/SolutionItems/SystemIndexPage"),IsStatic=true }} ,
};

public class ArtifactConfiguration{
	public ConvertableDirectoryPath Directory {get;set;}
	public bool IsDesktop {get;set;}
	public bool IsWeb {get;set;}
	public bool IsMigration {get;set;}
	public bool IsStatic {get;set;}
}


var nuGetPackSettings = new NuGetPackSettings
{
	Properties = new Dictionary<string,string>(){{"Configuration",configuration}},
	Verbosity = NuGetVerbosity.Detailed,	
	OutputDirectory = nugetPackageDir,			
};

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("RestoreNuGetPackages")    
    .Does(() =>
{
    NuGetRestore(solutionFile);
});

Task("Build")
    .IsDependentOn("RestoreNuGetPackages")
    .Does(() =>
{
	MSBuild(solutionFile,settings =>settings.SetConfiguration(configuration));    
});

Task("RunUnitTests")
    .IsDependentOn("Build")
    .Does(() => 
	{			
		foreach(var unitTestProject in GetFiles("./**/bin/"+configuration+"/*UnitTests*.dll")){
			XUnit2(unitTestProject.FullPath);
		}		
	});

Task("TransformConfigForIntegrationTests")
	.IsDependentOn("Build")	
	.WithCriteria(isRunningOnTeamCity)
    .Does(() => 
	{
		TransformConfig(@"src/Tests/ExchangeManagement.Host.WebApi.IntegrationTests/bin/"+configuration+"/app.config",
				new TransformationCollection {
					{ "configuration/connectionStrings/add[@name='SubscriptionConnectionString']/@connectionString", exchangeConnectionString }
				});	

		var migrationsDirectory = Directory(""+octoArtifacts["Migrations"].Directory+"/bin/"+configuration);
		var configFile = migrationsDirectory + File("App.config");
		TransformConfig(configFile,
          new TransformationCollection {
            { "configuration/connectionStrings/add[@name='SubscriptionConnectionString']/@connectionString", exchangeConnectionString },
        });		
	});

Task("RunMigration")
    .IsDependentOn("TransformConfigForIntegrationTests")	
    .Does(() => 
	{	
		var migrationsDirectory = Directory(""+octoArtifacts["Migrations"].Directory+"/bin/"+configuration);
		var configFile = migrationsDirectory + File("App.config");
		var assemblyFile = migrationsDirectory + File("ExchangeManagement.Migrations.dll");

		FluentMigrator(new FluentMigratorSettings{
    		ConnectionStringConfigPath = configFile,
			Connection = "SubscriptionConnectionString",
    		Provider= "sqlserver",
    		Assembly = assemblyFile	
		});

	});

Task("RunIntegrationTests")
	.IsDependentOn("RunMigration")	
    .Does(() => 
	{
		foreach(var unitTestProject in GetFiles("./**/bin/"+configuration+"/*IntegrationTests*.dll")){			
			XUnit2(unitTestProject.FullPath);
		}			
	});


Task("PackOctoArtifacts")	
	.IsDependentOn("Build")	
	.Does(() =>
	{
		CleanDirectory(octopusAtrifactsDirectory);
		foreach(var config in octoArtifacts){	
			var projDir = MakeAbsolute(config.Value.Directory);

			var pathToPack = ""+projDir+"/bin/"+configuration;

			if(config.Value.IsStatic)
			{
				pathToPack = ""+projDir;
			}

			Information("Path to pack: "+ pathToPack);

			if(config.Value.IsDesktop){

				Information("Copy squirell");
				var sqDir = pathToPack+"/tools/squirell";
				CreateDirectory(sqDir);
				CopyFiles("./tools/squirrel.windows/tools/*.*",sqDir);
				
				Information("Copy nuget");
				var nugetDir = pathToPack+"/tools/NuGet.CommandLine";
				CreateDirectory(nugetDir);
				CopyFiles("./tools/nuget.exe",nugetDir);

				//rename nuspec file to avoid limitation of nuget
				var squirellNuspecFile = pathToPack+"/.squirell/"+config.Key;
				CopyFile(squirellNuspecFile+".nuspec",squirellNuspecFile+".cepsun");

				//change version of index.htm
				ReplaceTextInFiles(pathToPack+"/.squirell/index.htm","{VERSION}",versionToUse);
			}

			if(config.Value.IsMigration){
				Information("Copy migration tool");				
				var migratorDir = pathToPack+"/tools/FluentMigratorTools";
				CreateDirectory(migratorDir);
				CopyDirectory(Directory("./tools/Addins/FluentMigrator.Tools/tools/AnyCPU/40/"),migratorDir);
			}

			if(config.Value.IsWeb){
				//if web project make publish
				MSBuild(projDir.GetFilePath(projDir.GetDirectoryName()+".csproj"), settings =>
            		settings.SetPlatformTarget(PlatformTarget.MSIL)                    	
                    	.WithProperty("DeployOnBuild","true")
						.WithProperty("PackageAsSingleFile","False")
						.WithProperty("AutoParameterizationWebConfigConnectionStrings","False")
						.WithProperty("_PackageTempDir",pathToPack)
                    	.WithTarget("Build")						
                    	.SetConfiguration(configuration));
			}

			var packSettings =  new OctopusPackSettings {
    			BasePath = pathToPack,
    			OutFolder = octopusAtrifactsDirectory,
    			Overwrite = false,
				Version = versionToUse
			};
			OctoPack(octoIdPrefix+config.Key,packSettings);
		}
	});

Task("NuGetPack")
	.IsDependentOn("Build")	
	.WithCriteria(isDefaultBranch)
	.Does(() => {
		CleanDirectory(nugetPackageDir);
		foreach(var nuspecFile in nuspecFiles){
			//find csprojFile to map
			var dir = nuspecFile.GetDirectory();
			var name = nuspecFile.GetFilenameWithoutExtension();
			var ext = ".csproj";			
			var csprojFile =""+ dir+"/"+name+ext;
			
			NuGetPack(csprojFile, nuGetPackSettings);
		}		
	});	

Task("PublishOctoArtifacts")
	.WithCriteria(() => isRunningOnTeamCity)
	.IsDependentOn("PackOctoArtifacts")	
	.IsDependentOn("NuGetPack")
	.Does(() => 
	{
		foreach(var octoPackage in GetFiles(""+octopusAtrifactsDirectory+"/*nupkg")){
			Information(octoPackage.ToString());

			var pushSettings = new OctopusPushSettings (){
				ReplaceExisting = false
			};

			OctoPush(octoServer,octoApiKey,octoPackage,pushSettings);
		}				
	});

Task("BuildTestPackPublish")
	.IsDependentOn("RunUnitTests")
	.IsDependentOn("RunIntegrationTests")
	.IsDependentOn("NuGetPack")
	.IsDependentOn("PublishOctoArtifacts");

Task("SetBuildNumber")
	.WithCriteria(() => isRunningOnTeamCity)
	.Does(() => 
	{
		TeamCity.SetBuildNumber(releaseNumber);					
	});	

Task("CreateQaRelease")
	.IsDependentOn("SetBuildNumber")
	.WithCriteria(() => isRunningOnTeamCity)		
	.Does(() => 
	{
		var octoCreateReleaseSettings = new CreateReleaseSettings {
			Server = octoServer,
			ApiKey = octoApiKey,
			ReleaseNumber = releaseNumber,
			DeployTo = "QA",
			Force = true,
			DefaultPackageVersion = versionToUse,
			ShowProgress = true
		};

		OctoCreateRelease("exchange",octoCreateReleaseSettings);		
	});

	
//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////
Task("Default")
    .IsDependentOn("RunUnitTests");

//////////////////////////////////////////////////////////////////////
// EXECUTION DEFAULT
//////////////////////////////////////////////////////////////////////

RunTarget(target);
