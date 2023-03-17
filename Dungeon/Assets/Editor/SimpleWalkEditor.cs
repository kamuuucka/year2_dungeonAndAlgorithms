using System.Collections;
using System.Collections.Generic;
using Algorithms;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SimpleRandomWalk))]
public class SimpleWalkEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        SimpleRandomWalk randomWalk = (SimpleRandomWalk)target;

        if (GUILayout.Button("Clear"))
        {
            randomWalk.Clear();
        }
        
        if (GUILayout.Button("Preview Floors"))
        {
            randomWalk.Run();
        }

        if (GUILayout.Button("Place tiles"))
        {
            randomWalk.PlaceTiles();
        }

        if (GUILayout.Button("Generate SO"))
        {
            RandomWalkSO newSo = new RandomWalkSO();
            newSo.iterations = randomWalk.Iterations;
            newSo.walkLength = randomWalk.WalkLength;
            newSo.startRandomly = randomWalk.StartRandomly;
            string path = $"Assets/SO/{randomWalk.FileName}.asset";
            AssetDatabase.CreateAsset(newSo, path);
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = newSo;
        }
    }
}
