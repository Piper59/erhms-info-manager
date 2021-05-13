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
    set templates_dir=%~1
)
if "%valid%" == "0" (
    call :usage >&2
    exit /b 1
)
goto :main

:usage
echo Usage: %arg0% TEMPLATES_DIR
goto :eof

:main
call :copy Worker WorkerRosteringForm || exit /b 1
call :copy Worker PreDeploymentHealthSurvey || exit /b 1
call :copy Incident WorkerDeploymentRecord || exit /b 1
call :copy Incident WorkerInProcessingForm || exit /b 1
call :copy Incident WorkerActivityReport || exit /b 1
call :copy Incident DeploymentHealthSurvey || exit /b 1
call :copy Incident WorkerOutProcessingForm || exit /b 1
call :copy Incident PostDeploymentHealthSurvey || exit /b 1
call :copy Incident AfterActionReview || exit /b 1
goto :eof

:copy
set project_type=%~1
set view_name=%~2
set relative_path=Forms\%project_type%\%view_name%.xml
set source_path=Templates\%relative_path%
set target_path=%templates_dir%\%relative_path%
copy /y "%source_path%" "%target_path%" || exit /b 1
goto :eof
