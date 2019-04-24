using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HorizontalLayoutGroup))]
public class SegmentedBar : MonoBehaviour {
    [SerializeField]
    private int value;
    [SerializeField]
    private int maxValue;
    [SerializeField]
    private Color fillColor, backgroundColor;

    public GameObject segment;

    List<Image> spawnedSegments = new List<Image>();

    public int Value { get { return value; } set {
            this.value = value;
            ValueChange();
        }
    }

    public int MaxValue { get { return maxValue; } set {
            maxValue = value;
            MaxValueChange();
        }
    }

    // Use this for initialization
    void Start () {
        MaxValueChange();
        ValueChange();
	}

    void MaxValueChange()
    {
        GetComponent<HorizontalLayoutGroup>().spacing = 32 / (2 * maxValue);
        if (maxValue > spawnedSegments.Count)
        {
            for (int i = spawnedSegments.Count; i < maxValue; i++)
            {
                spawnedSegments.Add(Instantiate(segment, transform).GetComponent<Image>());
            }
        }
        else
        {
            for (int i = 0; i < spawnedSegments.Count; i++)
            {
                if (i < maxValue)  spawnedSegments[i].gameObject.SetActive(true);
                else spawnedSegments[i].gameObject.SetActive(false);
            }
        }
        
    }

    void ValueChange()
    {
        for(int i = 0; i < spawnedSegments.Count; i++)
        {
            if (i < value) spawnedSegments[i].color = fillColor;
            else spawnedSegments[i].color = backgroundColor;
        }
    }
	
	// Update is called once per frame
	void Update () {

	}
}
