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
            _corridor = GenerateCorridors();
            _roomsCreated = GenerateRooms();
            _newroomsCreated = GenerateRoomsEnds(FindEnds(_corridor), _roomsCreated);
            var roomList = ProceduralGenerationAlgorithm.BSP(
                new BoundsInt(new Vector3Int(100, 100, 0), new Vector3Int(dungeonWidth, dungeonHeight, 0)),
                minWidth, minHeight);
            floor = CreateSimpleRooms(roomList, _roomsCreated, _newroomsCreated);

            // floor.ElementAt(1).Set(0,0);
            // foreach (var room in floor)
            // {
            //    SetRoomsCenter(room);
            // }
            // SetRoomsCenter(floor.ElementAt(1));
            // Debug.Log($"Room at 0,0: {floor.ElementAt(1).x},{floor.ElementAt(1).y}");

            //_roomsFinal = PutRoomsOnPossible();
        }

        // private HashSet<Vector2Int> PutRoomsOnPossible()
        // {
        //    List<Vector2Int> rooms = floor.ToList();
        //    for (int i =0; i < _newroomsCreated.Count; i++)
        //    {
        //    }
        //    
        // }

        private void SetRoomsCenter(Vector2Int room)
        {
            room.x = 0;
            room.y = 0;
            room.Set(0, 0);
        }

        private HashSet<Vector2Int> CreateSimpleRooms(List<BoundsInt> roomList, HashSet<Vector2Int> roomsPositions, HashSet<Vector2Int> endPositions)
        {
            //TODO: offset is not working correctly? Rooms are still spawning connected
            Debug.Log(roomList.Count);
            Debug.Log(roomsPositions.Count);
            HashSet<Vector2Int> roomFloor = new HashSet<Vector2Int>();

            List<Vector2Int> positions = roomsPositions.ToList();
            positions.AddRange(endPositions.ToList());

            for (int i = 0; i < roomList.Count; i++)
            {
                if (i < positions.Count)
                {
                    BoundsInt room = roomList[i];
                    var boundsInt1 = room;
                    boundsInt1.position = new Vector3Int(positions[i].x, positions[i].y, 0);

                    for (int col = offset; col < boundsInt1.size.x - offset; col++)
                    {
                        for (int row = offset; row < boundsInt1.size.y - offset; row++)
                        {
                            Vector2Int position = (Vector2Int)boundsInt1.min + new Vector2Int(row, col);
                            position.x -= boundsInt1.size.x / 2;
                            position.y -= boundsInt1.size.y / 2;
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
                Debug.Log(rooms[i]);
            }


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