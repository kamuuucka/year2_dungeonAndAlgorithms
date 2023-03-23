using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Algorithms
{
    public class CorridorGenerator : MonoBehaviour
    {
        [SerializeField] protected Vector2 startPosition = Vector2.zero;
        [SerializeField] private int corridorLength = 10;
        [SerializeField] private int corridorCount = 10;
        [SerializeField] private float percent = 0.8f;
        [SerializeField] private TileMapDrawer visualiser;
        [SerializeField] private int minHeight = 4;
        [SerializeField] private int minWidth = 4;
        [SerializeField] private int dungeonHeight = 20;
        [SerializeField] private int dungeonWidth = 20;

        private HashSet<Vector2> _corridor = new HashSet<Vector2>();
        private HashSet<Vector2> _corridorEnds = new HashSet<Vector2>();
        private HashSet<Vector2> _roomsCreated = new HashSet<Vector2>();
        private HashSet<Vector2> _newroomsCreated = new HashSet<Vector2>();
        private HashSet<Vector2> floor = new HashSet<Vector2>();
        private HashSet<Vector2> _roomsFinal = new HashSet<Vector2>();
        private List<Vector2> positions = new List<Vector2>();
        private Dictionary<Vector2, Vector2> doors = new Dictionary<Vector2, Vector2>();
        [SerializeField] private int offset = 1;

        public HashSet<Vector2> Corridor => _corridorEnds;

        public void Run()
        {
            visualiser.doorPositions.Clear();
            _corridor.Clear();
            _roomsCreated.Clear();
            _newroomsCreated.Clear();
            positions.Clear();
            doors.Clear();
            _corridor = GenerateCorridors();
            _roomsCreated = GenerateRooms();
            _newroomsCreated = GenerateRoomsEnds(FindEnds(_corridor), _roomsCreated);
            var roomList = ProceduralGenerationAlgorithm.BSP(
                new BoundsInt(new Vector3Int(0, 0, 0), new Vector3Int(dungeonWidth, dungeonHeight, 0)),
                minWidth, minHeight);
            floor = CreateSimpleRooms(roomList, _roomsCreated, _newroomsCreated);
            
            Debug.Log("Doors in run: " + doors.Count);
        }

        private HashSet<Vector2> CreateSimpleRooms(List<BoundsInt> roomList, HashSet<Vector2> roomsPositions, HashSet<Vector2> endPositions)
        {
            //Debug.Log("expected rooms 1: " + roomsPositions.Count + endPositions.Count);
            HashSet<Vector2> roomFloor = new HashSet<Vector2>();

            roomsPositions.UnionWith(endPositions);
            positions = roomsPositions.ToList();
            //Debug.Log("expected rooms 2: " + positions.Count);

            for (int i = 0; i < roomList.Count; i++)
            {
                if (i < positions.Count)
                {
                    BoundsInt room = roomList[i];
                    var boundsInt1 = room;
                    boundsInt1.position = new Vector3Int((int)positions[i].x - boundsInt1.size.x/2, (int)positions[i].y- boundsInt1.size.y/2, 0);
                    //Debug.Log(i + "\t" + boundsInt1.position + "\t" + boundsInt1.min + "\t" + boundsInt1.size + "\t" + boundsInt1.max );

                    for (int col = boundsInt1.min.x; col < boundsInt1.max.x; col++)
                    {
                        for (int row = boundsInt1.min.y; row < boundsInt1.max.y; row++)
                        {
                            Vector2 position = new Vector2(col, row);
                            roomFloor.Add(position);
                        }
                    }
                }
            }
            return roomFloor;
        }


        public void PlaceTiles()
        {
            //doors.Clear();
            visualiser.PaintTiles(_corridor);
            Debug.Log("Corridor walls");
            GenerateWalls.PlaceWalls(_corridor, visualiser);
            visualiser.PaintTiles(floor);
            Debug.Log("Room walls");
            GenerateWalls.PlaceWalls(floor, visualiser);
            Debug.Log(visualiser.doorPositions.Count);
            doors = FindDoors(positions, visualiser.doorPositions);
            Debug.Log(doors.Count);
            for (int i = 0; i < doors.Count; i++)
            {
                if (i % 3 == 0)
                {
                    visualiser.PaintDoor(doors.ElementAt(i).Key);
                    float roomDist = Vector2.Distance(doors.ElementAt(i).Value, startPosition);
                    float doorDist = Vector2.Distance(doors.ElementAt(i).Key, startPosition);
                    if (roomDist < doorDist)
                    {
                        visualiser.PaintDoor(doors.ElementAt(i).Value);
                    }
                }
                
            }
        }

        private void OnDrawGizmosSelected()
        {
            foreach (var doorPair in doors)
            {
                Gizmos.DrawLine(new Vector3(doorPair.Key.x, doorPair.Key.y,0), new Vector3(doorPair.Value.x, doorPair.Value.y,0));
            }
        }

        public void Clear()
        {
            visualiser.Clear();
        }

        private Dictionary<Vector2, Vector2> FindDoors(List<Vector2> roomsPositions, List<Vector2> doorPositions)
        {
            Dictionary<Vector2, Vector2> doorsAndCenters = new Dictionary<Vector2, Vector2>();
            Debug.Log("DOORS FINDER");
            Debug.Log(roomsPositions.Count);
            Debug.Log(doorPositions.Count);
            
            
            foreach (var door in doorPositions)
            {
                Vector2 roomCenter = Vector2.zero;
                float closest = Mathf.Infinity;
                Vector2 currentPosition = door;
                foreach (var room in roomsPositions)
                {
                    float distance = Vector2.Distance(room, currentPosition);
                    if (distance < closest)
                    {
                        
                        closest = distance;
                        roomCenter = room;
                    }
                }
                doorsAndCenters.Add(door,new Vector2((int)roomCenter.x, (int)roomCenter.y));
            }

            return doorsAndCenters;
        }
        

        private HashSet<Vector2> GenerateCorridors()
        {
            _corridorEnds.Clear();
            HashSet<Vector2> floorPositions = new HashSet<Vector2>();
            Vector2 currentPosition = startPosition;
            for (int i = 0; i < corridorCount; i++)
            {
                List<Vector2> corridor =
                    ProceduralGenerationAlgorithm.SimpleRandomWalkCorridor(currentPosition, corridorLength);
                currentPosition = corridor[^1]; //last one
                _corridorEnds.Add(currentPosition);
                floorPositions.UnionWith(corridor);
            }

            return floorPositions;
        }

        //TODO: SOMEHOW move it to another script (maybe SO?)
        private HashSet<Vector2> GenerateRooms()
        {
            HashSet<Vector2> possibleRoomPositions = _corridorEnds;
            int roomsToBeCreated = Mathf.RoundToInt(possibleRoomPositions.Count * percent);
            List<Vector2> rooms = new List<Vector2>(possibleRoomPositions);
            //rooms = possibleRoomPositions.OrderBy(x => Guid.NewGuid()).Take(possibleRoomPositions.Count)
                //.ToList();
            possibleRoomPositions.Clear();
            for (int i = 0; i < roomsToBeCreated; i++)
            {
                possibleRoomPositions.Add(rooms[i]);
                //Debug.Log("Room can be spawned at: " + rooms[i]);
            }

            //Debug.Log("Rooms positions: " + possibleRoomPositions.Count);
            return possibleRoomPositions;
        }

        private HashSet<Vector2> GenerateRoomsEnds(HashSet<Vector2> deadEnds,
            HashSet<Vector2> possibleRooms)
        {
            HashSet<Vector2> newRooms = new HashSet<Vector2>();
            foreach (var end in deadEnds)
            {
                if (!possibleRooms.Contains(end))
                {
                    newRooms.Add(end);
                }
            }
            //Debug.Log("Rooms end: " + newRooms.Count);
            return newRooms;
        }

        private HashSet<Vector2> FindEnds(HashSet<Vector2> floorPositions)
        {
            HashSet<Vector2> deadEnds = new HashSet<Vector2>();
            foreach (Vector2 floor in floorPositions)
            {
                int neighbours = 0;

                foreach (Vector2 direction in DirectionsClass.GetDirections())
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