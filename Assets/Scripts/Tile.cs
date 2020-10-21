using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public char Type;

    public Vector2Int RoomPosition;
    public Room ParentRoom;

    public Unit CurUnit;

    public bool IsAdjacentTo(Tile p_Tile)
    {
        if (ParentRoom != p_Tile.ParentRoom) return false;
        Vector3 delta = p_Tile.transform.position - transform.position;
        if ((math.abs(delta.x) == 0 && math.abs(delta.y) == 1) ^ (math.abs(delta.x) == 1 && math.abs(delta.y) == 0))
        {
            return true;
        }
        return false;
    }

    public bool IsFloorTile => Type == 'W';

    public bool IsDoorTile => Type == '0';

    public bool IsLavaTile => Type == 'L';
}
