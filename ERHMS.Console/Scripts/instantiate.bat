@echo off
setlocal

:main
call :instantiate Worker WorkerRosteringForm || exit /b 1
call :instantiate Worker PreDeploymentHealthSurvey || exit /b 1
call :instantiate Incident WorkerDeploymentRecord || exit /b 1
call :instantiate Incident WorkerInProcessingForm || exit /b 1
call :instantiate Incident WorkerActivityReport || exit /b 1
call :instantiate Incident DeploymentHealthSurvey || exit /b 1
call :instantiate Incident WorkerOutProcessingForm || exit /b 1
call :instantiate Incident PostDeploymentHealthSurvey || exit /b 1
call :instantiate Incident AfterActionReview || exit /b 1
goto :eof

:instantiate
set project_type=%~1
set form_name=%~2
set template_path=Templates\Forms\%project_type%\%form_name%.xml
set project_path=Projects\%project_type%\%project_type%.prj
ERHMS.Console InstantiateTemplate "%template_path%" "%project_path%" "%form_name%" || exit /b 1
goto :eof
