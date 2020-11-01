using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : UnitController
{
    public Action ActiveAction = null;
    public ButtonController ButnController;

    public bool CanThrow = true;
    public GameObject Sword;

    public bool DisableAction = false;

    public GameObject Win;
    public GameObject Lose;

    public override void PerformTurn(GameController p_Context, Unit p_Unit)
    {
        if (GetComponent<Unit>().GetStat("Health").CurrentValue <= 0)
        {
            Lose.SetActive(true);
            ButnController.DeactiveAll();
            p_Context.Over = true;
            return;
        }
        if (!DisableAction)
        {
            p_Context.UnMark();
            ActiveAction.Perform(p_Unit, null, p_Context);
            ProcessPlayerInput(p_Context, p_Unit);
        }
        if(GetComponent<Unit>().CurTile.Type == 'V')
        {
            Win.SetActive(true);
            ButnController.DeactiveAll();
            p_Context.Over = true;
        }
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
                if (Movable)
                {
                    if (CanThrow)
                    {
                        Vector2Int direction = tile.RoomPosition - p_Unit.CurTile.RoomPosition;
                        if (direction.x > 0)
                        {
                            Tile right = tile.ParentRoom.GetTileAt(tile.RoomPosition + new Vector2Int(1, 0));
                            if (right && right.CurUnit)
                                right.CurUnit.OnAttack();
                        }
                        else if (direction.x < 0)
                        {
                            Tile left = tile.ParentRoom.GetTileAt(tile.RoomPosition + new Vector2Int(-1, 0));
                            if (left && left.CurUnit) left.CurUnit.OnAttack();
                        }
                        else if (direction.y > 0)
                        {
                            Tile up = tile.ParentRoom.GetTileAt(tile.RoomPosition + new Vector2Int(0, 1));
                            if (up && up.CurUnit) up.CurUnit.OnAttack();
                        }
                        else
                        {
                            Tile down = tile.ParentRoom.GetTileAt(tile.RoomPosition + new Vector2Int(0, -1));
                            if (down && down.CurUnit) down.CurUnit.OnAttack();
                        }
                    }
                    
                    if (tile.HasSword)
                    {
                        CanThrow = true;
                        tile.HasSword = false;
                        if (tile.PickUp) Destroy(tile.PickUp);
                        tile.PickUp = null;
                    }
                    
                    p_Context.MoveUnitToTile(p_Unit, tile);
                }
                else if (ActiveAction.Name == "ThrowSpear" && CanThrow)
                {
                    GameObject cur_sword = Instantiate(Sword);
                    cur_sword.transform.position = p_Unit.transform.position;
                    Vector3 to_position = tile.transform.position;
                    StartCoroutine(MoveSword(cur_sword.transform, to_position, 0.2f));
                    tile.CurUnit.OnAttack();
                    tile.HasSword = true;
                    tile.PickUp = cur_sword;
                    CanThrow = false;
                    Movable = true;
                }
                else
                {
                    Unit enemy_unit = tile.CurUnit;
                    Vector2Int direction = tile.RoomPosition - p_Unit.CurTile.RoomPosition;
                    
                    if (direction.x > 0)
                    {
                        Tile right = tile.ParentRoom.GetTileAt(tile.RoomPosition + new Vector2Int(1, 0));
                        p_Context.MoveUnitToTile(enemy_unit, right);
                    }
                    else if (direction.x < 0)
                    {
                        Tile left = tile.ParentRoom.GetTileAt(tile.RoomPosition + new Vector2Int(-1, 0));
                        p_Context.MoveUnitToTile(enemy_unit, left);
                    }
                    else if (direction.y > 0)
                    {
                        Tile up = tile.ParentRoom.GetTileAt(tile.RoomPosition + new Vector2Int(0, 1));
                        p_Context.MoveUnitToTile(enemy_unit, up);
                    }
                    else
                    {
                        Tile down = tile.ParentRoom.GetTileAt(tile.RoomPosition + new Vector2Int(0, -1));
                        p_Context.MoveUnitToTile(enemy_unit, down);
                    }
                    Movable = true;
                }
                p_Context.PlayerTurn = false;
                p_Context.UnMark();
                p_Unit.SetStat("Mana", p_Unit.GetStat("Mana").CurrentValue - ActiveAction.Cost);
                ButnController.ActivateActions();
                SetActiveAction("Walk");
            }
        }
    }

    private IEnumerator MoveSword(Transform p_Target, Vector3 p_To, float p_Duration)
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
    }
}
