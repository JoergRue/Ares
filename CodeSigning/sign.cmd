@echo off
echo called path is %~dp0
if exist %~dp0ares_code_signing_certificate.pfx goto sign
echo Certificate not found, Exe will not be signed
goto end
:sign
setlocal
call "%~dp0signing_password.cmd"
%1signtool.exe sign /f "%~dp0ares_code_signing_certificate.pfx" /p "%SIGNPASS%" /t "http://timestamp.verisign.com/scripts/timstamp.dll" %2
endlocal
:end
