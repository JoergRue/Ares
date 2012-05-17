set ant="D:\eclipse\plugins\org.apache.ant_1.7.1.v20100518-1145\bin\ant"
set nant="D:\nant-0.91-alpha1\bin\NAnt.exe"
set installjammer="C:\Program Files (x86)\InstallJammer\installjammer.exe"
set svn="C:\Program Files (x86)\VisualSVN Server\bin\svn.exe"
set zip="C:\Program Files\7-Zip\7z.exe"
set JAVA_HOME=""C:\Program Files (x86)\Java\jdk1.6.0_20""

call %ant% -DProductVersion=%1

cd Ares.Controller\Ares.Controllers\ant
call %ant% -DProductVersion=%1 clean jar
cd ..\..\Ares.Controller\ant
call %ant% -DProductVersion=%1 prepareSetup
cd ..\..\..

%nant% clean prepareSetup
copy Libraries\oggenc2.exe build\Ares\Player_Editor

%installjammer% -DProductVersion %1 --build-dir build\temp --build-log-file build\setup.log --output-dir build\output --build-for-release --build Setup\Ares\Ares.mpi

cd MGPlugin
%zip% a ..\build\output\Ares-%1-MGPlugin.zip Ares.MeisterGeisterPlugin.dll de
cd ..
rmdir /S/Q MGPlugin

%svn% export file:///D:/Repositories/Ares/trunk build\Ares_%1_Source
cd build
%zip% a output\Ares_%1_Source.7z Ares_%1_Source

xcopy /S /E /I Ares Ares_Portable
xcopy /S /E ..\Ares_Portable Ares_Portable
del Ares_Portable\buildlog.txt
rmdir /S /Q Ares_Portable\Player
%zip% a output\Ares_Portable_%1.zip Ares_Portable

xcopy /S /E /I Ares Ares_Portable_Linux
xcopy /S /E ..\Ares_Portable Ares_Portable_Linux
del Ares_Portable_Linux\buildlog.txt
rmdir /S /Q Ares_Portable_Linux\Player_Editor
%zip% a output\Ares_Portable_Linux_%1.zip Ares_Portable_Linux

cd ..
