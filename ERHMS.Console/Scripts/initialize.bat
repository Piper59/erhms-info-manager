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
echo.
echo Database providers:
echo   Access2003
echo   Access2007
echo   SqlServer
goto :eof

:main
call :initialize_project Blank || exit /b 1
call :initialize_project Worker || exit /b 1
call :initialize_project Incident || exit /b 1
call :initialize_view Blank BlankFormWithWorkerInfo || exit /b 1
call :initialize_view Blank BlankFormWithoutWorkerInfo || exit /b 1
call :initialize_page Blank BlankPage || exit /b 1
call :initialize_view Worker WorkerRosteringForm || exit /b 1
call :initialize_view Worker PreDeploymentHealthSurvey || exit /b 1
call :initialize_view Incident WorkerDeploymentRecord || exit /b 1
call :initialize_view Incident WorkerInProcessingForm || exit /b 1
call :initialize_view Incident WorkerActivityReport || exit /b 1
call :initialize_view Incident DeploymentHealthSurvey || exit /b 1
call :initialize_view Incident WorkerOutProcessingForm || exit /b 1
call :initialize_view Incident PostDeploymentHealthSurvey || exit /b 1
call :initialize_view Incident AfterActionReview || exit /b 1
goto :eof

:initialize_project
set project_name=%~1
if "%database_provider%" == "Access2003" (
    set connection_string=Provider=Microsoft.Jet.OLEDB.4.0;Data Source=""%CD%\Projects\%project_name%\%project_name%.mdb""
) else if "%database_provider%" == "Access2007" (
    set connection_string=Provider=Microsoft.ACE.OLEDB.12.0;Data Source=""%CD%\Projects\%project_name%\%project_name%.accdb""
) else if "%database_provider%" == "SqlServer" (
    set connection_string=Data Source=^(localdb^)\MSSQLLocalDB;Initial Catalog=%project_name%;Integrated Security=True
) else (
    call :usage >&2
    exit /b 1
)
set project_location=%CD%\Projects\%project_name%
set project_path=Projects\%project_name%\%project_name%.prj
ERHMS.Console CreateDatabase "%database_provider%" "%connection_string%" || exit /b 1
ERHMS.Console CreateProject "%database_provider%" "%connection_string%" "%project_location%" "%project_name%" || exit /b 1
ERHMS.Console InitializeProject "%project_path%" || exit /b 1
goto :eof

:initialize_view
set project_name=%~1
set view_name=%~2
set project_path=Projects\%project_name%\%project_name%.prj
if "%project_name%" == "Blank" (
    set template_path=Templates\Forms\%view_name%.xml
) else (
    set template_path=Templates\Forms\%project_name%\%view_name%.xml
)
ERHMS.Console InstantiateTemplate "%template_path%" "%project_path%" "%view_name%" || exit /b 1
ERHMS.Console SynchronizeView "%project_path%" "%view_name%" || exit /b 1
goto :eof

:initialize_page
set project_name=%~1
set page_name=%~2
set view_name=%page_name%
set project_path=Projects\%project_name%\%project_name%.prj
if "%project_name%" == "Blank" (
    set template_path=Templates\Pages\%page_name%.xml
) else (
    set template_path=Templates\Pages\%project_name%\%page_name%.xml
)
ERHMS.Console CreateView "%project_path%" "%view_name%"
ERHMS.Console InstantiateTemplate "%template_path%" "%project_path%" "%view_name%" "%page_name%" || exit /b 1
ERHMS.Console SynchronizeView "%project_path%" "%view_name%" || exit /b 1
goto :eof
