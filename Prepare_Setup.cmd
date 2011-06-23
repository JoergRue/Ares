set ant="D:\eclipse\plugins\org.apache.ant_1.7.1.v20100518-1145\bin\ant"
set nant="D:\nant-0.91-alpha1\bin\NAnt.exe"
set installjammer="C:\Program Files (x86)\InstallJammer\installjammer.exe"
set svn="C:\Program Files (x86)\VisualSVN Server\bin\svn.exe"
set zip="C:\Program Files (x86)\7-Zip\7z.exe"

call %ant% -DProductVersion=%1

cd Ares.Controller\Ares.Controllers\ant
call %ant% -DProductVersion=%1 clean jar
cd ..\..\Ares.Controller\ant
call %ant% -DProductVersion=%1 prepareSetup
cd ..\..\..

%nant% clean prepareSetup
