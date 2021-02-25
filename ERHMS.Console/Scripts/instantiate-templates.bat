@echo off
setlocal

call :create_project Worker
call :instantiate_template Worker WorkerRosteringForm
call :instantiate_template Worker PreDeploymentHealthSurvey
call :create_project Incident
call :instantiate_template Incident WorkerDeploymentRecord
call :instantiate_template Incident WorkerInProcessingForm
call :instantiate_template Incident WorkerActivityReport
call :instantiate_template Incident DeploymentHealthSurvey
call :instantiate_template Incident WorkerOutProcessingForm
call :instantiate_template Incident PostDeploymentHealthSurvey
call :instantiate_template Incident AfterActionReview
goto :eof

:create_project
set project_type=%~1
set connection_string="Provider=Microsoft.Jet.OLEDB.4.0;Data Source=""%CD%\Projects\%project_type%\%project_type%.mdb"""
set project_location="%CD%\Projects\%project_type%"
set project_path="%CD%\Projects\%project_type%\%project_type%.prj"
ERHMS.Console CreateDatabase Access2003 %connection_string%
ERHMS.Console CreateProject Access2003 %connection_string% %project_location% %project_type%
ERHMS.Console InitializeProject %project_path%
goto :eof

:instantiate_template
set project_type=%~1
set form_name=%~2
set template_path="Templates\Forms\%project_type%\%form_name%.xml"
set project_path="Projects\%project_type%\%project_type%.prj"
ERHMS.Console InstantiateTemplate %template_path% %project_path% %form_name%
goto :eof
