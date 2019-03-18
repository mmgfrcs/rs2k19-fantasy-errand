set "BUILD_PATH=%cd%\Builds\Android\"
mkdir -p "%BUILD_PATH%"

echo Building for Android locally on %cd%, Build Path is on %BUILD_PATH%

Unity "%BUILD_PATH%" -projectPath "%cd%" -quit -batchmode -nographics -executeMethod BuildCI.PerformBuild -logFile "%cd%\log" || goto :CRITICAL_ERROR

IF ERRORLEVEL 2 goto ERR_TWO
IF ERRORLEVEL 1 goto ERR_ONE
IF ERRORLEVEL 0 goto ERR_ZERO

echo Unexpected exit code %ERRORLEVEL%
goto END

:ERR_ONE
echo Run succeeded, some tests failed
goto END

:ERR_TWO
echo Run failure (other failure)
goto END

:ERR_ZERO
echo Run succeeded, no failures occurred
goto END

:CRITICAL_ERROR
type "%cd%\log"
echo Batch Error
exit 1

:END
type "%cd%\log"
cd "%BUILD_PATH%" || goto :CRITICAL_ERROR
echo Checking built APK
IF not exist "%cd%\out.apk" exit 1