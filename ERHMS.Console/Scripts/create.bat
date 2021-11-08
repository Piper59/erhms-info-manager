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
    set database_provider=%~1
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
call :create_project Blank || exit /b 1
call :create_page BlankPage || exit /b 1
call :create_view BlankFormWithoutWorkerInfo || exit /b 1
call :create_view BlankFormWithWorkerInfo || exit /b 1
call :create_project Workers || exit /b 1
call :create_view Worker\WorkerRosteringForm || exit /b 1
call :create_view Worker\PreDeploymentHealthSurvey || exit /b 1
call :create_project Incident || exit /b 1
call :create_view Incident\WorkerDeploymentRecord || exit /b 1
call :create_view Incident\WorkerInProcessingForm || exit /b 1
call :create_view Incident\WorkerActivityReport || exit /b 1
call :create_view Incident\DeploymentHealthSurvey || exit /b 1
call :create_view Incident\WorkerOutProcessingForm || exit /b 1
call :create_view Incident\PostDeploymentHealthSurvey || exit /b 1
call :create_view Incident\AfterActionReview || exit /b 1
goto :eof

:create_project
set project_location=%CD%\Projects\%~1
set project_name=%~n1
set project_path=%project_location%\%project_name%.prj
if "%database_provider%" == "Access2003" (
    set connection_string=Provider=Microsoft.Jet.OLEDB.4.0;Data Source=""%project_location%\%project_name%.mdb""
) else if "%database_provider%" == "Access2007" (
    set connection_string=Provider=Microsoft.ACE.OLEDB.12.0;Data Source=""%project_location%\%project_name%.accdb""
) else if "%database_provider%" == "SqlServer" (
    set connection_string=Data Source=^(localdb^)\MSSQLLocalDB;Initial Catalog=%project_name%;Integrated Security=True
) else (
    call :usage >&2
    exit /b 1
)
ERHMS.Console CreateDatabase "%database_provider%" "%connection_string%" || exit /b 1
ERHMS.Console CreateProject "%database_provider%" "%connection_string%" "%project_location%" "%project_name%" || exit /b 1
ERHMS.Console InitializeProject "%project_path%" || exit /b 1
goto :eof

:create_page
set template_path=Templates\Pages\%~1.xml
set page_name=%~n1
set view_name=%page_name%
ERHMS.Console CreateView "%project_path%" "%view_name%"
ERHMS.Console InstantiateTemplate "%template_path%" "%project_path%" "%view_name%" || exit /b 1
goto :eof

:create_view
set template_path=Templates\Forms\%~1.xml
set view_name=%~n1
ERHMS.Console InstantiateTemplate "%template_path%" "%project_path%" || exit /b 1
goto :eof
