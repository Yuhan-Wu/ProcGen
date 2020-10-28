using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpPanelController : MonoBehaviour
{
    public PlayerController Player;
    public void Close()
    {
        GetComponentInParent<ButtonController>().ActivateActions();
        this.gameObject.SetActive(false);
        Player.DisableAction = false;
    }

    public void DisableActions()
    {
        GetComponentInParent<ButtonController>().DeactiveAll();
        Player.DisableAction = true;
    }
}
