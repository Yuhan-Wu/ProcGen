using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : UnitController
{
    public Action ActiveAction = null;
    public ButtonController ButnController;

    public override void PerformTurn(GameController p_Context, Unit p_Unit)
    {
        p_Context.UnMark();
        ActiveAction.Perform(p_Unit, null, p_Context);
        ProcessPlayerInput(p_Context, p_Unit);
    }

    public void SetActiveAction(string p_Name)
    {
        Unit unit = GetComponent<Unit>();
        foreach(Action action in unit.Actions)
        {
            if(action.Name == p_Name)
            {
                ActiveAction = action;
                break;
            }
        }
    }

    private void ProcessPlayerInput(GameController p_Context, Unit p_Unit)
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            p_Unit.SetStat("Health", p_Unit.GetStat("Health").MaxValue);
            p_Unit.SetStat("Mana", p_Unit.GetStat("Mana").MaxValue);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            p_Context.DestroyAllEnemies();
        }
        Ray ray = p_Context.MainCamera.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.forward, Vector3.zero);
        float d;
        if (plane.Raycast(ray, out d))
        {
            Vector3 worldPos = ray.GetPoint(d);
            Vector3 localPos = worldPos - p_Context.CurRoom.transform.position;
            Vector2Int roomPos = new Vector2Int((int)localPos.x, (int)localPos.y);
            Tile tile = p_Context.CurRoom.GetTileAt(roomPos);
            p_Context.Highlight(tile);

            if (tile && Input.GetMouseButtonDown(0) && (tile.Marked || tile.IsDoorTile))
            {
                p_Context.MoveUnitToTile(p_Unit, tile);
                p_Context.PlayerTurn = false;
                p_Context.UnMark();
                ButnController.ActivateActions();
                p_Unit.SetStat("Mana", p_Unit.GetStat("Mana").CurrentValue - ActiveAction.Cost);
                SetActiveAction("Walk");
            }
        }
    }
}
