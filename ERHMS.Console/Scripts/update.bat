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
call :update_view Blank BlankFormWithWorkerInfo || exit /b 1
call :update_view Blank BlankFormWithoutWorkerInfo || exit /b 1
call :update_page Blank BlankPage || exit /b 1
call :update_view Worker WorkerRosteringForm || exit /b 1
call :update_view Worker PreDeploymentHealthSurvey || exit /b 1
call :update_view Incident WorkerDeploymentRecord || exit /b 1
call :update_view Incident WorkerInProcessingForm || exit /b 1
call :update_view Incident WorkerActivityReport || exit /b 1
call :update_view Incident DeploymentHealthSurvey || exit /b 1
call :update_view Incident WorkerOutProcessingForm || exit /b 1
call :update_view Incident PostDeploymentHealthSurvey || exit /b 1
call :update_view Incident AfterActionReview || exit /b 1
goto :eof

:update_view
set project_name=%~1
set view_name=%~2
set project_path=Projects\%project_name%\%project_name%.prj
if "%project_name%" == "Blank" (
    set relative_template_path=Forms\%view_name%.xml
) else (
    set relative_template_path=Forms\%project_name%\%view_name%.xml
)
set source_template_path=Templates\%relative_template_path%
set target_template_path=%templates_dir%\%relative_template_path%
ERHMS.Console CreateTemplate "%source_template_path%" "%project_path%" "%view_name%" || exit /b 1
ERHMS.Console CanonizeTemplate "%source_template_path%" || exit /b 1
copy /y "%source_template_path%" "%target_template_path%" || exit /b 1
goto :eof

:update_page
set project_name=%~1
set page_name=%~2
set view_name=%page_name%
set project_path=Projects\%project_name%\%project_name%.prj
if "%project_name%" == "Blank" (
    set relative_template_path=Pages\%page_name%.xml
) else (
    set relative_template_path=Pages\%project_name%\%page_name%.xml
)
set source_template_path=Templates\%relative_template_path%
set target_template_path=%templates_dir%\%relative_template_path%
ERHMS.Console CreateTemplate "%source_template_path%" "%project_path%" "%view_name%" "%page_name%" || exit /b 1
ERHMS.Console CanonizeTemplate "%source_template_path%" || exit /b 1
copy /y "%source_template_path%" "%target_template_path%" || exit /b 1
goto :eof
