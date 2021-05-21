@echo off
setlocal

:main
call :delete_project Blank || exit /b 1
call :delete_project Workers || exit /b 1
call :delete_project Incidents\SampleIncident || exit /b 1
goto :eof

:delete_project
set project_location=Projects\%~1
set project_name=%~n1
set project_path=%project_location%\%project_name%.prj
ERHMS.Console DeleteDatabase "%project_path%" || exit /b 1
ERHMS.Console DeleteProject "%project_path%" true || exit /b 1
goto :eof
