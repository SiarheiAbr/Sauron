# Sauron

1) Please install Visual Studio 2017 for running this build solution and running builds using tools v15
2) After Installation add following assemblies into GAC(from Visual Studio Developer Command as Admin):

	a) gacutil /i "C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\MSBuild\15.0\Bin\Microsoft.Build.Framework.dll"

	b) gacutil /i "C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\MSBuild\15.0\Bin\Microsoft.Build.dll"

	c) gacutil /i "C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\MSBuild\15.0\Bin\Microsoft.Build.Engine.dll"

	d) gacutil /i "C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\MSBuild\15.0\Bin\Microsoft.Build.Conversion.Core.dll"

	e) gacutil /i "C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\MSBuild\15.0\Bin\Microsoft.Build.Tasks.Core.dll"

	f) gacutil /i "C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\MSBuild\15.0\Bin\Microsoft.Build.Utilities.Core.dll"

3) Don't use assemblies of v15 from "2" using Nuget Packet - there are some conflicts between versions of dlls in 
	Visual Studio and in the Nuget Packages(Visual Studio contains older version than last nuget package).

4) Configure DownloadRepositotyPathTemplate in the Sauron.Services.

5) DON'T REMOVE Microsoft.Build.Locator package from Sauron.Services. Project uses it to find correct MSBuild Path.