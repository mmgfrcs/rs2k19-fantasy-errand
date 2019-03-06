echo "Trying to run Unity"
${UNITY_EXECUTABLE:-xvfb-run --auto-servernum --server-args='-screen 0 640x480x24' /opt/Unity/Editor/Unity} \
  -projectPath $(pwd) \
  -quit \
  -batchmode \
  -silent-crashes \
  -logFile

cat $(pwd)/unity.log

UNITY_EXIT_CODE=$?
[ UNITY_EXIT_CODE -eq 0 ]
echo "Run complete"