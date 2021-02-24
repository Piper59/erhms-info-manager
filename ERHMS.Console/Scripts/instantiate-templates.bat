@echo off
setlocal

call :instantiate Worker
call :instantiate Incident
goto :eof

:instantiate
set project_type=%~1
set connection_string="Provider=Microsoft.Jet.OLEDB.4.0;Data Source=""%CD%\Projects\%project_type%\%project_type%.mdb"""
set project_location="%CD%\Projects\%project_type%"
set project_path="%CD%\Projects\%project_type%\%project_type%.prj"
ERHMS.Console CreateDatabase Access2003 %connection_string%
ERHMS.Console CreateProject Access2003 %connection_string% %project_location% %project_type%
ERHMS.Console InitializeProject %project_path%
for %%f in (..\..\..\ERHMS.Resources\Templates\Forms\%project_type%\*) do ERHMS.Console InstantiateTemplate "%%f" %project_path% "%%~nf"
goto :eof
