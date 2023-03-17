using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Algorithms;
using UnityEngine;
using UnityEngine.Serialization;

public class RoomsGenerator : MonoBehaviour
{
    [SerializeField] private CorridorGenerator corridors;
    [SerializeField] private float percent = 0.8f;

    public void Run()
    {
        
    }

    private HashSet<Vector2Int> GenerateRooms()
    {
        HashSet<Vector2Int> possibleRoomPositions = corridors.Corridor;
        int roomsToBeCreated = Mathf.RoundToInt(possibleRoomPositions.Count * percent);
        List<Vector2Int> rooms = new List<Vector2Int>();

        for (int i = 0; i < roomsToBeCreated; i++)
        {
            if (rooms.Contains(possibleRoomPositions.ElementAt(i)))
            {
                
            }
        }

        return possibleRoomPositions;
    }
}
