using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Algorithms
{
    public class SimpleRandomWalk : MonoBehaviour
    {
        [SerializeField] protected Vector2 startPosition = Vector2.zero;
    
    
        [SerializeField] private string filename;
        [SerializeField] private int iterations = 10;
        [SerializeField] private int walkLength = 10;
        [SerializeField] private bool startRandomly = true;
        [SerializeField] private TileMapDrawer visualiser;

        public int Iterations => iterations;
        public int WalkLength => walkLength;
        public bool StartRandomly => startRandomly;
        public string FileName => filename;

        private Vector2 _currentPosition;
        private HashSet<Vector2> _floor = new HashSet<Vector2>();


        public void Run()
        {
            _floor = RunRandomWalk();

        }

        public void PlaceTiles()
        {
            visualiser.PaintTiles(_floor);
            GenerateWalls.PlaceWalls(_floor, visualiser);
        }

        private void OnDrawGizmosSelected()
        {
            Vector2 currentPoint;
            Vector2 nextPoint;
            for (int i = 0; i < _floor.Count-1; i++)
            {
                currentPoint = _floor.ElementAt(i);
                nextPoint = _floor.ElementAt(i + 1);
                if (_floor.ElementAt(i+1).Equals(new Vector2(100, 100)))
                {
                    nextPoint = _floor.ElementAt(i);
                } else if (_floor.ElementAt(i).Equals(new Vector2(100, 100)))
                {
                    currentPoint = _floor.ElementAt(i + 1);
                }
            
                Gizmos.DrawLine(new Vector3(currentPoint.x, currentPoint.y),
                    new Vector3(nextPoint.x, nextPoint.y));
            }
        }

        public void Clear()
        {
            visualiser.Clear();
        }

        private HashSet<Vector2> RunRandomWalk()
        {
            _currentPosition = startPosition;
            HashSet<Vector2> floorPositions = new HashSet<Vector2>();

            for (int i = 0; i < iterations; i++)
            {
                HashSet<Vector2> path = ProceduralGenerationAlgorithm.SimpleRandomWalk(_currentPosition, walkLength);
                // Debug.Log("New path");
                // foreach (var tile in path)
                // {
                //     Debug.Log(tile.ToString());
                // }
                floorPositions.UnionWith(path);     //Add values from path to floorPositions while making sure that there are no duplicates
                //floorPositions.Add(new Vector2(100, 100));
                if (startRandomly)
                {
                
                    _currentPosition = floorPositions.ElementAt(Random.Range(0, floorPositions.Count));
                }
            }

            return floorPositions;
        }
    }
}
