using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FantasyErrand.Entities;

public enum PatternType
{
    Straight, Junction, LeftCorner, RightCorner
}

public class PathGenerator : MonoBehaviour {

    List<Pattern> patterns = new List<Pattern>();
    
    public GameObject startPrefab, straightPrefab, junctionPrefab, leftCornerPrefab, rightCornerPrefab;
    public float[] probabilityRatios = new float[5];

    public int patternToGenerate = 10;
    public int minStraights = 4;
    
    public float tileScale;

    public UnityEngine.UI.Text text;

	// Use this for initialization
	void Start () {
        StartCoroutine(Generate(patternToGenerate));
	}
    
    IEnumerator Generate(int n, bool isStart = true, Vector3 startPos = default(Vector3), Vector3 moveDir = new Vector3(), int straights = 0)
    {
        if (moveDir == new Vector3()) moveDir = Vector3.forward;
        if (startPos == default(Vector3)) startPos = Vector3.zero;
        for (int i = 0; i < n; i++)
        {
            if (isStart && i < 2)
            {
                GameObject obj = Instantiate(startPrefab, new Vector3(startPos.x, -0.05f, startPos.z), MoveDirToRotation(moveDir));
                obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.localScale.y / -2f, obj.transform.position.z);
                patterns.Add(new Pattern() { type = PatternType.Straight, position = new Vector3(startPos.x, 0, startPos.z) });
                
            }
            else
            {
                if (straights == 0)
                {

                    int rnd = MathRand.WeightedPick(probabilityRatios);
                    if (rnd <= 1)
                    {
                        GameObject obj = Instantiate(straightPrefab, new Vector3(startPos.x, -0.05f, startPos.z), MoveDirToRotation(moveDir));
                        obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.localScale.y / -2f, obj.transform.position.z);
                        patterns.Add(new Pattern() { type = PatternType.Straight, position = new Vector3(startPos.x, 0, startPos.z), obj = obj });
                    }
                    else if (rnd == 2)
                    {
                        GameObject obj = Instantiate(leftCornerPrefab, new Vector3(startPos.x, -0.05f, startPos.z), MoveDirToRotation(moveDir));
                        obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.localScale.y / -2f, obj.transform.position.z);
                        StartCoroutine(Generate(n - (i + 1), false, startPos + (Quaternion.AngleAxis(-90, Vector3.up) * moveDir * tileScale), Quaternion.AngleAxis(-90, Vector3.up) * moveDir, minStraights));
                        patterns.Add(new Pattern() { type = PatternType.LeftCorner, position = new Vector3(startPos.x, 0, startPos.z), obj = obj });
                        break;
                    }
                    else if (rnd == 3)
                    {
                        GameObject obj = Instantiate(rightCornerPrefab, new Vector3(startPos.x, -0.05f, startPos.z), MoveDirToRotation(moveDir));
                        obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.localScale.y / -2f, obj.transform.position.z);
                        StartCoroutine(Generate(n - (i + 1), false, startPos + (Quaternion.AngleAxis(90, Vector3.up) * moveDir* tileScale), Quaternion.AngleAxis(90, Vector3.up) * moveDir, minStraights));
                        patterns.Add(new Pattern() { type = PatternType.RightCorner, position = new Vector3(startPos.x, 0, startPos.z), obj = obj });
                        break;
                    }
                    else if(rnd == 4)
                    {
                        GameObject obj = Instantiate(junctionPrefab, new Vector3(startPos.x, -0.05f, startPos.z), MoveDirToRotation(moveDir));
                        obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.localScale.y / -2f, obj.transform.position.z);
                        StartCoroutine(Generate(n - (i + 1), false, startPos + (Quaternion.AngleAxis(-90, Vector3.up) * moveDir* tileScale), Quaternion.AngleAxis(-90, Vector3.up) * moveDir, minStraights));
                        StartCoroutine(Generate(n - (i + 1), false, startPos + (Quaternion.AngleAxis(90, Vector3.up) * moveDir* tileScale), Quaternion.AngleAxis(90, Vector3.up) * moveDir, minStraights));
                        patterns.Add(new Pattern() { type = PatternType.Junction, position = new Vector3(startPos.x, 0, startPos.z), obj = obj });
                        break;
                    }
                }
                else
                {
                    GameObject obj = Instantiate(straightPrefab, new Vector3(startPos.x, -0.05f, startPos.z), MoveDirToRotation(moveDir));
                    obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.localScale.y / -2f, obj.transform.position.z);
                    straights--;
                }
            }
            startPos += moveDir *tileScale;
            yield return new WaitForSeconds(1f);
        }
    }

    Quaternion MoveDirToRotation(Vector3 moveDir)
    {
        if (moveDir == Vector3.left) return Quaternion.AngleAxis(-90, Vector3.up);
        else if (moveDir == Vector3.right){
                return Quaternion.AngleAxis(90, Vector3.up);
        } 
        else if (moveDir == Vector3.back) return Quaternion.AngleAxis(180, Vector3.up);
        else return Quaternion.identity;
    }

    Vector3 RotateVector(Vector3 original, Vector3 direction)
    {
        return Quaternion.Euler(0, -45, 0) * original;
    }
}

public class Pattern
{
    public PatternType type;
    public Vector3 position;
    public GameObject obj;
}
