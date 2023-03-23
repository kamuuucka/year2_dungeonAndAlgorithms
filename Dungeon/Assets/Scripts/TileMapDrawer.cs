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
    [SerializeField] private TileBase doorOpen;
    [SerializeField] private TileBase doorClosed;

    [SerializeField] private List<TileBase> tilesWalls;

    public List<Vector2> doorPositions = new List<Vector2>();

    //IEnumerable is used as the most generic collection,
    //we don't need any access to the elements except for reading them, so we don't need more advanced collection
    public void PaintTiles(IEnumerable<Vector2> floorPositions)
    {
        foreach (var position in floorPositions)
        {
            Vector3Int tilePosition = tilemap.WorldToCell((Vector3)position);
            tilemap.SetTile(tilePosition, tileFloor);
        }
    }

    public void PaintRooms(IEnumerable<Vector2> floorPositions)
    {
        foreach (var position in floorPositions)
        {
            Vector3Int tilePosition = tilemap.WorldToCell((Vector3)position);
            tilemap.SetTile(tilePosition, null);
            tilemap.SetTile(tilePosition, tileRoom);
        }
    }

    public void PaintWall(Vector2 position, int type)
    {
        Vector3Int wallPosition = tilemap.WorldToCell((Vector3)position);
        if (tilemap.HasTile(wallPosition))
        {
            TileBase thisTile = tilemap.GetTile(wallPosition);
            if (thisTile == tileFloor)
            {
                tilemap.SetTile(wallPosition, null);
                tilemap.SetTile(wallPosition, doorOpen);
                doorPositions.Add(new Vector2((int)wallPosition.x, (int)wallPosition.y));
            }
        }
        else
        {
            tilemap.SetTile(wallPosition, tilesWalls[type]);
        }

        
    }

    public void PaintDoor(Vector2 position)
    {
        Vector3Int tilePosition = tilemap.WorldToCell(position);
        tilemap.SetTile(tilePosition, doorClosed);
    }

    public void Clear()
    {
        tilemap.ClearAllTiles();
    }
}