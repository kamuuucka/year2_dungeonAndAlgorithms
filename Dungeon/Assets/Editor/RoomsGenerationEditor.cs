using System.Collections;
using System.Collections.Generic;
using Algorithms;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RoomsGenerator))]
public class RoomsGenerationEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        RoomsGenerator corridor = (RoomsGenerator)target;

        if (GUILayout.Button("Preview Floors"))
        {
            corridor.Run();
            //corridor.PlaceTiles();
        }
        
        if (GUILayout.Button("Clear"))
        {
            //corridor.Clear();
        }
    }
}