using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Algorithms
{
   public class CorridorGenerator : MonoBehaviour
   {
      [SerializeField] protected Vector2Int startPosition = Vector2Int.zero;
      [SerializeField] private int corridorLength = 10;
      [SerializeField] private int corridorCount = 10;
      [SerializeField] private float percent = 0.8f;
      [SerializeField] private TileMapDrawer visualiser;

      private HashSet<Vector2Int> _corridor = new HashSet<Vector2Int>();
      private HashSet<Vector2Int> _corridorEnds = new HashSet<Vector2Int>();
      private HashSet<Vector2Int> _roomsCreated = new HashSet<Vector2Int>();
      private HashSet<Vector2Int> _newroomsCreated = new HashSet<Vector2Int>();

      public HashSet<Vector2Int> Corridor => _corridorEnds;

      public void Run()
      {
         _corridor = GenerateCorridors();
         _roomsCreated = GenerateRooms();
         _newroomsCreated = GenerateRoomsEnds(FindEnds(_corridor), _roomsCreated);
      }
      
      public void PlaceTiles()
      {
         visualiser.PaintTiles(_corridor);
         GenerateWalls.PlaceWalls(_corridor, visualiser);
         visualiser.PaintRooms(_roomsCreated);
         visualiser.PaintRooms(_newroomsCreated);
      }
      
      public void Clear()
      {
         visualiser.Clear();
      }
      
      private HashSet<Vector2Int> GenerateCorridors()
      {
         HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
         

         Vector2Int currentPosition = startPosition;
         for (int i = 0; i < corridorCount; i++)
         {
            List<Vector2Int> corridor =
               ProceduralGenerationAlgorithm.SimpleRandomWalkCorridor(currentPosition, corridorLength);
            currentPosition = corridor[^1];//last one
            _corridorEnds.Add(currentPosition);
            floorPositions.UnionWith(corridor);
         }

         return floorPositions;
      }
      
      //TODO: SOMEHOW move it to another script (maybe SO?)
      private HashSet<Vector2Int> GenerateRooms()
      {
         HashSet<Vector2Int> possibleRoomPositions = _corridorEnds;
         int roomsToBeCreated = Mathf.RoundToInt(possibleRoomPositions.Count * percent);
         Debug.Log(possibleRoomPositions.Count);
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

      private HashSet<Vector2Int> GenerateRoomsEnds(HashSet<Vector2Int> deadEnds, HashSet<Vector2Int> possibleRooms)
      {
         HashSet<Vector2Int> newRooms = new HashSet<Vector2Int>();
         foreach (var end in deadEnds)
         {
            if (!possibleRooms.Contains(end))
            {
               newRooms.Add(end);
            }
         }

         return newRooms;
      }

      private HashSet<Vector2Int> FindEnds(HashSet<Vector2Int> floorPositions)
      {
         HashSet<Vector2Int> deadEnds = new HashSet<Vector2Int>();
         foreach (Vector2Int floor in floorPositions)
         {
            int neighbours = 0;

            foreach (Vector2Int direction in DirectionsClass.GetDirections())
            {
               if (floorPositions.Contains(floor + direction))
               {
                  neighbours++;
               }
            }

            if (neighbours == 1)    //if has only one neighbour = dead end
            {
               deadEnds.Add(floor);
            }
            
         }

         return deadEnds;
      }
   }
}
