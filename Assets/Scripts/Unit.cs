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
        // TODO
        return Stats[0];
    }

    public Stat SetStat(string p_Name, int p_NewValue)
    {
        // TODO
        return Stats[0];
    }

    public List<Action> Actions;
}
