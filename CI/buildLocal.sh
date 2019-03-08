set -e
set -x

echo "Building for Android locally"

export BUILD_PATH=./Builds/Android/
mkdir -p $BUILD_PATH

Unity -projectPath $(pwd) -quit -batchmode -nographics -buildTarget Android -executeMethod BuildCI.PerformBuild $BUILD_PATH -logFile log

UNITY_EXIT_CODE=$?
cat log

if [ $UNITY_EXIT_CODE -eq 0 ]; then
  echo "Run succeeded, no failures occurred";
elif [ $UNITY_EXIT_CODE -eq 2 ]; then
  echo "Run succeeded, some tests failed";
elif [ $UNITY_EXIT_CODE -eq 3 ]; then
  echo "Run failure (other failure)";
else
  echo "Unexpected exit code $UNITY_EXIT_CODE";
fi

ls -la $BUILD_PATH
[ -n "$(ls -A $BUILD_PATH)" ] # fail job if build folder is empty