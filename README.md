# DopX
Redesign of DOP Software

[![Build status](https://ci.appveyor.com/api/projects/status/5vcpwy657jhc657o/branch/master?svg=true)](https://ci.appveyor.com/project/icarus-consulting/dop/branch/master) [![codecov](https://codecov.io/gh/icarus-consulting/dop/branch/master/graph/badge.svg?token=BvNWkVE2K1)](https://codecov.io/gh/icarus-consulting/dop)

# Get started
This software is currently in development and exists as a CLI inside Tecnomatix Process Simulate. You can install the package and try the so far included functions for yourself. 

1) Download the latest [release](https://github.com/icarus-consulting/dop/releases)
2) Install the msi package
3) Create config.xml in `C:\ProgramData\DopX\` with the following content
```xml
<?xml version="1.0" encoding="utf-8"?>
<config>
	<section name="general">
		<stylesheets>C:\\Program Files\\Tecnomatix_13.1\\eMPower\\DotNetCommands\\DopX\\stylesheets</stylesheets>
		<transientPath>C:\\Program Files\\Tecnomatix_13.1\\eMPower\\DotNetCommands\\DopX\\temp</transientPath>
		<assets>C:\\Program Files\\Tecnomatix_13.1\\eMPower\\DotNetCommands\\DopX\\assets</assets>
	</section>
	<section name="vendors">
		<section name="kuka-krc">
			<extension>.src</extension>
			<extension>.dat</extension>
			<primary>.src</primary>
		</section>
		<section name="kuka-vkrc">
			<extension>.src</extension>
			<extension>.dat</extension>
			<primary>.src</primary>
		</section>
		<section name="abb-rapid">
			<extension>.mod</extension>
			<primary>.mod</primary>
		</section>
	</section>
</config>
```

4) Customize the command (ICARUS\DopX) into your ribbon
5) Start the CLI with a click on DopX button

# Usage
Type `help`in DopX CLI

# How to contribute

### Setup Development Environment
Clone the repository to your local system
```
git clone https://github.com/icarus-consulting/dop.git
```
Make sure you have the latest [NodeJS](https://nodejs.org/en/) and [Yarn](https://yarnpkg.com) installed.
Install all development dependencies from _package.json_.
```
cd dop
yarn install
```

### Running Unit Tests for Tecnomatix
1. Make sure that the default processor arichtecture in VS is set to X64 (Test -> Test Settings -> Default Processor Architecture)
2. Make sure you have Process Simulate installed.
3. Make sure your Z: drive points to the eMPower directory of your Process Simulate installation. (hard linking of dll´s is bad, use Nuget packages instead)
4. Run the install task from cake build script with `.\build.ps1 -target install`. This task will set the environmental variable `TECNOMATIX`which points to the `Tune.exe` and the environment variable `EMPOWER`which points to the eMPower directory. You can use this variables in Step 6.
5. Make sure you have installed the latest version of the Pluginrunner (https://github.com/icarus-consulting/Yaapii.TmxUnit/releases)
6. You have set the correct Application-Properties in VS in your project (usage see https://github.com/icarus-consulting/Yaapii.TmxUnit)
7. Make sure your test project has an postbuild event, which copys the output to the pluginrunner directory (xcopy /y $(OutDir)* &quot;$(EMPOWER)DotNetCommands\ICARUS\Yaapii.TmxUnit.PluginRunner\assemblies&quot;&#xD;&#xA;del &quot;$(EMPOWER)DotNetCommands\ICARUS\Yaapii.TmxUnit.PluginRunner\assemblies\Tecnomatix.Engineering*.dll&quot;) 
8. Set the Testproject you wish to run as startup project
9. Start debugging (F5 or play button)

### Code Coverage
To generate a code coverage report on your local dev machine
```ps
PS> .\build.ps1 -report=true
```
This will generate a code coverage report in `.\artifacts/coverage-report`. Open the `index.html` with your browser of choice.