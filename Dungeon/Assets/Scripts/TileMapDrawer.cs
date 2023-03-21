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
    [SerializeField] private TileBase tileRoom;

    [SerializeField] private List<TileBase> tilesWalls;

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
    
    public void PaintRooms(IEnumerable<Vector2Int> floorPositions)
    {
        foreach (var position in floorPositions)
        {
            
            Vector3Int tilePosition = tilemap.WorldToCell((Vector3Int)position);
            tilemap.SetTile(tilePosition, null);
            tilemap.SetTile(tilePosition, tileRoom);
        }
    }

    public void PaintWall(Vector2Int position, int type)
    {
        Vector3Int wallPosition = tilemap.WorldToCell((Vector3Int)position);
        tilemap.SetTile(wallPosition, tilesWalls[type]);
    }

    public void Clear()
    {
        tilemap.ClearAllTiles();
    }
}
