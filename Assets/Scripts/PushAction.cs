using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushAction : Action
{
    public int Radius = 1;
    private void Start()
    {
        Cost = 3;
    }
    public override void Perform(Unit p_From, Unit p_To, GameController p_Context)
    {
        Tile original_tile = p_From.CurTile;
        Vector2Int original_pos = original_tile.RoomPosition;

        Vector2Int left = original_pos + new Vector2Int(-1, 0);
        Tile left_tile = original_tile.ParentRoom.GetTileAt(left);
        if (CanPush(left_tile)) p_Context.Mark(left_tile);

        Vector2Int right = original_pos + new Vector2Int(1, 0);
        Tile right_tile = original_tile.ParentRoom.GetTileAt(right);
        if (CanPush(right_tile)) p_Context.Mark(right_tile);

        Vector2Int up = original_pos + new Vector2Int(0, 1);
        Tile up_tile = original_tile.ParentRoom.GetTileAt(up);
        if (CanPush(up_tile)) p_Context.Mark(up_tile);

        Vector2Int down = original_pos + new Vector2Int(0, -1);
        Tile down_tile = original_tile.ParentRoom.GetTileAt(down);
        if (CanPush(down_tile)) p_Context.Mark(down_tile);

        p_From.Controller.Movable = false;
    }

    public bool CanPush(Tile p_Tile)
    {
        if(p_Tile) return p_Tile.CurUnit;
        return false;
    }
}
