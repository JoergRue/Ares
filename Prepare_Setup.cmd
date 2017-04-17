set ant="D:\eclipse_mars\plugins\org.apache.ant_1.9.4.v201504302020\bin\ant"
set nant="D:\nant-0.91-alpha1\bin\NAnt.exe"
rem set installjammer="C:\Program Files (x86)\InstallJammer\installjammer.exe"
set svn="C:\Program Files (x86)\VisualSVN Server\bin\svn.exe"
set zip="C:\Program Files (x86)\7-Zip\7z.exe"
set JAVA_HOME=""C:\Program Files (x86)\Java\jdk1.7.0_71""
set ARES_BASE_PATH=D:\Projekte\Ares\

call %ant% -DProductVersion=%1

cd Ares.Controller\Ares.Controllers\ant
call %ant% -DProductVersion=%1 clean jar
cd ..\..\Ares.Controller\ant
call %ant% -DProductVersion=%1 prepareSetup
cd ..\..\..

%nant% clean prepareSetup
