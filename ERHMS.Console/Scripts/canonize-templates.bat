@echo off
setlocal

call :canonize Worker
call :canonize Incident
goto :eof

:canonize
set project_type=%~1
set project_path="%CD%\Projects\%project_type%\%project_type%.prj"
for %%f in ("Templates\Forms\%project_type%\*") do (
    ERHMS.Console CreateTemplate "%%f" %project_path% "%%~nf"
    ERHMS.Console CanonizeTemplate "%%f"
    copy /y "%%f" "..\..\..\ERHMS.Resources\%%f"
)
goto :eof
