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
    /// <returns>HashSet of Vector2 which acts like a path generated by random walk algorithm</returns>
    public static HashSet<Vector2> SimpleRandomWalk(Vector2 startPosition, int numberOfSteps)
    {
        HashSet<Vector2> path = new HashSet<Vector2> { startPosition };
        Vector2 previousPosition = startPosition;

        for (int i = 0; i < numberOfSteps; i++)
        {
            Vector2 currentPosition = previousPosition + DirectionsClass.GetRandomDirection();
            path.Add(currentPosition);
            previousPosition = currentPosition;
        }

        return path;
    }

    //list instead of hashset to get the last position
    public static List<Vector2> SimpleRandomWalkCorridor(Vector2 startPosition, int corridorLength)
    {
        List<Vector2> corridor = new List<Vector2>();
        Vector2 direction = DirectionsClass.GetRandomDirection();
        Vector2 currentPosition = startPosition;
        corridor.Add(currentPosition);
        for (int i = 0; i < corridorLength; i++)
        {
            currentPosition += direction;
            corridor.Add(currentPosition);
        }

        return corridor;
    }


    //BoundsInt - never rotated square
    public static List<BoundsInt> BSP(BoundsInt spaceToSplit, int minWidth, int minHeight)
    {
        Queue<BoundsInt> rooms = new Queue<BoundsInt>();
        rooms.Clear();
        rooms.Enqueue(spaceToSplit);

        List<BoundsInt> roomsList = new List<BoundsInt>();
        roomsList.Clear();

        while (rooms.Count > 0)
        {
            BoundsInt room = rooms.Dequeue();
            if (room.size.x >= minWidth && room.size.y >= minHeight)
            {
                if (Random.value < 0.5f) //to make sure that random split is more random
                {
                    if (room.size.x >= minWidth * 2)
                    {
                        SplitV(rooms, room, minWidth);
                    }
                    else if (room.size.y >= minHeight * 2)
                    {
                        SplitH(rooms, room, minHeight);
                    }
                    else
                    {
                        roomsList.Add(room);
                    }
                }
                else
                {
                    if (room.size.y >= minHeight * 2)
                    {
                        SplitH(rooms, room, minHeight);
                    }
                    else if (room.size.x >= minWidth * 2)
                    {
                        SplitV(rooms, room, minWidth);
                    }
                    else
                    {
                        roomsList.Add(room);
                    }
                }
            }
        }

        return roomsList;
    }

    private static void SplitV(Queue<BoundsInt> rooms, BoundsInt room, int minWidth)
    {
        int xSplit = Random.Range(minWidth, room.size.y - minWidth);
        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(xSplit, room.size.y, room.size.z));
        BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x + xSplit, room.min.y, room.min.z),
            new Vector3Int(room.size.x - xSplit, room.size.y, room.size.z));
        rooms.Enqueue(room1);
        rooms.Enqueue(room2);
    }

    private static void SplitH(Queue<BoundsInt> rooms, BoundsInt room, int minHeight)
    {
        int ySplit = Random.Range(minHeight, room.size.x - minHeight);
        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(room.size.x, ySplit, room.size.z));
        BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x, room.min.y + ySplit, room.min.z),
            new Vector3Int(room.size.x, room.size.y - ySplit, room.size.z));
        rooms.Enqueue(room1);
        rooms.Enqueue(room2);
    }
}

public static class DirectionsClass
{
    //up, right, down, left
    private static readonly List<Vector2> Directions = new List<Vector2>
    {
        new Vector2(0, 1),
        new Vector2(1, 0),
        new Vector2(0, -1),
        new Vector2(-1, 0)
    };

    private static readonly Dictionary<Vector2, int> DirectionsTypes = new Dictionary<Vector2, int>()
    {
        [new Vector2(0, 1)]   = 0,
        [new Vector2(1, 0)]   = 1,
        [new Vector2(0, -1)]  = 2,
        [new Vector2(-1, 0)]  = 3,
        // [new Vector2(1, 1)]   = 4,
        // [new Vector2(1, -1)]  = 5,
        // [new Vector2(-1, -1)] = 6,
        // [new Vector2(-1, 1)]  = 7
    };

    //up-right, down-right, down-left, up-left
    private static readonly List<Vector2> DirectionsDiagonal = new List<Vector2>
    {
        new Vector2(1, 1),
        new Vector2(1, -1),
        new Vector2(-1, -1),
        new Vector2(-1, 1)
    };

    public static List<Vector2> GetDirections() => Directions;
    public static List<Vector2> GetDirectionsDiagonal() => DirectionsDiagonal;
    public static Dictionary<Vector2, int> GetDirectionsTypes() => DirectionsTypes;

    public static Vector2 GetRandomDirection()
    {
        return Directions[Random.Range(0, Directions.Count)];
    }
}