set ant="D:\eclipse_mars\plugins\org.apache.ant_1.9.4.v201504302020\bin\ant"
set nant="D:\nant-0.91-alpha1\bin\NAnt.exe"
set innosetup="C:\Program Files (x86)\Inno Setup 5\ISCC.exe"
set git="C:\Program Files (x86)\Git\bin\git.exe"
set zip="C:\Program Files\7-Zip\7z.exe"
set signtool=$qC:\Program Files (x86)\Windows Kits\8.0\bin\x86\$q
set JAVA_HOME=""C:\Program Files (x86)\Java\jdk1.6.0_20""
set ARES_BASE_PATH=D:\Projekte\Ares\

call %ant% -DProductVersion=%1

cd Ares.Controller\Ares.Controllers\ant
call %ant% -DProductVersion=%1 clean jar
cd ..\..\Ares.Controller\ant
call %ant% -DProductVersion=%1 prepareSetup
cd ..\..\..

%nant% clean prepareSetup

%innosetup% /Qp /Ssignscript="%ARES_BASE_PATH%\CodeSigning\sign.cmd %signtool% $f" Setup\Ares_Setup.iss

cd build\MGPlugin
%zip% a ..\output\Ares-%1-MGPlugin.zip Ares.MeisterGeisterPlugin.dll de
cd ..
rmdir /S/Q MGPlugin
cd ..

%git% archive --format zip --output build\output\Ares_%1_Source.zip HEAD 

cd build

xcopy /S /E /Q /I Ares Ares_Portable
xcopy /S /E /Q /Y ..\Ares_Portable Ares_Portable
del /Q Ares_Portable\Music\.gitignore
del /Q Ares_Portable\Sounds\.gitignore
rmdir /S /Q Ares_Portable\Player
rmdir /S /Q Ares_Portable\Player64
%zip% a output\Ares_Portable_%1.zip Ares_Portable

xcopy /S /E /Q /I Ares Ares_Portable_Linux
xcopy /S /E /Q /Y ..\Ares_Portable Ares_Portable_Linux
del /Q Ares_Portable\Music\.gitignore
del /Q Ares_Portable\Sounds\.gitignore
rmdir /S /Q Ares_Portable_Linux\Player_Editor
rmdir /S /Q Ares_Portable_Linux\Player64
%zip% a output\Ares_Portable_Linux_x86_%1.zip Ares_Portable_Linux

xcopy /S /E /Q /I Ares Ares_Portable_Linux_x64
xcopy /S /E /Q /Y ..\Ares_Portable Ares_Portable_Linux_x64
del /Q Ares_Portable\Music\.gitignore
del /Q Ares_Portable\Sounds\.gitignore
rmdir /S /Q Ares_Portable_Linux_x64\Player_Editor
rmdir /S /Q Ares_Portable_Linux_x64\Player
move /Y Ares_Portable_Linux_x64\Player64 Ares_Portable_Linux_x64\Player
%zip% a output\Ares_Portable_Linux_x64_%1.zip Ares_Portable_Linux_x64

xcopy /S /E /Q /I Ares Ares_Portable_Linux_arm_softfp
xcopy /S /E /Q /Y ..\Ares_Portable Ares_Portable_Linux_arm_softfp
del /Q Ares_Portable\Music\.gitignore
del /Q Ares_Portable\Sounds\.gitignore
rmdir /S /Q Ares_Portable_Linux_arm_softfp\Player_Editor
rmdir /S /Q Ares_Portable_Linux_arm_softfp\Player64
xcopy /S /E /Q /I /Y ..\Libraries_Linux_arm_softfp Ares_Portable_Linux_arm_softfp\Player
del Ares_Portable_Linux_arm_softfp\Player\libbass_aac.so
%zip% a output\Ares_Portable_Linux_arm_softfp_%1.zip Ares_Portable_Linux_arm_softfp

xcopy /S /E /Q /I Ares Ares_Portable_Linux_arm_hardfp
xcopy /S /E /Q /Y ..\Ares_Portable Ares_Portable_Linux_arm_hardfp
del /Q Ares_Portable\Music\.gitignore
del /Q Ares_Portable\Sounds\.gitignore
rmdir /S /Q Ares_Portable_Linux_arm_hardfp\Player_Editor
rmdir /S /Q Ares_Portable_Linux_arm_hardfp\Player64
xcopy /S /E /Q /I /Y ..\Libraries_Linux_arm_hardfp Ares_Portable_Linux_arm_hardfp\Player
del Ares_Portable_Linux_arm_hardfp\Player\libbass_aac.so
%zip% a output\Ares_Portable_Linux_arm_hardfp_%1.zip Ares_Portable_Linux_arm_hardfp

cd ..

:end
