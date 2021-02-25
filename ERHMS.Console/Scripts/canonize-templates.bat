@echo off
setlocal

call :canonize_template Worker WorkerRosteringForm
call :canonize_template Worker PreDeploymentHealthSurvey
call :canonize_template Incident WorkerDeploymentRecord
call :canonize_template Incident WorkerInProcessingForm
call :canonize_template Incident WorkerActivityReport
call :canonize_template Incident DeploymentHealthSurvey
call :canonize_template Incident WorkerOutProcessingForm
call :canonize_template Incident PostDeploymentHealthSurvey
call :canonize_template Incident AfterActionReview
goto :eof

:canonize_template
set project_type=%~1
set form_name=%~2
set template_path="Templates\Forms\%project_type%\%form_name%.xml"
set project_path="Projects\%project_type%\%project_type%.prj"
ERHMS.Console CreateTemplate %template_path% %project_path% %form_name%
ERHMS.Console CanonizeTemplate %template_path%
goto :eof
