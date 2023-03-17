using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralGenerationAlgorithm
{
    /// <summary>
    /// HashSet prevents the collection from having duplicates
    /// </summary>
    /// <param name="startPosition"></param>
    /// <param name="numberOfSteps"></param>
    /// <returns>HashSet of Vector2Int which acts like a path generated by random walk algorithm</returns>
    public static HashSet<Vector2Int> SimpleRandomWalk(Vector2Int startPosition, int numberOfSteps)
    {
        HashSet<Vector2Int> path = new HashSet<Vector2Int> { startPosition };
        Vector2Int previousPosition = startPosition;

        for (int i = 0; i < numberOfSteps; i++)
        {
            Vector2Int currentPosition = previousPosition + DirectionsClass.GetRandomDirection();
            path.Add(currentPosition);
            previousPosition = currentPosition;
        }

        return path;
    }
}

public static class DirectionsClass
{
    //up, right, down, left
    private static readonly List<Vector2Int> Directions = new List<Vector2Int>
    {
        new Vector2Int(0, 1),
        new Vector2Int(1, 0),
        new Vector2Int(0, -1),
        new Vector2Int(-1, 0)
    };

    public static Vector2Int GetRandomDirection()
    {
        return Directions[Random.Range(0, Directions.Count)];
    }
}