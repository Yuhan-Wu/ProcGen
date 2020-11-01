using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ArcherController : UnitController
{
    public override void PerformTurn(GameController p_Context, Unit p_Unit)
    {
        Vector2Int archer_position = p_Unit.CurTile.RoomPosition;
        Vector2Int player_position = p_Context.Player.CurTile.RoomPosition;

        if (Can_shoot(archer_position, player_position))
        {
            Action attack = p_Unit.GetAction("Attack");
            attack.Perform(p_Unit, p_Context.Player, null);
        }
        else
        {

            // Left
            Vector2Int left = new Vector2Int(archer_position.x - 1, archer_position.y);
            Tile curTile = p_Context.CurRoom.GetTileAt(left);
            int distance = 0;
            Vector2Int move_to_pos = archer_position;
            if (CanMoveTo(p_Unit, curTile) && !curTile.IsDoorTile)
            {
                distance = Mathf.Abs(left.x - player_position.x);
                move_to_pos = left;
            }

            // Right
            Vector2Int right = new Vector2Int(archer_position.x + 1, archer_position.y);
            curTile = p_Context.CurRoom.GetTileAt(right);
            if (CanMoveTo(p_Unit, curTile) && !curTile.IsDoorTile)
            {
                if (Mathf.Abs(right.x - player_position.x) < distance)
                {
                    distance = Mathf.Abs(right.x - player_position.x);
                    move_to_pos = right;
                }
            }

            // Up
            Vector2Int up = new Vector2Int(archer_position.x, archer_position.y + 1);
            curTile = p_Context.CurRoom.GetTileAt(up);
            if (CanMoveTo(p_Unit, curTile) && !curTile.IsDoorTile)
            {
                if (Mathf.Abs(up.y - player_position.y) < distance)
                {
                    distance = Mathf.Abs(up.y - player_position.y);
                    move_to_pos = up;
                }
            }

            // Down
            Vector2Int down = new Vector2Int(archer_position.x, archer_position.y - 1);
            curTile = p_Context.CurRoom.GetTileAt(down);
            if (CanMoveTo(p_Unit, curTile) && !curTile.IsDoorTile)
            {
                if (Mathf.Abs(down.y - player_position.y) < distance)
                {
                    distance = Mathf.Abs(down.y - player_position.y);
                    move_to_pos = down;
                }
            }

            curTile = p_Context.CurRoom.GetTileAt(move_to_pos);
            if (move_to_pos == archer_position)
            {
                curTile = p_Context.CurRoom.GetTileAt(left);
                if (!CanMoveTo(p_Unit, curTile) || curTile.IsDoorTile)
                {
                    curTile = p_Context.CurRoom.GetTileAt(right);
                    if (!CanMoveTo(p_Unit, curTile) || curTile.IsDoorTile)
                    {
                        curTile = p_Context.CurRoom.GetTileAt(up);
                        if (!CanMoveTo(p_Unit, curTile) || curTile.IsDoorTile)
                        {
                            curTile = p_Context.CurRoom.GetTileAt(down);
                        }
                    }
                }
            }
            p_Context.MoveUnitToTile(p_Unit, curTile);
        }
    }

    private bool Can_shoot(Vector2Int p_First, Vector2Int p_Second)
    {
        if (p_First.x == p_Second.x && Mathf.Abs(p_First.y - p_Second.y) >= 2) return true;
        if (p_First.y == p_Second.y && Mathf.Abs(p_First.x - p_Second.x) >= 2) return true;
        return false;
    }
}
