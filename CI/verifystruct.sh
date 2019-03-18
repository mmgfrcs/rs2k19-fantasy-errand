trap "echo ' > Verification failed'" ERR
echo "===Start Verifying Files in Folders==="

echo "Verifying Animations folder"

find "./Unity/Research/Assets/Animations" -depth -type f  | wc -l

OIFS="$IFS"
IFS=$'\n'
files=(meta anim controller)
for f in $(find "./Animations" -depth -print); do
    if [ -f "$f" ]; then
        found=0
        for g in "${files[@]}" ; do
            [ "$f" = "$g" ] && found=1
        done
        if [ "$found" -eq 0 ]; then
            echo "Unexpected $f. Verification failed" && exit 1
        else
            echo " > $f passed"
        fi
    fi
done



IFS="$OIFS"
echo "Verification Complete"