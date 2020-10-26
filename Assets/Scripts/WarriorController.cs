using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorController : UnitController
{
    public override void PerformTurn(GameController p_Context, Unit p_Unit)
    {
        Vector2Int warrior_position = p_Unit.CurTile.RoomPosition;
        Vector2Int player_position = p_Context.Player.CurTile.RoomPosition;

        int distance = calculate_distance(warrior_position, player_position);
        if (distance == 1)
        {
            // TODO attack
        }
        else
        {
            Vector2Int move_to_pos = warrior_position;

            // Left
            Vector2Int left = new Vector2Int(warrior_position.x-1, warrior_position.y);
            Tile curTile = p_Context.CurRoom.GetTileAt(left);
            if(CanMoveTo(p_Unit, curTile) && !curTile.IsDoorTile)
            {
                if(calculate_distance(left, player_position) < distance)
                {
                    distance = calculate_distance(left, player_position);
                    move_to_pos = left;
                }
            }

            // Right
            Vector2Int right = new Vector2Int(warrior_position.x + 1, warrior_position.y);
            curTile = p_Context.CurRoom.GetTileAt(right);
            if (CanMoveTo(p_Unit, curTile) && !curTile.IsDoorTile)
            {
                if (calculate_distance(right, player_position) < distance)
                {
                    distance = calculate_distance(right, player_position);
                    move_to_pos = right;
                }
            }

            // Up
            Vector2Int up = new Vector2Int(warrior_position.x, warrior_position.y + 1);
            curTile = p_Context.CurRoom.GetTileAt(up);
            if (CanMoveTo(p_Unit, curTile) && !curTile.IsDoorTile)
            {
                if (calculate_distance(up, player_position) < distance)
                {
                    distance = calculate_distance(up, player_position);
                    move_to_pos = up;
                }
            }

            // Down
            Vector2Int down = new Vector2Int(warrior_position.x, warrior_position.y - 1);
            curTile = p_Context.CurRoom.GetTileAt(down);
            if (CanMoveTo(p_Unit, curTile) && !curTile.IsDoorTile)
            {
                if (calculate_distance(down, player_position) < distance)
                {
                    distance = calculate_distance(down, player_position);
                    move_to_pos = down;
                }
            }

            curTile = p_Context.CurRoom.GetTileAt(move_to_pos);
            MoveUnitToTile(p_Unit, curTile);
        }
    }

    private int calculate_distance(Vector2Int p_First, Vector2Int p_Second)
    {
        Vector2Int distance = p_First - p_Second;
        return Mathf.Abs(distance.x) + Mathf.Abs(distance.y);
    }
}
