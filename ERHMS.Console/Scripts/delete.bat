@echo off
setlocal

:main
call :delete_project Blank
call :delete_project Workers
call :delete_project Incident
goto :eof

:delete_project
set project_location=Projects\%~1
set project_name=%~n1
set project_path=%project_location%\%project_name%.prj
ERHMS.Console DeleteDatabase "%project_path%"
ERHMS.Console DeleteProject "%project_path%" true
goto :eof
