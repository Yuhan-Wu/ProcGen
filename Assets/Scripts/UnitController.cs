using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitController : MonoBehaviour
{
    // Archer: get to the same row/column and within proper distance
    // Warrior: get as close to player as possible
    // Player: ProcessPlayerInput()
    public abstract void PerformTurn(/* context? */);
}
