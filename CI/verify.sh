trap "echo ' > Verification failed'" ERR
echo "===Start Verifying Project==="
echo "Verifying SDK Version, expected 24"
grep -Fq "AndroidMinSdkVersion: 24" "./ProjectSettings/ProjectSettings.asset"
echo " > SDK Version verified"
echo "Verifying Bundle Name, expected com.ferresearcher.fantasyerrand"
grep -Fq "Android: com.ferresearcher.fantasyerrand" "./ProjectSettings/ProjectSettings.asset"
echo " > Bundle Name verified"
echo "Verifying Game Name, expected Fantasy Errand"
grep -Fq "productName: Fantasy Errand" "./ProjectSettings/ProjectSettings.asset"
echo " > Game Name verified"
echo "Verifying Screen Orientation, expected Portrait 0"
grep -Fq "defaultScreenOrientation: 0" "./ProjectSettings/ProjectSettings.asset"
echo " > Screen Orientation verified"
echo "Verifying Project Version, expected 2018.1.1f1"
grep -Fq "m_EditorVersion: 2018.1.1f1" "./ProjectSettings/ProjectVersion.txt"
echo " > Editor version verified"
echo "Verifying Assets folder structure"
cd ./Assets/
files=(Imports/ Materials/ Sprites/ Scripts/ Scenes/ StreamingAssets/)
for f in $(ls -d */); do
    found=0
    for g in "${files[@]}" ; do
        [ "$f" = "$g" ] && found=1
    done
    if [ "$found" -eq 0 ]; then
        echo "Unexpected $f. Verification failed" || exit 1
    fi
done
echo " > Folder Structure verified"
echo "Verification Complete"