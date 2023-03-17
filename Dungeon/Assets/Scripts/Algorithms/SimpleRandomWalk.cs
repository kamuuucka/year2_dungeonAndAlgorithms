using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class SimpleRandomWalk : MonoBehaviour
{
    [SerializeField] private Vector2Int startPosition = Vector2Int.zero;
    
    
    [SerializeField] private string filename;
    [SerializeField] private int iterations = 10;
    [SerializeField] private int walkLength = 10;
    [SerializeField] private bool startRandomly = true;
    [SerializeField] private PathVisualiser visualiser;

    public int Iterations => iterations;
    public int WalkLength => walkLength;
    public bool StartRandomly => startRandomly;
    public string FileName => filename;

    private Vector2Int _currentPosition;
    private HashSet<Vector2Int> _floor = new HashSet<Vector2Int>();
    

    public void Run()
    {
        _floor = RunRandomWalk();
        
    }

    public void PlaceTiles()
    {
        visualiser.PaintTiles(_floor);
    }

    private void OnDrawGizmosSelected()
    {
        Vector2Int currentPoint;
        Vector2Int nextPoint;
        for (int i = 0; i < _floor.Count-1; i++)
        {
            currentPoint = _floor.ElementAt(i);
            nextPoint = _floor.ElementAt(i + 1);
            if (_floor.ElementAt(i+1).Equals(new Vector2Int(100, 100)))
            {
                nextPoint = _floor.ElementAt(i);
            } else if (_floor.ElementAt(i).Equals(new Vector2Int(100, 100)))
            {
                currentPoint = _floor.ElementAt(i + 1);
            }
            
            Gizmos.DrawLine(new Vector3(currentPoint.x, currentPoint.y),
                new Vector3(nextPoint.x, nextPoint.y));
        }
    }

    public void Clear()
    {
        visualiser.Clear();
    }

    private HashSet<Vector2Int> RunRandomWalk()
    {
        _currentPosition = startPosition;
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();

        for (int i = 0; i < iterations; i++)
        {
            HashSet<Vector2Int> path = ProceduralGenerationAlgorithm.SimpleRandomWalk(_currentPosition, walkLength);
            Debug.Log("New path");
            foreach (var tile in path)
            {
                Debug.Log(tile.ToString());
            }
            floorPositions.UnionWith(path);     //Add values from path to floorPositions while making sure that there are no duplicates
            //floorPositions.Add(new Vector2Int(100, 100));
            if (startRandomly)
            {
                
                _currentPosition = floorPositions.ElementAt(Random.Range(0, floorPositions.Count));
            }
        }

        return floorPositions;
    }
}