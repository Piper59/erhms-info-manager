@echo off
setlocal

set solution_dir_path=%CD%\..\..\..

:main
call :canonize Worker WorkerRosteringForm || exit /b 1
call :canonize Worker PreDeploymentHealthSurvey || exit /b 1
call :canonize Incident WorkerDeploymentRecord || exit /b 1
call :canonize Incident WorkerInProcessingForm || exit /b 1
call :canonize Incident WorkerActivityReport || exit /b 1
call :canonize Incident DeploymentHealthSurvey || exit /b 1
call :canonize Incident WorkerOutProcessingForm || exit /b 1
call :canonize Incident PostDeploymentHealthSurvey || exit /b 1
call :canonize Incident AfterActionReview || exit /b 1
goto :eof

:canonize
set project_type=%~1
set view_name=%~2
set template_path=Templates\Forms\%project_type%\%view_name%.xml
set project_path=Projects\%project_type%\%project_type%.prj
ERHMS.Console CreateTemplate "%template_path%" "%project_path%" "%view_name%" || exit /b 1
ERHMS.Console CanonizeTemplate "%template_path%" || exit /b 1
copy /y "%template_path%" "%solution_dir_path%\ERHMS.Resources\%template_path%" || exit /b 1
goto :eof
