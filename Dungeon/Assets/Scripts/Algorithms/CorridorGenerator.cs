using System.Collections.Generic;
using UnityEngine;

namespace Algorithms
{
   public class CorridorGenerator : MonoBehaviour
   {
      [SerializeField] protected Vector2Int startPosition = Vector2Int.zero;
      [SerializeField] private int corridorLength = 10;
      [SerializeField] private int corridorCount = 10;
      [SerializeField] private float percent;
      [SerializeField] private TileMapDrawer visualiser;
      
      private HashSet<Vector2Int> _corridor = new HashSet<Vector2Int>();
   
      public void Run()
      {
         _corridor = GenerateCorridors();
      }
      
      public void PlaceTiles()
      {
         visualiser.PaintTiles(_corridor);
         GenerateWalls.PlaceWalls(_corridor, visualiser);
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
            floorPositions.UnionWith(corridor);
         }

         return floorPositions;
      }
   }
}
