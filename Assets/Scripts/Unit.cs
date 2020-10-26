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
                stat.CurrentValue = p_NewValue;
                return stat;
            }
        }
        return null;
    }

    public List<Action> Actions;

    public void TakeTurn(GameController p_Context)
    {
        UnitController controller = GetComponent<UnitController>();
        if (controller)
        {
            controller.PerformTurn(p_Context, this);
        }
    }
}
