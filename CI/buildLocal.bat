echo "Building for Android locally"

set BUILD_PATH=.\Builds\Android\
mkdir -p %BUILD_PATH%

Unity.lnk -projectPath %cd% -quit -batchmode -nographics -buildTarget Android -executeMethod BuildCI.PerformBuild %BUILD_PATH% -logFile log

IF ERRORLEVEL 2 goto ERR_TWO
IF ERRORLEVEL 1 goto ERR_ONE
IF ERRORLEVEL 0 goto ERR_ZERO

echo "Unexpected exit code %ERRORLEVEL%";
goto END

:ERR_ONE
echo "Run succeeded, some tests failed";
goto END

:ERR_TWO
echo "Run failure (other failure)";
goto END

:ERR_ZERO
echo "Run succeeded, no failures occurred";
goto END

:END
type log
cd %BUILD_PATH%
IF not exist *.* exit 1