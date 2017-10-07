@echo off
REM setlocal
for /F tokens^=2^,3^,5delims^=^<^>^= %%a in (Saucery2.nuspec) do (
   if "%%a" equ "version" set version=%%b
)

echo version is %version%