using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitController : MonoBehaviour
{
    // Archer: get to the same row/column and within proper distance
    // Warrior: get as close to player as possible
    // Player: ProcessPlayerInput()
    public abstract void PerformTurn(GameController p_Context, Unit p_Unit);

    protected virtual bool CanMoveTo(Unit p_Unit, Tile p_Tile)
    {
        if (p_Tile == null || p_Tile.IsLavaTile || !(p_Tile.IsFloorTile || p_Tile.IsDoorTile) || p_Tile.CurUnit != null)
        {
            return false;
        }

        if (p_Tile.IsAdjacentTo(p_Unit.CurTile)) return true;
        return false;
    }
}
