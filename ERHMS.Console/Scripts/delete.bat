@echo off
setlocal

:main
call :delete_project Blank || exit /b 1
call :delete_project Worker || exit /b 1
call :delete_project Incident || exit /b 1
goto :eof

:delete_project
set project_name=%~1
set project_path=Projects\%project_name%\%project_name%.prj
ERHMS.Console DeleteDatabase "%project_path%" || exit /b 1
ERHMS.Console DeleteProject "%project_path%" || exit /b 1
goto :eof
