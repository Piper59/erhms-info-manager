@echo off
setlocal

:main
call :delete Worker || exit /b 1
call :delete Incident || exit /b 1
goto :eof

:delete
set project_type=%~1
set project_location=%CD%\Projects\%project_type%
set project_path=%CD%\Projects\%project_type%\%project_type%.prj
ERHMS.Console DeleteDatabase "%project_path%" || exit /b 1
ERHMS.Console DeleteProject "%project_path%" || exit /b 1
goto :eof
