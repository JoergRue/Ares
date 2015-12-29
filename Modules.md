# Ares Modules

Ares consists of several modules to

* enable reuse for several applications or plugins
* separate concerns clearly
* provide structure 

Those modules and their purpose are described here for a quick reference and overview over the Ares architecture.

## .NET modules (assemblies) for Editor and Players

### Ares.Data

Contains the data model for the Editor and Player. Persistency of the model is in a simple XML format; there are some helper functions to read and write it.

### Ares.Settings

Contains the settings for the Editor and Player (what can be changed in the settings dialog). Of particular importance are the music and sound directories. The 'basic' settings store where to find the other settings. The settings are also stored in a simple XML file.

### Ares.Tags

Contains the code to deal the music tags database, which is a [SQLite](https://www.sqlite.org/) database file located in the music root directory; the schema can be seen in the file Tags.uxf which can be viewed e.g. with [UMLet](http://www.umlet.com/). The module is also used by the online tags database since much of the logic can be shared.

### Ares.CommonGUI

Contains GUI used both by the Editor and the Player: in particular the settings dialog and the about dialog, but also some utilities.

### Ares.Ipc

Contains logic to share data (especially settings) between open instances of Editor and Player using shared memory.

### Ares.ModelChecks

Contains checks which test the model for errors which are shown in the Editor error window or in the Player messages.

### Ares.Online

Contains code to communicate with a web server (sourceforge) in particular for the update mechanism (new versions) and the news.

### Ares.WinSecurity

Contains a small executable to change Windows security settings (e.g. firewall) for the web server. This must be a separate executable because it needs elevated rights. It receives the input through command-line parameters and writes output (if there are errors) to a temporary file.

### Ares.Playing

Contains all the logic to actually play the elements in the projects. The low-level playing of the files and applying effects to the individual files is made using the [Un4Seen Bass](http://www.un4seen.com/) library wrapped by [Bass .Net](http://bass.radio42.com/). This module is also used in the Editor so that the user can test the elements there.

## .NET modules (assemblies) for Players

### Ares.Players

Contains common code for the players, in particular the communication with the Controller and the web server for using a browser to control the player. The web server is implemented using [ServiceStack](https://github.com/ServiceStack/ServiceStack). Communication with the specialized Controllers is made through a simple custom protocol based on TCP.

### Ares.Player

Contains the Player GUI, a Windows Forms program. 

### Ares.CmdLinePlayer

Contains the command line player executable. Based on Ares.Players. Handles input (mainly command line parameters) and output (to the console).

## .NET modules (assemblies) for the Editor

### Ares.TagsImport

Contains the code for the Editor to communicate with various online services to import tags. Tags can be imported from [MusicBrainz](https://musicbrainz.org/) and from [RPGMusicTags](http://rpgmusictags.org/). In both cases, the music is identified using the [Chromaprint](https://acoustid.org/chromaprint) fingerprinting algorithm.

### Ares.Editor.Actions

Contains most of the logic of the Editor in form of action classes which follow the command pattern. Using this pattern allows a simple implementation of undo and redo functionality.

### Ares.Editor.Controls

Contains Windows Forms controls to edit the different elements of a Ares project. The actual editors are composed of these controls.

### Ares.Editor

Contains the Editor executable, including the main GUI, File and Project Explorer windows, editor windows for the project elements and several dialogs. The main GUI uses the [DockPanel Suite](http://dockpanelsuite.com/) for the window management.

## .NET modules (assemblies) for Controllers

### Ares.Controllers

Contains common code for .NET based Controllers. Mainly handles the communication with the Player and the current state of the Controller.

### Ares.MGPlugin

Contains the code for the MeisterGeister plugin, which is mainly a controller GUI inside a Windows Forms UserControl.

### Ares.Plugin

Contains the code for the MediaPortal plugin. This is in effect a simple GUI which acts as a proxy for the Controllers: it starts a command line player in the background, receives commands through one TCP socket and puts them through to another TCP socket.

## Java modules (.jar files) for Controllers

These modules are located in the subdirectory Ares.Controller.

### Ares.Controllers

Contains common code for the controllers. Mainly this concerns the communication with the Player and the state of the Controller.

### Ares.Controller

Contains the controller GUI, which is based on Swing.

### Android

Contains the Android Controller app.

## .NET modules (assemblies) for the online tags database

### Ares.GlobalDB.Services

Contains the services for the web server of the online tags database. [ServiceStack](https://github.com/ServiceStack/ServiceStack) is used to implement the web server.

### Ares.GlobalDB.Web

An ASP.NET project to host the web server in IIS.

### Ares.GlobalDB.StandaloneHost

An executable to host the web server locally for tests.
