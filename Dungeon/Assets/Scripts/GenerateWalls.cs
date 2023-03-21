using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GenerateWalls : MonoBehaviour
{
    private Dictionary<Vector2Int, int> wallData = new Dictionary<Vector2Int, int>();

    public static void PlaceWalls(HashSet<Vector2Int> floor, TileMapDrawer visualiser)
    {
        Dictionary<Vector2Int, int> wallsTypes = WallsWithTypes(floor, DirectionsClass.GetDirectionsTypes());

        foreach (var wall in wallsTypes)
        {
            visualiser.PaintWall(wall.Key, wall.Value);
        }
    }

    private static Dictionary<Vector2Int, int> WallsWithTypes(HashSet<Vector2Int> positions,
        Dictionary<Vector2Int, int> directionTypes)
    {
        Dictionary<Vector2Int, int> wallPositions = new Dictionary<Vector2Int, int>();

        foreach (var floorTile in positions)
        {
            foreach (var directionWithType in directionTypes)
            {
                Vector2Int neighbourPosition = floorTile + directionWithType.Key;
                if (positions.Contains(neighbourPosition))
                {
                    continue;
                }

                if (!wallPositions.ContainsKey(neighbourPosition))
                {
                    wallPositions.Add(neighbourPosition, directionWithType.Value);
                }
            }
        }

        return wallPositions;
    }

    private static HashSet<Vector2Int> FindWallPositions(HashSet<Vector2Int> positions, List<Vector2Int> directionList)
    {
        Debug.Log("WALLS");
        HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();
        int type = 0;

        foreach (Vector2Int position in positions)
        {
            foreach (Vector2Int direction in directionList)
            {
                Vector2Int neighbourPosition = position + direction;
                if (positions.Contains(
                        neighbourPosition)) //if the neighbour position is on the positions' list, it's a floor tile so not a wall
                {
                    continue;
                }

                wallPositions.Add(neighbourPosition);
                Debug.Log(neighbourPosition + "\t" + direction);
            }
        }

        return wallPositions;
    }
}