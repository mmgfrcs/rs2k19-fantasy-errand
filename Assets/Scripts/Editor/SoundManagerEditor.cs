using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using FantasyErrand;

[CustomEditor(typeof(SoundManager))]
public class SoundManagerEditor : Editor
{
    public override bool RequiresConstantRepaint()
    {
        return true;
    }
    public override void OnInspectorGUI()
    {
        SoundManager sound = (SoundManager)target;
        EditorGUILayout.LabelField("BGM Playing/Total", $"{sound.GetPlayingListCount(SoundManager.SoundChannel.BGM)}/{sound.GetQueueCount(SoundManager.SoundChannel.BGM)}");
        EditorGUILayout.LabelField("SFX Playing/Total", $"{sound.GetPlayingListCount(SoundManager.SoundChannel.SFX)}/{sound.GetQueueCount(SoundManager.SoundChannel.SFX)}");
        EditorGUILayout.LabelField("Voice Playing/Total", $"{sound.GetPlayingListCount(SoundManager.SoundChannel.Voice)}/{sound.GetQueueCount(SoundManager.SoundChannel.Voice)}");
        base.OnInspectorGUI();

    }
}
