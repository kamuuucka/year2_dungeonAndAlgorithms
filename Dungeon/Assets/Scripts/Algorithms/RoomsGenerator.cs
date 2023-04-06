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
     [SerializeField] private CorridorGenerator corridor;
     [SerializeField] private TileMapDrawer visualiser;
     [SerializeField] private float percentOfRooms = 1.0f;
     [SerializeField] private int minRoomHeight = 10;
     [SerializeField] private int minRoomWidth = 10;
     [SerializeField] private int areaToSplitHeight = 70;
     [SerializeField] private int areaToSplitWidth = 70;

     public void Run()
     {
         GenerateRoomsPositions(corridor.DeadEnds);
     }
     
     /// <summary>
     /// Generates the possible positions where rooms can be spawned.
     /// Takes into consideration ends of the corridors and places where corridors are crossing.
     /// Multiplies the amount of number by the percentage of how many rooms should be created.
     /// Makes sure that rooms are always generated at the very ends of the corridors - no dead ends.
     /// </summary>
     private HashSet<Vector2> GenerateRoomsPositions(HashSet<Vector2> deadEnds)
     {
         HashSet<Vector2> possibleRoomPositions = corridor.CorridorEnds;
         int roomsToBeCreated = Mathf.RoundToInt(possibleRoomPositions.Count * percentOfRooms);
         List<Vector2> rooms = new List<Vector2>(possibleRoomPositions);
         possibleRoomPositions.Clear();
         
         for (int i = 0; i < roomsToBeCreated; i++)
         {
             possibleRoomPositions.Add(rooms[i]);
         }

         foreach (var end in deadEnds)
         {
             if (!possibleRoomPositions.Contains(end))
             {
                 possibleRoomPositions.Add(end);
             }
         }

         return possibleRoomPositions;
     }

 }
