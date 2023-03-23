using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GenerateWalls : MonoBehaviour
{
    private Dictionary<Vector2, int> wallData = new Dictionary<Vector2, int>();

    public static void PlaceWalls(HashSet<Vector2> floor, TileMapDrawer visualiser)
    {
        Dictionary<Vector2, int> wallsTypes = WallsWithTypes(floor, DirectionsClass.GetDirectionsTypes());

        foreach (var wall in wallsTypes)
        {
            
            visualiser.PaintWall(wall.Key, wall.Value);
        }
    }

    private static Dictionary<Vector2, int> WallsWithTypes(HashSet<Vector2> positions,
        Dictionary<Vector2, int> directionTypes)
    {
        Dictionary<Vector2, int> wallPositions = new Dictionary<Vector2, int>();

        foreach (var floorTile in positions)
        {
            foreach (var directionWithType in directionTypes)
            {
                Vector2 neighbourPosition = floorTile + directionWithType.Key;
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

    private static HashSet<Vector2> FindWallPositions(HashSet<Vector2> positions, List<Vector2> directionList)
    {
        Debug.Log("WALLS");
        HashSet<Vector2> wallPositions = new HashSet<Vector2>();
        int type = 0;

        foreach (Vector2 position in positions)
        {
            foreach (Vector2 direction in directionList)
            {
                Vector2 neighbourPosition = position + direction;
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