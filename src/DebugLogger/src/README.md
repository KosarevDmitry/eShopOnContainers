I have tried to add  a project as it is recommended in `\docs\ReferenceResolution.md`
but the explanation is a little bit incorrect there.

Actually
1. add project  (it is obvious) in dir  `\src\MyProject`.  Add dir `src`, as this is s common pattern so the path will end up being `\src\MyProject\sec`
1. add  in D:\src\aspnetcore\eng\Build.props
<DotNetProjects Include="
		$(RepoRoot)src\MyProject\**\src\*.csproj;
		where  `$(BuildMainlyReferenceProviders)' == 'true'`
2. run  eng/scripts/GenerateProjectList.ps1
3. add  in `eng\Dependencies.props`
in group  `<ItemGroup Label=".NET team dependencies (Non-source-build)" Condition="'$(DotNetBuildFromSource)' != 'true'">`
`<LatestPackageReference Include="MyProject" />`
4. \src\restore.cmd

5. `dotnet sln AspNetCore.sln add .\src\MyProject\src`
6. Add scripts to `\src\MyProject\` to work with new project 

build.cmd
```
@ECHO OFF
SET RepoRoot=%~dp0..\..

%RepoRoot%\eng\build.cmd -projects %~dp0**\*.*proj %*
```


MyProject.slnf 
```
{
  "solution": {
    "path": "..\\..\\AspNetCore.sln",
    "projects": [
	  "src\\DebugLogger\\src\\MyProject.csproj"
    ]
  }
}
```

startvs.cmd

```
@ECHO OFF

%~dp0..\..\startvs.cmd %~dp0MyProject.slnf
```

7. Add dependency to any csproj  <Reference Include="MyProject" /> 
Finish

debuglogger work
https://github.com/KosarevDmitry/aspnetcore/blob/0c8a569b9b5880ce54dc65c29647df0c99e775a9/src/Hosting/TestHost/test/HttpContextBuilderTests.cs#L18

But  probably add `Activitysource`  is better
 