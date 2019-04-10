using System.Collections;
using UnityEngine;
using UnityEditor;
using FantasyErrand.WebSockets;

[CustomEditor(typeof(ResearchDataManager))]
public class ResearchDataManagerEditor : Editor {

    public override bool RequiresConstantRepaint()
    {
        return true;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

    }
}
