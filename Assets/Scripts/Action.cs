using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Action : MonoBehaviour
{
    // Execute
    public abstract void Perform(/* context? */);
    
}
