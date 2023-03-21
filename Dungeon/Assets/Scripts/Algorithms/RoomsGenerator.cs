 using System;
 using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Algorithms;
using UnityEngine;
using UnityEngine.Serialization;
 using Random = System.Random;

 public class RoomsGenerator : MonoBehaviour
{
    [SerializeField] private CorridorGenerator corridors;
    [SerializeField] private float percent = 0.8f;
    [SerializeField] private TileMapDrawer visualiser;

    public void Start()
    {
        GenerateRooms();
    }

    private HashSet<Vector2Int> GenerateRooms()
    {
        HashSet<Vector2Int> possibleRoomPositions = corridors.Corridor;
        int roomsToBeCreated = Mathf.RoundToInt(possibleRoomPositions.Count * percent);
        Debug.Log(roomsToBeCreated);
        List<Vector2Int> rooms = new List<Vector2Int>(possibleRoomPositions);
        rooms = possibleRoomPositions.OrderBy(x => Guid.NewGuid()).Take(possibleRoomPositions.Count).ToList();
        possibleRoomPositions.Clear();
        for (int i = 0; i < roomsToBeCreated; i++)
        {
            possibleRoomPositions.Add(rooms[i]);
            Debug.Log(rooms[i]);
        }

        return possibleRoomPositions;
    }
}
