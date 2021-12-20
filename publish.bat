@echo off
setlocal

set arg0=%~0
if "%~1" == "/?" (
    call :usage
    goto :eof
)
if "%~1" == "" (
    call :usage >&2
    exit /b 1
) else (
    if "%~1" == "Debug" (
        set configuration=%~1
    ) else if "%configuration%" == "Release" (
        set configuration=%~1
    ) else (
        call :usage >&2
        exit /b 1
    )
)
goto :main

:usage
echo Usage: %arg0% CONFIGURATION
echo.
echo Configurations:
echo   Debug
echo   Release
goto :eof

:main
msbuild -t:CustomPublish "-p:Configuration=%configuration%"
goto :eof
