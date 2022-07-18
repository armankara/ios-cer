# Build Pipeline
The Build Pipeline package can be used to automate the standard unity building process through TeamCity.

Currently available building processes:
- WebGL
- Android SDK
- GooglePlay AAB
- iOS

## Getting Started
Use the standard unity package manager to install the Build Pipeline package.

Using the build pipeline within Unity:

1. Open the Package Manager: Window > Package Manager
2. Adding the package through git: + icon > Add package from git URL...
3. Enter the undermentioned link and click ADD:
```bash 
git@gitlab.azerdev.com:casual-group/unity-packages/build-pipeline-package.git
```

_**NOTE: If the cloning process is failing, check if your public SSH key is accessible and named id_rsa (since Unity can't read it otherwise)**_

Extensive explanations can be found at Unity's [Package Manager documentation](https://docs.unity3d.com/Packages/com.unity.package-manager-ui@latest/index.html).

## Usage
Building can now be done automatically or manually after importing the package.

Creating automatically signed builds without switching platforms can be done through:
1. Select your targeted platform: Azerion > select your targeted platform.



----
### On this page
- [</b> Introduction </b>](#b-introduction-b)
- [</b> Setting Up Fastlane</b>](#b-setting-up-fastlane-b)
  - [Necessary Files](#necessary-files)
    - [Fastfile](#fastfile)
    - [Appfile](#appfile)
    - [Gym & Gymfile](#gym--gymfile)
    - [Deploy](#deploy)
    - [Additional Commands](#additional-commands)
- [</b> Setting Up TeamCity</b>](#b-setting-up-teamcity-b)
    - [Buildsteps](#buildsteps)
    - [Driving your lanes](#drivinglane)

----

# </b> Introduction </b>
fastlane is the easiest way to automate beta deployments and releases for your iOS and Android apps. 🚀 It handles all tedious tasks, like generating screenshots, dealing with code signing, and releasing your application.

iOS => Please visit <a href="https://docs.fastlane.tools/getting-started/ios/setup/">here</a> and install fastlane to your project.

Android => Please visit <a href="https://docs.fastlane.tools/getting-started/android/setup/">here</a> and install fastlane to your project.

At the end, in your Unity project you have to have a fastlane folder with the necessary files in it. 
Which are:

1. [Fastfile](#fastfile)
2. [Appfile](#appfile) 
3. [Gymfile (_optional_)](#gymfile)
4. [Deploy](#deploy)
   
In these files, you will need to provide the parameters the automated build process requires.

[↑](#on-this-page)

----

# </b> Setting Up Fastlane</b>

## Necessary Files
We need some files set up in the fastlane directory of our project to get it up and running.
 
### Fastfile
The fastfile is the most important file, it manages the entireity of:

Android => Deployment process
iOS => XCode project (.xcodeproj) building and Deployment processes

The Fastfile is written in Ruby. Initially we must provide the platform we're working on. Keep in mind that the fastfile can include multiple platforms at once. Meaning, only one fastfile is enough per project even if the project supports multiple distribution platforms. 
```ruby
platform :ios do
  # Code your lanes here
end
```
or
```ruby
platform :android do
  # Code your lanes here
end
```

The Fastfile works in "lanes". Such as:
```ruby
lane :alpha do
    deploy(track: "alpha")
  end
```
These lanes are used to do seperate tasks, in our case:
* Android => "Deploy" lane (Google Play Store)
* iOS => "XCode Build" lane AND "Deploy" lane (Appstore Connect)


[↑](#on-this-page)
 
### Appfile
The Appfile will be automatically generated when Fastlane is installed to the project.
We must give necessary parameters to the Appfile for Fastlane to work properly.
These parameters are:

```ruby
json_key_file("fastlane/blabla.json") # Path to the json secret file - Follow https://docs.fastlane.tools/actions/supply/#setup to get one
package_name("blabla") # e.g. com.krausefx.app
app_identifier "blablabla" 
apple_id "blablabla" 
itc_team_name "blabla Games"
itc_team_id "123456"
```
Without these parameters, the signing process can not be done.

[↑](#on-this-page)
 
### Gym & Gymfile
Visit <a href=http://docs.fastlane.tools/actions/gym/#gym>here</a> for additional docs.

Gym is the module we want that handles the building and packaging process in XCode(.xcodeproj) for the iOS platform.
We need to prepare a Gymfile, provide necessary parameters and type "gym" into our build lane.

```csharp
output_directory("../iomyapps")
archive_path("../../Library/Developer/.../MyApp.xcarchive")
project("../Unity-iPhone.xcodeproj")
```

After creating our gymfile, as mentioned above we add "gym" into our respective lane.
```ruby
desc "Build and package the app"
  lane :build do 

    # other optional commands

    gym # Compile and package the build

  end
```
If the gymfile parameters are correct, this command will build and archive your XCode project.

[↑](#on-this-page)

### Deploy
We add the "deploy" command into our lane in order to upload the .ipa to Appstore Connect. 

An example:
```ruby
desc "Build, package and upload the app"
  lane :build do 

    # other optional commands

    gym # Compile and package the build

    deploy # Upload ipa to Appstore Connect

  end
```
Since we have all the necessary parameters, (team name, team id, apple id) "deploy" we can easily upload your archived package to Appstore Connect for you to test via Testflight.

[↑](#on-this-page)

### Additional Commands
There are plenty of <a href=https://docs.fastlane.tools/actions/>available additional commands (actions)</a> to use.
If the specific task you are looking for is not within those Actions, you are also able to <a href=https://docs.fastlane.tools/create-action/>create your own actions</a>.
The action we are showing in this example is `increment_build_number`, we use this to be able to follow our automated build numbers much more easily and without the hassle of having to manually update the version every time.

An example:
```ruby
desc "Build, package and upload the app"
  lane :build do 

    # other optional commands
    increment_build_number(
      xcodeproj: '../Unity-iPhone.xcodeproj'
    )

    gym # Compile and package the build

    deploy # Upload ipa to Appstore Connect

  end
```

[↑](#on-this-page)

# </b> Setting Up TeamCity</b>
Once Fastlane is set up locally, you will be able to upload the fastlane folder along with your other uncommited project files via any type of version control.
Make sure you have correctly <a href=https://www.jetbrains.com/help/teamcity/configuring-vcs-roots.html#Common+VCS+Root+Properties>set up</a> a commit hook on the VCS Root configuration page on TeamCity.

When building to android using a custom keystore, don't forget to include the keystore location path in the <a href=https://www.jetbrains.com/help/teamcity/artifact-dependencies.html#Configuring+Artifact+Dependencies+Using+Web+UI>TeamCity Artifact Dependencies</a>.

# Setting up parameters (optional)

Setting up parameters will improve the quality of life when testing and providing automated data to the build.
The optional parameters that will be used are:

- unity.serial: Unity license account serial code
- unity.username: Unity license account email
- unity.password: Unity license account password
- keystore.alias: Keystore alias
- keystore.location: The relative path to the keystore file
- keystore.password: The password for the keystore

We will use these parameters in the build steps.

### Buildsteps
We will help you set up the build steps properly.

1. Checking if the Unity version on the agent is activated (_optional_) <a href=https://docs.unity3d.com/Manual/CommandLineArguments.html>Activating via command line</a> or <a href=https://docs.unity3d.com/Manual/ManualActivationGuide.html>Activate manually</a>
2. Clean up your artifact output directory (_optional_)
```bash
rm -rf ./android
```
3. The build script, this one is most complicated.
I will post an example buildscript here. All optional parameters are below.
```bash
UnityLocation -projectPath /Projects/Unity/TutorialGame -buildTarget Android -executeMethod AndroidBuilder.BuildAAB -quit -batchmode -nographics -serial %unity.serial% -username %unity.username% -password %unity.password% -keystoreLocation %keystore.location% -keystoreAlias %keystore.alias% -keystorePass '%keystore.password%' -buildVersion 4000%build.number% -branchName %teamcity.build.branch%
```
<h3>Required for every build</h3>

- **UnityLocation**: This is the relative Unity executable path
```bash
/Unity/Hub/Editor/2019.3.0f1/Editor/Unity
```
- **projectPath**: this is the path to the root of the project
```bash
/Projects/Unity/TutorialGame
```
- **buildTarget**: Platform to build towards
```bash
Android, iOS, WebGL, etc
```
- **executeMethod**: Method to run when building, can be specifically called.
```bash
BuildAPK, BuildAAB, BuildiOS, etc
```
- **quit**: Quits application on finished build or on error
- **batchmode**: Keeps the build process in the background
- **nographics**: Keeps the build process in the terminal
- **serial**: Activates the license using this serial code if there is no activated license found
- **username**: Unity license account email
- **password**: Unity license account password

<h3>Android specific</h3>

- **buildVersion**: Adjust BuildVersionCode and BuildVersionNumber to this value
- **branchName**: Set to use the current branch name, will only complete the build if it's built from a release branch. Uses this branch name in the output file name.

<h4>With custom keystore (optional)</h4>

- **keystoreLocation**: Relative path to the keystore file
- **keystoreAlias**: Keystore's alias.
- **keystorePass**: Keystore's password

### Driving your lanes

In the previous steps you learned how to make a lane for yourself. In order to execute that lane on TeamCity you just run:
```ruby
bundle exec fastlane [lane]
```
It's also possible to pass parameters to lanes explanation <a href=http://docs.fastlane.tools/advanced/lanes/#lane-properties>here</a>
