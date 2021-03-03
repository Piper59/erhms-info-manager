@echo off
setlocal

set arg0=%~0
if "%~1" == "/?" (
    call :usage
    goto :eof
)
set valid=1
if "%~1" == "" (
    set valid=0
) else (
    set database_provider=%~1
)
if "%valid%" == "0" (
    call :usage >&2
    exit /b 1
)
goto :main

:usage
echo Usage: %arg0% DATABASE_PROVIDER
goto :eof

:main
call :create Worker || exit /b 1
call :create Incident || exit /b 1
goto :eof

:create
set project_type=%~1
if "%database_provider%" == "Access2003" (
    set connection_string=Provider=Microsoft.Jet.OLEDB.4.0;Data Source=""%CD%\Projects\%project_type%\%project_type%.mdb""
) else if "%database_provider%" == "Access2007" (
    set connection_string=Provider=Microsoft.ACE.OLEDB.12.0;Data Source=""%CD%\Projects\%project_type%\%project_type%.accdb""
) else if "%database_provider%" == "SqlServer" (
    set connection_string=Data Source=^(localdb^)\MSSQLLocalDB;Initial Catalog=%project_type%;Integrated Security=True
) else (
    exit /b 1
)
set project_location=%CD%\Projects\%project_type%
set project_path=%CD%\Projects\%project_type%\%project_type%.prj
ERHMS.Console CreateDatabase "%database_provider%" "%connection_string%" || exit /b 1
ERHMS.Console CreateProject "%database_provider%" "%connection_string%" "%project_location%" "%project_type%" || exit /b 1
ERHMS.Console InitializeProject "%project_path%" || exit /b 1
goto :eof
