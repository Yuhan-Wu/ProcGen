using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public Tile CurTile;

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
                    //TODO Die
                }
                stat.CurrentValue = p_NewValue;
                return stat;
            }
        }
        return null;
    }

    public Stat SetStatMax(string p_Name, int p_NewValue)
    {
        foreach (Stat stat in Stats)
        {
            if (stat.Name == p_Name)
            {
                stat.MaxValue = p_NewValue;
                return stat;
            }
        }
        return null;
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

    public void TakeTurn(GameController p_Context)
    {
        if (Controller)
        {
            Controller.PerformTurn(p_Context, this);
        }
    }

    public void OnAttack()
    {
        StartCoroutine(Disapear(0.25f));
    }

    IEnumerator Disapear(float p_Time)
    {
        GetComponentInChildren<SpriteRenderer>().enabled = false;
        yield return new WaitForSeconds(p_Time);
        GetComponentInChildren<SpriteRenderer>().enabled = true;
    }

    public UnitController Controller;
    private void Start()
    {
        Controller = GetComponent<UnitController>();
    }
}
