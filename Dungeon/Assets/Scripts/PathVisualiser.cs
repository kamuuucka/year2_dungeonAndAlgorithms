using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class PathVisualiser : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;

    [SerializeField] private TileBase _tileFloor;

    //IEnumerable is used as the most generic collection,
    //we don't need any access to the elements except for reading them, so we don't need more advanced collection
    public void PaintTiles(IEnumerable<Vector2Int> floorPositions)
    {
        foreach (var position in floorPositions)
        {
            Vector3Int tilePosition = tilemap.WorldToCell((Vector3Int)position);
            tilemap.SetTile(tilePosition, _tileFloor);
        }
    }

    public void Clear()
    {
        tilemap.ClearAllTiles();
    }
}
