using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitController : MonoBehaviour
{
    // Archer: get to the same row/column and within proper distance
    // Warrior: get as close to player as possible
    // Player: ProcessPlayerInput()
    public abstract void PerformTurn(GameController p_Context, Unit p_Unit);

    protected bool CanMoveTo(Unit p_Unit, Tile p_Tile)
    {
        if (p_Tile == null || p_Tile.IsLavaTile || !(p_Tile.IsFloorTile || p_Tile.IsDoorTile) || p_Tile.CurUnit != null)
        {
            return false;
        }

        if (p_Tile.IsAdjacentTo(p_Unit.CurTile)) return true;
        return false;
    }

    protected void MoveUnitToTile(Unit p_Unit, Tile p_Tile)
    {
        Tile from = null;
        if (p_Unit.CurTile != null)
        {
            from = p_Unit.CurTile;
            p_Unit.CurTile.CurUnit = null;
        }

        p_Unit.CurTile = p_Tile;
        p_Tile.CurUnit = p_Unit;

        StartCoroutine(MoveTo(p_Unit.transform, p_Tile.transform.position, 0.25f));

        // OnUnitEnterTile(p_Unit, p_Tile, from);
    }

    protected IEnumerator MoveTo(Transform p_Target, Vector3 p_To, float p_Duration)
    {
        float time = 0;
        Vector3 start = p_Target.position;
        while (time <= p_Duration)
        {
            float t = Mathf.Clamp01(time / p_Duration);
            p_Target.transform.position = Vector3.Lerp(start, p_To, t);
            yield return new WaitForEndOfFrame();
            time += Time.deltaTime;
        }

        transform.position = p_To;
    }

}
