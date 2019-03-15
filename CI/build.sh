set -e

echo "Building for Android"

export BUILD_PATH=./Builds/Android/
mkdir -p $BUILD_PATH

# sudo apt-get -qq update
# wget https://dl.google.com/android/repository/sdk-tools-linux-4333796.zip -O sdk-tools
# unzip sdk-tools
# ./tools/bin/sdkmanager "build-tools;28.0.3" "platform-tools" "platforms;android-24" --verbose --install
# yes | ./tools/bin/sdkmanager --licenses

${UNITY_EXECUTABLE:-xvfb-run --auto-servernum --server-args='-screen 0 640x480x24' /opt/Unity/Editor/Unity} \
  -projectPath $(pwd) \
  -quit \
  -batchmode \
  -buildWindowsPlayer $BUILD_PATH \
  -logFile

UNITY_EXIT_CODE=$?

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