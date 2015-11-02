set ant="D:\eclipse_mars\plugins\org.apache.ant_1.9.4.v201504302020\bin\ant"
set nant="D:\nant-0.91-alpha1\bin\NAnt.exe"
rem set installjammer="C:\Program Files (x86)\InstallJammer\installjammer.exe"
set innosetup="C:\Program Files (x86)\Inno Setup 5\ISCC.exe"
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

rem %installjammer% -DProductVersion %1 --build-dir build\temp --build-log-file build\setup.log --output-dir build\output --build-for-release --build Setup\Ares\Ares.mpi
%innosetup% /Qp /DMyAppVersion=ProductVersion Setup\Ares_Setup.iss

cd build\MGPlugin
%zip% a ..\output\Ares-%1-MGPlugin.zip Ares.MeisterGeisterPlugin.dll de
cd ..
rmdir /S/Q MGPlugin
cd ..

%svn% export https://localhost/svn/Ares/trunk build\Ares_%1_Source
cd build
%zip% a output\Ares_%1_Source.7z Ares_%1_Source

xcopy /S /E /Q /I Ares Ares_Portable
xcopy /S /E /Q /Y ..\Ares_Portable Ares_Portable
rmdir /S /Q Ares_Portable\Player
rmdir /S /Q Ares_Portable\Player64
%zip% a output\Ares_Portable_%1.zip Ares_Portable

xcopy /S /E /Q /I Ares Ares_Portable_Linux
xcopy /S /E /Q /Y ..\Ares_Portable Ares_Portable_Linux
rmdir /S /Q Ares_Portable_Linux\Player_Editor
rmdir /S /Q Ares_Portable_Linux\Player64
%zip% a output\Ares_Portable_Linux_x86_%1.zip Ares_Portable_Linux

xcopy /S /E /Q /I Ares Ares_Portable_Linux_x64
xcopy /S /E /Q /Y ..\Ares_Portable Ares_Portable_Linux_x64
rmdir /S /Q Ares_Portable_Linux_x64\Player_Editor
rmdir /S /Q Ares_Portable_Linux_x64\Player
move /Y Ares_Portable_Linux_x64\Player64 Ares_Portable_Linux_x64\Player
%zip% a output\Ares_Portable_Linux_x64_%1.zip Ares_Portable_Linux_x64

xcopy /S /E /Q /I Ares Ares_Portable_Linux_arm_softfp
xcopy /S /E /Q /Y ..\Ares_Portable Ares_Portable_Linux_arm_softfp
rmdir /S /Q Ares_Portable_Linux_arm_softfp\Player_Editor
rmdir /S /Q Ares_Portable_Linux_arm_softfp\Player64
xcopy /S /E /Q /I /Y ..\Libraries_Linux_arm_softfp Ares_Portable_Linux_arm_softfp\Player
del Ares_Portable_Linux_arm_softfp\Player\libbass_aac.so
%zip% a output\Ares_Portable_Linux_arm_softfp_%1.zip Ares_Portable_Linux_arm_softfp

xcopy /S /E /Q /I Ares Ares_Portable_Linux_arm_hardfp
xcopy /S /E /Q /Y ..\Ares_Portable Ares_Portable_Linux_arm_hardfp
rmdir /S /Q Ares_Portable_Linux_arm_hardfp\Player_Editor
rmdir /S /Q Ares_Portable_Linux_arm_hardfp\Player64
xcopy /S /E /Q /I /Y ..\Libraries_Linux_arm_hardfp Ares_Portable_Linux_arm_hardfp\Player
del Ares_Portable_Linux_arm_hardfp\Player\libbass_aac.so
%zip% a output\Ares_Portable_Linux_arm_hardfp_%1.zip Ares_Portable_Linux_arm_hardfp

cd ..

:end
