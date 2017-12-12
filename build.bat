@echo off

setlocal EnableDelayedExpansion

set configuration=Debug
set "flags="
for %%A in (%*) do (
    if "%%A"=="-r"           set configuration=Release
    if "%%A"=="/r"           set configuration=Release
    if "%%A"=="--release"    set configuration=Release

    if "%%A"=="-h"           goto HELP
    if "%%A"=="/h"           goto HELP
    if "%%A"=="--help"       goto HELP

    if "%%A"=="-c"           goto CLEAN
    if "%%A"=="/c"           goto CLEAN
    if "%%A"=="--clean"      goto CLEAN
)

set defs=
for %%A in (%flags%) do (
    set defs=!defs!%%A;
)

set command=msbuild /p:Configuration=%configuration% /p:Constants="%defs%" /t:Rebuild /m:1
@echo on
call %%command%%
@echo off
echo.
echo %command%
echo.
echo Configuration: %configuration%
echo Flags: %flags%
goto END

:CLEAN
@echo on
msbuild /property:Configuration=Debug /t:Clean /m
msbuild /property:Configuration=Release /t:Clean /m
@echo off
goto END

:HELP
echo Usage:
echo  "> build [OPTION]"
echo Options:
echo   -r    /r     --release     - Build in release mode.
echo   -c    /c     --clean       - Remove all binaries.
echo   -h    /h     --help        - Show this message.

:END
