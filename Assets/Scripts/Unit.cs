using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public Tile CurTile;
    public GameController Context;

    [Serializable]
    public class Stat
    { 
        public string Name;
        public int CurrentValue;
        public int MaxValue;
    }

    public List<Stat> Stats = new List<Stat>();

    public Stat GetStat(string p_Name)
    {
        foreach(Stat stat in Stats)
        {
            if (stat.Name == p_Name) return stat;
        }
        return null;
    }

    public Stat SetStat(string p_Name, int p_NewValue)
    {
        foreach (Stat stat in Stats)
        {
            if (stat.Name == p_Name)
            {
                if(stat.Name == "Health" && p_NewValue == 0)
                {
                    if(this != Context.Player)
                    {
                        Context.Enemies.Remove(this);
                        this.CurTile.CurUnit = null;
                        Die();
                    }
                }
                stat.CurrentValue = p_NewValue;
                return stat;
            }
        }
        return null;
    }

    public void SetStatMax(string p_Name, int p_NewValue)
    {
        foreach (Stat stat in Stats)
        {
            if (stat.Name == p_Name)
            {
                stat.MaxValue = p_NewValue;
            }
        }
    }

    public void IncreaseStatMax(string p_Name)
    {
        foreach (Stat stat in Stats)
        {
            if (stat.Name == p_Name)
            {
                stat.MaxValue++;
            }
        }
    }

    public void ResetStat(string p_Name)
    {
        foreach (Stat stat in Stats)
        {
            if (stat.Name == p_Name)
            {
                stat.CurrentValue = stat.MaxValue;
            }
        }
    }

    public List<Action> Actions;

    public Action GetAction(string p_Name)
    {
        foreach(Action action in Actions)
        {
            if (action.Name == p_Name)
            {
                return action;
            }
        }
        return null;
    }

    public void TakeTurn()
    {
        if (Controller)
        {
            Controller.PerformTurn(Context, this);
        }
    }

    public void OnAttack()
    {
        int cur = GetStat("Health").CurrentValue;
        cur = cur - 1;
        SetStat("Health", cur);
        StartCoroutine(Disapear(0.25f));
    }

    IEnumerator Disapear(float p_Time)
    {
        GetComponentInChildren<SpriteRenderer>().enabled = false;
        yield return new WaitForSeconds(p_Time);
        GetComponentInChildren<SpriteRenderer>().enabled = true;
    }

    public void Die()
    {
        Destroy(this.gameObject);
    }

    public UnitController Controller;
    private void Start()
    {
        Controller = GetComponent<UnitController>();
    }
}
