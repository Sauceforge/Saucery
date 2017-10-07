@echo off

set _argcActual=0
set _argcExpected=1

for %%i in (%*) do set /A _argcActual+=1

if %_argcActual% NEQ %_argcExpected% (
  call :_ShowUsage %0%, "Bad human...bad args." 
  set _exitStatus=1
  goto:_EOF
)

del Saucery.*.nupkg
nuget pack Saucery.nuspec
nuget setapikey 8eac014c-8128-4aed-97c2-2e7d9371ea96
nuget push Saucery.%1.nupkg

goto:_EOF

:_ShowUsage  
  echo [USAGE]: %~1 arg1
  echo.
  if NOT "%~2" == "" (
    echo %~2
    echo.
  )
  goto:eof

:_EOF
 
echo The exit status is %_exitStatus%.