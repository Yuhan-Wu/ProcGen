using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Action : MonoBehaviour
{
    public string Name;
    public int Cost;

    // Execute
    public abstract void Perform(Unit p_From, Unit p_To, GameController p_Context);
    
}
