using System.Collections;
using System.Collections.Generic;
using Algorithms;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CorridorGenerator))]
public class CorridorGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        CorridorGenerator corridor = (CorridorGenerator)target;

        if (GUILayout.Button("Preview Floors"))
        {
            corridor.Run();
            corridor.PlaceTiles();
        }
        
        if (GUILayout.Button("Clear"))
        {
            corridor.Clear();
        }
    }
}