using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateWalls : MonoBehaviour
{
    public static void PlaceWalls(HashSet<Vector2Int> floor, TileMapDrawer visualiser)
    {
        HashSet<Vector2Int> walls = FindWallPositions(floor);

        foreach (Vector2Int wall in walls)
        {
            visualiser.PaintWall(wall);
        }
    }
    private static HashSet<Vector2Int> FindWallPositions(HashSet<Vector2Int> positions)
    {
        HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();
        List<Vector2Int> directionList = DirectionsClass.GetDirections();

        foreach (Vector2Int position in positions)
        {
            foreach (Vector2Int direction in directionList)
            {
                Vector2Int neighbourPosition = position + direction;
                if (positions.Contains(neighbourPosition))       //if the neighbour position is on the positions' list, it's a floor tile so not a wall
                {
                    continue;
                }
                wallPositions.Add(neighbourPosition);
            }
        }

        return wallPositions;
    }
}
