using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class TileMapDrawer : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;

    [SerializeField] private TileBase tileFloor;
    [SerializeField] private TileBase tileWall;

    //IEnumerable is used as the most generic collection,
    //we don't need any access to the elements except for reading them, so we don't need more advanced collection
    public void PaintTiles(IEnumerable<Vector2Int> floorPositions)
    {
        foreach (var position in floorPositions)
        {
            Vector3Int tilePosition = tilemap.WorldToCell((Vector3Int)position);
            tilemap.SetTile(tilePosition, tileFloor);
        }
    }

    public void PaintWall(Vector2Int position)
    {
        Vector3Int wallPosition = tilemap.WorldToCell((Vector3Int)position);
        tilemap.SetTile(wallPosition, tileWall);
    }

    public void Clear()
    {
        tilemap.ClearAllTiles();
    }
}
