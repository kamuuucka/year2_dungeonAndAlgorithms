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
        [SerializeField] private int minHeight = 4;
        [SerializeField] private int minWidth = 4;
        [SerializeField] private int dungeonHeight = 20;
        [SerializeField] private int dungeonWidth = 20;

        private HashSet<Vector2Int> _corridor = new HashSet<Vector2Int>();
        private HashSet<Vector2Int> _corridorEnds = new HashSet<Vector2Int>();
        private HashSet<Vector2Int> _roomsCreated = new HashSet<Vector2Int>();
        private HashSet<Vector2Int> _newroomsCreated = new HashSet<Vector2Int>();
        private HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        private HashSet<Vector2Int> _roomsFinal = new HashSet<Vector2Int>();
        [SerializeField] private int offset = 1;

        public HashSet<Vector2Int> Corridor => _corridorEnds;

        public void Run()
        {
            _corridor.Clear();
            _roomsCreated.Clear();
            _newroomsCreated.Clear();
            _corridor = GenerateCorridors();
            _roomsCreated = GenerateRooms();
            _newroomsCreated = GenerateRoomsEnds(FindEnds(_corridor), _roomsCreated);
            var roomList = ProceduralGenerationAlgorithm.BSP(
                new BoundsInt(new Vector3Int(0, 0, 0), new Vector3Int(dungeonWidth, dungeonHeight, 0)),
                minWidth, minHeight);
            floor = CreateSimpleRooms(roomList, _roomsCreated, _newroomsCreated);
        }

        private void SetRoomsCenter(Vector2Int room)
        {
            room.x = 0;
            room.y = 0;
            room.Set(0, 0);
        }

        private HashSet<Vector2Int> CreateSimpleRooms(List<BoundsInt> roomList, HashSet<Vector2Int> roomsPositions, HashSet<Vector2Int> endPositions)
        {
            Debug.Log("expected rooms 1: " + roomsPositions.Count + endPositions.Count);
            //TODO: offset is not working correctly? Rooms are still spawning connected
            HashSet<Vector2Int> roomFloor = new HashSet<Vector2Int>();

            roomsPositions.UnionWith(endPositions);
            List<Vector2Int> positions = roomsPositions.ToList();
            Debug.Log("expected rooms 2: " + positions.Count);

            for (int i = 0; i < roomList.Count; i++)
            {
                if (i < positions.Count)
                {
                    BoundsInt room = roomList[i];
                    var boundsInt1 = room;
                    boundsInt1.position = new Vector3Int(positions[i].x - boundsInt1.size.x/2, positions[i].y- boundsInt1.size.y/2, 0);
                    Debug.Log(i + "\t" + boundsInt1.position + "\t" + boundsInt1.min + "\t" + boundsInt1.size + "\t" + boundsInt1.max );

                    for (int col = boundsInt1.min.x; col < boundsInt1.max.x; col++)
                    {
                        for (int row = boundsInt1.min.y; row < boundsInt1.max.y; row++)
                        {
                            Vector2Int position = new Vector2Int(col, row);
                            roomFloor.Add(position);
                        }
                    }
                }
            }

            return roomFloor;
        }


        public void PlaceTiles()
        {
            visualiser.PaintTiles(_corridor);
            GenerateWalls.PlaceWalls(_corridor, visualiser);
            visualiser.PaintRooms(_roomsCreated);
            visualiser.PaintRooms(_newroomsCreated);
            visualiser.PaintTiles(floor);
            GenerateWalls.PlaceWalls(floor, visualiser);
        }

        public void Clear()
        {
            visualiser.Clear();
        }

        private HashSet<Vector2Int> GenerateCorridors()
        {
            _corridorEnds.Clear();
            HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
            Vector2Int currentPosition = startPosition;
            for (int i = 0; i < corridorCount; i++)
            {
                List<Vector2Int> corridor =
                    ProceduralGenerationAlgorithm.SimpleRandomWalkCorridor(currentPosition, corridorLength);
                currentPosition = corridor[^1]; //last one
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
            List<Vector2Int> rooms = new List<Vector2Int>(possibleRoomPositions);
            rooms = possibleRoomPositions.OrderBy(x => Guid.NewGuid()).Take(possibleRoomPositions.Count)
                .ToList();
            possibleRoomPositions.Clear();
            for (int i = 0; i < roomsToBeCreated; i++)
            {
                possibleRoomPositions.Add(rooms[i]);
                Debug.Log("Room can be spawned at: " + rooms[i]);
            }

            Debug.Log("Rooms positions: " + possibleRoomPositions.Count);
            return possibleRoomPositions;
        }

        private HashSet<Vector2Int> GenerateRoomsEnds(HashSet<Vector2Int> deadEnds,
            HashSet<Vector2Int> possibleRooms)
        {
            HashSet<Vector2Int> newRooms = new HashSet<Vector2Int>();
            foreach (var end in deadEnds)
            {
                if (!possibleRooms.Contains(end))
                {
                    newRooms.Add(end);
                }
            }
            Debug.Log("Rooms end: " + newRooms.Count);
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

                if (neighbours == 1) //if has only one neighbour = dead end
                {
                    deadEnds.Add(floor);
                }
            }

            return deadEnds;
        }
    }
}