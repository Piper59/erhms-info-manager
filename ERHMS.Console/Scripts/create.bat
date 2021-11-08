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
    set templates_dir=%~1
)
goto :main

:usage
echo Usage: %arg0% TEMPLATES_DIR
goto :eof

:main
call :set_project Blank
call :update_page BlankPage || exit /b 1
call :update_view BlankFormWithoutWorkerInfo || exit /b 1
call :update_view BlankFormWithWorkerInfo || exit /b 1
call :set_project Workers
call :update_view Worker\WorkerRosteringForm || exit /b 1
call :update_view Worker\PreDeploymentHealthSurvey || exit /b 1
call :set_project Incident
call :update_view Incident\WorkerDeploymentRecord || exit /b 1
call :update_view Incident\WorkerInProcessingForm || exit /b 1
call :update_view Incident\WorkerActivityReport || exit /b 1
call :update_view Incident\DeploymentHealthSurvey || exit /b 1
call :update_view Incident\WorkerOutProcessingForm || exit /b 1
call :update_view Incident\PostDeploymentHealthSurvey || exit /b 1
call :update_view Incident\AfterActionReview || exit /b 1
goto :eof

:set_project
set project_location=Projects\%~1
set project_name=%~n1
set project_path=%project_location%\%project_name%.prj
goto :eof

:update_page
set relative_path=Pages\%~1.xml
set page_name=%~n1
set view_name=%page_name%
set source_path=Templates\%relative_path%
set target_path=%templates_dir%\%relative_path%
ERHMS.Console CreateTemplate "%source_path%" "%project_path%" "%view_name%" "%page_name%" || exit /b 1
ERHMS.Console CanonizeTemplate "%source_path%" || exit /b 1
copy /y "%source_path%" "%target_path%" || exit /b 1
goto :eof

:update_view
set relative_path=Forms\%~1.xml
set view_name=%~n1
set source_path=Templates\%relative_path%
set target_path=%templates_dir%\%relative_path%
ERHMS.Console CreateTemplate "%source_path%" "%project_path%" "%view_name%" || exit /b 1
ERHMS.Console CanonizeTemplate "%source_path%" || exit /b 1
copy /y "%source_path%" "%target_path%" || exit /b 1
goto :eof
