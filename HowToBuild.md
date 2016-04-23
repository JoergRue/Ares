# How to build Ares

Ares consists of several different applications / plugins (Editor, Player, Controller) for several different platforms (Windows, Linux, Android) and built with several different technologies (.NET, Java). Therefore, you'll need different programs to build the various applications and plugins. There are also some scripts (described below) to build most of the applications / plugins automatically in preparation for a release.

## Applications / Plugins

### Editor / Player for Windows

The Editor and the Player for Windows are built with [Microsoft Visual Studio (Community 2015)](http://www.visualstudio.com/). Load the Ares.sln file, then just select Debug or Release configuration and build the solution. The output will be in bin/Debug or bin/Release. For it to run, you'll also have to manually copy the files from the Libraries directory to the output directory.

### Player for Linux

The Player for Linux (Mono) is built with [Xamarin Studio](https://xamarin.com/platform). Load the Ares.MonoPlayer.sln file, then just select Debug or Release configuration and build the solution. The output will be in bin/Mono\_Debug or bin/Mono\_Release. For it to run on Linux, you'll also have to manually copy the files from the correct Libraries_\* directory to the output directory depending on the processor architecture.

### Controller

The Controller is built with [Eclipse (Mars)](https://eclipse.org/) and [JDK](http://www.oracle.com/technetwork/java/javase/downloads/index.html) 6 or higher. Import the existing project from Ares.Controller/Ares.Controller into your Eclipse workspace. Localization is done with an ant task (to correctly process German umlauts). Note: in the build scripts, Eclipse is not used, but ant and JDK must be available.

### Setup for Windows

The Setup program for Windows is built with [InnoSetup](http://www.jrsoftware.org/isinfo.php). If you don't have a certificate to sign the setup, you'll have to adapt the Setup script (Setup/Ares_Setup.iss) and remove the SignTool setting. If you do have a certificate, include it in the CodeSigning directory as described at the end of this document.

The Setup can only be built / tested by using the build scripts (see below).

### Controller plugin for MeisterGeister

The Controller plugin for the MeisterGeister program is built with Visual Studio. It is contained in the Ares.Controllers.sln. You'll also need to have [ILMerge](https://www.microsoft.com/en-us/download/details.aspx?id=17630) installed which is used to produce a single plugin assembly out of several modules. Note that ILMerge still needs the .NET Framework 3.5.

### Controller for Android

The Android Controller is built with [Android Studio](https://developer.android.com/sdk/index.html). The project is in the Ares.Controller/Android folder. Note: you'll not be able to create a signed application package since you don't have the passphrase to the key store. The Android Controller is not build with the build scripts.

### Player for Android

The Android Player is built with [Xamarin Studio](https://xamarin.com/platform). Load the Ares.AndroidPlayer.sln file, then just select Debug or Release configuration and build the solution. It should work with the now freely available Studio Community, though I haven't tested that yet; Xamarin previously gave complementary licenses to open source developers (a big thank you for that!). Note: you'll not be able to create a signed application package which fits to the existing Google play entry since you don't have the passphrase to the key store. The Android Player is not build with the build scripts.

### Plugin for MediaPortal

The [MediaPortal](http://www.team-mediaportal.com/) plugin is built with Visual Studio. It is contained in the Ares_MediaPortal.sln. You'll also need the MediaPortal Extension Maker program to create the extension setup file. The MediaPortal plugin is not built with the build scripts.

### Online Tags Database

The online tags database (reachable at http://www.rpgmusictags.org/) is also partly based on the Ares sources. It is built with Visual Studio and contained in the Ares.GlobalDB.sln. The database logic is not built with the build scripts.

## Build Scripts

### Build\_All.cmd

The Build\_All.cmd builds most of the applications in preparations for a release. For it to work, you need to have several build tools installed (see also above). Adapt the paths to those tools in the top part of the build script. Additionally, you'll need some more build tools:

- [Nant](http://sourceforge.net/projects/nant/). Note: Under Windows 10, you need to install the Windows SDK for Windows __8__ (in particular, the .NET Framework 4.5 SDK, strange as that sounds) for Nant to work correctly.
- [launch4j](http://launch4j.sourceforge.net/). You may need to adapt the path to the tool in Ares.Controller/Ares.Controller/ant/build.xml.
- [Git for Windows](https://desktop.github.com/)

The script needs as parameter the version of the release (three version numbers) and will adapt file versions if necessary. If everything works, it will generate a Windows setup program, several zip files for Ares Portable (one per OS / architecture) and a zip of the sources in the directory build/output.

### Prepare\_Setup.cmd

The Prepare\_Setup.cmd is similar to the Build\_All.cmd except that it doesn't create the zip files and the setup. I.e., it already builds everything in preparation for the setup. Then the setup script can be edited and manually tested.

## Signing

The Ares.WinSecurity.exe and the setup can be signed with a digital certificate so that Windows presents the known origin in the dialog asking for elevated rights. If you don't have a certificate, the Ares.WinSecurity project should build without problems, but the setup script must be adapted (see above). If you do have a certificate, copy it into the CodeSigning directory with the name ares\_code\_signing\_certificate.pfx. Also, create a script CodeSigning\\signing\_password.cmd which just temporarily puts the password for the certificate into an environment variable called SIGNPASS:
```
@echo off
set SIGNPASS=MyPassword
```
